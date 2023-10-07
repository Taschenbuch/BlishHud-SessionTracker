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
        public TextureService(Model model, ContentsManager contentsManager)
        {
            _contentsManager = contentsManager;
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
            StatIconPlaceholderTexture = contentsManager.GetTexture(@"stats\statIconPlaceholder_1444524.png");
            CornerIconTexture = contentsManager.GetTexture(@"cornerIcon.png");
            CornerIconHoverTexture = contentsManager.GetTexture(@"cornerIconHover.png");
            HiddenStatsTexture = contentsManager.GetTexture(@"hiddenStats.png");
            CreateStatTextures(model);
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
            StatIconPlaceholderTexture?.Dispose();
            CornerIconTexture?.Dispose();
            CornerIconHoverTexture?.Dispose();
            HiddenStatsTexture?.Dispose();
            DisposeStatTextures();
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
        public Texture2D StatIconPlaceholderTexture { get; }
        public Texture2D CornerIconTexture { get; }
        public Texture2D CornerIconHoverTexture { get; }
        public Dictionary<string, AsyncTexture2D> StatTextureByStatId { get; } = new Dictionary<string, AsyncTexture2D>();

        private void CreateStatTextures(Model model)
        {
            var notFoundTextures = new List<string>();
            var exception        = new Exception("i am a dummy. ignore me");

            foreach (var stat in model.Stats)
            {
                try
                {
                    if (stat.HasIconAssetId)
                        StatTextureByStatId[stat.Id] = GetStatTexture(stat.Id, stat.IconAssetId, StatIconPlaceholderTexture);
                    else if (stat.HasIconFile)
                        StatTextureByStatId[stat.Id] = _contentsManager.GetTexture($@"stats\{stat.IconFileName}");
                    else
                    {
                        Module.Logger.Error($"Error: Icon texture missing for statId: {stat.Id}. Use placeholder icon as fallback.");
                        StatTextureByStatId[stat.Id] = StatIconPlaceholderTexture;
                    }
                }
                catch (Exception e)
                {
                    StatTextureByStatId[stat.Id] = new AsyncTexture2D(StatIconPlaceholderTexture);
                    notFoundTextures.Add(stat.Name.English);
                    exception = e;
                }
            }

            if (notFoundTextures.Any())
                Module.Logger.Error(exception, $"Could not get stat texture for: {string.Join(", ", notFoundTextures)}. :(");
        }

        private static AsyncTexture2D GetStatTexture(string statId, int statIconAssetId, Texture2D statIconPlaceholderTexture)
        {
            if (GameService.Content.DatAssetCache.TryGetTextureFromAssetId(statIconAssetId, out AsyncTexture2D statTexture))
                return statTexture;
            else
            {
                // blish will only show info message for that instead of a warning. That is why this was added here to make it more obvious
                Module.Logger.Warn($"DatAssetCache is missing texture for '{statId}', iconAssetId: {statIconAssetId}");
                return new AsyncTexture2D(statIconPlaceholderTexture);
            }
        }

        private void DisposeStatTextures()
        {
            foreach (var statTextureAndIdPair in StatTextureByStatId)
            {
                var statId = statTextureAndIdPair.Key;
                var statTexture = statTextureAndIdPair.Value;
                var isNotErrorIcon = statTexture.Texture != StatIconPlaceholderTexture;
                var isNotFromAssetCache = !_model.GetStat(statId).HasIconAssetId;
                if (isNotErrorIcon && isNotFromAssetCache)
                    statTexture?.Dispose();
            }
        }

        private readonly ContentsManager _contentsManager;
        private readonly Model _model;
    }
}