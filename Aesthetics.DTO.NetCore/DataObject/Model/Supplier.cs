using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject.Model
{
    public class Supplier
    {
        [Key]
        public int SupplierID { get; set; }
        public string? SupplierName { get; set; }
        public int? DeleteStatus { get; set; }
        public ICollection<Products> Products { get; set; }
    }
}
