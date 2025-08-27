using Microsoft.AspNetCore.Mvc;
using Mobile.Services;
using Mobile.Model;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LemburController : ControllerBase
    {
        private readonly LemburService _lemburService;

        public LemburController()
        {
            _lemburService = new LemburService();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var lemburs = await _lemburService.GetAll();
            return Ok(lemburs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var lembur = await _lemburService.GetLemburWithEmployeeDetails(id); //untuk dapatin detail employee
            if (lembur == null)
            {
                return NotFound();
            }
            return Ok(lembur);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLemburRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }
            
            //untuk validasi model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            //ini untuk mendapatkan email karyawan dri token Jwt
            var employeeEmailClaim = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            
            //kalau misal ngga ditemukan, coba ambil dari claim lain
            //yang umum digunakan untuk email
            if (string.IsNullOrEmpty(employeeEmailClaim))
            {
                employeeEmailClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            }
            
            //kalau emailnya kosong, return error
            //untuk memastikan bisa dapetin employeeId dari email
            if (string.IsNullOrEmpty(employeeEmailClaim))
            {
                return BadRequest("Unable to extract employee email from token");
            }
            
            //untuk mendapatkan employeeId berdasarkan email
            var employeeId = await _lemburService.GetEmployeeIdByEmail(employeeEmailClaim);
            if (employeeId <= 0)
            {
                return BadRequest("Unable to find employee with email: " + employeeEmailClaim);
            }
            
            //untuk buat lembur baru
            var lembur = new Lembur
            {
                EmployeeId = employeeId,
                TanggalLembur = request.TanggalLembur,
                Durasi = request.Durasi,
                Alasan = request.Alasan,
                Status = LemburStatus.Pending, //default status
                DateCreated = DateTime.UtcNow //set tanggal dibuat
            };
            
            var newId = await _lemburService.Create(lembur);
            lembur.Id = newId;
            
            return CreatedAtAction(nameof(GetById), new { id = newId }, lembur);
        }
        [HttpPut("approval/{id}")]
        public async Task<IActionResult> Approve(int id, [FromBody] Lembur lembur)
        {
            if (lembur == null)
            {
                return BadRequest();
            }
            
            //ini untuk mendapatkan email karyawan dri token Jwt
            var employeeEmailClaim = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            
            //kalau misal ngga ditemukan, coba ambil dari claim lain
            //yang umum digunakan untuk email
            if (string.IsNullOrEmpty(employeeEmailClaim))
            {
                employeeEmailClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            }
            
            //kalau emailnya kosong, return error
            //untuk memastikan bisa dapetin employeeId dari email
            if (string.IsNullOrEmpty(employeeEmailClaim))
            {
                return BadRequest("Unable to extract employee email from token");
            }
            
            //untuk mendapatkan employeeId berdasarkan email
            var (employeeId, employeeRoleId) = await _lemburService.GetEmployeeInfoByEmail(employeeEmailClaim);
            if (employeeId <= 0)
            {
                return BadRequest("Unable to find employee with email: " + employeeEmailClaim);
            }
            
            //untuk cek apakah karyawan punya role admin ( admin = 1 )
            if (employeeRoleId != 1)
            {
                return BadRequest("Only administrators can update overtime request status");
            }
            
            lembur.Id = id;
            
            var exists = await _lemburService.GetById(id);
            if (exists == null)
            {
                return NotFound();
            }

            //yang gabole diubah
            lembur.EmployeeId = exists.EmployeeId;
            lembur.TanggalLembur = exists.TanggalLembur;
            lembur.Durasi = exists.Durasi;
            lembur.Alasan = exists.Alasan;
            lembur.DateCreated = exists.DateCreated;
            
            //untuk set status lembur berdasarkan nilai status yang dikirim
            var currentTime = DateTime.UtcNow;
            
            switch (lembur.Status)
            {
                case LemburStatus.Approved:
                    lembur.ApprovedBy = employeeId;
                    lembur.ApprovedAt = currentTime;
                    lembur.RejectedBy = null;
                    lembur.RejectedAt = null;
                    break;
                    
                case LemburStatus.Rejected:
                    lembur.RejectedBy = employeeId;
                    lembur.RejectedAt = currentTime;
                    lembur.ApprovedBy = null;
                    lembur.ApprovedAt = null;
                    break;
                    
                default:
                    //untuk status pending atau lainnya, clear both approved dan rejected fields
                    lembur.ApprovedBy = null;
                    lembur.ApprovedAt = null;
                    lembur.RejectedBy = null;
                    lembur.RejectedAt = null;
                    break;
            }
            
            await _lemburService.Update(lembur);
            return NoContent();
        }

        
        [HttpPut("reject/{id}")]
        public async Task<IActionResult> Reject(int id, [FromBody] RejectLemburRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }
            
            //ini untuk mendapatkan email karyawan dri token Jwt
            var employeeEmailClaim = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            
            //kalau misal ngga ditemukan, coba ambil dari claim lain
            //yang umum digunakan untuk email
            if (string.IsNullOrEmpty(employeeEmailClaim))
            {
                employeeEmailClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            }
            
            //kalau emailnya kosong, return error
            //untuk memastikan bisa dapetin employeeId dari email
            if (string.IsNullOrEmpty(employeeEmailClaim))
            {
                return BadRequest("Unable to extract employee email from token");
            }
            
            //untuk mendapatkan employeeId berdasarkan email
            var (employeeId, employeeRoleId) = await _lemburService.GetEmployeeInfoByEmail(employeeEmailClaim);
            if (employeeId <= 0)
            {
                return BadRequest("Unable to find employee with email: " + employeeEmailClaim);
            }
            
            //untuk cek apakah karyawan punya role admin ( admin = 1 ) testes
            if (employeeRoleId != 1)
            {
                return BadRequest("Only administrators can reject overtime requests");
            }
            
            var exists = await _lemburService.GetById(id);
            if (exists == null)
            {
                return NotFound();
            }
            
            // membuat lembur dengan alasan penolakannya
            var lembur = new Lembur
            {
                Id = id,
                EmployeeId = exists.EmployeeId,
                TanggalLembur = exists.TanggalLembur,
                Durasi = exists.Durasi,
                Alasan = exists.Alasan,
                DateCreated = exists.DateCreated,
                Status = LemburStatus.Rejected,
                RejectedBy = employeeId,
                RejectedAt = DateTime.UtcNow,
                RejectReason = request.RejectReason
            };
            
            await _lemburService.Update(lembur);
            return NoContent();
        }
        
        [HttpPost("list")]
        public async Task<IActionResult> GetFilteredLembur([FromBody] LemburFilterRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request cannot be null");
            }
            
            //untuk validasi model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            //untuk memastikan request.EmployeeId dan request.Status tidak null
            Console.WriteLine($"Request - EmployeeId: {request.EmployeeId} (Type: {request.EmployeeId.GetType()})");
            Console.WriteLine($"Request - Status: {request.Status} (Type: {request.Status.GetType()})");
            Console.WriteLine($"Request - CompanyId: {request.CompanyId} (Type: {request.CompanyId.GetType()})");
            
            var result = await _lemburService.GetFilteredLembur(request);
            return Ok(result);
        }
    }
}
