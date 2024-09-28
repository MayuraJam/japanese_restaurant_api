namespace japanese_resturant_project.model.DatabaseModel
{
    public class Payment_tb
    {
        public string receiptID { get; set; }
        public string tableID { get; set; }
        public string? staffID { get; set; }
        public string orderID { get; set; }
        public string paymentStatus { get; set; }
        public string paymentType { get; set; }
        public decimal totalAmount { get; set; }
        public decimal totalTax { get; set; }
        public decimal cash { get; set; }
        public decimal change { get; set; }
        public decimal netTotalAmount {  get; set; }

        public DateTime payDatetime { get; set; }

        public ICollection<OrderDetail_tb> OrderDetailList { get; set; } //เป็นคำสั่งที่หมายถึง ตาราง Orderr เป็นตารางแม่ที่มีคำสั่งซื้อ orderdetail หลายรายการ

    }
}
