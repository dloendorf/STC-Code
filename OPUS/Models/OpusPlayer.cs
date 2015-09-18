using System.ComponentModel.DataAnnotations;

namespace OPUS.Models
{
    public class OpusPlayer
    {
        [Key]
        public int PlayerID { get; set; }
        [MaxLength(4)]
        [Display(Name = "Group")]
        public string Group { get; set; }
        [Display(Name = "First Name")]
        public string First { get; set; }
        [Display(Name = "Last Name")]
        public string Last { get; set; }
        [Display(Name = "Play Codes")]
        public string PlayCodes { get; set; }
        public string Name { get; set; }

        [Display(Name = "STC Rank")]
        [RegularExpression(@"^(A|B\+|B|C\+|C)$", ErrorMessage = "Ranking must be A, B+, B, C+, C")]
        public string STCRank { get; set; }

        [RegularExpression(@"^([0-9]|10)$", ErrorMessage = "Factor must be 10 to 0 where 10 is high and 0 is low")]
        [Display(Name = "Opus Factor")]
        public int Factor { get; set; }

        [Display(Name = "% Won")]
        public int OverallPercentWon { get; set; }
        [Display(Name = "Weekly OPUS Rank")]
        public int? Rank { get; set; }

        public string NameRank
        {
            get
            {
                return First + " " + Last + " (" + STCRank + ")";
            }
        }
    }
}