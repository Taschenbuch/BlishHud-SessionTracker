using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;
using SessionTracker.Models;
using SessionTracker.OtherServices;
using SessionTracker.SettingEntries;
using SessionTracker.StatTooltip;
using System;

namespace SessionTracker.StatsWindow
{
    public class StatTitleFlowPanel : FlowPanel
    {
        public StatTitleFlowPanel(Stat stat, Container parent, Services services)
        {
            _stat           = stat;
            _settingService = services.SettingService;

            FlowDirection    = ControlFlowDirection.SingleLeftToRight;
            WidthSizingMode  = SizingMode.AutoSize;
            HeightSizingMode = SizingMode.AutoSize;
            Parent           = stat.IsVisible ? parent : null;

            var titleLabel = new Label()
            {
                Text           = stat.Name.Localized,
                TextColor      = _settingService.TitleLabelColorSetting.Value.GetColor(),
                ShowShadow     = true,
                AutoSizeHeight = true,
                AutoSizeWidth  = true,
                Tooltip        = new SummaryTooltip()
            };

            var asyncTexture2D = services.TextureService.StatTextureByStatId[stat.Id];
            _titleImage = new Image(asyncTexture2D)
            {
                Size    = new Point(titleLabel.Height),
                Tooltip = new SummaryTooltip()
            };

            _titleLabel                     = titleLabel;
            _paddingLabelBetweenIconAndText = CreatePaddingLabel();
            _paddingLabelBeforeValue        = CreatePaddingLabel();

            OnStatTitlePaddingSettingChanged();
            OnStatTitleWidthOrIsFixedSettingChanged();
            ShowOrHideTextAndIcon(_settingService.LabelTypeSetting.Value);
            _settingService.StatTitleWidthIsFixedSetting.SettingChanged += OnStatTitleWidthOrIsFixedSettingChanged;
            _settingService.StatTitleWidthSetting.SettingChanged        += OnStatTitleWidthOrIsFixedSettingChanged;
            _settingService.StatTitlePaddingSetting.SettingChanged      += OnStatTitlePaddingSettingChanged;
            _settingService.LabelTypeSetting.SettingChanged             += OnLabelTypeSettingChanged;
            _settingService.TitleLabelColorSetting.SettingChanged       += OnTitleLabelColorSettingChanged;
        }

        protected override void DisposeControl()
        {
            _settingService.StatTitleWidthIsFixedSetting.SettingChanged -= OnStatTitleWidthOrIsFixedSettingChanged;
            _settingService.StatTitleWidthSetting.SettingChanged        -= OnStatTitleWidthOrIsFixedSettingChanged;
            _settingService.StatTitlePaddingSetting.SettingChanged      -= OnStatTitlePaddingSettingChanged;
            _settingService.LabelTypeSetting.SettingChanged             -= OnLabelTypeSettingChanged;
            _settingService.TitleLabelColorSetting.SettingChanged       -= OnTitleLabelColorSettingChanged;
            base.DisposeControl();
        }

        public void UpdateTooltips(SummaryTooltipContent summaryTooltipContent)
        {
            ((SummaryTooltip)_titleLabel.Tooltip).UpdateTooltip(summaryTooltipContent);
            ((SummaryTooltip)_titleImage.Tooltip).UpdateTooltip(summaryTooltipContent);
        }

        public void UpdateLabelText()
        {
            _titleLabel.Text = _stat.Name.Localized;
        }

        public void SetFont(BitmapFont font)
        {
            _titleLabel.Font = font;
            _paddingLabelBetweenIconAndText.Font = font;
            _paddingLabelBeforeValue.Font = font;
            _titleImage.Size = new Point(_titleLabel.Height);
        }

        private void OnTitleLabelColorSettingChanged(object sender, ValueChangedEventArgs<ColorType> e)
        {
            _titleLabel.TextColor = e.NewValue.GetColor();
        }

        private void OnStatTitlePaddingSettingChanged(object sender = null, ValueChangedEventArgs<int> e = null)
        {
            var numberOfBlanks = _settingService.StatTitlePaddingSetting.Value;
            var paddingBlanks  = new string(' ', numberOfBlanks);
            _paddingLabelBeforeValue.Text        = paddingBlanks;
            _paddingLabelBetweenIconAndText.Text = paddingBlanks;
        }

        private void OnLabelTypeSettingChanged(object sender, ValueChangedEventArgs<LabelType> e)
        {
            ShowOrHideTextAndIcon(e.NewValue);
        }

        private void OnStatTitleWidthOrIsFixedSettingChanged(object sender = null, EventArgs e = null)
        {
            Width = _settingService.StatTitleWidthSetting.Value;
            WidthSizingMode = _settingService.StatTitleWidthIsFixedSetting.Value
                ? SizingMode.Standard
                : SizingMode.AutoSize;
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
            _titleImage.Parent                     = null;
            _paddingLabelBetweenIconAndText.Parent = null;
            _titleLabel.Parent                     = null;
            _paddingLabelBeforeValue.Parent        = null;

            switch (labelType)
            {
                case LabelType.Icon:
                    _titleImage.Parent              = this;
                    _paddingLabelBeforeValue.Parent = this;
                    break;
                case LabelType.Text:
                    _titleLabel.Parent              = this;
                    _paddingLabelBeforeValue.Parent = this;
                    break;
                case LabelType.IconAndText:
                    _titleImage.Parent                     = this;
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