using Avalonia.Controls;
using Avalonia.Interactivity;
using kvalikfinal.Data;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Linq;

namespace kvalikfinal.Views
{
    public partial class ProductsView : UserControl
    {
        private ProductEditDialog? _openDialog;

        public ProductsView()
        {
            InitializeComponent();
            LoadCategories();
            ApplyFilters();
        }

        private void LoadCategories()
        {
            var categories = App.dbContext.ProductCategories.ToList();
            var allItem = new ProductCategory { CategoryId = 0, Name = "Все продукты" };
            categories.Insert(0, allItem);
            CategoryFilter.ItemsSource = categories;
            CategoryFilter.SelectedIndex = 0;
        }

        private void ApplyFilters()
        {
            var query = App.dbContext.Products.Include(p => p.Category).AsQueryable();

            var search = SearchTextBox.Text?.ToLower();
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.ToLower().Contains(search));

            if (CategoryFilter.SelectedItem is ProductCategory category && category.CategoryId != 0)
                query = query.Where(p => p.CategoryId == category.CategoryId);

            var products = query
                .Select(p => new
                {
                    p.ProductId,
                    p.Name,
                    p.Unit,
                    p.MinStock,
                    Category = p.Category,
                    CurrentStock = App.dbContext.StockBalances
                        .Where(sb => sb.ProductId == p.ProductId)
                        .Sum(sb => (int?)sb.Quantity) ?? 0
                })
                .ToList();

            ProductsGrid.ItemsSource = products;
        }

        private void FilterChanged(object? sender, SelectionChangedEventArgs e) => ApplyFilters();
        private void SearchTextBox_TextChanged(object? sender, TextChangedEventArgs e) => ApplyFilters();

        private void ResetFiltersButton_Click(object? sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            CategoryFilter.SelectedIndex = 0;
            ApplyFilters();
        }

        private void AddProductButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_openDialog != null) return;

            _openDialog = new ProductEditDialog(null);
            _openDialog.Closed += (s, args) =>
            {
                _openDialog = null;
                ApplyFilters();
            };
            _openDialog.Show();
        }

        private void EditButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_openDialog != null) return;

            if (sender is Button btn && btn.Tag != null)
            {
                var productId = (int)btn.Tag.GetType().GetProperty("ProductId").GetValue(btn.Tag);
                var product = App.dbContext.Products.Find(productId);
                if (product == null) return;

                _openDialog = new ProductEditDialog(product);
                _openDialog.Closed += (s, args) =>
                {
                    _openDialog = null;
                    ApplyFilters();
                };
                _openDialog.Show();
            }
        }

        private async void DeleteButton_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                var productId = (int)btn.Tag.GetType().GetProperty("ProductId").GetValue(btn.Tag);
                var product = App.dbContext.Products.Find(productId);
                if (product == null) return;

                if (App.dbContext.DeliveryItems.Any(di => di.ProductId == productId))
                {
                    var warning = MessageBoxManager.GetMessageBoxStandard(
                        "Cannot Delete",
                        "This product is used in delivery items and cannot be deleted.",
                        ButtonEnum.Ok,
                        Icon.Warning);
                    await warning.ShowAsync();
                    return;
                }

                var msgBox = MessageBoxManager.GetMessageBoxStandard(
                    "Confirm Delete",
                    $"Are you sure you want to delete \"{product.Name}\"?",
                    ButtonEnum.YesNo,
                    Icon.Question);

                var result = await msgBox.ShowAsync();
                if (result == ButtonResult.Yes)
                {
                    App.dbContext.Products.Remove(product);
                    await App.dbContext.SaveChangesAsync();
                    ApplyFilters();
                }
            }
        }
    }
}
