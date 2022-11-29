using DataLayer;
using DataLayer.Model;
using Lottery_2022.Models;
using System.ComponentModel.DataAnnotations;

namespace Lottery_2022.Services
{
    public class GameSessionService : IGameSessionService
    {
        private readonly LotteryDbContext dbContext;

        public GameSessionService(LotteryDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public LotteryDbContext DbContext { get; }

        public void ValidateSession(GameSessionViewModel model)
        {
            var session = new GameSession()
            {
                PlayedNumbers = model.PlayedNumbers,
                ShortGuid = model.ShortGuid,
                GameDrawId = model.GameDrawId
            };

}
    }
}
