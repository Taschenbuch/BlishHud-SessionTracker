using Blish_HUD.Controls;
using Microsoft.Xna.Framework;

namespace SessionTracker.Controls
{
    public class HintLabel : Label
    {
        public HintLabel(string text, Container parent)
        {
            Text             = text;
            BasicTooltipText = text;
            ShowShadow       = true;
            AutoSizeHeight   = true;
            AutoSizeWidth    = true;
            Parent           = parent;

            _parent = parent;
        }

        public void SetVisibility(bool isVisible)
        {
            if (isVisible)
                Show();
            else
                Hide();
        }

        public override void Show()
        {
            Parent = _parent;
            base.Show();
        }

        public override void Hide()
        {
            Parent = null;
            base.Hide();
        }

        private readonly Container _parent;
    }
}