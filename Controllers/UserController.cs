using CongEspVilaGuilhermeApi.AppCore.Models;
using CongEspVilaGuilhermeApi.Domain.Models;
using CongEspVilaGuilhermeApi.Domain.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;
using System.Net;
using System.Security.Principal;

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
            { TransactionEntityStatus.NotUpdated, 404 }
        };

        public UserController(UserUseCases useCases)
        {
            this.useCases = useCases;
        }

        [HttpPost]
        public async Task<ObjectResult> Post(NewAccount newAccount)
        {
            return MapResponse(newAccount.UserName, await useCases.CreateNewUser(newAccount));
        }

        private ObjectResult MapResponse(string UserName, TransactionEntityStatus status)
        {
            var statusCode = mapStatusCodes[status];

            var response = new
            {
                userName = UserName,
                status = status.GetDisplayName(),
                statusCode
            };

            return StatusCode(statusCode, response);
        }

        [HttpGet("{userName}/role/{role}")]
        [Authorize(Roles = "Admin")]
        public Task AddRole(string userName, string role)
        {
            return useCases.AddRole(userName, role);       
        }

        [HttpGet("{username}/reset-password")]
        public Task StartResetPassword(string userName)
        {
            return useCases.StartResetPassword(userName);
        }

        [HttpPost("{username}/reset-password")]
        public async Task<ObjectResult> FinishResetPassword(string userName, [FromBody] ResetPasswordBody body)
        {
            return MapResponse(userName, await useCases.FinishResetPassword(userName, body.ResetPasswordId, body.NewPassword));
        }
    }
}
