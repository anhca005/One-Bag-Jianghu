"""
Mô phỏng mùa giải Luận Kiếm Đài (Season Simulation) cho One Bag Jianghu.
Thiết kế số liệu: Gemini (2026-10-21).

Mô phỏng:
- Hệ thống Elo K=32, sàn 1000 (theo #68).
- Ghép trận ±50 điểm, nới dần theo #70.
- Mùa giải 28 ngày, 10 trận/ngày.
- Đo độ phình điểm do sàn 1000, phân bố bậc cuối mùa và bậc cao nhất.
- Kiểm tra cơ chế Soft Reset qua nhiều mùa giải.
- Kiểm tra kinh tế Danh Vọng và Chí Tôn Ngọc Tủy.
"""

import math
import random
import sys

# Ensure UTF-8 output on Windows console
if hasattr(sys.stdout, 'reconfigure'):
    try:
        sys.stdout.reconfigure(encoding='utf-8')
    except Exception:
        pass


def elo_expected(rating_a: float, rating_b: float) -> float:
    return 1.0 / (1.0 + 10.0 ** ((rating_b - rating_a) / 400.0))


def elo_change(rating_a: int, rating_b: int, score: float, k: int = 32) -> int:
    exp = elo_expected(rating_a, rating_b)
    return round(k * (score - exp))


