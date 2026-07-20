using Microsoft.AspNetCore.Mvc;
using Project_Task_Management_API.Base;
using TaskManager.Core.Features.Authentication.Commands.Models;
using static Project_Task_Management.Data.AppMetaData.Router;

namespace Project_Task_Management_API.Controllers
{

    [ApiController]
    public class AuthenticationController : AppControllerBase
    {
        [HttpPost(AuthenticationRouting.SignIn)]
        public async Task<IActionResult> SignIn([FromBody] SigninCommand command)
        {
            var response = await Mediator.Send(command);

            return NewResult(response);
        }

        [HttpPost(AuthenticationRouting.RefreshToken)]
        public async Task<IActionResult> RefreshToken([FromForm] RefreshTokenCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

    }
}
