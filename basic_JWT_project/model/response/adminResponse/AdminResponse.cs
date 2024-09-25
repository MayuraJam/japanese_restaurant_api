using japanese_resturant_project.model.DatabaseModel;
using japanese_resturant_project.services;

namespace japanese_resturant_project.model.response.adminResponse
{
    public class AdminResponse : Message
    {
        public Staft_Authentication_tb staftItem { get; set; }

        public List<Option_tb_> optionList {  get; set; }
        public Option_tb_ optionitem { get; set; }
        public Menu_tb menuitem { get; set; }
        public List<Menu_tb> menuList { get; set; }
        public List<Table_tb> tableList { get; set; }
        public List<Order_tb> orders {  get; set; }
        public Order_tb orderItem { get; set; }
        public List<OrderDetail_tb> orderList { get; set; }
        public OrderDetail_tb orderOne { get; set; }

    }
}
