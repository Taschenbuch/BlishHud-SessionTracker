using Blish_HUD.Graphics.UI;
using System;

namespace SessionTracker.Services.RemoteFiles
{
    public class ModuleLoadError : IDisposable
    {
        public bool HasModuleLoadFailed { get; set; }
        public string InfoText { get; set; } = string.Empty;
        public string HyperLink { get; set; } = string.Empty;
        public string HyperLinkDisplayText { get; set; } = string.Empty;

        public void Dispose()
        {
            _errorWindow?.Dispose();
        }

        public void InitForFailedDownload(string moduleName)
        {
            InfoText = $"Failed to download required files. {moduleName} module cannot start. :-(\n" +
                       $"Retry later by restarting blish or by disable and then enable {moduleName} module again.\n\n" +
                       $"Possible reasons:\n" +
                       $"- You are offline or your internet connection has problems connecting to blish web server.\n" +
                       $"- the blish web server is offline or certain files on the server are not accessable.\n";
            HyperLinkDisplayText = "!! Click here to test in your web browser if you can access the blish hud server !!";
            HyperLink = @"https://bhm.blishhud.com/ecksofa.sessiontracker/online.html";
        }

        public void ShowErrorWindow(string windowTitle)
        {
            _errorWindow = new ErrorWindow(windowTitle, InfoText, HyperLink, HyperLinkDisplayText);
            _errorWindow.Show();
        }

        public IView CreateErrorSettingsView()
        {
            return new ErrorSettingsView(InfoText, HyperLink, HyperLinkDisplayText);
        }

        private ErrorWindow _errorWindow;
    }
}
