using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandeTravel.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
        public string FullName { get; set; }
        public DateTime Date { get; set; }
        public int Rating { get; set; }
        public string Content { get; set; }

        //Relationships
        public int PackageId { get; set; }
        //Association
        public Package Package { get; set; }

    }
}
