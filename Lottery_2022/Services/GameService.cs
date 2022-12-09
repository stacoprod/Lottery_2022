using DataLayer;
using DataLayer.Model;
using Lottery_2022.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
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

        #region get
        public IndexViewModel GetCurrentDrawData()
        {
            //retrieve jackpot and number of players for the current draw:
            var lastDraw = dbContext.GameDraws.OrderBy(x => x.Id).LastOrDefault();

            var jackpot = dbContext.GameDraws.Where(d => d.Id.Equals(lastDraw.Id))
                                             .Select(d => d.Jackpot)
                                             .FirstOrDefault()
                                             .ToString();
            var numberOfPlayers = dbContext.GameSessions.Join(dbContext.GameDraws,
                                              gamesession => gamesession.GameDrawId, gamedraw => gamedraw.Id,
                                             (gamesession, gamedraw) => new { GameSession = gamesession, GameDraw = gamedraw })
                                             .Where(d => d.GameDraw.Id.Equals(lastDraw.Id))
                                             .Select(s => s.GameSession)
                                             .Count()
                                             .ToString();                                                            
            //return view model:
            var result = new IndexViewModel()
            {
                Jackpot = jackpot,
                NumberOfPlayers = numberOfPlayers,     
            };
            return result;
        }
        
        #endregion

        #region post

        public GetResultsWithCodeViewModel GetResultsWithCode(string shortGUID)
        {
            var controlOfShortGuid = dbContext.GameSessions.Where(s => s.ShortGuid.Equals(shortGUID))
                                             .Count();
      
            //if code ok than launch calcul of results
            if (controlOfShortGuid == 1)
            {
                // Retrieve data from both session and related draw
                var sessionData = dbContext.GameSessions.Where(s => s.ShortGuid.Equals(shortGUID))
                                                 .FirstOrDefault();
                
                var relatedDrawData = dbContext.GameDraws.Where(s => s.Id.Equals(sessionData.GameDrawId))
                                                 .FirstOrDefault();
                //record GameDraw Id
                var gameDrawID = relatedDrawData?.Id;

                // Retrieve number of winners by category

                int rank1Winners = dbContext.GameSessions.Where(s => s.Rank.Equals(1))
                                                        .Where(i => i.GameDrawId.Equals(gameDrawID))
                                                        .Count();
                int rank2Winners = dbContext.GameSessions.Where(s => s.Rank.Equals(2))
                                                        .Where(i => i.GameDrawId.Equals(gameDrawID))
                                                        .Count();
                int rank3Winners = dbContext.GameSessions.Where(s => s.Rank.Equals(3))
                                                        .Where(i => i.GameDrawId.Equals(gameDrawID))
                                                        .Count();
                // Manage the cases with 0 winners for calculation
                int rank1divide;
                int rank2divide;
                int rank3divide;

                if (rank1Winners == 0) { rank1divide = 1; } else { rank1divide = rank1Winners; }
                if (rank2Winners == 0) { rank2divide = 1; } else { rank2divide = rank2Winners; }
                if (rank3Winners == 0) { rank3divide = 1; } else { rank3divide = rank3Winners; }
                //Calculate general gains
                int rank0Gain = 0;
                double? rank1Gain = (relatedDrawData.Jackpot) * 0.6 / rank1divide;
                double? rank2Gain = (relatedDrawData.Jackpot) * 0.2 / rank2divide;
                double? rank3Gain = (relatedDrawData.Jackpot) * 0.2 / rank3divide;

                //Display amount of good numbers
                int? numberOfGoodNumbers;
                if (sessionData.Rank == 0)
                    numberOfGoodNumbers = 0;
                else
                    numberOfGoodNumbers = 7-sessionData.Rank;

                //Display personnal gain by checking the rank:
                double? gain = 0;
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
                // Filling ViewModel
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
            else
            {
                throw new Exception(message: "Le code entré ne correspond pas à une session de jeu, veuillez vérifier votre saisie");
            }
            
        }
        public SessionValidationViewModel ValidateGameSession(int[] numbers)
        {
            
            var playedNumbersFromParameters = numbers.ToString();

            //record into database
            var session = new DataLayer.Model.GameSession
            {
                PlayedNumbers = playedNumbersFromParameters
            };
            dbContext.GameSessions.Add(session);
            // dbContext.SaveChanges();


            //display confirmation page
            var playedNumbers = "01 02 03 04 05 06";
            var shortGuid = "1234567890123456789012";

            var result = new SessionValidationViewModel()
            {
                PlayedNumbers = playedNumbers.Split(" "),
                Code = shortGuid,
                DrawDateTime = DateTime.Today
            };
            return result;
        }
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
            dbContext.SaveChanges();
        }
        #endregion
    }
}
