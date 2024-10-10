using DAL.Concrete;
using DAL.Interfaces;
using DTO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

IUserDal _userDal = null;
IOrderDal _orderDal = null;
IProductDal _productDal = null;
ICommentDal _commentDal = null;

IConfiguration configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("config.json").Build();
string connectionString = configuration.GetConnectionString("DefaultConnection");

_userDal = new UserDal(connectionString);
_orderDal = new OrderDal(connectionString);
_productDal = new ProductDal(connectionString);
_commentDal = new CommentDal(connectionString);

// Login logic
Console.WriteLine("Welcome to the Shop!\n");

Console.WriteLine("Please choose an option:");
Console.WriteLine("1 - Login");
Console.WriteLine("2 - Register\n");

string loginOrRegisterOption = Console.ReadLine()?.Trim().ToLower() ?? "";

User loggedInUser = null;

if (loginOrRegisterOption == "1")
{
    loggedInUser = Login();
}
else if (loginOrRegisterOption == "2")
{
    RegisterUser();
    loggedInUser = Login();
}
else
{
    Console.WriteLine("Invalid option selected.");
    return;
}

if (loggedInUser == null)
{
    Console.WriteLine("Login failed. Exiting application.");
    return;
}

// Console App
char option = 's';
while (option != 'q')
{
    Console.WriteLine("\nPlease select option:\n" +
        "1 - View order history\n" +
        "2 - Repeat order\n" +
        "3 - Search product\n" +
        "4 - Sort products\n" +
        "5 - Sort orders" +
        "6 - Comment on product\n" +
        "q - Logout\n");

    string selectedOption = Console.ReadLine()?.Trim().ToLower() ?? "";
    if (string.IsNullOrWhiteSpace(selectedOption) || selectedOption.Length > 1)
    {
        Console.WriteLine("Incorrect option selected!");
        continue;
    }

    option = Convert.ToChar(selectedOption.Trim().ToLower());

    switch (option)
    {
        case '1':
            ViewOrderHistory(loggedInUser.UserId);
            break;
        case '2':
            RepeatOrder(loggedInUser.UserId);
            break;
        case '3':
            SearchProduct();
            break;
        case '4':
            SortProducts();
            break;
        case '5':
            SortOrders(loggedInUser.UserId);
            break;
        case '6':
            CommentOnProduct(loggedInUser.UserId);
            break;
        case '7':
            AddOrder(loggedInUser.UserId);
            break;
        case 'q':
            Console.WriteLine("Logging out...");
            break;
        default:
            Console.WriteLine("Incorrect option selected!");
            break;
    }
}

//
//
// 

User Login()
{
    Console.Write("Enter your username: ");
    string username = Console.ReadLine();

    Console.Write("Enter your password: ");
    string password = ReadPassword();

    string hashedPassword = HashPassword(password);

    User user = _userDal.Login(username, hashedPassword);

    return user;
}

string ReadPassword()
{
    StringBuilder passwordBuilder = new StringBuilder();
    ConsoleKeyInfo keyInfo;

    do
    {
        keyInfo = Console.ReadKey(true);

        if (keyInfo.Key != ConsoleKey.Backspace && keyInfo.Key != ConsoleKey.Enter)
        {
            passwordBuilder.Append(keyInfo.KeyChar);
            Console.Write("*");
        }
        else if (keyInfo.Key == ConsoleKey.Backspace && passwordBuilder.Length > 0)
        {
            passwordBuilder.Remove(passwordBuilder.Length - 1, 1);
            Console.Write("\b \b");
        }
    } while (keyInfo.Key != ConsoleKey.Enter);

    Console.WriteLine();
    return passwordBuilder.ToString();
}

void RegisterUser()
{
    Console.Write("Enter a new username: ");
    string username = Console.ReadLine();

    Console.Write("Enter a password: ");
    string password = Console.ReadLine();

    string hashedPassword = HashPassword(password);
    bool userCreated = _userDal.CreateUser(username, hashedPassword);

    if (userCreated)
    {
        Console.WriteLine("User created successfully.");
    }
    else
    {
        Console.WriteLine("Failed to create user.");
    }
}

string HashPassword(string password)
{
    using (SHA256 sha256 = SHA256.Create())
    {
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        StringBuilder builder = new StringBuilder();
        foreach (byte b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }
        return builder.ToString();
    }
}

void ViewOrderHistory(int userId)
{
    List<Order> orders = _orderDal.GetAllOrdersForUser(userId);
    if (orders.Count == 0)
    {
        Console.WriteLine("No orders found.");
        return;
    }

    foreach (var order in orders)
    {
        Console.WriteLine($"Order ID: {order.OrderId}, Total Price: {order.TotalPrice}, Date: {order.OrderDate}");
    }
}

