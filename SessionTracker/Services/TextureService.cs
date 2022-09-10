using System;
using System.Collections.Generic;
using System.Linq;
using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Modules.Managers;
using Model = SessionTracker.Models.Model;

namespace SessionTracker.Services
{
    public class TextureService : IDisposable
    {
        public TextureService(Model model, ContentsManager contentsManager, Logger logger)
        {
            _contentsManager = contentsManager;
            _logger          = logger;

            GameService.Graphics.QueueMainThreadRender((graphicsDevice) =>
            {
                // hack for blish 0.11.7 because only main thread should use graphicsDevice and thus .GetTexture() has to be called on main thread.
                DebugTabTexture.SwapTexture(contentsManager.GetTexture(@"settingsWindow\debugTab_440025.png"));
                VisibilityTabTexture.SwapTexture(contentsManager.GetTexture(@"settingsWindow\visibilityTab.png"));
                StatsTabTexture.SwapTexture(contentsManager.GetTexture(@"settingsWindow\statsTab_156909.png"));
                GeneralTabTexture.SwapTexture(contentsManager.GetTexture(@"settingsWindow\generalTab_156736.png"));
                SettingsWindowBackgroundTexture.SwapTexture(contentsManager.GetTexture(@"settingsWindow\windowBackground_155985.png"));
                SettingsWindowEmblemTexture.SwapTexture(contentsManager.GetTexture(@"settingsWindow\settingsWindowEmblem.png")); // hack MUST BE below windowBackground! hack for blish 0.11.7 tabbedWindow2 not handling asyncTexture swapping
                MoveDownTexture.SwapTexture(contentsManager.GetTexture(@"settingsWindow\moveDown_155953.png"));
                MoveDownActiveTexture.SwapTexture(contentsManager.GetTexture(@"settingsWindow\moveDownActive_155953.png"));
                MoveUpTexture.SwapTexture(contentsManager.GetTexture(@"settingsWindow\moveUp_155953.png"));
                MoveUpActiveTexture.SwapTexture(contentsManager.GetTexture(@"settingsWindow\moveUpActive_155953.png"));
                EntryIconPlaceholderTexture.SwapTexture(contentsManager.GetTexture(@"stats\entryIconPlaceholder_1444524.png"));
                CornerIconTexture.SwapTexture(contentsManager.GetTexture(@"cornerIcon.png"));
                CornerIconHoverTexture.SwapTexture(contentsManager.GetTexture(@"cornerIconHover.png"));
                HiddenStatsTexture.SwapTexture(contentsManager.GetTexture(@"hiddenStats.png"));
            });

            CreateEntryTextures(model);
        }

        public void Dispose()
        {
            DebugTabTexture?.Dispose();
            VisibilityTabTexture?.Dispose();
            GeneralTabTexture?.Dispose();
            StatsTabTexture?.Dispose();
            SettingsWindowEmblemTexture?.Dispose();
            SettingsWindowBackgroundTexture?.Dispose();
            MoveDownTexture?.Dispose();
            MoveDownActiveTexture?.Dispose();
            MoveUpTexture?.Dispose();
            MoveUpActiveTexture?.Dispose();
            EntryIconPlaceholderTexture?.Dispose();
            CornerIconTexture?.Dispose();
            CornerIconHoverTexture?.Dispose();
            HiddenStatsTexture?.Dispose();

            foreach (var entryIcon in EntryTextureByEntryId.Values)
                entryIcon?.Dispose();
        }

        public AsyncTexture2D DebugTabTexture { get; } = new AsyncTexture2D();
        public AsyncTexture2D HiddenStatsTexture { get; } = new AsyncTexture2D(ContentService.Textures.Error);
        public AsyncTexture2D SettingsWindowEmblemTexture { get; } = new AsyncTexture2D(ContentService.Textures.Error);
        public AsyncTexture2D StatsTabTexture { get; } = new AsyncTexture2D(ContentService.Textures.Error);
        public AsyncTexture2D GeneralTabTexture { get; } = new AsyncTexture2D(ContentService.Textures.Error);
        public AsyncTexture2D VisibilityTabTexture { get; } = new AsyncTexture2D(ContentService.Textures.Error);
        public AsyncTexture2D SettingsWindowBackgroundTexture { get; } = new AsyncTexture2D(ContentService.Textures.Error);
        public AsyncTexture2D MoveDownTexture { get; } = new AsyncTexture2D(ContentService.Textures.Error);
        public AsyncTexture2D MoveDownActiveTexture { get; } = new AsyncTexture2D(ContentService.Textures.Error);
        public AsyncTexture2D MoveUpTexture { get; } = new AsyncTexture2D(ContentService.Textures.Error);
        public AsyncTexture2D MoveUpActiveTexture { get; } = new AsyncTexture2D(ContentService.Textures.Error);
        public AsyncTexture2D EntryIconPlaceholderTexture { get; } = new AsyncTexture2D(ContentService.Textures.Error);
        public AsyncTexture2D CornerIconTexture { get; } = new AsyncTexture2D(ContentService.Textures.Error);
        public AsyncTexture2D CornerIconHoverTexture { get; } = new AsyncTexture2D(ContentService.Textures.Error);
        public Dictionary<string, AsyncTexture2D> EntryTextureByEntryId { get; } = new Dictionary<string, AsyncTexture2D>();

        private void CreateEntryTextures(Model model)
        {
            var notFoundTextures = new List<string>();
            var exception        = new Exception("i am a dummy. ignore me");

            foreach (var entry in model.Entries)
            {
                try
                {
                    if (entry.HasIconUrl)
                        EntryTextureByEntryId[entry.Id] = GameService.Content.GetRenderServiceTexture(entry.IconUrl);
                    else if (entry.HasIconFile)
                    {
                        EntryTextureByEntryId[entry.Id] = new AsyncTexture2D(ContentService.Textures.Error);

                        GameService.Graphics.QueueMainThreadRender((graphicsDevice) =>
                        {
                            // workaround for blish 0.11.7 because only main thread should use graphicsDevice
                            EntryTextureByEntryId[entry.Id].SwapTexture(_contentsManager.GetTexture($@"stats\{entry.IconFileName}"));
                        });
                    }
                    else
                    {
                        _logger.Error($"Error: Icon texture missing for entryId: {entry.Id}. Use placeholder icon as fallback.");
                        EntryTextureByEntryId[entry.Id] = EntryIconPlaceholderTexture;
                    }
                }
                catch (Exception e)
                {
                    EntryTextureByEntryId[entry.Id] = new AsyncTexture2D(EntryIconPlaceholderTexture);
                    notFoundTextures.Add(entry.Name.English);
                    exception = e;
                }
            }

            if (notFoundTextures.Any())
                _logger.Error(exception, $"Could not get entry texture for: {string.Join(", ", notFoundTextures)}. :(");
        }

        private readonly ContentsManager _contentsManager;
        private readonly Logger _logger;
    }
}