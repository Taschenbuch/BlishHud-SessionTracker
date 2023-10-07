using Microsoft.Xna.Framework;

namespace SessionTracker.RelativePositionWindow
{
    public class ScreenBoundariesService
    {
        public static Point AdjustCoordinatesToKeepContainerInsideScreenBoundaries(Point coordinates, Point containerSize, Point screenSize)
        {
            var x = AdjustCoordinateToKeepContainerInsideScreenBoundaries(
                coordinates.X,
                containerSize.X,
                screenSize.X);

            var y = AdjustCoordinateToKeepContainerInsideScreenBoundaries(
                coordinates.Y,
                containerSize.Y,
                screenSize.Y);

            return new Point(x, y);
        }

        private static int AdjustCoordinateToKeepContainerInsideScreenBoundaries(int coordinate, int containerWidthOrHeight, int screenWidthOrHeight)
        {
            var containerIsTooBigForScreenDimension = containerWidthOrHeight >= screenWidthOrHeight;
            if (containerIsTooBigForScreenDimension)
                return 0;

            var containerIsOutsideOfLeftOrTopScreenBoundary = coordinate <= 0;
            if (containerIsOutsideOfLeftOrTopScreenBoundary)
                return 0;

            var containerIsOutsideOfRightOrBottomScreenBoundary = containerWidthOrHeight + coordinate > screenWidthOrHeight;
            if (containerIsOutsideOfRightOrBottomScreenBoundary)
                return screenWidthOrHeight - containerWidthOrHeight;

            return coordinate;
        }
    }
}