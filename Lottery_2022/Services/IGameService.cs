using Lottery_2022.Models;

namespace Lottery_2022.Services
{
    public interface IGameService
    {
        /// <summary>
        /// Generates the code, records session and displays the summary of the session (date & time of draw, numbers played and code)
        /// </summary>
        /// <returns>SessionValidationViewModel</returns>
        SessionValidationViewModel ValidateGameSession(List<int> numbers);
    }
}
