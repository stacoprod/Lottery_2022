using Lottery_2022.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lottery_2022.Services
{
    public interface IGameService
    {
        IndexViewModel GetCurrentDrawData();
        SessionSummaryViewModel GetSessionData();
        GameResultsViewModel GetGameResults();
        void ValidateSession(ValidateSessionViewModel model);
        void GetResultsWithCode(GetResultsWithCodeViewModel model);
    }
}
