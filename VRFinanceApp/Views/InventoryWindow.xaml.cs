using VRFinanceApp.Data;
using VRFinanceApp.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VRFinanceApp.Views
{
    public partial class InventoryWindow : Window
    {
        public InventoryWindow()
        {
            InitializeComponent();
            dpDate.SelectedDate = DateTime.Today;
            LoadInventory();
        }

        private void OnlyNumber(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c))
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtItemName.Text) ||
                    cmbCategory.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(txtQuantity.Text) ||
                    string.IsNullOrWhiteSpace(txtPrice.Text))
                {
                    MessageBox.Show("Lütfen tüm alanları doldurun.");
                    return;
                }

                int quantity = int.Parse(txtQuantity.Text);
                decimal price = decimal.Parse(txtPrice.Text);
                string category = ((ComboBoxItem)cmbCategory.SelectedItem).Content.ToString()!;

                InventoryItem item = new InventoryItem
                {
                    ItemName = txtItemName.Text,
                    Category = category,
                    Quantity = quantity,
                    UnitPrice = price,
                    PurchaseDate = dpDate.SelectedDate ?? DateTime.Today,
                    Note = txtNote.Text
                };

                using (AppDbContext db = new AppDbContext())
                {
                    db.InventoryItems.Add(item);
                    db.SaveChanges();
                }

                MessageBox.Show("Envanter kaydedildi.");
                ClearForm();
                LoadInventory();
            }
            catch (Exception ex)
            {
                string errorMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : ex.Message;

                MessageBox.Show("Hata: " + errorMessage);
            }
        }

        private void LoadInventory()
        {
            using (AppDbContext db = new AppDbContext())
            {
                var items = db.InventoryItems
                    .AsEnumerable()
                    .Select(x => new
                    {
                        x.ItemName,
                        x.Category,
                        x.Quantity,
                        UnitPrice = x.UnitPrice.ToString("N0") + " ₺",
                        TotalValue = (x.Quantity * x.UnitPrice).ToString("N0") + " ₺",
                        PurchaseDate = x.PurchaseDate.ToString("dd.MM.yyyy"),
                        Note = string.IsNullOrWhiteSpace(x.Note) ? "-" : x.Note
                    })
                    .ToList();

                dgInventory.ItemsSource = items;

                decimal totalValue = db.InventoryItems
                    .AsEnumerable()
                    .Sum(x => x.Quantity * x.UnitPrice);

                txtTotalInventoryValue.Text = totalValue.ToString("N0") + " ₺";
            }
        }

        private void ClearForm()
        {
            txtItemName.Clear();
            cmbCategory.SelectedItem = null;
            txtQuantity.Clear();
            txtPrice.Clear();
            txtNote.Clear();
            dpDate.SelectedDate = DateTime.Today;
        }
    }
}