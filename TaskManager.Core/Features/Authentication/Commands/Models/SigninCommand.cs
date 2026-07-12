using MediatR;
using Project_Task_Management.Data.Helpers;
using TaskManager.Core.Bases;

namespace TaskManager.Core.Features.Authentication.Commands.Models
{
    public class SigninCommand : IRequest<Response<JwtAuthResult>>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
