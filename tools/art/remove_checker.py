"""Remove a fake transparency checkerboard (or plain light background) baked into AI-generated icons.

ChatGPT sometimes paints the grey/white checkerboard instead of real alpha. This flood-fills from the
image border over light, low-saturation pixels and makes them transparent; the ink outline of the object
stops the fill. Images that already have real transparency are skipped.

    py tools/art/remove_checker.py <file-or-folder> [--backup DIR] [--tolerance 40]
"""
import argparse
import shutil
from collections import deque
from pathlib import Path

from PIL import Image, ImageFilter


def is_background(rgb, tol):
    r, g, b = rgb[:3]
    lo, hi = min(r, g, b), max(r, g, b)
    return lo >= 255 - tol * 2.5 and hi - lo <= tol // 2


def fill(px, mp, w, h, queue, tol, mark):
    """Flood-fill background-like pixels from the queued seeds, marking them with `mark`; returns them."""
    filled = []
    while queue:
        x, y = queue.popleft()
        if mp[x, y] or not is_background(px[x, y], tol):
            continue
        mp[x, y] = mark
        filled.append((x, y))
        if x > 0: queue.append((x - 1, y))
        if x < w - 1: queue.append((x + 1, y))
        if y > 0: queue.append((x, y - 1))
        if y < h - 1: queue.append((x, y + 1))
    return filled


def already_transparent(im):
    if im.mode != "RGBA":
        return False
    alpha = im.getchannel("A")
    w, h = im.size
    corners = [alpha.getpixel((0, 0)), alpha.getpixel((w - 1, 0)), alpha.getpixel((0, h - 1)), alpha.getpixel((w - 1, h - 1))]
    return all(a < 10 for a in corners)


def process(path, tol, backup, min_pocket):
    im = Image.open(path)
    if already_transparent(im):
        print(f"skip (already transparent): {path.name}")
        return
    if backup:
        backup.mkdir(parents=True, exist_ok=True)
        shutil.copy2(path, backup / path.name)
    im = im.convert("RGBA")
    w, h = im.size
    px = im.load()
    mask = Image.new("L", (w, h), 0)
    mp = mask.load()
    queue = deque()
    for x in range(w):
        queue.append((x, 0))
        queue.append((x, h - 1))
    for y in range(h):
        queue.append((0, y))
        queue.append((w - 1, y))
    fill(px, mp, w, h, queue, tol, 255)
    # Second pass: enclosed pockets (between blade and haft, inside tassels...) are not reachable
    # from the border. Remove a pocket only if it looks like the checkerboard: big enough and made of
    # both near-white and light-grey pixels.
    for y in range(h):
        for x in range(w):
            if mp[x, y] or not is_background(px[x, y], tol):
                continue
            pocket = fill(px, mp, w, h, deque([(x, y)]), tol, 1)
            bright = sum(1 for p in pocket if min(px[p][:3]) >= 245)
            if len(pocket) >= min_pocket and 0.15 <= bright / len(pocket) <= 0.85:
                for p in pocket:
                    mp[p] = 255
    for y in range(h):
        for x in range(w):
            if mp[x, y] == 1:
                mp[x, y] = 0
    # Grow the removed area by 1px to eat the light fringe, then soften the edge.
    mask = mask.filter(ImageFilter.MaxFilter(3)).filter(ImageFilter.GaussianBlur(0.7))
    im.putalpha(mask.point(lambda m: 255 - m))
    im.save(path)
    removed = sum(1 for v in mask.get_flattened_data() if v > 128) * 100 // (w * h)
    print(f"done: {path.name}  background removed {removed}%")


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("target")
    ap.add_argument("--backup")
    ap.add_argument("--tolerance", type=int, default=40)
    ap.add_argument("--min-pocket", type=int, default=300, help="min pixels of an enclosed checker pocket to remove")
    a = ap.parse_args()
    target = Path(a.target)
    files = sorted(target.glob("*.png")) if target.is_dir() else [target]
    for f in files:
        process(f, a.tolerance, Path(a.backup) if a.backup else None, a.min_pocket)


if __name__ == "__main__":
    main()
