using System.ComponentModel.DataAnnotations;

namespace Lottery_2022.Models
{
    public class SessionSummaryViewModel
    {
        
        public string PlayedNumbers { get; set; }
        public string ShortGuid { get; set; }
        public DateTime DateTime { get; set; }
    }
}
