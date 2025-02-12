using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject
{
    public class Wallets
    {
        [Key]
        public int WalletsID { get; set; }
        public int UserID { get; set; }
        public int VoucherID { get; set; }
        public ICollection<Vouchers> Vouchers { get; set; }
        public ICollection<Users> Users { get; set; }
    }
}
