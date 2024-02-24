using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using SessionTracker.Models;
using SessionTracker.OtherServices;

namespace SessionTracker.SelectStats
{
    public class SelectStatContainer : Container
    {
        public Stat Stat { get; }

        public SelectStatContainer(Stat stat, Services services, Container parent)
        {
            Stat = stat;
            _service = services;

            Parent = parent;
            Click += (sender, args) => ToggleSelectStat();

            _backgroundImage = new Image(services.TextureService.StatBackgroundTexture)
            {
                Location = new Point(BACKGROUND_IMAGE_MARGIN),
                BasicTooltipText = stat.GetTextWithNameAndDescription(),
                Parent = this,
            };

            _icon = new Image(services.TextureService.StatTextureByStatId[stat.Id])
            {
                Location = new Point(BACKGROUND_IMAGE_MARGIN + ICON_MARGIN),
                BasicTooltipText = stat.GetTextWithNameAndDescription(),
                Parent = this,
            };

            UpdateIconOpacity();
            SetIconSize((int)services.SettingService.SelectStatsIconSizeSetting.Value);
        }

        public void SetIconSize(int statIconSize)
        {
            Size = new Point(statIconSize + 2 * ICON_MARGIN + 2 * BACKGROUND_IMAGE_MARGIN);
            _backgroundImage.Size = new Point(statIconSize + 2 * ICON_MARGIN);
            _icon.Size = new Point(statIconSize);
        }

        public void ToggleSelectStat()
        {
            SelectOrUnselectStat(!Stat.IsVisible);
        }

        public void SelectStat()
        {
            SelectOrUnselectStat(true);
        }

        public void UnselectStat()
        {
            SelectOrUnselectStat(false);
        }

        public void SelectOrUnselectStat(bool isSelected)
        {
            Stat.IsVisible = isSelected;
            UpdateIconOpacity();
            _service.Model.UiHasToBeUpdated = true;
        }

        private void UpdateIconOpacity()
        {
            _icon.Opacity = Stat.IsVisible ? 1f : 0.3f;
        }

        private readonly Image _backgroundImage;
        private readonly Image _icon;
        private readonly Services _service;
        private const int ICON_MARGIN = 1;
        private const int BACKGROUND_IMAGE_MARGIN = 1;
    }
}
