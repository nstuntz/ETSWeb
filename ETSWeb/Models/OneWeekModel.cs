using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETSWeb.Models
{
    public class OneWeekModel
    {
        public List<ClientWeek> Clients = new List<ClientWeek>();

        public IEnumerable<SelectListItem> NewClients { get; set; }

        [Required(ErrorMessage="Please Enter a Client")]
        public string NewClientNameKey { get; set; }

        public DateTime WeekEnd { get; set; }
        public DateTime WeekStart { get; set; }

        public bool IsSubmitted { get; set; }
        public bool IsApproved { get; set; }
    }

    public class ClientWeek
    {
        public ClientModel Client { get; set; }
        public Dictionary<int, decimal> Hours { get; set; }
    }
}