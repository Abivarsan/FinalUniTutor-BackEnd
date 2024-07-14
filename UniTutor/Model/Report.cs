using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniTutor.Model
{
    public class Report
    {
        // Report model with senderId, receiverId, discription, and date properties here sender and resever can be tutor or student
        [Key]
        public int _id { get; set; }
        [Required]
        public string senderMail { get; set; }
        [Required]
        public string receiverMail { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        public DateTime date { get; set; }


    
        [ForeignKey("Tutor")]
        public int? tutorId { get; set; }
        public virtual Tutor Tutor { get; set; }

        [ForeignKey("Student")]
        public int? studentId { get; set; }
        public virtual Student Student { get; set; }






    }
}
