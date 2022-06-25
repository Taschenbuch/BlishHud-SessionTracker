using Microsoft.Xna.Framework;

namespace SessionTracker.Services
{
    public class ScreenBoundariesService
    {
        /// <summary>
        /// WARNING: this will only work properly for containers that are smaller than the SpriteScreen in X and Y dimension.
        /// </summary>
        public static Point AdjustCoordinatesToKeepContainerInsideScreenBoundaries(Point coordinates,
                                                                                   int containerWidth,
                                                                                   int containerHeight, 
                                                                                   int screenWidth, 
                                                                                   int screenHeight)
        {
            var x = AdjustCoordinateToKeepContainerInsideScreenBoundaries(
                coordinates.X,
                containerWidth,
                screenWidth);

            var y = AdjustCoordinateToKeepContainerInsideScreenBoundaries(
                coordinates.Y,
                containerHeight,
                screenHeight);

            return new Point(x, y);
        }

        private static int AdjustCoordinateToKeepContainerInsideScreenBoundaries(int coordinate, int containerWidthOrHeight, int screenWidthOrHeight)
        {
            var containerIsOutsideOfLeftOrTopScreenBoundary = coordinate < 0;
            if (containerIsOutsideOfLeftOrTopScreenBoundary)
                return 0;

            var containerIsOutsideOfRightOrBottomScreenBoundary = containerWidthOrHeight + coordinate > screenWidthOrHeight;
            if (containerIsOutsideOfRightOrBottomScreenBoundary)
                return screenWidthOrHeight - containerWidthOrHeight;

            return coordinate;
        }
    }
}