using System;
using System.Collections.Generic;
using System.Linq;
using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Modules.Managers;
using Gw2Sharp.WebApi;
using Microsoft.Xna.Framework.Graphics;
using Model = SessionTracker.Models.Model;

namespace SessionTracker.Services
{
    public class TextureService : IDisposable
    {
        public TextureService(Model model, ContentsManager contentsManager, Logger logger)
        {
            _contentsManager = contentsManager;
            _logger          = logger;

            DebugTabTexture             = contentsManager.GetTexture(@"settingsWindow\debugTab_440025.png");
            VisibilityTabTexture        = contentsManager.GetTexture(@"settingsWindow\visibilityTab.png");
            StatsTabTexture             = contentsManager.GetTexture(@"settingsWindow\statsTab_156909.png");
            GeneralTabTexture           = contentsManager.GetTexture(@"settingsWindow\generalTab_156736.png");
            SettingsWindowEmblemTexture = contentsManager.GetTexture(@"settingsWindow\settingsWindowEmblem.png");
            WindowBackgroundTexture     = contentsManager.GetTexture(@"settingsWindow\windowBackground_155985.png");
            MoveDownTexture             = contentsManager.GetTexture(@"settingsWindow\moveDown_155953.png");
            MoveDownActiveTexture       = contentsManager.GetTexture(@"settingsWindow\moveDownActive_155953.png");
            MoveUpTexture               = contentsManager.GetTexture(@"settingsWindow\moveUp_155953.png");
            MoveUpActiveTexture         = contentsManager.GetTexture(@"settingsWindow\moveUpActive_155953.png");
            EntryIconPlaceholderTexture = contentsManager.GetTexture(@"stats\entryIconPlaceholder_1444524.png");
            CornerIconTexture           = contentsManager.GetTexture(@"cornerIcon.png");
            CornerIconHoverTexture      = contentsManager.GetTexture(@"cornerIconHover.png");
            HiddenStatsTexture          = contentsManager.GetTexture(@"hiddenStats_605021.png");
            CreateEntryTextures(model);
        }

        public void Dispose()
        {
            DebugTabTexture?.Dispose();
            VisibilityTabTexture?.Dispose();
            GeneralTabTexture?.Dispose();
            StatsTabTexture?.Dispose();
            SettingsWindowEmblemTexture?.Dispose();
            WindowBackgroundTexture?.Dispose();
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

        public Texture2D DebugTabTexture { get; }
        public Texture2D HiddenStatsTexture { get; }
        public Texture2D SettingsWindowEmblemTexture { get; }
        public Texture2D StatsTabTexture { get; }
        public Texture2D GeneralTabTexture { get; }
        public Texture2D VisibilityTabTexture { get; }
        public Texture2D WindowBackgroundTexture { get; }
        public Texture2D MoveDownTexture { get; }
        public Texture2D MoveDownActiveTexture { get; }
        public Texture2D MoveUpTexture { get; }
        public Texture2D MoveUpActiveTexture { get; }
        public Texture2D EntryIconPlaceholderTexture { get; }
        public Texture2D CornerIconTexture { get; }
        public Texture2D CornerIconHoverTexture { get; }
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
                        EntryTextureByEntryId[entry.Id] = _contentsManager.GetTexture($@"stats\{entry.IconFileName}");
                    else
                    {
                        _logger.Error($"Error: Icon texture missing for entryId: {entry.Id}. Use placeholder icon as fallback.");
                        EntryTextureByEntryId[entry.Id] = EntryIconPlaceholderTexture;
                    }
                }
                catch (Exception e)
                {
                    EntryTextureByEntryId[entry.Id] = new AsyncTexture2D(EntryIconPlaceholderTexture);
                    notFoundTextures.Add(entry.LabelText.English);
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