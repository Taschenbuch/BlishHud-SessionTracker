using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Microsoft.Xna.Framework;
using SessionTracker.SettingEntries;
using Container = Blish_HUD.Controls.Container;

namespace SessionTracker.RelativePositionWindow
{
    public class RelativePositionAndMouseDraggableContainer : Container
    {
        public RelativePositionAndMouseDraggableContainer(SettingService settingService)
        {
            _settingService = settingService;

            Location = ConvertCoordinatesService.ConvertRelativeToAbsoluteCoordinates(
                settingService.XMainWindowRelativeLocationSetting.Value,
                settingService.YMainWindowRelativeLocationSetting.Value,
                GameService.Graphics.SpriteScreen.Size.X,
                GameService.Graphics.SpriteScreen.Size.Y);

            GameService.Input.Mouse.LeftMouseButtonReleased += OnLeftMouseButtonReleased;
            GameService.Graphics.SpriteScreen.Resized       += OnSpriteScreenResized;
        }

        public override void UpdateContainer(GameTime gameTime)
        {
            if (_settingService.DragWindowWithMouseIsEnabledSetting.Value && _containerIsDraggedByMouse)
            {
                var newLocation = Input.Mouse.Position - _mousePressedLocationInsideContainer;

                Location = ScreenBoundariesService.AdjustCoordinatesToKeepContainerInsideScreenBoundaries(newLocation, Size, GameService.Graphics.SpriteScreen.Size);
            }
        }

        protected override void DisposeControl()
        {
            GameService.Input.Mouse.LeftMouseButtonReleased -= OnLeftMouseButtonReleased;
            GameService.Graphics.SpriteScreen.Resized       -= OnSpriteScreenResized;
            base.DisposeControl();
        }

        protected override void OnLeftMouseButtonPressed(MouseEventArgs e)
        {
            if (_settingService.DragWindowWithMouseIsEnabledSetting.Value)
            {
                _containerIsDraggedByMouse           = true;
                _mousePressedLocationInsideContainer = Input.Mouse.Position - Location;
            }

            base.OnLeftMouseButtonPressed(e);
        }

        // not using the override on purpose because it does not register the release when clicking fast (workaround suggested by freesnow)
        private void OnLeftMouseButtonReleased(object sender, MouseEventArgs e)
        {
            if (_settingService.DragWindowWithMouseIsEnabledSetting.Value)
            {
                _containerIsDraggedByMouse = false;

                (_settingService.XMainWindowRelativeLocationSetting.Value, _settingService.YMainWindowRelativeLocationSetting.Value)
                    = ConvertCoordinatesService.ConvertAbsoluteToRelativeCoordinates(
                        Location.X,
                        Location.Y,
                        GameService.Graphics.SpriteScreen.Size.X,
                        GameService.Graphics.SpriteScreen.Size.Y);
            }
        }

        private void OnSpriteScreenResized(object sender, ResizedEventArgs resizedEventArgs)
        {
            Location = ConvertCoordinatesService.ConvertRelativeToAbsoluteCoordinates(
                _settingService.XMainWindowRelativeLocationSetting.Value,
                _settingService.YMainWindowRelativeLocationSetting.Value,
                GameService.Graphics.SpriteScreen.Size.X,
                GameService.Graphics.SpriteScreen.Size.Y);
        }

        private Point _mousePressedLocationInsideContainer = Point.Zero;
        private bool _containerIsDraggedByMouse;
        private readonly SettingService _settingService;
    }
}