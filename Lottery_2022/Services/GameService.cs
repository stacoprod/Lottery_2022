using CSharpVitamins;
using DataLayer;
using DataLayer.Model;
using Lottery_2022.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

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
        /// <summary>
        /// Retrieves jackpot and number of players for the current draw
        /// </summary>
        private void CurrentDrawRequests(out string? jackpot, out string numberOfPlayers)
        {
            var lastDraw = dbContext.GameDraws.OrderBy(x => x.Id).LastOrDefault();

            jackpot = dbContext.GameDraws.Where(d => d.Id.Equals(lastDraw.Id))
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
        

        public GetResultsWithCodeViewModel GetResultsWithCode(string shortGUID)
        {
            var shortGuidVerification = dbContext.GameSessions.Where(s => s.ShortGuid.Equals(shortGUID)).Count();
            
            if (shortGuidVerification == 1)
            {
                GameSession? sessionData;
                GameDraw? relatedDrawData;
                int? gameDrawID;
                string? drawCompleted;

                SessionDrawRequest(shortGUID, out sessionData, out relatedDrawData, out gameDrawID, out drawCompleted);
                
                if (drawCompleted == null)
                {
                    throw new Exception(message: "Le tirage n'a pas encore eu lieu, il faudra consulter les résultats plus tard");
                }
                else
                {
                    int rank1Winners, rank2Winners, rank3Winners;
  
                    GlobalWinnersRequest(gameDrawID, out rank1Winners, out rank2Winners, out rank3Winners);

                    double? rank1Gain, rank2Gain, rank3Gain, gain;
                    int? numberOfGoodNumbers;

                    CalculateResults(sessionData, relatedDrawData, rank1Winners, rank2Winners, rank3Winners, out rank1Gain, out rank2Gain, out rank3Gain, out numberOfGoodNumbers, out gain);

                    var result = new GetResultsWithCodeViewModel()
                    {
                        Rank1Winners = rank1Winners,
                        Rank2Winners = rank2Winners,
                        Rank3Winners = rank3Winners,
                        Rank1Gain = rank1Gain,
                        Rank2Gain = rank2Gain,
                        Rank3Gain = rank3Gain,
                        NumberOfGoodNumbers = numberOfGoodNumbers,
                        Gain = gain,
                        DateTime = relatedDrawData.DateTime,
                        DrawNumbers = relatedDrawData.DrawnNumbers.Split(' '),
                        PlayedNumbers = sessionData.PlayedNumbers.Split(' ')
                    };
                    return result;
                }
            }
            else
            {
                throw new Exception(message: "Le code entré ne correspond pas à une session de jeu, veuillez vérifier votre saisie");
            }   
        }
        /// <summary>
        /// Retrieve data from both session and related draw, with code
        /// </summary>
        private void SessionDrawRequest(string shortGUID, out GameSession? sessionData, out GameDraw? relatedDrawData, out int? gameDrawID, out string? drawCompleted)
        {
            sessionData = dbContext.GameSessions.Where(s => s.ShortGuid.Equals(shortGUID))
                                                                        .FirstOrDefault();
            var session = sessionData;
            relatedDrawData = dbContext.GameDraws.Where(s => s.Id.Equals(session.GameDrawId))
                                                                        .FirstOrDefault();
            gameDrawID = relatedDrawData?.Id;
            
            int? drawId = gameDrawID;
            drawCompleted = dbContext.GameDraws.Where(d => d.Id.Equals(drawId))
                                         .Select(d => d.DrawnNumbers)
                                         .FirstOrDefault();
        }
        /// <summary>
        /// Find number of winners for each rank
        /// </summary>
        private void GlobalWinnersRequest(int? gameDrawID, out int rank1Winners, out int rank2Winners, out int rank3Winners)
        {
            rank1Winners = dbContext.GameSessions.Where(s => s.Rank.Equals(1))
                                                                        .Where(i => i.GameDrawId.Equals(gameDrawID))
                                                                        .Count();
            rank2Winners = dbContext.GameSessions.Where(s => s.Rank.Equals(2))
                                                                        .Where(i => i.GameDrawId.Equals(gameDrawID))
                                                                        .Count();
            rank3Winners = dbContext.GameSessions.Where(s => s.Rank.Equals(3))
                                                                        .Where(i => i.GameDrawId.Equals(gameDrawID))
                                                                        .Count();
        }
        /// <summary>
        /// Calculates general gains, and user results
        /// </summary>
        private static void CalculateResults(GameSession? sessionData, GameDraw? relatedDrawData, int rank1Winners, int rank2Winners, int rank3Winners, out double? rank1Gain, out double? rank2Gain, out double? rank3Gain, out int? numberOfGoodNumbers, out double? gain)
        {
            // Manage the cases with 0 winners
            int rank1divide;
            int rank2divide;
            int rank3divide;

            if (rank1Winners == 0) { rank1divide = 1; } else { rank1divide = rank1Winners; }
            if (rank2Winners == 0) { rank2divide = 1; } else { rank2divide = rank2Winners; }
            if (rank3Winners == 0) { rank3divide = 1; } else { rank3divide = rank3Winners; }

            //General gains
            int rank0Gain = 0;

            rank1Gain = (relatedDrawData.Jackpot) * 0.6 / rank1divide;
            rank2Gain = (relatedDrawData.Jackpot) * 0.2 / rank2divide;
            rank3Gain = (relatedDrawData.Jackpot) * 0.2 / rank3divide;

            //Number of good numbers
            if (sessionData.Rank == 0)
                numberOfGoodNumbers = 0;
            else
                numberOfGoodNumbers = 7 - sessionData.Rank;

            //Personal gain:
            gain = 0;
            switch (sessionData.Rank)
            {
                case 0:
                    gain = rank0Gain;
                    break;
                case 1:
                    gain = rank1Gain;
                    break;
                case 2:
                    gain = rank2Gain;
                    break;
                case 3:
                    gain = rank3Gain;
                    break;
            }
        }

        
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
            int codeVerification;
            string shortGuid;

            do
            {
                Guid? guid = Guid.NewGuid();
                ShortGuid? sGuid = guid;
                shortGuid = sGuid;
                codeVerification = dbContext.GameSessions.Where(s => s.ShortGuid.Equals(shortGuid)).Count();
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
            dbContext.GameSessions.Add(session);
            dbContext.SaveChanges();

            // add 5€ to the jackpot
            var lastDraw = dbContext.GameDraws.OrderBy(x => x.Id).LastOrDefault();
            lastDraw.Jackpot = (lastDraw.Jackpot)+5;
            dbContext.SaveChanges();
        }








        /// <summary>
        /// Tool used to add test data to DB (like seed method)
        /// </summary>
        public void AddGenericData()
        {     
            var data1 = new GameDraw()
            {
                DateTime = DateTime.Now,
                DrawnNumbers = "01 05 10 17 19 35",
                Jackpot =100000

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
            dbContext.GameDraws.Add(data1);
            dbContext.GameSessions.Add(data2);
            dbContext.GameSessions.Add(data3);
            dbContext.GameSessions.Add(data4);
            dbContext.GameSessions.Add(data5);
            //dbContext.SaveChanges();
        }
    }
}
