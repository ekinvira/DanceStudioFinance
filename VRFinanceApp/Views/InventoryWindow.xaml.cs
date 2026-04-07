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
        private int? selectedInventoryId = null;
        private const int LowStockThreshold = 5;

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
                if (!ValidateForm())
                    return;

                int quantity = int.Parse(txtQuantity.Text);
                decimal price = decimal.Parse(txtPrice.Text);

                InventoryItem item = new InventoryItem
                {
                    ItemName = txtItemName.Text.Trim(),
                    Category = txtCategory.Text.Trim(),
                    Quantity = quantity,
                    UnitPrice = price,
                    PurchaseDate = dpDate.SelectedDate ?? DateTime.Today,
                    Note = txtNote.Text.Trim()
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

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedInventoryId == null)
                {
                    MessageBox.Show("Lütfen güncellenecek bir kayıt seçin.");
                    return;
                }

                if (!ValidateForm())
                    return;

                using (AppDbContext db = new AppDbContext())
                {
                    var item = db.InventoryItems.FirstOrDefault(x => x.Id == selectedInventoryId.Value);

                    if (item == null)
                    {
                        MessageBox.Show("Kayıt bulunamadı.");
                        return;
                    }

                    item.ItemName = txtItemName.Text.Trim();
                    item.Category = txtCategory.Text.Trim();
                    item.Quantity = int.Parse(txtQuantity.Text);
                    item.UnitPrice = decimal.Parse(txtPrice.Text);
                    item.PurchaseDate = dpDate.SelectedDate ?? DateTime.Today;
                    item.Note = txtNote.Text.Trim();

                    db.SaveChanges();
                }

                MessageBox.Show("Envanter güncellendi.");
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

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedInventoryId == null)
                {
                    MessageBox.Show("Lütfen silinecek bir kayıt seçin.");
                    return;
                }

                var result = MessageBox.Show(
                    "Seçilen envanter kaydını silmek istiyor musunuz?",
                    "Silme Onayı",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                    return;

                using (AppDbContext db = new AppDbContext())
                {
                    var item = db.InventoryItems.FirstOrDefault(x => x.Id == selectedInventoryId.Value);

                    if (item == null)
                    {
                        MessageBox.Show("Kayıt bulunamadı.");
                        return;
                    }

                    db.InventoryItems.Remove(item);
                    db.SaveChanges();
                }

                MessageBox.Show("Envanter kaydı silindi.");
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

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void dgInventory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgInventory.SelectedItem == null)
                return;

            dynamic selected = dgInventory.SelectedItem;

            selectedInventoryId = selected.Id;
            txtItemName.Text = selected.ItemName;
            txtCategory.Text = selected.Category;
            txtQuantity.Text = selected.Quantity.ToString();
            txtPrice.Text = selected.UnitPriceRaw.ToString();
            txtNote.Text = selected.NoteRaw ?? "";
            dpDate.SelectedDate = selected.PurchaseDateRaw;
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtItemName.Text) ||
                string.IsNullOrWhiteSpace(txtCategory.Text) ||
                string.IsNullOrWhiteSpace(txtQuantity.Text) ||
                string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.");
                return false;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Adet geçerli bir sayı olmalıdır.");
                return false;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Birim fiyat geçerli bir sayı olmalıdır.");
                return false;
            }

            return true;
        }

        private void LoadInventory()
        {
            using (AppDbContext db = new AppDbContext())
            {
                var items = db.InventoryItems
                    .AsEnumerable()
                    .Select(x => new
                    {
                        x.Id,
                        x.ItemName,
                        x.Category,
                        x.Quantity,
                        UnitPrice = x.UnitPrice.ToString("N0") + " ₺",
                        TotalValue = (x.Quantity * x.UnitPrice).ToString("N0") + " ₺",
                        PurchaseDate = x.PurchaseDate.ToString("dd.MM.yyyy"),
                        Note = string.IsNullOrWhiteSpace(x.Note) ? "-" : x.Note,

                        // raw values for editing
                        UnitPriceRaw = x.UnitPrice,
                        PurchaseDateRaw = x.PurchaseDate,
                        NoteRaw = x.Note
                    })
                    .ToList();

                dgInventory.ItemsSource = items;

                decimal totalValue = db.InventoryItems
                    .AsEnumerable()
                    .Sum(x => x.Quantity * x.UnitPrice);

                txtTotalInventoryValue.Text = totalValue.ToString("N0") + " ₺";

                var lowStockItems = db.InventoryItems
                    .AsEnumerable()
                    .Where(x => x.Quantity <= LowStockThreshold)
                    .ToList();

                txtLowStockCount.Text = lowStockItems.Count.ToString();

                if (lowStockItems.Any())
                {
                    txtLowStockWarning.Text = "Düşük stoklu ürünler: " +
                                              string.Join(", ", lowStockItems.Select(x => $"{x.ItemName} ({x.Quantity})"));
                }
                else
                {
                    txtLowStockWarning.Text = "Düşük stoklu ürün yok.";
                }
            }
        }

        private void ClearForm()
        {
            selectedInventoryId = null;
            txtItemName.Clear();
            txtCategory.Clear();
            txtQuantity.Clear();
            txtPrice.Clear();
            txtNote.Clear();
            dpDate.SelectedDate = DateTime.Today;
            dgInventory.SelectedItem = null;
        }
    }
}