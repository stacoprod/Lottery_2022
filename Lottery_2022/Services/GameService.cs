using CSharpVitamins;
using DataLayer;
using Lottery_2022.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lottery_2022.Services
{
    public class GameService : IGameService
    {
        #region constructor / properties
        private readonly LotteryDbContext dbContext;

        public GameService(LotteryDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public LotteryDbContext DbContext { get; set; }
        #endregion

        #region public method
        /// <summary>
        /// Generates the code, records session and displays a review for the user (date & time of draw, numbers played and code)
        /// </summary>
        /// <returns>SessionValidationViewModel</returns>
        public SessionValidationViewModel ValidateGameSession(List<int> numbers)
        {
            // Verification of real time before operations:
            bool timesUp = CheckTime();
            if (!timesUp)
            {
                string playedNumbers = FormatNumbers(numbers);
                string shortGuid = GenerateShortGuid();

                // Database record method:
                RecordSessionData(playedNumbers, shortGuid);

                //Instanciation of viewmodel with a review for user:
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
                // Exception:
                var result = new SessionValidationViewModel()
                {
                    ErrorMessage = "Les jeux sont faits, veuillez attendre la prochaine partie pour jouer"
                };
                return result;
            }
        }
        #endregion

        #region private methods
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
            // format is like: "XX XX XX XX XX XX";
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
            //while code already exists:
            while (codeVerification != 0);

            return shortGuid;
        }
        /// <summary>
        /// Retrieve Id of the current GameDraw and record session into database
        /// </summary>
        private void RecordSessionData(string playedNumbers, string shortGuid)
        {
            int currentGameDrawId = dbContext.GameDraws.OrderBy(x => x.Id).Select(i => i.Id).LastOrDefault();

            //Record:
            var session = new DataLayer.Model.GameSession
            {
                PlayedNumbers = playedNumbers,
                ShortGuid = shortGuid,
                GameDrawId = currentGameDrawId
            };
            dbContext.GameSessions?.Add(session);
            dbContext.SaveChanges();

            // add 5€ to the jackpot, per new session:
            var lastDraw = dbContext.GameDraws.OrderBy(x => x.Id).LastOrDefault();
            lastDraw.Jackpot = (lastDraw.Jackpot)+5;
            dbContext.SaveChanges();
        }
        #endregion
    }
}
