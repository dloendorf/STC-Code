using System.ComponentModel.DataAnnotations;

namespace OPUS.Models
{
    public class OPUSPlayerInfoList
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string STCRank { get; set; }
        public int Factor { get; set; }
        [Display(Name = "% Won Overall")]
        public int OverallPercentWon { get; set; }
        [Display(Name = "Weekly OPUS Rank")]
        public int? Rank { get; set; }
        public string NameRank
        {
            get
            {
                return Name + " (" + STCRank + ")";
            }
        }
    }
}