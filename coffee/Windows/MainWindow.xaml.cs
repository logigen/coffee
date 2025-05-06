using Coffee.Windows;
using System.Globalization;
using System.Windows.Threading;
using System.Windows;

public MainWindow()
{
    InitializeComponent();

    _dataManager = DataManager.Instance;

    _timer = new DispatcherTimer
    {
        Interval = TimeSpan.FromSeconds(1)
    };
    _timer.Tick += Timer_Tick;
    _timer.Start();

    UpdateTimeAndDate();
    LoadDashboardData();
}

private void Timer_Tick(object? sender, EventArgs e)
{
    UpdateTimeAndDate();
}

private void UpdateTimeAndDate()
{
    var culture = new CultureInfo("ru-RU");
    CurrentTimeTextBlock.Text = DateTime.Now.ToString("HH:mm:ss", culture);
    CurrentDateTextBlock.Text = DateTime.Now.ToString("dddd, d MMMM yyyy", culture);
}

private void LoadDashboardData()
{
    try
    {
        int activeDeliveries = _dataManager.GetActiveDeliveriesCount();
        int completedOrders = _dataManager.GetCompletedOrdersCount();
        decimal totalRevenue = _dataManager.GetTotalRevenue();

        ActiveDeliveriesTextBlock.Text = activeDeliveries.ToString();
        CompletedOrdersTextBlock.Text = completedOrders.ToString();
        TotalRevenueTextBlock.Text = $"{totalRevenue:N0}";
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

private void AddOrder_Click(object sender, RoutedEventArgs e)
{
    var window = new AddOrderWindow(_dataManager);
    window.ShowDialog();
    LoadDashboardData();
}

private void ViewOrders_Click(object sender, RoutedEventArgs e)
{
    var window = new OrdersListWindow(_dataManager);
    window.ShowDialog();
    LoadDashboardData();
}

private void ManageProducts_Click(object sender, RoutedEventArgs e)
{
    MessageBox.Show("Функция управления меню пока не реализована.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
}

private void ManageClients_Click(object sender, RoutedEventArgs e)
{
    var window = new ManageClientWindow();
    window.ShowDialog();
}

private void ManageCouriers_Click(object sender, RoutedEventArgs e)
{
    MessageBox.Show("Функция управления курьерами пока не реализована.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
}

private void TrackDelivery_Click(object sender, RoutedEventArgs e)
{
    MessageBox.Show("Функция отслеживания доставки пока не реализована.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
}

private void ProcessPayment_Click(object sender, RoutedEventArgs e)
{
    MessageBox.Show("Функция оплаты заказов пока не реализована.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
}

private void ShowReports_Click(object sender, RoutedEventArgs e)
{
    MessageBox.Show("Функция отчетов и статистики пока не реализована.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
}

private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
{
    _timer.Stop();
}
}