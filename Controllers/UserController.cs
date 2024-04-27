using CongEspVilaGuilhermeApi.Domain.Models;
using CongEspVilaGuilhermeApi.Domain.UseCases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;
using System.Net;

namespace CongEspVilaGuilhermeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserUseCases useCases;
        Dictionary<TransactionEntityStatus, int> mapStatusCodes = new Dictionary<TransactionEntityStatus, int>
        {
            { TransactionEntityStatus.CreatedOrUpdated, 200 },
            { TransactionEntityStatus.UserNameAlreadyExists, 422 },
        };

        public UserController(UserUseCases useCases)
        {
            this.useCases = useCases;
        }

        [HttpPost]
        public async Task<ObjectResult> Post(NewAccount newAccount)
        {

            var status = await useCases.CreateNewUser(newAccount);
            var statusCode = mapStatusCodes[status];

            var response = new {
                userName = newAccount.UserName,
                status = status.GetDisplayName(),
                statusCode
            };

            return StatusCode(statusCode, response);
        }
    }
}
