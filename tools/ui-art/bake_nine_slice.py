"""Bake a 9-slice UI sprite into a Simple image of an exact size, without stretching anything (decision #66).

Corners are copied whole. Edges and the centre are *tiled* (repeated with a short cross-fade), never stretched.
Everything is scaled by one uniform factor, chosen so the result looks like the old Sliced image did in Unity.

Usage:
    py tools/ui-art/bake_nine_slice.py <source.png> <out.png> <canvas_w> <canvas_h> <multiplier>
        [--border L,B,R,T] [--density 2] [--bar] [--sample 0.5]

- canvas_w/h: the rect size in canvas units (1920x1080 reference).
- multiplier: the pixelsPerUnitMultiplier the Sliced image used, so the ornaments keep their old on-screen size.
- border: the 9-slice border in source px (Unity order: left, bottom, right, top). Read from the .meta by default.
- density: output pixels per canvas unit (default 2). The long side is capped at 2048.
- bar: horizontal 3-slice for buttons, fields and bars. The whole art is scaled to the target height and only the
  middle column is tiled, so end caps (and anything in them, like a dropdown arrow) appear once. multiplier is
  ignored.
- sample: share of each edge and of the centre (taken from its middle) used as the repeating tile, so ornaments
  near the corners are not repeated along the edges.
"""
import argparse
import re
from pathlib import Path

import numpy as np
from PIL import Image

FADE_SHARE = 0.15
MAX_SIDE = 2048


def read_border(source):
    meta = Path(str(source) + ".meta").read_text(encoding="utf-8")
    match = re.search(r"spriteBorder: \{x: (\d+), y: (\d+), z: (\d+), w: (\d+)\}", meta)
    if match is None:
        raise SystemExit("No spriteBorder in " + str(source) + ".meta; pass --border")
    return tuple(int(value) for value in match.groups())


def scaled(pixels, factor):
    height, width = pixels.shape[:2]
    if height == 0 or width == 0:
        return np.zeros((round(height * factor), round(width * factor), 4), np.float32)
    size = (max(1, round(width * factor)), max(1, round(height * factor)))
    image = Image.fromarray(pixels.astype(np.uint8), "RGBA").resize(size, Image.Resampling.LANCZOS)
    return np.asarray(image).astype(np.float32)


def tile(piece, length, axis):
    """Repeats piece along axis to exactly length, cross-fading joins; the strip still ends on piece's end."""
    size = piece.shape[axis]
    if size == 0 or length <= 0:
        shape = list(piece.shape)
        shape[axis] = max(0, length)
        return np.zeros(shape, np.float32)
    fade = max(1, int(size * FADE_SHARE)) if size > 4 else 0
    strip = piece
    while strip.shape[axis] < length + fade:
        if fade == 0:
            strip = np.concatenate([strip, piece], axis=axis)
            continue
        weight = np.linspace(0.0, 1.0, fade)
        weight = weight[:, None, None] if axis == 0 else weight[None, :, None]
        head = np.take(strip, range(strip.shape[axis] - fade), axis=axis)
        tail = np.take(strip, range(strip.shape[axis] - fade, strip.shape[axis]), axis=axis)
        start = np.take(piece, range(fade), axis=axis)
        rest = np.take(piece, range(fade, size), axis=axis)
        strip = np.concatenate([head, tail * (1 - weight) + start * weight, rest], axis=axis)
    strip = np.take(strip, range(length), axis=axis)
    # Blend the last stretch into the piece's own end so the join with the far corner stays seamless.
    blend = min(fade, length, size)
    if blend > 0:
        weight = np.linspace(0.0, 1.0, blend)
        weight = weight[:, None, None] if axis == 0 else weight[None, :, None]
        ending = np.take(piece, range(size - blend, size), axis=axis)
        current = np.take(strip, range(length - blend, length), axis=axis)
        keep = np.take(strip, range(length - blend), axis=axis)
        strip = np.concatenate([keep, current * (1 - weight) + ending * weight], axis=axis)
    return strip


def middle_share(start, end, share):
    """The centred share of the range start..end, at least 8 px."""
    length = end - start
    keep = max(min(length, 8), round(length * share))
    first = start + (length - keep) // 2
    return first, first + keep


def bake(source, border, width, height, factor, sample):
    pixels = np.asarray(Image.open(source).convert("RGBA")).astype(np.float32)
    rows, cols = pixels.shape[:2]
    left, bottom, right, top = border
    # Source regions: rows count from the top, so Unity's top border is the first rows.
    # A bar (no top/bottom border) keeps its full height; only the middle column is sampled.
    middle_rows = middle_share(top, rows - bottom, sample) if top or bottom else (0, rows)
    ys = [(0, top), middle_rows, (rows - bottom, rows)]
    xs = [(0, left), middle_share(left, cols - right, sample), (cols - right, cols)]
    parts = [[scaled(pixels[ys[r][0]:ys[r][1], xs[c][0]:xs[c][1]], factor) for c in range(3)] for r in range(3)]
    left_px = parts[0][0].shape[1]
    right_px = parts[0][2].shape[1]
    top_px = parts[0][0].shape[0]
    bottom_px = parts[2][0].shape[0]
    middle_w = width - left_px - right_px
    middle_h = height - top_px - bottom_px
    if middle_w < 0 or middle_h < 0:
        raise SystemExit("Corners do not fit; use a larger multiplier for this size")
    rows_out = []
    for r, row_height in ((0, top_px), (1, middle_h), (2, bottom_px)):
        cells = []
        for c, cell_width in ((0, left_px), (1, middle_w), (2, right_px)):
            cell = parts[r][c]
            if c == 1:
                cell = tile(cell, cell_width, axis=1)
            if r == 1:
                cell = tile(cell, row_height, axis=0)
            cells.append(cell)
        rows_out.append(np.concatenate(cells, axis=1))
    result = np.concatenate(rows_out, axis=0)
    return Image.fromarray(np.clip(result, 0, 255).astype(np.uint8), "RGBA")


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("source")
    parser.add_argument("out")
    parser.add_argument("canvas_w", type=float)
    parser.add_argument("canvas_h", type=float)
    parser.add_argument("multiplier", type=float)
    parser.add_argument("--border")
    parser.add_argument("--density", type=float, default=2.0)
    parser.add_argument("--bar", action="store_true")
    parser.add_argument("--sample", type=float, default=0.5)
    args = parser.parse_args()
    border = tuple(int(v) for v in args.border.split(",")) if args.border else read_border(args.source)
    density = min(args.density, MAX_SIDE / max(args.canvas_w, args.canvas_h))
    width = round(args.canvas_w * density)
    height = round(args.canvas_h * density)
    # A Sliced border of b source px showed as b / multiplier canvas units; keep that on-screen size.
    factor = density / args.multiplier
    if args.bar:
        # Scale to the full height and keep the whole top and bottom: only the middle column repeats.
        source_height = Image.open(args.source).height
        factor = height / source_height
        border = (border[0], 0, border[2], 0)
    bake(args.source, border, width, height, factor, args.sample).save(args.out)
    print(f"{Path(args.out).name}: {width}x{height} px for {args.canvas_w:g}x{args.canvas_h:g}, scale {factor:.3f}")


if __name__ == "__main__":
    main()
