using FluentAssertions;
using NUnit.Framework;
using SessionTracker.StatsWindow.RelativePositionWindow;

namespace SessionTracker.UnitTest
{
    public class ConvertLocationService_Test
    {
        // some test cases overlap each other because multiple conditions apply at the same time
        // negative absolute x and/or y position -> container outside of screen or is partially cutoff
        [TestCase(-1, 0, 1, 2, 0, 0)]
        [TestCase(0, -1, 1, 2, 0, 0)]
        [TestCase(-1, -1, 1, 2, 0, 0)]
        [TestCase(-2, 0, 1, 2, 0, 0)]
        [TestCase(0, -2, 1, 2, 0, 0)]
        [TestCase(-2, -2, 1, 2, 0, 0)]
        // absolute bigger than screen -> container outside of screen (partially cutoff would be still in bounds for this case)
        [TestCase(1, 2, 0, 0, 0, 0)]
        [TestCase(20, 30, 0, 0, 0, 0)]
        [TestCase(2, 3, 1, 2, 1, 2)]
        [TestCase(20, 30, 1, 2, 1, 2)]
        // 0 as screen width and/or height -> relative has to be 0 for that dimension too
        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(0, 0, 0, 1, 0, 0)]
        [TestCase(0, 0, 1, 0, 0, 0)]
        [TestCase(5, 10, 0, 0, 0, 0)]
        [TestCase(5, 10, 0, 10, 0, 1)]
        [TestCase(5, 10, 5, 0, 1, 0)]
        // absolute equal to screenWidth or height -> container exactly on screen border
        [TestCase(0, 0, 10, 30, 0, 0)]
        [TestCase(5, 30, 10, 30, 0.5f, 1)]
        [TestCase(10, 30, 10, 30, 1, 1)]
        // absolute position is within screen borders (the happy path)
        [TestCase(0, 0, 10, 20, 0, 0)]
        [TestCase(1, 1, 10, 20, 0.1f, 0.05f)]
        [TestCase(453, 805, 1440, 1080, 0.3146f, 0.7453f)] // HD resolution example
        public void GetRelativeCoordinates(int xAbsolute, int yAbsolute,
                                           int screenWidth, int screenHeight,
                                           float expectedXRelative, float expectedYRelative)
        {
            var (xRelative, yRelative) = ConvertCoordinatesService.ConvertAbsoluteToRelativeCoordinates(xAbsolute, yAbsolute, screenWidth, screenHeight);

            xRelative.Should().BeApproximately(expectedXRelative, 0.0001f);
            yRelative.Should().BeApproximately(expectedYRelative, 0.0001f);
        }
    }
}