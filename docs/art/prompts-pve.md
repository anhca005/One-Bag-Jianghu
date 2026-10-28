# Prompt hình Hành Tẩu Giang Hồ (ChatGPT)

Kế hoạch PvE: [design/pve.md](../design/pve.md).

## Quy ước
- Lưu vào `One-Bag-Jianghu/Assets/_Game/Textures2D/Pve/` (tạo thư mục nếu chưa có), **đúng tên cột File**.
- Dùng **Tin mở đầu** của [prompts-ui.md](prompts-ui.md) và đính kèm `docs/art/reference/01-style-board-thuy-mac.png`.
- Icon, nút, khung: nền trong suốt. Nếu ChatGPT trả về nền trắng hoặc caro giả, Claude sẽ tách nền.

## Đợt 1: hình dùng chung (vẽ được ngay)

| # | File | Kích thước | Dùng ở đâu | Prompt |
|---|---|---|---|---|
| 1 | `node_stage.png` | 1024x1024 | Nút ải thường trên bản đồ | `A small round stone stepping-marker seen from above: a flat grey river stone with a thin ink brush ring around it and a tiny vermilion dot in the center, centered, transparent background, no text.` |
| 2 | `node_elite.png` | 1024x1024 | Nút ải tinh anh | `A round bronze shield-like marker seen from the front: dark bronze disc with an ochre-gold rim, two crossed ink-brush sword strokes engraved on it, centered, transparent background, no text.` |
| 3 | `node_boss.png` | 1024x1024 | Nút ải boss | `A large square vermilion seal stamp marker with a carved fierce beast-mask pattern (abstract, no readable characters), rough ink edges, small ochre-gold corner caps, centered, transparent background, no text.` |
| 4 | `icon_star.png` | 1024x1024 | Sao chấm điểm ải | `A single five-petal plum blossom painted in ochre gold with a thin ink outline, flat, centered, transparent background, no text.` |
| 5 | `icon_star_empty.png` | 1024x1024 | Sao chưa đạt | `The same five-petal plum blossom shape as only a light grey ink outline, empty inside, flat, centered, transparent background, no text.` |
| 6 | `icon_stamina.png` | 1024x1024 | Thể Lực | `A pair of woven straw sandals tied with a red cord, a traveller's symbol, flat ink and ochre washes, centered, transparent background, no text.` |
| 7 | `icon_bac.png` | 1024x1024 | Tiền Bạc | `A single shiny silver sycee ingot (boat-shaped), cool silver-grey ink wash with a crisp outline, centered, transparent background, no text.` |
| 8 | `icon_nguyen_bao.png` | 1024x1024 | Tiền Nguyên Bảo | `A single gold yuanbao ingot (boat-shaped, rounded top), ochre-gold wash with a crisp ink outline, a tiny vermilion knot ribbon, centered, transparent background, no text.` |
| 9 | `icon_danh_vong.png` | 1024x1024 | Danh Vọng | `A jade token pendant (rectangular with a rounded top) in pale green jade with a red tassel, carved abstract cloud pattern, no readable characters, centered, transparent background, no text.` |
| 10 | `frame_dialogue.png` | 1536x512 | Hộp hội thoại | `A wide horizontal unrolled paper scroll for dialogue text: pale rice-paper surface, dark wooden rollers at both ends with ochre-gold caps, thin ink border, the whole inside empty, flat 2D, transparent background outside the scroll, no text.` (Unity: 9-slice) |
| 11 | `frame_stage_card.png` | 1024x1536 | Bảng thông tin ải | `A tall vertical wooden notice board with a pale rice-paper sheet pinned on it, small ochre-gold nail heads at the corners, a vermilion ribbon hanging from the top, the paper sheet completely empty, flat 2D, transparent background, no text.` (Unity: 9-slice) |

**Hình 1–9:** icon nhỏ, game hiển thị khoảng 48–96 px, nên vật thể phải đơn giản, rõ khối và đặt chính giữa.

## Đợt 2: theo từng Hồi (nội dung Gemini, `docs/reviews/2026-10-01-gemini-pve-content.md`)

