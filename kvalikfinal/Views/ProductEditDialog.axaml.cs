using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Linq;
using kvalikfinal.Data;

namespace kvalikfinal.Views
{
    public partial class ProductEditDialog : Window
    {
        private readonly Product? _product;

        public ProductEditDialog(Product? product)
        {
            InitializeComponent();
            _product = product;
            LoadCategories();

            if (_product != null)
            {
                NameTextBox.Text = _product.Name;
                ArticleTextBox.Text = _product.Article;
                UnitTextBox.Text = _product.Unit;
                MinStockBox.Value = _product.MinStock;
                CategoryComboBox.SelectedItem = App.dbContext.ProductCategories
                    .FirstOrDefault(c => c.CategoryId == _product.CategoryId);
            }
        }

        private void LoadCategories()
        {
            var categories = App.dbContext.ProductCategories.ToList();
            CategoryComboBox.ItemsSource = categories;
            if (_product == null)
                CategoryComboBox.SelectedIndex = 0;
        }

        private async void SaveButton_Click(object? sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text)) return;
            if (string.IsNullOrWhiteSpace(ArticleTextBox.Text)) return;
            if (CategoryComboBox.SelectedItem is not ProductCategory selectedCategory) return;

            if (_product == null)
            {
                var newProduct = new Product
                {
                    Name = NameTextBox.Text.Trim(),
                    Article = ArticleTextBox.Text.Trim(),
                    Unit = UnitTextBox.Text.Trim(),
                    MinStock = (int)MinStockBox.Value,
                    CategoryId = selectedCategory.CategoryId
                };
                App.dbContext.Products.Add(newProduct);
            }
            else
            {
                _product.Name = NameTextBox.Text.Trim();
                _product.Article = ArticleTextBox.Text.Trim();
                _product.Unit = UnitTextBox.Text.Trim();
                _product.MinStock = (int)MinStockBox.Value;
                _product.CategoryId = selectedCategory.CategoryId;
                App.dbContext.Products.Update(_product);
            }

            await App.dbContext.SaveChangesAsync();
            Close();
        }

        private void CancelButton_Click(object? sender, RoutedEventArgs e) => Close();
    }
}
