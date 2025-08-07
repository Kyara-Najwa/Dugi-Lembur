using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mobile.Infrastructure;
using Mobile.Model;


namespace Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AGeneralController : ControllerBase
    {
        private readonly GeneralRepository generalRepository;
        public AGeneralController()
        {
            generalRepository = new GeneralRepository();
        }
        [HttpPost("Register")]
        public async Task<GeneralResponse> Post([FromBody] Register register)
        {
            if (ModelState.IsValid)
            {
                return await generalRepository.Register(register);
            }
            return null;
        }

        [HttpPost("Login")]
        public async Task<ResultLogin> Login([FromBody] loginMobile login)
        {
            if (ModelState.IsValid)
            {
                return await generalRepository.login(login);
            }
            return null;
        }
       
        [HttpPost("Refreshtoken")]
        public async Task<ActionResult> Refreshtoken([FromBody] refreshlogin data)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    var user = User.Claims.Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").FirstOrDefault()?.Value;
                    return Ok(await generalRepository.Refreshtoken(data, user));
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("checkToken")]
        public async Task<ActionResult> cekToken()
        {
            if (ModelState.IsValid)
            {
                return Ok();
            }
            return null;
        }
       
    }
}