Lưu cùng thư mục `Textures2D/Pve/`. Không vẽ lại `hero_portrait`: đã có.
- **Bản đồ và nền trận:** khổ 1536x1024 (tối đa ChatGPT ra được), giữ phần quan trọng ở giữa.
- **Chân dung:** nửa thân, khổ 1024x1536, nền trong suốt. Kẻ địch quay mặt **sang trái**, đồng minh quay **sang phải**.
- **Nút ải theo Hồi:** dùng thay cho `node_stage` và `node_boss` của đợt 1. `node_elite` vẫn dùng chung.

### Bản đồ chọn ải

| # | File | Prompt |
|---|---|---|
| 12 | `map_hoi_1.png` | `Stage-select map, a wide horizontal ink-wash landscape scroll of an autumn river town: a calm river on the left with faint rowing boats in morning mist, red maple trees along the bank, a winding ochre dirt path that starts at the bottom left, passes a thatched tea house and two small wooden bridges, and climbs a hill on the right to a bandit stockade with wisps of smoke. The path must be clearly visible from start to end with room for about ten stops. Keep the top 8% and bottom 10% calm. Landscape 1536x1024, no people, no text.` |
| 13 | `map_hoi_2.png` | `Stage-select map, a wide ink-wash landscape scroll of a primeval jungle swamp: giant ancient trees wrapped in vines, roots rising from dark water like serpents, pale purple poison mist over the marsh, moss-covered cliffs. A clear winding trail starts at the bottom left, crosses a narrow stream gorge and stepping stones, and ends at a ruined stone altar of carved serpent pillars in the upper right. Green-grey misty tones. Keep the top 8% and bottom 10% calm. Landscape 1536x1024, no people, no text.` |
| 14 | `map_hoi_3.png` | `Stage-select map, a wide ink-wash landscape scroll of towering snowy peaks like raised blades piercing the clouds, lone pine trees under snow, swirling wind and snow. An icy mountain trail starts at the bottom left, climbs switchbacks along a deep chasm and a hanging rope bridge, and ends at a frozen stone terrace on the highest peak under a pale crescent moon in the upper right. Cold blue-white tones. Keep the top 8% and bottom 10% calm. Landscape 1536x1024, no people, no text.` |

### Nền trận

| # | File | Prompt |
|---|---|---|
| 15 | `bg_battle_hoi_1.png` | `Battle background with the same composition as a duel stage: a flat wooden riverside pier platform seen from the front, autumn river with faint ripples behind, a few fishing boats moored under red maple trees, a mossy wooden footbridge far left, the large middle area empty and low-contrast so game boards sit on top. Landscape 1536x1024, no people, no text.` |
| 16 | `bg_battle_hoi_2.png` | `Battle background with the same composition as a duel stage: a flat clearing of cracked ancient stone slabs in a swamp jungle, hollow giant trees with thorny vines on both sides, moss-covered carved tribal stone pillars, pale green poison mist drifting low, the large middle area empty and low-contrast so game boards sit on top. Landscape 1536x1024, no people, no text.` |
| 17 | `bg_battle_hoi_3.png` | `Battle background with the same composition as a duel stage: a flat icy stone terrace on a mountain peak during a snowstorm, clear icicle columns at both edges reflecting a pale blue moon, swirling snow, an abyss with rolling clouds far below in the distance, the large middle area empty and low-contrast so game boards sit on top. Landscape 1536x1024, no people, no text.` |

### Nút ải theo Hồi (1024x1024, nền trong suốt)

