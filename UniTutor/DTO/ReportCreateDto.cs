using System.ComponentModel.DataAnnotations;

namespace UniTutor.DTO
{
  

        public class ReportCreateDto
        {
            public string senderMail { get; set; }
            public string receiverMail { get; set; }
            public string description { get; set; }

            public string? status { get; set; } = "Pending";
            // Additional properties as needed for creating a report
        }
    
}
