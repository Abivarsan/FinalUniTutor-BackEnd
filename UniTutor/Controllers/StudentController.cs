using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore;
using System.Threading.Tasks;
using UniTutor.DataBase;
using UniTutor.Model;
using UniTutor.Interface;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using UniTutor.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using UniTutor.Services;
using UniTutor.DTO;

using AutoMapper;

namespace UniTutor.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        IStudent _student;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper; 
        private readonly IEmailService _emailService;
       


        public StudentController(IStudent student,IConfiguration config,IMapper mapper,IEmailService emailService)
        {
            _config = config;
            _student = student;
            _mapper = mapper;
            _emailService = emailService;
            
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateAccountAsync([FromBody] StudentRegistration studentDto)
        {
            if (ModelState.IsValid)
            {
                // Set CreatedAt to local time
                TimeZoneInfo localZone = TimeZoneInfo.FindSystemTimeZoneById("Sri Lanka Standard Time"); // Change to your local time zone
                DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, localZone);

                var student = _mapper.Map<Student>(studentDto);
                PasswordHash ph = new PasswordHash();
                student.password = ph.HashPassword(studentDto.password); // Hash the password
                student.CreatedAt = localDateTime;

                var result = _student.SignUp(student);
                if (result)
                {
                    Console.WriteLine("registration success");

                    // Send welcome email
                    var emailSubject = "Welcome to UniTutor!";
                    var emailMessage = $@"

                Dear {student.firstName},
                <br>Welcome to UniTutor! <br>We are excited to have you join our community.Here at UniTutor, we strive to provide the best educational support to help you achieve your academic goals.
                <br>If you have any questions or need assistance, feel free to reach out to our support team.
                <br>
                <br>   
                Best regards,<br>
                The UniTutor Team";

                    await _emailService.SendEmailAsync(student.email, emailSubject, emailMessage);
                    return CreatedAtAction(nameof(GetAccountById), new { id = student._id }, student);
                }
                else
                {
                    Console.WriteLine("registration failed");
                    return BadRequest(result);
                }
            }
            else
            {
                return BadRequest("ModelError");
            }
        }
        [HttpGet("details/{id}")]
        public IActionResult GetAccountById(int id)
        {
            var student = _student.GetById(id);
            if (student == null)
            {
                return NotFound();
            }
            return Ok(student);
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] Loginrequest studentLogin)
        {
            var email = studentLogin.email;
            var password = studentLogin.password;

            var result = _student.Login(email, password);
            if (!result)
            {
                return Unauthorized("Invalid email or password");
            }
            var loggedInStudent = _student.GetByMail(email);

            if (loggedInStudent.isSuspended)
            {
                return Unauthorized("Account is suspended. Please contact the administrator.");
            }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                         new Claim(ClaimTypes.Name, email),  // Email claim
                         new Claim(ClaimTypes.NameIdentifier, loggedInStudent._id.ToString()),  // Student ID claim
                         new Claim(ClaimTypes.GivenName, loggedInStudent.firstName),  // Student name claim
                         new Claim(ClaimTypes.Role, "Student")
                    }),
                    Expires = DateTime.UtcNow.AddDays(30),
                    SigningCredentials = credentials
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return Ok(new { token = tokenHandler.WriteToken(token), Id = loggedInStudent._id });
            
        }

        [HttpPut("ProfileUpdate{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentDto updateStudentDto)
        {
            if (id != updateStudentDto._id)
            {
                return BadRequest();
            }

            var student = await _student.GetByIdAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            student.firstName = updateStudentDto.firstName;
            student.lastName = updateStudentDto.lastName;
            student.email = updateStudentDto.email;
            student.phoneNumber = updateStudentDto.phoneNumber;
            student.grade = updateStudentDto.grade;
            student.address = updateStudentDto.address;

            if (!string.IsNullOrEmpty(updateStudentDto.ProfileUrl))
            {
                student.ProfileUrl = updateStudentDto.ProfileUrl;
            }

            await _student.UpdateAsync(student);

            return NoContent();
        }

        [HttpGet("{studentId}")]
        public async Task<IActionResult> GetStudentDashboardDetails(int studentId)
        {
            var studentDetails = await _student.GetStudentDashboardDetails(studentId);
            if (studentDetails == null)
            {
                return NotFound();
            }
            return Ok(studentDetails);
        }
        //[HttpPost("requesttutor")]
        //public IActionResult requesttutor([FromBody] Request request)
        //{
        //    var result = _student.CreateRequest(request);
        //    if (result)
        //    {
        //        return Ok(result);
        //    }
        //    else
        //    {
        //        return BadRequest();
        //    }
        //}

        //[HttpDelete("deleterequest")]
        //public IActionResult deleterequest([FromBody] Request request)
        //{
        //    var result = _student.DeleteRequest(request);
        //    if (result)
        //    {
        //        return Ok(result);
        //    }
        //    else
        //    {
        //        return BadRequest();
        //    }
        //}




    }
}
