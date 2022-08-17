using System.Globalization;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using SessionTracker.Services;
using SessionTracker.Settings;

namespace SessionTracker.UnitTest
{
    public class ValueTextService_Test
    {
        // Xc
        [TestCase(CoinDisplayFormat.Xc, -1234567890, "-1234567890c")] 
        [TestCase(CoinDisplayFormat.Xc, -1000000000, "-1000000000c")] 
        [TestCase(CoinDisplayFormat.Xc, -123456, "-123456c")] 
        [TestCase(CoinDisplayFormat.Xc, -10, "-10c")] 
        [TestCase(CoinDisplayFormat.Xc, -1, "-1c")] 
        [TestCase(CoinDisplayFormat.Xc, 0, "0c")] 
        [TestCase(CoinDisplayFormat.Xc, 1, "1c")] 
        [TestCase(CoinDisplayFormat.Xc, 10, "10c")] 
        [TestCase(CoinDisplayFormat.Xc, 100, "100c")]
        [TestCase(CoinDisplayFormat.Xc, 1000, "1000c")]
        [TestCase(CoinDisplayFormat.Xc, 10000, "10000c")]
        [TestCase(CoinDisplayFormat.Xc, 123456, "123456c")] 
        [TestCase(CoinDisplayFormat.Xc, 1234567890, "1234567890c")] 
        // Xg
        [TestCase(CoinDisplayFormat.Xg, -1234567890, "-123456g")] 
        [TestCase(CoinDisplayFormat.Xg, -1000000000, "-100000g")] 
        [TestCase(CoinDisplayFormat.Xg, -123456, "-12g")] 
        [TestCase(CoinDisplayFormat.Xg, -10, "0g")] 
        [TestCase(CoinDisplayFormat.Xg, -1, "0g")] 
        [TestCase(CoinDisplayFormat.Xg, 0, "0g")] 
        [TestCase(CoinDisplayFormat.Xg, 1, "0g")] 
        [TestCase(CoinDisplayFormat.Xg, 10, "0g")] 
        [TestCase(CoinDisplayFormat.Xg, 100, "0g")]
        [TestCase(CoinDisplayFormat.Xg, 1000, "0g")]
        [TestCase(CoinDisplayFormat.Xg, 10000, "1g")]
        [TestCase(CoinDisplayFormat.Xg, 123456, "12g")] 
        [TestCase(CoinDisplayFormat.Xg, 1234567890, "123456g")] 
        // XgX
        [TestCase(CoinDisplayFormat.XgX, -1234567890, "-123456g7")] 
        [TestCase(CoinDisplayFormat.XgX, -1000000000, "-100000g0")] 
        [TestCase(CoinDisplayFormat.XgX, -123456, "-12g3")] 
        [TestCase(CoinDisplayFormat.XgX, -10, "0g")] 
        [TestCase(CoinDisplayFormat.XgX, -1, "0g")] 
        [TestCase(CoinDisplayFormat.XgX, 0, "0g")] 
        [TestCase(CoinDisplayFormat.XgX, 1, "0g")] 
        [TestCase(CoinDisplayFormat.XgX, 10, "0g")] 
        [TestCase(CoinDisplayFormat.XgX, 100, "0g")]
        [TestCase(CoinDisplayFormat.XgX, 1000, "0g1")]
        [TestCase(CoinDisplayFormat.XgX, 10000, "1g0")]
        [TestCase(CoinDisplayFormat.XgX, 123456, "12g3")] 
        [TestCase(CoinDisplayFormat.XgX, 1234567890, "123456g7")] 
        // XgXX
        [TestCase(CoinDisplayFormat.XgXX, -1234567890, "-123456g78")] 
        [TestCase(CoinDisplayFormat.XgXX, -1000000000, "-100000g00")] 
        [TestCase(CoinDisplayFormat.XgXX, -123456, "-12g34")] 
        [TestCase(CoinDisplayFormat.XgXX, -10, "0g")] 
        [TestCase(CoinDisplayFormat.XgXX, -1, "0g")] 
        [TestCase(CoinDisplayFormat.XgXX, 0, "0g")] 
        [TestCase(CoinDisplayFormat.XgXX, 1, "0g")] 
        [TestCase(CoinDisplayFormat.XgXX, 10, "0g")] 
        [TestCase(CoinDisplayFormat.XgXX, 100, "0g01")]
        [TestCase(CoinDisplayFormat.XgXX, 1000, "0g10")]
        [TestCase(CoinDisplayFormat.XgXX, 10000, "1g00")]
        [TestCase(CoinDisplayFormat.XgXX, 123456, "12g34")] 
        [TestCase(CoinDisplayFormat.XgXX, 1234567890, "123456g78")] 
        // XgXXsXXc
        [TestCase(CoinDisplayFormat.XgXXsXXc, -1234567890, "-123456g\u200978s\u200990c")]
        [TestCase(CoinDisplayFormat.XgXXsXXc, -123456, "-12g\u200934s\u200956c")]
        [TestCase(CoinDisplayFormat.XgXXsXXc, -10, "-0g\u20090s\u200910c")]
        [TestCase(CoinDisplayFormat.XgXXsXXc, -1, "-0g\u20090s\u20091c")]
        [TestCase(CoinDisplayFormat.XgXXsXXc, 0, "0g\u20090s\u20090c")]
        [TestCase(CoinDisplayFormat.XgXXsXXc, 1, "0g\u20090s\u20091c")]
        [TestCase(CoinDisplayFormat.XgXXsXXc, 10, "0g\u20090s\u200910c")]
        [TestCase(CoinDisplayFormat.XgXXsXXc, 100, "0g\u20091s\u20090c")]
        [TestCase(CoinDisplayFormat.XgXXsXXc, 1000, "0g\u200910s\u20090c")]
        [TestCase(CoinDisplayFormat.XgXXsXXc, 10000, "1g\u20090s\u20090c")]
        [TestCase(CoinDisplayFormat.XgXXsXXc, 123456, "12g\u200934s\u200956c")]
        [TestCase(CoinDisplayFormat.XgXXsXXc, 1234567890, "123456g\u200978s\u200990c")]
        public void Create_coin_text_depending_on_displayFormat(CoinDisplayFormat coinDisplayFormat, int valueInCopper, string expectedText)
        {
            Remove1000SeparatorForThisTest();

            var result = ValueTextService.CreateCoinValueText(valueInCopper, coinDisplayFormat);
            
            result.Should().Be(expectedText);
        }

        private static void Remove1000SeparatorForThisTest()
        {
            var cultureInfo = new CultureInfo("en-US", false);
            cultureInfo.NumberFormat.NumberGroupSeparator = "";
            Thread.CurrentThread.CurrentUICulture         = cultureInfo;
        }
    }
}