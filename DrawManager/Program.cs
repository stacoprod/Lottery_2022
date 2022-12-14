using System.Data.SqlClient;

namespace DrawLaunch
{
    internal class Program
    {
        const string CHAINE_CONNEXION = @"Data Source=(localdb)\MSSqlLocalDB;Initial Catalog=LotteryDB;Integrated Security=True";
        static SqlConnection? connection = default;
        static void Main(string[] args)
        {

            #region Launch of new game
            Console.WriteLine("Nouveau jeu");
            Console.WriteLine("###########");
            Thread.Sleep(200);
            Connect();
            // At this point: delay of 800 ms because of "thread sleeps"
            // Players should not be able to create session for the draw already finished !!
            // That's why the planned task has to start few seconds before 12:05 or 12:10 ...

            // A new row is created in GameDraws table with initial jackpot value = 10:
            Console.WriteLine("Ajout d'un nouvelle ligne en base de données...");
            var sql = ($"insert into GameDraws (Jackpot) values ({10})");
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = connection;
            cmd.ExecuteNonQuery();
            Thread.Sleep(300);
            DoneMessage();

            Disconnect();
            Console.WriteLine("Temps d'attente de 4 minutes avant le tirage, le temps d'un café...");
            Thread.Sleep(243300);
            #endregion

            // The game has started 4 minutes ago, at this step, it should be impossible to create a session during 1 minutes.
            // Draws are based on real time, every 5 minutes (see planned task).
            // On MVC, game is blocked by puting a verification on Time, for special ranges like 12:04 to 12:05, 12:34 to 12:35...
            // Minute % 5 = 0 to unlock and (minute+1) % 5 = 0 to lock.

            #region launch of final draw
            // Launch of draw:
            Connect();
            Console.WriteLine("Le tirage commence ...");
            string drawnNumbers = "";
            drawnNumbers = LaunchGameDraw(drawnNumbers);
            Thread.Sleep(600);

            // Record drawn numbers into database
            Console.WriteLine("Enregistrement des résultats...");
            string datetime = DateTime.Now.ToString("s");
            var sql2 = ($"UPDATE GameDraws SET DateTime = '{datetime}', DrawnNumbers = '{drawnNumbers}' where [Id] = (SELECT MAX([Id]) FROM GameDraws)");
            SqlCommand cmd2 = new SqlCommand(sql2);
            cmd2.Connection = connection;
            cmd2.ExecuteNonQuery();
            Thread.Sleep(300);
            DoneMessage();

            // Select all sessions related to this game:
            Console.WriteLine("Calcul des résultats...");
            List<int>? idSession = new List<int>();
            List<int>? ranks = new List<int>();
            var sessions = @"select * FROM Gamesessions LEFT JOIN GameDraws ON GameSessions.GameDrawId = GameDraws.Id WHERE GameDrawId = (SELECT MAX([Id]) FROM GameDraws)";
            SqlCommand cmd3 = new SqlCommand(sessions);
            cmd3.Connection = connection;
            SqlDataReader reader = cmd3.ExecuteReader();

            // Update each session with calculated ranks
            // Time available on this thread = 58200ms
            // Sufficient for a small amount of sessions (>1000) but can be exponentially long
            CalculateRanks(drawnNumbers, idSession, ranks, reader);
            RecordRanks(idSession, ranks);

            Thread.Sleep(300);
            DoneMessage();
            Disconnect();
            Console.WriteLine("End of game");
            Console.WriteLine("###########");
            Thread.Sleep(800);
            #endregion
        }
        #region methods
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
        /// <summary>
        /// Open/Close methods
        /// </summary>
        static void Connect()
        {
            Console.WriteLine("Opening connection...");
            connection = new SqlConnection(CHAINE_CONNEXION);
            connection?.Open();
            Thread.Sleep(300);
            Console.WriteLine("Succeed");
            Thread.Sleep(300);
        }
        static void Disconnect()
        {
            Console.WriteLine("Closing connection...");
            connection?.Close();
            Thread.Sleep(300);
        }
        private static void DoneMessage()
        {
            Console.WriteLine("Done");
            Thread.Sleep(300);
        }
        #endregion methods
    }
}