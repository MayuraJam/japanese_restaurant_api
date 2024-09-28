using System.Drawing;

namespace japanese_resturant_project.model.DatabaseModel
{
    public class Revenue_tb
    {
        public string revenueID {  get; set; }
        public DateTime createDate { get; set; }
        public string revenueDescritption { get; set; }
        public string orderID { get; set; }
        public decimal totalAmount { get; set; }
        public decimal tax {  get; set; }
        public decimal netAmount { get; set; }
    }
}
