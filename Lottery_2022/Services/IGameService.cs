using Lottery_2022.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lottery_2022.Services
{
    public interface IGameService
    {
        IndexViewModel GetCurrentDrawData();
        SessionValidationViewModel ValidateGameSession(int[] numbers);
        GetResultsWithCodeViewModel GetResultsWithCode(string shortGUID);
    }
}
