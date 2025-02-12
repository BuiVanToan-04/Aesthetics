using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject
{
    public class Comments
    {
        [Key]
        public int CommentID { get; set; }
        public int ProductID { get; set; }
        public int ServiceID { get; set; }
        public int UserID { get; set; }
        public int Comment_Content { get; set; }
        public int CreationDate { get; set; }
        public Products Products { get; set; }
        public Servicess Servicess { get; set; }
        public Users Users { get; set; }

    }
}
