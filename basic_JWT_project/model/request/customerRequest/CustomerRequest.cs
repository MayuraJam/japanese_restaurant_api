﻿using System.ComponentModel.DataAnnotations.Schema;

namespace japanese_resturant_project.model.request.customerRequest
{
    public class OpenTableRequest
    {
        public string tableID { get; set; }
    }
    public class AddCartRequest
    {
        public Guid menuID { get; set; }
        public string tableID { get; set; }
        //public int quantity { get; set; }
        public string? optionValue { get; set; }
    }
    public class UpdateCartRequest
    {
        public Guid cartID { get; set; }
        public int quantity { get; set; }
    }
}