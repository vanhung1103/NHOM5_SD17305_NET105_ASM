﻿using NHOM5_NET105_SD17305.Models;

namespace NHOM5_NET105_SD17305.IServices
{
    public interface ICombosServices
    {
        public Task<bool> CreateCombosAsync(Combos p);

        public Task<bool> UpdateCombosAsync(Combos p);
        public Task<bool> DeleteCombosAsync(int id);
        public Task<Combos> GetCombosByIdAsync(int id);

        public Task<List<Combos>> GetAllCombosAsync();
    }
}
