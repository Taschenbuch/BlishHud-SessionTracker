using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using SessionTracker.Models;
using SessionTracker.OtherServices;

namespace SessionTracker.SelectStats
{
    public class SelectStatContainer : Container
    {
        public SelectStatContainer(SelectStatViewModel selectStatViewModel, string superCategoryId, string subCategoryId, Services services, Container parent)
        {
            _selectStatViewModel = selectStatViewModel;
            SuperCategoryId = superCategoryId;
            SubCategoryId = subCategoryId;

            Parent = parent;
            Click += (sender, args) => ToggleSelectStat();

            _backgroundImage = new Image(services.TextureService.StatBackgroundTexture)
            {
                Location = new Point(BACKGROUND_IMAGE_MARGIN),
                BasicTooltipText = Stat.GetTextWithNameAndDescription(),
                Parent = this,
            };

            _icon = new Image(services.TextureService.StatTextureByStatId[Stat.Id])
            {
                Location = new Point(BACKGROUND_IMAGE_MARGIN + ICON_MARGIN),
                BasicTooltipText = Stat.GetTextWithNameAndDescription(),
                Opacity = GetIconOpacity(_selectStatViewModel.IsSelected),
                Parent = this,
            };

            SetIconSize((int)services.SettingService.SelectStatsIconSizeSetting.Value);

            _selectStatViewModel.IsSelectedChanged += (s, e) =>
            {
                _icon.Opacity = GetIconOpacity(_selectStatViewModel.IsSelected);
            };
        }

        public Stat Stat => _selectStatViewModel.Stat;
        public string SuperCategoryId { get; }
        public string SubCategoryId { get; }

        public void SetIconSize(int statIconSize)
        {
            Size = new Point(statIconSize + 2 * ICON_MARGIN + 2 * BACKGROUND_IMAGE_MARGIN);
            _backgroundImage.Size = new Point(statIconSize + 2 * ICON_MARGIN);
            _icon.Size = new Point(statIconSize);
        }

        public void SelectStat()
        {
            SelectOrUnselectStat(true);
        }

        public void UnselectStat()
        {
            SelectOrUnselectStat(false);
        }

        private void ToggleSelectStat()
        {
            SelectOrUnselectStat(!_selectStatViewModel.IsSelected);
        }

        private void SelectOrUnselectStat(bool isSelected)
        {
            _selectStatViewModel.IsSelected = isSelected;
        }

        private static float GetIconOpacity(bool isSelected)
        {
           return isSelected ? 1f : 0.3f;
        }

        private readonly Image _backgroundImage;
        private readonly Image _icon;
        private readonly SelectStatViewModel _selectStatViewModel;
        private const int ICON_MARGIN = 1;
        private const int BACKGROUND_IMAGE_MARGIN = 1;
    }
}
