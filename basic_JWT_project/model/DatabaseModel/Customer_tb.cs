namespace japanese_resturant_project.model.DatabaseModel
{
    public class Customer_tb
    {
        public Guid memberID {  get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string phone { get; set; }
        public Guid userID { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string roleName { get; set; }
        public decimal totalPoint { get; set; }
        public int countOfPoint { get; set; }
        public ICollection<Point_tb> pointlList { get; set; } //เป็นคำสั่งที่หมายถึง ตาราง Orderr เป็นตารางแม่ที่มีคำสั่งซื้อ orderdetail หลายรายการ

    }
}
