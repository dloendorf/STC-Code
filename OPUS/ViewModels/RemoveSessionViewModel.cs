using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Collections.Generic;

namespace OPUS.ViewModels
{
    public class RemoveSessionViewModel
    {
        [Required]
        [Display(Name = "Session Name")]
        public string Session { get; set; }
        public string Message { get; set; }
        public bool Removed { get; set; }
        public List<SelectListItem> Sessions { get; set; }
    }
}