| # | File | Prompt |
|---|---|---|
| 18 | `node_hoi_1.png` | `Map marker icon: a round ink stone carved in the shape of a red maple leaf, soft ink-wash shading, thin pale gold rim, centered, transparent background, no text.` |
| 19 | `node_boss_hoi_1.png` | `Boss map marker icon: a roaring tiger head painted with bold splashed ink strokes, fierce red eyes, inside a vermilion round frame, centered, transparent background, no text.` |
| 20 | `node_hoi_2.png` | `Map marker icon: a diamond-shaped poison leaf carved from dark green jade with one glowing green dewdrop, centered, transparent background, no text.` |
| 21 | `node_boss_hoi_2.png` | `Boss map marker icon: a coiled serpent wrapped around a crescent moon, dark purple mist, abstract ancient rune marks with no readable characters, centered, transparent background, no text.` |
| 22 | `node_hoi_3.png` | `Map marker icon: a six-pointed snowflake drawn with sharp brush strokes, a small ice-blue gem in the center, centered, transparent background, no text.` |
| 23 | `node_boss_hoi_3.png` | `Boss map marker icon: a stern demon mask made of ice crossed with a spear, cold blue frost, pale glowing eyes, centered, transparent background, no text.` |

### Chân dung đồng minh (quay sang phải)

| # | File | Prompt |
|---|---|---|
| 24 | `npc_lao_tieu_phu.png` | `Half-body portrait of an old woodcutter hermit in his sixties, frost-white hair and beard, weathered kind face with sharp bright eyes, a bundle of dry firewood and a worn axe on his back, leaning on a bamboo staff, plain brown hemp clothes, three-quarter view turned slightly to the right, from the waist up, transparent background, no text.` |
| 25 | `npc_tieu_linh_nhi.png` | `Half-body portrait of a lively girl of about sixteen, a young herbalist healer, pale green robe with white silk, a fragrant herb pouch and a small jade gourd at her waist, bright clear eyes and a kind smile, three-quarter view turned slightly to the right, from the waist up, transparent background, no text.` |
| 26 | `npc_phong_loi.png` | `Half-body portrait of a proud lone blade swordsman in his late twenties, loose black hair blowing in the wind, a weathered jet-black cloak, one hand resting on the hilt of a long saber at his side, cold but righteous eyes, three-quarter view turned slightly to the right, from the waist up, transparent background, no text.` |

### Chân dung boss (quay sang trái)

| # | File | Prompt |
|---|---|---|
| 27 | `boss_bui_chen.png` | `Half-body portrait of a massive bandit chief in his fifties, built like a temple guardian, fierce full beard, bare muscular chest under a dark red tiger-hide armor vest, gripping a huge heavy broad saber with faint flame-like ink strokes, furious look, three-quarter view facing left, from the waist up, transparent background, no text.` |
| 28 | `boss_hac_doc_nuong_tu.png` | `Half-body portrait of an alluring dangerous poison mistress of about thirty, sharp bewitching eyes, a tight deep-purple night robe patterned with faint snake scales, long black hair held by a jade snake hairpin, holding a long thorned whip that curls like a living snake, three-quarter view facing left, from the waist up, transparent background, no text.` |
| 29 | `boss_da_sat_thien.png` | `Half-body portrait of a handsome but evil cult leader, half of his face covered by a carved silver demon mask, a noble dark blue-black robe with a white snow-fur collar, holding an ice spear that leaks cold frost mist, overwhelming calm menace, three-quarter view facing left, from the waist up, transparent background, no text.` |

### Chân dung quái thú (quay sang trái, thấy đầu và nửa thân trước)

