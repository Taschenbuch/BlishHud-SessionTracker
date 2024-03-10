using System;
using System.Collections.Generic;
using System.Linq;
using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework.Graphics;
using SessionTracker.Models;
using Model = SessionTracker.Models.Model;

namespace SessionTracker.OtherServices
{
    public class TextureService : IDisposable
    {
        public TextureService(Model model, ContentsManager contentsManager)
        {
            _contentsManager = contentsManager;
            _model           = model;

            var missingTextures = new List<string>();

            FallbackTexture = new AsyncTexture2D(contentsManager.GetTexture(@"stats\statIconPlaceholder_1444524.png"));
            SettingsWindowEmblemTexture = contentsManager.GetTexture(@"settingsWindow\settingsWindowEmblem.png");
            MoveDownTexture = contentsManager.GetTexture(@"settingsWindow\moveDown_155953.png");
            MoveDownActiveTexture = contentsManager.GetTexture(@"settingsWindow\moveDownActive_155953.png");
            MoveUpTexture = contentsManager.GetTexture(@"settingsWindow\moveUp_155953.png");
            MoveUpActiveTexture = contentsManager.GetTexture(@"settingsWindow\moveUpActive_155953.png");
            CornerIconTexture = contentsManager.GetTexture(@"cornerIcon.png");
            CornerIconHoverTexture = contentsManager.GetTexture(@"cornerIconHover.png");
            AllStatsHiddenByZeroValuesSettingTexture = GetTextureFromAssetCache(358463, FallbackTexture, missingTextures);
            SelectStatsTabTexture = GetTextureFromAssetCache(156909, FallbackTexture, missingTextures);
            ArrangeStatsTabTexture = GetTextureFromAssetCache(156756, FallbackTexture, missingTextures);
            GeneralTabTexture = GetTextureFromAssetCache(156736, FallbackTexture, missingTextures); 
            VisibilityTabTexture = GetTextureFromAssetCache(358463, FallbackTexture, missingTextures);
            DebugTabTexture = GetTextureFromAssetCache(440025, FallbackTexture, missingTextures);
            SelectStatsWindowBackgroundTexture = GetTextureFromAssetCache(155979, FallbackTexture, missingTextures); 
            StatBackgroundTexture = GetTextureFromAssetCache(1318622, FallbackTexture, missingTextures);

            foreach (var stat in model.Stats)
                StatTextureByStatId[stat.Id] = CreateStatTexture(stat, missingTextures);

            if (missingTextures.Any())
                Module.Logger.Error($"Using fallbacks because could not get texture for: {string.Join(", ", missingTextures)}. :(");
        }

        public void Dispose()
        {
            SettingsWindowEmblemTexture?.Dispose();
            MoveDownTexture?.Dispose();
            MoveDownActiveTexture?.Dispose();
            MoveUpTexture?.Dispose();
            MoveUpActiveTexture?.Dispose();
            FallbackTexture?.Dispose();
            CornerIconTexture?.Dispose();
            CornerIconHoverTexture?.Dispose();
            DisposeStatTextures();
        }

        public AsyncTexture2D DebugTabTexture { get; }
        public AsyncTexture2D AllStatsHiddenByZeroValuesSettingTexture { get; }
        public AsyncTexture2D ArrangeStatsTabTexture { get; }
        public Texture2D SettingsWindowEmblemTexture { get; }
        public AsyncTexture2D SelectStatsTabTexture { get; }
        public AsyncTexture2D GeneralTabTexture { get; }
        public AsyncTexture2D VisibilityTabTexture { get; }
        public AsyncTexture2D SelectStatsWindowBackgroundTexture { get; }
        public AsyncTexture2D StatBackgroundTexture { get; }
        public Texture2D MoveDownTexture { get; }
        public Texture2D MoveDownActiveTexture { get; }
        public Texture2D MoveUpTexture { get; }
        public Texture2D MoveUpActiveTexture { get; }
        public AsyncTexture2D FallbackTexture { get; }
        public Texture2D CornerIconTexture { get; }
        public Texture2D CornerIconHoverTexture { get; }
        public Dictionary<string, AsyncTexture2D> StatTextureByStatId { get; } = new Dictionary<string, AsyncTexture2D>();

        private AsyncTexture2D CreateStatTexture(Stat stat, List<string> missingTextures)
        {
            try
            {
                if (stat.Icon.HasIconAssetId)
                    return GetTextureFromAssetCache(stat.Icon.AssetId, FallbackTexture, missingTextures);
                else if (stat.Icon.HasIconFile)
                    return _contentsManager.GetTexture($@"stats\{stat.Icon.FileName}");
                else
                {
                    missingTextures.Add($"{stat.Name.English} (stat without AssetId or IconFile)");
                    return FallbackTexture;
                }
            }
            catch (Exception e)
            {
                missingTextures.Add($"{stat.Name.English} (Exception: {e.Message})");
                return FallbackTexture;
            }
        }

        private static AsyncTexture2D GetTextureFromAssetCache(int assetId, AsyncTexture2D fallbackTexture, List<string> missingTextures)
        {
            if (GameService.Content.DatAssetCache.TryGetTextureFromAssetId(assetId, out AsyncTexture2D texture))
                return texture;
            else
            {
                missingTextures.Add($"{assetId} (asset id missing in DatAssetCache)");
                return fallbackTexture;
            }
        }

        private void DisposeStatTextures()
        {
            foreach (var (statId, statTexture) in StatTextureByStatId.Select(s => (s.Key, s.Value)))
            {
                var isNotFallbackIcon = statTexture.Texture != FallbackTexture.Texture;
                var isNotFromAssetCache = !_model.GetStat(statId).Icon.HasIconAssetId;
                if (isNotFallbackIcon && isNotFromAssetCache)
                    statTexture?.Dispose();
            }
        }

        private readonly ContentsManager _contentsManager;
        private readonly Model _model;
    }
}