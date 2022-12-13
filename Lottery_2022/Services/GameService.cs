using CSharpVitamins;
using DataLayer;
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
        /// <summary>
        /// Generates the code, records session and displays the summary of the session (date & time of draw, numbers played and code)
        /// </summary>
        /// <returns>SessionValidationViewModel</returns>
        public SessionValidationViewModel ValidateGameSession(List<int> numbers)
        {
            bool timesUp = CheckTime();
            if (!timesUp)
            {
                string playedNumbers = FormatNumbers(numbers);

                string shortGuid = GenerateShortGuid();

                RecordSessionData(playedNumbers, shortGuid);

                var result = new SessionValidationViewModel()
                {
                    PlayedNumbers = playedNumbers.Split(" "),
                    Code = shortGuid,
                    DrawDateTime = DateTime.Now
                };
                return result;
            }
            else
            {
                throw new Exception(message: "Les jeux sont faits, veuillez attendre le prochain jeu pour valider vos numéros");
            }
        }

        /// <summary>
        /// Block session validation if it remains less than 1 minute before next game
        /// </summary>
        private static bool CheckTime()
        {
            int time = DateTime.UtcNow.Minute;
            if (time % 5 < 4)
                return false;
            else
                return true;
        }
        /// <summary>
        /// Put the 6 numbers into a string of 17 caracters
        /// </summary>
        private static string FormatNumbers(List<int> numbers)
        {
            string[] numericString = new string[6];

            for (int i = 0; i < 6; i++)
            {
                if (numbers[i] < 10)
                {
                    numericString[i] = "0" + numbers[i].ToString();
                }
                else
                {
                    numericString[i] = numbers[i].ToString();
                }
            }
            string playedNumbers = String.Join(" ", numericString);
            return playedNumbers;
        }
        /// <summary>
        /// Generate Short Guid that doesn't already exist in database, thanks to CSharpVitamin package
        /// </summary>
        private string GenerateShortGuid()
        {
            int? codeVerification;
            string? shortGuid;

            do
            {
                Guid? guid = Guid.NewGuid();
                ShortGuid? sGuid = guid;
                shortGuid = sGuid;
                codeVerification = dbContext.GameSessions?.Where(s => s.ShortGuid.Equals(shortGuid)).Count();
            }
            while (codeVerification != 0);
            return shortGuid;
        }
        /// <summary>
        /// Retrieve Id of the current GameDraw and record session into database
        /// </summary>
        private void RecordSessionData(string playedNumbers, string shortGuid)
        {
            int currentGameDrawId = dbContext.GameDraws.OrderBy(x => x.Id).Select(i => i.Id).LastOrDefault();

            var session = new DataLayer.Model.GameSession
            {
                PlayedNumbers = playedNumbers,
                ShortGuid = shortGuid,
                GameDrawId = currentGameDrawId
            };
            dbContext.GameSessions?.Add(session);
            dbContext.SaveChanges();

            // add 5€ to the jackpot
            var lastDraw = dbContext.GameDraws.OrderBy(x => x.Id).LastOrDefault();
            lastDraw.Jackpot = (lastDraw.Jackpot)+5;
            dbContext.SaveChanges();
        }        
    }
}
