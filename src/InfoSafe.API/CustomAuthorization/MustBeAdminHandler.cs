using InfoSafe.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace InfoSafe.API.CustomAuthorization
{
    public class MustBeAdminHandler : AuthorizationHandler<MustBeAdminRequirement>
    {
        private readonly UserService _userService;

        public MustBeAdminHandler(
            UserService userService)
        {
            _userService = userService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MustBeAdminRequirement requirement)
        {
            var userId = _userService.UserId;

            if (userId != Guid.Parse("129587be-e15b-4c74-a02c-8f39f4b209e8"))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}