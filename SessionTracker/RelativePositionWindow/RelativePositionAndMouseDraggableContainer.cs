using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Microsoft.Xna.Framework;
using SessionTracker.SettingEntries;

namespace SessionTracker.RelativePositionWindow
{
    public class RelativePositionAndMouseDraggableContainer : Container
    {
        public RelativePositionAndMouseDraggableContainer(SettingService settingService)
        {
            _settingService = settingService;
            SetLocationFromSettings();
            GameService.Input.Mouse.LeftMouseButtonReleased   += OnLeftMouseButtonReleased;
            GameService.Graphics.SpriteScreen.Resized         += OnSpriteScreenResized;
            settingService.WindowAnchorSetting.SettingChanged += WindowAnchorSettingChanged;
        }

        protected override void DisposeControl()
        {
            GameService.Input.Mouse.LeftMouseButtonReleased    -= OnLeftMouseButtonReleased;
            GameService.Graphics.SpriteScreen.Resized          -= OnSpriteScreenResized;
            _settingService.WindowAnchorSetting.SettingChanged -= WindowAnchorSettingChanged;
            base.DisposeControl();
        }

        public override void RecalculateLayout()
        {
            base.RecalculateLayout(); // not required because Control and Container dot not implement them. But maybe in a future blish version they do.

            var windowAnchorLocation 
                = ConvertCoordinatesService.ConvertRelativeToAbsoluteCoordinates(_settingService.WindowRelativeLocationSetting.Value, GameService.Graphics.SpriteScreen.Size);

            var location = ConvertBetweenControlAndWindowAnchorLocation(windowAnchorLocation, ConvertLocation.ToControlLocation);
            var adjustedLocation = ScreenBoundariesService.AdjustLocationToKeepContainerInsideScreenBoundaries(location, Size, GameService.Graphics.SpriteScreen.Size);
            SaveLocationInSettings(adjustedLocation);
            Location = adjustedLocation;
        }

        public override void UpdateContainer(GameTime gameTime)
        {
            if (_settingService.DragWindowWithMouseIsEnabledSetting.Value && _containerIsDraggedByMouse)
            {
                var newLocation = Input.Mouse.Position - _mousePressedLocationInsideContainer;
                var adjustedLocation = ScreenBoundariesService.AdjustLocationToKeepContainerInsideScreenBoundaries(newLocation, Size, GameService.Graphics.SpriteScreen.Size);
                SaveLocationInSettings(adjustedLocation);
                Location = adjustedLocation;
            }
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
                SaveLocationInSettings(Location);
            }
        }

        private void WindowAnchorSettingChanged(object sender, ValueChangedEventArgs<WindowAnchor> e)
        {
            SaveLocationInSettings(Location);
        }

        private void OnSpriteScreenResized(object sender, ResizedEventArgs resizedEventArgs)
        {
            SetLocationFromSettings();
        }

        // do not use AdjustCoordinates here, because it is called by OnSpriteScreenResized -> can cause unwanted adjusting because of multiple SpriteScreen resizing on module start up
        private void SetLocationFromSettings()
        {
            var windowAnchorLocation 
                = ConvertCoordinatesService.ConvertRelativeToAbsoluteCoordinates(_settingService.WindowRelativeLocationSetting.Value, GameService.Graphics.SpriteScreen.Size);
            
            Location = ConvertBetweenControlAndWindowAnchorLocation(windowAnchorLocation, ConvertLocation.ToControlLocation);
        }

        private void SaveLocationInSettings(Point location)
        {
            var windowAnchorLocation = ConvertBetweenControlAndWindowAnchorLocation(location, ConvertLocation.ToWindowAnchorLocation);
            _settingService.WindowRelativeLocationSetting.Value = ConvertCoordinatesService.ConvertAbsoluteToRelativeCoordinates(windowAnchorLocation, GameService.Graphics.SpriteScreen.Size);
        }

        private Point ConvertBetweenControlAndWindowAnchorLocation(Point location, ConvertLocation convertLocation)
        {
            var xIsControlLocation = _settingService.WindowAnchorSetting.Value == WindowAnchor.TopLeft || _settingService.WindowAnchorSetting.Value == WindowAnchor.BottomLeft;
            var yIsControlLocation = _settingService.WindowAnchorSetting.Value == WindowAnchor.TopLeft || _settingService.WindowAnchorSetting.Value == WindowAnchor.TopRight;
            var x = ConvertCoordinate(location.X, xIsControlLocation, Width, convertLocation);
            var y = ConvertCoordinate(location.Y, yIsControlLocation, Height, convertLocation);
            return new Point(x, y);
        }

        private int ConvertCoordinate(int coordinate, bool isControlLocation, int widthOrHeight, ConvertLocation convertLocation)
        {
            if(isControlLocation)
                return coordinate;

            return convertLocation == ConvertLocation.ToWindowAnchorLocation
                ? coordinate + widthOrHeight
                : coordinate - widthOrHeight;
        }

        private Point _mousePressedLocationInsideContainer = Point.Zero;
        private bool _containerIsDraggedByMouse;
        private readonly SettingService _settingService;
    }
}