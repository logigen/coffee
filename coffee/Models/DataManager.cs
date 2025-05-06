using Coffee.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System;

public static DataManager Instance => _instance;

private DataManager()
{
    _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
    _filePaths = new Dictionary<Type, string>
        {
            { typeof(Product), Path.Combine(_dataPath, "products.json") },
            { typeof(Client), Path.Combine(_dataPath, "clients.json") },
            { typeof(Courier), Path.Combine(_dataPath, "couriers.json") },
            { typeof(Vehicle), Path.Combine(_dataPath, "vehicles.json") },
            { typeof(Order), Path.Combine(_dataPath, "orders.json") }
        };

    InitializeDataFiles();
}

private void InitializeDataFiles()
{
    try
    {
        Directory.CreateDirectory(_dataPath);

        if (!File.Exists(_filePaths[typeof(Product)]))
        {
            SaveData(new List<Product>
                {
                    new Product { Id = 1, Name = "Кофе Латте", Category = "Кофе", Price = 200, Description = "Кофе с молоком", IsAvailable = true },
                    new Product { Id = 2, Name = "Чизкейк", Category = "Десерты", Price = 350, Description = "Классический чизкейк", IsAvailable = true },
                    new Product { Id = 3, Name = "Чай зеленый", Category = "Напитки", Price = 150, Description = "Зеленый чай", IsAvailable = true }
                });
        }

        if (!File.Exists(_filePaths[typeof(Client)]))
        {
            SaveData(new List<Client>
                {
                    new Client { Id = 1, FullName = "Петров Алексей Иванович", Phone = "+79031234567", Email = "petrov@example.com", Address = new Address { Street = "ул. Ленина", BuildingNumber = "15", ApartmentNumber = "42", Entrance = "2", Floor = "5", City = "Москва" } },
                    new Client { Id = 2, FullName = "Сидорова Елена Петровна", Phone = "+79169876543", Email = "sidorova@example.com", Address = new Address { Street = "пр. Победы", BuildingNumber = "78", ApartmentNumber = "15", Entrance = "1", Floor = "3", City = "Москва" } }
                });
        }

        if (!File.Exists(_filePaths[typeof(Courier)]))
        {
            SaveData(new List<Courier>
                {
                    new Courier { Id = 1, FullName = "Соколов Михаил Андреевич", Phone = "+79012223344", Email = "sokolov@example.com", VehicleId = 1, IsActive = true },
                    new Courier { Id = 2, FullName = "Морозова Анна Владимировна", Phone = "+79154445566", Email = "morozova@example.com", VehicleId = 2, IsActive = true }
                });
        }

        if (!File.Exists(_filePaths[typeof(Vehicle)]))
        {
            SaveData(new List<Vehicle>
                {
                    new Vehicle { Id = 1, Type = VehicleType.Scooter, Model = "Honda PCX", RegistrationNumber = "А123ВС77", Color = "Красный", IsActive = true },
                    new Vehicle { Id = 2, Type = VehicleType.Bicycle, Model = "Giant Escape", RegistrationNumber = "-", Color = "Черный", IsActive = true }
                });
        }

        if (!File.Exists(_filePaths[typeof(Order)]))
        {
            SaveData(new List<Order>
                {
                    new Order
                    {
                        Id = 1,
                        OrderNumber = "250505-001",
                        ClientId = 1,
                        ItemsJson = JsonConvert.SerializeObject(new List<OrderItem>
                        {
                            new OrderItem { ProductId = 1, Quantity = 2, Price = 200 },
                            new OrderItem { ProductId = 3, Quantity = 1, Price = 150 }
                        }),
                        Status = OrderStatus.Completed,
                        DeliveryAddress = new Address { Street = "ул. Ленина", BuildingNumber = "15", ApartmentNumber = "42", Entrance = "2", Floor = "5", City = "Москва" },
                        TotalAmount = 550,
                        PaymentType = PaymentType.Card,
                        CourierId = 1,
                        DeliveryInstructions = "Позвонить за 10 минут",
                        CreatedAt = DateTime.Now.AddHours(-5),
                        CompletedAt = DateTime.Now.AddHours(-4)
                    }
                });
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка при инициализации файлов данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

private List<T> LoadData<T>()
{
    string filePath = _filePaths[typeof(T)];
    try
    {
        if (!File.Exists(filePath))
        {
            return new List<T>();
        }

        string json = File.ReadAllText(filePath);
        var data = JsonConvert.DeserializeObject<List<T>>(json);
        return data ?? new List<T>();
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка при загрузке данных из {Path.GetFileName(filePath)}: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        return new List<T>();
    }
}

private void SaveData<T>(List<T> data)
{
    string filePath = _filePaths[typeof(T)];
    try
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка при сохранении данных в {Path.GetFileName(filePath)}: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

public List<Product> GetAllProducts()
{
    return LoadData<Product>();
}

public void AddProduct(Product product)
{
    if (product == null) throw new ArgumentNullException(nameof(product));
    var products = LoadData<Product>();
    if (product.Id == 0)
    {
        product.Id = products.Count > 0 ? products.Max(p => p.Id) + 1 : 1;
    }
    products.Add(product);
    SaveData(products);
}

public void UpdateProduct(Product product)
{
    if (product == null) throw new ArgumentNullException(nameof(product));
    var products = LoadData<Product>();
    int index = products.FindIndex(p => p.Id == product.Id);
    if (index != -1)
    {
        products[index] = product;
        SaveData(products);
    }
}

public void DeleteProduct(int productId)
{
    var products = LoadData<Product>();
    products.RemoveAll(p => p.Id == productId);
    SaveData(products);
}

public List<Client> GetAllClients()
{
    return LoadData<Client>();
}

public void AddClient(Client client)
{
    if (client == null) throw new ArgumentNullException(nameof(client));
    var clients = LoadData<Client>();
    if (client.Id == 0)
    {
        client.Id = clients.Count > 0 ? clients.Max(c => c.Id) + 1 : 1;
    }
    clients.Add(client);
    SaveData(clients);
}

public void UpdateClient(Client client)
{
    if (client == null) throw new ArgumentNullException(nameof(client));
    var clients = LoadData<Client>();
    int index = clients.FindIndex(c => c.Id == client.Id);
    if (index != -1)
    {
        clients[index] = client;
        SaveData(clients);
    }
}

public void DeleteClient(int clientId)
{
    var clients = LoadData<Client>();
    clients.RemoveAll(c => c.Id == clientId);
    SaveData(clients);
}

public List<Courier> GetAllCouriers()
{
    return LoadData<Courier>();
}

public List<Courier> GetAvailableCouriers()
{
    var orders = LoadData<Order>();
    var couriers = LoadData<Courier>();
    var activeCourierIds = orders
        .Where(o => o.Status == OrderStatus.InProgress || o.Status == OrderStatus.InDelivery)
        .Select(o => o.CourierId)
        .Where(id => id.HasValue)
        .ToList();
    return couriers.Where(c => !activeCourierIds.Contains(c.Id) && c.IsActive).ToList();
}

public void AddCourier(Courier courier)
{
    if (courier == null) throw new ArgumentNullException(nameof(courier));
    var couriers = LoadData<Courier>();
    if (courier.Id == 0)
    {
        courier.Id = couriers.Count > 0 ? couriers.Max(c => c.Id) + 1 : 1;
    }
    couriers.Add(courier);
    SaveData(couriers);
}

public void UpdateCourier(Courier courier)
{
    if (courier == null) throw new ArgumentNullException(nameof(courier));
    var couriers = LoadData<Courier>();
    int index = couriers.FindIndex(c => c.Id == courier.Id);
    if (index != -1)
    {
        couriers[index] = courier;
        SaveData(couriers);
    }
}

public void DeleteCourier(int courierId)
{
    var couriers = LoadData<Courier>();
    couriers.RemoveAll(c => c.Id == courierId);
    SaveData(couriers);
}

public List<Vehicle> GetAllVehicles()
{
    return LoadData<Vehicle>();
}

public List<Vehicle> GetAvailableVehicles()
{
    var couriers = LoadData<Courier>();
    var vehicles = LoadData<Vehicle>();
    var activeVehicleIds = couriers
        .Where(c => c.VehicleId.HasValue && c.IsActive)
        .Select(c => c.VehicleId)
        .ToList();
    return vehicles.Where(v => !activeVehicleIds.Contains(v.Id) && v.IsActive).ToList();
}

public void AddVehicle(Vehicle vehicle)
{
    if (vehicle == null) throw new ArgumentNullException(nameof(vehicle));
    var vehicles = LoadData<Vehicle>();
    if (vehicle.Id == 0)
    {
        vehicle.Id = vehicles.Count > 0 ? vehicles.Max(v.wikipedia: Id) +1 : 1;
        }
        vehicles.Add(vehicle);
SaveData(vehicles);
    }

    public void UpdateVehicle(Vehicle vehicle)
{
    if (vehicle == null) throw new ArgumentNullException(nameof(vehicle));
    var vehicles = LoadData<Vehicle>();
    int index = vehicles.FindIndex(v => v.Id == vehicle.Id);
    if (index != -1)
    {
        vehicles[index] = vehicle;
        SaveData(vehicles);
    }
}

public void DeleteVehicle(int vehicleId)
{
    var vehicles = LoadData<Vehicle>();
    vehicles.RemoveAll(v => v.Id == vehicleId);
    SaveData(vehicles);
}

public List<Order> GetAllOrders()
{
    return LoadData<Order>();
}

public List<Order> GetActiveOrders()
{
    return LoadData<Order>()
        .Where(o => o.Status != OrderStatus.Completed && o.Status != OrderStatus.Cancelled)
        .OrderByDescending(o => o.CreatedAt)
        .ToList();
}

public List<Order> GetCompletedOrders()
{
    return LoadData<Order>()
        .Where(o => o.Status == OrderStatus.Completed)
        .OrderByDescending(o => o.CompletedAt)
        .ToList();
}

public void AddOrder(Order order)
{
    if (order == null) throw new ArgumentNullException(nameof(order));
    var orders = LoadData<Order>();
    if (order.Id == 0)
    {
        order.Id = orders.Count > 0 ? orders.Max(o => o.Id) + 1 : 1;
    }
    order.OrderNumber = GenerateOrderNumber();
    order.CreatedAt = DateTime.Now;
    orders.Add(order);
    SaveData(orders);
}

public void UpdateOrder(Order order)
{
    if (order == null) throw new ArgumentNullException(nameof(order));
    var orders = LoadData<Order>();
    int index = orders.FindIndex(o => o.Id == order.Id);
    if (index != -1)
    {
        if (order.Status == OrderStatus.Completed && !orders[index].CompletedAt.HasValue)
        {
            order.CompletedAt = DateTime.Now;
        }
        orders[index] = order;
        SaveData(orders);
    }
}

public void DeleteOrder(int orderId)
{
    var orders = LoadData<Order>();
    orders.RemoveAll(o => o.Id == orderId);
    SaveData(orders);
}

private string GenerateOrderNumber()
{
    var orders = LoadData<Order>();
    string date = DateTime.Now.ToString("yyMMdd");
    int countToday = orders.Count(o => o.CreatedAt.Date == DateTime.Now.Date) + 1;
    return $"{date}-{countToday:D3}";
}

public int GetActiveDeliveriesCount()
{
    return LoadData<Order>().Count(o => o.Status == OrderStatus.InDelivery);
}

public int GetCompletedOrdersCount()
{
    return LoadData<Order>().Count(o => o.Status == OrderStatus.Completed && o.CompletedAt?.Date == DateTime.Now.Date);
}

public decimal GetTotalRevenue()
{
    return LoadData<Order>()
        .Where(o => o.Status == OrderStatus.Completed && o.CompletedAt?.Date == DateTime.Now.Date)
        .Sum(o => o.TotalAmount);
}
}