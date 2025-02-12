using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject
{
    public class Users
    {
        [Key]
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string? Email { get; set; }
        public DateTime? DateBirth { get; set; }
        public string? Sex { get; set; }
        public DateTime? Creation { get; set; }
        public string? Phone { get; set; }
        public string? Addres { get; set; }
        public string? IDCard { get; set; }
        public string? TypePerson { get; set; }
        public int? AccumulatedPoints { get; set; }
        public string? ReferralCode { get; set; }
        public string? RefeshToken { get; set; }
        public int? DeleteStatus { get; set; }
        public double? Moneyy { get; set; }
        public DateTime? TokenExprired { get; set; }
        public Carts Cart { get; set; }
        public ICollection<Wallets> Wallets { get; set; }
        public ICollection<UserSession> UserSession { get; set; }
        public ICollection<Comments> Comments { get; set; }
        public ICollection<Invoice> Invoice { get; set; }
        public ICollection<InvoiceDetail> InvoicesDetail { get; set; }
        public ICollection<Permission> Permissions { get; set; }
    }
}
