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
            _contentsManager            = contentsManager;
            _logger                     = logger;
            MoveDownTexture             = contentsManager.GetTexture("moveDown_155953.png");
            MoveDownActiveTexture       = contentsManager.GetTexture("moveDownActive_155953.png");
            MoveUpTexture               = contentsManager.GetTexture("moveUp_155953.png");
            MoveUpActiveTexture         = contentsManager.GetTexture("moveUpActive_155953.png");
            EntryIconPlaceholderTexture = contentsManager.GetTexture("entryIconPlaceholder_1444524.png");
            CornerIconTexture           = contentsManager.GetTexture("cornerIcon.png");
            CornerIconHoverTexture      = contentsManager.GetTexture("cornerIconHover.png");
            CreateEntryTextures(model);
        }

        public void Dispose()
        {
            MoveDownTexture?.Dispose();
            MoveDownActiveTexture?.Dispose();
            MoveUpTexture?.Dispose();
            MoveUpActiveTexture?.Dispose();
            EntryIconPlaceholderTexture?.Dispose();
            CornerIconTexture?.Dispose();
            CornerIconHoverTexture?.Dispose();

            foreach (var entryIcon in EntryTextureByEntryId.Values)
                entryIcon?.Dispose();
        }

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
                        EntryTextureByEntryId[entry.Id] = _contentsManager.GetTexture(entry.IconFileName);
                    else
                    {
                        _logger.Error($"Error: Icon texture missing for entryId: {entry.Id}. Use placeholder icon as fallback.");
                        EntryTextureByEntryId[entry.Id] = EntryIconPlaceholderTexture;
                    }
                }
                catch (Exception e)
                {
                    EntryTextureByEntryId[entry.Id] = new AsyncTexture2D(EntryIconPlaceholderTexture);
                    notFoundTextures.Add(entry.LabelText);
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