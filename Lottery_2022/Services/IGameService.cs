using Lottery_2022.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.ObjectModel;

namespace Lottery_2022.Services
{
    public interface IGameService
    {
        /// <summary>
        /// Displays current draw's Jackpot and number of players, on index page
        /// </summary>
        /// <returns>IndexViewModel</returns>
        IndexViewModel GetCurrentDrawData();
        /// <summary>
        /// Generates the code, records session and displays the summary of the session (date & time of draw, numbers played and code)
        /// </summary>
        /// <returns>SessionValidationViewModel</returns>
        SessionValidationViewModel ValidateGameSession(List<int> numbers);
        /// <summary>
        /// If code ok, displasy the results of both related session and draw
        /// </summary>
        /// <returns>SessionValidationViewModel</returns>
        GetResultsWithCodeViewModel GetResultsWithCode(string shortGUID);
        void AddGenericData();
    }
}
