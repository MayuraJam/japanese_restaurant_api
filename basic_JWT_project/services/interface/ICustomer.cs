﻿using japanese_resturant_project.model.request.customerRequest;
using japanese_resturant_project.model.response.customerResponse;
namespace japanese_resturant_project.services
{
    public interface ICustomer
    {
        public Task<CustomerResponse> OpenTable(OpenTableRequest request);
        public Task<CustomerResponse> CloseTable(CloseTableRequest request);
        public Task<CustomerResponse> GetCartBytableID(string tableID);

        public Task<CustomerResponse> AddCart(AddCartRequest request);

        public Task<CustomerResponse> UpdateCart(UpdateCartRequest request);

        public Task<CustomerResponse> DeleteCartItem(Guid cartID);
        public Task<CustomerResponse> AddOrder(AddOrderRequest request);
        public Task<CustomerResponse> GetOrderDetail(string orderID);
        public Task<CustomerResponse> GetOrder(string customerID);
        public Task<CustomerResponse> CancleOrder(string orderID);
        public Task<CustomerResponse> AddPayment(PaymentRequest request);
        public Task<CustomerResponse> AddNotification(NotificationRequest request);
        public Task<CustomerResponse> GetPayment(string orderID);
        public Task<CustomerResponse> GetOrderAndPayment(string customerID);
        public Task<CustomerResponse> AddReview(ReviewRequest request);

    }
}
