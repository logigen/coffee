using Coffee.Models;
using Coffee.Windows;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

public AddOrderWindow(DataManager dataManager)
{
    InitializeComponent();
    _dataManager = dataManager ?? throw new ArgumentNullException(nameof(dataManager));
    _orderItems = new ObservableCollection<OrderItemViewModel>();
    OrderItemsListView.ItemsSource = _orderItems;

    LoadProducts();
    LoadClients();

    CategoryFilter.SelectedIndex = 0;
    ClientComboBox.ItemsSource = _dataManager.GetAllClients();
}

private void LoadProducts()
{
    _allProducts = _dataManager.GetAllProducts().Where(p => p.IsAvailable).ToList();
    ProductsList.ItemsSource = _allProducts;
}

private void LoadClients()
{
    ClientComboBox.ItemsSource = _dataManager.GetAllClients();
}

private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
{
    FilterProducts();
}

private void CategoryFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
{
    FilterProducts();
}

private void FilterProducts()
{
    string? searchText = SearchBox.Text?.ToLower();
    string selectedCategory = ((CategoryFilter.SelectedItem as ComboBoxItem)?.Content as string) ?? "Все категории";

    var filteredProducts = _allProducts.Where(p =>
        (selectedCategory == "Все категории" || p.Category == selectedCategory) &&
        (string.IsNullOrEmpty(searchText) || (p.Name?.ToLower().Contains(searchText) ?? false))
    );

    ProductsList.ItemsSource = filteredProducts;
}

private void ProductsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
{
    var selectedProduct = ProductsList.SelectedItem as Product;
    if (selectedProduct != null && selectedProduct.IsAvailable)
    {
        AddProductToOrder(selectedProduct);
    }
}

private void AddToOrderButton_Click(object sender, RoutedEventArgs e)
{
    var button = sender as Button;
    var product = button?.DataContext as Product;
    if (product != null && product.IsAvailable)
    {
        AddProductToOrder(product);
    }
}

private void RemoveItemButton_Click(object sender, RoutedEventArgs e)
{
    var button = sender as Button;
    var item = button?.DataContext as OrderItemViewModel;
    if (item != null)
    {
        _orderItems.Remove(item);
        UpdateTotalAmount();
    }
}

private void AddProductToOrder(Product product)
{
    var existingItem = _orderItems.FirstOrDefault(item => item.ProductId == product.Id);
    if (existingItem != null)
    {
        existingItem.Quantity++;
        existingItem.TotalPrice = existingItem.Quantity * product.Price;
    }
    else
    {
        _orderItems.Add(new OrderItemViewModel
        {
            ProductId = product.Id,
            ProductName = product.Name ?? "Unknown",
            Quantity = 1,
            Price = product.Price,
            TotalPrice = product.Price
        });
    }

    UpdateTotalAmount();
}

private void UpdateTotalAmount()
{
    decimal total = _orderItems.Sum(item => item.TotalPrice);
    TotalAmountTextBlock.Text = $"{total:N0}";
}

private void ClientComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
{
    var selectedClient = ClientComboBox.SelectedItem as Client;
    DeliveryAddressTextBox.Text = selectedClient?.Address?.ToString() ?? "Выберите клиента";
}

private void NewClientButton_Click(object sender, RoutedEventArgs e)
{
    var newClientWindow = new ManageClientWindow();
    if (newClientWindow.ShowDialog() == true)
    {
        if (newClientWindow.NewClient != null)
        {
            _dataManager.AddClient(newClientWindow.NewClient);
            LoadClients();
            ClientComboBox.SelectedIndex = ClientComboBox.Items.Count - 1;
        }
    }
}

private void SaveOrderButton_Click(object sender, RoutedEventArgs e)
{
    if (ClientComboBox.SelectedItem == null)
    {
        MessageBox.Show("Выберите клиента", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
    }

    if (_orderItems.Count == 0)
    {
        MessageBox.Show("Добавьте товары в заказ", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
    }

    if (PaymentTypeComboBox.SelectedItem == null)
    {
        MessageBox.Show("Выберите способ оплаты", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
    }

    if (DeliveryInstructionsTextBox.Text.Length > 500)
    {
        MessageBox.Show("Инструкции по доставке не должны превышать 500 символов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
    }

    try
    {
        var client = ClientComboBox.SelectedItem as Client;
        var orderItems = _orderItems.Select(vm => new OrderItem
        {
            ProductId = vm.ProductId,
            Quantity = vm.Quantity,
            Price = vm.Price
        }).ToList();

        var order = new Order
        {
            ClientId = client!.Id,
            ItemsJson = JsonConvert.SerializeObject(orderItems),
            Status = OrderStatus.New,
            DeliveryAddress = client.Address ?? new Address { Street = "Не указана", BuildingNumber = "0" },
            TotalAmount = _orderItems.Sum(item => item.TotalPrice),
            PaymentType = (PaymentType)PaymentTypeComboBox.SelectedIndex,
            DeliveryInstructions = DeliveryInstructionsTextBox.Text,
            IsPaid = false,
            CreatedAt = DateTime.Now,
            EstimatedDeliveryTime = DateTime.Now.AddHours(1)
        };

        _dataManager.AddOrder(order);

        MessageBox.Show($"Заказ №{order.OrderNumber} успешно создан", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        DialogResult = true;
        Close();
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка при создании заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

private void CancelButton_Click(object sender, RoutedEventArgs e)
{
    DialogResult = false;
    Close();
}
}

public class OrderItemViewModel
{
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal TotalPrice { get; set; }
}