void RepeatOrder(int userId)
{
    Console.WriteLine("Enter the Order ID you want to repeat:");
    if (int.TryParse(Console.ReadLine(), out int orderId))
    {
        var repeatedOrder = _orderDal.RepeatOrder(orderId, userId);
        if (repeatedOrder != null)
        {
            Console.WriteLine($"Order {orderId} repeated successfully. New Order ID: {repeatedOrder.OrderId}");
        }
        else
        {
            Console.WriteLine("Failed to repeat the order. Order not found.");
        }
    }
    else
    {
        Console.WriteLine("Invalid Order ID.");
    }
}

void SearchProduct()
{
    Console.WriteLine("Enter product name to search:");
    string productName = Console.ReadLine();
    List<Product> products = _productDal.SearchProducts(productName);

    if (products.Count == 0)
    {
        Console.WriteLine("No products found.");
        return;
    }

    foreach (var product in products)
    {
        Console.WriteLine($"Product ID: {product.ProductId}, Name: {product.ProductName}, Price: {product.Price}");
    }
}

void SortOrders(int userId)
{
    Console.WriteLine("Sort by (date/totalprice):");
    string sortBy = Console.ReadLine();

    Console.WriteLine("Sort in ascending order? (yes/no):");
    string sortOrder = Console.ReadLine();
    bool ascending = sortOrder?.Trim().ToLower() == "yes";

    List<Order> orders = _orderDal.GetAllOrdersForUser(userId);
    List<Order> sortedOrders = _orderDal.SortOrders(orders, sortBy, ascending);

    foreach (var order in sortedOrders)
    {
        Console.WriteLine($"Order ID: {order.OrderId}, Total Price: {order.TotalPrice}, Date: {order.OrderDate}");
    }
}

void SortProducts()
{
    Console.WriteLine("Sort by (name/price):");
    string sortBy = Console.ReadLine();

    Console.WriteLine("Sort in ascending order? (yes/no):");
    string sortOrder = Console.ReadLine();
    bool ascending = sortOrder?.Trim().ToLower() == "yes";

    List<Product> products = _productDal.GetAllProducts();
    List<Product> sortedProducts = _productDal.SortProducts(products, sortBy, ascending);

    foreach (var product in sortedProducts)
    {
        Console.WriteLine($"Product ID: {product.ProductId}, Name: {product.ProductName}, Price: {product.Price}");
    }
}

void AddOrder(int userId)
{
    List<OrderProduct> selectedOrderProducts = new List<OrderProduct>();
    string input;

    // Add Products to Order
    do
    {
        Console.WriteLine("Enter product ID to add (or 'done' to finish):");
        input = Console.ReadLine();

        if (input.Trim().ToLower() != "done")
        {
            int productId;
            if (int.TryParse(input, out productId))
            {
                Product product = _productDal.GetProductById(productId);
                if (product != null)
                {
                    Console.WriteLine($"Adding product: {product.ProductName}, Price: {product.Price}");

                    selectedOrderProducts.Add(new OrderProduct
                    {
                        ProductId = product.ProductId,
                        Quantity = 1
                    });
                }
                else
                {
                    Console.WriteLine("Invalid product ID.");
                }
            }
            else
            {
                Console.WriteLine("Please enter a valid number.");
            }
        }

    } while (input.Trim().ToLower() != "done");

    // Calculate Total Price
    decimal totalPrice = selectedOrderProducts.Sum(op => _productDal.GetProductById(op.ProductId).Price);

    // Add Order to Database
    Order newOrder = new Order
    {
        UserId = userId,
        TotalPrice = totalPrice,
        OrderDate = DateTime.Now
    };

    _orderDal.AddOrder(newOrder, selectedOrderProducts);

    Console.WriteLine($"Order added successfully with {selectedOrderProducts.Count} products. Total price: {totalPrice:C}");
}


void CommentOnProduct(int userId)
{
    Console.WriteLine("Enter Product ID to comment on:");
    if (int.TryParse(Console.ReadLine(), out int productId))
    {
        Console.WriteLine("Enter your comment:");
        string commentText = Console.ReadLine();

        var comment = new Comment
        {
            ProductId = productId,
            UserId = userId,
            CommentText = commentText,
            CommentTime = DateTime.Now
        };

        _commentDal.AddComment(comment);
        Console.WriteLine("Comment added successfully.");
    }
    else
    {
        Console.WriteLine("Invalid Product ID.");
    }
}
