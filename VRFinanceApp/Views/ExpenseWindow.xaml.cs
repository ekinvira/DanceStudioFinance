using VRFinanceApp.Data;
using VRFinanceApp.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VRFinanceApp.Views
{
    public partial class ExpenseWindow : Window
    {
        public ExpenseWindow()
        {
            InitializeComponent();
            dpDate.SelectedDate = DateTime.Today;
        }

        private void txtAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

        private void btnSaveExpense_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbCategory.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(txtAmount.Text) ||
                    dpDate.SelectedDate == null)
                {
                    MessageBox.Show("Lütfen tüm gerekli alanları doldurun.");
                    return;
                }

                if (!decimal.TryParse(txtAmount.Text, out decimal amount))
                {
                    MessageBox.Show("Tutar geçerli bir sayı olmalıdır.");
                    return;
                }

                string selectedCategory = ((ComboBoxItem)cmbCategory.SelectedItem!).Content.ToString()!;

                Expense expense = new Expense
                {
                    Category = selectedCategory,
                    Amount = amount,
                    Date = dpDate.SelectedDate.Value,
                    Note = txtNote.Text
                };

                using (AppDbContext db = new AppDbContext())
                {
                    db.Expenses.Add(expense);
                    db.SaveChanges();
                }

                MessageBox.Show("Gider başarıyla kaydedildi.");
                this.Close();
            }
            catch (Exception ex)
            {
                string errorMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : ex.Message;

                MessageBox.Show("Hata oluştu: " + errorMessage);
            }
        }
    }
}