﻿using System;
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
        public string? ShortGuid { get; set; }
        [MinLength(17)]
        [MaxLength(17)]
        public string? PlayedNumbers { get; set; }
        [MinLength(1)]
        [MaxLength(1)]
        public int? Rank { get; set; }   
        
        // Navigation Property
        public GameDraw GameDraw { get; set; }
    }
}
