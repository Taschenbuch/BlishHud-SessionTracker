using Blish_HUD.Controls;
using SessionTracker.StatsWindow;
using System.Threading.Tasks;

namespace SessionTracker.Controls
{
    public class MaxHeightFlowPanel: FlowPanel
    {
        public MaxHeightFlowPanel(StatsRootFlowPanel statsRootFlowPanel)
        {
            _statsRootFlowPanel = statsRootFlowPanel;
        }

        protected override void OnChildRemoved(ChildChangedEventArgs e)
        {
            base.OnChildRemoved(e);
            _statsRootFlowPanel.UpdateHeight(Height);
            //Task.Run(async () =>
            //{
            //    await Task.Delay(500);
            //    _statsRootFlowPanel.UpdateHeight(Height);
            //});
        }

        protected override void OnChildAdded(ChildChangedEventArgs e)
        {
            base.OnChildAdded(e);
            _statsRootFlowPanel.UpdateHeight(Height);
            //Task.Run(async () =>
            //{
            //    await Task.Delay(500);
            //    _statsRootFlowPanel.UpdateHeight(Height);
            //});
        }

        private readonly StatsRootFlowPanel _statsRootFlowPanel;
    }
}
