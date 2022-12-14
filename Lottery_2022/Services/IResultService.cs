using Lottery_2022.Models;

namespace Lottery_2022.Services
{
    public interface IResultService
    {
        /// <summary>
        /// If code ok, displasy the results of both related session and draw
        /// </summary>
        /// <returns>GetResultsWithCodeViewModel</returns>
        GetResultsWithCodeViewModel GetResultsWithCode(string shortGUID);
    }
}
