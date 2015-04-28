using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETSWeb.Models
{
    public class WeeksModel
    {
        public List<WeekModel> Weeks = new List<WeekModel>();
    }

    
    public class WeekModel
    {
        public DateTime WeekEnd { get; set; }
        public DateTime WeekStart { get; set; }
        public int TotalClients { get; set; }

        [DisplayFormat(DataFormatString="{0:F}")]
        public decimal TotalHours { get; set; }


        [DisplayFormat(DataFormatString = "{0:F0}")]
        public decimal TotalUnits { get { return TotalHours*4; }  }
    }
}