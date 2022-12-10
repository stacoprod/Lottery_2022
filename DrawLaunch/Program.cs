using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using static System.Formats.Asn1.AsnWriter;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DrawLaunch
{
    internal class Program
    {
        const string CHAINE_CONNEXION = @"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=LotteryDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        static SqlConnection? connection = default;
        static void Main(string[] args)
        {
            //prepare new line into database with initial jackpot (10€)
            Connect();
            var sql = ($"insert into GameDraws (Jackpot) values ({10})");
            SqlCommand cmd = new SqlCommand(sql);
            cmd.ExecuteNonQuery();
            cmd.Connection = connection;
            Disconnect();

            //wait 4 min
            Thread.Sleep(241000);

            Console.WriteLine("Démarrage d'un nouveau tirage de Loterie");
            string drawnNumbers ="";
            drawnNumbers = LaunchGameDraw(drawnNumbers);

            //record and finalize game draw:
            Connect();
            var sql2 = ($"UPDATE GameDraws SET DateTime = { DateTime.Now }, DrawnNumbers = { drawnNumbers } where[Id] = (SELECT MAX([Id]) FROM GameDraws)");
            SqlCommand cmd2 = new SqlCommand(sql2);
            cmd2.ExecuteNonQuery();
            cmd2.Connection = connection;
            Disconnect();

            // calculate ranks all in SQl

            Connect();

             // request goes here

                var sql3 = ("select * from GameDraws");


            SqlCommand cmd3 = new SqlCommand(sql3);
            cmd3.ExecuteNonQuery();
            cmd3.Connection = connection;

            Disconnect();

        }

        static void Connect()
        {
            connection =
                new SqlConnection(CHAINE_CONNEXION);

            connection.Open();
            Console.WriteLine("Connection is open");
        }
        static void Disconnect()
        {
            Console.WriteLine("Closing the connection");
            connection?.Close();

        /// <summary>
        /// Random selects 6 numbers between 1 and 49 and put them into a string
        /// </summary>
        }private static string LaunchGameDraw(string drawnNumbers)
        {
            Random rnd = new Random();
            string[]? numbersTab = new string[6];
            for (int i = 0; i < 6; i++)
            {
                do
                {
                    int number = rnd.Next(1, 50);
                    if (number < 10)
                        numbersTab[i] = "0" + number;
                    else
                        numbersTab[i] = number.ToString();
                }
                while (numbersTab.Contains(numbersTab[i]));

                drawnNumbers = String.Join(" ", numbersTab);
            }

            return drawnNumbers;
        }

        
    }
}