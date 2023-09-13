using Blish_HUD.Controls;
using Blish_HUD;
using Microsoft.Xna.Framework;

namespace SessionTracker.Services.RemoteFiles
{
    public class DeprecatedModuleVersionWindow : StandardWindow
    {
        public DeprecatedModuleVersionWindow(string moduleName, string deprecatedText)
            : base(ContentService.Textures.TransparentPixel, new Rectangle(10, 30, WINDOW_WIDTH, 200), new Rectangle(30, 30, WINDOW_WIDTH, 200))
        {
            Title = $"{moduleName}: Update module version";
            BackgroundColor = new Color(Color.Black, 0.8f);
            Location = new Point(300, 300);
            Id = "ecksofa deprecated window";
            Parent = GameService.Graphics.SpriteScreen;

            new Label()
            {
                Text = deprecatedText,
                Font = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size20, ContentService.FontStyle.Regular),
                ShowShadow = true,
                AutoSizeHeight = true,
                Width = WINDOW_WIDTH - 100,
                WrapText = true,
                Parent = this
            };

            Show();
        }

        private const int WINDOW_WIDTH = 710;
    }
}