using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject
{
    public class Functions 
    {
        [Key]
        public int FunctionID { get; set; }
        public string FunctionCode { get; set; }
        public string FunctionName { get; set; }
        public string FunctionDescription { get; set; }
        public int? DeleteStatus { get; set; }
        public ICollection<Permission> Permission { get; set; }
    }
}
