using MediatR;
using Microsoft.AspNetCore.Identity;
using Project_Task_Management.Data.Entities.Identity;
using Project_Task_Management.Data.Helpers;
using TaskManager.Core.Bases;
using TaskManager.Core.Features.Authentication.Commands.Models;
using TaskManager.Service.Abstracts;

namespace TaskManager.Core.Features.Authentication.Commands.Handlers
{
    public class AuthenticationCommandHandler : ResponseHandler,
        IRequestHandler<SigninCommand, Response<JwtAuthResult>>
    {
        #region Fields
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAuthenticationService _authenticationService;
        #endregion

        #region Constructors
        public AuthenticationCommandHandler(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
           IAuthenticationService authenticationService

            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authenticationService = authenticationService;

        }
        #endregion

        #region Handle Functions
        public async Task<Response<JwtAuthResult>> Handle(SigninCommand request, CancellationToken cancellationToken)
        {

            // 1. Check if the user exists by username
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                return BadRequest<JwtAuthResult>("User Name Is Not Exist");
            }

            // 2. Validate the password using SignInManager        
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return BadRequest<JwtAuthResult>("Password Not Correct");
            }

            // 3 Generate your JWT Token 
            var AccessToken = await _authenticationService.GetJWTToken(user);
            // 4. Return successful response using your base ResponseHandler method
            if (AccessToken == null)
            {
                return BadRequest<JwtAuthResult>("Token Not Generated");
            }
            return Success(AccessToken);

            #endregion
        }
    }
}

