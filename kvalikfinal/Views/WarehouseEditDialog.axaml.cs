using Avalonia.Controls;
using Avalonia.Interactivity;
using kvalikfinal.Data;

namespace kvalikfinal.Views
{
    public partial class WarehouseEditDialog : Window
    {
        private readonly Warehouse? _warehouse;

        public WarehouseEditDialog(Warehouse? warehouse)
        {
            InitializeComponent();
            _warehouse = warehouse;

            if (_warehouse != null)
            {
                NameTextBox.Text = _warehouse.Name;
                AddressTextBox.Text = _warehouse.Address;
            }
        }

        private async void SaveButton_Click(object? sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text)) return;
            if (string.IsNullOrWhiteSpace(AddressTextBox.Text)) return;

            if (_warehouse == null)
            {
                var newWarehouse = new Warehouse
                {
                    Name = NameTextBox.Text.Trim(),
                    Address = AddressTextBox.Text.Trim()
                };
                App.dbContext.Warehouses.Add(newWarehouse);
            }
            else
            {
                _warehouse.Name = NameTextBox.Text.Trim();
                _warehouse.Address = AddressTextBox.Text.Trim();
                App.dbContext.Warehouses.Update(_warehouse);
            }

            await App.dbContext.SaveChangesAsync();
            Close();
        }

        private void CancelButton_Click(object? sender, RoutedEventArgs e) => Close();
    }
}
