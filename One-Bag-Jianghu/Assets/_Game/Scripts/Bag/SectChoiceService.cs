using OneBagJianghu.Economy;

namespace OneBagJianghu.Bag
{
    public sealed class SectChoiceService
    {
        private readonly SectCatalog catalog;
        public SectChoiceService(SectCatalog catalog)
        {
            this.catalog = catalog;
        }

        public bool TryChoose(PlayerProfile profile, string sectId, out string error)
        {
            if (profile.HeroLevel < LevelService.SectUnlockLevel)
            {
                error = "Đạt cấp 10 để gia nhập môn phái";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(profile.SectId))
            {
                error = "Môn phái đã chọn không thể thay đổi.";
                return false;
            }

            if (!catalog.TryGet(sectId, out _))
            {
                error = "Môn phái không hợp lệ.";
                return false;
            }

            profile.SectId = sectId;
            profile.TamPhapLevel = 1;
            error = string.Empty;
            return true;
        }
    }
}