using System.ComponentModel.DataAnnotations;

namespace OPUS.Models
{
    public class PastCourtAssignment
    {
        [Key]
        public int ID { get; set; }
        [MaxLength(4)]
        public string Group { get; set; }
        public string PlayCode { get; set; }
        public string Season { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public int? Court { get; set; }
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public string Player3 { get; set; }
        public string Player4 { get; set; }
        public int Player1ID { get; set; }
        public int Player2ID { get; set; }
        public int Player3ID { get; set; }
        public int Player4ID { get; set; }
    }
}