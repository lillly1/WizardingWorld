"""
Sprite Generator for Wizarding World tModLoader Mod
Generates detailed procedural pixel art .png textures for all mod content.
Run: python generate_sprites.py
"""

from PIL import Image, ImageDraw
import os
import math

BASE = os.path.dirname(os.path.abspath(__file__))


def save(img, *path_parts):
    full = os.path.join(BASE, *path_parts)
    os.makedirs(os.path.dirname(full), exist_ok=True)
    img.save(full)
    print(f"  Created: {os.path.relpath(full, BASE)}")


def darker(color, amount=60):
    return tuple(max(0, c - amount) for c in color[:3]) + (color[3] if len(color) > 3 else 255,)


def lighter(color, amount=60):
    return tuple(min(255, c + amount) for c in color[:3]) + (color[3] if len(color) > 3 else 255,)


def blend(c1, c2, t):
    """Blend two colors. t=0 gives c1, t=1 gives c2."""
    return tuple(int(a + (b - a) * t) for a, b in zip(c1[:3], c2[:3])) + (255,)


def safe_pixel(img, x, y, color):
    """Set a pixel only if within image bounds."""
    if 0 <= x < img.width and 0 <= y < img.height:
        img.putpixel((int(x), int(y)), color)


def draw_outlined_rect(d, bbox, fill, outline_color):
    """Draw a rectangle with 1px outline."""
    x0, y0, x1, y1 = bbox
    d.rectangle([x0, y0, x1, y1], fill=outline_color)
    if x1 - x0 > 2 and y1 - y0 > 2:
        d.rectangle([x0 + 1, y0 + 1, x1 - 1, y1 - 1], fill=fill)


# ============================================================
# DRAWING FUNCTIONS
# ============================================================

