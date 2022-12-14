using Microsoft.EntityFrameworkCore.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.Model
{
    public class GameDraw
    {
        // Id is auto-incremented with each new draw row creation
        public int Id { get; set; }
        // Datetime.Now is written as soon as the draw has finished
        public DateTime? DateTime { get; set; }
        [MinLength(17)]
        [MaxLength(17)]
        // The 6 Drawn numbers are written as soon as the draw has finished (format "XX XX XX XX XX XX")
        public string? DrawnNumbers { get; set; }
        // Jackpot is initialized with a value of 10 with each new draw row creation
        public int? Jackpot { get; set; }

        // Navigation Property
        public List<GameSession>? GameSessions { get; set; }
    }
}
