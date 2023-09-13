using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Microsoft.Xna.Framework;

namespace SessionTracker.Services.RemoteFiles
{
    public class DeprecatedModuleVersionSettingsView : View
    {
        public DeprecatedModuleVersionSettingsView(string deprecatedText)
        {
            _deprecatedText = deprecatedText;
        }

        protected override void Build(Container buildPanel)
        {
            _deprecatedLabel = new Label()
            {
                Text = _deprecatedText,
                Font = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size20, ContentService.FontStyle.Regular),
                ShowShadow = true,
                AutoSizeHeight = true,
                Width = 500,
                WrapText = true,
                Parent = buildPanel,
            };
        }

        protected override void Unload()
        {
            _deprecatedLabel?.Dispose();
        }

        private Label _deprecatedLabel;
        private readonly string _deprecatedText;
    }
}