namespace japanese_resturant_project.model.DatabaseModel
{
    public class OrderDetail_tb
    {
        public string orderID { get; set; }
        public string orderDetailStatus { get; set; }
        public int quantity { get; set; }
        public string? optionValue { get; set; }
        public Guid menuID { get; set; }
        public string menuName { get; set; }
        public string imageName { get; set; }
        public string imageSrc { get; set; }
        public decimal? unitPrice { get; set; }
        public int ? quantityPrice { get; set; }
        public int stockQuantity { get; set; }
        public decimal? productiPrice { get; set; } //quantity * unitprice
        public Order_tb orderMain { get; set; } //เชื่อมโยงคำสั่ง 1 รายการต่อ 1 order
    }
}
