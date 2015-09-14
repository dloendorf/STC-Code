using System.ComponentModel.DataAnnotations;

namespace OPUS.ViewModels
{
    public class SettingsViewModel
    {
        public int ID { get; set; }
        [Display(Name = "STC Coordinator Email")]
        public string STCEmailAddress { get; set; }
        [Display(Name = "OPUS Monitor Email")]
        public string OpusCoordinatorEmailAddress { get; set; }
    }
}