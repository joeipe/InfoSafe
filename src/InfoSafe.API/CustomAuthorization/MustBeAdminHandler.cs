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

            if (userId != Guid.Parse("f47a627f-daee-4491-a1be-1a3a918e982f"))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}