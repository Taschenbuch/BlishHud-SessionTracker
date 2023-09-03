using System;
using System.Collections.Generic;
using System.Linq;
using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Modules.Managers;
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
            _model           = model;

            DebugTabTexture = contentsManager.GetTexture(@"settingsWindow\debugTab_440025.png");
            VisibilityTabTexture = contentsManager.GetTexture(@"settingsWindow\visibilityTab.png");
            StatsTabTexture = contentsManager.GetTexture(@"settingsWindow\statsTab_156909.png");
            GeneralTabTexture = contentsManager.GetTexture(@"settingsWindow\generalTab_156736.png");
            SettingsWindowBackgroundTexture = contentsManager.GetTexture(@"settingsWindow\windowBackground_155985.png");
            SettingsWindowEmblemTexture = contentsManager.GetTexture(@"settingsWindow\settingsWindowEmblem.png");
            MoveDownTexture = contentsManager.GetTexture(@"settingsWindow\moveDown_155953.png");
            MoveDownActiveTexture = contentsManager.GetTexture(@"settingsWindow\moveDownActive_155953.png");
            MoveUpTexture = contentsManager.GetTexture(@"settingsWindow\moveUp_155953.png");
            MoveUpActiveTexture = contentsManager.GetTexture(@"settingsWindow\moveUpActive_155953.png");
            EntryIconPlaceholderTexture = contentsManager.GetTexture(@"stats\entryIconPlaceholder_1444524.png");
            CornerIconTexture = contentsManager.GetTexture(@"cornerIcon.png");
            CornerIconHoverTexture = contentsManager.GetTexture(@"cornerIconHover.png");
            HiddenStatsTexture = contentsManager.GetTexture(@"hiddenStats.png");
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
            DisposeEntryTextures();
        }

        public Texture2D DebugTabTexture { get; }
        public Texture2D HiddenStatsTexture { get; }
        public Texture2D SettingsWindowEmblemTexture { get; }
        public Texture2D StatsTabTexture { get; }
        public Texture2D GeneralTabTexture { get; }
        public Texture2D VisibilityTabTexture { get; }
        public Texture2D SettingsWindowBackgroundTexture { get; }
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
                    if (entry.HasIconAssetId)
                        EntryTextureByEntryId[entry.Id] = GetEntryTexture(entry.Id, entry.IconAssetId, EntryIconPlaceholderTexture, _logger);
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
                    notFoundTextures.Add(entry.Name.English);
                    exception = e;
                }
            }

            if (notFoundTextures.Any())
                _logger.Error(exception, $"Could not get entry texture for: {string.Join(", ", notFoundTextures)}. :(");
        }

        private static AsyncTexture2D GetEntryTexture(string entryId, int entryIconAssetId, Texture2D entryIconPlaceholderTexture, Logger logger)
        {
            if (GameService.Content.DatAssetCache.TryGetTextureFromAssetId(entryIconAssetId, out AsyncTexture2D entryTexture))
                return entryTexture;
            else
            {
                // blish will only show info message for that instead of a warning. That is why this was added here to make it more obvious
                logger.Warn($"DatAssetCache is missing texture for '{entryId}', iconAssetId: {entryIconAssetId}");
                return new AsyncTexture2D(entryIconPlaceholderTexture);
            }
        }

        private void DisposeEntryTextures()
        {
            foreach (var entryTextureAndIdPair in EntryTextureByEntryId)
            {
                var entryId = entryTextureAndIdPair.Key;
                var entryTexture = entryTextureAndIdPair.Value;
                var isNotErrorIcon = entryTexture.Texture != EntryIconPlaceholderTexture;
                var isNotFromAssetCache = !_model.GetEntry(entryId).HasIconAssetId;
                if (isNotErrorIcon && isNotFromAssetCache)
                    entryTexture?.Dispose();
            }
        }

        private readonly ContentsManager _contentsManager;
        private readonly Logger _logger;
        private readonly Model _model;
    }
}