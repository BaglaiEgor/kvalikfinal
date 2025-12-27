using Avalonia.Controls;
using kvalikfinal.Views;

namespace kvalikfinal
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void btnProducts_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            mainControl.Content = new ProductsView();
        }
        private void btnWarehouses_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            mainControl.Content = new WarehousesView();
        }
        private void btnSuppliers_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            mainControl.Content = new SuppliersView();
        }
        private void btnDelivers_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            mainControl.Content = new DeliveriesView();
        }
    }
}
