using UniTutor.Model;

namespace UniTutor.Interface
{
    public interface IReport
    {
        Task<Report> Create(Report report);
        Task<Report> GetById(int id);
        public  Task<List<Report>> GetAll();
    }
}
