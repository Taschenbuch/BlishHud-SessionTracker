using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SessionTracker.Settings
{
    public static class ColorTypeExtensions
    {
        public static Color GetColor(this ColorType colorType)
        {
            return ColorByColorType[colorType];
        }

        public static Dictionary<ColorType, Color> ColorByColorType = new Dictionary<ColorType, Color>
        {
            [ColorType.White]                = Color.White,
            [ColorType.LightGray]            = Color.LightGray,
            [ColorType.Gray]                 = Color.Gray, // Gray is darker than DarkGray... very funny
            [ColorType.DarkGray]             = Color.DarkGray,
            [ColorType.LightGreen]           = Color.LightGreen,
            [ColorType.Green]                = Color.Green,
            [ColorType.DarkGreen]            = Color.DarkGreen,
            [ColorType.Red]                  = Color.Red,
            [ColorType.DarkRed]              = Color.DarkRed,
            [ColorType.LightBlue]            = Color.LightBlue,
            [ColorType.Blue]                 = Color.Blue,
            [ColorType.DarkBlue]             = Color.DarkBlue,
            [ColorType.Beige]                = Color.Beige,
            [ColorType.Orange]               = Color.Orange,
            [ColorType.LightYellow]          = Color.LightYellow,
            [ColorType.Yellow]               = Color.Yellow,
            [ColorType.LightGoldenrodYellow] = Color.LightGoldenrodYellow,
        };
    }
}