using System.Data.SqlClient;

namespace DrawLaunch
{
    internal class Program
    {/*
        const string CHAINE_CONNEXION = @"Data Source=(localdb)\MSSqlLocalDB;Initial Catalog=LotteryDB;Integrated Security=True";
        static SqlConnection? connection = default;*/
        static void Main(string[] args)
        {
            Console.WriteLine("Test...");
            Console.WriteLine(DateTime.Now);
            /*Connect();
            // select list of session played numbers
           var sessions = @"select * FROM Gamesessions LEFT JOIN GameDraws ON GameSessions.GameDrawId = GameDraws.Id WHERE GameDrawId = (SELECT MAX([Id]) FROM GameDraws)";
           SqlCommand cmd3 = new SqlCommand(sessions);
           cmd3.Connection = connection;
           SqlDataReader reader = cmd3.ExecuteReader();
           while (reader.Read())
           {
               int idSession = reader.GetInt32(0);
               string playedNumbers = reader.GetString(2);
               Console.WriteLine($"{idSession}   {playedNumbers}");
           }
           reader.Close();*/
            // select draw numbers
            /*var game = @"select * FROM GameDraws where [Id] = (SELECT MAX([Id]) FROM GameDraws)";
            SqlCommand cmd4 = new SqlCommand(game);
            cmd4.Connection = connection;
            SqlDataReader reader2 = cmd4.ExecuteReader();
            while (reader2.Read())
            {string? drawnNumbers = reader2.GetString(2);
                Console.WriteLine($"Combinaison gagnante:   {drawnNumbers}");
            }
            reader2.Close();
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
        }*/

        }
    }
}