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
        public int stockQuantity { get; set; }
    }
    public class MenuUpdate
    {
        public string menuID { get; set; }
        public string? menuName { get; set; }
        public string? menuDescription { get; set; }

        public decimal? unitPrice { get; set; }
        public string? categoryName { get; set; }
        public Guid? optionID { get; set; }
        //public byte[]? image { get; set; }
        [NotMapped]
        public IFormFile imageFile { get; set; }
        public int stockQuantity { get; set; }

    }
    public class SearchRequest
    {
        public string? menuName { get; set; }
    }
    public class ConfirmRequest
    {
        public string confirm { get; set; }
        public string orderID { get; set; }
        public string? staftID { get; set; }

    }
    public class UpdateOrderStatusRequest
    {
        public string orderID { get; set; }
        public string menuID { get; set; }
        public string orderDetailStatus { get; set; }

    }
    public class UpdateStaftProfileRequest
    {
        public string staftID { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? phone { get; set; }
        public string? email { get; set; }
        public string? jobType { get; set; }
    }

}

