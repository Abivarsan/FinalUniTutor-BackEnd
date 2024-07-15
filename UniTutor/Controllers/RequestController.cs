﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Xml.Linq;
using UniTutor.DTO;
using UniTutor.Interface;
using UniTutor.Model;
using UniTutor.Repository;

namespace UniTutor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly IRequest _request;
        private readonly ITutor _tutor;
        private readonly IConfiguration _config;
        private readonly IStudent _student;
        private readonly IEmailService _emailService;


        public RequestController(IConfiguration config, IRequest request, ITutor tutor,IStudent student)
        {
            _request = request;
            _tutor = tutor;
            _config = config;
            _student = student;
        }

       
       // POST: api/SubjectRequests/request
        [HttpPost("request")]
        public async Task<ActionResult<RequestDto>> CreateSubjectRequest([FromBody] RequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var newRequest = await _request.Create(request);
                return CreatedAtAction(nameof(GetSubjectRequestById), new { id = newRequest.subjectId }, newRequest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

       // GET: api/SubjectRequests/student/{id}
        [HttpGet("student/{id}")]
        public async Task<ActionResult<IEnumerable<Request>>> GetSubjectRequestsByStudentId(int id)
        {
            try
            {
                var requests = await _request.GetByStudentId(id);
                return Ok(requests);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        //get the detils by id
        [HttpGet("tutor/{id}")]
        public async Task<ActionResult<IEnumerable<Request>>> GetSubjectRequestsByTutorId(int id)
        {
            var requests = await _request.GetByTutorId(id);

            var result = requests.Select(r => new 
            {
                r._id,
                subjectId    = new
                {
                    r.Subject._id,
                    r.Subject.title,
                    r.Subject.coverImage
                },
                studentId = new
                {
                    r.Student._id,
                    r.Student.firstName,
                    r.Student.email,
                    r.Student.ProfileUrl,
                    r.Student.phoneNumber, 
                    r.Student.district,
                    r.Student.schoolName,
                    r.Student.grade
                }, 
                r.tutorId,
                r.studentEmail,
                r.status,
                r.timestamp,
            }).ToList();

            return Ok(result);
        }
        //get the list of acceped request by tutor id in repositorymethode
        [HttpGet("tutor/{id}/accepted")]
        public async Task<ActionResult<IEnumerable<Request>>> GetAcceptedRequestsByTutorId(int id)
        {
            var requests = await _request.GetAcceptedRequestsByTutorId(id);
            var result = requests.Select(r => new
            {
                r._id,
                subjectId = new
                {
                    r.Subject._id,
                    r.Subject.title,
                    r.Subject.coverImage
                },
                studentId = new
                {
                    r.Student._id,
                    r.Student.firstName,
                    r.Student.lastName,
                    r.Student.email,
                    r.Student.phoneNumber,
                    r.Student.ProfileUrl,
                    r.Student.district,
                    r.Student.schoolName,
                    r.Student.grade
                    
                },
                r.tutorId,
                r.studentEmail,
                r.status,
                r.timestamp,
            }).ToList();

            return Ok(result);
        }
        //get accepted request list by student list 
        [HttpGet("student/{id}/accepted")]
        public async Task<ActionResult<IEnumerable<Request>>> GetAcceptedRequestsByStudentId(int id)
        {
            var requests = await _request.GetAcceptedRequestsByStudentId(id);
            var result = requests.Select(r => new
            {
                r._id,
                subjectId = new
                {
                    r.Subject._id,
                    r.Subject.title,
                    r.Subject.description
                },
                TutorId = new
                {
                    r.Tutor._id,
                    r.Tutor.firstName,
                    r.Tutor.lastName,
                    r.Tutor.ProfileUrl,
                    r.Tutor.district,
                    r.Tutor.universityMail,
                    r.Tutor.phoneNumber
                    
                }
            }).ToList();

            return Ok(result);
        }
        //get all request  pending and rejected by student id
        [HttpGet("student/{id}/reject/pending")]
        public async Task<ActionResult<IEnumerable<Request>>> GetAllRequestsByStudentId(int id)
        {
            var requests = await _request.GetAllRequestsByStudentId(id);
            var result = requests.Select(r => new
            {
                r._id,
                subjectId = new
                {
                    r.Subject._id,
                    r.Subject.title,
                    r.Subject.coverImage
                },
                tutorId = new
                {
                    r.Tutor._id,
                    r.Tutor.firstName,
                    r.Tutor.lastName,
                    r.Tutor.ProfileUrl,

                },
                r.status,
                r.timestamp,
            }).ToList();

            return Ok(result);
        }









        // DELETE: api/SubjectRequests/request/{id}
        [HttpDelete("request/{id}")]
        public async Task<ActionResult<Request>> DeleteSubjectRequest(int id)
        {
            try
            {
                var request = await _request.Delete(id);
                if (request == null)
                {
                    return NotFound();
                }

                return Ok(request);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PUT: api/SubjectRequests/request/{id}
        //[HttpPut("request/{id}")]
        //public async Task<IActionResult> UpdateRequestStatus(int id, [FromBody] RequestStatues StatusDto)
        //{
        //    if (string.IsNullOrEmpty(StatusDto.status))
        //    {
        //        return BadRequest("Status is required");
        //    }

        //    try
        //    {
        //        var updatedRequest = await _request.UpdateRequestStatus(id, StatusDto.status);
        //        if (updatedRequest == null)
        //        {
        //            return NotFound();
        //        }
        //        return Ok(updatedRequest);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = ex.Message });
        //    }
        //}
        [HttpPut("request/{id}")]
        public async Task<IActionResult> UpdateRequestStatus(int id, [FromBody] RequestStatues statusDto)
        {
            if (string.IsNullOrEmpty(statusDto.status))
            {
                return BadRequest("Status is required");
            }

            try
            {
                var updatedRequest = await _request.UpdateRequestStatus(id, statusDto.status);
                if (updatedRequest == null)
                {
                    return NotFound();
                }

                if (statusDto.status == "ACCEPTED")
                {
                    var studentId = updatedRequest.studentId; // Assuming you have the StudentId in the updatedRequest
                    var student = await _student.GetByIdAsync(studentId); // Get the student information
                    if (student == null)
                    {
                        return NotFound("Student not found");
                    }
                    // Send welcome email
                    var emailSubject = "Welcome to UniTutor!";
                    var emailMessage = $@"$Dear {student.firstName},< br />< br /> Your request has been accepted.< br />< br /> Regards,< br /> Your Team";
                    await _emailService.SendEmailAsync(student.email, emailSubject, emailMessage);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }




        // GET: api/SubjectRequests/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Request>> GetSubjectRequestById(int id)
        {
            var request = await _request.GetById(id);

            if (request == null)
            {
                return NotFound();
            }

            return Ok(request);


        }
        
        [HttpGet("{studentId}/mypendingsubjectscount")]
        public async Task<IActionResult> GetMySubjectsCount(int studentId)
        {
            var count = await _request.GetMySubjectsCount(studentId);
            return Ok(count);
        }

        [HttpGet("{studentId}/acceptedrequestscount")]
        public async Task<IActionResult> GetAcceptedRequestsCount(int studentId)
        {
            var count = await _request.GetAcceptedRequestsCount(studentId);
            return Ok(count);
        }

        [HttpGet("{studentId}/rejectedrequestscount")]
        public async Task<IActionResult> GetRejectedRequestsCount(int studentId)
        {
            var count = await _request.GetRejectedRequestsCount(studentId);
            return Ok(count);

        }

       

        [HttpGet("{tutorId}/mystudentcount")]
        public async Task<IActionResult> GetMyStudentCount(int tutorId)
        {
            var count = await _request.GetMyStudentCount(tutorId);
            return Ok(count);
        }

        // Get the count of pending requests
        [HttpGet("{tutorId}/pendingrequestcount")]
        public async Task<IActionResult> GetRequestStudentCount(int tutorId)
        {
            var count = await _request.GetAllRequestscount(tutorId);
            return Ok(count);
        }


    }
}
