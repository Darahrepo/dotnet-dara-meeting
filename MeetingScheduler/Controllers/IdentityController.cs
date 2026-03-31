using MeetingScheduler.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MeetingScheduler.UI.Controllers
{

    public class IdentityController : Controller
    {
        private IUserProvider provider;

        public IdentityController(IUserProvider provider)
        {
            this.provider = provider;
        }

        [HttpGet]
        public async Task<List<AdUser>> GetDomainUsers() => await provider.GetDomainUsers();

        [HttpGet]
        public async Task<List<AdUser>> FindDomainUser([FromRoute] string search) => await provider.FindDomainUser(search);

        [HttpGet]
        public AdUser GetCurrentUser(){
            var user = provider.CurrentUser;
            return user;
        }
        

    }
    
}
