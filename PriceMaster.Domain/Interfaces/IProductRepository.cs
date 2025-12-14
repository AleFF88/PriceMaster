using PriceMaster.Domain.Entities;

namespace PriceMaster.Domain.Interfaces {
    public interface IProductRepository {
        Task Add(Product product); 
        Task<bool> Exists(string productCode);
    }
}
