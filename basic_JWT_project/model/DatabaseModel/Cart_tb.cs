using System.ComponentModel.DataAnnotations.Schema;

namespace japanese_resturant_project.model.DatabaseModel
{
    public class Cart_tb
    {
        public Guid cartID { get; set; }
        public Guid menuID { get; set; }
        public string menuName { get; set; }
        public decimal? unitPrice { get; set; }
        public string imageName { get; set; }

        [NotMapped]
        public string imageSrc { get; set; }
        public string tableID { get; set; }
        public int quantity { get; set; }
        public string? optionValue {  get; set; }
        public decimal? netprice { get; set; }

    }
}
