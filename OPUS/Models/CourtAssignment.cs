using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OPUS.Models
{
    public class CourtAssignment
    {
        [Key]
        public int ID { get; set; }
        [MaxLength(4)]
        public string Group { get; set; }
        public string PlayCode { get; set; }
        public string defaultDate = "";

        [Required]
        [RegularExpression(@"^(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d$", ErrorMessage = "Date must be in format mm/dd/yyyy")]
        public string Date
        {
            get
            {
                return defaultDate;
            }
            set
            {
                defaultDate = value;
            }
        }

        [Required]
        [RegularExpression(@"^([0-9]|1[0-8])$", ErrorMessage = "Court must be between 1 and 18")]
        public int? Court { get; set; }

        [Required (ErrorMessage = "Please select early or late.")]
        public string Time { get; set; }

        [MaxLength(50)]
        [Display(Name = "Player 1")]
        public string Player1 { get; set; }

        [MaxLength(50)]
        [Display(Name = "Player 2")]
        public string Player2 { get; set; }

        [MaxLength(50)]
        [Display(Name = "Player 3")]
        public string Player3 { get; set; }

        [MaxLength(50)]
        [Display(Name = "Player 4")]
        public string Player4 { get; set; }

        [Required (ErrorMessage ="Player1 Name Required")]
        public int Player1ID { get; set; }

        [Required(ErrorMessage = "Player2 Name Required")]
        public int Player2ID { get; set; }

        [Required(ErrorMessage = "Player3 Name Required")]
        public int Player3ID { get; set; }

        [Required(ErrorMessage = "Player4 Name Required")]
        public int Player4ID { get; set; }

        //public string GetPlayerName(string player)
        //{
        //    //Remove the STC Rank from back of name for emailing and printing
        //    int iFLength = player.IndexOf(" ");
        //    string sFirst = player.Substring(0, iFLength);
        //    int iLength = player.IndexOf("(") - iFLength - 2;
        //    string sLast = player.Substring(iFLength + 1, iLength);
        //    return sFirst + " " + sLast;
        //}

        public string Player1Name
        {
            get
            {
                return Player1.Substring(0, Player1.IndexOf('(')-1);
            }
        }

        public string Player2Name
        {
            get
            {
                return Player2.Substring(0, Player2.IndexOf('(') - 1);
            }
        }

        public string Player3Name
        {
            get
            {
                return Player3.Substring(0, Player3.IndexOf('(') - 1);
            }
        }

        public string Player4Name
        {
            get
            {
                return Player4.Substring(0, Player4.IndexOf('(') - 1);
            }
        }

    }
}