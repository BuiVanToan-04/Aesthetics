using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject.Model
{
    public class TypeProductsOfServices
    {
        [Key]
        public int ProductsOfServicesID { get; set; }
        public string? ProductsOfServicesName { get; set; }
        public string? ProductsOfServicesType { get; set; }
        public int? DeleteStatus { get; set; }
        public ICollection<Servicess> Services { get; set; }
        public ICollection<Products> Products { get; set; }
    }
}
