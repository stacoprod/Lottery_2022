using DataLayer;
using DataLayer.Model;

namespace Lottery_2022.Services
{
    public class TestService : ITestService
    {
        private readonly LotteryDbContext dbContext;

        public TestService(LotteryDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public LotteryDbContext DbContext { get; set; }

        /// <summary>
        /// Tool used to add test data to DB (like seed method)
        /// </summary>
        public void AddGenericData()
        {
            var data1 = new GameDraw()
            {
                DateTime = DateTime.Now,
                DrawnNumbers = "01 05 10 17 19 35",
                Jackpot = 100000

            };
            var data2 = new GameSession()
            {
                PlayedNumbers = "10 12 14 24 36 39",
                ShortGuid = "6666666666666666666666",
                GameDrawId = 8,
                Rank = 3
            };
            var data3 = new GameSession()
            {
                PlayedNumbers = "01 05 10 17 19 45",
                ShortGuid = "7777777777777777777777",
                GameDrawId = 8,
                Rank = 2
            };
            var data4 = new GameSession()
            {
                PlayedNumbers = "02 03 10 17 19 35",
                ShortGuid = "8888888888888888888888",
                GameDrawId = 8,
                Rank = 3
            };
            var data5 = new GameSession()
            {
                PlayedNumbers = "01 05 10 17 19 35",
                ShortGuid = "9999999999999999999999",
                GameDrawId = 8,
                Rank = 1
            };
            dbContext.GameDraws?.Add(data1);
            dbContext.GameSessions?.Add(data2);
            dbContext.GameSessions?.Add(data3);
            dbContext.GameSessions?.Add(data4);
            dbContext.GameSessions?.Add(data5);
            //dbContext.SaveChanges();
        }
    }
}
