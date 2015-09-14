using System.ComponentModel.DataAnnotations;

namespace OPUS.Models
{
    public class PastOpusPlayer
    {
        [Key]
        public int PlayerID { get; set; }
        [MaxLength(4)]
        public string Group { get; set; }
        public string Season { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
        public string PlayCodes { get; set; }
        public string Name { get; set; }
        [Display(Name = "STC Rank")]
        public string STCRank { get; set; }
        [Display(Name = "Opus Factor")]
        public int Factor { get; set; }
        [Display(Name = "Weeks Played")]
        public int WeeksPlayed { get; set; }
        [Display(Name = "% Won")]
        public int OverallPercentWon { get; set; }
        [Display(Name = "Weekly OPUS Rank")]
        public int? Rank { get; set; }

        public string LastFirst
        {
            get
            {
                //int iComma = Name.IndexOf(',');
                //string sL = Name.Substring(0, iComma);
                //string sF = Name.Substring(iComma + 2);
                //return sF + " " + sL;
                return Name;
            }
        }

        public string FirstLast
        {
            get
            {
                int iComma = Name.IndexOf(',');
                string sL = Name.Substring(0, iComma);
                string sF = Name.Substring(iComma + 2);
                return sF + " " + sL;
            }
        }

        public string DisplayName
        {
            get
            {
                return FirstLast;
            }
        }

    }
}