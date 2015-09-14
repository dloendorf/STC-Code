using System.ComponentModel.DataAnnotations;
namespace OPUS.ViewModels
{
    public class CloseSessionViewModel
    {
        [Required]
        [RegularExpression(@"^([A-Za-z]+ (20)\d\d)$", ErrorMessage = "Of the form Spring 2015")]
        [Display(Name = "Session Name")]
        public string Name { get; set; }
        public string Message { get; set; }
        public bool Closed { get; set; }
    }
}