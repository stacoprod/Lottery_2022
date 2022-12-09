using System.ComponentModel.DataAnnotations;

namespace Lottery_2022.Models
{
    public class SessionValidationViewModel
    { 
        public string[] PlayedNumbers { get; set; }
        public string Code { get; set; }
        public DateTime DrawDateTime { get; set; }
    }
}
