using Microsoft.Xna.Framework;

namespace SessionTracker.StatsWindow
{
    public class ConvertCoordinatesService
    {
        public static Point ConvertRelativeToAbsoluteCoordinates(float xRelative, float yRelative, int screenWidth, int screenHeight)
        {
            var xAbsolute = (int)(screenWidth * xRelative);
            var yAbsolute = (int)(screenHeight * yRelative);

            return new Point(xAbsolute, yAbsolute);
        }

        public static (float xRelative, float yRelative) ConvertAbsoluteToRelativeCoordinates(int xAbsolute,
                                                                                              int yAbsolute,
                                                                                              int screenWidth,
                                                                                              int screenHeight)
        {
            var xRelative = ConvertAbsoluteToRelativeCoordinate(xAbsolute, screenWidth);
            var yRelative = ConvertAbsoluteToRelativeCoordinate(yAbsolute, screenHeight);

            return (xRelative, yRelative);
        }

        private static float ConvertAbsoluteToRelativeCoordinate(int absolute, int screenWidthOrHeight)
        {
            var screenHasMessedUpDimensions = screenWidthOrHeight == 0;
            if (screenHasMessedUpDimensions)
                return 0;

            var absoluteIsOutSideOfLeftOrTopScreenBoundary = absolute < 0;
            if (absoluteIsOutSideOfLeftOrTopScreenBoundary)
                return 0;

            var absoluteIsOutsideOfRightOrBottomScreenBoundary = absolute > screenWidthOrHeight;
            if (absoluteIsOutsideOfRightOrBottomScreenBoundary)
                return screenWidthOrHeight;

            return (float)absolute / screenWidthOrHeight;
        }
    }
}