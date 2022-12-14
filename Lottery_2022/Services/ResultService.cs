using DataLayer.Model;
using DataLayer;
using Lottery_2022.Models;

namespace Lottery_2022.Services
{
    public class ResultService : IResultService 
    {
        #region constructor / properties
        private readonly LotteryDbContext dbContext;

        public ResultService(LotteryDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public LotteryDbContext DbContext { get; set; }
        #endregion

        #region public methods
        /// <summary>
        /// If code ok, display the results of both related session and draw
        /// </summary>
        /// <returns>GetResultsWithCodeViewModel</returns>
        public GetResultsWithCodeViewModel GetResultsWithCode(string shortGUID)
        {
            // Verification of existence of user code:
            var shortGuidVerification = dbContext.GameSessions?.Where(s => s.ShortGuid.Equals(shortGUID)).Count();

            if (shortGuidVerification == 1)
            {
                // Check if the drawn is complete, before sending the results to user:
                GameSession sessionData;
                GameDraw relatedDrawData;
                int gameDrawID;
                string? drawCompleted;

                SessionDrawRequest(shortGUID, out sessionData, out relatedDrawData, out gameDrawID, out drawCompleted);

                if (drawCompleted == null)
                {
                    // Exception:
                    var result = new GetResultsWithCodeViewModel()
                    {
                        ErrorMessage = "Le tirage n'a pas encore eu lieu, il faudra consulter les résultats plus tard"
                    };
                    return result;
                }
                else
                {
                    // If drawn is complete:
                    int rank1Winners, rank2Winners, rank3Winners;
                    double rank1Gain, rank2Gain, rank3Gain, gain;
                    int numberOfGoodNumbers;

                    GlobalWinnersRequest(gameDrawID, out rank1Winners, out rank2Winners, out rank3Winners);

                    CalculateResults(sessionData, relatedDrawData, rank1Winners, rank2Winners, rank3Winners, out rank1Gain, out rank2Gain, out rank3Gain, out numberOfGoodNumbers, out gain);

                    //Instanciation of viewmodel with all the results requested / calculated:
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
                        DateTime = (DateTime)relatedDrawData.DateTime,
                        DrawNumbers = relatedDrawData.DrawnNumbers.Split(' '),
                        PlayedNumbers = sessionData.PlayedNumbers.Split(' ')
                    };
                    return result;
                }
            }
            else
            {
                // Exception:
                var result = new GetResultsWithCodeViewModel()
                {
                    ErrorMessage = "Le code entré ne correspond pas à une session de jeu"
                };
                return result;
            }
        }
        #endregion

        #region private methods
        /// <summary>
        /// Retrieve data from both session and related draw, with code
        /// </summary>
        private void SessionDrawRequest(string shortGUID, out GameSession sessionData, out GameDraw relatedDrawData, out int gameDrawID, out string? drawCompleted)
        {
            sessionData = dbContext.GameSessions.Where(s => s.ShortGuid.Equals(shortGUID))
                                                                        .FirstOrDefault();
            var session = sessionData;
            relatedDrawData = dbContext.GameDraws?.Where(s => s.Id.Equals(session.GameDrawId))
                                                                        .FirstOrDefault();
            gameDrawID = relatedDrawData.Id;

            int? drawId = gameDrawID;
            // Check if draw is completed and if results exist:
            drawCompleted = dbContext.GameDraws?.Where(d => d.Id.Equals(drawId))
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
        private static void CalculateResults(GameSession sessionData, GameDraw relatedDrawData, int rank1Winners, int rank2Winners, int rank3Winners, out double rank1Gain, out double rank2Gain, out double rank3Gain, out int numberOfGoodNumbers, out double gain)
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

            rank1Gain = (int)(relatedDrawData.Jackpot) * 0.6 / rank1divide;
            rank2Gain = (int)(relatedDrawData.Jackpot) * 0.2 / rank2divide;
            rank3Gain = (int)(relatedDrawData.Jackpot) * 0.2 / rank3divide;

            //Number of good numbers
            if (sessionData.Rank == 0)
                numberOfGoodNumbers = 0;
            else
                numberOfGoodNumbers = 7 - (int)sessionData.Rank;

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
        #endregion
    }
}
