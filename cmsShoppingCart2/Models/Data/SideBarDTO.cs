using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace cmsShoppingCart2.Models.Data
{
    [Table("tblSideBar")]
    public class SideBarDTO
    {
        public int Id { get; set; }
        public string Body { get; set; }
    }
}