using cmsShoppingCart2.Models.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace cmsShoppingCart2.Models.ViewModels.Pages
{
    public class PagesVM
    {
        public PagesVM()
        {

        }
        public PagesVM(PageDTO raw)
        {
            Id = raw.Id;
            Title = raw.Title;
            Slug = raw.Slug;
            Body = raw.Body;
            HasSideBar = raw.HasSideBar;
        }

        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public String Title { get; set; }
        public String Slug { get; set; }
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 3)]
        [AllowHtml]
        public String Body { get; set; }
        public int Sorting { get; set; }
        public bool HasSideBar { get; set; }
    }
}