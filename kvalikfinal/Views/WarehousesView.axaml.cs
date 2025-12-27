using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Linq;
using kvalikfinal.Data;

namespace kvalikfinal.Views
{
    public partial class WarehousesView : UserControl
    {
        private WarehouseEditDialog? _openWarehouseDialog;
        private ZoneEditDialog? _openZoneDialog;

        public WarehousesView()
        {
            InitializeComponent();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var query = App.dbContext.Warehouses.AsQueryable();
            var search = SearchTextBox.Text?.ToLower();
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(w => w.Name.ToLower().Contains(search));

            var warehouses = query.OrderBy(w => w.Name).ToList();
            WarehousesGrid.ItemsSource = warehouses;

            if (warehouses.Any())
            {
                WarehousesGrid.SelectedItem = warehouses.First();
                LoadZones(warehouses.First().WarehouseId);
            }
            else
                ZonesGrid.ItemsSource = null;
        }

        private void SearchTextBox_TextChanged(object? sender, TextChangedEventArgs e) => ApplyFilters();
        private void ResetFiltersButton_Click(object? sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            ApplyFilters();
        }

        private void WarehousesGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (WarehousesGrid.SelectedItem is Warehouse warehouse)
                LoadZones(warehouse.WarehouseId);
            else
                ZonesGrid.ItemsSource = null;
        }

        private void LoadZones(int warehouseId)
        {
            var zones = App.dbContext.StorageZones
                .Where(z => z.WarehouseId == warehouseId)
                .OrderBy(z => z.ZoneCode)
                .ToList();
            ZonesGrid.ItemsSource = zones;
        }

        private void AddWarehouseButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_openWarehouseDialog != null) return;

            _openWarehouseDialog = new WarehouseEditDialog(null);
            _openWarehouseDialog.Closed += (s, args) =>
            {
                _openWarehouseDialog = null;
                ApplyFilters();
            };
            _openWarehouseDialog.Show();
        }

        private void EditWarehouseButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_openWarehouseDialog != null) return;

            if (sender is Button btn && btn.Tag is Warehouse warehouse)
            {
                _openWarehouseDialog = new WarehouseEditDialog(warehouse);
                _openWarehouseDialog.Closed += (s, args) =>
                {
                    _openWarehouseDialog = null;
                    ApplyFilters();
                };
                _openWarehouseDialog.Show();
            }
        }

        private async void DeleteWarehouseButton_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Warehouse warehouse)
            {
                var zonesExist = App.dbContext.StorageZones.Any(z => z.WarehouseId == warehouse.WarehouseId);
                if (zonesExist)
                {
                    var warning = MessageBoxManager.GetMessageBoxStandard(
                        "Cannot Delete",
                        "Warehouse has storage zones and cannot be deleted.",
                        ButtonEnum.Ok,
                        Icon.Warning);
                    await warning.ShowAsync();
                    return;
                }

                var msgBox = MessageBoxManager.GetMessageBoxStandard(
                    "Confirm Delete",
                    $"Are you sure you want to delete \"{warehouse.Name}\"?",
                    ButtonEnum.YesNo,
                    Icon.Question);
                var result = await msgBox.ShowAsync();
                if (result == ButtonResult.Yes)
                {
                    App.dbContext.Warehouses.Remove(warehouse);
                    await App.dbContext.SaveChangesAsync();
                    ApplyFilters();
                }
            }
        }

        private void AddZoneButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_openZoneDialog != null) return;
            if (WarehousesGrid.SelectedItem is Warehouse warehouse)
            {
                _openZoneDialog = new ZoneEditDialog(null, warehouse.WarehouseId);
                _openZoneDialog.Closed += (s, args) =>
                {
                    _openZoneDialog = null;
                    LoadZones(warehouse.WarehouseId);
                };
                _openZoneDialog.Show();
            }
        }

        private void EditZoneButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_openZoneDialog != null) return;
            if (sender is Button btn && btn.Tag is StorageZone zone)
            {
                _openZoneDialog = new ZoneEditDialog(zone, zone.WarehouseId);
                _openZoneDialog.Closed += (s, args) =>
                {
                    _openZoneDialog = null;
                    LoadZones(zone.WarehouseId);
                };
                _openZoneDialog.Show();
            }
        }

        private async void DeleteZoneButton_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is StorageZone zone)
            {
                var msgBox = MessageBoxManager.GetMessageBoxStandard(
                    "Confirm Delete",
                    $"Are you sure you want to delete zone \"{zone.ZoneCode}\"?",
                    ButtonEnum.YesNo,
                    Icon.Question);
                var result = await msgBox.ShowAsync();
                if (result == ButtonResult.Yes)
                {
                    App.dbContext.StorageZones.Remove(zone);
                    await App.dbContext.SaveChangesAsync();
                    LoadZones(zone.WarehouseId);
                }
            }
        }
    }
}
