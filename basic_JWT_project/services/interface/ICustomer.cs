using japanese_resturant_project.model.request.customerRequest;
using japanese_resturant_project.model.response;
using japanese_resturant_project.model.response.customerResponse;
namespace japanese_resturant_project.services
{
    public interface ICustomer
    {
        public Task<CustomerResponse> OpenTable(OpenTableRequest request);
        public Task<CustomerResponse> CloseTable(OpenTableRequest request);
    }
}
