using DTO;

namespace DAL.Interfaces
{
    public interface IOrderProductDal
    {
        List<OrderProduct> GetOrderProductsByOrderId(int orderId);
        public List<OrderProduct> GetProductsForOrder(int orderId);
    }
}
