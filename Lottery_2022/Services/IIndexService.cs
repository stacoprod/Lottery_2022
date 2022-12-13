using Lottery_2022.Models;

namespace Lottery_2022.Services
{
    public interface IIndexService
    {
        /// <summary>
        /// Displays current draw's Jackpot and number of players, on index page
        /// </summary>
        /// <returns>IndexViewModel</returns>
        IndexViewModel GetCurrentDrawData();
    }
}
