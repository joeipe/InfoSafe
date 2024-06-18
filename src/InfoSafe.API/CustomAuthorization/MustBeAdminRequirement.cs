using Microsoft.AspNetCore.Authorization;

namespace InfoSafe.API.CustomAuthorization
{
    public class MustBeAdminRequirement : IAuthorizationRequirement
    {
        public MustBeAdminRequirement()
        {
        }
    }
}