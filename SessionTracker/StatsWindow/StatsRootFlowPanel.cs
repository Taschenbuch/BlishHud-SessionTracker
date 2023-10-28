using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using SessionTracker.SettingEntries;

namespace SessionTracker.StatsWindow
{
    public class StatsRootFlowPanel : FlowPanel
    {
        public StatsRootFlowPanel(SettingService settingService)
        {
            _settingService = settingService;
            FlowDirection   = ControlFlowDirection.SingleLeftToRight;
            Height          = 200;
            WidthSizingMode = SizingMode.AutoSize;

            OnUiHeightIsFixedSettingChanged();

            settingService.UiHeightSetting.SettingChanged        += OnUiHeightSettingChanged;
            settingService.UiHeightIsFixedSetting.SettingChanged += OnUiHeightIsFixedSettingChanged;
            MouseEntered                                         += OnMouseEntered;
            MouseLeft                                            += OnMouseLeft;
        }

        protected override void DisposeControl()
        {
            _settingService.UiHeightSetting.SettingChanged        -= OnUiHeightSettingChanged;
            _settingService.UiHeightIsFixedSetting.SettingChanged -= OnUiHeightIsFixedSettingChanged;
            MouseEntered                                          -= OnMouseEntered;
            MouseLeft                                             -= OnMouseLeft;
            base.DisposeControl();
        }

        public void SetParentAndHandleScrollbar(Container parent)
        {
            if (Parent == parent) // prevents that scrollbar is disabled on every session update/reset.
                return;

            Parent = parent;
            OnUiHeightIsFixedSettingChanged();
        }

        private void OnUiHeightSettingChanged(object sender, ValueChangedEventArgs<int> e)
        {
            if (_settingService.UiHeightIsFixedSetting.Value)
                Height = _settingService.UiHeightSetting.Value;
        }

        private void OnUiHeightIsFixedSettingChanged(object sender = null, ValueChangedEventArgs<bool> e = null)
        {
            if (Parent == null) // prevents exception when this flowpanel is hidden and this code trys to get the scrollbar
                return;

            if (_settingService.UiHeightIsFixedSetting.Value)
            {
                HeightSizingMode = SizingMode.Standard;
                Height           = _settingService.UiHeightSetting.Value;
                CanScroll        = true;
                _scrollbar       = (Scrollbar)Parent.Children.First(c => c is Scrollbar);
                HideScrollbarIfExists();
            }
            else
            {
                _scrollbar       = null;
                HeightSizingMode = SizingMode.AutoSize;
                CanScroll        = false;
            }
        }

        private void HideScrollbarIfExists()
        {
            if (_scrollbar != null && _semaphoreSlim.Wait(0) == false)
            {
                _scrollbar.Visible = false;

                Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(500);
                        if (_scrollbar != null) // because of task.Delay scrollbar could exist now when something was changed in the ui in the meantime
                            _scrollbar.Visible = false;
                    }
                    finally
                    {
                        _semaphoreSlim.Release();
                    }
                });
            }
        }

        private void OnMouseEntered(object s, MouseEventArgs ev)
        {
            if (_scrollbar != null)
                _scrollbar.Visible = true;
        }

        private void OnMouseLeft(object s, MouseEventArgs ev)
        {
            if (_scrollbar != null)
                _scrollbar.Visible = false;
        }

        private readonly SettingService _settingService;
        private Scrollbar _scrollbar;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);
    }
}