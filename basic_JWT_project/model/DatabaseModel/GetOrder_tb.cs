namespace japanese_resturant_project.model.DatabaseModel
{
    public class GetOrder_tb
    {
        public string orderID { get; set; }
        public Guid? staftID { get; set; }
        public string orderStatus { get; set; }
        public DateTime orderDate { get; set; }
        public decimal totalPrice { get; set; }
        public string confirmOrder { get; set; }
        public int orderQ { get; set; }
        //รายการใน order
        public string tableID { get; set; }
        public string paymentStatus { get; set; }

        public string orderDetailStatus { get; set; }
        public int quantity { get; set; }
        public string? optionValue { get; set; }
        public string menuID { get; set; }
        public string menuName { get; set; }
        public string imageName { get; set; }
        public string imageSrc { get; set; }
        public decimal netprice { get; set; }
    }
}
