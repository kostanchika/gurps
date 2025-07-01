using Hangfire.Dashboard;
using System.Net;

namespace UsersService.Presentation.Filters
{
    public class AllowLocalAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var remoteIp = context.GetHttpContext().Connection.RemoteIpAddress;

            return remoteIp != null &&
                (
                    IPAddress.IsLoopback(remoteIp) ||
                    remoteIp.ToString().StartsWith("172.") ||
                    remoteIp.ToString().Contains(":172")
                );
        }
    }
}
