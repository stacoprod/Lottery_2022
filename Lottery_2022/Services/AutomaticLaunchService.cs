using DataLayer;
using DataLayer.Model;
using Lottery_2022.Models;
using System.Linq;
using System.Threading.Tasks.Dataflow;

namespace Lottery_2022.Services
{
    public class AutomaticLaunchService : IAutomaticLaunchService
    {
        private readonly LotteryDbContext dbContext;

        public AutomaticLaunchService(LotteryDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public LotteryDbContext DbContext { get; set; }

        public void LaunchNewGameDraw()
        {
            var realtime = DateTime.Now;
            if (realtime.Minute % 4 == 0)
            {
                Random rnd = new Random();
                string[] numbersTab = new string[6];
                for (int i = 0; i < 6; i++)
                {
                    do {
                    int number = rnd.Next(1, 50);
                    if (number < 10)
                    {
                         (numbersTab.Contains)
                        numbersTab[i] = "0" + number;
                    }
                    else
                    {
                        numbersTab[i] = number.ToString();
                    }
                }
                string drawnNumbers = String.Join(" ", numbersTab);
                Console.Write("Voici les numéros gagnants: ");
                Console.Write(drawnNumbers);
            }
                var gameDraw = new GameDraw()
            {
                PlayedNumbers = model.PlayedNumbers
            };

            dbContext.GameDraws.Add(gameDraw);

            dbContext.SaveChanges();

        }
    }
}
