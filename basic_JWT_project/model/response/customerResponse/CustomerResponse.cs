using japanese_resturant_project.model.DatabaseModel;
using japanese_resturant_project.services;

namespace japanese_resturant_project.model.response.customerResponse
{
    public class CustomerResponse : Message
    {
        public Table_tb table {get;set;}
        public List<Cart_tb> cartList {get;set;}
        public Cart_tb cartItem { get;set;}
        public Order_tb orderItem {get;set;}
        public List<OrderDetail_tb> orderList { get; set; }

    }

}
