using System.ComponentModel.DataAnnotations;

namespace Lottery_2022.Models
{
    public class GetResultsWithCodeViewModel
    {
        public DateTime DateTime { get; set; }
        public int Rank1Winners { get; set; }
        public int Rank2Winners { get; set; }
        public int Rank3Winners { get; set; }
        public int Rank1Gain { get; set; }
        public int Rank2Gain { get; set; }
        public int Rank3Gain { get; set; }
        public int NumberOfGoodNumbers { get; set; }
        public int Gain { get; set; }
        public string[] DrawNumbers { get; set; }
        public string[] PlayedNumbers { get; set; }
    }
}