def draw_wand(w, h, wood_color, core_color, handle_style='simple'):
    """Draw a detailed diagonal wand with handle, wood grain, core line, and glowing tip."""
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    outline_color = darker(wood_color, 80)

    # Wand goes from bottom-left to top-right
    # Calculate wand line endpoints with margin
    margin = max(3, w // 8)
    x0, y0 = margin, h - margin  # bottom-left (handle)
    x1, y1 = w - margin, margin  # top-right (tip)

    dx = x1 - x0
    dy = y1 - y0
    length = math.sqrt(dx * dx + dy * dy)
    if length == 0:
        return img
    ux, uy = dx / length, dy / length
    # Perpendicular
    px, py = -uy, ux

    # Draw wand body - outline first (thicker)
    d.line([(x0, y0), (x1, y1)], fill=outline_color, width=max(4, w // 8))

    # Draw wood body
    wand_width = max(3, w // 10)
    d.line([(x0, y0), (x1, y1)], fill=wood_color, width=wand_width)

    # Wood grain: alternating lighter/darker lines along the wand
    grain_light = lighter(wood_color, 25)
    grain_dark = darker(wood_color, 20)
    steps = int(length)
    for i in range(0, steps, 3):
        t = i / length
        cx = x0 + dx * t
        cy = y0 + dy * t
        grain_c = grain_light if (i // 3) % 2 == 0 else grain_dark
        safe_pixel(img, int(cx + px * 0.5), int(cy + py * 0.5), grain_c)

    # Core line through center
    core_start_t = 0.15
    core_end_t = 0.85
    for i in range(int(length * core_start_t), int(length * core_end_t)):
        t = i / length
        cx = int(x0 + dx * t)
        cy = int(y0 + dy * t)
        safe_pixel(img, cx, cy, core_color)

    # Handle at bottom-left
    handle_cx = x0 + dx * 0.05
    handle_cy = y0 + dy * 0.05
    handle_r = max(2, w // 10)
    handle_color = darker(wood_color, 30)

    if handle_style == 'knob':
        d.ellipse([int(handle_cx - handle_r - 1), int(handle_cy - handle_r - 1),
                    int(handle_cx + handle_r + 1), int(handle_cy + handle_r + 1)],
                   fill=outline_color)
        d.ellipse([int(handle_cx - handle_r), int(handle_cy - handle_r),
                    int(handle_cx + handle_r), int(handle_cy + handle_r)],
                   fill=handle_color)
    else:
        # Wrapped handle - a few perpendicular lines
        for j in range(-1, 3):
            wrap_t = 0.02 + j * 0.04
            if 0 <= wrap_t <= 0.15:
                wx = x0 + dx * wrap_t
                wy = y0 + dy * wrap_t
                d.line([(int(wx - px * 2), int(wy - py * 2)),
                        (int(wx + px * 2), int(wy + py * 2))],
                       fill=darker(handle_color, 30), width=1)

    # Glowing tip at top-right - concentric circles getting brighter
    tip_cx = x1 - dx * 0.02
    tip_cy = y1 - dy * 0.02
    glow_base = core_color
    for ring in range(4, 0, -1):
        r = ring + 1
        alpha = max(80, 255 - ring * 50)
        glow_c = (min(255, glow_base[0] + ring * 20),
                  min(255, glow_base[1] + ring * 20),
                  min(255, glow_base[2] + ring * 20),
                  alpha)
        d.ellipse([int(tip_cx - r), int(tip_cy - r),
                    int(tip_cx + r), int(tip_cy + r)],
                   fill=glow_c)
    # Bright center
    bright = lighter(core_color, 80)
    safe_pixel(img, int(tip_cx), int(tip_cy), (min(255, bright[0]), min(255, bright[1]), min(255, bright[2]), 255))

    return img


def draw_potion(w, h, liquid_color, bottle_color=(200, 200, 220, 255)):
    """Draw a glass potion bottle with cork, liquid, meniscus, highlight, and bubble."""
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    outline = darker(bottle_color, 80)

    # Proportions
    neck_top = max(2, h // 8)
    cork_top = max(0, neck_top - 2)
    neck_bottom = h // 3
    body_top = neck_bottom
    body_bottom = h - 2
    body_left = max(1, w // 5)
    body_right = w - body_left - 1
    neck_left = w // 3
    neck_right = w - neck_left - 1

    # Cork
    cork_color = (160, 110, 60, 255)
    d.rectangle([neck_left, cork_top, neck_right, neck_top + 1], fill=cork_color, outline=darker(cork_color))

    # Neck outline + fill
    d.rectangle([neck_left - 1, neck_top, neck_right + 1, neck_bottom], fill=outline)
    d.rectangle([neck_left, neck_top + 1, neck_right, neck_bottom], fill=bottle_color)

    # Body outline + fill
    d.rectangle([body_left - 1, body_top - 1, body_right + 1, body_bottom + 1], fill=outline)
    d.rectangle([body_left, body_top, body_right, body_bottom], fill=bottle_color)

    # Taper from neck to body (shoulders)
    for y in range(neck_bottom, body_top + 2):
        t = 0 if body_top == neck_bottom else (y - neck_bottom) / max(1, (body_top + 2 - neck_bottom))
        xl = int(neck_left + (body_left - neck_left) * t)
        xr = int(neck_right + (body_right - neck_right) * t)
        d.line([(xl, y), (xr, y)], fill=bottle_color)

    # Flat bottom
    d.line([(body_left, body_bottom), (body_right, body_bottom)], fill=outline)

    # Liquid fills 65% of body
    liquid_top = int(body_top + (body_bottom - body_top) * 0.35)
    liquid_bottom = body_bottom - 1

    for y in range(liquid_top, liquid_bottom + 1):
        d.line([(body_left + 1, y), (body_right - 1, y)], fill=liquid_color)

    # Meniscus curve at liquid top
    meniscus_color = lighter(liquid_color, 30)
    mid_x = (body_left + body_right) // 2
    liq_w = body_right - body_left - 2
    if liq_w > 2:
        for x in range(body_left + 1, body_right):
            dist = abs(x - mid_x) / max(1, liq_w / 2)
            if dist < 0.7:
                safe_pixel(img, x, liquid_top, meniscus_color)

    # Glass highlight (upper-left of body)
    highlight_x = body_left + 2
    highlight_y = body_top + 2
    safe_pixel(img, highlight_x, highlight_y, (255, 255, 255, 200))
    safe_pixel(img, highlight_x, highlight_y + 1, (255, 255, 255, 140))

    # Bubble in the liquid
    bubble_x = mid_x + 1
    bubble_y = (liquid_top + liquid_bottom) // 2
    safe_pixel(img, bubble_x, bubble_y, lighter(liquid_color, 60))

    return img


def draw_accessory(w, h, main_color, shape='circle', detail_color=None):
    """Draw an accessory item with various shapes."""
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    outline = darker(main_color, 80)
    if detail_color is None:
        detail_color = lighter(main_color, 50)

    cx, cy = w // 2, h // 2

    if shape == 'pendant':
        # Chain loop at top
        chain_color = (180, 160, 80, 255)
        d.arc([cx - 4, 1, cx + 4, 8], 0, 180, fill=chain_color, width=1)
        d.line([(cx - 4, 4), (cx - 3, cy - 4)], fill=chain_color)
        d.line([(cx + 4, 4), (cx + 3, cy - 4)], fill=chain_color)
        # Pendant body (oval)
        pr = min(w, h) // 4
        d.ellipse([cx - pr - 1, cy - pr + 1, cx + pr + 1, cy + pr + 3], fill=outline)
        d.ellipse([cx - pr, cy - pr + 2, cx + pr, cy + pr + 2], fill=main_color)
        # Emblem
        d.ellipse([cx - 2, cy - 1, cx + 2, cy + 3], fill=detail_color)
        # Highlight
        safe_pixel(img, cx - pr + 2, cy - pr + 4, lighter(main_color, 70))

    elif shape == 'square':
        # Book/diary
        bx0, by0 = max(1, cx - w // 3), max(1, cy - h // 3)
        bx1, by1 = min(w - 2, cx + w // 3), min(h - 2, cy + h // 3)
        d.rectangle([bx0 - 1, by0 - 1, bx1 + 1, by1 + 1], fill=outline)
        d.rectangle([bx0, by0, bx1, by1], fill=main_color)
        # Spine
        d.line([(bx0 + 2, by0), (bx0 + 2, by1)], fill=darker(main_color, 40))
        # Page lines
        for ly in range(by0 + 3, by1 - 1, 3):
            d.line([(bx0 + 4, ly), (bx1 - 2, ly)], fill=lighter(main_color, 30))
        # Emblem on cover
        ecx = (bx0 + bx1) // 2 + 1
        ecy = (by0 + by1) // 2
        safe_pixel(img, ecx, ecy, detail_color)
        safe_pixel(img, ecx - 1, ecy, detail_color)
        safe_pixel(img, ecx + 1, ecy, detail_color)
        safe_pixel(img, ecx, ecy - 1, detail_color)
        safe_pixel(img, ecx, ecy + 1, detail_color)

    elif shape == 'triangle':
        # Hat/pointed shape
        pts = [(cx, 2), (2, h - 3), (w - 3, h - 3)]
        d.polygon(pts, fill=main_color, outline=outline)
        # Brim
        d.line([(1, h - 3), (w - 2, h - 3)], fill=outline, width=2)
        # Band
        band_y = h - 5
        d.line([(4, band_y), (w - 5, band_y)], fill=detail_color, width=1)
        # Highlight
        safe_pixel(img, cx - 1, 5, lighter(main_color, 50))

    else:
        # Circle: glowing orb/amulet
        r = min(w, h) // 2 - 2
        # Outer glow
        d.ellipse([cx - r - 1, cy - r - 1, cx + r + 1, cy + r + 1], fill=outline)
        d.ellipse([cx - r, cy - r, cx + r, cy + r], fill=main_color)
        # Inner gradient (lighter center)
        inner_r = max(1, r // 2)
        d.ellipse([cx - inner_r, cy - inner_r, cx + inner_r, cy + inner_r],
                  fill=lighter(main_color, 40))
        # Highlight
        safe_pixel(img, cx - r // 2, cy - r // 2, (255, 255, 255, 180))
        # Detail emblem
        safe_pixel(img, cx, cy, detail_color)

    return img


def draw_sword(w, h, blade_color, handle_color, gem_color=None):
    """Draw a diagonal sword with blade, crossguard, handle, and optional gem."""
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    outline = darker(blade_color, 80)

    # Blade from bottom-left handle to top-right tip
    margin = max(3, w // 8)
    hx, hy = margin + 2, h - margin - 2  # handle position
    tx, ty = w - margin - 1, margin + 1   # tip position

    dx = tx - hx
    dy = ty - hy
    length = math.sqrt(dx * dx + dy * dy)
    if length == 0:
        return img
    ux, uy = dx / length, dy / length
    px, py = -uy, ux

    # Blade outline
    d.line([(hx + ux * 8, hy + uy * 8), (tx, ty)], fill=outline, width=max(5, w // 8))
    # Blade body
    blade_w = max(3, w // 10)
    d.line([(hx + ux * 8, hy + uy * 8), (tx, ty)], fill=blade_color, width=blade_w)

    # Lighter blade edge (sharpened side)
    edge_color = lighter(blade_color, 50)
    blade_len = length - 8
    for i in range(0, int(blade_len)):
        t = (i + 8) / length
        bx = hx + dx * t
        by = hy + dy * t
        safe_pixel(img, int(bx + px * 1.5), int(by + py * 1.5), edge_color)

    # Crossguard
    guard_cx = hx + dx * 0.2
    guard_cy = hy + dy * 0.2
    guard_len = max(4, w // 5)
    guard_color = darker(handle_color, 20)
    d.line([(int(guard_cx - px * guard_len), int(guard_cy - py * guard_len)),
            (int(guard_cx + px * guard_len), int(guard_cy + py * guard_len))],
           fill=guard_color, width=max(2, w // 14))

    # Handle (wrapped)
    handle_dark = darker(handle_color, 30)
    d.line([(hx, hy), (int(hx + ux * 8), int(hy + uy * 8))],
           fill=handle_color, width=max(3, w // 12))
    # Wrap lines
    for j in range(0, 8, 2):
        wx = hx + ux * j
        wy = hy + uy * j
        d.line([(int(wx - px * 2), int(wy - py * 2)),
                (int(wx + px * 2), int(wy + py * 2))],
               fill=handle_dark, width=1)

    # Pommel
    pommel_x = hx - ux * 1
    pommel_y = hy - uy * 1
    d.ellipse([int(pommel_x - 2), int(pommel_y - 2),
               int(pommel_x + 2), int(pommel_y + 2)],
              fill=guard_color)

    # Optional gem in crossguard
    if gem_color:
        d.ellipse([int(guard_cx - 2), int(guard_cy - 2),
                    int(guard_cx + 2), int(guard_cy + 2)],
                   fill=gem_color)
        safe_pixel(img, int(guard_cx - 1), int(guard_cy - 1), lighter(gem_color, 60))

    return img


def draw_npc_sheet(w, h, frames, skin_color, robe_color, hair_color, features=None):
    """Draw a vertical NPC spritesheet with humanoid frames."""
    if features is None:
        features = {}
    total_h = frames * (h + 2)
    img = Image.new("RGBA", (w, total_h), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    outline = darker(robe_color, 60)

    cx = w // 2
    head_r = max(3, min(w, h) // 6)
    eye_color = (30, 30, 40, 255)

    for f in range(frames):
        y_off = f * (h + 2)

        # Head
        head_cy = y_off + head_r + 2
        d.ellipse([cx - head_r - 1, head_cy - head_r - 1,
                    cx + head_r + 1, head_cy + head_r + 1],
                   fill=darker(skin_color, 40))
        d.ellipse([cx - head_r, head_cy - head_r,
                    cx + head_r, head_cy + head_r],
                   fill=skin_color)

        # Hair
        hair_top = head_cy - head_r
        d.rectangle([cx - head_r, hair_top, cx + head_r, hair_top + max(2, head_r // 2)],
                     fill=hair_color)

        # Eyes
        eye_offset = max(1, head_r // 2)
        eye_y = head_cy
        safe_pixel(img, cx - eye_offset, eye_y, eye_color)
        safe_pixel(img, cx + eye_offset, eye_y, eye_color)

        # Body / Robe
        body_top = head_cy + head_r + 1
        body_bottom = y_off + h - max(6, h // 5)
        body_half = max(3, w // 4)
        d.rectangle([cx - body_half - 1, body_top - 1, cx + body_half + 1, body_bottom + 1],
                     fill=outline)
        d.rectangle([cx - body_half, body_top, cx + body_half, body_bottom],
                     fill=robe_color)

        # Arms
        arm_len = max(3, (body_bottom - body_top) // 2)
        arm_offset = (f % 2) * 2 - 1  # slight animation
        # Left arm
        d.line([(cx - body_half - 1, body_top + 2),
                (cx - body_half - 2, body_top + arm_len + arm_offset)],
               fill=robe_color, width=2)
        # Right arm
        d.line([(cx + body_half + 1, body_top + 2),
                (cx + body_half + 2, body_top + arm_len - arm_offset)],
               fill=robe_color, width=2)

        # Legs
        leg_top = body_bottom + 1
        leg_bottom = y_off + h - 2
        leg_w = max(1, body_half // 2)
        leg_offset = (f % 2) * 1
        # Left leg
        d.rectangle([cx - leg_w - 1 - leg_offset, leg_top,
                      cx - 1 - leg_offset, leg_bottom],
                     fill=darker(robe_color, 20))
        # Right leg
        d.rectangle([cx + 1 + leg_offset, leg_top,
                      cx + leg_w + 1 + leg_offset, leg_bottom],
                     fill=darker(robe_color, 20))

        # Features
        if features.get('hat'):
            hat_color = features.get('hat_color', darker(robe_color, 20))
            d.polygon([(cx, hair_top - max(3, head_r)),
                        (cx - head_r - 1, hair_top),
                        (cx + head_r + 1, hair_top)],
                       fill=hat_color, outline=darker(hat_color))

        if features.get('beard'):
            beard_color = features.get('beard_color', hair_color)
            chin_y = head_cy + head_r
            d.rectangle([cx - head_r // 2, chin_y, cx + head_r // 2, chin_y + max(2, head_r // 2)],
                         fill=beard_color)

        if features.get('weapon'):
            wand_color = features.get('weapon_color', (139, 90, 43, 255))
            wx = cx + body_half + 3
            d.line([(wx, body_top + 2), (wx + 3, body_top - 3)], fill=wand_color, width=1)

    return img


def draw_enemy_sheet(w, h, frames, body_color, eye_color, shape='humanoid'):
    """Draw an enemy spritesheet with shape variants."""
    total_h = frames * (h + 2)
    img = Image.new("RGBA", (w, total_h), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    outline = darker(body_color, 70)
    cx, cy_base = w // 2, h // 2

    for f in range(frames):
        y_off = f * (h + 2)
        cy = y_off + cy_base
        anim = (f % 2) * 2 - 1  # -1 or 1

        if shape == 'spider':
            # Round body
            br = min(w, h) // 4
            d.ellipse([cx - br - 1, cy - br - 1, cx + br + 1, cy + br + 1], fill=outline)
            d.ellipse([cx - br, cy - br, cx + br, cy + br], fill=body_color)
            # 8 legs (4 per side)
            for i in range(4):
                angle = math.pi * 0.3 + i * math.pi * 0.12
                leg_len = br + max(3, w // 5)
                anim_off = anim * (1 if i % 2 == 0 else -1)
                # Left legs
                lx = cx - int(math.cos(angle) * leg_len)
                ly = cy + int(math.sin(angle) * leg_len) + anim_off
                d.line([(cx - br, cy + (i - 1) * 2), (lx, ly)], fill=outline, width=1)
                # Right legs
                rx = cx + int(math.cos(angle) * leg_len)
                ry = cy + int(math.sin(angle) * leg_len) + anim_off
                d.line([(cx + br, cy + (i - 1) * 2), (rx, ry)], fill=outline, width=1)
            # Multiple eyes (4)
            for i in range(-2, 2):
                safe_pixel(img, cx + i * 2, cy - br // 2, eye_color)
            # Fangs
            safe_pixel(img, cx - 1, cy + br, lighter(body_color, 80))
            safe_pixel(img, cx + 1, cy + br, lighter(body_color, 80))

        elif shape == 'serpent':
            # S-curve body
            seg_count = max(6, w // 3)
            for i in range(seg_count):
                t = i / max(1, seg_count - 1)
                sx = int(w * 0.15 + t * w * 0.7)
                sy = int(cy + math.sin(t * math.pi * 2 + f * 0.5) * h * 0.2)
                seg_r = max(1, int(3 - t * 1.5))
                d.ellipse([sx - seg_r, sy - seg_r, sx + seg_r, sy + seg_r], fill=body_color)
            # Head at left
            hx = int(w * 0.12)
            hy = int(cy + math.sin(f * 0.5) * h * 0.2)
            hr = max(2, min(w, h) // 6)
            d.ellipse([hx - hr - 1, hy - hr - 1, hx + hr + 1, hy + hr + 1], fill=outline)
            d.ellipse([hx - hr, hy - hr, hx + hr, hy + hr], fill=body_color)
            # Eye
            safe_pixel(img, hx - 1, hy - 1, eye_color)
            safe_pixel(img, hx + 1, hy - 1, eye_color)
            # Forked tongue
            safe_pixel(img, hx - hr - 1, hy, (200, 50, 50, 255))
            safe_pixel(img, hx - hr - 2, hy - 1, (200, 50, 50, 255))
            safe_pixel(img, hx - hr - 2, hy + 1, (200, 50, 50, 255))

        elif shape == 'bat':
            # Body (small oval)
            br = max(2, min(w, h) // 6)
            d.ellipse([cx - br, cy - br, cx + br, cy + br], fill=body_color, outline=outline)
            # Wings
            wing_span = max(4, w // 3)
            wing_up = anim * 2
            # Left wing
            d.polygon([(cx - br, cy - 1),
                        (cx - wing_span, cy - br - wing_up),
                        (cx - wing_span // 2, cy + br)],
                       fill=darker(body_color, 20), outline=outline)
            # Right wing
            d.polygon([(cx + br, cy - 1),
                        (cx + wing_span, cy - br - wing_up),
                        (cx + wing_span // 2, cy + br)],
                       fill=darker(body_color, 20), outline=outline)
            # Eyes
            safe_pixel(img, cx - 1, cy - 1, eye_color)
            safe_pixel(img, cx + 1, cy - 1, eye_color)
            # Ears
            safe_pixel(img, cx - br + 1, cy - br - 1, body_color)
            safe_pixel(img, cx + br - 1, cy - br - 1, body_color)

        elif shape == 'ghost':
            # Floating wispy form
            gr = max(3, min(w, h) // 4)
            float_off = anim * 2
            ghost_cy = cy + float_off
            # Wispy bottom (jagged)
            d.ellipse([cx - gr - 1, ghost_cy - gr - 1, cx + gr + 1, ghost_cy + 1],
                       fill=outline)
            d.ellipse([cx - gr, ghost_cy - gr, cx + gr, ghost_cy],
                       fill=body_color)
            # Wispy tails
            for i in range(-gr, gr + 1, 2):
                tail_len = max(1, gr // 2) + abs(i) % 3
                d.line([(cx + i, ghost_cy), (cx + i, ghost_cy + tail_len)],
                       fill=body_color)
            # Eyes and mouth
            safe_pixel(img, cx - 2, ghost_cy - gr // 2, eye_color)
            safe_pixel(img, cx + 2, ghost_cy - gr // 2, eye_color)
            safe_pixel(img, cx, ghost_cy - gr // 4 + 2, darker(body_color, 40))

        elif shape == 'blob':
            # Amorphous shape
            br = max(3, min(w, h) // 3)
            blob_off = anim
            points = []
            for i in range(8):
                angle = i * math.pi / 4
                r_var = br + (1 if (i + f) % 3 == 0 else -1) * max(1, br // 4)
                px_b = cx + int(math.cos(angle) * r_var)
                py_b = cy + int(math.sin(angle) * r_var) + blob_off
                points.append((px_b, py_b))
            d.polygon(points, fill=body_color, outline=outline)
            # Eyes
            safe_pixel(img, cx - 2, cy - 2 + blob_off, eye_color)
            safe_pixel(img, cx + 2, cy - 2 + blob_off, eye_color)

        else:
            # Humanoid
            head_r = max(2, min(w, h) // 8)
            head_cy = y_off + head_r + 3
            # Head
            d.ellipse([cx - head_r - 1, head_cy - head_r - 1,
                        cx + head_r + 1, head_cy + head_r + 1], fill=outline)
            d.ellipse([cx - head_r, head_cy - head_r,
                        cx + head_r, head_cy + head_r], fill=body_color)
            # Eyes
            safe_pixel(img, cx - max(1, head_r // 2), head_cy, eye_color)
            safe_pixel(img, cx + max(1, head_r // 2), head_cy, eye_color)

            # Body
            body_top = head_cy + head_r + 1
            body_bottom = y_off + h - max(5, h // 4)
            body_half_w = max(2, w // 4)
            d.rectangle([cx - body_half_w - 1, body_top - 1,
                          cx + body_half_w + 1, body_bottom + 1], fill=outline)
            d.rectangle([cx - body_half_w, body_top,
                          cx + body_half_w, body_bottom], fill=body_color)

            # Arms
            arm_l = max(2, (body_bottom - body_top) // 2)
            d.line([(cx - body_half_w - 1, body_top + 1),
                    (cx - body_half_w - 2, body_top + arm_l + anim)],
                   fill=body_color, width=1)
            d.line([(cx + body_half_w + 1, body_top + 1),
                    (cx + body_half_w + 2, body_top + arm_l - anim)],
                   fill=body_color, width=1)

            # Legs
            leg_top = body_bottom + 1
            leg_bottom = y_off + h - 2
            leg_w_half = max(1, body_half_w // 2)
            d.rectangle([cx - leg_w_half - 1, leg_top, cx - 1, leg_bottom],
                         fill=darker(body_color, 20), outline=outline)
            d.rectangle([cx + 1, leg_top, cx + leg_w_half + 1, leg_bottom],
                         fill=darker(body_color, 20), outline=outline)

    return img


def draw_boss_sheet(w, h, frames, body_color, eye_color, boss_type):
    """Draw a boss spritesheet."""
    total_h = frames * (h + 2)
    img = Image.new("RGBA", (w, total_h), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    outline = darker(body_color, 80)
    cx = w // 2

    for f in range(frames):
        y_off = f * (h + 2)
        cy = y_off + h // 2
        anim = (f % 3) - 1  # -1, 0, 1

        if boss_type == 'serpent':
            # Massive snake head with coiled body
            # Coiled body behind
            coil_r = min(w, h) // 3
            for i in range(3):
                angle_off = i * 0.7 + f * 0.3
                coil_x = cx + int(math.cos(angle_off) * coil_r * 0.5)
                coil_y = cy + int(math.sin(angle_off) * coil_r * 0.3) + h // 6
                sr = max(3, coil_r // 3)
                d.ellipse([coil_x - sr, coil_y - sr, coil_x + sr, coil_y + sr],
                           fill=body_color, outline=outline)
                # Scale pattern
                for s in range(-sr + 1, sr, 2):
                    safe_pixel(img, coil_x + s, coil_y, darker(body_color, 15))

            # Large head
            head_w = max(6, w // 3)
            head_h = max(5, h // 4)
            hx = cx
            hy = cy - h // 6
            d.ellipse([hx - head_w - 1, hy - head_h - 1,
                        hx + head_w + 1, hy + head_h + 1], fill=outline)
            d.ellipse([hx - head_w, hy - head_h,
                        hx + head_w, hy + head_h], fill=body_color)

            # Yellow slit eyes
            ey = hy - head_h // 3
            d.rectangle([hx - head_w // 2 - 1, ey - 2,
                          hx - head_w // 2 + 1, ey + 2], fill=eye_color)
            d.rectangle([hx + head_w // 2 - 1, ey - 2,
                          hx + head_w // 2 + 1, ey + 2], fill=eye_color)
            # Slit pupils
            safe_pixel(img, hx - head_w // 2, ey, (0, 0, 0, 255))
            safe_pixel(img, hx + head_w // 2, ey, (0, 0, 0, 255))

            # Fangs
            fang_y = hy + head_h
            d.line([(hx - 3, fang_y), (hx - 3, fang_y + 3)], fill=(240, 240, 220, 255), width=1)
            d.line([(hx + 3, fang_y), (hx + 3, fang_y + 3)], fill=(240, 240, 220, 255), width=1)

        elif boss_type == 'dragon':
            # Winged beast with horns
            # Body
            body_r_x = max(6, w // 4)
            body_r_y = max(5, h // 5)
            d.ellipse([cx - body_r_x - 1, cy - body_r_y - 1,
                        cx + body_r_x + 1, cy + body_r_y + 1], fill=outline)
            d.ellipse([cx - body_r_x, cy - body_r_y,
                        cx + body_r_x, cy + body_r_y], fill=body_color)

            # Wings
            wing_span = max(6, w // 3)
            wing_up = anim * 3
            # Left wing
            d.polygon([(cx - body_r_x, cy - 2),
                        (cx - body_r_x - wing_span, cy - body_r_y - wing_up - 3),
                        (cx - body_r_x - wing_span // 2, cy + 2)],
                       fill=darker(body_color, 25), outline=outline)
            # Right wing
            d.polygon([(cx + body_r_x, cy - 2),
                        (cx + body_r_x + wing_span, cy - body_r_y - wing_up - 3),
                        (cx + body_r_x + wing_span // 2, cy + 2)],
                       fill=darker(body_color, 25), outline=outline)

            # Head
            head_r = max(3, min(w, h) // 8)
            hx = cx
            hy = cy - body_r_y - head_r
            d.ellipse([hx - head_r - 1, hy - head_r - 1,
                        hx + head_r + 1, hy + head_r + 1], fill=outline)
            d.ellipse([hx - head_r, hy - head_r,
                        hx + head_r, hy + head_r], fill=body_color)

            # Horns
            d.line([(hx - head_r + 1, hy - head_r), (hx - head_r - 2, hy - head_r - 4)],
                   fill=darker(body_color, 40), width=1)
            d.line([(hx + head_r - 1, hy - head_r), (hx + head_r + 2, hy - head_r - 4)],
                   fill=darker(body_color, 40), width=1)

            # Eyes
            safe_pixel(img, hx - 2, hy - 1, eye_color)
            safe_pixel(img, hx + 2, hy - 1, eye_color)

            # Spiky tail
            tail_x = cx + body_r_x
            tail_y = cy + body_r_y // 2
            for i in range(4):
                tx = tail_x + i * 3
                ty = tail_y + anim + (i % 2) * 2
                d.line([(tx, ty), (tx + 2, ty - 2)], fill=darker(body_color, 30), width=1)

            # Fire breath area
            if f % 3 == 0:
                for i in range(3):
                    fx = hx - head_r - 2 - i * 2
                    fy = hy + i - 1
                    fire_c = (255, max(0, 200 - i * 60), 0, max(100, 200 - i * 50))
                    safe_pixel(img, fx, fy, fire_c)

        elif boss_type == 'wizard':
            # Tall robed figure with wand and dark aura
            head_r = max(3, min(w, h) // 8)
            head_cy = y_off + head_r + 5
            robe_top = head_cy + head_r + 1
            robe_bottom = y_off + h - 4
            robe_half = max(4, w // 4)

            # Dark aura (wispy outline)
            for a in range(0, 360, 20):
                ar = max(8, min(w, h) // 3) + anim
                ax = cx + int(math.cos(math.radians(a)) * ar)
                ay = cy + int(math.sin(math.radians(a)) * ar)
                aura_c = (body_color[0] // 2, body_color[1] // 2, body_color[2] // 2, 80)
                safe_pixel(img, ax, ay, aura_c)

            # Robe
            d.rectangle([cx - robe_half - 1, robe_top - 1,
                          cx + robe_half + 1, robe_bottom + 1], fill=outline)
            d.rectangle([cx - robe_half, robe_top,
                          cx + robe_half, robe_bottom], fill=body_color)

            # Head (pale)
            skin = (200, 200, 200, 255)
            d.ellipse([cx - head_r - 1, head_cy - head_r - 1,
                        cx + head_r + 1, head_cy + head_r + 1], fill=darker(skin, 40))
            d.ellipse([cx - head_r, head_cy - head_r,
                        cx + head_r, head_cy + head_r], fill=skin)

            # Eyes (red, menacing)
            safe_pixel(img, cx - max(1, head_r // 2), head_cy, eye_color)
            safe_pixel(img, cx + max(1, head_r // 2), head_cy, eye_color)
            # Slit nose
            safe_pixel(img, cx, head_cy + 1, darker(skin, 30))

            # Wand extended
            wand_start_x = cx + robe_half + 1
            wand_start_y = robe_top + 4
            d.line([(wand_start_x, wand_start_y),
                    (wand_start_x + max(4, w // 6), wand_start_y - 3 + anim)],
                   fill=(60, 60, 70, 255), width=1)
            # Wand tip glow
            safe_pixel(img, wand_start_x + max(4, w // 6), wand_start_y - 3 + anim,
                       (0, 255, 0, 255))

    return img


def draw_projectile(w, h, color, glow_color, style='orb'):
    """Draw a spell projectile with various styles."""
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    cx, cy = w // 2, h // 2
    r = min(w, h) // 2 - 1

    if style == 'beam':
        # Elongated oval with trailing particles
        d.ellipse([1, cy - r // 2, w - 2, cy + r // 2], fill=color)
        inner_r = max(1, r // 3)
        d.ellipse([w // 3, cy - inner_r, w * 2 // 3, cy + inner_r], fill=glow_color)
        # Trailing particles
        for i in range(3):
            px = 2 + i * 2
            py = cy + (i % 2) * 2 - 1
            safe_pixel(img, px, py, (color[0], color[1], color[2], max(80, 200 - i * 50)))

    elif style == 'flame':
        # Fire shape: yellow core, orange middle, red outer
        # Outer red
        flame_pts = [(cx, 1), (w - 2, cy + r // 2), (cx + r // 2, h - 2),
                      (cx, h - r // 2), (cx - r // 2, h - 2), (1, cy + r // 2)]
        d.polygon(flame_pts, fill=(200, 50, 0, 255))
        # Middle orange
        inner_pts = [(cx, 3), (w - 4, cy + r // 3), (cx + r // 3, h - 4),
                      (cx, h - r // 3), (cx - r // 3, h - 4), (3, cy + r // 3)]
        if r > 3:
            d.polygon(inner_pts, fill=(255, 150, 0, 255))
        # Core yellow
        core_r = max(1, r // 3)
        d.ellipse([cx - core_r, cy - core_r, cx + core_r, cy + core_r],
                   fill=(255, 255, 100, 255))

    elif style == 'shield':
        # Circle outline with energy pattern
        d.ellipse([1, 1, w - 2, h - 2], outline=color, width=max(2, r // 3))
        # Inner energy lines
        for angle in range(0, 360, 45):
            ix = cx + int(math.cos(math.radians(angle)) * (r - 3))
            iy = cy + int(math.sin(math.radians(angle)) * (r - 3))
            safe_pixel(img, ix, iy, glow_color)
        # Center glow
        d.ellipse([cx - 2, cy - 2, cx + 2, cy + 2], fill=glow_color)

    elif style == 'lightning':
        # Jagged zigzag line with glow
        points = []
        segments = max(3, w // 4)
        for i in range(segments + 1):
            lx = int(i * (w - 2) / segments) + 1
            ly = cy + ((-1) ** i) * max(2, r // 2)
            points.append((lx, ly))
        d.line(points, fill=color, width=2)
        # Glow around center
        for p in points[1:-1]:
            safe_pixel(img, p[0], p[1] - 1, glow_color)
            safe_pixel(img, p[0], p[1] + 1, glow_color)

    else:
        # Orb: filled circle with outer glow ring
        # Outer glow
        if r > 2:
            d.ellipse([cx - r, cy - r, cx + r, cy + r],
                       fill=(color[0], color[1], color[2], 100))
        # Main body
        inner_r = max(1, r - 2)
        d.ellipse([cx - inner_r, cy - inner_r, cx + inner_r, cy + inner_r], fill=color)
        # Bright core
        core_r = max(1, inner_r - 2)
        d.ellipse([cx - core_r, cy - core_r, cx + core_r, cy + core_r], fill=glow_color)
        # Highlight
        safe_pixel(img, cx - core_r + 1, cy - core_r + 1, (255, 255, 255, 200))

    return img


def draw_buff_icon(bg_color, symbol_color, icon_type='circle'):
    """Draw a 32x32 buff icon with border, symbol, and inner shadow."""
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    # Border frame (2px)
    border_color = darker(bg_color, 40)
    d.rectangle([0, 0, 31, 31], fill=border_color)
    d.rectangle([2, 2, 29, 29], fill=bg_color)

    # Inner shadow (subtle darkening at bottom/right)
    shadow_color = darker(bg_color, 20)
    d.line([(2, 28), (29, 28)], fill=shadow_color)
    d.line([(2, 29), (29, 29)], fill=shadow_color)
    d.line([(28, 2), (28, 29)], fill=shadow_color)
    d.line([(29, 2), (29, 29)], fill=shadow_color)

    cx, cy = 16, 16

    if icon_type == 'potion':
        # Small bottle
        d.rectangle([13, 10, 19, 12], fill=(200, 200, 200, 255))  # neck
        d.rectangle([14, 9, 18, 10], fill=(160, 110, 60, 255))    # cork
        d.rectangle([10, 12, 22, 24], fill=symbol_color, outline=darker(symbol_color))
        safe_pixel(img, 12, 14, (255, 255, 255, 150))

    elif icon_type == 'wing':
        # Wing shape
        d.polygon([(8, 22), (16, 8), (24, 14), (20, 22)],
                   fill=symbol_color, outline=darker(symbol_color))
        d.polygon([(10, 20), (14, 10), (18, 14)],
                   fill=lighter(symbol_color, 40))

    elif icon_type == 'shield':
        # Shield shape
        d.polygon([(16, 8), (8, 12), (8, 20), (16, 26), (24, 20), (24, 12)],
                   fill=symbol_color, outline=darker(symbol_color))
        safe_pixel(img, 14, 14, lighter(symbol_color, 50))

    elif icon_type == 'eye':
        # Eye shape
        d.ellipse([8, 12, 24, 22], fill=(255, 255, 255, 255), outline=darker(symbol_color))
        d.ellipse([13, 14, 19, 20], fill=symbol_color)
        safe_pixel(img, 15, 16, (0, 0, 0, 255))

    elif icon_type == 'heart':
        # Heart
        d.ellipse([8, 10, 16, 18], fill=symbol_color)
        d.ellipse([16, 10, 24, 18], fill=symbol_color)
        d.polygon([(8, 15), (16, 24), (24, 15)], fill=symbol_color)
        safe_pixel(img, 12, 12, lighter(symbol_color, 60))

    elif icon_type == 'star':
        # 5-pointed star
        pts = []
        for i in range(10):
            angle = math.radians(i * 36 - 90)
            r = 8 if i % 2 == 0 else 4
            pts.append((cx + int(math.cos(angle) * r), cy + int(math.sin(angle) * r)))
        d.polygon(pts, fill=symbol_color, outline=darker(symbol_color))
        safe_pixel(img, cx, cy - 1, lighter(symbol_color, 50))

    elif icon_type == 'flame':
        # Fire
        d.polygon([(16, 8), (10, 18), (12, 24), (16, 20), (20, 24), (22, 18)],
                   fill=(255, 80, 0, 255), outline=(200, 40, 0, 255))
        d.polygon([(16, 11), (13, 17), (16, 15), (19, 17)],
                   fill=(255, 200, 50, 255))
        safe_pixel(img, 16, 12, (255, 255, 150, 255))

    else:
        # Circle (generic)
        d.ellipse([9, 9, 23, 23], fill=symbol_color, outline=darker(symbol_color))
        safe_pixel(img, 13, 12, lighter(symbol_color, 60))

    return img


def draw_mount_sheet(w, h, frames, mount_type, color):
    """Draw a mount spritesheet."""
    total_h = frames * (h + 2)
    img = Image.new("RGBA", (w, total_h), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    outline = darker(color, 70)

    for f in range(frames):
        y_off = f * (h + 2)
        cy = y_off + h // 2
        anim = (f % 2) * 2 - 1

        if mount_type == 'hippogriff':
            # Four-legged creature with wings and eagle head
            body_w = max(8, w // 3)
            body_h = max(4, h // 5)
            bcx = w // 2
            bcy = cy + 2

            # Body
            d.ellipse([bcx - body_w, bcy - body_h, bcx + body_w, bcy + body_h],
                       fill=color, outline=outline)

            # Wings
            wing_up = anim * 3
            d.polygon([(bcx - body_w // 2, bcy - body_h),
                        (bcx - body_w - 6, bcy - body_h - 8 - wing_up),
                        (bcx - 2, bcy - body_h + 2)],
                       fill=darker(color, 20), outline=outline)
            d.polygon([(bcx + body_w // 2, bcy - body_h),
                        (bcx + body_w + 6, bcy - body_h - 8 - wing_up),
                        (bcx + 2, bcy - body_h + 2)],
                       fill=darker(color, 20), outline=outline)

            # Eagle head
            head_r = max(2, body_h // 2 + 1)
            hx = bcx - body_w - head_r
            hy = bcy - body_h // 2
            d.ellipse([hx - head_r, hy - head_r, hx + head_r, hy + head_r],
                       fill=color, outline=outline)
            # Beak
            d.polygon([(hx - head_r, hy), (hx - head_r - 3, hy + 1), (hx - head_r, hy + 2)],
                       fill=(200, 170, 50, 255))
            # Eye
            safe_pixel(img, hx - 1, hy - 1, (200, 150, 0, 255))

            # Legs (4)
            leg_len = max(3, h // 5)
            for i in range(4):
                lx = bcx - body_w // 2 + i * (body_w // 2)
                ly = bcy + body_h
                leg_off = anim if i % 2 == 0 else -anim
                d.line([(lx, ly), (lx + leg_off, ly + leg_len)],
                       fill=outline, width=2)

            # Tail
            d.line([(bcx + body_w, bcy),
                    (bcx + body_w + 5, bcy + 3 + anim)],
                   fill=darker(color, 20), width=2)

        else:
            # Broomstick
            broom_y = cy
            # Main stick
            d.line([(5, broom_y), (w - 10, broom_y)], fill=color, width=3)
            # Outline
            d.line([(5, broom_y - 2), (w - 10, broom_y - 2)], fill=outline, width=1)
            d.line([(5, broom_y + 2), (w - 10, broom_y + 2)], fill=outline, width=1)

            # Wood grain
            for i in range(10, w - 15, 4):
                grain_c = lighter(color, 15) if (i // 4) % 2 == 0 else darker(color, 10)
                safe_pixel(img, i, broom_y, grain_c)

            # Bristles at right end
            bristle_x = w - 10
            bristle_color = (200, 180, 50, 255)
            for i in range(-3, 4):
                blen = max(3, 6 - abs(i)) + anim % 2
                d.line([(bristle_x, broom_y + i),
                        (bristle_x + blen, broom_y + i + (1 if i > 0 else -1 if i < 0 else 0))],
                       fill=bristle_color, width=1)
            # Bristle binding
            d.line([(bristle_x - 1, broom_y - 3), (bristle_x - 1, broom_y + 3)],
                   fill=darker(color, 40), width=1)

            # Speed lines
            for i in range(3):
                sx = 3 + i * 6
                sy = broom_y - 4 + i * 4
                d.line([(sx, sy), (sx - 3, sy)],
                       fill=(255, 255, 255, max(60, 150 - i * 40)), width=1)

            # Foot rest area (slightly thicker near center)
            d.line([(w // 3, broom_y - 1), (w // 3 + 4, broom_y - 1)],
                   fill=darker(color, 20), width=1)

    return img


def draw_pet_sheet(w, h, frames, pet_type, color):
    """Draw a pet spritesheet."""
    total_h = frames * (h + 2)
    img = Image.new("RGBA", (w, total_h), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    outline = darker(color, 60)
    cx = w // 2

    for f in range(frames):
        y_off = f * (h + 2)
        cy = y_off + h // 2
        anim = (f % 2) * 2 - 1

        if pet_type == 'mammal':
            # Small four-legged creature with snout (niffler-like)
            body_rx = max(3, w // 4)
            body_ry = max(2, h // 5)
            # Body
            d.ellipse([cx - body_rx - 1, cy - body_ry - 1,
                        cx + body_rx + 1, cy + body_ry + 1], fill=outline)
            d.ellipse([cx - body_rx, cy - body_ry,
                        cx + body_rx, cy + body_ry], fill=color)

            # Head/snout
            head_r = max(2, body_ry)
            hx = cx - body_rx - head_r // 2
            d.ellipse([hx - head_r, cy - head_r, hx + head_r, cy + head_r // 2],
                       fill=color, outline=outline)
            # Snout
            safe_pixel(img, hx - head_r, cy - 1, darker(color, 40))
            # Eye
            safe_pixel(img, hx - 1, cy - head_r // 2, (30, 30, 30, 255))

            # Legs (4 small)
            for i in range(4):
                lx = cx - body_rx // 2 + i * (body_rx // 2)
                ly = cy + body_ry
                leg_off = anim if i % 2 == 0 else -anim
                d.line([(lx, ly), (lx + leg_off, ly + max(2, h // 6))],
                       fill=darker(color, 30), width=1)

            # Tail
            d.line([(cx + body_rx, cy - 1),
                    (cx + body_rx + 2, cy - 3 + anim)],
                   fill=darker(color, 20), width=1)

        elif pet_type == 'elf':
            # Small house-elf figure
            head_r = max(2, min(w, h) // 5)
            head_cy = y_off + head_r + 2
            # Big head
            d.ellipse([cx - head_r - 1, head_cy - head_r - 1,
                        cx + head_r + 1, head_cy + head_r + 1], fill=outline)
            d.ellipse([cx - head_r, head_cy - head_r,
                        cx + head_r, head_cy + head_r], fill=color)
            # Big ears
            d.polygon([(cx - head_r, head_cy - 1),
                        (cx - head_r - 3, head_cy - head_r),
                        (cx - head_r + 1, head_cy - head_r + 2)],
                       fill=color, outline=outline)
            d.polygon([(cx + head_r, head_cy - 1),
                        (cx + head_r + 3, head_cy - head_r),
                        (cx + head_r - 1, head_cy - head_r + 2)],
                       fill=color, outline=outline)
            # Eyes
            safe_pixel(img, cx - 2, head_cy, (80, 200, 80, 255))
            safe_pixel(img, cx + 2, head_cy, (80, 200, 80, 255))

            # Body (small, wearing cloth)
            body_top = head_cy + head_r
            body_bottom = y_off + h - 3
            d.rectangle([cx - 3, body_top, cx + 3, body_bottom],
                         fill=(160, 140, 100, 255), outline=darker((160, 140, 100, 255)))
            # Arms
            d.line([(cx - 3, body_top + 2), (cx - 5, body_top + 4 + anim)],
                   fill=color, width=1)
            d.line([(cx + 3, body_top + 2), (cx + 5, body_top + 4 - anim)],
                   fill=color, width=1)

        else:
            # Bird (owl/snitch-like)
            body_r = max(2, min(w, h) // 4)
            # Body
            d.ellipse([cx - body_r - 1, cy - body_r - 1,
                        cx + body_r + 1, cy + body_r + 1], fill=outline)
            d.ellipse([cx - body_r, cy - body_r,
                        cx + body_r, cy + body_r], fill=color)

            # Wings in different flap positions
            wing_angle = anim * 3
            wing_len = max(3, body_r + 2)
            # Left wing
            d.polygon([(cx - body_r, cy),
                        (cx - body_r - wing_len, cy - wing_angle - 2),
                        (cx - body_r - wing_len // 2, cy + 2)],
                       fill=darker(color, 15), outline=outline)
            # Right wing
            d.polygon([(cx + body_r, cy),
                        (cx + body_r + wing_len, cy - wing_angle - 2),
                        (cx + body_r + wing_len // 2, cy + 2)],
                       fill=darker(color, 15), outline=outline)

            # Eyes
            safe_pixel(img, cx - 1, cy - 1, (30, 30, 30, 255))
            safe_pixel(img, cx + 1, cy - 1, (30, 30, 30, 255))

            # Beak/tail
            safe_pixel(img, cx, cy + 1, (200, 150, 50, 255))
            safe_pixel(img, cx, cy + body_r + 1, darker(color, 20))

    return img


def draw_armor_item(w, h, color, accent, piece):
    """Draw an armor item icon for inventory."""
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    outline = darker(color, 70)
    cx, cy = w // 2, h // 2

    if piece == 'head':
        # Hood/helmet silhouette
        r = min(w, h) // 2 - 2
        d.ellipse([cx - r - 1, cy - r - 1, cx + r + 1, cy + r + 1], fill=outline)
        d.ellipse([cx - r, cy - r, cx + r, cy + r], fill=color)
        # Point at top
        d.polygon([(cx, cy - r - 2), (cx - 3, cy - r + 2), (cx + 3, cy - r + 2)],
                   fill=color, outline=outline)
        # Opening for face
        face_r = max(1, r // 2)
        d.ellipse([cx - face_r, cy, cx + face_r, cy + face_r], fill=(0, 0, 0, 0))
        # Accent stripe
        d.line([(cx - r + 1, cy - r // 2), (cx + r - 1, cy - r // 2)],
               fill=accent, width=1)
        # Highlight
        safe_pixel(img, cx - r + 2, cy - r + 2, lighter(color, 40))

    elif piece == 'body':
        # Robe/chestpiece
        top = 1
        bottom = h - 2
        half_w = max(3, w // 3)
        shoulder_w = half_w + 2
        # Shoulders wider, tapers down
        d.polygon([(cx - shoulder_w, top + 2), (cx + shoulder_w, top + 2),
                    (cx + half_w, bottom), (cx - half_w, bottom)],
                   fill=color, outline=outline)
        # Collar accent
        d.line([(cx - shoulder_w + 2, top + 2), (cx + shoulder_w - 2, top + 2)],
               fill=accent, width=2)
        # Center line
        d.line([(cx, top + 4), (cx, bottom - 1)], fill=darker(color, 20), width=1)
        # Belt
        belt_y = (top + bottom) // 2 + 2
        d.line([(cx - half_w + 1, belt_y), (cx + half_w - 1, belt_y)],
               fill=accent, width=1)
        # Highlight
        safe_pixel(img, cx - half_w + 2, top + 4, lighter(color, 40))

    else:
        # Legs/trousers
        top = 1
        bottom = h - 2
        leg_w = max(2, w // 5)
        gap = max(1, leg_w // 2)
        # Left leg
        d.rectangle([cx - gap - leg_w - 1, top - 1, cx - gap + 1, bottom + 1], fill=outline)
        d.rectangle([cx - gap - leg_w, top, cx - gap, bottom], fill=color)
        # Right leg
        d.rectangle([cx + gap - 1, top - 1, cx + gap + leg_w + 1, bottom + 1], fill=outline)
        d.rectangle([cx + gap, top, cx + gap + leg_w, bottom], fill=color)
        # Boot cuffs (accent trim)
        boot_y = bottom - 3
        d.rectangle([cx - gap - leg_w, boot_y, cx - gap, bottom],
                     fill=darker(color, 30))
        d.rectangle([cx + gap, boot_y, cx + gap + leg_w, bottom],
                     fill=darker(color, 30))
        d.line([(cx - gap - leg_w, boot_y), (cx - gap, boot_y)], fill=accent, width=1)
        d.line([(cx + gap, boot_y), (cx + gap + leg_w, boot_y)], fill=accent, width=1)

    return img


def draw_armor_equip(w, h, color, accent, piece):
    """Draw armor equipment sprite (worn on character)."""
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    outline = darker(color, 70)
    cx = w // 2

    if piece == 'head':
        # Hood on character head (40x36 standard)
        # Rounded hood top
        d.ellipse([8, 2, 32, 28], fill=outline)
        d.ellipse([9, 3, 31, 27], fill=color)
        # Face opening
        d.ellipse([13, 14, 27, 26], fill=(0, 0, 0, 0))
        # Point/peak
        d.polygon([(cx, 0), (cx - 4, 6), (cx + 4, 6)], fill=color, outline=outline)
        # Neck drape
        d.rectangle([12, 22, 28, 34], fill=color, outline=outline)
        # Accent band
        d.line([(10, 10), (30, 10)], fill=accent, width=2)
        # Highlight
        safe_pixel(img, 12, 6, lighter(color, 50))

    elif piece == 'body':
        # Robe on character (40x54 standard)
        # Shoulders
        d.rectangle([6, 2, 34, 8], fill=color, outline=outline)
        # Main robe body
        d.polygon([(6, 8), (34, 8), (32, 50), (8, 50)], fill=color, outline=outline)
        # Arms/sleeves
        d.rectangle([2, 6, 8, 28], fill=color, outline=outline)
        d.rectangle([32, 6, 38, 28], fill=color, outline=outline)
        # Collar accent
        d.line([(10, 2), (30, 2)], fill=accent, width=2)
        d.line([(10, 4), (30, 4)], fill=accent, width=1)
        # Center fold
        d.line([(cx, 8), (cx, 48)], fill=darker(color, 25), width=1)
        # Belt
        d.line([(8, 28), (32, 28)], fill=accent, width=2)
        # Highlights
        safe_pixel(img, 10, 10, lighter(color, 40))
        safe_pixel(img, 10, 12, lighter(color, 30))

    else:
        # Legs/boots on character (40x44 standard)
        # Left leg
        d.rectangle([10, 2, 18, 40], fill=color, outline=outline)
        # Right leg
        d.rectangle([22, 2, 30, 40], fill=color, outline=outline)
        # Belt area at top
        d.line([(10, 3), (30, 3)], fill=accent, width=1)
        # Boot detail at bottom
        d.rectangle([10, 34, 18, 40], fill=darker(color, 30), outline=outline)
        d.rectangle([22, 34, 30, 40], fill=darker(color, 30), outline=outline)
        # Boot trim
        d.line([(10, 34), (18, 34)], fill=accent, width=1)
        d.line([(22, 34), (30, 34)], fill=accent, width=1)
        # Knee detail
        safe_pixel(img, 14, 18, lighter(color, 25))
        safe_pixel(img, 26, 18, lighter(color, 25))

    return img


def draw_tile_sprite(tw, th, color):
    """Draw a tile sprite for TileObjectData."""
    pw = tw * 18
    ph = th * 18
    img = Image.new("RGBA", (pw, ph), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    outline = darker(color, 60)
    highlight = lighter(color, 30)

    for tx in range(tw):
        for ty in range(th):
            x0 = tx * 18 + 1
            y0 = ty * 18 + 1
            x1 = tx * 18 + 16
            y1 = ty * 18 + 16
            # Tile outline
            d.rectangle([x0, y0, x1, y1], fill=outline)
            d.rectangle([x0 + 1, y0 + 1, x1 - 1, y1 - 1], fill=color)
            # Inner bevel/highlight
            d.line([(x0 + 1, y0 + 1), (x1 - 1, y0 + 1)], fill=highlight)
            d.line([(x0 + 1, y0 + 1), (x0 + 1, y1 - 1)], fill=highlight)
            # Shadow
            d.line([(x0 + 2, y1 - 1), (x1 - 1, y1 - 1)], fill=darker(color, 30))
            d.line([(x1 - 1, y0 + 2), (x1 - 1, y1 - 1)], fill=darker(color, 30))
            # Surface detail
            safe_pixel(img, x0 + 4, y0 + 4, lighter(color, 15))
            safe_pixel(img, x0 + 8, y0 + 8, darker(color, 10))

    return img


def draw_head_icon(w, h, skin_color, hair_color, features=None):
    """Draw a head icon for town NPC map display."""
    if features is None:
        features = {}
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    cx, cy = w // 2, h // 2
    head_r = min(w, h) // 3

    # Head
    d.ellipse([cx - head_r - 1, cy - head_r - 1,
                cx + head_r + 1, cy + head_r + 1],
               fill=darker(skin_color, 40))
    d.ellipse([cx - head_r, cy - head_r,
                cx + head_r, cy + head_r],
               fill=skin_color)

    # Hair
    hair_top = cy - head_r
    d.rectangle([cx - head_r, hair_top, cx + head_r, hair_top + max(2, head_r // 2)],
                 fill=hair_color)

    # Eyes
    eye_off = max(1, head_r // 2)
    safe_pixel(img, cx - eye_off, cy, (30, 30, 40, 255))
    safe_pixel(img, cx + eye_off, cy, (30, 30, 40, 255))

    # Mouth
    safe_pixel(img, cx, cy + max(1, head_r // 2), darker(skin_color, 30))

    if features.get('hat'):
        hat_color = features.get('hat_color', hair_color)
        d.polygon([(cx, hair_top - max(3, head_r)),
                    (cx - head_r - 1, hair_top),
                    (cx + head_r + 1, hair_top)],
                   fill=hat_color, outline=darker(hat_color))

    if features.get('beard'):
        beard_color = features.get('beard_color', hair_color)
        chin_y = cy + head_r
        d.rectangle([cx - head_r // 2, chin_y - 1, cx + head_r // 2, chin_y + max(2, head_r // 3)],
                     fill=beard_color)

    if features.get('big_ears'):
        d.polygon([(cx - head_r, cy),
                    (cx - head_r - 3, cy - head_r + 1),
                    (cx - head_r + 1, cy - head_r + 3)],
                   fill=skin_color, outline=darker(skin_color))
        d.polygon([(cx + head_r, cy),
                    (cx + head_r + 3, cy - head_r + 1),
                    (cx + head_r - 1, cy - head_r + 3)],
                   fill=skin_color, outline=darker(skin_color))

    return img


def draw_mod_icon():
    """Draw the mod icon (80x80) with a wand and magical circle."""
    img = Image.new("RGBA", (80, 80), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    # Dark background
    d.rectangle([0, 0, 79, 79], fill=(20, 5, 40, 255))
    # Border
    d.rectangle([0, 0, 79, 79], outline=(100, 50, 150, 255))
    d.rectangle([1, 1, 78, 78], outline=(60, 20, 100, 255))

    # Magical circle
    cx, cy = 40, 40
    for r in range(28, 24, -1):
        alpha = 100 + (28 - r) * 40
        circ_color = (200, 170, 50, min(255, alpha))
        d.ellipse([cx - r, cy - r, cx + r, cy + r], outline=circ_color)

    # Inner star pattern
    for i in range(5):
        angle = math.radians(i * 72 - 90)
        sx = cx + int(math.cos(angle) * 16)
        sy = cy + int(math.sin(angle) * 16)
        angle2 = math.radians((i + 2) * 72 - 90)
        ex = cx + int(math.cos(angle2) * 16)
        ey = cy + int(math.sin(angle2) * 16)
        d.line([(sx, sy), (ex, ey)], fill=(255, 215, 0, 180), width=1)

    # Wand across the icon
    d.line([(18, 62), (62, 18)], fill=(100, 60, 30, 255), width=3)
    d.line([(19, 61), (61, 19)], fill=(139, 90, 43, 255), width=2)

    # Wand tip glow
    for r in range(6, 0, -1):
        glow_alpha = min(255, 60 + (6 - r) * 40)
        d.ellipse([62 - r, 18 - r, 62 + r, 18 + r],
                   fill=(255, 255, 200, glow_alpha))

    # "WW" text approximation (pixel art letters)
    # W at left
    for dy in range(5):
        safe_pixel(img, 10, 10 + dy, (255, 215, 0, 255))
        safe_pixel(img, 14, 10 + dy, (255, 215, 0, 255))
    safe_pixel(img, 11, 14, (255, 215, 0, 255))
    safe_pixel(img, 12, 13, (255, 215, 0, 255))
    safe_pixel(img, 13, 14, (255, 215, 0, 255))

    return img


def draw_broom_item(w, h, color, bristle_color=(200, 180, 50, 255)):
    """Draw a broomstick item sprite."""
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    outline = darker(color, 60)
    cy = h // 2

    # Stick outline
    d.line([(2, cy), (w - 6, cy)], fill=outline, width=4)
    # Stick body
    d.line([(2, cy), (w - 6, cy)], fill=color, width=2)
    # Wood grain
    for i in range(4, w - 8, 3):
        grain_c = lighter(color, 15) if (i // 3) % 2 == 0 else darker(color, 10)
        safe_pixel(img, i, cy, grain_c)

    # Bristles
    bx = w - 5
    for i in range(-2, 3):
        d.line([(bx, cy + i), (bx + 3, cy + i + (1 if i > 0 else -1 if i < 0 else 0))],
               fill=bristle_color, width=1)
    # Binding
    safe_pixel(img, bx - 1, cy - 2, darker(color, 40))
    safe_pixel(img, bx - 1, cy - 1, darker(color, 40))
    safe_pixel(img, bx - 1, cy, darker(color, 40))
    safe_pixel(img, bx - 1, cy + 1, darker(color, 40))
    safe_pixel(img, bx - 1, cy + 2, darker(color, 40))

    # Handle nub at left end
    d.ellipse([0, cy - 2, 3, cy + 2], fill=darker(color, 30))

    return img


def draw_generic_item(w, h, color, shape='rect', detail_color=None):
    """Draw a generic item icon with various shapes and detail."""
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    outline = darker(color, 70)
    cx, cy = w // 2, h // 2
    if detail_color is None:
        detail_color = lighter(color, 40)

    if shape == 'round':
        r = min(w, h) // 2 - 2
        d.ellipse([cx - r - 1, cy - r - 1, cx + r + 1, cy + r + 1], fill=outline)
        d.ellipse([cx - r, cy - r, cx + r, cy + r], fill=color)
        # Shine
        safe_pixel(img, cx - r // 2, cy - r // 2, lighter(color, 60))
        safe_pixel(img, cx - r // 2 + 1, cy - r // 2, lighter(color, 40))

    elif shape == 'egg':
        # Egg shape (taller than wide)
        ry = min(w, h) // 2 - 2
        rx = max(1, ry * 2 // 3)
        d.ellipse([cx - rx - 1, cy - ry - 1, cx + rx + 1, cy + ry + 1], fill=outline)
        d.ellipse([cx - rx, cy - ry, cx + rx, cy + ry], fill=color)
        # Shine
        safe_pixel(img, cx - rx // 2, cy - ry // 2, lighter(color, 70))

    elif shape == 'fang':
        # Pointed fang shape
        d.polygon([(cx, 2), (cx - w // 3, h // 3), (cx, h - 2), (cx + w // 3, h // 3)],
                   fill=color, outline=outline)
        # Drip detail
        safe_pixel(img, cx, h - 3, detail_color)
        # Highlight
        safe_pixel(img, cx - 2, h // 4, lighter(color, 50))

    elif shape == 'cloak':
        # Flowing cloak shape
        d.polygon([(cx - 2, 2), (w - 3, 4), (w - 2, h - 3), (2, h - 2), (3, 4)],
                   fill=color, outline=outline)
        # Shimmer pixels (transparency effect)
        for i in range(3):
            sx = cx - 2 + i * 3
            sy = h // 3 + i * 2
            safe_pixel(img, sx, sy, lighter(color, 40 + i * 15))

    elif shape == 'scroll':
        # Scroll/map shape
        margin = max(2, w // 6)
        d.rectangle([margin, margin, w - margin, h - margin], fill=color, outline=outline)
        # Scroll rolls at top and bottom
        d.ellipse([margin - 1, margin - 2, w - margin + 1, margin + 2], fill=lighter(color, 20), outline=outline)
        d.ellipse([margin - 1, h - margin - 2, w - margin + 1, h - margin + 2], fill=lighter(color, 20), outline=outline)
        # Lines on the map
        for ly in range(margin + 4, h - margin - 2, 3):
            d.line([(margin + 3, ly), (w - margin - 3, ly)], fill=darker(color, 30), width=1)
        # Detail mark
        safe_pixel(img, cx, cy, detail_color)

    elif shape == 'gem':
        # Gem/stone shape (diamond-ish)
        d.polygon([(cx, 2), (w - 3, cy), (cx, h - 3), (3, cy)],
                   fill=color, outline=outline)
        # Facet lines
        d.line([(cx, 2), (cx, h - 3)], fill=lighter(color, 30), width=1)
        d.line([(3, cy), (w - 3, cy)], fill=lighter(color, 20), width=1)
        # Bright facet
        safe_pixel(img, cx - 2, cy - 2, lighter(color, 60))

    elif shape == 'cup':
        # Cup/chalice
        cup_w = max(3, w // 3)
        cup_top = max(2, h // 4)
        cup_bottom = h - 3
        # Cup body
        d.polygon([(cx - cup_w, cup_top), (cx + cup_w, cup_top),
                    (cx + cup_w - 1, cup_bottom), (cx - cup_w + 1, cup_bottom)],
                   fill=color, outline=outline)
        # Rim
        d.line([(cx - cup_w, cup_top), (cx + cup_w, cup_top)], fill=lighter(color, 40), width=1)
        # Stem
        d.line([(cx, cup_bottom), (cx, cup_bottom + 1)], fill=outline, width=1)
        # Base
        d.line([(cx - 2, h - 2), (cx + 2, h - 2)], fill=outline, width=1)
        # Emblem
        safe_pixel(img, cx, (cup_top + cup_bottom) // 2, detail_color)
        # Highlight
        safe_pixel(img, cx - cup_w + 2, cup_top + 2, lighter(color, 50))

    elif shape == 'crown':
        # Diadem/tiara/crown
        band_y = cy + 1
        d.rectangle([3, band_y, w - 4, band_y + 3], fill=color, outline=outline)
        # Points
        points = max(3, w // 8)
        for i in range(points):
            px = 4 + i * ((w - 8) // max(1, points - 1))
            d.polygon([(px - 1, band_y), (px, band_y - max(3, h // 4)), (px + 1, band_y)],
                       fill=color, outline=outline)
        # Gems
        safe_pixel(img, cx, band_y + 1, detail_color)
        safe_pixel(img, cx - 3, band_y + 1, detail_color)
        safe_pixel(img, cx + 3, band_y + 1, detail_color)

    elif shape == 'ears':
        # Extendable ears (pair of ear shapes on a string)
        # String
        d.line([(cx - 4, 2), (cx + 4, 2)], fill=(180, 160, 100, 255), width=1)
        d.line([(cx - 4, 2), (cx - 4, cy - 2)], fill=(180, 160, 100, 255), width=1)
        d.line([(cx + 4, 2), (cx + 4, cy - 2)], fill=(180, 160, 100, 255), width=1)
        # Left ear
        d.ellipse([2, cy - 1, cx - 1, h - 2], fill=color, outline=outline)
        safe_pixel(img, cx // 2, cy + 2, darker(color, 20))
        # Right ear
        d.ellipse([cx + 1, cy - 1, w - 3, h - 2], fill=color, outline=outline)
        safe_pixel(img, cx + (w - cx) // 2, cy + 2, darker(color, 20))

    elif shape == 'letter':
        # Letter/envelope (howler)
        margin = max(2, min(w, h) // 6)
        d.rectangle([margin, margin + 2, w - margin, h - margin], fill=color, outline=outline)
        # Flap
        d.polygon([(margin, margin + 2), (cx, cy + 1), (w - margin, margin + 2)],
                   fill=lighter(color, 20), outline=outline)
        # Seal
        safe_pixel(img, cx, cy + 2, detail_color)

    elif shape == 'orb':
        # Crystal ball / remembrall
        r = min(w, h) // 2 - 2
        d.ellipse([cx - r - 1, cy - r - 1, cx + r + 1, cy + r + 1], fill=outline)
        d.ellipse([cx - r, cy - r, cx + r, cy + r], fill=color)
        # Inner swirl
        for i in range(3):
            sx = cx + int(math.cos(i * 2.1) * r // 2)
            sy = cy + int(math.sin(i * 2.1) * r // 2)
            safe_pixel(img, sx, sy, detail_color)
        # Glass highlight
        safe_pixel(img, cx - r // 2, cy - r // 2, (255, 255, 255, 180))
        safe_pixel(img, cx - r // 2 + 1, cy - r // 2, (255, 255, 255, 120))

    elif shape == 'feather':
        # Feather shape
        # Central shaft
        d.line([(cx, 2), (cx - 2, h - 2)], fill=darker(color, 30), width=1)
        # Barbs
        for i in range(3, h - 3, 2):
            t = i / h
            barb_w = int(w * 0.3 * (1 - abs(t - 0.5) * 2))
            by = i
            d.line([(cx - 2 - barb_w, by + 1), (cx - int(2 * t), by)], fill=color, width=1)
            d.line([(cx - int(2 * t), by), (cx - 2 + barb_w, by - 1)], fill=color, width=1)
        # Tip
        safe_pixel(img, cx, 2, lighter(color, 40))

    elif shape == 'bat_item':
        # Baseball/beater's bat
        # Handle
        d.rectangle([2, cy - 1, w // 3, cy + 1], fill=darker(color, 20), outline=darker(color, 50))
        # Bat head (wider)
        d.ellipse([w // 3 - 2, cy - h // 3, w - 2, cy + h // 3], fill=color, outline=outline)
        # Wood grain
        safe_pixel(img, w // 2, cy, lighter(color, 20))
        safe_pixel(img, w // 2 + 2, cy + 1, lighter(color, 15))

    elif shape == 'staff':
        # Wizard staff (vertical with orb on top)
        # Shaft
        d.line([(cx, h // 4), (cx, h - 2)], fill=color, width=3)
        d.line([(cx, h // 4), (cx, h - 2)], fill=lighter(color, 15), width=1)
        # Orb
        orb_r = max(2, w // 5)
        d.ellipse([cx - orb_r - 1, 1, cx + orb_r + 1, 1 + orb_r * 2 + 1],
                   fill=outline)
        d.ellipse([cx - orb_r, 2, cx + orb_r, 2 + orb_r * 2],
                   fill=detail_color)
        safe_pixel(img, cx - 1, orb_r, lighter(detail_color, 50))
        # Prongs holding orb
        d.line([(cx - orb_r, 2 + orb_r * 2), (cx - 1, h // 4)], fill=color, width=1)
        d.line([(cx + orb_r, 2 + orb_r * 2), (cx + 1, h // 4)], fill=color, width=1)

    elif shape == 'bell':
        # Small bell
        d.ellipse([cx - w // 3, cy - h // 4, cx + w // 3, cy + h // 3],
                   fill=color, outline=outline)
        # Handle
        d.arc([cx - 3, 2, cx + 3, cy - h // 4 + 4], 0, 180, fill=outline, width=1)
        # Clapper
        safe_pixel(img, cx, cy + h // 4, darker(color, 50))
        # Highlight
        safe_pixel(img, cx - w // 4, cy - 1, lighter(color, 50))

    elif shape == 'powder':
        # Pouch/bag of powder
        d.ellipse([3, cy - 2, w - 4, h - 2], fill=color, outline=outline)
        # Gathered top
        d.polygon([(cx - 3, cy - 1), (cx, 2), (cx + 3, cy - 1)],
                   fill=lighter(color, 20), outline=outline)
        # String tie
        d.line([(cx - 1, cy - 3), (cx + 1, cy - 3)], fill=(180, 160, 100, 255))
        # Sparkle
        safe_pixel(img, cx + 2, cy + 2, detail_color)

    elif shape == 'key':
        # Portkey (glowing key-like object)
        # Ring at top
        d.ellipse([cx - 3, 2, cx + 3, 8], outline=color, width=1)
        # Shaft
        d.line([(cx, 8), (cx, h - 5)], fill=color, width=2)
        # Teeth at bottom
        d.line([(cx, h - 5), (cx + 3, h - 5)], fill=color, width=1)
        d.line([(cx + 3, h - 5), (cx + 3, h - 3)], fill=color, width=1)
        d.line([(cx, h - 3), (cx + 2, h - 3)], fill=color, width=1)
        # Glow
        safe_pixel(img, cx - 1, 4, lighter(color, 60))
        safe_pixel(img, cx + 1, h - 6, detail_color)

    elif shape == 'dagger':
        # Dagger / fang dagger
        # Blade
        d.polygon([(cx, 2), (cx - 2, h // 2), (cx + 2, h // 2)],
                   fill=lighter(color, 30), outline=outline)
        # Blood/venom drip on blade
        safe_pixel(img, cx, h // 3, detail_color)
        # Guard
        d.line([(cx - 4, h // 2), (cx + 4, h // 2)], fill=darker(color, 20), width=2)
        # Handle
        d.rectangle([cx - 1, h // 2 + 2, cx + 1, h - 3], fill=darker(color, 30), outline=outline)
        # Pommel
        d.ellipse([cx - 2, h - 4, cx + 2, h - 1], fill=darker(color, 40))

    else:
        # Default rectangle with detail
        margin = max(1, min(w, h) // 6)
        d.rectangle([margin - 1, margin - 1, w - margin, h - margin], fill=outline)
        d.rectangle([margin, margin, w - margin - 1, h - margin - 1], fill=color)
        # Highlight
        safe_pixel(img, margin + 1, margin + 1, lighter(color, 50))
        # Detail
        safe_pixel(img, cx, cy, detail_color)

    return img


# ============================================================
# SPRITE GENERATION
# ============================================================
print("=== Wizarding World Sprite Generator ===\n")

# --- MOD ICON ---
print("[Mod Icon]")
save(draw_mod_icon(), "icon.png")

# --- WANDS ---
print("\n[Wands]")
wands = {
    "OakWand": ((139, 90, 43, 255), (255, 255, 200, 255), 'simple'),
    "WillowWand": ((100, 140, 80, 255), (200, 220, 255, 255), 'simple'),
    "AshWand": ((160, 100, 60, 255), (255, 100, 50, 255), 'simple'),
    "PhoenixFeatherWand": ((180, 60, 30, 255), (255, 150, 50, 255), 'knob'),
    "DragonHeartstringWand": ((80, 40, 80, 255), (200, 50, 50, 255), 'knob'),
    "UnicornHairWand": ((220, 220, 240, 255), (200, 200, 255, 255), 'simple'),
    "ElderWand": ((60, 60, 70, 255), (150, 255, 150, 255), 'knob'),
}
for name, (color, core, style) in wands.items():
    save(draw_wand(36, 36, color, core, style), "Content", "Items", "Weapons", "Wands", f"{name}.png")

# Elder Wand at 42x42 (overwrites the 36x36 above)
save(draw_wand(42, 42, (60, 60, 70, 255), (150, 255, 150, 255), 'knob'),
     "Content", "Items", "Weapons", "Wands", "ElderWand.png")

# --- SWORD ---
print("\n[Weapons]")
save(draw_sword(48, 48, (192, 192, 192, 255), (139, 90, 43, 255), gem_color=(200, 50, 50, 255)),
     "Content", "Items", "Weapons", "SwordOfGryffindor.png")

# Nimbus broomstick item
save(draw_broom_item(40, 20, (139, 90, 43, 255)),
     "Content", "Items", "Weapons", "NimbusBroomstick.png")

# --- ARMOR SETS ---
print("\n[Armor]")
houses = {
    "Gryffindor": ((120, 0, 0, 255), (255, 215, 0, 255)),
    "Slytherin": ((0, 100, 50, 255), (192, 192, 192, 255)),
    "Ravenclaw": ((0, 50, 150, 255), (180, 140, 80, 255)),
    "Hufflepuff": ((200, 170, 50, 255), (50, 50, 50, 255)),
    "DarkWizard": ((30, 10, 40, 255), (100, 0, 150, 255)),
}
for house, (main_color, accent) in houses.items():
    hood_name = f"{house}Hood"
    robes_name = f"{house}Robes"
    legs_name = f"{house}Leggings"

    # Item sprites (inventory icons)
    save(draw_armor_item(18, 18, main_color, accent, 'head'),
         "Content", "Items", "Armor", house, f"{hood_name}.png")
    save(draw_armor_item(18, 18, main_color, accent, 'body'),
         "Content", "Items", "Armor", house, f"{robes_name}.png")
    save(draw_armor_item(18, 18, main_color, accent, 'legs'),
         "Content", "Items", "Armor", house, f"{legs_name}.png")

    # Equipment sprites (worn on character)
    save(draw_armor_equip(40, 36, main_color, accent, 'head'),
         "Content", "Items", "Armor", house, f"{hood_name}_Head.png")
    save(draw_armor_equip(40, 54, main_color, accent, 'body'),
         "Content", "Items", "Armor", house, f"{robes_name}_Body.png")
    save(draw_armor_equip(40, 44, main_color, accent, 'legs'),
         "Content", "Items", "Armor", house, f"{legs_name}_Legs.png")

# --- ACCESSORIES ---
print("\n[Accessories]")
save(draw_accessory(24, 24, (0, 120, 0, 255), shape='circle', detail_color=(200, 255, 200, 255)),
     "Content", "Items", "Accessories", "BasiliskFang.png")
save(draw_generic_item(24, 24, (200, 200, 220, 128), shape='cloak'),
     "Content", "Items", "Accessories", "InvisibilityCloak.png")
save(draw_accessory(24, 24, (255, 215, 0, 255), shape='pendant', detail_color=(200, 170, 50, 255)),
     "Content", "Items", "Accessories", "TimeTurner.png")
save(draw_generic_item(24, 24, (200, 180, 140, 255), shape='scroll'),
     "Content", "Items", "Accessories", "MaraudersMap.png")
save(draw_accessory(24, 24, (50, 50, 50, 255), shape='circle', detail_color=(100, 100, 100, 255)),
     "Content", "Items", "Accessories", "ResurrectionStone.png")

# --- CONSUMABLES ---
print("\n[Consumables]")
save(draw_tile_sprite(1, 1, (75, 0, 130, 255)),
     "Content", "Items", "Consumables", "EnchantingTableItem.png")
save(draw_generic_item(20, 20, (0, 100, 0, 255), shape='fang', detail_color=(0, 200, 0, 255)),
     "Content", "Items", "Consumables", "BasiliskSummonItem.png")
save(draw_generic_item(20, 20, (200, 80, 20, 255), shape='egg', detail_color=(255, 150, 50, 255)),
     "Content", "Items", "Consumables", "HorntailSummonItem.png")
save(draw_generic_item(20, 20, (40, 40, 40, 255), shape='gem', detail_color=(100, 0, 0, 255)),
     "Content", "Items", "Consumables", "VoldemortSummonItem.png")
save(draw_generic_item(20, 20, (150, 50, 20, 255), shape='round', detail_color=(200, 100, 50, 255)),
     "Content", "Items", "Consumables", "DragonScale.png")
save(draw_generic_item(20, 20, (255, 215, 0, 255), shape='egg', detail_color=(255, 255, 150, 255)),
     "Content", "Items", "Consumables", "GoldenEgg.png")

# Potions
print("\n[Potions]")
save(draw_potion(20, 26, (200, 160, 80, 255)),
     "Content", "Items", "Consumables", "Potions", "Butterbeer.png")
save(draw_potion(20, 26, (255, 215, 0, 255)),
     "Content", "Items", "Consumables", "Potions", "FelixFelicis.png")
save(draw_potion(20, 26, (100, 130, 80, 255)),
     "Content", "Items", "Consumables", "Potions", "PolyjuicePotion.png")
save(draw_potion(20, 26, (80, 80, 120, 255)),
     "Content", "Items", "Consumables", "Potions", "WolfsbanePotion.png")

# --- PROJECTILES ---
print("\n[Projectiles]")
save(draw_projectile(12, 12, (255, 255, 200, 255), (255, 255, 255, 255), style='orb'),
     "Content", "Projectiles", "Spells", "LumosProjectile.png")
save(draw_projectile(12, 12, (200, 30, 30, 255), (255, 80, 80, 255), style='orb'),
     "Content", "Projectiles", "Spells", "StupefyProjectile.png")
save(draw_projectile(12, 12, (255, 215, 0, 255), (255, 255, 200, 255), style='orb'),
     "Content", "Projectiles", "Spells", "ExpelliarmusProjectile.png")
save(draw_projectile(12, 12, (255, 100, 0, 255), (255, 200, 50, 255), style='flame'),
     "Content", "Projectiles", "Spells", "IncendioProjectile.png")
save(draw_projectile(30, 30, (192, 192, 192, 255), (240, 240, 255, 255), style='orb'),
     "Content", "Projectiles", "Spells", "PatronusProjectile.png")
save(draw_projectile(12, 12, (120, 0, 60, 255), (180, 0, 80, 255), style='beam'),
     "Content", "Projectiles", "Spells", "SectumsempraProjectile.png")
save(draw_projectile(16, 16, (0, 200, 0, 255), (150, 255, 150, 255), style='orb'),
     "Content", "Projectiles", "Spells", "AvadaKedavraProjectile.png")

# --- BUFFS ---
print("\n[Buffs]")
save(draw_buff_icon((200, 160, 80, 255), (255, 200, 100, 255), 'potion'),
     "Content", "Buffs", "ButterbeerBuff.png")
save(draw_buff_icon((255, 215, 0, 255), (255, 255, 200, 255), 'star'),
     "Content", "Buffs", "FelixFelicisBuff.png")
save(draw_buff_icon((100, 130, 80, 255), (200, 200, 200, 255), 'potion'),
     "Content", "Buffs", "PolyjuiceBuff.png")
save(draw_buff_icon((80, 80, 120, 255), (200, 200, 200, 255), 'potion'),
     "Content", "Buffs", "WolfsbaneBuff.png")
save(draw_buff_icon((139, 90, 43, 255), (255, 255, 200, 255), 'wing'),
     "Content", "Buffs", "NimbusMountBuff.png")
save(draw_buff_icon((255, 215, 0, 255), (255, 255, 255, 255), 'wing'),
     "Content", "Buffs", "GoldenSnitchBuff.png")

# --- TILES ---
print("\n[Tiles]")
save(draw_tile_sprite(3, 2, (75, 0, 130, 255)),
     "Content", "Tiles", "EnchantingTable.png")

# --- ENEMIES ---
print("\n[Enemies]")
save(draw_enemy_sheet(30, 50, 4, (30, 30, 40, 255), (100, 100, 120, 255), shape='ghost'),
     "Content", "NPCs", "Enemies", "Dementor.png")
save(draw_enemy_sheet(40, 30, 4, (60, 40, 20, 255), (80, 60, 40, 255), shape='spider'),
     "Content", "NPCs", "Enemies", "Acromantula.png")
save(draw_enemy_sheet(18, 20, 4, (100, 150, 255, 255), (200, 220, 255, 255), shape='bat'),
     "Content", "NPCs", "Enemies", "MagicPixie.png")
save(draw_enemy_sheet(36, 56, 6, (100, 100, 90, 255), (130, 130, 120, 255), shape='humanoid'),
     "Content", "NPCs", "Enemies", "Troll.png")
save(draw_enemy_sheet(18, 40, 3, (60, 50, 70, 255), (100, 80, 110, 255), shape='humanoid'),
     "Content", "NPCs", "Enemies", "Inferius.png")

# --- BOSSES ---
print("\n[Bosses]")
# Basilisk
save(draw_boss_sheet(60, 60, 6, (0, 80, 0, 255), (255, 255, 0, 255), 'serpent'),
     "Content", "NPCs", "Bosses", "Basilisk", "BasiliskBoss.png")
save(draw_head_icon(32, 32, (0, 80, 0, 255), (0, 60, 0, 255)),
     "Content", "NPCs", "Bosses", "Basilisk", "BasiliskBoss_Head_Boss.png")

# Horntail
save(draw_boss_sheet(80, 80, 6, (120, 60, 20, 255), (255, 100, 0, 255), 'dragon'),
     "Content", "NPCs", "Bosses", "Horntail", "HorntailBoss.png")
save(draw_head_icon(32, 32, (120, 60, 20, 255), (100, 40, 10, 255)),
     "Content", "NPCs", "Bosses", "Horntail", "HorntailBoss_Head_Boss.png")

# Voldemort
save(draw_boss_sheet(40, 60, 6, (30, 30, 40, 255), (200, 0, 0, 255), 'wizard'),
     "Content", "NPCs", "Bosses", "Voldemort", "VoldemortBoss.png")
save(draw_head_icon(32, 32, (200, 200, 200, 255), (30, 30, 40, 255)),
     "Content", "NPCs", "Bosses", "Voldemort", "VoldemortBoss_Head_Boss.png")

# --- TOWN NPCs ---
print("\n[Town NPCs]")
# Ollivander: elderly wandmaker with white hair
save(draw_npc_sheet(40, 56, 25, (220, 180, 140, 255), (80, 60, 40, 255), (200, 200, 200, 255),
                    features={'weapon': True, 'weapon_color': (139, 90, 43, 255)}),
     "Content", "NPCs", "Town", "Ollivander.png")
save(draw_head_icon(36, 36, (220, 180, 140, 255), (200, 200, 200, 255)),
     "Content", "NPCs", "Town", "Ollivander_Head.png")

# Hagrid: large, bushy brown hair and beard
save(draw_npc_sheet(40, 56, 25, (200, 160, 120, 255), (100, 70, 40, 255), (80, 50, 30, 255),
                    features={'beard': True, 'beard_color': (80, 50, 30, 255)}),
     "Content", "NPCs", "Town", "Hagrid.png")
save(draw_head_icon(36, 36, (200, 160, 120, 255), (80, 50, 30, 255),
                    features={'beard': True, 'beard_color': (80, 50, 30, 255)}),
     "Content", "NPCs", "Town", "Hagrid_Head.png")

# PotionsMaster: dark robes, black hair
save(draw_npc_sheet(40, 56, 25, (220, 190, 150, 255), (20, 20, 30, 255), (10, 10, 15, 255)),
     "Content", "NPCs", "Town", "PotionsMaster.png")
save(draw_head_icon(36, 36, (220, 190, 150, 255), (10, 10, 15, 255)),
     "Content", "NPCs", "Town", "PotionsMaster_Head.png")

# --- PET ---
print("\n[Pet]")
save(draw_pet_sheet(20, 20, 4, 'bird', (255, 215, 0, 255)),
     "Content", "Pets", "GoldenSnitch", "GoldenSnitchProjectile.png")
save(draw_generic_item(20, 20, (255, 215, 0, 255), shape='round'),
     "Content", "Pets", "GoldenSnitch", "GoldenSnitchItem.png")

# --- MOUNT ---
print("\n[Mount]")
save(draw_mount_sheet(80, 50, 4, 'broom', (139, 90, 43, 255)),
     "Content", "Mounts", "NimbusMount.png")

# ============================================================
# PHASE 2 CONTENT
# ============================================================

# --- PHASE 2 WANDS ---
print("\n[Phase 2 Wands]")
phase2_wands = {
    "HollyWand": ((139, 69, 19, 255), (255, 50, 50, 255), 'simple'),
    "VineWand": ((34, 139, 34, 255), (200, 255, 200, 255), 'simple'),
    "YewWand": ((60, 40, 40, 255), (100, 0, 100, 255), 'knob'),
}
for name, (color, core, style) in phase2_wands.items():
    save(draw_wand(38, 38, color, core, style), "Content", "Items", "Weapons", "Wands", f"{name}.png")

# --- PHASE 2 WEAPONS ---
print("\n[Phase 2 Weapons]")
save(draw_generic_item(40, 40, (139, 90, 43, 255), shape='bat_item'),
     "Content", "Items", "Weapons", "BeatersBat.png")
save(draw_generic_item(20, 20, (180, 40, 40, 255), shape='round'),
     "Content", "Items", "Weapons", "Quaffle.png")

# Firebolt broomstick
save(draw_broom_item(40, 20, (180, 50, 20, 255), bristle_color=(255, 100, 30, 255)),
     "Content", "Items", "Weapons", "FireboltBroomstick.png")

save(draw_generic_item(30, 24, (120, 100, 80, 255), shape='feather'),
     "Content", "Items", "Weapons", "HippogriffFeather.png")
save(draw_generic_item(36, 36, (200, 80, 20, 255), shape='staff', detail_color=(255, 150, 50, 255)),
     "Content", "Items", "Weapons", "PhoenixFeatherStaff.png")

# --- PHASE 2 ACCESSORIES ---
print("\n[Phase 2 Accessories]")
save(draw_accessory(24, 24, (40, 40, 50, 255), shape='square', detail_color=(80, 80, 100, 255)),
     "Content", "Items", "Accessories", "RiddlesDiary.png")
save(draw_accessory(24, 24, (0, 100, 50, 255), shape='pendant', detail_color=(0, 200, 100, 255)),
     "Content", "Items", "Accessories", "SlytherinsLocket.png")
save(draw_generic_item(24, 24, (200, 170, 50, 255), shape='cup', detail_color=(150, 120, 30, 255)),
     "Content", "Items", "Accessories", "HufflepuffsCup.png")
save(draw_generic_item(24, 24, (100, 100, 200, 255), shape='crown', detail_color=(150, 150, 255, 255)),
     "Content", "Items", "Accessories", "DiademOfRavenclaw.png")

# --- PHASE 2 PROJECTILES (Spells) ---
print("\n[Phase 2 Projectiles - Spells]")
save(draw_projectile(60, 60, (100, 150, 255, 255), (200, 220, 255, 255), style='shield'),
     "Content", "Projectiles", "Spells", "ProtegoProjectile.png")
save(draw_projectile(14, 14, (200, 200, 255, 255), (255, 255, 255, 255), style='orb'),
     "Content", "Projectiles", "Spells", "WingardiumProjectile.png")
save(draw_projectile(10, 10, (200, 30, 30, 255), (255, 80, 80, 255), style='lightning'),
     "Content", "Projectiles", "Spells", "CrucioProjectile.png")
save(draw_projectile(14, 14, (80, 130, 255, 255), (150, 200, 255, 255), style='orb'),
     "Content", "Projectiles", "Spells", "ReductoProjectile.png")
save(draw_projectile(10, 10, (50, 100, 200, 255), (150, 200, 255, 255), style='beam'),
     "Content", "Projectiles", "Spells", "AguamentiProjectile.png")

# --- PHASE 2 PROJECTILES (Other) ---
print("\n[Phase 2 Projectiles - Other]")
save(draw_generic_item(16, 16, (180, 40, 40, 255), shape='round'),
     "Content", "Projectiles", "QuaffleProjectile.png")

# PhoenixMinion
phoenix_w, phoenix_h = 30, 30
save(draw_pet_sheet(phoenix_w, phoenix_h, 4, 'bird', (255, 120, 30, 255)),
     "Content", "Projectiles", "PhoenixMinion.png")

# --- PHASE 2 ENEMIES ---
print("\n[Phase 2 Enemies]")
save(draw_enemy_sheet(40, 44, 4, (60, 60, 70, 255), (100, 100, 110, 255), shape='bat'),
     "Content", "NPCs", "Enemies", "Thestral.png")
save(draw_enemy_sheet(30, 46, 4, (30, 30, 40, 255), (80, 80, 100, 255), shape='humanoid'),
     "Content", "NPCs", "Enemies", "DeathEater.png")

# --- PHASE 2 BUFFS ---
print("\n[Phase 2 Buffs]")
save(draw_buff_icon((120, 100, 80, 255), (200, 180, 140, 255), 'wing'),
     "Content", "Buffs", "HippogriffMountBuff.png")
save(draw_buff_icon((200, 80, 20, 255), (255, 150, 50, 255), 'wing'),
     "Content", "Buffs", "FireboltMountBuff.png")
save(draw_buff_icon((255, 120, 30, 255), (255, 200, 50, 255), 'flame'),
     "Content", "Buffs", "PhoenixMinionBuff.png")
save(draw_buff_icon((80, 60, 40, 255), (255, 215, 0, 255), 'heart'),
     "Content", "Buffs", "NifflerBuff.png")

# --- PHASE 2 MOUNTS ---
print("\n[Phase 2 Mounts]")
save(draw_mount_sheet(80, 50, 4, 'hippogriff', (120, 100, 80, 255)),
     "Content", "Mounts", "HippogriffMount.png")
save(draw_mount_sheet(80, 50, 4, 'broom', (180, 50, 20, 255)),
     "Content", "Mounts", "FireboltMount.png")

# --- PHASE 2 PET (Niffler) ---
print("\n[Phase 2 Pet - Niffler]")
save(draw_pet_sheet(20, 20, 4, 'mammal', (80, 60, 40, 255)),
     "Content", "Pets", "Niffler", "NifflerProjectile.png")
save(draw_generic_item(20, 20, (80, 60, 40, 255), shape='round'),
     "Content", "Pets", "Niffler", "NifflerItem.png")

# ============================================================
# PHASE 3 CONTENT
# ============================================================

# --- PHASE 3 TOWN NPCs ---
print("\n[Phase 3 Town NPCs]")
save(draw_npc_sheet(40, 56, 25, (180, 150, 120, 255), (120, 100, 60, 255), (120, 100, 60, 255),
                    features={'big_ears': True}),
     "Content", "NPCs", "Town", "Dobby.png")
save(draw_head_icon(36, 36, (180, 150, 120, 255), (120, 100, 60, 255),
                    features={'big_ears': True}),
     "Content", "NPCs", "Town", "Dobby_Head.png")

# --- PHASE 3 ACCESSORIES ---
print("\n[Phase 3 Accessories]")
save(draw_accessory(26, 26, (80, 40, 20, 255), shape='triangle', detail_color=(200, 170, 50, 255)),
     "Content", "Items", "Accessories", "SortingHat.png")

# --- PHASE 3 ENEMIES ---
print("\n[Phase 3 Enemies]")
save(draw_enemy_sheet(20, 28, 4, (50, 120, 30, 255), (80, 160, 50, 255), shape='blob'),
     "Content", "NPCs", "Enemies", "Mandrake.png")
save(draw_enemy_sheet(24, 36, 4, (60, 50, 70, 255), (100, 80, 120, 255), shape='ghost'),
     "Content", "NPCs", "Enemies", "Boggart.png")

# ============================================================
# PHASE 4 CONTENT
# ============================================================

# --- PHASE 4 WANDS ---
print("\n[Phase 4 Wands]")
save(draw_wand(42, 42, (180, 50, 20, 255), (255, 150, 0, 255), 'knob'),
     "Content", "Items", "Weapons", "Wands", "FiendfyreWand.png")

# --- PHASE 4 WEAPONS ---
print("\n[Phase 4 Weapons]")
save(draw_generic_item(20, 16, (200, 40, 40, 255), shape='letter', detail_color=(255, 100, 100, 255)),
     "Content", "Items", "Weapons", "Howler.png")

# --- PHASE 4 ACCESSORIES ---
print("\n[Phase 4 Accessories]")
save(draw_accessory(24, 24, (150, 100, 200, 255), shape='circle', detail_color=(200, 150, 255, 255)),
     "Content", "Items", "Accessories", "ApparitionCharm.png")

# --- PHASE 4 PROJECTILES ---
print("\n[Phase 4 Projectiles]")
save(draw_projectile(20, 20, (255, 100, 0, 255), (255, 200, 50, 255), style='flame'),
     "Content", "Projectiles", "Spells", "FiendfyreProjectile.png")
save(draw_projectile(16, 16, (200, 40, 40, 255), (255, 100, 100, 255), style='beam'),
     "Content", "Projectiles", "HowlerProjectile.png")

# --- PHASE 4 TOWN NPCs ---
print("\n[Phase 4 Town NPCs]")
save(draw_npc_sheet(40, 56, 25, (220, 190, 160, 255), (100, 50, 150, 255), (180, 180, 180, 255),
                    features={'hat': True, 'hat_color': (100, 50, 150, 255),
                              'beard': True, 'beard_color': (180, 180, 180, 255),
                              'weapon': True}),
     "Content", "NPCs", "Town", "Dumbledore.png")
save(draw_head_icon(36, 36, (220, 190, 160, 255), (180, 180, 180, 255),
                    features={'hat': True, 'hat_color': (100, 50, 150, 255),
                              'beard': True, 'beard_color': (180, 180, 180, 255)}),
     "Content", "NPCs", "Town", "Dumbledore_Head.png")

# ============================================================
# PHASE 5 CONTENT
# ============================================================

# --- PHASE 5 PET (Hedwig) ---
print("\n[Phase 5 Pet - Hedwig]")
save(draw_pet_sheet(20, 20, 4, 'bird', (240, 240, 240, 255)),
     "Content", "Pets", "Hedwig", "HedwigProjectile.png")
save(draw_generic_item(20, 20, (240, 240, 240, 255), shape='round', detail_color=(200, 200, 200, 255)),
     "Content", "Pets", "Hedwig", "HedwigItem.png")

# --- PHASE 5 ENEMIES ---
print("\n[Phase 5 Enemies]")
save(draw_enemy_sheet(30, 48, 6, (80, 60, 40, 255), (100, 80, 60, 255), shape='humanoid'),
     "Content", "NPCs", "Enemies", "Werewolf.png")
save(draw_enemy_sheet(24, 24, 4, (40, 100, 80, 255), (60, 140, 100, 255), shape='blob'),
     "Content", "NPCs", "Enemies", "Grindylow.png")

# --- PHASE 5 CONSUMABLES ---
print("\n[Phase 5 Consumables]")
save(draw_generic_item(20, 20, (100, 60, 30, 255), shape='round', detail_color=(70, 40, 20, 255)),
     "Content", "Items", "Consumables", "ChocolateFrog.png")
save(draw_generic_item(16, 20, (200, 50, 50, 255), shape='rect', detail_color=(255, 200, 50, 255)),
     "Content", "Items", "Consumables", "BertieBottsBeans.png")

# --- PHASE 5 BUFFS ---
print("\n[Phase 5 Buffs]")
save(draw_buff_icon((240, 240, 240, 255), (255, 255, 200, 255), 'wing'),
     "Content", "Buffs", "HedwigBuff.png")

# ============================================================
# PHASE 6 CONTENT
# ============================================================

# --- PHASE 6 ENEMIES ---
print("\n[Phase 6 Enemies]")
save(draw_enemy_sheet(14, 14, 4, (100, 50, 120, 255), (140, 80, 160, 255), shape='bat'),
     "Content", "NPCs", "Enemies", "Doxy.png")

# --- PHASE 6 PROJECTILES ---
print("\n[Phase 6 Projectiles]")
save(draw_pet_sheet(20, 30, 4, 'elf', (180, 150, 120, 255)),
     "Content", "Projectiles", "HouseElfMinion.png")
save(draw_projectile(16, 16, (50, 200, 50, 255), (150, 255, 150, 255), style='orb'),
     "Content", "Projectiles", "Spells", "ReparoProjectile.png")

# --- PHASE 6 WEAPONS ---
print("\n[Phase 6 Weapons]")
save(draw_generic_item(24, 24, (200, 170, 50, 255), shape='bell'),
     "Content", "Items", "Weapons", "HouseElfBell.png")
save(draw_wand(36, 36, (140, 120, 80, 255), (100, 255, 100, 255)),
     "Content", "Items", "Weapons", "Wands", "ElmWand.png")

# --- PHASE 6 BUFFS ---
print("\n[Phase 6 Buffs]")
save(draw_buff_icon((180, 150, 120, 255), (255, 220, 180, 255), 'circle'),
     "Content", "Buffs", "HouseElfMinionBuff.png")
save(draw_buff_icon((200, 200, 220, 255), (255, 255, 255, 255), 'eye'),
     "Content", "Buffs", "VeritaserumBuff.png")
save(draw_buff_icon((255, 150, 180, 255), (255, 200, 220, 255), 'heart'),
     "Content", "Buffs", "AmortentiaBuff.png")

# --- PHASE 6 POTIONS ---
print("\n[Phase 6 Potions]")
save(draw_potion(20, 26, (200, 200, 220, 255)),
     "Content", "Items", "Consumables", "Potions", "Veritaserum.png")
save(draw_potion(20, 26, (255, 150, 180, 255)),
     "Content", "Items", "Consumables", "Potions", "Amortentia.png")

# ============================================================
# PHASE 7 CONTENT
# ============================================================

# --- PHASE 7 ENEMIES ---
print("\n[Phase 7 Enemies]")
save(draw_enemy_sheet(24, 38, 4, (150, 150, 170, 255), (200, 200, 220, 255), shape='ghost'),
     "Content", "NPCs", "Enemies", "Peeves.png")

# --- PHASE 7 BUFFS ---
print("\n[Phase 7 Buffs]")
save(draw_buff_icon((40, 120, 80, 255), (100, 200, 150, 255), 'potion'),
     "Content", "Buffs", "GillyweedBuff.png")

# --- PHASE 7 POTIONS ---
print("\n[Phase 7 Potions]")
save(draw_potion(20, 26, (40, 120, 80, 255)),
     "Content", "Items", "Consumables", "Potions", "Gillyweed.png")

# --- PHASE 7 ACCESSORIES ---
print("\n[Phase 7 Accessories]")
save(draw_generic_item(20, 20, (200, 200, 210, 255), shape='orb', detail_color=(255, 50, 50, 255)),
     "Content", "Items", "Accessories", "Remembrall.png")

# --- PHASE 7 PROJECTILES ---
print("\n[Phase 7 Projectiles]")
save(draw_projectile(14, 14, (200, 30, 30, 255), (255, 120, 120, 255), style='orb'),
     "Content", "Projectiles", "Spells", "ChainStupefyProjectile.png")

# --- PHASE 7 WEAPONS ---
print("\n[Phase 7 Weapons]")
save(draw_wand(38, 38, (160, 140, 100, 255), (200, 30, 30, 255)),
     "Content", "Items", "Weapons", "Wands", "HawthornWand.png")
save(draw_generic_item(32, 32, (0, 100, 0, 255), shape='dagger', detail_color=(0, 200, 0, 255)),
     "Content", "Items", "Weapons", "BasiliskFangDagger.png")

# ============================================================
# PHASE 8 CONTENT
# ============================================================

# --- PHASE 8 ENEMIES ---
print("\n[Phase 8 Enemies]")
save(draw_enemy_sheet(40, 30, 6, (40, 80, 20, 255), (60, 100, 30, 255), shape='serpent'),
     "Content", "NPCs", "Enemies", "Nagini.png")

# --- PHASE 8 PROJECTILES ---
print("\n[Phase 8 Projectiles]")
save(draw_projectile(14, 14, (150, 50, 200, 255), (200, 100, 255, 255), style='orb'),
     "Content", "Projectiles", "Spells", "ImpedimentaProjectile.png")
save(draw_projectile(10, 10, (255, 100, 50, 255), (255, 200, 100, 255), style='flame'),
     "Content", "Projectiles", "WeasleyFireworkProjectile.png")

# --- PHASE 8 WEAPONS ---
print("\n[Phase 8 Weapons]")
save(draw_generic_item(20, 20, (255, 100, 50, 255), shape='round', detail_color=(255, 200, 100, 255)),
     "Content", "Items", "Weapons", "WeasleyFireworks.png")
save(draw_wand(38, 38, (40, 30, 30, 255), (150, 50, 200, 255), 'knob'),
     "Content", "Items", "Weapons", "Wands", "BlackthornWand.png")

# --- PHASE 8 ACCESSORIES ---
print("\n[Phase 8 Accessories]")
save(draw_generic_item(28, 28, (180, 180, 200, 255), shape='round', detail_color=(140, 140, 160, 255)),
     "Content", "Items", "Accessories", "Pensieve.png")

# --- PHASE 8 ARMOR (Wizard Robes - Vanity) ---
print("\n[Phase 8 Armor - Wizard Robes]")
save(draw_armor_item(18, 18, (50, 0, 100, 255), (200, 170, 50, 255), 'head'),
     "Content", "Items", "Armor", "WizardRobes", "WizardHat.png")
save(draw_armor_equip(40, 36, (50, 0, 100, 255), (200, 170, 50, 255), 'head'),
     "Content", "Items", "Armor", "WizardRobes", "WizardHat_Head.png")
save(draw_armor_item(18, 18, (50, 0, 100, 255), (200, 170, 50, 255), 'body'),
     "Content", "Items", "Armor", "WizardRobes", "WizardRobe.png")
save(draw_armor_equip(40, 54, (50, 0, 100, 255), (200, 170, 50, 255), 'body'),
     "Content", "Items", "Armor", "WizardRobes", "WizardRobe_Body.png")
save(draw_armor_item(18, 18, (50, 0, 100, 255), (200, 170, 50, 255), 'legs'),
     "Content", "Items", "Armor", "WizardRobes", "WizardBoots.png")
save(draw_armor_equip(40, 44, (50, 0, 100, 255), (200, 170, 50, 255), 'legs'),
     "Content", "Items", "Armor", "WizardRobes", "WizardBoots_Legs.png")

# ============================================================
# PHASE 9 CONTENT
# ============================================================

# --- PHASE 9 CONSUMABLES ---
print("\n[Phase 9 Consumables]")
save(draw_generic_item(20, 20, (150, 150, 200, 255), shape='key', detail_color=(100, 100, 180, 255)),
     "Content", "Items", "Consumables", "Portkey.png")

# --- PHASE 9 ACCESSORIES ---
print("\n[Phase 9 Accessories]")
save(draw_generic_item(22, 22, (200, 160, 120, 255), shape='ears'),
     "Content", "Items", "Accessories", "ExtendableEars.png")

# --- PHASE 9 PROJECTILES ---
print("\n[Phase 9 Projectiles]")
save(draw_projectile(24, 24, (255, 255, 200, 255), (255, 255, 255, 255), style='orb'),
     "Content", "Projectiles", "Spells", "LumosMaximaProjectile.png")
save(draw_projectile(16, 16, (30, 30, 40, 255), (60, 60, 80, 255), style='orb'),
     "Content", "Projectiles", "DarknessPowderProjectile.png")

# --- PHASE 9 WANDS ---
print("\n[Phase 9 Wands]")
save(draw_wand(36, 36, (160, 100, 60, 255), (255, 255, 150, 255)),
     "Content", "Items", "Weapons", "Wands", "CedarWand.png")

# --- PHASE 9 ENEMIES ---
print("\n[Phase 9 Enemies]")
save(draw_enemy_sheet(40, 40, 4, (30, 30, 50, 255), (60, 60, 80, 255), shape='ghost'),
     "Content", "NPCs", "Enemies", "Obscurus.png")

# --- PHASE 9 WEAPONS ---
print("\n[Phase 9 Weapons]")
save(draw_generic_item(18, 18, (40, 40, 50, 255), shape='powder', detail_color=(80, 80, 100, 255)),
     "Content", "Items", "Weapons", "PeruvianDarknessPowder.png")

# --- PHASE 10 ACCESSORIES & CONSUMABLES ---
print("\n[Phase 10 Accessories]")
save(draw_accessory(24, 24, (200, 50, 50, 255), shape='circle', detail_color=(255, 215, 0, 255)),
     "Content", "Items", "Accessories", "PhilosophersStone.png")
save(draw_generic_item(20, 20, (192, 192, 192, 255), shape='rect', detail_color=(255, 255, 200, 255)),
     "Content", "Items", "Accessories", "Deluminator.png")
save(draw_generic_item(20, 20, (200, 200, 210, 255), shape='round', detail_color=(230, 230, 240, 255)),
     "Content", "Items", "Accessories", "Sneakoscope.png")
save(draw_generic_item(28, 28, (255, 215, 0, 255), shape='cup', detail_color=(255, 255, 180, 255)),
     "Content", "Items", "Accessories", "TriwizardCup.png")

print("\n[Phase 10 Consumables]")
save(draw_generic_item(16, 16, (50, 180, 50, 255), shape='powder', detail_color=(120, 255, 120, 255)),
     "Content", "Items", "Consumables", "FlooPowder.png")

# ============================================================
# PHASE 11 CONTENT
# ============================================================

# --- PHASE 11 DEBUFFS ---
print("\n[Phase 11 Debuffs]")
save(draw_buff_icon((150, 150, 150, 255), (100, 100, 100, 255), icon_type='shield'),
     "Content", "Buffs", "Debuffs", "PetrifiedDebuff.png")
save(draw_buff_icon((120, 50, 160, 255), (200, 100, 255, 255), icon_type='star'),
     "Content", "Buffs", "Debuffs", "JinxedDebuff.png")
save(draw_buff_icon((20, 40, 20, 255), (0, 200, 0, 255), icon_type='flame'),
     "Content", "Buffs", "Debuffs", "DarkCurseDebuff.png")

# --- PHASE 11 ENEMIES ---
print("\n[Phase 11 Enemies]")
save(draw_enemy_sheet(12, 16, 4, (60, 140, 40, 255), (20, 60, 20, 255), shape='humanoid'),
     "Content", "NPCs", "Enemies", "Bowtruckle.png")
save(draw_enemy_sheet(16, 8, 2, (160, 140, 100, 255), (120, 100, 60, 255), shape='serpent'),
     "Content", "NPCs", "Enemies", "Flobberworm.png")
save(draw_enemy_sheet(44, 30, 4, (120, 80, 40, 255), (255, 100, 0, 255), shape='spider'),
     "Content", "NPCs", "Enemies", "BlastEndedSkrewt.png")

# --- PHASE 11 CONSUMABLES ---
print("\n[Phase 11 Consumables]")
save(draw_generic_item(16, 20, (60, 140, 40, 255), shape='round'),
     "Content", "Items", "Consumables", "BowtruckleCatch.png")

# --- PHASE 11 TOWN NPCs ---
print("\n[Phase 11 Town NPCs]")
save(draw_npc_sheet(40, 56, 25, (180, 140, 100, 255), (100, 80, 50, 255), (50, 30, 10, 255)),
     "Content", "NPCs", "Town", "Centaur.png")
save(draw_head_icon(36, 36, (180, 140, 100, 255), (50, 30, 10, 255)),
     "Content", "NPCs", "Town", "Centaur_Head.png")

# --- PHASE 11 ACCESSORIES ---
print("\n[Phase 11 Accessories]")
save(draw_generic_item(22, 22, (200, 50, 50, 255), shape='round'),
     "Content", "Items", "Accessories", "ShieldHat.png")
save(draw_generic_item(20, 20, (255, 215, 0, 255), shape='round'),
     "Content", "Items", "Accessories", "PrefectBadge.png")

# --- PHASE 11 WEAPONS ---
print("\n[Phase 11 Weapons]")
save(draw_generic_item(18, 18, (200, 170, 50, 255), shape='round'),
     "Content", "Items", "Weapons", "DecoyDetonator.png")

# --- PHASE 11 PROJECTILES ---
print("\n[Phase 11 Projectiles]")
save(draw_projectile(14, 14, (200, 170, 50, 255), (255, 220, 100, 255), style='orb'),
     "Content", "Projectiles", "DecoyDetonatorProjectile.png")

# ============================================================
# PHASE 12 CONTENT
# ============================================================

# --- PHASE 12 CONSUMABLES ---
print("\n[Phase 12 Consumables]")
save(draw_generic_item(16, 16, (150, 50, 255, 255), shape='gem'),
     "Content", "Items", "Consumables", "EssenceOfMagic.png")
save(draw_generic_item(20, 20, (30, 50, 30, 255), shape='round'),
     "Content", "Items", "Consumables", "DarkMarkSummon.png")

# --- PHASE 12 WEAPONS ---
print("\n[Phase 12 Weapons]")
save(draw_wand(44, 44, (40, 40, 50, 255), (200, 150, 255, 255)),
     "Content", "Items", "Weapons", "Wands", "WandOfDestiny.png")
save(draw_wand(38, 38, (140, 100, 60, 255), (255, 200, 50, 255)),
     "Content", "Items", "Weapons", "Wands", "CypressWand.png")

# --- PHASE 12 BUFFS ---
print("\n[Phase 12 Buffs]")
save(draw_buff_icon((192, 192, 220, 255), (240, 240, 255, 255), icon_type='shield'),
     "Content", "Buffs", "PatronusCharmBuff.png")

# --- PHASE 12 PROJECTILES ---
print("\n[Phase 12 Projectiles]")
save(draw_pet_sheet(36, 36, 4, 'mammal', (200, 200, 240, 255)),
     "Content", "Projectiles", "PatronusGuardian.png")
save(draw_projectile(12, 12, (255, 200, 50, 255), (255, 255, 150, 255), style='orb'),
     "Content", "Projectiles", "Spells", "ConjunctivitisProjectile.png")

# --- PHASE 12 ACCESSORIES ---
print("\n[Phase 12 Accessories]")
save(draw_accessory(28, 28, (192, 192, 220, 255), shape='circle', detail_color=(240, 240, 255, 255)),
     "Content", "Items", "Accessories", "PatronusCharm.png")
save(draw_accessory(24, 24, (180, 150, 50, 255), shape='pendant', detail_color=(255, 215, 0, 255)),
     "Content", "Items", "Accessories", "UnbreakableVow.png")

# --- PHASE 12 ENEMIES ---
print("\n[Phase 12 Enemies]")
save(draw_enemy_sheet(34, 56, 4, (20, 20, 30, 255), (100, 100, 150, 255), shape='ghost'),
     "Content", "NPCs", "Enemies", "AzkabanGuard.png")

# ============================================================
# PHASE 13 CONTENT
# ============================================================

# --- PHASE 13 ENEMIES ---
print("\n[Phase 13 Enemies]")
save(draw_enemy_sheet(24, 38, 4, (50, 120, 100, 255), (80, 200, 180, 255), shape='humanoid'),
     "Content", "NPCs", "Enemies", "Merfolk.png")

# --- PHASE 13 PROJECTILES ---
print("\n[Phase 13 Projectiles]")
save(draw_projectile(16, 16, (255, 150, 50, 255), (255, 220, 100, 255), style='flame'),
     "Content", "Projectiles", "Spells", "BombardaProjectile.png")
save(draw_projectile(14, 14, (150, 50, 200, 255), (200, 100, 255, 255), style='orb'),
     "Content", "Projectiles", "Spells", "LevicorpusProjectile.png")
save(draw_projectile(14, 14, (255, 100, 0, 255), (255, 200, 50, 255), style='flame'),
     "Content", "Projectiles", "BlazeBoxProjectile.png")

# --- PHASE 13 WANDS ---
print("\n[Phase 13 Wands]")
save(draw_wand(36, 36, (160, 80, 40, 255), (255, 150, 50, 255)),
     "Content", "Items", "Weapons", "Wands", "RowanWand.png")
save(draw_wand(36, 36, (220, 210, 190, 255), (150, 50, 200, 255)),
     "Content", "Items", "Weapons", "Wands", "BirchWand.png")

# --- PHASE 13 CONSUMABLES ---
print("\n[Phase 13 Consumables]")
save(draw_generic_item(18, 18, (100, 200, 100, 255), shape='round'),
     "Content", "Items", "Consumables", "PeppermintToad.png")
save(draw_generic_item(18, 18, (100, 60, 30, 255), shape='cup'),
     "Content", "Items", "Consumables", "ChocolateCauldron.png")

# --- PHASE 13 WEAPONS ---
print("\n[Phase 13 Weapons]")
save(draw_generic_item(18, 18, (200, 80, 20, 255), shape='rect'),
     "Content", "Items", "Weapons", "WeasleyBlazeBox.png")

# --- PHASE 14 PROJECTILES ---
print("\n[Phase 14 Projectiles]")
save(draw_projectile(80, 80, (255, 215, 0, 255), (255, 255, 200, 255), style='shield'),
     "Content", "Projectiles", "Spells", "FiniteIncantatemProjectile.png")
save(draw_projectile(40, 40, (150, 150, 255, 255), (200, 200, 255, 255), style='orb'),
     "Content", "Projectiles", "Spells", "AccioProjectile.png")
save(draw_projectile(16, 16, (255, 200, 50, 255), (255, 150, 200, 255), style='orb'),
     "Content", "Projectiles", "Spells", "RiddikulusProjectile.png")

# --- PHASE 14 WANDS ---
print("\n[Phase 14 Wands]")
save(draw_wand(36, 36, (180, 140, 80, 255), (255, 215, 0, 255)),
     "Content", "Items", "Weapons", "Wands", "LarchWand.png")
save(draw_wand(36, 36, (180, 60, 40, 255), (255, 200, 100, 255)),
     "Content", "Items", "Weapons", "Wands", "RedOakWand.png")

# --- PHASE 14 BUFFS ---
print("\n[Phase 14 Buffs]")
save(draw_buff_icon((255, 100, 50, 255), (255, 200, 100, 255), icon_type='flame'),
     "Content", "Buffs", "PepperupBuff.png")
save(draw_buff_icon((200, 200, 200, 255), (255, 255, 255, 255), icon_type='heart'),
     "Content", "Buffs", "SkeleGroBuff.png")

# --- PHASE 14 POTIONS ---
print("\n[Phase 14 Potions]")
save(draw_potion(20, 26, (255, 100, 50, 255)),
     "Content", "Items", "Consumables", "Potions", "PepperupPotion.png")
save(draw_potion(20, 26, (200, 200, 200, 255)),
     "Content", "Items", "Consumables", "Potions", "SkeleGro.png")

# --- PHASE 14 ENEMIES ---
print("\n[Phase 14 Enemies]")
save(draw_enemy_sheet(48, 40, 4, (200, 180, 120, 255), (255, 200, 50, 255), shape='spider'),
     "Content", "NPCs", "Enemies", "Sphinx.png")
save(draw_enemy_sheet(50, 80, 6, (100, 90, 80, 255), (60, 60, 50, 255), shape='humanoid'),
     "Content", "NPCs", "Enemies", "Giant.png")

# --- PHASE 15 BIOME ---
print("\n[Phase 15 Biome]")
save(draw_buff_icon((20, 60, 20, 255), (50, 120, 50, 255), icon_type='eye'),
     "Content", "Biomes", "ForbiddenForestBiome_Icon.png")

# --- PHASE 15 ACCESSORIES ---
print("\n[Phase 15 Accessories]")
save(draw_generic_item(20, 14, (180, 180, 200, 255), shape='round'),
     "Content", "Items", "Accessories", "WizardSpectacles.png")
save(draw_generic_item(20, 14, (180, 180, 200, 255), shape='round'),
     "Content", "Items", "Accessories", "WizardSpectacles_Face.png")
save(draw_generic_item(16, 16, (200, 50, 50, 255), shape='round'),
     "Content", "Items", "Accessories", "LightningScar.png")
save(draw_generic_item(16, 16, (200, 50, 50, 255), shape='round'),
     "Content", "Items", "Accessories", "LightningScar_Face.png")
save(draw_generic_item(18, 24, (200, 200, 210, 255), shape='round'),
     "Content", "Items", "Accessories", "WizardBeard.png")
save(draw_generic_item(18, 24, (200, 200, 210, 255), shape='round'),
     "Content", "Items", "Accessories", "WizardBeard_Face.png")

# --- PHASE 15 ENEMIES ---
print("\n[Phase 15 Enemies]")
save(draw_enemy_sheet(20, 18, 4, (255, 100, 150, 255), (50, 50, 50, 255), shape='bat'),
     "Content", "NPCs", "Enemies", "Fwooper.png")

# --- PHASE 15 PET ---
print("\n[Phase 15 Pet]")
save(draw_pet_sheet(20, 20, 4, 'mammal', (140, 100, 60, 255)),
     "Content", "Pets", "Kneazle", "KneazleProjectile.png")
save(draw_generic_item(20, 20, (140, 100, 60, 255), shape='round'),
     "Content", "Pets", "Kneazle", "KneazleItem.png")
save(draw_buff_icon((140, 100, 60, 255), (200, 170, 100, 255), icon_type='eye'),
     "Content", "Buffs", "KneazleBuff.png")

# --- PHASE 15 CONSUMABLES ---
print("\n[Phase 15 Consumables]")
save(draw_generic_item(24, 20, (220, 200, 170, 255), shape='scroll'),
     "Content", "Items", "Consumables", "DailyProphet.png")

# --- PHASE 17 ENEMIES ---
print("\n[Phase 17 Enemies]")
save(draw_enemy_sheet(24, 44, 1, (139, 90, 43, 255), (200, 200, 200, 255), shape='humanoid'),
     "Content", "NPCs", "Enemies", "DuelingDummy.png")

# --- PHASE 17 CONSUMABLES ---
print("\n[Phase 17 Consumables]")
save(draw_generic_item(24, 32, (139, 90, 43, 255), shape='rect'),
     "Content", "Items", "Consumables", "DuelingDummyItem.png")
save(draw_generic_item(24, 20, (200, 200, 210, 255), shape='rect'),
     "Content", "Items", "Consumables", "WizardChessSet.png")

# ============================================================
# PHASE 18 SPRITES
# ============================================================

# --- PHASE 18 CONSUMABLES ---
print("\n[Phase 18 Consumables]")
save(draw_generic_item(20, 16, (220, 200, 170, 255), shape='scroll'),
     "Content", "Items", "Consumables", "HogwartsLetter.png")

# --- PHASE 18 ENEMIES ---
print("\n[Phase 18 Enemies]")
save(draw_enemy_sheet(26, 40, 4, (180, 200, 240, 255), (150, 200, 255, 255), shape='ghost'),
     "Content", "NPCs", "Enemies", "SnowWraith.png")
save(draw_enemy_sheet(24, 44, 4, (180, 160, 100, 255), (0, 200, 0, 255), shape='humanoid'),
     "Content", "NPCs", "Enemies", "CursedMummy.png")

# --- PHASE 18 PROJECTILES ---
print("\n[Phase 18 Projectiles]")
save(draw_projectile(16, 16, (150, 50, 200, 255), (200, 100, 255, 255), style='lightning'),
     "Content", "Projectiles", "Spells", "ApparitionProjectile.png")

# --- PHASE 18 WANDS ---
print("\n[Phase 18 Wands]")
save(draw_wand(38, 38, (30, 25, 25, 255), (150, 50, 200, 255)),
     "Content", "Items", "Weapons", "Wands", "EbonyWand.png")

# ============================================================
# PHASE 19 SPRITES — Boss Loot (Bags, Trophies, Expert Items)
# ============================================================

# --- PHASE 19 BASILISK BOSS LOOT ---
print("\n[Phase 19 Basilisk Boss Loot]")
save(draw_generic_item(32, 32, (0, 120, 0, 255), shape='round'),
     "Content", "Items", "BossLoot", "Basilisk", "BasiliskBag.png")
save(draw_accessory(20, 20, (200, 200, 0, 255), shape='circle', detail_color=(0, 100, 0, 255)),
     "Content", "Items", "BossLoot", "Basilisk", "BasiliskEye.png")
save(draw_generic_item(30, 30, (0, 100, 0, 255), shape='rect'),
     "Content", "Items", "BossLoot", "Basilisk", "BasiliskTrophy.png")

# --- PHASE 19 HORNTAIL BOSS LOOT ---
print("\n[Phase 19 Horntail Boss Loot]")
save(draw_generic_item(32, 32, (180, 80, 20, 255), shape='round'),
     "Content", "Items", "BossLoot", "Horntail", "HorntailBag.png")
save(draw_accessory(24, 24, (200, 50, 30, 255), shape='circle', detail_color=(255, 100, 50, 255)),
     "Content", "Items", "BossLoot", "Horntail", "DragonHeart.png")
save(draw_generic_item(30, 30, (150, 60, 20, 255), shape='rect'),
     "Content", "Items", "BossLoot", "Horntail", "HorntailTrophy.png")

# --- PHASE 19 VOLDEMORT BOSS LOOT ---
print("\n[Phase 19 Voldemort Boss Loot]")
save(draw_generic_item(32, 32, (40, 40, 50, 255), shape='round'),
     "Content", "Items", "BossLoot", "Voldemort", "VoldemortBag.png")
save(draw_accessory(24, 24, (30, 30, 40, 255), shape='circle', detail_color=(0, 200, 0, 255)),
     "Content", "Items", "BossLoot", "Voldemort", "SoulFragment.png")
save(draw_generic_item(30, 30, (40, 40, 50, 255), shape='rect'),
     "Content", "Items", "BossLoot", "Voldemort", "VoldemortTrophy.png")

# ============================================================
# PHASE 20 SPRITES
# ============================================================

# --- PHASE 20 CONSUMABLES ---
print("\n[Phase 20 Consumables]")
save(draw_generic_item(24, 28, (100, 50, 30, 255), shape='rect'),
     "Content", "Items", "Consumables", "SpellBook.png")

# HogwartsLetter.png already generated in Phase 18 — skip
hogwarts_letter_path = os.path.join(BASE, "Content", "Items", "Consumables", "HogwartsLetter.png")
if os.path.exists(hogwarts_letter_path):
    print("  HogwartsLetter.png already exists (Phase 18) — skipped")
else:
    save(draw_generic_item(20, 16, (220, 200, 170, 255), shape='scroll'),
         "Content", "Items", "Consumables", "HogwartsLetter.png")

# ============================================================
# PHASE 22 SPRITES
# ============================================================

# --- PHASE 22 WEAPONS ---
print("\n[Phase 22 Weapons]")
save(draw_sword(32, 32, (0, 100, 50, 255), (192, 192, 192, 255), gem_color=(0, 200, 0, 255)),
     "Content", "Items", "Weapons", "SlytherinsDagger.png")
save(draw_wand(40, 40, (0, 50, 150, 255), (100, 150, 255, 255)),
     "Content", "Items", "Weapons", "RavenclawsStaff.png")
save(draw_sword(44, 44, (160, 140, 80, 255), (80, 60, 30, 255), gem_color=(200, 170, 50, 255)),
     "Content", "Items", "Weapons", "HufflepuffsMace.png")

# --- PHASE 22 CONSUMABLES ---
print("\n[Phase 22 Consumables]")
save(draw_generic_item(16, 20, (180, 200, 240, 255), shape='round'),
     "Content", "Items", "Consumables", "UnicornBlood.png")

# --- PHASE 22 ACCESSORIES ---
print("\n[Phase 22 Accessories]")
save(draw_generic_item(30, 30, (255, 215, 0, 255), shape='rect'),
     "Content", "Items", "Accessories", "MasterWizardBanner.png")

# --- PHASE 22 PLACEABLE ---
print("\n[Phase 22 Placeable]")
save(draw_generic_item(28, 24, (255, 215, 0, 255), shape='cup'),
     "Content", "Items", "Placeable", "FelixCauldron.png")

# ============================================================
# PHASE 23 SPRITES
# ============================================================

# --- PHASE 23 WEAPONS ---
print("\n[Phase 23 Weapons]")
save(draw_wand(36, 36, (160, 120, 70, 255), (255, 255, 180, 255)),
     "Content", "Items", "Weapons", "Wands", "AlderWand.png")

# ============================================================
# PHASE 24 SPRITES
# ============================================================

# --- PHASE 24 ARMOR: DRAGON SCALE ---
print("\n[Phase 24 Armor]")
save(draw_armor_item(18, 18, (180, 80, 30, 255), (255, 150, 50, 255), 'head'),
     "Content", "Items", "Armor", "DragonScale", "DragonScaleHelm.png")
save(draw_armor_equip(40, 36, (180, 80, 30, 255), (255, 150, 50, 255), 'head'),
     "Content", "Items", "Armor", "DragonScale", "DragonScaleHelm_Head.png")
save(draw_armor_item(18, 18, (180, 80, 30, 255), (255, 150, 50, 255), 'body'),
     "Content", "Items", "Armor", "DragonScale", "DragonScaleBreastplate.png")
save(draw_armor_equip(40, 54, (180, 80, 30, 255), (255, 150, 50, 255), 'body'),
     "Content", "Items", "Armor", "DragonScale", "DragonScaleBreastplate_Body.png")
save(draw_armor_item(18, 18, (180, 80, 30, 255), (255, 150, 50, 255), 'legs'),
     "Content", "Items", "Armor", "DragonScale", "DragonScaleGreaves.png")
save(draw_armor_equip(40, 44, (180, 80, 30, 255), (255, 150, 50, 255), 'legs'),
     "Content", "Items", "Armor", "DragonScale", "DragonScaleGreaves_Legs.png")

# --- PHASE 24 MOUNT: NIMBUS 2001 ---
print("\n[Phase 24 Mount]")
save(draw_mount_sheet(80, 50, 4, 'broom', (192, 192, 200, 255)),
     "Content", "Mounts", "Nimbus2001Mount.png")

# --- PHASE 24 BUFF ---
print("\n[Phase 24 Buff]")
save(draw_buff_icon((192, 192, 200, 255), (255, 255, 255, 255), icon_type='wing'),
     "Content", "Buffs", "Nimbus2001MountBuff.png")

# --- PHASE 24 ITEMS ---
print("\n[Phase 24 Items]")
save(draw_generic_item(40, 20, (192, 192, 200, 255), shape='rect'),
     "Content", "Items", "Weapons", "Nimbus2001Broomstick.png")

# --- PHASE 25 NPCs ---
print("\n[Phase 25 NPCs]")
# Fred and George Weasley: ginger hair, orange Weasley robes
save(draw_npc_sheet(40, 56, 25, (220, 160, 120, 255), (200, 80, 20, 255), (200, 100, 30, 255)),
     "Content", "NPCs", "Town", "FredAndGeorge.png")
save(draw_head_icon(36, 36, (220, 160, 120, 255), (200, 80, 20, 255)),
     "Content", "NPCs", "Town", "FredAndGeorge_Head.png")

# --- REMAINING TOWN NPCs (previously generated ad-hoc, now standardized at 25 frames) ---
print("\n[Remaining Town NPCs]")
# Aberforth: elderly, grey-blue robes, white beard
save(draw_npc_sheet(40, 56, 25, (210, 175, 140, 255), (80, 90, 110, 255), (190, 190, 190, 255),
                    features={'beard': True, 'beard_color': (190, 190, 190, 255)}),
     "Content", "NPCs", "Town", "Aberforth.png")
save(draw_head_icon(36, 36, (210, 175, 140, 255), (190, 190, 190, 255),
                    features={'beard': True, 'beard_color': (190, 190, 190, 255)}),
     "Content", "NPCs", "Town", "Aberforth_Head.png")

# GoblinTeller: small, dark banker robes, pointed features
save(draw_npc_sheet(40, 56, 25, (200, 180, 140, 255), (30, 30, 40, 255), (60, 50, 40, 255)),
     "Content", "NPCs", "Town", "GoblinTeller.png")
save(draw_head_icon(36, 36, (200, 180, 140, 255), (60, 50, 40, 255)),
     "Content", "NPCs", "Town", "GoblinTeller_Head.png")

# Healer: white robes with green trim, light hair
save(draw_npc_sheet(40, 56, 25, (220, 190, 160, 255), (220, 220, 230, 255), (180, 140, 80, 255),
                    features={'weapon': True, 'weapon_color': (100, 200, 100, 255)}),
     "Content", "NPCs", "Town", "Healer.png")
save(draw_head_icon(36, 36, (220, 190, 160, 255), (180, 140, 80, 255)),
     "Content", "NPCs", "Town", "Healer_Head.png")

# Kingsley: dark skin, purple Auror robes, bald
save(draw_npc_sheet(40, 56, 25, (120, 80, 50, 255), (80, 40, 120, 255), (80, 60, 40, 255)),
     "Content", "NPCs", "Town", "Kingsley.png")
save(draw_head_icon(36, 36, (120, 80, 50, 255), (80, 60, 40, 255)),
     "Content", "NPCs", "Town", "Kingsley_Head.png")

# Lupin: scarred, shabby brown robes, light brown hair
save(draw_npc_sheet(40, 56, 25, (210, 180, 150, 255), (120, 100, 70, 255), (150, 120, 80, 255)),
     "Content", "NPCs", "Town", "Lupin.png")
save(draw_head_icon(36, 36, (210, 180, 150, 255), (150, 120, 80, 255)),
     "Content", "NPCs", "Town", "Lupin_Head.png")

# MadamHooch: yellow hawk eyes, grey hair, dark robes
save(draw_npc_sheet(40, 56, 25, (215, 185, 155, 255), (50, 50, 60, 255), (160, 160, 170, 255)),
     "Content", "NPCs", "Town", "MadamHooch.png")
save(draw_head_icon(36, 36, (215, 185, 155, 255), (160, 160, 170, 255)),
     "Content", "NPCs", "Town", "MadamHooch_Head.png")

# MadamRosmerta: warm skin, green robes, blonde hair
save(draw_npc_sheet(40, 56, 25, (225, 190, 155, 255), (60, 100, 50, 255), (220, 190, 120, 255)),
     "Content", "NPCs", "Town", "MadamRosmerta.png")
save(draw_head_icon(36, 36, (225, 190, 155, 255), (220, 190, 120, 255)),
     "Content", "NPCs", "Town", "MadamRosmerta_Head.png")

# MrBorgin: pale, dark robes, oily black hair
save(draw_npc_sheet(40, 56, 25, (200, 190, 170, 255), (30, 25, 30, 255), (20, 20, 25, 255)),
     "Content", "NPCs", "Town", "MrBorgin.png")
save(draw_head_icon(36, 36, (200, 190, 170, 255), (20, 20, 25, 255)),
     "Content", "NPCs", "Town", "MrBorgin_Head.png")

# Neville: warm skin, Gryffindor robes, brown hair
save(draw_npc_sheet(40, 56, 25, (220, 180, 150, 255), (140, 40, 40, 255), (100, 70, 40, 255)),
     "Content", "NPCs", "Town", "Neville.png")
save(draw_head_icon(36, 36, (220, 180, 150, 255), (100, 70, 40, 255)),
     "Content", "NPCs", "Town", "Neville_Head.png")

# --- PHASE 25 ACCESSORIES ---
print("\n[Phase 25 Accessories]")
# Padfoot's Amulet: dark pendant with silver detail
save(draw_accessory(24, 24, (40, 40, 50, 255), shape='pendant', detail_color=(100, 100, 120, 255)),
     "Content", "Items", "Accessories", "PadfootAmulet.png")
# Prongs' Charm: silver stag charm
save(draw_accessory(24, 24, (192, 192, 220, 255), shape='circle', detail_color=(240, 240, 255, 255)),
     "Content", "Items", "Accessories", "ProngsCharm.png")

# ============================================================
# BOSS EXPANSION
# ============================================================

# --- TROLL ---
print("\n[Boss Expansion - Troll]")
save(draw_boss_sheet(50, 60, 6, (120, 110, 90, 255), (60, 50, 40, 255), 'wizard'),
     "Content", "NPCs", "Bosses", "Troll", "TrollBoss.png")
save(draw_generic_item(32, 32, (120, 110, 90, 255), shape='round'),
     "Content", "NPCs", "Bosses", "Troll", "TrollBoss_Head_Boss.png")
save(draw_generic_item(32, 32, (120, 110, 90, 255), shape='round'),
     "Content", "Items", "BossLoot", "Troll", "TrollBag.png")
save(draw_accessory(20, 20, (100, 90, 70, 255), shape='circle'),
     "Content", "Items", "BossLoot", "Troll", "TrollHide.png")
save(draw_generic_item(30, 30, (120, 110, 90, 255), shape='rect'),
     "Content", "Items", "BossLoot", "Troll", "TrollTrophy.png")
save(draw_sword(36, 36, (100, 80, 50, 255), (80, 60, 30, 255)),
     "Content", "Items", "Weapons", "TrollClub.png")
save(draw_generic_item(20, 20, (160, 140, 100, 255), shape='round'),
     "Content", "Items", "Consumables", "TrollSummonItem.png")

# --- ARAGOG ---
print("\n[Boss Expansion - Aragog]")
save(draw_boss_sheet(60, 50, 6, (60, 40, 20, 255), (200, 0, 0, 255), 'serpent'),
     "Content", "NPCs", "Bosses", "Aragog", "AragogBoss.png")
save(draw_generic_item(32, 32, (60, 40, 20, 255), shape='round'),
     "Content", "NPCs", "Bosses", "Aragog", "AragogBoss_Head_Boss.png")
save(draw_generic_item(32, 32, (60, 40, 20, 255), shape='round'),
     "Content", "Items", "BossLoot", "Aragog", "AragogBag.png")
save(draw_accessory(20, 20, (40, 30, 20, 255), shape='pendant', detail_color=(200, 0, 0, 255)),
     "Content", "Items", "BossLoot", "Aragog", "AragogsFang.png")
save(draw_generic_item(30, 30, (60, 40, 20, 255), shape='rect'),
     "Content", "Items", "BossLoot", "Aragog", "AragogTrophy.png")
save(draw_generic_item(20, 20, (200, 200, 200, 255), shape='round'),
     "Content", "Items", "Consumables", "AragogSummonItem.png")
save(draw_generic_item(20, 20, (220, 220, 230, 255), shape='round'),
     "Content", "Items", "Consumables", "SpiderSilkWeave.png")

# --- FLUFFY ---
print("\n[Boss Expansion - Fluffy]")
save(draw_boss_sheet(70, 60, 6, (120, 80, 40, 255), (200, 200, 0, 255), 'dragon'),
     "Content", "NPCs", "Bosses", "Fluffy", "FluffyBoss.png")
save(draw_generic_item(32, 32, (120, 80, 40, 255), shape='round'),
     "Content", "NPCs", "Bosses", "Fluffy", "FluffyBoss_Head_Boss.png")
save(draw_generic_item(32, 32, (120, 80, 40, 255), shape='round'),
     "Content", "Items", "BossLoot", "Fluffy", "FluffyBag.png")
save(draw_accessory(24, 24, (80, 60, 30, 255), shape='pendant', detail_color=(200, 170, 50, 255)),
     "Content", "Items", "BossLoot", "Fluffy", "ThreeHeadedCollar.png")
save(draw_generic_item(30, 30, (120, 80, 40, 255), shape='rect'),
     "Content", "Items", "BossLoot", "Fluffy", "FluffyTrophy.png")
save(draw_generic_item(20, 20, (200, 170, 50, 255), shape='round'),
     "Content", "Items", "Consumables", "FluffySummonItem.png")
save(draw_generic_item(20, 20, (200, 180, 150, 255), shape='fang'),
     "Content", "Items", "Consumables", "CerberusFang.png")

# --- BELLATRIX ---
print("\n[Boss Expansion - Bellatrix]")
save(draw_boss_sheet(40, 60, 6, (30, 20, 30, 255), (200, 0, 200, 255), 'wizard'),
     "Content", "NPCs", "Bosses", "Bellatrix", "BellatrixBoss.png")
save(draw_generic_item(32, 32, (30, 20, 30, 255), shape='round'),
     "Content", "NPCs", "Bosses", "Bellatrix", "BellatrixBoss_Head_Boss.png")
save(draw_generic_item(32, 32, (30, 20, 30, 255), shape='round'),
     "Content", "Items", "BossLoot", "Bellatrix", "BellatrixBag.png")
save(draw_accessory(24, 24, (30, 20, 30, 255), shape='circle', detail_color=(200, 0, 200, 255)),
     "Content", "Items", "BossLoot", "Bellatrix", "BellatrixWandHand.png")
save(draw_generic_item(30, 30, (30, 20, 30, 255), shape='rect'),
     "Content", "Items", "BossLoot", "Bellatrix", "BellatrixTrophy.png")
save(draw_generic_item(20, 20, (80, 80, 90, 255), shape='rect'),
     "Content", "Items", "Consumables", "BellatrixSummonItem.png")
save(draw_generic_item(20, 20, (40, 20, 40, 255), shape='rect'),
     "Content", "Items", "Consumables", "DarkArtsTome.png")

# --- FENRIR ---
print("\n[Boss Expansion - Fenrir]")
save(draw_boss_sheet(50, 60, 6, (80, 60, 40, 255), (255, 200, 0, 255), 'dragon'),
     "Content", "NPCs", "Bosses", "Fenrir", "FenrirBoss.png")
save(draw_generic_item(32, 32, (80, 60, 40, 255), shape='round'),
     "Content", "NPCs", "Bosses", "Fenrir", "FenrirBoss_Head_Boss.png")
save(draw_generic_item(32, 32, (80, 60, 40, 255), shape='round'),
     "Content", "Items", "BossLoot", "Fenrir", "FenrirBag.png")
save(draw_accessory(24, 24, (150, 30, 30, 255), shape='circle', detail_color=(200, 50, 50, 255)),
     "Content", "Items", "BossLoot", "Fenrir", "LycansBiteMark.png")
save(draw_generic_item(30, 30, (80, 60, 40, 255), shape='rect'),
     "Content", "Items", "BossLoot", "Fenrir", "FenrirTrophy.png")
save(draw_generic_item(20, 20, (150, 30, 30, 255), shape='fang'),
     "Content", "Items", "Consumables", "FenrirSummonItem.png")
save(draw_generic_item(20, 20, (100, 80, 50, 255), shape='round'),
     "Content", "Items", "Consumables", "WerewolfPelt.png")

# --- DEMENTOR KING ---
print("\n[Boss Expansion - Dementor King]")
save(draw_boss_sheet(60, 70, 6, (15, 15, 25, 255), (100, 150, 255, 255), 'wizard'),
     "Content", "NPCs", "Bosses", "DementorKing", "DementorKingBoss.png")
save(draw_generic_item(32, 32, (15, 15, 25, 255), shape='round'),
     "Content", "NPCs", "Bosses", "DementorKing", "DementorKingBoss_Head_Boss.png")
save(draw_generic_item(32, 32, (15, 15, 25, 255), shape='round'),
     "Content", "Items", "BossLoot", "DementorKing", "DementorKingBag.png")
save(draw_accessory(24, 24, (20, 20, 40, 255), shape='circle', detail_color=(100, 150, 255, 255)),
     "Content", "Items", "BossLoot", "DementorKing", "SoulSiphon.png")
save(draw_generic_item(30, 30, (15, 15, 25, 255), shape='rect'),
     "Content", "Items", "BossLoot", "DementorKing", "DementorKingTrophy.png")
save(draw_generic_item(20, 20, (100, 130, 200, 255), shape='round'),
     "Content", "Items", "Consumables", "DementorKingSummonItem.png")
save(draw_generic_item(20, 20, (20, 20, 35, 255), shape='cloak'),
     "Content", "Items", "Consumables", "DementorsShroud.png")

print("\n[Biome Enemies]")
save(draw_enemy_sheet(34, 34, 4, (200, 150, 255, 255), (255, 200, 255, 255), shape='ghost'),
     "Content", "NPCs", "Enemies", "CorruptedPatronus.png")
save(draw_enemy_sheet(28, 44, 4, (50, 30, 60, 255), (0, 200, 0, 255), shape='ghost'),
     "Content", "NPCs", "Enemies", "CursedSpecter.png")
save(draw_enemy_sheet(50, 30, 4, (60, 80, 120, 255), (200, 220, 255, 255), shape='bat'),
     "Content", "NPCs", "Enemies", "Thunderbird.png")
save(draw_enemy_sheet(20, 34, 4, (100, 80, 50, 255), (255, 215, 0, 255), shape='humanoid'),
     "Content", "NPCs", "Enemies", "GoblinRebel.png")
save(draw_enemy_sheet(36, 20, 4, (10, 10, 15, 255), (50, 50, 70, 255), shape='blob'),
     "Content", "NPCs", "Enemies", "Lethifold.png")

# ============================================================
# PRIORITY 4 BOSSES
# ============================================================

# --- QUIRRELL ---
print("\n[Priority 4 Boss - Quirrell]")
save(draw_boss_sheet(40, 56, 6, (120, 80, 140, 255), (200, 0, 200, 255), 'wizard'),
     "Content", "NPCs", "Bosses", "Quirrell", "QuirrellBoss.png")
save(draw_generic_item(32, 32, (120, 80, 140, 255), shape='round'),
     "Content", "NPCs", "Bosses", "Quirrell", "QuirrellBoss_Head_Boss.png")
save(draw_generic_item(32, 32, (120, 80, 140, 255), shape='round'),
     "Content", "Items", "BossLoot", "Quirrell", "QuirrellBag.png")
save(draw_accessory(20, 20, (120, 80, 140, 255), shape='circle'),
     "Content", "Items", "BossLoot", "Quirrell", "QuirrellsTurban.png")
save(draw_generic_item(30, 30, (120, 80, 140, 255), shape='rect'),
     "Content", "Items", "BossLoot", "Quirrell", "QuirrellTrophy.png")
save(draw_generic_item(20, 20, (150, 100, 170, 255), shape='round'),
     "Content", "Items", "Consumables", "QuirrellSummonItem.png")

# --- UMBRIDGE ---
print("\n[Priority 4 Boss - Umbridge]")
save(draw_boss_sheet(36, 50, 6, (255, 150, 180, 255), (200, 100, 130, 255), 'wizard'),
     "Content", "NPCs", "Bosses", "Umbridge", "UmbridgeBoss.png")
save(draw_generic_item(32, 32, (255, 150, 180, 255), shape='round'),
     "Content", "NPCs", "Bosses", "Umbridge", "UmbridgeBoss_Head_Boss.png")
save(draw_generic_item(32, 32, (255, 150, 180, 255), shape='round'),
     "Content", "Items", "BossLoot", "Umbridge", "UmbridgeBag.png")
save(draw_accessory(20, 20, (200, 170, 50, 255), shape='circle', detail_color=(100, 50, 50, 255)),
     "Content", "Items", "BossLoot", "Umbridge", "MinistryBadge.png")
save(draw_generic_item(30, 30, (255, 150, 180, 255), shape='rect'),
     "Content", "Items", "BossLoot", "Umbridge", "UmbridgeTrophy.png")
save(draw_generic_item(20, 20, (255, 180, 200, 255), shape='scroll'),
     "Content", "Items", "Consumables", "UmbridgeSummonItem.png")
save(draw_wand(30, 30, (200, 50, 80, 255), (255, 150, 180, 255)),
     "Content", "Items", "Weapons", "UmbridgesQuill.png")

# --- BARTY CROUCH ---
print("\n[Priority 4 Boss - Barty Crouch]")
save(draw_boss_sheet(40, 56, 6, (80, 60, 40, 255), (200, 0, 0, 255), 'wizard'),
     "Content", "NPCs", "Bosses", "BartyCrouch", "BartyCrouchBoss.png")
save(draw_generic_item(32, 32, (80, 60, 40, 255), shape='round'),
     "Content", "NPCs", "Bosses", "BartyCrouch", "BartyCrouchBoss_Head_Boss.png")
save(draw_generic_item(32, 32, (80, 60, 40, 255), shape='round'),
     "Content", "Items", "BossLoot", "BartyCrouch", "BartyCrouchBag.png")
save(draw_accessory(24, 24, (100, 130, 80, 255), shape='circle', detail_color=(150, 180, 100, 255)),
     "Content", "Items", "BossLoot", "BartyCrouch", "PolyjuiceFlask.png")
save(draw_generic_item(30, 30, (80, 60, 40, 255), shape='rect'),
     "Content", "Items", "BossLoot", "BartyCrouch", "BartyCrouchTrophy.png")
save(draw_generic_item(20, 20, (100, 130, 80, 255), shape='round'),
     "Content", "Items", "Consumables", "BartyCrouchSummonItem.png")

# ============================================================
# CRAFTING DEPTH
# ============================================================

print("\n[Crafting Depth]")
save(draw_accessory(26, 26, (220, 220, 230, 255), shape='circle', detail_color=(180, 180, 200, 255)),
     "Content", "Items", "Accessories", "SpiderSilkCloak.png")
save(draw_accessory(24, 24, (200, 180, 150, 255), shape='pendant', detail_color=(255, 255, 200, 255)),
     "Content", "Items", "Accessories", "CerberusFangNecklace.png")
save(draw_generic_item(28, 32, (40, 20, 40, 255), shape='rect'),
     "Content", "Items", "Weapons", "DarkArtsGrimoire.png")
save(draw_accessory(26, 26, (100, 80, 50, 255), shape='circle', detail_color=(80, 60, 30, 255)),
     "Content", "Items", "Accessories", "WerewolfCloak.png")
save(draw_accessory(26, 26, (20, 20, 35, 255), shape='circle', detail_color=(100, 100, 180, 255)),
     "Content", "Items", "Accessories", "DementorsEmbrace.png")
save(draw_enemy_sheet(50, 36, 4, (150, 130, 100, 255), (255, 150, 50, 255), shape='spider'),
     "Content", "NPCs", "Enemies", "Erumpent.png")
save(draw_enemy_sheet(30, 24, 4, (180, 190, 210, 255), (200, 220, 240, 255), shape='serpent'),
     "Content", "NPCs", "Enemies", "Occamy.png")

print("\n[Fishing & Weasley]")
save(draw_generic_item(20, 16, (100,180,255,255), shape='round'),
     "Content", "Items", "Consumables", "MagicalKoi.png")
save(draw_generic_item(16, 16, (100,200,180,255), shape='round'),
     "Content", "Items", "Consumables", "MerfolkScale.png")
save(draw_generic_item(16, 12, (80,160,80,255), shape='round'),
     "Content", "Items", "Consumables", "EnchantedTadpole.png")
save(draw_generic_item(24, 24, (120,80,200,255), shape='rect'),
     "Content", "Items", "Consumables", "WizardCrate.png")
save(draw_generic_item(20, 20, (200,150,50,255), shape='rect'),
     "Content", "Items", "Consumables", "SkivingSnackbox.png")
save(draw_generic_item(16, 16, (100,200,80,255), shape='round'),
     "Content", "Items", "Consumables", "PukingPastille.png")
save(draw_generic_item(16, 16, (200,50,50,255), shape='round'),
     "Content", "Items", "Consumables", "NosebleedNougat.png")

print("\n[Fantastic Beasts 2]")
save(draw_enemy_sheet(60, 40, 6, (200,120,50,255), (255,200,50,255), shape='spider'),
     "Content", "NPCs", "Enemies", "Zouwu.png")
save(draw_enemy_sheet(24, 30, 4, (180,180,190,255), (200,200,220,255), shape='humanoid'),
     "Content", "NPCs", "Enemies", "Demiguise.png")
save(draw_generic_item(16, 16, (200,200,220,255), shape='round'),
     "Content", "Items", "Consumables", "DemiguiseHair.png")
save(draw_accessory(24, 24, (50,150,130,255), shape='pendant', detail_color=(100,200,180,255)),
     "Content", "Items", "Accessories", "MerfolkAmulet.png")

print("\n[Cross-Boss Recipes]")
save(draw_accessory(24, 24, (200,100,30,255), shape='pendant', detail_color=(255,200,100,255)),
     "Content", "Items", "Accessories", "DragonToothNecklace.png")
save(draw_accessory(24, 24, (180,200,180,255), shape='circle', detail_color=(0,150,0,255)),
     "Content", "Items", "Accessories", "VenomSilkWraps.png")
save(draw_accessory(28, 28, (255,215,0,255), shape='circle', detail_color=(200,50,50,255)),
     "Content", "Items", "Accessories", "DarkLordsBane.png")
save(draw_enemy_sheet(20, 24, 4, (180,180,200,255), (220,220,240,255), shape='humanoid'),
     "Content", "NPCs", "Enemies", "Mooncalf.png")

print("\n[Fishing & Pets 2]")
save(draw_generic_item(24, 28, (100,60,150,255), shape='staff'),
     "Content", "Items", "Weapons", "WizardFishingRod.png")
save(draw_potion(20, 26, (200,80,20,255)),
     "Content", "Items", "Consumables", "Potions", "Firewhiskey.png")
save(draw_buff_icon((200,80,20,255), (255,150,50,255), icon_type='flame'),
     "Content", "Buffs", "FirewhiskeyBuff.png")
save(draw_pet_sheet(16, 16, 4, 'mammal', (255,150,200,255)),
     "Content", "Pets", "PygmyPuff", "PygmyPuffProjectile.png")
save(draw_generic_item(18, 18, (255,150,200,255), shape='round'),
     "Content", "Pets", "PygmyPuff", "PygmyPuffItem.png")
save(draw_buff_icon((255,150,200,255), (255,200,230,255), icon_type='heart'),
     "Content", "Buffs", "PygmyPuffBuff.png")

# ============================================================
# DEPTH PASS
# ============================================================

print("\n[Depth Pass]")
save(draw_enemy_sheet(14, 12, 4, (50,100,200,255), (100,200,255,255), shape='bat'),
     "Content", "NPCs", "Enemies", "Billywig.png")
save(draw_accessory(18, 18, (200,120,50,255), shape='pendant', detail_color=(255,180,80,255)),
     "Content", "Items", "Accessories", "DirigiblePlumEarring.png")
save(draw_sword(44, 44, (100,80,50,255), (80,60,30,255), gem_color=(150,50,200,255)),
     "Content", "Items", "Weapons", "EnchantedTrollClub.png")

# ============================================================
# MOUNTS & ACCESSORIES
# ============================================================

print("\n[Mounts & Accessories]")
save(draw_accessory(22, 22, (139,90,43,255), shape='square', detail_color=(200,170,100,255)),
     "Content", "Items", "Accessories", "WandHolster.png")
save(draw_mount_sheet(80, 50, 4, 'hippogriff', (40,40,50,255)),
     "Content", "Mounts", "ThestralMount.png")
save(draw_buff_icon((40,40,50,255), (150,150,180,255), icon_type='wing'),
     "Content", "Buffs", "ThestralMountBuff.png")
save(draw_generic_item(28, 22, (40,40,50,255), shape='rect'),
     "Content", "Items", "Weapons", "ThestralReins.png")

# ============================================================
# ENDGAME CONTENT
# ============================================================

print("\n[Endgame Content]")
save(draw_enemy_sheet(30, 16, 4, (120,110,100,255), (255,100,30,255), shape='serpent'),
     "Content", "NPCs", "Enemies", "Ashwinder.png")
save(draw_generic_item(16, 16, (255,120,40,255), shape='round'),
     "Content", "Items", "Consumables", "AshwinderEgg.png")
save(draw_armor_item(18, 18, (80,40,120,255), (200,170,50,255), 'head'),
     "Content", "Items", "Armor", "Wizengamot", "WizengamotHood.png")
save(draw_armor_equip(40, 36, (80,40,120,255), (200,170,50,255), 'head'),
     "Content", "Items", "Armor", "Wizengamot", "WizengamotHood_Head.png")
save(draw_armor_item(18, 18, (80,40,120,255), (200,170,50,255), 'body'),
     "Content", "Items", "Armor", "Wizengamot", "WizengamotRobes.png")
save(draw_armor_equip(40, 54, (80,40,120,255), (200,170,50,255), 'body'),
     "Content", "Items", "Armor", "Wizengamot", "WizengamotRobes_Body.png")
save(draw_armor_item(18, 18, (80,40,120,255), (200,170,50,255), 'legs'),
     "Content", "Items", "Armor", "Wizengamot", "WizengamotBoots.png")
save(draw_armor_equip(40, 44, (80,40,120,255), (200,170,50,255), 'legs'),
     "Content", "Items", "Armor", "Wizengamot", "WizengamotBoots_Legs.png")

print("\n[Fantastic Beasts 3]")
save(draw_enemy_sheet(54, 36, 6, (200,170,80,255), (0,200,0,255), shape='spider'),
     "Content", "NPCs", "Enemies", "Nundu.png")
save(draw_enemy_sheet(40, 20, 4, (220,140,40,255), (200,50,0,255), shape='serpent'),
     "Content", "NPCs", "Enemies", "Runespoor.png")

# ============================================================
# FISHING DEPTH
# ============================================================

print("\n[Fishing Depth]")
save(draw_generic_item(16, 20, (255,200,50,255), shape='round'),
     "Content", "Items", "Consumables", "PhoenixTear.png")
save(draw_generic_item(14, 14, (200,200,180,255), shape='fang'),
     "Content", "Items", "Consumables", "GrindylowTooth.png")
save(draw_buff_icon((50,120,200,255), (100,200,255,255), icon_type='shield'),
     "Content", "Buffs", "AquaFortisBuff.png")
save(draw_potion(20, 26, (50,120,200,255)),
     "Content", "Items", "Consumables", "Potions", "AquaFortis.png")

# ============================================================
# FANTASTIC BEASTS 4
# ============================================================

print("\n[Fantastic Beasts 4]")
save(draw_enemy_sheet(22, 14, 4, (160,120,70,255), (40,30,20,255), shape='humanoid'),
     "Content", "NPCs", "Enemies", "Jarvey.png")
save(draw_enemy_sheet(32, 22, 4, (180,60,30,255), (255,150,50,255), shape='spider'),
     "Content", "NPCs", "Enemies", "FireCrab.png")

# ============================================================
# FANTASTIC BEASTS 5
# ============================================================

print("\n[Fantastic Beasts 5]")
save(draw_enemy_sheet(16, 14, 2, (240,210,140,255), (60,40,20,255), shape='blob'),
     "Content", "NPCs", "Enemies", "Puffskein.png")
save(draw_enemy_sheet(24, 18, 2, (180,80,200,255), (200,100,220,255), shape='blob'),
     "Content", "NPCs", "Enemies", "Streeler.png")

# ============================================================
# WEAPON UPGRADES
# ============================================================

print("\n[Weapon Upgrades]")
save(draw_wand(40, 40, (200,50,20,255), (255,200,50,255)),
     "Content", "Items", "Weapons", "Wands", "InfernalPhoenixWand.png")
save(draw_sword(52, 52, (192,192,192,255), (0,120,0,255), gem_color=(200,50,50,255)),
     "Content", "Items", "Weapons", "VenomousSwordOfGryffindor.png")
save(draw_wand(42, 42, (20,20,30,255), (100,50,150,255)),
     "Content", "Items", "Weapons", "Wands", "ShadowElderWand.png")

# ============================================================
# QoL ITEMS
# ============================================================

print("\n[QoL Items]")
save(draw_accessory(24, 24, (200,170,50,255), shape='circle', detail_color=(255,50,50,255)),
     "Content", "Items", "Accessories", "BossCompass.png")

# ============================================================
# FINAL ITEMS
# ============================================================

print("\n[Final Items]")
save(draw_accessory(20, 20, (200,170,50,255), shape='circle', detail_color=(255,255,200,255)),
     "Content", "Items", "Accessories", "WizardPocketWatch.png")
save(draw_potion(20, 26, (80,160,60,255)),
     "Content", "Items", "Consumables", "Potions", "MandrakeRestorative.png")

# ============================================================
# WEAPON UPGRADES 2
# ============================================================

print("\n[Weapon Upgrades 2]")
save(draw_generic_item(22, 18, (200,50,50,255), shape='letter'),
     "Content", "Items", "Weapons", "ScreechingHowler.png")

# ============================================================
# QUIDDITCH GEAR
# ============================================================

print("\n[Quidditch Gear]")
save(draw_accessory(22, 22, (139,90,43,255), shape='square', detail_color=(200,170,100,255)),
     "Content", "Items", "Accessories", "KeeperGloves.png")
save(draw_buff_icon((255,215,0,255), (255,255,200,255), icon_type='wing'),
     "Content", "Buffs", "SeekersReflexesBuff.png")
save(draw_potion(20, 26, (255,200,50,255)),
     "Content", "Items", "Consumables", "Potions", "SeekersReflexes.png")

# ============================================================
# LUNA & DRAGONS
# ============================================================

print("\n[Luna & Dragons]")
save(draw_generic_item(22, 14, (255,100,200,255), shape='round'),
     "Content", "Items", "Accessories", "Spectrespecs.png")
save(draw_generic_item(22, 14, (255,100,200,255), shape='round'),
     "Content", "Items", "Accessories", "Spectrespecs_Face.png")
save(draw_enemy_sheet(36, 24, 4, (180,120,60,255), (200,50,0,255), shape='serpent'),
     "Content", "NPCs", "Enemies", "PeruvianVipertooth.png")

# ============================================================
# PUSH TO 400
# ============================================================

print("\n[Push to 400]")
save(draw_accessory(22, 26, (150,150,170,255), shape='square', detail_color=(200,200,220,255)),
     "Content", "Items", "Accessories", "FoeGlass.png")
save(draw_generic_item(24, 28, (100,150,255,255), shape='cup'),
     "Content", "Items", "Placeable", "GobletOfFire.png")
save(draw_pet_sheet(20, 20, 4, 'bird', (200,100,30,255)),
     "Content", "Pets", "BabyDragon", "BabyDragonProjectile.png")
save(draw_generic_item(20, 20, (200,100,30,255), shape='egg'),
     "Content", "Pets", "BabyDragon", "BabyDragonItem.png")
save(draw_buff_icon((200,100,30,255), (255,200,50,255), icon_type='flame'),
     "Content", "Buffs", "BabyDragonBuff.png")

# ============================================================
# PUSH TO 400 BATCH 2
# ============================================================

print("\n[Push to 400 Batch 2]")
save(draw_enemy_sheet(20, 18, 4, (180,160,140,255), (200,180,160,255), shape='humanoid'),
     "Content", "NPCs", "Enemies", "Diricawl.png")
save(draw_enemy_sheet(18, 12, 4, (120,100,70,255), (80,60,40,255), shape='blob'),
     "Content", "NPCs", "Enemies", "Knarl.png")
save(draw_accessory(28, 28, (200,150,255,255), shape='circle', detail_color=(255,200,255,255)),
     "Content", "Items", "Accessories", "FairyWings.png")
save(draw_generic_item(20, 24, (255,215,0,255), shape='round'),
     "Content", "Items", "Accessories", "QuillOfAcceptance.png")
save(draw_sword(44, 44, (192,192,210,255), (139,90,43,255), gem_color=(255,215,0,255)),
     "Content", "Items", "Weapons", "GoblinSilverSword.png")

# ============================================================
# 400 MILESTONE
# ============================================================

print("\n[400 Milestone]")
save(draw_enemy_sheet(16, 14, 1, (220,150,160,255), (180,100,120,255), shape='blob'),
     "Content", "NPCs", "Enemies", "Horklump.png")
save(draw_enemy_sheet(14, 12, 4, (100,130,200,255), (150,180,240,255), shape='bat'),
     "Content", "NPCs", "Enemies", "Jobberknoll.png")
save(draw_accessory(20, 20, (100,150,200,255), shape='circle', detail_color=(255,50,50,255)),
     "Content", "Items", "Accessories", "MoodysEye.png")
save(draw_accessory(18, 18, (180,80,30,255), shape='circle', detail_color=(255,215,0,255)),
     "Content", "Items", "Accessories", "DragonScaleRing.png")
save(draw_generic_item(16, 16, (200,150,80,255), shape='round'),
     "Content", "Items", "Consumables", "PhoenixAsh.png")
save(draw_potion(20, 26, (255,150,50,255)),
     "Content", "Items", "Consumables", "Potions", "ResurrectionPotion.png")
save(draw_buff_icon((255,150,50,255), (255,220,100,255), icon_type='heart'),
     "Content", "Buffs", "ResurrectionBuff.png")
save(draw_potion(20, 26, (100,150,255,255)),
     "Content", "Items", "Consumables", "Potions", "ShieldCharmPotion.png")
save(draw_buff_icon((100,150,255,255), (200,220,255,255), icon_type='shield'),
     "Content", "Buffs", "ShieldCharmBuff.png")
save(draw_accessory(20, 22, (200,200,220,255), shape='square', detail_color=(255,255,255,255)),
     "Content", "Items", "Accessories", "EnchantedMirror.png")
save(draw_potion(20, 26, (200,50,20,255)),
     "Content", "Items", "Consumables", "Potions", "DraconisElixir.png")

# ============================================================
# RECIPE DEPTH FINAL
# ============================================================

print("\n[Recipe Depth Final]")
save(draw_generic_item(14, 18, (100,130,200,255), shape='round'),
     "Content", "Items", "Consumables", "JobberknollFeather.png")
save(draw_accessory(24, 24, (150,100,50,255), shape='pendant', detail_color=(200,50,30,255)),
     "Content", "Items", "Accessories", "BeastHuntersCharm.png")

# ============================================================
# COMBO REWARDS
# ============================================================

print("\n[Combo Rewards]")
save(draw_generic_item(26, 26, (150,80,200,255), shape='rect'),
     "Content", "Items", "Consumables", "WizardCrateHardmode.png")

# ============================================================
# FINAL BEASTS
# ============================================================

print("\n[Final Beasts]")
save(draw_enemy_sheet(30, 16, 4, (100,80,40,255), (60,50,30,255), shape='serpent'),
     "Content", "NPCs", "Enemies", "Dugbog.png")
save(draw_enemy_sheet(16, 20, 4, (200,80,30,255), (255,150,50,255), shape='humanoid'),
     "Content", "NPCs", "Enemies", "MagicalImp.png")

# ============================================================
# 900 PUSH
# ============================================================

print("\n[900 Push]")
save(draw_generic_item(14, 14, (255,100,30,255), shape='round'),
     "Content", "Items", "Consumables", "ImpFlame.png")
save(draw_generic_item(16, 16, (100,80,40,255), shape='round'),
     "Content", "Items", "Consumables", "DugbogHide.png")
save(draw_accessory(26, 26, (80,100,60,255), shape='circle', detail_color=(120,140,80,255)),
     "Content", "Items", "Accessories", "CamouflageCloak.png")
save(draw_buff_icon((120,80,40,255), (200,150,50,255), icon_type='star'),
     "Content", "Buffs", "FenrirsWolfsbaneBuff.png")
save(draw_potion(20, 26, (120,80,40,255)),
     "Content", "Items", "Consumables", "Potions", "FenrirsWolfsbane.png")

# ============================================================
# 900 MILESTONE
# ============================================================

print("\n[900 Milestone]")
save(draw_buff_icon((60,80,50,255), (120,160,100,255), icon_type='eye'),
     "Content", "Buffs", "StealthDraughtBuff.png")
save(draw_potion(20, 26, (60,100,60,255)),
     "Content", "Items", "Consumables", "Potions", "StealthDraught.png")

# ============================================================
# FINAL ENCYCLOPEDIA
# ============================================================

print("\n[Final Encyclopedia]")
save(draw_generic_item(28, 32, (100,50,30,255), shape='rect'),
     "Content", "Items", "Consumables", "WizardsAlmanac.png")

# ============================================================
# FINAL ITEMS 2
# ============================================================

print("\n[Final Items 2]")
save(draw_potion(20, 26, (150,150,220,255)),
     "Content", "Items", "Consumables", "Potions", "MemoryPotion.png")

# ============================================================
# REDESIGN PHASE — Phase 1-3 new items
# ============================================================

print("\n[Redesign Phase]")
save(draw_projectile(16, 16, (255,200,50,255), (255,255,150,255), style='orb'),
     "Content", "Projectiles", "Spells", "EpiskeyProjectile.png")

save(draw_projectile(12, 12, (255,215,0,255), (255,255,200,255), style='orb'),
     "Content", "Projectiles", "Spells", "AlohomoraProjectile.png")

save(draw_generic_item(18, 18, (255,215,0,255), shape='round'),
     "Content", "Items", "Weapons", "Wands", "AlohomoraWand.png")

# ============================================================
# PHASE 30 — LANDMARK TILES & PLACEABLE ITEMS
# 12 decorative landmark tiles + matching placement items.
# Tile dims match ModTile Style; placement item icons are 20-32px.
# All remain procedural placeholders; see ASSET_BACKLOG.md.
# ============================================================

print("\n[Phase 30 Landmark Tiles]")

# Hogwarts / Grounds
save(draw_tile_sprite(2, 3, (200, 180, 100, 255)),
     "Content", "Tiles", "Landmarks", "HousePointHourglass.png")
save(draw_tile_sprite(1, 4, (180, 160, 80, 255)),
     "Content", "Tiles", "Landmarks", "QuidditchGoalpost.png")
save(draw_tile_sprite(2, 2, (140, 140, 160, 255)),
     "Content", "Tiles", "Landmarks", "CastleWardStone.png")
save(draw_tile_sprite(3, 2, (90, 70, 40, 255)),
     "Content", "Tiles", "Landmarks", "WhompingWillowStump.png")
save(draw_tile_sprite(2, 2, (80, 70, 60, 255)),
     "Content", "Tiles", "Landmarks", "ShriekingShackSign.png")
save(draw_tile_sprite(2, 3, (50, 50, 55, 255)),
     "Content", "Tiles", "Landmarks", "GrimmauldDoorway.png")

# London / Ministry / Dark Districts
save(draw_tile_sprite(2, 2, (90, 60, 30, 255)),
     "Content", "Tiles", "Landmarks", "LeakyCauldronSign.png")
save(draw_tile_sprite(3, 3, (230, 210, 140, 255)),
     "Content", "Tiles", "Landmarks", "GringottsFacade.png")
save(draw_tile_sprite(2, 2, (60, 50, 45, 255)),
     "Content", "Tiles", "Landmarks", "BorginStorefront.png")
save(draw_tile_sprite(2, 2, (180, 170, 160, 255)),
     "Content", "Tiles", "Landmarks", "StMungosMannequin.png")
save(draw_tile_sprite(2, 3, (150, 180, 220, 255)),
     "Content", "Tiles", "Landmarks", "ProphecyShelf.png")
save(draw_tile_sprite(3, 4, (100, 100, 120, 255)),
     "Content", "Tiles", "Landmarks", "VeilArch.png")

print("\n[Phase 30 Landmark Placement Items]")
save(draw_generic_item(20, 30, (200, 180, 100, 255), shape='rect', detail_color=(255, 215, 0, 255)),
     "Content", "Items", "Placeable", "HousePointHourglassItem.png")
save(draw_generic_item(18, 32, (180, 160, 80, 255), shape='rect', detail_color=(255, 215, 100, 255)),
     "Content", "Items", "Placeable", "QuidditchGoalpostItem.png")
save(draw_generic_item(24, 24, (140, 140, 160, 255), shape='gem', detail_color=(200, 200, 230, 255)),
     "Content", "Items", "Placeable", "CastleWardStoneItem.png")
save(draw_generic_item(30, 22, (90, 70, 40, 255), shape='rect', detail_color=(130, 100, 60, 255)),
     "Content", "Items", "Placeable", "WhompingWillowStumpItem.png")
save(draw_generic_item(24, 24, (80, 70, 60, 255), shape='rect', detail_color=(150, 140, 120, 255)),
     "Content", "Items", "Placeable", "ShriekingShackSignItem.png")
save(draw_generic_item(22, 30, (50, 50, 55, 255), shape='rect', detail_color=(120, 100, 80, 255)),
     "Content", "Items", "Placeable", "GrimmauldDoorwayItem.png")
save(draw_generic_item(24, 24, (90, 60, 30, 255), shape='rect', detail_color=(180, 140, 70, 255)),
     "Content", "Items", "Placeable", "LeakyCauldronSignItem.png")
save(draw_generic_item(30, 30, (230, 210, 140, 255), shape='rect', detail_color=(255, 215, 0, 255)),
     "Content", "Items", "Placeable", "GringottsFacadeItem.png")
save(draw_generic_item(24, 24, (60, 50, 45, 255), shape='rect', detail_color=(120, 80, 60, 255)),
     "Content", "Items", "Placeable", "BorginStorefrontItem.png")
save(draw_generic_item(24, 24, (180, 170, 160, 255), shape='rect', detail_color=(230, 220, 210, 255)),
     "Content", "Items", "Placeable", "StMungosMannequinItem.png")
save(draw_generic_item(24, 30, (150, 180, 220, 255), shape='rect', detail_color=(220, 230, 255, 255)),
     "Content", "Items", "Placeable", "ProphecyShelfItem.png")
save(draw_generic_item(28, 36, (100, 100, 120, 255), shape='rect', detail_color=(60, 60, 80, 255)),
     "Content", "Items", "Placeable", "VeilArchItem.png")

print("\n=== All sprites generated! ===")
print(f"Output directory: {BASE}")
