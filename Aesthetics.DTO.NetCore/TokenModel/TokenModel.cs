using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE_102024.DataAces.NetCore.DataOpject.TokenModel
{
    public class TokenModel
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }

    public class TokenLogOutModel
    {
        public string? AccessToken { get; set; }
        //public string? DeviceName { get; set; }
    }

    public class CheckTokenRequestData
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public int UserID { get; set; }
        public string? DeviceName { get; set; }
    }

}
