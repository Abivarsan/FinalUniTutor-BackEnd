using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniTutor.DataBase;
using UniTutor.Interface;
using UniTutor.Model;

namespace UniTutor.Repository
{
    public class ReportRepository : IReport
    {
        private readonly ApplicationDBContext _dbContext;

        public ReportRepository(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Report> GetById(int id)
        {
            var report = await _dbContext.Reports
                .Include(r => r.Tutor)
                .Include(r => r.Student)
                .FirstOrDefaultAsync(r => r._id == id);

            return report;
        }

        public async Task<Report> Create(Report report)
        {
            _dbContext.Reports.Add(report);
            await _dbContext.SaveChangesAsync();
            return report;
        }

        public async Task<List<Report>> GetAll()
        {
            var reports = await _dbContext.Reports.ToListAsync();
            return reports;
        }
    }
}
