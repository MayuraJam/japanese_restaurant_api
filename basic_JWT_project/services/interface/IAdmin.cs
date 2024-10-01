using japanese_resturant_project.model.DatabaseModel;
using japanese_resturant_project.model.request;
using japanese_resturant_project.model.response.adminResponse;
using japanese_resturant_project.model.response.customerResponse;
using Microsoft.AspNetCore.Mvc;

namespace japanese_resturant_project.services
{
    public interface IAdmin
    {
        public Task<AdminResponse> GetStaftProfile(string staftID);
        public Task<AdminResponse> UpdateStaftProfile(UpdateStaftProfileRequest request);
        public Task<AdminResponse> GetOptionList();
        public Task<AdminResponse> AddOption(OptionRequest request);
        public Task<AdminResponse> UpdateOption(Option_tb_ request);
        public Task<AdminResponse> DeleteOption([FromBody] Guid optionID);
        public Task<AdminResponse> GetMenuList2(SearchRequest request); //ลองสร้างใน controller ก่อน
        public Task<AdminResponse> GetMenuByID(string menuID);
        public Task<AdminResponse> AddMenu([FromForm] MenuRequest request);
        public Task<AdminResponse> UpdateMenu(MenuUpdate request);
        public Task<AdminResponse> DeleteMenu(string menuID);
        public Task<AdminResponse> GetTableList();
        public Task<AdminResponse> GetOrderForAdmin(SearchOrderRequest request);
        public Task<AdminResponse> GetOrderByID(string orderID);
        public Task<AdminResponse> ConfirmOrder(ConfirmRequest request);
        public Task<AdminResponse> GetOrderDetail();
        public Task<AdminResponse> updateOrderStatus(UpdateOrderStatusRequest request);
        public Task<AdminResponse> GetNotification();
        public Task<AdminResponse> Readable(string notificationID);
        public Task<AdminResponse> DeleteNotification(string notificationID);




    }
}
