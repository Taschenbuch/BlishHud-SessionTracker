using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gw2Sharp.WebApi.V2;
using Gw2Sharp.WebApi.V2.Models;
using SessionTracker.Models;

namespace SessionTracker.Services.Api
{
    public class ItemSearchService
    {
        public static void SetItemTotalValues(Model model,
                                              Task<IApiV2ObjectList<Character>> charactersTask,
                                              Task<IApiV2ObjectList<AccountItem>> bankTask,
                                              Task<IApiV2ObjectList<AccountItem>> sharedInventoryTask,
                                              Task<IApiV2ObjectList<AccountMaterial>> materialStorageTask)
        {
            foreach (var stat in model.Stats.Where(e => e.IsItem))
                stat.Value.Total = GetItemValue(charactersTask, bankTask, sharedInventoryTask, materialStorageTask, stat.ApiId);
        }

        private static int GetItemValue(Task<IApiV2ObjectList<Character>> charactersTask,
                                        Task<IApiV2ObjectList<AccountItem>> bankTask,
                                        Task<IApiV2ObjectList<AccountItem>> sharedInventoryTask,
                                        Task<IApiV2ObjectList<AccountMaterial>> materialStorageTask,
                                        int itemId)
        {
            var inventoryItems = GetAllCharactersInventoryItems(charactersTask);
            return GetItemCount(itemId, bankTask, sharedInventoryTask, materialStorageTask, inventoryItems);
        }

        private static List<AccountItem> GetAllCharactersInventoryItems(Task<IApiV2ObjectList<Character>> charactersTask)
        {
            var allCharactersInventoryItems = new List<AccountItem>();

            foreach (var character in charactersTask.Result)
            {
                if (character.Bags == null)
                    continue;

                var singleCharacterInventoryItems = GetSingleCharacterInventoryItems(character);
                allCharactersInventoryItems.AddRange(singleCharacterInventoryItems);
            }

            return allCharactersInventoryItems;
        }

        private static List<AccountItem> GetSingleCharacterInventoryItems(Character character)
        {
            return character.Bags
                            .Where(IsNotEmptyBagSlot)
                            .Select(b => b.Inventory)
                            .SelectMany(i => i)
                            .Where(IsNotEmptyItemSlot)
                            .ToList();
        }

        private static int GetItemCount(int itemId,
                                        Task<IApiV2ObjectList<AccountItem>> bankTask,
                                        Task<IApiV2ObjectList<AccountItem>> sharedInventoryTask,
                                        Task<IApiV2ObjectList<AccountMaterial>> materialStorageTask,
                                        List<AccountItem> inventoryItems)
        {
            var itemCount = 0;
            itemCount += CountItem(itemId, inventoryItems);
            itemCount += CountItem(itemId, bankTask.Result);
            itemCount += CountItem(itemId, sharedInventoryTask.Result);
            itemCount += CountItem(itemId, materialStorageTask.Result);
            return itemCount;
        }

        private static int CountItem(int itemId, IEnumerable<AccountItem> accountItems)
        {
            return accountItems.Where(IsNotEmptyItemSlot)
                               .Where(i => i.Id == itemId)
                               .Sum(i => i.Count);
        }

        private static int CountItem(int itemId, IEnumerable<AccountMaterial> accountMaterials)
        {
            return accountMaterials.Where(i => i.Id == itemId)
                                   .Sum(i => i.Count);
        }

        private static bool IsNotEmptyBagSlot(CharacterInventoryBag bag) => bag != null;
        private static bool IsNotEmptyItemSlot(AccountItem itemSlot) => itemSlot != null;
    }
}