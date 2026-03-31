using MeetingScheduler.Domain.Common.Interfaces;
using MeetingScheduler.Domain.Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace MeetingScheduler.Tests.UnitTests
{
    [TestClass]
    public class Test
    {
        private readonly IZoomService _zoomService;

        public Test(IZoomService zoomService)
        {
            _zoomService = zoomService;
        }


        [TestMethod]
        public async Task<int> TestAdd()
        {
            ZoomMeetingDetails zoomMeetingDetails = new ZoomMeetingDetails();
            zoomMeetingDetails.StartDateTime= DateTime.Now;
            zoomMeetingDetails.DurationInMinutes = 60;
            var result = await _zoomService.CheckMeetingExists(zoomMeetingDetails);
            return 1;
        }
        
        //[TestMethod]
        //public string GetCurrentDomainLDAPInfo()
        //{
        //    DirectoryEntry de = new DirectoryEntry("LDAP://RootDSE");

        //    return "LDAP://" + de.Properties["defaultNamingContext"][0].ToString();
        //}



        ////Several solutions presented here lack the ability to differentiate between a wrong user / password, 
        ////and a password that needs to be changed.That can be done in the following way:
        //[TestMethod]
        //public void ConnectToLDAP()
        //{

        //    try
        //    {
        //        // CN = MerDarah,DC = org,DC = sa
        //        LdapConnection connection = new LdapConnection("MerDarah.org.sa");
        //        NetworkCredential credential = new NetworkCredential("merna", "P@ssw0rd1");
        //        connection.Credential = credential;
        //        connection.Bind();
        //    }
        //    catch (LdapException lexc)
        //    {
        //        string error = lexc.ServerErrorMessage;
        //    }
        //    catch (Win32Exception ex)
        //    {
        //        switch (ex.NativeErrorCode)
        //        {
        //            case 1326: // ERROR_LOGON_FAILURE (incorrect user name or password)
        //                       // ...
        //            case 1327: // ERROR_ACCOUNT_RESTRICTION
        //                       // ...
        //            case 1330: // ERROR_PASSWORD_EXPIRED
        //                       // ...
        //            case 1331: // ERROR_ACCOUNT_DISABLED
        //                       // ...
        //            case 1907: // ERROR_PASSWORD_MUST_CHANGE
        //                       // ...
        //            case 1909: // ERROR_ACCOUNT_LOCKED_OUT
        //                       // ...
        //            default: // Other
        //                break;
        //        }
        //    }
        //}
    }
}