| # | File | Prompt |
|---|---|---|
| 30 | `monster_soi_xam.png` | `Portrait of a huge wild grey wolf, ash-grey fur, long sharp fangs, blood-red eyes, snarling, head and front body facing left, portrait 1024x1536, transparent background, no text.` |
| 31 | `monster_heo_rung.png` | `Portrait of a giant wild boar with black iron-like scaled hide, curved tusks plated with spikes, burning furious eyes, head and front body facing left, portrait 1024x1536, transparent background, no text.` |
| 32 | `monster_tri_chu.png` | `Portrait of a giant venomous spider with eight blade-like legs, a dark purple demon-face pattern on its back, venom fangs, seen from the front-left, portrait 1024x1536, transparent background, no text.` |
| 33 | `monster_gau_den.png` | `Portrait of a huge black bear with a glowing green poison scar across its chest, shovel-sized claws, standing on its hind legs, facing left, portrait 1024x1536, transparent background, no text.` |
| 34 | `monster_xich_xa.png` | `Portrait of a thousand-year serpent with ember-red scales, a crown-like crest on its head, blazing golden eyes, coiled and rearing up, head facing left, portrait 1024x1536, transparent background, no text.` |
| 35 | `monster_ngan_lang.png` | `Portrait of a majestic snow wolf pack leader with pure white fur and icy blue eyes, head raised, front body facing left, portrait 1024x1536, transparent background, no text.` |
| 36 | `monster_bach_ho.png` | `Portrait of a giant white tiger with black stripes, fangs like short daggers, cold wind swirling around its claws, head and front body facing left, portrait 1024x1536, transparent background, no text.` |
| 37 | `monster_bang_quy.png` | `Portrait of a giant ancient turtle whose shell is covered in tall eternal ice crystals like a fortress, slow heavy gaze, head facing left, portrait 1024x1536, transparent background, no text.` |

### Icon vật phẩm quái (theo kích thước ô)

| # | File | Ô | Kích thước | Prompt |
|---|---|---|---|---|
| 38 | `soi_nanh_canh.png` | 1x2 | 1024x1536 | `Icon: a long sharp wolf fang mounted on an old bronze handle as a throwing blade, faint red streaks, vertical, centered, transparent background, no text.` |
| 39 | `heo_rung_bap_giap.png` | 2x2 | 1024x1024 | `Icon: a thick armor vest made of wild boar hide with black iron rivets, sturdy, centered, transparent background, no text.` |
| 40 | `man_xa_doc_tui.png` | 1x1 | 1024x1024 | `Icon: a small venom pouch sewn from shimmering dark purple snakeskin, tied with a gold silk cord, centered, transparent background, no text.` |
| 41 | `u_minh_quy_trao.png` | 2x2 | 1024x1024 | `Icon: a pair of black steel claw gauntlets curved like demon claws, faint blue-green smoke, centered, transparent background, no text.` |
| 42 | `kich_doc_chu_nga.png` | 1x2 | 1024x1536 | `Icon: a curved spider fang dagger like violet jade, a drop of glowing venom at the tip, vertical, centered, transparent background, no text.` |
| 43 | `bang_tam_linh_chau.png` | 1x1 | 1024x1024 | `Icon: a pearl spun from frozen silkworm silk, soft pale blue sheen, small silver cap, centered, transparent background, no text.` |
| 44 | `tuyet_lang_bach_bi.png` | 2x1 | 1536x1024 | `Icon: a folded pure white wolf-fur cloak with a soft snowy fur trim, horizontal, centered, transparent background, no text.` |
| 45 | `hac_sat_hung_dam.png` | 1x1 | 1024x1024 | `Icon: a dried black bear gall bladder with amber highlights tied with red string, a medicine ingredient, centered, transparent background, no text.` |
| 46 | `bach_ho_cuong_mong.png` | 2x1 | 1536x1024 | `Icon: a set of white tiger claws mounted on a leather knuckle strap, sharp and gleaming, faint engraved patterns with no readable characters, horizontal, centered, transparent background, no text.` |
| 47 | `hoa_diem_xa_cot.png` | 1x2 | 1024x1536 | `Icon: a segment of an ancient serpent spine glowing ember-red like molten cinnabar, wrapped with an old cloth talisman strip with no readable characters, vertical, centered, transparent background, no text.` |
| 48 | `bang_phach_co_cot.png` | 2x2 | 1024x1024 | `Icon: a fossilized ancient bone slab encased in clear glass-like ice, hard and translucent, centered, transparent background, no text.` |
| 49 | `thien_nien_quy_giap.png` | 2x2 | 1024x1024 | `Icon: a thousand-year turtle shell with natural eight-trigram-like plate patterns, faint golden protective sheen, centered, transparent background, no text.` |

**Lưu ý:**
- `npc_phong_loi` thay cho `npc_doc_co_hanh`. "Độc Cô" là họ gắn với truyện Kim Dung nên nhân vật này sẽ được đổi tên.
- `hoa_diem_xa_cot` là tên đã sửa của khóa gõ sai `hoa_ngiem_xa_cot`.
- Cả hai sẽ được sửa trong dữ liệu ở vòng sau.

