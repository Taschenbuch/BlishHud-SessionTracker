using Blish_HUD.Controls;
using Blish_HUD;
using Microsoft.Xna.Framework;

namespace SessionTracker.Files.RemoteFiles
{
    public class ErrorWindow : StandardWindow
    {
        public ErrorWindow(string windowTitle, string windowText, string hyperLink, string hyperLinkDisplayText)
            : base(ContentService.Textures.TransparentPixel, new Rectangle(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT), new Rectangle(30, 30, WINDOW_WIDTH - 30, WINDOW_HEIGHT - 30))
        {
            Title = windowTitle;
            BackgroundColor = new Color(Color.Black, 0.8f);
            Location = new Point(300, 300);
            Id = "ecksofa error window";
            Parent = GameService.Graphics.SpriteScreen;

            var formattedLabel = new FormattedLabelBuilder()
                .SetWidth(WINDOW_WIDTH - 100)
                .AutoSizeHeight()
                .Wrap()
                .CreatePart(windowText, builder => builder.SetFontSize(FONT_SIZE))
                .CreatePart(hyperLinkDisplayText, builder => builder.SetHyperLink(hyperLink).SetFontSize(FONT_SIZE))
                .Build();

            formattedLabel.Parent = this;
            Height = formattedLabel.Height + 100;
        }

        private const int WINDOW_WIDTH = 910;
        private const int WINDOW_HEIGHT = 300;
        private const ContentService.FontSize FONT_SIZE = ContentService.FontSize.Size20;
    }
}