using System.Linq;
using System.Threading.Tasks;
using Gw2Sharp.WebApi.V2;
using Gw2Sharp.WebApi.V2.Models;
using SessionTracker.Models;

namespace SessionTracker.Services.Api
{
    public class CurrencyTotalValueService
    {
        public static void SetCurrencyTotalValues(Model model, Task<IApiV2ObjectList<AccountCurrency>> walletTask)
        {
            foreach (var entry in model.Entries.Where(v => v.IsCurrency))
                entry.Value.Total = GetCurrencyValue(walletTask, entry.ApiId);
        }

        private static int GetCurrencyValue(Task<IApiV2ObjectList<AccountCurrency>> walletTask, int currencyId)
        {
            // not sure if value can be missing == null. 
            return walletTask.Result
                             .FirstOrDefault(c => c.Id == currencyId)
                             ?.Value ?? 0;
        }
    }
}