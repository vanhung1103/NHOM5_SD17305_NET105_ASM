﻿using NHOM5_NET105_SD17305.Models;

namespace NHOM5_NET105_SD17305.IServices
{
    public interface ICustomerServices
    {
        public Task<bool> CreateCustomerAsync(Customer p);

        public Task<bool> UpdateCustomerAsync(Customer p);
        public Task<bool> DeleteCustomerAsync(int id);
        public Task<Customer> GetCustomerByIdAsync(int id);

        public Task<List<Customer>> GetAllCustomerAsync();
    }
}
