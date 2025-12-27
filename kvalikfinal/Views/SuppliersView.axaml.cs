using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Linq;
using kvalikfinal.Data;

namespace kvalikfinal.Views
{
    public partial class SuppliersView : UserControl
    {
        private SupplierEditDialog? _openDialog;

        public SuppliersView()
        {
            InitializeComponent();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var query = App.dbContext.Suppliers.AsQueryable();

            var search = SearchTextBox.Text?.ToLower();
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(s => s.Name.ToLower().Contains(search));

            var suppliers = query.OrderBy(s => s.Name).ToList();
            SuppliersGrid.ItemsSource = suppliers;
        }

        private void SearchTextBox_TextChanged(object? sender, TextChangedEventArgs e) => ApplyFilters();
        private void ResetFiltersButton_Click(object? sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            ApplyFilters();
        }

        private void AddSupplierButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_openDialog != null) return;

            _openDialog = new SupplierEditDialog(null);
            _openDialog.Closed += (s, args) =>
            {
                _openDialog = null;
                ApplyFilters();
            };
            _openDialog.Show();
        }

        private void EditSupplierButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_openDialog != null) return;

            if (sender is Button btn && btn.Tag is Supplier supplier)
            {
                _openDialog = new SupplierEditDialog(supplier);
                _openDialog.Closed += (s, args) =>
                {
                    _openDialog = null;
                    ApplyFilters();
                };
                _openDialog.Show();
            }
        }

        private async void DeleteSupplierButton_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Supplier supplier)
            {
                if (App.dbContext.Deliveries.Any(d => d.SupplierId == supplier.SupplierId))
                {
                    var warning = MessageBoxManager.GetMessageBoxStandard(
                        "Cannot Delete",
                        "This supplier has deliveries and cannot be deleted.",
                        ButtonEnum.Ok,
                        Icon.Warning);
                    await warning.ShowAsync();
                    return;
                }

                var msgBox = MessageBoxManager.GetMessageBoxStandard(
                    "Confirm Delete",
                    $"Are you sure you want to delete supplier \"{supplier.Name}\"?",
                    ButtonEnum.YesNo,
                    Icon.Question);
                var result = await msgBox.ShowAsync();
                if (result == ButtonResult.Yes)
                {
                    App.dbContext.Suppliers.Remove(supplier);
                    await App.dbContext.SaveChangesAsync();
                    ApplyFilters();
                }
            }
        }
    }
}
