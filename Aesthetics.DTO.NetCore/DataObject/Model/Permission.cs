using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject.Model
{
    public class Permission
    {
        [Key]
        public int PermissionID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public int FunctionID { get; set; }
        public int IsView { get; set; } 
        public int IsInsert { get; set; }
        public int IsUpdate { get; set; }
        public int IsDelete { get; set; }
        public Users Users { get; set; }
        public Functions Functions { get; set; }
    }
}
