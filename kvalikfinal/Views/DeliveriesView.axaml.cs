using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Linq;
using kvalikfinal.Data;

namespace kvalikfinal.Views
{
    public partial class DeliveriesView : UserControl
    {
        private DeliveryEditDialog? _currentEditDialog;
        public DeliveriesView()
        {
            InitializeComponent();
            LoadSuppliers();
            ApplyFilters();
        }

        private void LoadSuppliers()
        {
            var suppliers = App.dbContext.Suppliers
                .OrderBy(s => s.Name)
                .ToList();

            var allSuppliersItem = new Supplier { SupplierId = 0, Name = "Все поставщики" };
            suppliers.Insert(0, allSuppliersItem);

            SupplierFilter.ItemsSource = suppliers;
            SupplierFilter.SelectedIndex = 0;
        }

        private void SupplierFilter_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var query = App.dbContext.Deliveries
                .Include(d => d.Supplier)
                .Include(d => d.Warehouse)
                .AsQueryable();

            if (SupplierFilter.SelectedItem is Supplier supplier && supplier.SupplierId != 0)
                query = query.Where(d => d.SupplierId == supplier.SupplierId);

            if (DateFilter.SelectedDate.HasValue)
            {
                var date = DateFilter.SelectedDate.Value.Date;
                query = query.Where(d => d.DeliveryDate.ToDateTime(TimeOnly.MinValue) == date);
            }

            DeliveriesGrid.ItemsSource = query.OrderBy(d => d.DeliveryDate).ToList();
        }


        private void DateFilter_SelectedDateChanged(object? sender, DatePickerSelectedValueChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ResetFiltersButton_Click(object? sender, RoutedEventArgs e)
        {
            SupplierFilter.SelectedItem = null;
            DateFilter.SelectedDate = null;
            SupplierFilter.SelectedIndex = 0;
            ApplyFilters();
        }

        private void AddDeliveryButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_currentEditDialog != null)
            {
                _currentEditDialog.Focus();
                return;
            }

            _currentEditDialog = new DeliveryEditDialog(null);
            _currentEditDialog.Closed += (s, args) => _currentEditDialog = null;
            _currentEditDialog.Show();
        }
        private void EditDeliveryButton_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not Delivery delivery)
                return;

            if (_currentEditDialog != null)
            {
                _currentEditDialog.Focus();
                return;
            }

            _currentEditDialog = new DeliveryEditDialog(delivery);
            _currentEditDialog.Closed += (s, args) => _currentEditDialog = null;
            _currentEditDialog.Show();
        }

        private async void DeleteDeliveryButton_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Delivery delivery)
            {
                var msgBox = MessageBoxManager.GetMessageBoxStandard(
                    "Confirm Delete",
                    $"Are you sure you want to delete delivery from {delivery.DeliveryDate.ToDateTime(TimeOnly.MinValue):d}?",
                    ButtonEnum.YesNo,
                    Icon.Question);
                var result = await msgBox.ShowAsync();
                if (result == ButtonResult.Yes)
                {
                    var items = App.dbContext.DeliveryItems.Where(di => di.DeliveryId == delivery.DeliveryId).ToList();
                    App.dbContext.DeliveryItems.RemoveRange(items);
                    App.dbContext.Deliveries.Remove(delivery);
                    await App.dbContext.SaveChangesAsync();
                    ApplyFilters();
                }
            }
        }

        private void ViewItemsButton_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Delivery delivery)
            {
                var dialog = new DeliveryItemsDialog(delivery.DeliveryId);
                dialog.Show();
            }
        }
    }
}
