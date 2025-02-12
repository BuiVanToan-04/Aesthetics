using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject
{
    public class InvoiceDetail
    {
        [Key]
        public int InvoiceDetailID { get; set; }
        public int InvoiceID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public float SellingPrice { get; set; }
        public int TotalQuantity { get; set; }
        public float TotalMoney { get; set; }
        public int DeleteStatus { get; set; }
        public string Type { get; set; }
        public Invoice Invoice { get; set; }
        public Users Users { get; set; }
        public Products Products { get; set; }
        public Servicess Servicess { get; set; }
    }
}
