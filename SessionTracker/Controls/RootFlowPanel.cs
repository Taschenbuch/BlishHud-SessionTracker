using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using SessionTracker.Settings;

namespace SessionTracker.Controls
{
    public class RootFlowPanel : FlowPanel
    {
        public RootFlowPanel(Container parent, SettingService settingService)
        {
            _parent         = parent;
            _settingService = settingService;
            FlowDirection   = ControlFlowDirection.SingleLeftToRight;
            Height          = 200;
            WidthSizingMode = SizingMode.AutoSize;
            Parent          = parent;

            SetFixedOrAutoHeight();

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

        public void HideScrollbarIfExists()
        {
            if (_scrollbar != null && _semaphoreSlim.Wait(0) == false)
            {
                _scrollbar.Visible = false;

                Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(500);
                        if(_scrollbar != null) // because of task.Delay scrollbar could exist now when something was changed in the ui in the meantime
                            _scrollbar.Visible = false;
                    }
                    finally
                    {
                        _semaphoreSlim.Release();
                    }
                });
            }
        }

        private void OnUiHeightSettingChanged(object sender, ValueChangedEventArgs<int> e)
        {
            if (_settingService.UiHeightIsFixedSetting.Value)
                Height = _settingService.UiHeightSetting.Value;
        }

        private void OnUiHeightIsFixedSettingChanged(object sender, ValueChangedEventArgs<bool> e)
        {
            SetFixedOrAutoHeight();
        }

        private void SetFixedOrAutoHeight()
        {
            if (_settingService.UiHeightIsFixedSetting.Value)
            {
                HeightSizingMode = SizingMode.Standard;
                Height           = _settingService.UiHeightSetting.Value;
                CanScroll        = true;
                _scrollbar       = (Scrollbar)_parent.Children.First(c => c is Scrollbar);

                HideScrollbarIfExists();
            }
            else
            {
                _scrollbar       = null;
                HeightSizingMode = SizingMode.AutoSize;
                CanScroll        = false;
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
        private readonly Container _parent;
        private Scrollbar _scrollbar;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);
    }
}