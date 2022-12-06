using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Model
{
    public class GameDraw
    {
        public int Id { get; set; }
        public DateTime? DateTime { get; set; }
        [MinLength(17)]
        [MaxLength(17)]
        public string? DrawnNumbers { get; set; }
        public int? Jackpot { get; set; }
        
        // Navigation Property
        public List<GameSession>? GameSessions { get; set; }
    }
}
