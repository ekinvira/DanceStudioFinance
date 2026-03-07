using DanceStudioFinance.Data;
using DanceStudioFinance.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DanceStudioFinance.Views
{
    public partial class IncomeWindow : Window
    {
        public IncomeWindow()
        {
            InitializeComponent();
            dpDate.SelectedDate = DateTime.Today;
        }

        private void cmbBranch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbClassType.Items.Clear();
            cmbClassType.SelectedItem = null;

            if (cmbBranch.SelectedItem == null)
                return;

            string selectedBranch = ((ComboBoxItem)cmbBranch.SelectedItem!).Content.ToString()!;

            switch (selectedBranch)
            {
                case "K-pop":
                case "HipHop":
                    cmbClassType.Items.Add(new ComboBoxItem { Content = "Genç (8-15)" });
                    cmbClassType.Items.Add(new ComboBoxItem { Content = "Yetişkin" });
                    break;

                case "Resim":
                    cmbClassType.Items.Add(new ComboBoxItem { Content = "Çocuk" });
                    cmbClassType.Items.Add(new ComboBoxItem { Content = "Yetişkin" });
                    break;

                case "Salsa":
                case "Tango":
                    cmbClassType.Items.Add(new ComboBoxItem { Content = "Beginner" });
                    cmbClassType.Items.Add(new ComboBoxItem { Content = "Orta" });
                    cmbClassType.Items.Add(new ComboBoxItem { Content = "İleri" });
                    break;

                case "Halk Oyunları":
                case "Oryantal":
                case "Zumba":
                    cmbClassType.Items.Add(new ComboBoxItem { Content = "Yetişkin" });
                    break;

                case "Bale":
                case "Modern Dans":
                case "Jimnastik":
                    cmbClassType.Items.Add(new ComboBoxItem { Content = "Çocuk" });
                    break;
            }

            if (cmbClassType.Items.Count > 0)
            {
                cmbClassType.SelectedIndex = 0;
            }
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

        private void btnSaveIncome_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbBranch.SelectedItem == null ||
                    cmbClassType.SelectedItem == null ||
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

                string selectedBranch = ((ComboBoxItem)cmbBranch.SelectedItem!).Content.ToString()!;
                string selectedClassType = ((ComboBoxItem)cmbClassType.SelectedItem!).Content.ToString()!;

                Income income = new Income
                {
                    Branch = selectedBranch,
                    ClassType = selectedClassType,
                    Amount = amount,
                    Date = dpDate.SelectedDate.Value,
                    Note = txtNote.Text
                };

                using (AppDbContext db = new AppDbContext())
                {
                    db.Incomes.Add(income);
                    db.SaveChanges();
                }

                MessageBox.Show("Gelir başarıyla kaydedildi.");
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