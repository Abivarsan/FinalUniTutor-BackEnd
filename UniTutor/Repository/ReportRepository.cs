using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UniTutor.DataBase;
using UniTutor.Interface;
using UniTutor.Model;

namespace UniTutor.Repository
{
    public class ReportRepository: IReport
    {
        private ApplicationDBContext _DBcontext;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public ReportRepository(ApplicationDBContext DBcontext, IConfiguration config, IMapper mapper)
        {
            _DBcontext = DBcontext;
            _config = config;
            _mapper = mapper;

        }
        //get report by id
        public async Task<Report> GetById(int id)
        {
            var report = await _DBcontext.Reports.FindAsync(id);
            return report;
        }
        //create report
        public async Task<Report> Create(Report report)
        {
            _DBcontext.Reports.Add(report);
            await _DBcontext.SaveChangesAsync();
            return report;
        }
        //get all reports
        public async Task<List<Report>> GetAll()
        {
            var reports = await _DBcontext.Reports.ToListAsync();
            return reports;
        }

    }
}
