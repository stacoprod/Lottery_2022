using System.Data.SqlClient;

namespace DrawLaunch
{
    internal class Program
    {
        const string CHAINE_CONNEXION = @"Data Source=(localdb)\MSSqlLocalDB;Initial Catalog=LotteryDB;Integrated Security=True";
        static SqlConnection? connection = default;
        static void Main(string[] args)
        {
            Console.WriteLine("Starting a new game");
            Console.WriteLine("###################");
            Thread.Sleep(200);

            Connect();
            Thread.Sleep(300);
            Console.WriteLine("Succeed");
            Thread.Sleep(300);

            // at this point: delay of 800 ms
            Console.WriteLine("Creation of a new entry in database...");
            var sql = ($"insert into GameDraws (Jackpot) values ({10})");
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = connection;
            cmd.ExecuteNonQuery();
            Thread.Sleep(300);
            Console.WriteLine("Done");
            Thread.Sleep(300);

            Disconnect();
            Thread.Sleep(300);

            Console.WriteLine("Waiting of 4 minutes before the draw, take a coffee...");
            Thread.Sleep(238300);

            //The game has started 4 minutes ago, at this step, it should be impossible to create a session during 1 minutes.
            //Draws are based on real time, every 5 minutes, I can block the game by puting a verification on Time, for special
            // ranges like 12:04 to 12:05, 12:34 to 12:35: minute % 5 = 0 to unlock and (minute+1) % 5 = 0 to lock.

            Connect();
            Thread.Sleep(300);
            Console.WriteLine("Succeed");
            Thread.Sleep(300);

            Console.WriteLine("Draw is starting ...");
            string drawnNumbers = "";
            drawnNumbers = LaunchGameDraw(drawnNumbers);
            Thread.Sleep(600);

            Console.WriteLine("Recording of the results...");
            var sql2 = ($"UPDATE GameDraws SET DateTime = '{DateTime.Now}', DrawnNumbers = '{drawnNumbers}' where [Id] = (SELECT MAX([Id]) FROM GameDraws)");
            SqlCommand cmd2 = new SqlCommand(sql2);
            cmd2.Connection = connection;
            cmd2.ExecuteNonQuery(); 
            Thread.Sleep(300);
            Console.WriteLine("Done");
            Thread.Sleep(300);

            // calculate ranks, all in SQl, can be long. Time available on this thread = 58200ms
            // Sufficient for a small amount of sessions (>1000) but can be exponenitally long

            //Connect();


            // request goes here


            // problem of id
            // ALTER DATABASE SCOPED CONFIGURATION SET IDENTITY_CACHE = OFF

            //select of sessions from last gamedraw
            //  UPDATE GameSessions SET Rank = () where[Id] = (SELECT MAX([Id]) FROM GameDraws)


            // Find a way to compare strings

            /*


            SELECT * FROM GameSessions INNER JOIN GameDraws ON GameSessions.GameDrawId = (SELECT MAX(Id) from GameDraws);

             *UPDATE GameSessions SET Rank = (

            SELECT * FROM GameSessions INNER JOIN GameDraws ON GameSessions.GameDrawId = GameDraws.MAX(Id)
            (
                SELECT 1

                FROM STRING_SPLIT(GameSessions.PlayedNumbers, ' ') str1

                LEFT JOIN STRING_SPLIT(GameDraws.DrawnNumbers, ' ') str2 ON str2.value = str1.value

                HAVING COUNT(CASE WHEN str2.value IS NULL THEN 1 = 0)

                  AND COUNT(*) = (SELECT COUNT(*) FROM STRING_SPLIT(GameDraws.DrawnNumbers, ' '))
             )) where[Id] = (SELECT MAX([Id]) FROM GameDraws)


            SELECT* FROM GameSessions INNER JOIN GameDraws ON GameSessions.GameDrawId = GameDraws.MAX(Id)
            (
                SELECT 1
                FROM STRING_SPLIT(GameSessions.PlayedNumbers, ' ') str1
                LEFT JOIN STRING_SPLIT(GameDraws.DrawnNumbers, ' ') str2 ON str2.value = str1.value
                HAVING COUNT(CASE WHEN str2.value IS NULL THEN 1 = 0)
                  AND COUNT(*) = (SELECT COUNT(*) FROM STRING_SPLIT(GameDraws.DrawnNumbers, ' '))
            );


            var sql3 = ("select * from GameDraws");


            SqlCommand cmd3 = new SqlCommand(sql3);
            cmd3.ExecuteNonQuery();
            cmd3.Connection = connection;

            Disconnect();
             */
            Disconnect();
            Thread.Sleep(300);
            Console.WriteLine("End of game");
            Console.WriteLine("###########");
            Thread.Sleep(600);
        }
       
        static void Connect()
        {
            Console.WriteLine("Opening connection...");
            connection = new SqlConnection(CHAINE_CONNEXION);
            connection?.Open();   
        }
        static void Disconnect()
        {
            Console.WriteLine("Closing connection...");
            connection?.Close();
        }
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


    }
}