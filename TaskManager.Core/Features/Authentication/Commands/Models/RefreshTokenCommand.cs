using MediatR;
using Project_Task_Management.Data.Helpers;
using TaskManager.Core.Bases;

namespace TaskManager.Core.Features.Authentication.Commands.Models
{
    public class RefreshTokenCommand : IRequest<Response<JwtAuthResult>>
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
