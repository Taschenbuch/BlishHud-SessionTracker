using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;

namespace SessionTracker.Services.RemoteFiles
{
    public class ErrorSettingsView : View
    {
        public ErrorSettingsView(string infoText, string hyperLink, string hyperLinkDisplayText)
        {
            _infoText = infoText;
            _hyperLink = hyperLink;
            _hyperLinkDisplayText = hyperLinkDisplayText;
        }

        protected override void Build(Container buildPanel)
        {
            _formattedLabel = new FormattedLabelBuilder()
                .SetWidth(buildPanel.Width)
                .AutoSizeHeight()
                .Wrap()
                .CreatePart(_infoText, builder => builder.SetFontSize(FONT_SIZE))
                .CreatePart(_hyperLinkDisplayText, builder => builder.SetHyperLink(_hyperLink).SetFontSize(FONT_SIZE))
                .Build();

            _formattedLabel.Parent = buildPanel;
        }

        protected override void Unload()
        {
            _formattedLabel?.Dispose();
        }

        private FormattedLabel _formattedLabel;
        private const ContentService.FontSize FONT_SIZE = ContentService.FontSize.Size16;
        private readonly string _infoText;
        private readonly string _hyperLink;
        private readonly string _hyperLinkDisplayText;
    }
}