using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Linq;
using kvalikfinal.Data;

namespace kvalikfinal.Views
{
    public partial class DeliveryEditDialog : Window
    {
        private Delivery? _delivery;

        public DeliveryEditDialog(Delivery? delivery)
        {
            InitializeComponent();
            _delivery = delivery;

            SupplierCombo.ItemsSource = App.dbContext.Suppliers.OrderBy(x => x.Name).ToList();
            WarehouseCombo.ItemsSource = App.dbContext.Warehouses.OrderBy(x => x.Name).ToList();

            DeliveryDate.SelectedDate = (_delivery == null)
                ? DateTime.Now
                : _delivery.DeliveryDate.ToDateTime(TimeOnly.MinValue);

            if (_delivery != null)
            {
                SupplierCombo.SelectedItem = _delivery.Supplier;
                WarehouseCombo.SelectedItem = _delivery.Warehouse;
                StatusBox.Text = _delivery.Status;
            }
        }

        private async void Save_Click(object? s, RoutedEventArgs e)
        {
            if (SupplierCombo.SelectedItem is not Supplier sup ||
                WarehouseCombo.SelectedItem is not Warehouse wh ||
                DeliveryDate.SelectedDate is not DateTimeOffset dto) return;

            var date = DateOnly.FromDateTime(dto.DateTime);

            var statusText = string.IsNullOrWhiteSpace(StatusBox.Text) ? "В ожидании" : StatusBox.Text;

            if (_delivery == null)
            {
                _delivery = new Delivery
                {
                    SupplierId = sup.SupplierId,
                    WarehouseId = wh.WarehouseId,
                    DeliveryDate = date,
                    Status = statusText
                };
                App.dbContext.Deliveries.Add(_delivery);
            }
            else
            {
                _delivery.SupplierId = sup.SupplierId;
                _delivery.WarehouseId = wh.WarehouseId;
                _delivery.DeliveryDate = date;
                _delivery.Status = statusText;
            }

            await App.dbContext.SaveChangesAsync();
            Close();
        }

        private void Cancel_Click(object? s, RoutedEventArgs e) => Close();
    }
}
