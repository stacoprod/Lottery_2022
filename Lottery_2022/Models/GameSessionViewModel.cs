using System.ComponentModel.DataAnnotations;

namespace Lottery_2022.Models
{
    public class GameSessionViewModel
    {
        [Required]
        [MinLength(17, ErrorMessage = "Vous n'avez pas choisi 6 numéros, veuillez compléter votre sélection")]
        [MaxLength(17, ErrorMessage = "Vous avez choisi trop de numéros, veuillez n'en choisir que 6")]
        public string PlayedNumbers { get; set; }
        [Required]
        public string ShortGuid { get; set; }
        [Required]
        public int GameDrawId { get; set; }
    }
}
