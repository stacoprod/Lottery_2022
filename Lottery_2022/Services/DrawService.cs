using DataLayer;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Data.SqlClient;
using DataLayer.Model;

namespace Lottery_2022.Services;

public class DrawService : BackgroundService
{
    private Timer? timer;
    private readonly LotteryDbContext dbContext;

    public DrawService(LotteryDbContext dbContext)
    {
        this.DbContext = dbContext;
    }

    public LotteryDbContext DbContext { get; set; }

    private void MyCallback(object? state)
    {
        if (state != null)
        {
            var date = (DateTime)state;
            Console.WriteLine(date.ToShortTimeString() + " -Execution du background service");
        }

    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {

        var timer = new Timer(
            callback: MyCallback, // launch to loop
            state: null, // data to receive
            dueTime: TimeSpan.Zero, // time to initially wait
            period: TimeSpan.FromMinutes(5) // period of time
            );

        while (!stoppingToken.IsCancellationRequested)
        {
            // Add a new entry in database
            var gameDraw = new GameDraw
            {
                Jackpot = 10
            };

            dbContext?.GameDraws?.Add(gameDraw);
            dbContext?.SaveChanges();

            Thread.Sleep(243600);
            // Launch of draw, 4 minutes after game start:
            drawnNumbers = LaunchGameDraw(drawnNumbers);

            string datetime = DateTime.Now.ToString("s");
            var lastGameDraw = dbContext.GameDraws.OrderByDescending(g => g.Id).FirstOrDefault();

            // Record drawn numbers into database
            if (lastGameDraw != null)
            {
                lastGameDraw.DateTime = datetime;
                lastGameDraw.DrawnNumbers = drawnNumbers;

                dbContext.SaveChanges();
            }

            // Select all sessions related to this game:
            List<int>? idSession = new List<int>();
            List<int>? ranks = new List<int>();

            var lastGameDrawId = dbContext.GameDraws.Max(g => g.Id);

            var sessions = dbContext.GameSessions
                .Where(gs => gs.GameDrawId == lastGameDrawId)
                .Join(
                    dbContext.GameDraws,
                    gs => gs.GameDrawId,
                    gd => gd.Id,
                    (gs, gd) => new { GameSession = gs, GameDraw = gd })
                .ToList();

            // Update each session with calculated ranks
            // Time available on this thread = 58200ms
            // Sufficient for a small amount of sessions (>1000) but can be exponentially long
            CalculateRanks(drawnNumbers, idSession, ranks, reader);
            RecordRanks(idSession, ranks);

            Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }

        return Task.CompletedTask;


    }
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        timer?.Change(Timeout.Infinite, Timeout.Infinite);
        timer?.Dispose(); // Stop the Timer

        return base.StopAsync(cancellationToken);
    }

    /// <summary>
    /// Random selects 6 numbers between 1 and 49 and put them into a string
    /// </summary>
    private static string LaunchGameDraw(string drawnNumbers)
    {
        Random rnd = new Random();
        int[]? numbers = new int[6];

        // random select of 6 numbers
        for (int i = 0; i < 6; i++)
        {
            int number;
            do
            {
                number = rnd.Next(1, 50);
            }
            while (numbers.Contains(number));

            numbers[i] = number;
        }

        // reformat numbers and return of a string
        string[]? numbersTab = new string[6];
        for (int i = 0; i < 6; i++)
        {
            if (numbers[i] < 10)
                numbersTab[i] = "0" + numbers[i];
            else
                numbersTab[i] = numbers[i].ToString();
        }
        drawnNumbers = String.Join(" ", numbersTab);

        return drawnNumbers;
    }
    /// <summary>
    /// Reads each row, compare numbers to draw-numbers, and calculate ranks
    /// </summary>
    private static void CalculateRanks(string drawnNumbers, List<int> idSession, List<int> ranks, SqlDataReader reader)
    {
        while (reader.Read())
        {
            idSession.Add(reader.GetInt32(0));
            string[] playedNumbers = reader.GetString(2).Split(' ');
            int matches = 0;
            int rank;

            // compare numbers to draw-numbers
            foreach (string playedNumber in playedNumbers)
            {
                if (drawnNumbers.Contains(playedNumber))
                    matches++;
            }

            // calculate ranks
            if (matches == 0)
                rank = 0;
            else
                rank = 7 - matches;
            ranks.Add(rank);
        }
        reader.Close();
    }
    /// <summary>
    /// Record each rank into corresponding row
    /// </summary>
    private static void RecordRanks(List<int> idSession, List<int> ranks)
    {
        for (int i = 0; i < idSession.Count(); i++)
        {
            var sqli = ($"UPDATE GameSessions SET Rank = '{ranks[i]}' where [Id] = {idSession[i]}");
            SqlCommand cmdi = new SqlCommand(sqli);
            cmdi.Connection = connection;
            cmdi.ExecuteNonQuery();
        }
    }
}
