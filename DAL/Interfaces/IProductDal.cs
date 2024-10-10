using DTO;

namespace DAL.Interfaces
{
    public interface IProductDal
    {
        List<Product> GetAllProducts(); 
        Product GetProductById(int productId);
        List<Product> SearchProducts(string productName);
        List<Product> SortProducts(List<Product> products, string sortBy, bool ascending);
        Product AddProduct(Product product);
        void DeleteProduct(int productId);
    }
}
