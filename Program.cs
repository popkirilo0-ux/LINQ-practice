using System.Linq;

namespace Practic
{
    enum Categories 
    {
        Electronics,
        Food,
        Clothes
    }
    class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public bool IsActive { get; set; } = false;

        public User(int id, string name, int age)
        {
            Id = id;
            Name = name;
            Age = age;
        }
        public User(int id, string name, int age, bool isActive):this(id, name, age) 
        {
            isActive = IsActive;
        } 
    }

    class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Price { get; set; }

        public Order(int id, int userId, int price)
        {
            Id = id;
            UserId = userId;
            Price = price;
        }
    }

    class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Categories Category { get; set; }
        public decimal Price { get; set; }
    }

    class OrderItem
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class Practic
    {
        public static void Main(string[] args)
        {
            var users = new List<User>()
            {
                new User(1, "Alex", 21),
                new User(2, "Bob", 17, true),
                new User(3, "Maria", 19, true),
                new User(4, "John", 15),
                new User(5, "Kyryl", 20, true)
            };

            var orders = new List<Order>()
            {
                new Order(1, 1, 500),
                new Order(2, 1, 1200),
                new Order(3, 2, 300),
                new Order(4, 3, 1500),
                new Order(5, 3, 700),
                new Order(6, 3, 2500),
                new Order(7, 5, 100)
            };

            var products = new List<Product>()
            {
               new Product { Id = 1, Name = "Phone", Category = Categories.Electronics, Price = 2500 },
               new Product { Id = 2, Name = "Laptop", Category = Categories.Electronics, Price = 4500 },
               new Product { Id = 3, Name = "Apple", Category = Categories.Food, Price = 200 },
               new Product { Id = 4, Name = "Cake", Category = Categories.Food, Price = 1000 },
               new Product { Id = 5, Name = "T-Shirt", Category = Categories.Clothes, Price = 800 }
            };

            var orderItems = new List<OrderItem>()
            {
                new OrderItem { OrderId = 1, ProductId = 1, Quantity = 2 },
                new OrderItem { OrderId = 1, ProductId = 3, Quantity = 5 },
                new OrderItem { OrderId = 2, ProductId = 2, Quantity = 1 },
                new OrderItem { OrderId = 3, ProductId = 4, Quantity = 2 },
                new OrderItem { OrderId = 3, ProductId = 3, Quantity = 10 },
                new OrderItem { OrderId = 4, ProductId = 2, Quantity = 1 },
                new OrderItem { OrderId = 4, ProductId = 5, Quantity = 3 },
                new OrderItem { OrderId = 5, ProductId = 1, Quantity = 1 },
                new OrderItem { OrderId = 6, ProductId = 2, Quantity = 2 },
                new OrderItem { OrderId = 7, ProductId = 3, Quantity = 20 }
            };

            // Task 26
            // Get the top 5 most active users by number of orders.
            // Result:
            // Name → OrdersCount
            //
            // Example:
            // Maria → 3
            // Alex  → 2
            // Bob   → 1
            // Kyryl → 1

            var result26 = users.Join(orders,
                userKey => userKey.Id,
                orderKey => orderKey.UserId,
                (userKey, orderKey) => new { userKey.Name, orderKey.Price})
                .GroupBy(user => user.Name)
                .OrderByDescending(user => user.Count())
                .Take(5)
                .Select(x => new 
                { 
                    Name = x.Key,
                    OrdersCount = x.Count() 
                })
                .ToList();

            // Task 27
            // Get users who:
            // - are older than 18;
            // - are active;
            // - have made at least one order;
            // - have spent more than 10,000 in total.
            //
            // Result:
            // Name
            // TotalSpent


            var result27 = users.Where(user => user.Age > 18 && user.IsActive)
                .Join(orders,
                userKey => userKey.Id,
                orderKey => orderKey.UserId,
                (userKey, orderKey) => new { userKey.Name, orderKey.Price })
                .GroupBy(user => user.Name)
                .Select(x => new
                {
                    Name = x.Key,
                    OrdersSumPrice = x.Sum(x => x.Price)
                }).Where(x => x.OrdersSumPrice > 10000).ToList();

            // Task 28
            // Get statistics for each user:
            // - Name
            // - OrdersCount
            // - TotalSpent
            // - AverageOrderPrice
            //
            // Example:
            // Maria
            // Orders: 3
            // Total: 4700
            // Average: 1566.67

            var result28 = users.Join(orders,
                userKey => userKey.Id,
                orderKey => orderKey.UserId,
                (userKey, orderKey) => new { userKey.Name, orderKey.Price })
                .GroupBy(user => user.Name)
                .Select(x => new
                {
                    Name = x.Key,
                    OrdersCount = x.Count(),
                    OrdersSumPrice = x.Sum(x => x.Price),
                    Average = x.Average(x => x.Price)
                })
                .ToList();

            // Task 29
            // Create a report for users who:
            // - are older than 18;
            // - are active;
            // - have at least 3 orders;
            // - have spent more than 5,000 in total.
            //
            // The order price must be calculated using:
            // Product.Price * OrderItem.Quantity
            //
            // Result for each user:
            // - Name
            // - OrdersCount
            // - TotalSpent
            // - AverageOrderPrice
            // - MostExpensiveOrder
            //
            // Sort the result by TotalSpent in descending order.
            //
            // Important:
            // One Order can contain multiple OrderItems.
            // Therefore, first calculate the total price of each Order,
            // and only then calculate the statistics for each User.

            var result29 = users.Where(user => user.Age > 18 && user.IsActive)
                .Join(orders, userKey => userKey.Id, orderKey => orderKey.UserId,
                (userKey, orderKey) => new { userKey.Name, orderKey.Id })
                .Join(orderItems, orderKey => orderKey.Id, orderItemsKey => orderItemsKey.OrderId,
                (orderKey, orderItemsKey) => new { orderKey.Name, orderKey.Id, orderItemsKey.ProductId, orderItemsKey.Quantity })
                .Join(products, orderItemsKey => orderItemsKey.ProductId, productKey => productKey.Id,
                (orderItemsKey, productKey) => new { productKey.Price, orderItemsKey.Quantity, orderItemsKey.Name, orderItemsKey.Id })
                .GroupBy(x => x.Id)
                .Select(x => new
                {
                    OrderId = x.Key,
                    Name = x.First().Name,
                    OrderPrice = x.Sum(item => item.Price * item.Quantity)})
                .GroupBy(x => x.Name)
                .Select(x => new
                {
                    OrdersCount = x.Count(),
                    TotalSpend = x.Sum(order => order.OrderPrice),
                    AverageOrderPrice = x.Average(order => order.OrderPrice),
                    MostExpensiveOrder = x.Max(order => order.OrderPrice)
                })
                .Where(user => user.OrdersCount >= 3 && user.TotalSpend > 5000).ToList();
        }
    }
}