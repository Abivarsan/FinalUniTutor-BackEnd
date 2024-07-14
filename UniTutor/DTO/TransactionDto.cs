namespace UniTutor.DTO
{
    public class TransactionDto
    {
        public DateTime TransactionTime { get; set; }
        public string Description { get; set; }
        public int Coins { get; set; }
        public int tutorId { get; set; }
    }
}
