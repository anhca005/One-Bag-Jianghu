"""Give AI-generated VFX sprites real transparency, with soft ink edges and no white/grey halo.

ChatGPT usually returns VFX as an opaque image with a fake transparency checkerboard (or plain white)
painted in. remove_checker.py (made for hard-outlined icons) cuts that with a hard flood fill, which leaves
a light fringe around soft ink washes and dry-brush strokes. This tool instead:

1. finds the background: light, neutral pixels connected to the border, plus big enclosed pockets that
   look like the checkerboard (both near-white and light-grey pixels, e.g. the hole inside a ring);
2. estimates the background color *under every pixel* from the nearest background pixel (so each checker
   square gets its own white or grey);
3. in a thin band around the background, converts "color to alpha" against that local color, so
   semi-transparent ink edges become semi-transparent pixels; everything deeper inside stays opaque
   (rice-paper highlights are kept).

Images that already have real transparency are skipped.

    py tools/art/ink_to_alpha.py <file-or-folder> --backup DIR [--band 6] [--hole 1500]
"""
import argparse
import shutil
from pathlib import Path

import numpy as np
from PIL import Image
from scipy import ndimage

LIGHT_MIN = 196   # min(r, g, b) of a background pixel (checker grey is ~205-222, white ~250)
NEUTRAL_MAX = 14  # max(r, g, b) - min(r, g, b) of a background pixel


def already_transparent(im):
    if im.mode not in ("RGBA", "LA") and "transparency" not in im.info:
        return False
    alpha = np.asarray(im.convert("RGBA"))[..., 3]
    return (alpha < 250).mean() > 0.02


def background_region(rgb, hole):
    lo = rgb.min(axis=2)
    candidate = (lo >= LIGHT_MIN) & ((rgb.max(axis=2) - lo) <= NEUTRAL_MAX)
    labels, count = ndimage.label(candidate)
    if count == 0:
        return candidate
    idx = np.arange(1, count + 1)
    sizes = ndimage.sum(candidate, labels, idx)
    bright = ndimage.sum(lo >= 245, labels, idx)
    border = set(np.unique(np.concatenate([labels[0], labels[-1], labels[:, 0], labels[:, -1]]))) - {0}
    keep = np.zeros(count + 1, dtype=bool)
    for i in idx:
        share = bright[i - 1] / sizes[i - 1]
        keep[i] = i in border or (sizes[i - 1] >= hole and 0.15 <= share <= 0.85)
    return keep[labels]


def process(path, backup, band, hole):
    im = Image.open(path)
    if already_transparent(im):
        print(f"skip (already transparent): {path.name}")
        return
    if backup:
        backup.mkdir(parents=True, exist_ok=True)
        shutil.copy2(path, backup / path.name)
    raw = np.asarray(im.convert("RGB"))
    rgb = raw.astype(np.float32) / 255.0
    region = background_region(raw, hole)

    # Local background color = color of the nearest background pixel.
    _, (iy, ix) = ndimage.distance_transform_edt(~region, return_indices=True)
    bg = rgb[iy, ix]

    # Color to alpha against the local background (GIMP style, per channel, then max).
    below = np.where(rgb < bg, (bg - rgb) / np.maximum(bg, 1e-6), 0.0)
    above = np.where(rgb > bg, (rgb - bg) / np.maximum(1.0 - bg, 1e-6), 0.0)
    alpha = np.clip(np.maximum(below, above).max(axis=2), 0.0, 1.0)
    safe = np.maximum(alpha, 1e-6)[..., None]
    unmixed = np.clip(bg + (rgb - bg) / safe, 0.0, 1.0)

    zone = ndimage.binary_dilation(region, iterations=band)
    out_a = np.where(zone, alpha, 1.0)
    out_a[region] = 0.0
    out_rgb = np.where(zone[..., None], unmixed, rgb)
    out = np.dstack([out_rgb, out_a])
    Image.fromarray((out * 255 + 0.5).astype(np.uint8), "RGBA").save(path)
    print(f"done: {path.name}  transparent {(out_a < 0.03).mean():.0%}")


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("target")
    ap.add_argument("--backup", required=True, help="folder to keep the untouched originals")
    ap.add_argument("--band", type=int, default=6, help="px around the background that get soft alpha")
    ap.add_argument("--hole", type=int, default=1500, help="min px of an enclosed checker pocket to clear")
    args = ap.parse_args()
    target = Path(args.target)
    files = sorted(target.glob("*.png")) if target.is_dir() else [target]
    for f in files:
        process(f, Path(args.backup), args.band, args.hole)


if __name__ == "__main__":
    main()
