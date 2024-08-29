using japanese_resturant_project.model.DatabaseModel;
using japanese_resturant_project.services;

namespace japanese_resturant_project.model.response.customerResponse
{
    public class CustomerResponse : Message
    {
        public Table_tb table {get;set;}
        
    }
}
