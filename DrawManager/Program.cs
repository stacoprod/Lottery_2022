using System.Data.SqlClient;
//using DataLayer;


namespace DrawLaunch
{
    internal class Program
    {
        /*private readonly LotteryDbContext dbContext;

        public Program (LotteryDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public LotteryDbContext DbContext { get; set; }*/

        const string CHAINE_CONNEXION = @"Data Source=(localdb)\MSSqlLocalDB;Initial Catalog=LotteryDB;Integrated Security=True";
        static SqlConnection? connection = default;
        static void Main(string[] args)
        {
            Console.WriteLine("Starting a new game");
            Console.WriteLine("###################");
            Thread.Sleep(200);
            Connect();

            // at this point: delay of 800 ms because of "thread sleeps"
            // players could create session for the draw already finished
            // that's why the planned task has to start few seconds before 12:05 or 12:10 ...
            Console.WriteLine("Creation of a new entry in database...");
            var sql = ($"insert into GameDraws (Jackpot) values ({10})");
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = connection;
            cmd.ExecuteNonQuery();
            Thread.Sleep(300);
            DoneMessage();

            Disconnect();
            Console.WriteLine("Waiting of 4 minutes before the draw, take a coffee...");
            //Thread.Sleep(238300);
            Thread.Sleep(18300);

            //The game has started 4 minutes ago, at this step, it should be impossible to create a session during 1 minutes.
            //Draws are based on real time, every 5 minutes (see planned task).
            //On MVC, game is blocked by puting a verification on Time, for special ranges like 12:04 to 12:05, 12:34 to 12:35...
            //Minute % 5 = 0 to unlock and (minute+1) % 5 = 0 to lock.

            Connect();

            Console.WriteLine("Draw is starting ...");
            string drawnNumbers = "";
            drawnNumbers = LaunchGameDraw(drawnNumbers);
            Thread.Sleep(600);
            Console.WriteLine("Recording of the draw...");
            string datetime = DateTime.Now.ToString("s");
            var sql2 = ($"UPDATE GameDraws SET DateTime = '{datetime}', DrawnNumbers = '{drawnNumbers}' where [Id] = (SELECT MAX([Id]) FROM GameDraws)");
            SqlCommand cmd2 = new SqlCommand(sql2);
            cmd2.Connection = connection;
            cmd2.ExecuteNonQuery();
            Thread.Sleep(300);
            DoneMessage();

            // calculate ranks, all in SQl, can be long. Time available on this thread = 58200ms
            // Sufficient for a small amount of sessions (>1000) but can be exponentially long
            Console.WriteLine("Calcul of the results...");
            var sessions = @"select * FROM Gamesessions LEFT JOIN GameDraws ON GameSessions.GameDrawId = GameDraws.Id WHERE GameDrawId = (SELECT MAX([Id]) FROM GameDraws)";
            SqlCommand cmd3 = new SqlCommand(sessions);
            cmd3.Connection = connection;
            SqlDataReader reader = cmd3.ExecuteReader();
            while (reader.Read())
            {
                int idSession = reader.GetInt32(0);
                string[] playedNumbers = reader.GetString(2).Split(' ');
                int matches = 0;
                int rank;

                foreach (string playedNumber in playedNumbers)
                {
                    if (drawnNumbers.Contains(playedNumber))
                        matches++;
                }
                if (matches == 0)
                    rank = 0;
                else
                    rank = 7 - matches;

                var sql4 = ($"UPDATE GameSessions SET Rank = '{rank}' where [Id] = {idSession}");
                SqlCommand cmd4 = new SqlCommand(sql4);
                cmd4.Connection = connection;
                cmd4.ExecuteNonQuery();
            }
            reader.Close();
            Thread.Sleep(300);
            DoneMessage();

            Disconnect();
            Console.WriteLine("End of game");
            Console.WriteLine("###########");
            Thread.Sleep(600);
        }
        #region methods
        /// <summary>
        /// Random selects 6 numbers between 1 and 49 and put them into a string
        /// </summary>
        private static string LaunchGameDraw(string drawnNumbers)
        {
            Random rnd = new Random();
            int[]? numbers = new int[6];
            
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
            // reformat
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
        /// Reads the current session line in database, compare numbers to draw-numbers, and gives a rank
        /// </summary>
        


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