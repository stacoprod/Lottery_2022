using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Model
{
    public class GameDraw
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string WinningNumbers { get; set; } //DrawnNumbers à la prochaine migration
        public int Jackpot { get; set; }
        public List<GameSession>? GameSessions { get; set; }
    }
}
