using System.ComponentModel.DataAnnotations.Schema;

namespace japanese_resturant_project.model.request.customerRequest
{
    public class OpenTableRequest
    {
        public string tableID { get; set; }
    }
    public class AddCartRequest
    {
        public string menuID { get; set; }
        public string tableID { get; set; }
        //public int quantity { get; set; }
        public string? optionValue { get; set; }
        public decimal unitPrice { get; set; }
        public string customerID { get; set; }

    }
    public class UpdateCartRequest
    {
        public Guid cartID { get; set; }
        public int quantity { get; set; }
        public decimal unitPrice { get; set; }


    }
    public class AddOrderRequest
    {
        public string tableID { get; set; } //เปรียบเสมือน customerID
        public decimal totalPrice { get; set; }
        public string? staftID { get; set; }
        public string customerID { get; set; }

    }
    public class PaymentRequest
    {
        public string orderID { get; set; } 
        public string tableID { get; set; } 
        public string paymentType { get; set; }
        public decimal totalAmount { get; set; }
        public decimal totalFee { get; set; }
        public decimal cash { get; set; }
        public decimal change { get; set; }
        public decimal netTotalAmount { get; set; }
        public string? staffID { get; set; }
        public string customerID { get; set; }

    }

    public class NotificationRequest
    {
        public string title { get; set; }
        public string message { get; set; }
        public string tableID { get; set; }


    }
}
