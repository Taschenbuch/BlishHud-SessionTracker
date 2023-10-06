using System;
using System.Collections.Generic;
using System.Linq;
using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using SessionTracker.Models;
using SessionTracker.Services;
using SessionTracker.SettingEntries;
using SessionTracker.Settings;
using SessionTracker.Settings.Window;

namespace SessionTracker.Controls.Hint
{
    public class HintFlowPanel : FlowPanel
    {
        public HintFlowPanel(List<Stat> stats,
                             SettingsWindowService settingsWindowService,
                             TextureService textureService,
                             SettingService settingService,
                             Container parent)
        {
            _stats        = stats;
            _parent         = parent;
            _settingService = settingService;

            Parent = parent;

            _hiddenByZeroSessionValuesImage = new Image(textureService.HiddenStatsTexture)
            {
                Size = new Point(30),
                BasicTooltipText = "All stats are hidden because their current session values are 0.\n" +
                                   "Stats will be visible again when the session value is not 0 anymore.\n" +
                                   "This hidden-when-zero-feature can be turned off in the session tracker module settings."
            };

            var hiddenByUserHintText = "No stats selected.\n" +
                                       "Select the stats you want to see\n" +
                                       "in the session tracker module settings. :-)";

            _hiddenByUserLabel = new Label()
            {
                Text             = hiddenByUserHintText,
                BasicTooltipText = hiddenByUserHintText,
                ShowShadow       = true,
                AutoSizeHeight   = true,
                AutoSizeWidth    = true,
            };

            _openSettingsButton = new OpenSettingsButton(settingsWindowService);

            OnFontSizeIndexSettingChanged();
            settingService.FontSizeIndexSetting.SettingChanged += OnFontSizeIndexSettingChanged;
        }

        protected override void DisposeControl()
        {
            _settingService.FontSizeIndexSetting.SettingChanged -= OnFontSizeIndexSettingChanged;
            _hiddenByZeroSessionValuesImage?.Dispose();
            _hiddenByUserLabel?.Dispose();
            _openSettingsButton?.Dispose();
            base.DisposeControl();
        }

        public void ShowHintWhenAllStatsAreHidden(UpdateLoopState updateLoopState)
        {
            // remove all from parent to prevent messing up their order
            _hiddenByZeroSessionValuesImage.Parent = null;
            _hiddenByUserLabel.Parent              = null;
            _openSettingsButton.Parent             = null;

            var hintType = DetermineWhichHintToShow(_stats, updateLoopState, _settingService.HideStatsWithValueZeroSetting.Value);

            switch (hintType)
            {
                case HintType.AllStatsHiddenByUser:
                    _hiddenByZeroSessionValuesImage.Parent = null;
                    _hiddenByUserLabel.Parent              = this;
                    _openSettingsButton.Parent             = this;
                    Show();
                    break;
                case HintType.AllStatsHiddenByHideZeroValuesSetting:
                    _hiddenByZeroSessionValuesImage.Parent = this;
                    _hiddenByUserLabel.Parent              = null;
                    _openSettingsButton.Parent             = null;
                    Show();
                    break;
                case HintType.None:
                default:
                    _hiddenByZeroSessionValuesImage.Parent = null;
                    _hiddenByUserLabel.Parent              = null;
                    _openSettingsButton.Parent             = null;
                    Hide();
                    break;
            }
        }

        public override void Show()
        {
            Parent = _parent;
            base.Show();
        }

        public override void Hide()
        {
            Parent = null;
            base.Hide();
        }

        private void OnFontSizeIndexSettingChanged(object sender = null, ValueChangedEventArgs<int> e = null)
        {
            var font = FontService.Fonts[_settingService.FontSizeIndexSetting.Value];
            _hiddenByUserLabel.Font = font; 
            _hiddenByZeroSessionValuesImage.Size = new Point(font.LineHeight);
        }

        private static HintType DetermineWhichHintToShow(List<Stat> stats, UpdateLoopState updateLoopState, bool hideStatsWithValueZero)
        {
            var allHiddenByUser = stats.Any(e => e.IsVisible) == false;
            if (allHiddenByUser)
                return HintType.AllStatsHiddenByUser;

            var allHiddenBecauseOfZeroValue = stats.Any(e => e.IsVisible && e.HasNonZeroSessionValue) == false;
            var hasModuleInitializedStatValues = updateLoopState != UpdateLoopState.WaitingForApiTokenAfterModuleStart;
            if (hideStatsWithValueZero && allHiddenBecauseOfZeroValue && hasModuleInitializedStatValues)
                return HintType.AllStatsHiddenByHideZeroValuesSetting;

            return HintType.None;
        }

        private readonly SettingService _settingService;
        private readonly List<Stat> _stats;
        private readonly Container _parent;
        private readonly Label _hiddenByUserLabel;
        private readonly Image _hiddenByZeroSessionValuesImage;
        private readonly OpenSettingsButton _openSettingsButton;
    }
}