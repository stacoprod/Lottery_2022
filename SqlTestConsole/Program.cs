using System.Data.SqlClient;

namespace SqlTestConsole
{
    internal class Program
    {
        const string CHAINE_CONNEXION = @"Data Source=(localdb)\MSSqlLocalDB;Initial Catalog=LotteryDB;Integrated Security=True";
        static SqlConnection connection = default;
        static void Main(string[] args)
        {
            Console.WriteLine("Test...");
            Console.WriteLine(DateTime.Now);
            connection?.Open();
            // select list of session played numbers
            var sessions = @"select * FROM Gamesessions LEFT JOIN GameDraws ON GameSessions.GameDrawId = GameDraws.Id WHERE GameDrawId = (SELECT MAX([Id]) FROM GameDraws)";
           SqlCommand cmd3 = new SqlCommand(sessions);
           cmd3.Connection = connection;
           SqlDataReader reader = cmd3.ExecuteReader();
           while (reader.Read())
           {
               int idSession = reader.GetInt32(0);
               string playedNumbers = reader.GetString(2);
               Console.WriteLine($"Liste des grilles jouées pour le dernier tirage: {idSession}   {playedNumbers}");
           }
           reader.Close();
           connection?.Close();
        }
    }
}