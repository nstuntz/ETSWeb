using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Data.Entity;

namespace ETSWeb.Models
{
    public class CardModel
    {
        public List<TimeLineModel> Lines {get; set;}

        public TimeLineModel EditLine { get; set; }

        public IEnumerable<SelectListItem> Clients { get; set; }

        public IEnumerable<SelectListItem> ActivityCodes { get; set; }

        public DateTime WeekEnd { get; set; }
        public DateTime WeekStart { get; set; }

    }

    public class LineModel
    {
        public List<TimeLineModel> Lines { get; set; }

        public TimeLineModel EditLine { get; set; }

        public ClientModel Client { get; set; }

        public IEnumerable<SelectListItem> ActivityCodes { get; set; }

        public DateTime WeekEnd { get; set; }
        public DateTime WeekStart { get; set; }
        public Dictionary<int, decimal> Hours { get; set; }

        public bool IsSubmitted
        {
            get
            {
                if (Lines == null)
                {
                    return false;
                }
                else
                {
                    return Lines.Count(x => x.DateSubmitted == null) == 0 && Lines.Count > 0;
                }
            }
        }
        public bool IsApproved
        {
            get
            {
                if (Lines == null)
                {
                    return false;
                }
                else
                { return Lines.Count(x => x.DateApproved == null) == 0 && Lines.Count > 0; }
            }
        }

    }

    [Table("TimeLine")]
    public class TimeLineModel
    {

        public TimeLineModel() : this(-1, DateTime.Today)
        {           
        }

        public TimeLineModel(DateTime defaultDate)
            : this(-1, defaultDate)
        {
        }
        public TimeLineModel(int lineNum)
            : this(lineNum, DateTime.Today)
        {
        }
        public TimeLineModel(int lineNum, DateTime defaultDate)
        {
            this.Id = lineNum;
            this.LineDate = defaultDate;
            this.ActivityCode = "";
            this.ClientID = "";
        }

        [Required(ErrorMessage = "ID field is required.")]
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime LineDate { get; set; }

        [Required(ErrorMessage = "Client is required.")]
        //[RegularExpression(".+\\@.+\\..+", ErrorMessage = "Please enter a valid email address.")]
        [DisplayName("Client")]
        public string ClientID { get; set; }

        [Required(ErrorMessage = "Activity Code is required.")]
        [DisplayName("Activity")]
        public string ActivityCode { get; set; }

        [DisplayName("Pieces")]
        public int Pieces { get; set; }

        [Required(ErrorMessage = "Start Time is required.")]
        [DisplayName("Start Time")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "End Time is required.")]
        [DisplayName("End Time")]
        public TimeSpan EndTime { get; set; }

        [Required(ErrorMessage = "Hours are required.")]
        [DisplayName("Hours")]
        public decimal Hours { get; set; }

        public string UserID { get; set; }

        public DateTime? DateSubmitted { get; set; }
        public DateTime? DateApproved { get; set; }
        public int? PayRollNumber { get; set; }

        [DisplayName("Comments")]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

    }


    [Table("nam_cc")]
    public class ClientModel
    {
        public ClientModel()
        {
        }

        [Key]
        public string Name_Key { get; set; }

        public string sort_name { get; set; }
        //public string screen_name { get; set; }

    }


    [Table("ccjob")]
    public class ActivityModel
    {
        public ActivityModel()
        {
        }

        [Key]
        public string job_num { get; set; }

        public string job_desc { get; set; }
        public string name_key { get; set; }

    }



    [Table("ConsumerCoachHistory")]
    public class ConsumerCoachHistoryModel
    {
        public ConsumerCoachHistoryModel()
        {
        }

        [Key]
        public int ukey { get; set; }

        public string CoachKey { get; set; }
        public string ConsumerKey { get; set; }
        public string Active { get; set; }
        public DateTime ChangeDate { get; set; }

    }


    [Table("SupervisorCoachHistory")]
    public class SupervisorCoachHistoryModel
    {
        public SupervisorCoachHistoryModel()
        {
        }

        [Key]
        public int ukey { get; set; }
        
        public string SupervisorKey { get; set; }
        public string CoachKey { get; set; }
        public string Active { get; set; }
        public DateTime ChangeDate { get; set; }

    }


    [Table("nam_ee")]
    public class EmployeeModel
    {
        public EmployeeModel()
        {
        }

        [Key]
        public string name_key { get; set; }

        public string sort_name { get; set; }
        public string screen_nam { get; set; }
    }


    public class TimeLineDB : DbContext
    {
        public TimeLineDB() : base("ETSConnection")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<TimeLineModel> TimeLines { get; set; }
        public DbSet<ClientModel> Clients { get; set; }
        public DbSet<ActivityModel> Activities { get; set; }

        public DbSet<ConsumerCoachHistoryModel> ConsumerCoachs { get; set; }
        public DbSet<SupervisorCoachHistoryModel> SupervisorCoachs { get; set; }
        public DbSet<EmployeeModel> Employees { get; set; }
    }
}