using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using kvalikfinal.Data;

namespace kvalikfinal.Views
{
    public partial class DeliveryItemsDialog : Window
    {
        private int _deliveryId;

        public DeliveryItemsDialog(int deliveryId)
        {
            InitializeComponent();
            _deliveryId = deliveryId;
            LoadProducts();
            LoadItems();
        }

        private void LoadProducts()
        {
            ProductCombo.ItemsSource = App.dbContext.Products.OrderBy(p => p.Name).ToList();
        }

        private void LoadItems()
        {
            ItemsGrid.ItemsSource = App.dbContext.DeliveryItems
                .Include(x => x.Product)
                .Where(x => x.DeliveryId == _deliveryId)
                .ToList();
        }

        private async void Add_Click(object? s, RoutedEventArgs e)
        {
            if (ProductCombo.SelectedItem is not Product p) return;
            if (!int.TryParse(QtyBox.Text, out int q) || q <= 0) return;
            if (!decimal.TryParse(PriceBox.Text, out decimal pr)) return;

            var existingItem = App.dbContext.DeliveryItems
                .FirstOrDefault(x => x.DeliveryId == _deliveryId && x.ProductId == p.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += q;
                existingItem.PurchasePrice += pr;
                App.dbContext.DeliveryItems.Update(existingItem);
            }
            else
            {
                var item = new DeliveryItem
                {
                    DeliveryId = _deliveryId,
                    ProductId = p.ProductId,
                    Quantity = q,
                    PurchasePrice = pr
                };
                App.dbContext.DeliveryItems.Add(item);
            }

            var delivery = App.dbContext.Deliveries.First(x => x.DeliveryId == _deliveryId);
            var stock = App.dbContext.StockBalances
                .FirstOrDefault(x => x.ProductId == p.ProductId && x.WarehouseId == delivery.WarehouseId && x.ZoneId == 1);

            if (stock != null) stock.Quantity += q;
            else App.dbContext.StockBalances.Add(new StockBalance
            {
                ProductId = p.ProductId,
                WarehouseId = delivery.WarehouseId,
                ZoneId = 1,
                Quantity = q
            });

            await App.dbContext.SaveChangesAsync();
            LoadItems();
        }


        private async void Delete_Click(object? s, RoutedEventArgs e)
        {
            if (s is Button b && b.Tag is DeliveryItem di)
            {
                App.dbContext.DeliveryItems.Remove(di);
                await App.dbContext.SaveChangesAsync();
                LoadItems();
            }
        }
    }
}
