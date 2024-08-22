

using japanese_resturant_project.model.response;

namespace japanese_resturant_project.services
{
    public interface Itest
    {
        public Task<testResponse> GetMenuData();
    }
    
}
