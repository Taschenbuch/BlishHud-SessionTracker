using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;
using SessionTracker.Models;
using SessionTracker.Services;
using SessionTracker.Settings;

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

            _label = new Label()
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
                Size             = new Point(_label.Height),
                Parent           = this,
            };

            _label.Parent = this; // because image has to be first, but image needs label height

            ShowOrHideTextAndIcon(settingService.LabelTypeSetting.Value);
            settingService.LabelTypeSetting.SettingChanged       += OnLabelTypeSettingChanged;
            settingService.FontSizeIndexSetting.SettingChanged   += OnFontSizeIndexSettingChanged;
            settingService.TitleLabelColorSetting.SettingChanged += OnTitleLabelColorSettingChanged;
        }

        protected override void DisposeControl()
        {
            _settingService.LabelTypeSetting.SettingChanged       -= OnLabelTypeSettingChanged;
            _settingService.FontSizeIndexSetting.SettingChanged   -= OnFontSizeIndexSettingChanged;
            _settingService.TitleLabelColorSetting.SettingChanged -= OnTitleLabelColorSettingChanged;
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
            _label.Font = FontService.Fonts[_settingService.FontSizeIndexSetting.Value];
            _image.Size = new Point(_label.Height);
        }

        private void OnLabelTypeSettingChanged(object sender, Blish_HUD.ValueChangedEventArgs<LabelType> e)
        {
            ShowOrHideTextAndIcon(e.NewValue);
        }

        private void ShowOrHideTextAndIcon(LabelType labelType)
        {
            _image.Parent = null; // without reset the icon may end up left or right of the text because container is a flowPanel
            _label.Parent = null;

            switch (labelType)
            {
                case LabelType.Icon:
                    _image.Parent = this;
                    break;
                case LabelType.Text:
                    _label.Parent = this;
                    break;
                case LabelType.IconAndText:
                    _image.Parent = this;
                    _label.Parent = this;
                    break;
            }
        }

        private readonly Image _image;
        private readonly Label _label;
        private readonly Container _parent;
        private readonly SettingService _settingService;
    }
}