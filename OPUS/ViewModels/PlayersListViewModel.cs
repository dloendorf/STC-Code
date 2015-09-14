using System.Collections.Generic;
using System.Web.Mvc;

namespace OPUS.ViewModels
{
    public class PlayerListViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public string PlayerId { get; set; }
        public IEnumerable<SelectListItem> Players { get; set; }
    }
}
