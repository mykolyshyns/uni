using DTO;

namespace DAL.Interfaces
{
    public interface IOrderDal
    {
        List<Order> GetAllOrdersForUser(int userId);

        Order GetOrderById(int orderId);
        public void UpdateOrderTotalPrice(int orderId, decimal totalPrice);
        List<OrderProduct> GetOrderProductsByOrderId(int orderId);

        List<Order> SortOrders(List<Order> orders, string sortBy, bool ascending);

        Order RepeatOrder(int orderId, int userId);
        void AddOrder(Order order, List<OrderProduct> orderProducts);
        void DeleteOrder(int orderId);

    }
}

