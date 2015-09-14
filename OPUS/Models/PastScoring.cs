using System.ComponentModel.DataAnnotations;

namespace OPUS.Models
{
    public class PastScoring
    {
        [Key]
        public int ID { get; set; }
        [MaxLength(4)]
        public string Group { get; set; }
        public string Season { get; set; }
        public string Date { get; set; }
        public string Name { get; set; }
        public int? Played { get; set; }
        public int? Won { get; set; }
        public int PercentWon { get; set; }
        public int OverallPercentWon { get; set; }
        public int? Rank { get; set; }
        public int? OpusRank { get; set; }
    }
}