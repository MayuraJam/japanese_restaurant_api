using japanese_resturant_project.model.DatabaseModel;
using japanese_resturant_project.model.request;
using japanese_resturant_project.model.response.adminResponse;
using Microsoft.AspNetCore.Mvc;

namespace japanese_resturant_project.services
{
    public interface IAdmin
    {
        public Task<AdminResponse> GetOptionList();
        public Task<AdminResponse> AddOption(OptionRequest request);
        public Task<AdminResponse> UpdateOption(Option_tb_ request);
        public Task<AdminResponse> DeleteOption([FromBody] Guid optionID);
        public Task<AdminResponse> GetMenuList2(); //ลองสร้างใน controller ก่อน
        public Task<AdminResponse> GetMenuByID(Guid menuID);
        public Task<AdminResponse> AddMenu([FromForm] MenuRequest request);
        public Task<AdminResponse> UpdateMenu([FromForm] MenuUpdate request);
        public Task<AdminResponse> DeleteMenu(Guid menuID);
        public Task<AdminResponse> GetTableList();

    }
}
