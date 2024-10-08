﻿using japanese_resturant_project.model.DatabaseModel;
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
        public List<Order_tb> orders { get; set; } 
        public List<Payment_tb> payList { get; set; }
        public Payment_tb payItem { get; set; }
        public Notification_tb notiItem { get; set; }
        public Review_tb reviewItem { get; set; }
        public List<Review_tb> reviewList { get; set; }
    } 

}
