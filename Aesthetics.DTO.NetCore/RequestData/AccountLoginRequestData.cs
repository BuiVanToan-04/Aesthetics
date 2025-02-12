using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE_102024.DataAces.NetCore.DataOpject.RequestData
{
    public class AccountLoginRequestData
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        //public string? DeviceID { get; set; }
    }
}
