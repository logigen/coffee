using Coffee.Windows;
using System.Text.RegularExpressions;
using System.Windows;
using System;

public ManageClientWindow(Client client = null)
{
    InitializeComponent();
    _client = client ?? new Client();

    if (_client.Id != 0)
    {
        FullNameTextBox.Text = _client.FullName;
        PhoneTextBox.Text = _client.Phone;
        EmailTextBox.Text = _client.Email;
        StreetTextBox.Text = _client.Address?.Street;
        BuildingNumberTextBox.Text = _client.Address?.BuildingNumber;
        ApartmentNumberTextBox.Text = _client.Address?.ApartmentNumber;
        EntranceTextBox.Text = _client.Address?.Entrance;
        FloorTextBox.Text = _client.Address?.Floor;
        PostalCodeTextBox.Text = _client.Address?.PostalCode;
        NotesTextBox.Text = _client.Notes;
    }
}

private void SaveClientButton_Click(object sender, RoutedEventArgs e)
{
    if (string.IsNullOrWhiteSpace(FullNameTextBox.Text))
    {
        MessageBox.Show("Введите ФИО клиента.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
    }

    if (string.IsNullOrWhiteSpace(PhoneTextBox.Text) || !Regex.IsMatch(PhoneTextBox.Text, @"^\+?\d{10,15}$"))
    {
        MessageBox.Show("Введите корректный номер телефона.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
    }

    if (!string.IsNullOrWhiteSpace(EmailTextBox.Text) && !Regex.IsMatch(EmailTextBox.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
    {
        MessageBox.Show("Введите корректный email.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
    }

    _client.FullName = FullNameTextBox.Text;
    _client.Phone = PhoneTextBox.Text;
    _client.Email = EmailTextBox.Text;
    _client.Address = new Address
    {
        Street = StreetTextBox.Text,
        BuildingNumber = BuildingNumberTextBox.Text,
        ApartmentNumber = ApartmentNumberTextBox.Text,
        Entrance = EntranceTextBox.Text,
        Floor = FloorTextBox.Text,
        PostalCode = PostalCodeTextBox.Text,
        City = "Москва"
    };
    _client.Notes = NotesTextBox.Text;

    try
    {
        if (_client.Id == 0)
        {
            DataManager.Instance.AddClient(_client);
        }
        else
        {
            DataManager.Instance.UpdateClient(_client);
        }

        DialogResult = true;
        Close();
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка при сохранении клиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

private void CancelButton_Click(object sender, RoutedEventArgs e)
{
    DialogResult = false;
    Close();
}
}