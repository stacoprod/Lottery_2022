using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Model
{
    public class GameSession
    {
        public int Id { get; set; }
        [MinLength(22)]
        [MaxLength(22)]
        public string ShortGuid { get; set; }
        [MinLength(17)]
        [MaxLength(17)]
        public string PlayedNumbers { get; set; }
        public GameDraw GameDraw { get; set; }
        public int GameDrawId { get; set; }
    }
}
