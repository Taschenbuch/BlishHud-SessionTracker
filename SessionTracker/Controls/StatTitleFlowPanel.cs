using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;
using SessionTracker.Models;
using SessionTracker.Services;
using SessionTracker.Settings.SettingEntries;

namespace SessionTracker.Controls
{
    public class StatTitleFlowPanel : FlowPanel
    {
        public StatTitleFlowPanel(Stat stat, BitmapFont font, Container parent, TextureService textureService, SettingService settingService)
        {
            _stat          = stat;
            _parent         = parent;
            _settingService = settingService;

            FlowDirection    = ControlFlowDirection.SingleLeftToRight;
            WidthSizingMode  = SizingMode.AutoSize;
            HeightSizingMode = SizingMode.AutoSize;
            Parent           = stat.IsVisible ? parent : null;

            var titleLabel = new Label()
            {
                Text           = stat.Name.Localized,
                TextColor      = settingService.TitleLabelColorSetting.Value.GetColor(),
                Font           = font,
                ShowShadow     = true,
                AutoSizeHeight = true,
                AutoSizeWidth  = true,
            };

            var asyncTexture2D = textureService.StatTextureByStatId[stat.Id];
            _titleImage = new Image(asyncTexture2D)
            {
                Size = new Point(titleLabel.Height),
            };

            _titleLabel                     = titleLabel;
            _paddingLabelBetweenIconAndText = CreatePaddingLabel(font);
            _paddingLabelBeforeValue        = CreatePaddingLabel(font);

            OnStatTitlePaddingSettingChanged();
            ShowOrHideTextAndIcon(settingService.LabelTypeSetting.Value);
            settingService.StatTitlePaddingSetting.SettingChanged += OnStatTitlePaddingSettingChanged;
            settingService.LabelTypeSetting.SettingChanged        += OnLabelTypeSettingChanged;
            settingService.FontSizeIndexSetting.SettingChanged    += OnFontSizeIndexSettingChanged;
            settingService.TitleLabelColorSetting.SettingChanged  += OnTitleLabelColorSettingChanged;
        }

        public void SetTooltip(string tooltipText)
        {
            _titleLabel.BasicTooltipText = tooltipText;
            _titleImage.BasicTooltipText = tooltipText;
            BasicTooltipText             = tooltipText; // does not work for the flowPanel. Probably because it is transparent or because label and image are above it.
        }

        protected override void DisposeControl()
        {
            _settingService.StatTitlePaddingSetting.SettingChanged -= OnStatTitlePaddingSettingChanged;
            _settingService.LabelTypeSetting.SettingChanged        -= OnLabelTypeSettingChanged;
            _settingService.FontSizeIndexSetting.SettingChanged    -= OnFontSizeIndexSettingChanged;
            _settingService.TitleLabelColorSetting.SettingChanged  -= OnTitleLabelColorSettingChanged;
            base.DisposeControl();
        }

        public void UpdateLabelText()
        {
            _titleLabel.Text             = _stat.Name.Localized;
            _titleLabel.BasicTooltipText = _stat.Description.Localized;
            _titleImage.BasicTooltipText = _stat.Description.Localized;
        }

        public override void Show()
        {
            Parent = _parent;
            base.Show();
        }

        private void OnTitleLabelColorSettingChanged(object sender, Blish_HUD.ValueChangedEventArgs<ColorType> e)
        {
            _titleLabel.TextColor = e.NewValue.GetColor();
        }

        private void OnFontSizeIndexSettingChanged(object sender, Blish_HUD.ValueChangedEventArgs<int> e)
        {
            var font = FontService.Fonts[_settingService.FontSizeIndexSetting.Value];
            _titleLabel.Font                     = font;
            _paddingLabelBetweenIconAndText.Font = font;
            _paddingLabelBeforeValue.Font        = font;
            _titleImage.Size                      = new Point(_titleLabel.Height);
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

        private static Label CreatePaddingLabel(BitmapFont font)
        {
            return new Label
            {
                Text           = "  ",
                Font           = font,
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
        private readonly Container _parent;
        private readonly SettingService _settingService;
        private readonly Label _paddingLabelBetweenIconAndText;
        private readonly Label _paddingLabelBeforeValue;
    }
}