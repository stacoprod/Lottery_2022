using DataLayer;
using DataLayer.Model;
using Lottery_2022.Models;

namespace Lottery_2022.Services
{
    public class GameService : IGameService
    {
        private readonly LotteryDbContext dbContext;

        public GameService(LotteryDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public LotteryDbContext DbContext { get; set; }
        
        #region get
        public IndexViewModel GetCurrentDrawData()
        {
            var jackpot = 1000;
            var numberOfPlayers = 100;

            var result = new IndexViewModel()
            {
                Jackpot = jackpot,
                NumberOfPlayers = numberOfPlayers,     
            };
            return result;
            //operations en BDD, intelligence & Busnisess Layer
        }
        public SessionSummaryViewModel GetSessionData()
        {
            var playedNumbers = "01 02 03 04 05 06";
            var shortGuid = "1234567890123456789012";

            var result = new SessionSummaryViewModel()
            {
                PlayedNumbers = playedNumbers,
                ShortGuid = shortGuid,
                DateTime = DateTime.Today
            };
            return result;
            //operations en BDD, intelligence & Busnisess Layer
        }
        public GameResultsViewModel GetGameResults()
        {
            var rank1Winners = 0;
            var rank2Winners = 0;
            var rank3Winners = 1;
            var rank1Gain = 666;
            var rank2Gain = 166;
            var rank3Gain = 166;
            var winningNumbers = "11 12 13 07 08 09";
            var playedNumbers = "01 02 03 04 05 06";
            var numberOfGoodNumbers = 0;
            var gain = 0;

            var result = new GameResultsViewModel()
            {
                Rank1Winners = rank1Winners,
                Rank2Winners = rank2Winners,
                Rank3Winners = rank3Winners,
                Rank1Gain = rank1Gain,
                Rank2Gain = rank2Gain,
                Rank3Gain = rank3Gain,
                NumberOfGoodNumbers = numberOfGoodNumbers,
                Gain = gain,
                DateTime = DateTime.Today,
                DrawNumbers = winningNumbers.Split(' '),
                PlayedNumbers = playedNumbers.Split(' ')

            };
            return result;
            //operations en BDD, intelligence & Business Layer
        }
        #endregion

        #region post
        public void ValidateSession(ValidateSessionViewModel model)
        {
            var session = new GameSession()
            {
                PlayedNumbers = model.PlayedNumbers
            };
            
            dbContext.GameSessions.Add(session);
            dbContext.SaveChanges();
            //operations en BDD, intelligence & Busnisess Layer
        }
        public void GetResultsWithCode(GetResultsWithCodeViewModel model)
        {
            var session = new GameResultsViewModel();
            {
                // ShortGuid = model.ShortGuid;
            };
            // dbContext.GameSessions.Add(session);

            // dbContext.SaveChanges();
            //operations en BDD, intelligence & Busnisess Layer
        }
        #endregion
    }
}
