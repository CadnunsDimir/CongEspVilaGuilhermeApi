using CongEspVilaGuilhermeApi.Core.Models;
using CongEspVilaGuilhermeApi.Domain.Entities;
using CongEspVilaGuilhermeApi.Domain.Services;
using CongEspVilaGuilhermeApi.Domain.UseCases;
using CongEspVilaGuilhermeApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CongEspVilaGuilhermeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly UserUseCases useCases;

        public TokenController(UserUseCases useCases)
        {
            this.useCases = useCases;
        }

        [HttpPost]
        public async Task<IResult> PostAsync([FromBody] LoginBody value)
        {
            var isValidated = await useCases.PasswordIsValid(value.Login, value.Password);

            if (isValidated)
            {
                var token = await useCases.GenerateToken(value.Login);
                return TypedResults.Ok(new
                {
                    token
                });                
            }

            return TypedResults.NotFound(new { message = "Usuário ou senha inválidos" });

        }

        [HttpGet("validate")]
        [Authorize]
        public string ValidateToken()
        {
            return "Authenticated 20-04";
        }
    }
}
