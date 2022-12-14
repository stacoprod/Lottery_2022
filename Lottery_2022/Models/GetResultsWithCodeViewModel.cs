namespace Lottery_2022.Models
{
    public class GetResultsWithCodeViewModel
    {
        public DateTime? DateTime { get; set; }
        public int? Rank1Winners { get; set; }
        public int? Rank2Winners { get; set; }
        public int? Rank3Winners { get; set; }
        public double? Rank1Gain { get; set; }
        public double? Rank2Gain { get; set; }
        public double? Rank3Gain { get; set; }
        public int? NumberOfGoodNumbers { get; set; }
        public double? Gain { get; set; }
        public string[]? DrawNumbers { get; set; }
        public string[]? PlayedNumbers { get; set; }
        public string? ErrorMessage { get; set; }

        #region method
        // Allows to color the good numbers in green:
        public bool playedNumberIsGood(string number)
        {
            bool isGoodNumber = false;

            if (this.DrawNumbers.Contains(number))
                isGoodNumber = true;

            return isGoodNumber;
        }
        #endregion
    }
}
