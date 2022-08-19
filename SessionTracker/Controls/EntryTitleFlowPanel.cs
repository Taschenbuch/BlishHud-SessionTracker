using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;
using SessionTracker.Models;
using SessionTracker.Services;
using SessionTracker.Settings.SettingEntries;

namespace SessionTracker.Controls
{
    public class EntryTitleFlowPanel : FlowPanel
    {
        public EntryTitleFlowPanel(Entry entry, BitmapFont font, Container parent, TextureService textureService, SettingService settingService)
        {
            _parent         = parent;
            _settingService = settingService;

            FlowDirection    = ControlFlowDirection.SingleLeftToRight;
            WidthSizingMode  = SizingMode.AutoSize;
            HeightSizingMode = SizingMode.AutoSize;
            Parent           = entry.IsVisible ? parent : null;

            var label = new Label()
            {
                Text             = entry.LabelText,
                TextColor        = settingService.TitleLabelColorSetting.Value.GetColor(),
                Font             = font,
                BasicTooltipText = entry.LabelTooltip,
                ShowShadow       = true,
                AutoSizeHeight   = true,
                AutoSizeWidth    = true,
            };

            var asyncTexture2D = textureService.EntryTextureByEntryId[entry.Id];
            _image = new Image(asyncTexture2D)
            {
                BasicTooltipText = entry.LabelTooltip,
                Size             = new Point(label.Height),
            };

            _label                          = label;
            _paddingLabelBetweenIconAndText = CreatePaddingLabel(font);
            _paddingLabelBeforeValue        = CreatePaddingLabel(font);

            OnStatTitlePaddingSettingChanged(null, null);
            ShowOrHideTextAndIcon(settingService.LabelTypeSetting.Value);
            settingService.StatTitlePaddingSetting.SettingChanged += OnStatTitlePaddingSettingChanged;
            settingService.LabelTypeSetting.SettingChanged        += OnLabelTypeSettingChanged;
            settingService.FontSizeIndexSetting.SettingChanged    += OnFontSizeIndexSettingChanged;
            settingService.TitleLabelColorSetting.SettingChanged  += OnTitleLabelColorSettingChanged;
        }

        protected override void DisposeControl()
        {
            _settingService.StatTitlePaddingSetting.SettingChanged -= OnStatTitlePaddingSettingChanged;
            _settingService.LabelTypeSetting.SettingChanged        -= OnLabelTypeSettingChanged;
            _settingService.FontSizeIndexSetting.SettingChanged    -= OnFontSizeIndexSettingChanged;
            _settingService.TitleLabelColorSetting.SettingChanged  -= OnTitleLabelColorSettingChanged;
            base.DisposeControl();
        }

        public override void Show()
        {
            Parent = _parent;
            base.Show();
        }

        private void OnTitleLabelColorSettingChanged(object sender, Blish_HUD.ValueChangedEventArgs<ColorType> e)
        {
            _label.TextColor = e.NewValue.GetColor();
        }

        private void OnFontSizeIndexSettingChanged(object sender, Blish_HUD.ValueChangedEventArgs<int> e)
        {
            var font = FontService.Fonts[_settingService.FontSizeIndexSetting.Value];
            _label.Font                          = font;
            _paddingLabelBetweenIconAndText.Font = font;
            _paddingLabelBeforeValue.Font        = font;
            _image.Size                          = new Point(_label.Height);
        }

        private void OnStatTitlePaddingSettingChanged(object sender, Blish_HUD.ValueChangedEventArgs<int> e)
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
            _image.Parent                          = null;
            _paddingLabelBetweenIconAndText.Parent = null;
            _label.Parent                          = null;
            _paddingLabelBeforeValue.Parent        = null;

            switch (labelType)
            {
                case LabelType.Icon:
                    _image.Parent                   = this;
                    _paddingLabelBeforeValue.Parent = this;
                    break;
                case LabelType.Text:
                    _label.Parent                   = this;
                    _paddingLabelBeforeValue.Parent = this;
                    break;
                case LabelType.IconAndText:
                    _image.Parent                          = this;
                    _paddingLabelBetweenIconAndText.Parent = this;
                    _label.Parent                          = this;
                    _paddingLabelBeforeValue.Parent        = this;
                    break;
            }
        }

        private readonly Image _image;
        private readonly Label _label;
        private readonly Container _parent;
        private readonly SettingService _settingService;
        private readonly Label _paddingLabelBetweenIconAndText;
        private readonly Label _paddingLabelBeforeValue;
    }
}