using System.ComponentModel.DataAnnotations;

namespace OPUS.Models
{
    public class Scoring
    {
        [Key]
        public int ID { get; set; }
        public int STCPlayerID { get; set; }
        [MaxLength(4)]
        public string Group { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
        [MaxLength(50)]
        public string Date { get; set; }
        [MaxLength(70)]
        public string Name { get; set; }
        public int? Played { get; set; }
        public int? Won { get; set; }
        [Display(Name = "% Won")]
        public int PercentWon { get; set; }
        [Display(Name = "Overall % Won")]
        public int OverallPercentWon { get; set; }
        public int? Rank { get; set; }
        public int? OpusRank { get; set; }
    }
}