using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Configuration;

namespace RestService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        //[ClaimsPrincipalPermission(SecurityAction.Demand, Operation = "Withdraw", Resource = "BankAccountFunds")]
        public string GetDrivers()
        {
            var a = System.Security.Claims.ClaimsPrincipal.Current;
            return "xxx";
        }

    }
}
