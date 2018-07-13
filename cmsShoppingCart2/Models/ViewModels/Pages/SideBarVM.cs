using cmsShoppingCart2.Models.Data;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace cmsShoppingCart2.Models.ViewModels.Pages
{
    public class SideBarVM
    {
        public SideBarVM()
        {

        }
        public SideBarVM(SideBarDTO row)
        {
            Id = row.Id;
            Body = row.Body;
        }

        public int Id { get; set; }
        [AllowHtml]
        public string Body { get; set; }
    }
}