## Đợt 3: Hồi 4–5 (ải 31–50, nội dung Codex `docs/reviews/2026-10-04-codex-pve-hoi-4-5.md`)

Quy ước như đợt 2: bản đồ 1536x1024, nền trận 1536x1024, nút 1024x1024 nền trong suốt; đồng minh quay sang phải, boss quay sang trái.

| # | File | Prompt |
|---|---|---|
| 50 | `map_hoi_4.png` | `Stage-select map, a wide ink-wash landscape scroll of the central plains in spring rain: green rice fields, willow-lined canals, arched stone bridges, a small post station with a tiled roof, soft slanting rain and mist. A muddy road starts at the bottom left, winds past the post station and along a canal, and ends at a walled river water-gate of a distant capital in the upper right, with a dark warship moored by it. Fresh jade-green and grey tones. Keep the top 8% and bottom 10% calm. Landscape 1536x1024, no people, no text.` |
| 51 | `map_hoi_5.png` | `Stage-select map, a wide ink-wash landscape scroll of an imperial capital in high summer: long straight avenues, red palace walls, lotus ponds, a bronze-roofed workshop with gears, a large stone altar terrace. A path starts at the outer city gate at the bottom left, runs through the avenues and gardens, and ends at a grand palace hall glowing on a high platform in the upper right under a hot white sun. Warm vermilion, gold and ink tones. Keep the top 8% and bottom 10% calm. Landscape 1536x1024, no people, no text.` |
| 52 | `bg_battle_hoi_4.png` | `Battle background with the same composition as a duel stage: a flat wet stone dock at a city water-gate in spring rain, jade-green water with lantern reflections behind, a black warship hull with furled sails at one side, willow branches at the other edge, the large middle area empty and low-contrast so game boards sit on top. Landscape 1536x1024, no people, no text.` |
| 53 | `bg_battle_hoi_5.png` | `Battle background with the same composition as a duel stage: a flat white marble palace courtyard under harsh summer sun, bronze crossbow machines and red-orange ceremonial banners at both edges, a palace hall roof far behind in heat haze, the large middle area empty and low-contrast so game boards sit on top. Landscape 1536x1024, no people, no text.` |
| 54 | `node_hoi_4.png` | `Map marker icon: a round jade-green token shaped like a raindrop with a tiny willow leaf carved on it, soft ink-wash shading, thin pale gold rim, centered, transparent background, no text.` |
| 55 | `node_boss_hoi_4.png` | `Boss map marker icon: the prow of a black warship with a flame-shaped figurehead, splashed ink waves, inside a vermilion round frame, centered, transparent background, no text.` |
| 56 | `node_hoi_5.png` | `Map marker icon: a round vermilion lacquer medallion with a single open lotus flower in the center, carved in soft relief, surrounded by a few gentle swirling cloud wisps, thin pale gold rim, warm summer palace mood, ink-wash shading, centered, transparent background. No text, no letters, no symbols, no geometric patterns, no square or angular motifs, no fret, meander or key patterns, no swastika.` (bản cũ "square seal stamp" bị ra hoa văn giống chữ vạn) |
| 57 | `node_boss_hoi_5.png` | `Boss map marker icon: an interlocking bronze gear wheel with a glowing amber eye in the center, inside a vermilion round frame, centered, transparent background, no text.` |
| 58 | `npc_yen_vo_tam.png` | `Half-body portrait of a calm, sharp-eyed imperial censor in his early thirties, neat topknot with a black official cap, deep blue-green robe with a subtle crane emblem, holding a sealed secret memorial scroll, composed and intelligent, three-quarter view turned slightly to the right, from the waist up, transparent background, no text.` |
| 59 | `boss_lo_tan_quang.png` | `Half-body portrait of a cult envoy in his forties, lean and cruel, a black robe with ember-red trim like smouldering fire, wet hair from the rain, gripping a curved saber whose edge glows like hot coals, standing on a rainy ship deck, three-quarter view facing left, from the waist up, transparent background, no text.` |
| 60 | `boss_tao_huyen_ky.png` | `Half-body portrait of a cold scheming court mechanist in his fifties, thin grey beard, copper-red official robes with bronze gear-shaped ornaments, one hand raised over a glowing mechanical array of interlocking rings, calculating gaze, three-quarter view facing left, from the waist up, transparent background, no text.` |
## Đợt 4: chân dung Hồi 4–5 và Luận Kiếm

