using Blish_HUD.Controls;
using SessionTracker.Api;

namespace SessionTracker.StatsHint
{
    public class ErrorLabel : Label
    {
        public ErrorLabel()
        {
            AutoSizeHeight = true;
            AutoSizeWidth = true;
        }

        public void SetToApiTokenIssue(ApiTokenService apiTokenService, bool isLoading)
        {
            var apiErrorTooltip = apiTokenService.CreateApiErrorText();

            switch (apiTokenService.ApiTokenState)
            {
                case ApiTokenState.hasNotLoggedIntoCharacterSinceStartingGw2:
                    SetTextAndTooltip("Log into character!", apiErrorTooltip);
                    break;
                case ApiTokenState.ApiTokenMissing:
                    if(isLoading)
                        SetTextAndTooltip("Loading...", "Waiting for API token from Blish."); // DONT use apiErrorTooltip here. we still wait for blish giving us token here.
                    else
                        SetTextAndTooltip("Add API key!", apiErrorTooltip); 
                    break;
                case ApiTokenState.RequiredPermissionsMissing:
                    SetTextAndTooltip("Missing API key permissions!", apiErrorTooltip);
                    break;
                default:
                    var errorMessage = $"Switch case missing or should not be be handled here: {nameof(ApiTokenService)}.{apiTokenService.ApiTokenState}.";
                    SetTextAndTooltip("Error: Unexpected state", errorMessage);
                    Module.Logger.Error(errorMessage);
                    break;
            }
        }

        public void SetTextAndTooltip(string textAndTooltip)
        {
            SetTextAndTooltip(textAndTooltip, textAndTooltip);
        }

        public void SetTextAndTooltip(string text, string tooltip)
        {
            Text = text;
            BasicTooltipText = tooltip;
        }
    }
}
