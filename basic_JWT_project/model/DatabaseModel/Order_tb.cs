namespace japanese_resturant_project.model.DatabaseModel
{
    public class Order_tb
    {
        public string orderID { get; set; }
        public Guid? staftID { get; set; }
        public string orderStatus { get; set; }
        public DateTime orderDate { get; set; }
        public decimal totalPrice { get; set; } 
        public string confirmOrder { get; set; }
        public int orderQ {  get; set; }
        //รายการใน order
        public string tableID { get; set; }
        public string paymentStatus { get; set; }
        public ICollection<OrderDetail_tb>OrderDetailList { get; set; } //เป็นคำสั่งที่หมายถึง ตาราง Orderr เป็นตารางแม่ที่มีคำสั่งซื้อ orderdetail หลายรายการ
        
  


    }
}
