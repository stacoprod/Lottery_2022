using System.ComponentModel.DataAnnotations;

namespace Lottery_2022.Models
{
    public class GetResultsWithCodeViewModel
    {
        [Required]
        [MinLength(22)]
        [MaxLength(22)]
        public string ShortGuid { get; set; }
    }
}
