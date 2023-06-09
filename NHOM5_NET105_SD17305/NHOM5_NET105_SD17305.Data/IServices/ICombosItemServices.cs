﻿using NHOM5_NET105_SD17305.Data.Models;

namespace NHOM5_NET105_SD17305.Data.IServices
{
    public interface ICombosItemServices
    {
        public Task<bool> CreateCombosItemAsync(CombosItem p);

        public Task<bool> UpdateCombosItemAsync(CombosItem p);
        public Task<bool> DeleteCombosItemAsync(int id);
        public Task<CombosItem> GetCombosItemByIdAsync(int id);

        public Task<List<CombosItem>> GetAllCombosItemAsync();
    }
}
