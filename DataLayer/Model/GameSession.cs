using System.ComponentModel.DataAnnotations;

namespace DataLayer.Model
{
    public class GameSession
    {
        // Id is auto-incremented with each new session row creation
        public int Id { get; set; }
        [MinLength(22)]
        [MaxLength(22)]
        // The short guid is written as soon as the session has been validated
        public string? ShortGuid { get; set; }
        [MinLength(17)]
        [MaxLength(17)]
        // The 6 Played numbers are written as soon as the session has been validated (format "XX XX XX XX XX XX")
        public string? PlayedNumbers { get; set; }
        //Rank is written as soon as the related drawn has been finished (0 = 0 good number, 1 = 6 good numbers, 2 = 5 good numbers...6 = 1 good number)
        [MinLength(1)]
        [MaxLength(1)]
        public int? Rank { get; set; }   
        
        // Navigation Properties
        public int? GameDrawId { get; set; } 
        public GameDraw? GameDraw { get; set; }
    }
}
