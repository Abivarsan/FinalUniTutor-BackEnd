using Microsoft.AspNetCore.Mvc;
using UniTutor.Interface;
using UniTutor.Model;
using System;
using System.Threading.Tasks;
using UniTutor.DTO;

namespace UniTutor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReport _report;
        private readonly ITutor _tutor;
        private readonly IStudent _student;

        public ReportController(IReport report, ITutor tutor, IStudent student)
        {
            _report = report;
            _tutor = tutor;
            _student = student;
        }

        // POST api/report/create/{sendermail}/{receivermail}
        [HttpPost("create/{sendermail}/{receivermail}")]
        public async Task<ActionResult<Report>> CreateReport(string sendermail, string receivermail, [FromBody] ReportDto reportDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Determine sender
                var sender = _tutor.GetTutorByEmail(sendermail) ?? (object)_student.GetByMail(sendermail);

                // Determine receiver
                var receiver = _tutor.GetTutorByEmail(receivermail) ?? (object)_student.GetByMail(receivermail);

                // Validate sender and receiver
                if (sender == null || receiver == null)
                {
                    return NotFound("Sender or receiver not found.");
                }

                var report = new Report
                {
                    senderMail = sendermail,
                    receiverMail = receivermail,
                    description = reportDto.description,
                    date = DateTime.UtcNow
                };

                // Assign Tutor or Student to the report
                if (sender is Tutor senderTutor)
                {
                    report.tutorId = senderTutor._id;
                    report.Tutor = senderTutor;
                }
                else if (sender is Student senderStudent)
                {
                    report.tutorId = senderStudent._id;
                    report.Student = senderStudent;
                }

                var newReport = await _report.Create(report);
                return CreatedAtAction(nameof(GetReportById), new { id = newReport._id }, newReport);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Report>> GetReportById(int id)
        {
            var report = await _report.GetById(id);
            if (report == null)
            {
                return NotFound();
            }
            return report;
        }
        [HttpGet("allreport")]
        public async Task<ActionResult<Report>> GetAllReport()
        {
            var report = await _report.GetAll();
            if (report == null)
            {
                return NotFound();
            }
            return Ok(report);
        }
    }
}
