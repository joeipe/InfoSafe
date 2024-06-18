namespace InfoSafe.API.Services
{
    public class UserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        //public Guid UserId => Guid.Parse("8A0B6FFF-ACA4-4B53-B913-47A3DAD028A6");
        public Guid UserId => Guid.Parse(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value);
        public object Claims => (from c in _httpContextAccessor.HttpContext.User.Claims select new { c.Type, c.Value }).ToList();
    }
}
