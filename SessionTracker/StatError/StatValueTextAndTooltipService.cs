using Blish_HUD.Controls;
using SessionTracker.Api;
using System.Collections.Generic;

namespace SessionTracker.StatError
{
    internal class StatValueTextAndTooltipService
    {
        public static void SetToApiError(ApiTokenService apiTokenService, Dictionary<string, Label>.ValueCollection valueLabelByStatId, string retryInXSecondsMessage)
        {
            var apiErrorText = apiTokenService.CreateApiErrorText();
            var tooltip = $"{apiErrorText}\n{retryInXSecondsMessage}";
            Set("Error: read tooltip.", tooltip, valueLabelByStatId);
        }

        public static void SetToLoadingOrError(ApiTokenService apiTokenService, Dictionary<string, Label>.ValueCollection valueLabelByStatId)
        {
            var apiErrorTooltip = apiTokenService.CreateApiErrorText();

            switch (apiTokenService.ApiTokenState)
            {
                case ApiTokenState.hasNotLoggedIntoCharacterSinceStartingGw2:
                    Set("Log into character!", apiErrorTooltip, valueLabelByStatId);
                    break;
                case ApiTokenState.ApiTokenMissing:
                    Set("Loading...", "Waiting for API token from blish.", valueLabelByStatId); // DONT use apiErrorTooltip here. we still wait for blish giving us token here.
                    break;
                case ApiTokenState.RequiredPermissionsMissing:
                    Set("Add API permissions!", apiErrorTooltip, valueLabelByStatId);
                    break;
                default:
                    var errorMessage = $"Fallback: never reset. Switch case missing or should not be be handled here: {nameof(ApiTokenService)}.{apiTokenService.ApiTokenState}.";
                    Set("Error: Unexpected state", errorMessage, valueLabelByStatId);
                    Module.Logger.Error(errorMessage);
                    break;
            }
        }

        public static void Set(string text, string tooltip, Dictionary<string, Label>.ValueCollection valueLabelByStatId)
        {
            foreach (var valueLabel in valueLabelByStatId)
            {
                valueLabel.Text = text;
                valueLabel.BasicTooltipText = tooltip;
            }
        }
    }
}
