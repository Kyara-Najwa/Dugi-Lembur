using Microsoft.AspNetCore.Mvc;
using Mobile.Services;
using Mobile.Model;
using System.Threading.Tasks;

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
            var lembur = await _lemburService.GetById(id);
            if (lembur == null)
            {
                return NotFound();
            }
            return Ok(lembur);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Lembur lembur)
        {
            if (lembur == null)
            {
                return BadRequest();
            }
            
            var newId = await _lemburService.Create(lembur);
            lembur.Id = newId;
            
            return CreatedAtAction(nameof(GetById), new { id = newId }, lembur);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Lembur lembur)
        {
            if (lembur == null || id != lembur.Id)
            {
                return BadRequest();
            }
            
            var exists = await _lemburService.GetById(id);
            if (exists == null)
            {
                return NotFound();
            }

            await _lemburService.Update(lembur);
            return NoContent();
        }
    }
}