Tất cả prompt dưới đây dùng chung phần phong cách và ràng buộc: `Traditional Chinese shui-mo ink-wash game illustration, crisp expressive black ink contours, pale mineral washes, subtle aged rice-paper texture within the subject, matching the supplied style references. Vertical portrait 1024x1536, isolated centered cutout, complete head, arms, waist and weapon silhouette visible, three-quarter view facing left, genuine transparent alpha background, no scenery, no frame, no letters, no readable marks, no watermark, no text, and no existing fictional character.`

| File | Prompt |
|---|---|
| `enemy_hoi_4_01.png` | `Half-body portrait of a rugged roadside saber fighter in his late thirties, weathered face, tied black hair, iron-gray layered robe and leather shoulder guards, carrying a heavy broad saber suitable for the practical hard-hitting Thiet Phu school, alert and hostile, three-quarter view facing left, from the waist up, transparent background, no text.` |
| `enemy_hoi_4_02.png` | `Half-body portrait of a deceitful male courier guard in his twenties carrying a forged command scroll, neat but suspicious face, pale blue-gray flowing robes of the agile Thai Thanh sword school, narrow straight sword and messenger satchel, three-quarter view facing left, from the waist up, transparent background, no text.` |
| `enemy_hoi_4_03.png` | `Portrait of a supernatural ink-shadow spirit cat, sleek black feline with smoky brushstroke fur, luminous pale jade eyes, arched shoulders and one raised forepaw, head and front half of body facing left, transparent background, no text.` |
| `enemy_hoi_4_04.png` | `Half-body portrait of a shrewd female cloud-post innkeeper in her forties, poised calculating expression, layered jade-green and mist-blue robes of the fluid Bich Thuy school, holding an iron folding fan and a small abacus charm, three-quarter view facing left, from the waist up, transparent background, no text.` |
| `enemy_hoi_4_05.png` | `Half-body portrait of an ominous male feather-cloaked guard in his thirties, sharp masked eyes, black-purple robe and raven-feather mantle of the poison-using Huyen Doc school, holding a hooked dagger with faint green toxic mist, three-quarter view facing left, from the waist up, transparent background, no text.` |
| `enemy_hoi_4_06.png` | `Half-body portrait of a muscular male road-opening envoy in his forties, square jaw, iron-gray lamellar shoulder armor and rugged dark ochre robe of the hard-hitting Thiet Phu school, resting a massive double-headed battle axe across one shoulder, three-quarter view facing left, from the waist up, transparent background, no text.` |
| `enemy_hoi_4_07.png` | `Portrait of a colossal enraged bull with coal-black hide, blazing eyes, powerful neck and shoulders, two huge cinnabar-red horns and torn red harness ribbons, snorting ink-like steam, head and front half of body facing left, transparent background, no text.` |
| `enemy_hoi_4_08.png` | `Half-body portrait of a stern female cold-mountain signal escort in her early thirties, pale frost-blue layered armor and white fur collar of the Han Son school, carrying a long ice-steel spear and a small signal pennant with no markings, three-quarter view facing left, from the waist up, transparent background, no text.` |
| `enemy_hoi_4_09.png` | `Half-body portrait of a broad-shouldered male river-lock guard in his fifties, shaved head and disciplined expression, heavy bronze prayer-bead armor and dark gold robes of the defensive Kim Cang school, gripping a thick chain and square iron shield, three-quarter view facing left, from the waist up, transparent background, no text.` |
| `enemy_hoi_5_01.png` | `Half-body portrait of an imposing female counterfeit-edict imperial guard in her thirties, stern face, heavy dark-gold lamellar armor and crimson under-robe in the defensive Kim Cang manner, holding a broad ceremonial shield and short mace, three-quarter view facing left, from the waist up, transparent background, no text.` |
| `enemy_hoi_5_02.png` | `Half-body portrait of a swift female Vermilion-Bird shadow assassin in her twenties, sharp focused eyes, pale smoke-blue Thai Thanh robes with restrained crimson feather accents, twin slender straight swords, three-quarter view facing left, from the waist up, transparent background, no text.` |
| `enemy_hoi_5_03.png` | `Half-body portrait of an elderly male bronze automaton engineer, clever intense face, soot-stained gray artisan robe with bronze tool harness, holding a jointed bronze puppet controller while a compact mechanical doll perches on his forearm, three-quarter view facing left, from the waist up, transparent background, no text.` |
| `enemy_hoi_5_04.png` | `Half-body portrait of an arrogant young male false academy sword student, overly pristine pale blue-white scholarly Thai Thanh robe, smug expression, holding a narrow jian sword and an empty bamboo scroll, three-quarter view facing left, from the waist up, transparent background, no text.` |
| `enemy_hoi_5_05.png` | `Half-body portrait of a severe female celestial-mechanism seal officer in her forties, iron-gray and muted ochre official armor adapted to the Thiet Phu school, carrying a heavy square-headed pole hammer and a bronze command seal with no writing, three-quarter view facing left, from the waist up, transparent background, no text.` |
| `enemy_hoi_5_06.png` | `Half-body portrait of a gaunt male underworld sewer emissary in his fifties, sallow face and calculating eyes, layered black-purple waterproof Huyen Doc robes, holding a hooked chain blade and poison gourd leaking faint green vapor, three-quarter view facing left, from the waist up, transparent background, no text.` |
| `enemy_hoi_5_07.png` | `Portrait of a majestic gigantic golden-winged sun bird, fierce eagle-like head, brilliant ochre-gold feathers edged with vermilion flame, glowing amber eye, powerful chest and one spread wing, head and front half of body facing left, transparent background, no text.` |
| `enemy_hoi_5_08.png` | `Half-body portrait of a refined male imperial-lake zither master in his late thirties, serene but hostile expression, flowing jade-green and mist-blue Bich Thuy robes, carrying a compact seven-string guqin diagonally across his torso with one hand poised to pluck, three-quarter view facing left, from the waist up, transparent background, no text.` |
| `enemy_hoi_5_09.png` | `Half-body portrait of a battle-scarred male celestial-mechanism fire-crossbow guard in his thirties, ember-red and charcoal armor of the explosive Liet Diem school, gripping a heavy bronze repeating crossbow with glowing red mechanisms and a quiver of fire bolts, three-quarter view facing left, from the waist up, transparent background, no text.` |
| `pvp_avatar_1.png` | `Half-body portrait of a young adult female wandering swordswoman, calm determined face, practical neutral gray-blue travel robes and leather bracers, holding a plain single-edged saber, three-quarter view facing left, from the waist up, transparent background, no text.` |
| `pvp_avatar_2.png` | `Half-body portrait of a weathered male wandering spear fighter in his late forties, short beard and steady eyes, neutral brown hemp robe with simple iron shoulder guard, holding a long plain spear, three-quarter view facing left, from the waist up, transparent background, no text.` |
| `pvp_avatar_3.png` | `Half-body portrait of an elderly female jianghu staff fighter in her sixties, silver hair in a practical bun, alert wise expression, muted charcoal and dusty teal travel clothes, gripping a knotty ironwood staff, three-quarter view facing left, from the waist up, transparent background, no text.` |
| `pvp_avatar_4.png` | `Half-body portrait of a confident young male wandering archer in his early twenties, cropped tied hair, neutral sand and ink-black travel robe, holding a plain recurved bow with a quiver visible over one shoulder, three-quarter view facing left, from the waist up, transparent background, no text.` |
