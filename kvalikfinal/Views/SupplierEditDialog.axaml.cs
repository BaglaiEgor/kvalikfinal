using Avalonia.Controls;
using Avalonia.Interactivity;
using kvalikfinal.Data;

namespace kvalikfinal.Views
{
    public partial class SupplierEditDialog : Window
    {
        private readonly Supplier? _supplier;

        public SupplierEditDialog(Supplier? supplier)
        {
            InitializeComponent();
            _supplier = supplier;

            if (_supplier != null)
            {
                NameTextBox.Text = _supplier.Name;
                PhoneTextBox.Text = _supplier.Phone;
                ContactPersonTextBox.Text = _supplier.ContactPerson;
                EmailTextBox.Text = _supplier.Email;
            }
        }

        private async void SaveButton_Click(object? sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text)) return;

            if (_supplier == null)
            {
                var newSupplier = new Supplier
                {
                    Name = NameTextBox.Text.Trim(),
                    Phone = PhoneTextBox.Text.Trim(),
                    ContactPerson = ContactPersonTextBox.Text.Trim(),
                    Email = EmailTextBox.Text.Trim()
                };
                App.dbContext.Suppliers.Add(newSupplier);
            }
            else
            {
                _supplier.Name = NameTextBox.Text.Trim();
                _supplier.Phone = PhoneTextBox.Text.Trim();
                _supplier.ContactPerson = ContactPersonTextBox.Text.Trim();
                _supplier.Email = EmailTextBox.Text.Trim();
                App.dbContext.Suppliers.Update(_supplier);
            }

            await App.dbContext.SaveChangesAsync();
            Close();
        }

        private void CancelButton_Click(object? sender, RoutedEventArgs e) => Close();
    }
}
