using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace MeetingScheduler.Identity
{
    public class AdUserMiddleware
    {
        private readonly RequestDelegate next;

        public AdUserMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context,  IUserProvider userProvider, IConfiguration config)
        {
            //if (!(userProvider.Initialized) && context.User.Identity.IsAuthenticated)
            if (context.User.Identity.IsAuthenticated)
            {
                await userProvider.Create(context);
            }
            if (next!=null) { 
                await next(context);
            }
        }
    }
}