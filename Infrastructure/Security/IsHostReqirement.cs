using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Persistence;

namespace Infrastructure.Security
{
    public class IsHostReqirement : IAuthorizationRequirement
    {

    }
    public class IsHostReqirementHandler : AuthorizationHandler
    <IsHostReqirement>
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IsHostReqirementHandler(IHttpContextAccessor httpContextAccessor, DataContext context
        )
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostReqirement requirement)
        {
            var currentUsername = _httpContextAccessor
            .HttpContext.User?.Claims?
            .SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var activityId = Guid.Parse
            (
                _httpContextAccessor.HttpContext.Request.RouteValues
                .SingleOrDefault(x => x.Key =="id").Value.ToString()
            );

            var activity = _context.Activities.FindAsync(activityId).Result;

            var host = activity.UserActivity.FirstOrDefault(x=>x.IsHost);

            if(host?.AppUser?.UserName == currentUsername)
                    context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}