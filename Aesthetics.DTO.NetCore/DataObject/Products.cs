using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject
{
    public class Products
    {
        [Key]
        public int ProductID { get; set; }
        public int ProductsOfServicesID { get; set; }
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public double SellingPrice { get; set; }
        public int Quantity { get; set; }
        public string ProductImages { get; set; }
        public int DeleteStatus { get; set; }
        public Supplier Supplier { get; set; }
        public TypeProductsOfServices TypeProductsOfServices { get; set; }
        public ICollection<InvoiceDetail> InvoiceDetail { get; set; } 
		public ICollection<CartProduct> CartProducts { get; set; }
		public ICollection<Comments> Comments { get; set; }
    }
}
