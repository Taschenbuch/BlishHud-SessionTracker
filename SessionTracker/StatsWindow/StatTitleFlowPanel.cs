using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;
using SessionTracker.Constants;
using SessionTracker.Models;
using SessionTracker.Services;
using SessionTracker.SettingEntries;
using SessionTracker.StatTooltip;

namespace SessionTracker.StatsWindow
{
    public class StatTitleFlowPanel : FlowPanel
    {
        public StatTitleFlowPanel(Stat stat, Container parent, TextureService textureService, SettingService settingService)
        {
            _stat           = stat;
            _settingService = settingService;

            FlowDirection    = ControlFlowDirection.SingleLeftToRight;
            WidthSizingMode  = SizingMode.AutoSize;
            HeightSizingMode = SizingMode.AutoSize;
            Parent           = stat.IsVisible ? parent : null;

            var titleLabel = new Label()
            {
                Text           = stat.Name.Localized,
                TextColor      = settingService.TitleLabelColorSetting.Value.GetColor(),
                ShowShadow     = true,
                AutoSizeHeight = true,
                AutoSizeWidth  = true,
                Tooltip        = new SummaryTooltip()
            };

            var asyncTexture2D = textureService.StatTextureByStatId[stat.Id];
            _titleImage = new Image(asyncTexture2D)
            {
                Size    = new Point(titleLabel.Height),
                Tooltip = new SummaryTooltip()
            };

            _titleLabel                     = titleLabel;
            _paddingLabelBetweenIconAndText = CreatePaddingLabel();
            _paddingLabelBeforeValue        = CreatePaddingLabel();

            OnStatTitlePaddingSettingChanged();
            ShowOrHideTextAndIcon(settingService.LabelTypeSetting.Value);
            settingService.StatTitlePaddingSetting.SettingChanged += OnStatTitlePaddingSettingChanged;
            settingService.LabelTypeSetting.SettingChanged        += OnLabelTypeSettingChanged;
            settingService.TitleLabelColorSetting.SettingChanged  += OnTitleLabelColorSettingChanged;
        }

        public void UpdateTooltips(SummaryTooltipContent summaryTooltipContent)
        {
            ((SummaryTooltip)_titleLabel.Tooltip).UpdateTooltip(summaryTooltipContent);
            ((SummaryTooltip)_titleImage.Tooltip).UpdateTooltip(summaryTooltipContent);
        }

        protected override void DisposeControl()
        {
            _settingService.StatTitlePaddingSetting.SettingChanged -= OnStatTitlePaddingSettingChanged;
            _settingService.LabelTypeSetting.SettingChanged        -= OnLabelTypeSettingChanged;
            _settingService.TitleLabelColorSetting.SettingChanged  -= OnTitleLabelColorSettingChanged;
            base.DisposeControl();
        }

        public void UpdateLabelText()
        {
            _titleLabel.Text             = _stat.Name.Localized;
            _titleLabel.BasicTooltipText = _stat.Description.Localized;
            _titleImage.BasicTooltipText = _stat.Description.Localized;
        }

        public void SetFont(BitmapFont font)
        {
            _titleLabel.Font = font;
            _paddingLabelBetweenIconAndText.Font = font;
            _paddingLabelBeforeValue.Font = font;
            _titleImage.Size = new Point(_titleLabel.Height);
        }

        private void OnTitleLabelColorSettingChanged(object sender, Blish_HUD.ValueChangedEventArgs<ColorType> e)
        {
            _titleLabel.TextColor = e.NewValue.GetColor();
        }

        private void OnStatTitlePaddingSettingChanged(object sender = null, Blish_HUD.ValueChangedEventArgs<int> e = null)
        {
            var numberOfBlanks = _settingService.StatTitlePaddingSetting.Value;
            var paddingBlanks  = new string(' ', numberOfBlanks);
            _paddingLabelBeforeValue.Text        = paddingBlanks;
            _paddingLabelBetweenIconAndText.Text = paddingBlanks;
        }

        private void OnLabelTypeSettingChanged(object sender, Blish_HUD.ValueChangedEventArgs<LabelType> e)
        {
            ShowOrHideTextAndIcon(e.NewValue);
        }

        private static Label CreatePaddingLabel()
        {
            return new Label
            {
                Text           = "  ",
                AutoSizeHeight = true,
                AutoSizeWidth  = true
            };
        }

        private void ShowOrHideTextAndIcon(LabelType labelType)
        {
            // without reset the icon may end up left or right of the text because container is a flowPanel
            _titleImage.Parent                      = null;
            _paddingLabelBetweenIconAndText.Parent = null;
            _titleLabel.Parent                     = null;
            _paddingLabelBeforeValue.Parent        = null;

            switch (labelType)
            {
                case LabelType.Icon:
                    _titleImage.Parent               = this;
                    _paddingLabelBeforeValue.Parent = this;
                    break;
                case LabelType.Text:
                    _titleLabel.Parent              = this;
                    _paddingLabelBeforeValue.Parent = this;
                    break;
                case LabelType.IconAndText:
                    _titleImage.Parent                      = this;
                    _paddingLabelBetweenIconAndText.Parent = this;
                    _titleLabel.Parent                     = this;
                    _paddingLabelBeforeValue.Parent        = this;
                    break;
            }
        }

        private readonly Image _titleImage;
        private readonly Label _titleLabel;
        private readonly Stat _stat;
        private readonly SettingService _settingService;
        private readonly Label _paddingLabelBetweenIconAndText;
        private readonly Label _paddingLabelBeforeValue;
    }
}