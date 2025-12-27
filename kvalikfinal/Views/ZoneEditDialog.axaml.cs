using Avalonia.Controls;
using Avalonia.Interactivity;
using kvalikfinal.Data;

namespace kvalikfinal.Views
{
    public partial class ZoneEditDialog : Window
    {
        private readonly StorageZone? _zone;
        private readonly int _warehouseId;

        public ZoneEditDialog(StorageZone? zone, int warehouseId)
        {
            InitializeComponent();
            _zone = zone;
            _warehouseId = warehouseId;

            if (_zone != null)
            {
                CodeTextBox.Text = _zone.ZoneCode;
                DescriptionTextBox.Text = _zone.Description;
            }
        }

        private async void SaveButton_Click(object? sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CodeTextBox.Text)) return;

            if (_zone == null)
            {
                var newZone = new StorageZone
                {
                    WarehouseId = _warehouseId,
                    ZoneCode = CodeTextBox.Text.Trim(),
                    Description = DescriptionTextBox.Text.Trim()
                };
                App.dbContext.StorageZones.Add(newZone);
            }
            else
            {
                _zone.ZoneCode = CodeTextBox.Text.Trim();
                _zone.Description = DescriptionTextBox.Text.Trim();
                App.dbContext.StorageZones.Update(_zone);
            }

            await App.dbContext.SaveChangesAsync();
            Close();
        }

        private void CancelButton_Click(object? sender, RoutedEventArgs e) => Close();
    }
}
