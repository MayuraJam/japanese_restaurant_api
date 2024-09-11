using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace japanese_resturant_project.model.request
{
    public class AdminRequest
    {
    }
    public class OptionRequest
    {
         public string? optionName { get; set; }
         public string? value { get; set; }
     }
    public class MenuRequest
    {
        public string? menuName { get; set; }
        public string? menuDescription { get; set; }

        public decimal? unitPrice { get; set; }
        public string? categoryName { get; set; }
        public Guid? optionID { get; set; }
        //public string imageName { get; set; }

        [NotMapped]
        public IFormFile imageFile { get; set; }
        public int quantity { get; set; }
    }
    public class MenuUpdate
    {
        public Guid menuID { get; set; }
        public string? menuName { get; set; }
        public string? menuDescription { get; set; }

        public decimal? unitPrice { get; set; }
        public string? categoryName { get; set; }
        public Guid? optionID { get; set; }
        //public byte[]? image { get; set; }
        [NotMapped]
        public IFormFile imageFile { get; set; }
        public int quantity { get; set; }

    }

}
