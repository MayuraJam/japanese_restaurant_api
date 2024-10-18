namespace japanese_resturant_project.model.DatabaseModel
{
    public class OrderDetail_tb
    {
        //public string orderDetailID { get; set; }
        public string orderID { get; set; }
        public string orderDetailStatus { get; set; }
        public int quantity { get; set; }
        public string? optionValue { get; set; }
        public string menuID { get; set; }
        public string menuName { get; set; }
        public string imageName { get; set; }
        public string imageSrc { get; set; }
        public decimal? unitPrice { get; set; }
        public string addReview { get; set; }
        public int stockQuantity { get; set; }
        public decimal netprice { get; set; } //quantity * unitprice
        public DateTime orderDate {  get; set; } 
        public string categoryName { get; set; }
        public string tableID { get; set; }
        public int Q { get; set; }
        public Order_tb orderMain { get; set; } //เชื่อมโยงคำสั่ง 1 รายการต่อ 1 order
    }
}
