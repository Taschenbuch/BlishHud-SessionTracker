using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace SessionTracker.SelectStats
{
    public class SelectedCountLabel : Label
    {
        public SelectedCountLabel(List<SelectStatViewModel> subStatViewModels, Container parent, int rightShift)
        {
            AutoSizeHeight = true;
            Width = 150;
            HorizontalAlignment = HorizontalAlignment.Right;
            Location = new Point(485 + rightShift, 10);
            Parent = parent;

            var totalCount = subStatViewModels.Count;
            var selectedCount = subStatViewModels.Where(v => v.IsSelected).Count();
            SetTextAndOpacity(totalCount, selectedCount);

            foreach (var subStatViewModel in subStatViewModels)
            {
                subStatViewModel.IsSelectedChanged += (s, e) =>
                {
                    selectedCount = subStatViewModel.IsSelected
                        ? selectedCount + 1
                        : selectedCount - 1;

                    SetTextAndOpacity(totalCount, selectedCount);
                };
            }
        }

        protected override CaptureType CapturesInput()
        {
            return CaptureType.None; // allows collapsing the category panel when clicking on this label
        }

        private void SetTextAndOpacity(int totalCount, int selectedCount)
        {
            Text = CreateSelectedCountText(totalCount, selectedCount);
            Opacity = selectedCount == 0 ? 0.3f : 1f;
        }

        private static string CreateSelectedCountText(int totalCount, int selectedCount)
        {
            if (selectedCount == 0)
                return "none selected";

            if (selectedCount == totalCount)
                return "all selected";

            return $"{selectedCount} selected";
        }
    }
}
