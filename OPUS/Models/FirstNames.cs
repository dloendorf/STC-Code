using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPUS.Models
{
    public class FirstNames
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string FirstName { get; set; }

        public bool IsFemale()
        {
            if (Code.Equals("FO"))
                return true;
            else
                return false;
        }

        public bool IsMale()
        {
            if (Code.Equals("MO"))
                return true;
            else
                return false;
        }
    }
}