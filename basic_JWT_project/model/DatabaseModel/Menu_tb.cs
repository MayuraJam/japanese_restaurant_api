using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace japanese_resturant_project.model.DatabaseModel
{
    public class Menu_tb
    {
        public Guid menuID { get; set; }
        public string menuName { get; set; }
        public string menuDescription { get; set; }
        public decimal? unitPrice { get; set; }
        public string categoryName { get; set; }
        public Guid optionID { get; set; }
        public DateTime createDate { get; set; }
        public DateTime updateDate { get; set; }
        public double rating { get; set; }
        public string imageName { get; set; }
        //[NotMapped]
        //public IFormFile image { get; set; }
        // public byte[]? image { get; set; }

        public string optionName { get; set; }
        public string value { get; set; }

    }

}