class SeasonSimulation:
    def __init__(self, num_players: int = 500, k: int = 32, days: int = 28, matches_per_day: int = 10, seed: int = 42):
        self.num_players = num_players
        self.k = k
        self.days = days
        self.matches_per_day = matches_per_day
        self.rng = random.Random(seed)
        
        # True skill distribution ~ N(1000, 150)
        self.skills = [self.rng.gauss(1000, 150) for _ in range(num_players)]
        self.ratings = [1000 for _ in range(num_players)]
        self.peak_ratings = [1000 for _ in range(num_players)]
        self.total_matches = 0
        self.total_minted_points = 0

    def matchmake(self, player_idx: int) -> int:
        r_i = self.ratings[player_idx]
        window = 50
        while window <= 2000:
            candidates = [
                j for j in range(self.num_players)
                if j != player_idx and abs(self.ratings[j] - r_i) <= window
            ]
            if candidates:
                return self.rng.choice(candidates)
            window += 50
        return (player_idx + 1) % self.num_players

    def simulate_season(self, season_num: int = 1):
        self.peak_ratings = list(self.ratings)
        season_minted = 0
        daily_minted = []

        for day in range(1, self.days + 1):
            day_minted_start = season_minted
            # Randomize order of players taking turns to attack
            player_order = list(range(self.num_players))
            self.rng.shuffle(player_order)

            for i in player_order:
                for _ in range(self.matches_per_day):
                    j = self.matchmake(i)
                    
                    # Probability attacker i beats defender j based on true hidden skill
                    p_win = elo_expected(self.skills[i], self.skills[j])
                    won = self.rng.random() < p_win
                    score = 1.0 if won else 0.0
                    
                    r_i = self.ratings[i]
                    r_j = self.ratings[j]
                    delta = elo_change(r_i, r_j, score, self.k)
                    
                    new_i = max(1000, r_i + delta)
                    new_j = max(1000, r_j - delta)
                    
                    # Points minted from floor:
                    net_points = (new_i - r_i) + (new_j - r_j)
                    season_minted += net_points
                    
                    self.ratings[i] = new_i
                    self.ratings[j] = new_j
                    if new_i > self.peak_ratings[i]:
                        self.peak_ratings[i] = new_i
                    if new_j > self.peak_ratings[j]:
                        self.peak_ratings[j] = new_j
                    
                    self.total_matches += 1

            daily_minted.append(season_minted - day_minted_start)

        self.total_minted_points += season_minted
        return season_minted, daily_minted

    def apply_soft_reset(self, factor: float = 0.5):
        """Soft reset formula: 1000 + round((R - 1000) * factor), clamped to >= 1000."""
        self.ratings = [max(1000, 1000 + round((r - 1000) * factor)) for r in self.ratings]
        self.peak_ratings = list(self.ratings)

    def get_tier_breakdown(self, data_list):
        # Tiers per arena.json & decision #69
        # Vo Danh: 1000-1099, Tan Tu: 1100-1249, Thieu Hiep: 1250-1399,
        # Dai Hiep: 1400-1599, Danh Tuc: 1600-1799, Tong Su: >= 1800
        tiers = {
            "Vô Danh (1000–1099)": 0,
            "Tân Tú (1100–1249)": 0,
            "Thiếu Hiệp (1250–1399)": 0,
            "Đại Hiệp (1400–1599)": 0,
            "Danh Túc (1600–1799)": 0,
            "Tông Sư (>= 1800)": 0,
        }
        for r in data_list:
            if r < 1100:
                tiers["Vô Danh (1000–1099)"] += 1
            elif r < 1250:
                tiers["Tân Tú (1100–1249)"] += 1
            elif r < 1400:
                tiers["Thiếu Hiệp (1250–1399)"] += 1
            elif r < 1600:
                tiers["Đại Hiệp (1400–1599)"] += 1
            elif r < 1800:
                tiers["Danh Túc (1600–1799)"] += 1
            else:
                tiers["Tông Sư (>= 1800)"] += 1
        return tiers

    def stats(self, data_list):
        s = sorted(data_list)
        n = len(s)
        mean_val = sum(s) / n
        std_val = math.sqrt(sum((x - mean_val) ** 2 for x in s) / n)
        return {
            "min": s[0],
            "p10": s[int(n * 0.10)],
            "p25": s[int(n * 0.25)],
            "median": s[n // 2],
            "p75": s[int(n * 0.75)],
            "p90": s[int(n * 0.90)],
            "max": s[-1],
            "mean": mean_val,
            "std": std_val,
        }


def run_full_simulation():
    print("=" * 70)
    print("ONE BAG JIANGHU - MÔ PHỎNG MÙA GIẢI LUẬN KIẾM ĐÀI (SEASON SIMULATION)")
    print("=" * 70)
    
    sim = SeasonSimulation(num_players=500, k=32, days=28, matches_per_day=10, seed=12345)
    
    for s in range(1, 4):
        print(f"\n>>> BẮT ĐẦU MÙA GIẢI {s} (28 ngày, 500 người chơi, 10 trận/ngày)...")
        minted, daily_minted = sim.simulate_season(season_num=s)
        
        final_stats = sim.stats(sim.ratings)
        peak_stats = sim.stats(sim.peak_ratings)
        
        print(f"--- KẾT THÚC MÙA {s} ---")
        print(f"Tổng điểm sinh ra từ sàn 1000: +{minted} điểm (+{minted / sim.num_players:.1f} điểm/người chơi)")
        print(f"Trung bình điểm sinh ra mỗi ngày: {minted / sim.days:.1f} điểm/ngày")
        print(f"Điểm lúc chốt sổ (Final Rating): Min={final_stats['min']}, Median={final_stats['median']}, "
              f"Mean={final_stats['mean']:.1f}, Max={final_stats['max']}, Std={final_stats['std']:.1f}")
        print(f"Điểm cao nhất mùa (Peak Rating): Min={peak_stats['min']}, Median={peak_stats['median']}, "
              f"Mean={peak_stats['mean']:.1f}, Max={peak_stats['max']}, Std={peak_stats['std']:.1f}")
        
        print("\n[Phân bố Bậc lúc chốt sổ (Final Rating)]:")
        final_tiers = sim.get_tier_breakdown(sim.ratings)
        for tier_name, count in final_tiers.items():
            pct = count * 100.0 / sim.num_players
            print(f"  {tier_name:<25}: {count:>3} người ({pct:>5.1f}%)")
            
        print("\n[Phân bố Bậc cao nhất trong mùa (Peak Rating)]:")
        peak_tiers = sim.get_tier_breakdown(sim.peak_ratings)
        for tier_name, count in peak_tiers.items():
            pct = count * 100.0 / sim.num_players
            print(f"  {tier_name:<25}: {count:>3} người ({pct:>5.1f}%)")

        if s < 3:
            print(f"\n--- Áp dụng Soft Reset mùa {s} -> {s+1}: 1000 + round((R - 1000) * 0.5) ---")
            sim.apply_soft_reset(factor=0.5)
            reset_stats = sim.stats(sim.ratings)
            print(f"Điểm sau khi đặt lại: Min={reset_stats['min']}, Median={reset_stats['median']}, "
                  f"Mean={reset_stats['mean']:.1f}, Max={reset_stats['max']}")
            reset_tiers = sim.get_tier_breakdown(sim.ratings)
            print("[Phân bố Bậc ngay sau Soft Reset]:")
            for tier_name, count in reset_tiers.items():
                pct = count * 100.0 / sim.num_players
                print(f"  {tier_name:<25}: {count:>3} người ({pct:>5.1f}%)")

    print_economic_analysis()


def print_economic_analysis():
    print("\n" + "=" * 70)
    print("KIỂM TRA KINH TẾ LUẬN KIẾM ĐÀI (28 NGÀY / MÙA)")
    print("=" * 70)

    profiles = [
        {"name": "Người chơi đều đặn (Casual)", "matches_day": 10, "win_rate": 0.50, "peak_tier": "Thiếu Hiệp (1250–1399)"},
        {"name": "Người chơi giỏi (Hardcore)", "matches_day": 10, "win_rate": 0.65, "peak_tier": "Tông Sư (>= 1800)"},
        {"name": "Người chơi ít / mới", "matches_day": 5, "win_rate": 0.40, "peak_tier": "Tân Tú (1100–1249)"},
    ]

    print(f"{'Nhóm người chơi':<30} | {'Danh Vọng/ngày':<15} | {'Danh Vọng/mùa':<15} | {'Ngọc Tủy (Tuần+Shop)':<22} | {'Thời gian đủ 5 NT'}")
    print("-" * 105)

    for p in profiles:
        m_day = p["matches_day"]
        wr = p["win_rate"]
        wins_day = m_day * wr
        loss_day = m_day * (1 - wr)
        dv_day = wins_day * 30 + loss_day * 10
        dv_season = dv_day * 28

        # Weekly wins
        wins_week = wins_day * 7
        weekly_jade = 4 if wins_week >= 10 else int(wins_week * 4 / 10)
        weekly_jade = min(4, weekly_jade)

        # Shop jade (500 DV each, max 1/week = 4/season)
        shop_jade = min(4, int(dv_season // 500))

        base_jade = weekly_jade + shop_jade
        # Days to get 5 jade without season rewards
        jade_per_week = (1 if wins_week >= 10 else 0) + (1 if dv_day * 7 >= 500 else 0)
        if jade_per_week > 0:
            weeks_for_5 = math.ceil(5.0 / jade_per_week)
            days_for_5 = f"~{weeks_for_5 * 7} ngày ({weeks_for_5} tuần)"
        else:
            days_for_5 = "Rất lâu"

        print(f"{p['name']:<30} | {dv_day:<15.0f} | {dv_season:<15.0f} | {weekly_jade} tuần + {shop_jade} shop = {base_jade:<3} | {days_for_5}")

    print("-" * 105)
    print("NHẬN XÉT:")
    print("1. Tốc độ nhận Chí Tôn Ngọc Tủy hiện tại quá nhanh: chỉ mất 14-21 ngày để đủ 5 viên lên 1 món Chí Tôn.")
    print("2. Trong 1 mùa (28 ngày), cả Casual và Hardcore đều kiếm được ít nhất 8 viên Ngọc Tủy (chưa tính thưởng mùa).")
    print("3. Cần giảm lượng Ngọc Tủy thường nhật (tăng giá/hạ limit ở shop hoặc nâng mốc tuần) và tập trung Ngọc Tủy vào thưởng cuối mùa theo bậc.")


if __name__ == "__main__":
    run_full_simulation()
