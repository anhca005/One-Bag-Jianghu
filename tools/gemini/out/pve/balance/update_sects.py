for fname in ["chapter_1_data.py", "chapter_2_data.py", "chapter_3_data.py"]:
    path = f"tools/gemini/out/pve/balance/{fname}"
    with open(path, "r", encoding="utf-8") as f:
        text = f.read()
    text = text.replace('"phong_loi"', '"thai_thanh"').replace('"van_hoa"', '"bich_thuy"')
    with open(path, "w", encoding="utf-8") as f:
        f.write(text)
    print(f"Updated {fname}")
