using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cmsShoppingCart2.Models.Data
{
    [Table("tblPages")]
    public class PageDTO
    {
        [Key]
        public int Id { get; set; }
        public String Title { get; set; }
        public String Slug { get; set; }
        public String Body { get; set; }
        public int Sorting { get; set; }
        public bool HasSideBar { get; set; }
    }
}