using DataLayer;
using Lottery_2022.Models;
using System.Drawing;

namespace Lottery_2022.Services
{
    public class IndexService : IIndexService
    {
        #region constructor / properties
        private readonly LotteryDbContext dbContext;

        public IndexService(LotteryDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public LotteryDbContext DbContext { get; set; }
        #endregion

        #region public method
        /// <summary>
        /// Displays current draw's Jackpot and number of players, on index page
        /// </summary>
        /// <returns>IndexViewModel</returns>
        public IndexViewModel GetCurrentDrawData()
        {
            string? jackpot, numberOfPlayers;
            CurrentDrawRequests(out jackpot, out numberOfPlayers);

            var result = new IndexViewModel()
            {
                Jackpot = jackpot,
                NumberOfPlayers = numberOfPlayers,
            };
            return result;
        }
        #endregion

        #region private method
        /// <summary>
        /// Retrieves jackpot and number of players for the current draw
        /// </summary>
        private void CurrentDrawRequests(out string? jackpot, out string numberOfPlayers)
        {
            var lastDraw = dbContext.GameDraws.OrderBy(x => x.Id).LastOrDefault();

            jackpot = dbContext.GameDraws?.Where(d => d.Id.Equals(lastDraw.Id))
                                             .Select(d => d.Jackpot)
                                             .FirstOrDefault()
                                             .ToString();
            numberOfPlayers = dbContext.GameSessions.Join(dbContext.GameDraws,
                                              gamesession => gamesession.GameDrawId, gamedraw => gamedraw.Id,
                                             (gamesession, gamedraw) => new { GameSession = gamesession, GameDraw = gamedraw })
                                             .Where(d => d.GameDraw.Id.Equals(lastDraw.Id))
                                             .Select(s => s.GameSession)
                                             .Count()
                                             .ToString();
        }
        #endregion
    }
}
