using VRFinanceApp.Data;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace VRFinanceApp.Views
{
    public partial class ReportsWindow : Window
    {
        public ReportsWindow()
        {
            InitializeComponent();
            LoadMonths();
            LoadYears();
            LoadInitialReport();
        }

        private void LoadMonths()
        {
            cmbMonth.Items.Clear();

            for (int i = 1; i <= 12; i++)
            {
                cmbMonth.Items.Add(new MonthItem
                {
                    Value = i,
                    Name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i)
                });
            }

            cmbMonth.DisplayMemberPath = "Name";
            cmbMonth.SelectedValuePath = "Value";
            cmbMonth.SelectedValue = DateTime.Now.Month;
        }

        private void LoadYears()
        {
            cmbYear.Items.Clear();

            for (int i = 2020; i <= DateTime.Now.Year + 2; i++)
            {
                cmbYear.Items.Add(i);
            }

            cmbYear.SelectedItem = DateTime.Now.Year;
        }

        private void LoadInitialReport()
        {
            if (cmbMonth.SelectedValue != null && cmbYear.SelectedItem != null)
            {
                int month = (int)cmbMonth.SelectedValue;
                int year = (int)cmbYear.SelectedItem;
                LoadReport(month, year);
            }
        }

        private void btnLoadReport_Click(object sender, RoutedEventArgs e)
        {
            if (cmbMonth.SelectedValue == null || cmbYear.SelectedItem == null)
            {
                MessageBox.Show("Lütfen ay ve yıl seçiniz.");
                return;
            }

            int month = (int)cmbMonth.SelectedValue;
            int year = (int)cmbYear.SelectedItem;

            LoadReport(month, year);
        }

        private void LoadReport(int month, int year)
        {
            using (AppDbContext db = new AppDbContext())
            {
                var incomeSource = db.Incomes
                    .AsEnumerable()
                    .Where(x => x.Date.Month == month && x.Date.Year == year)
                    .ToList();

                var expenseSource = db.Expenses
                    .AsEnumerable()
                    .Where(x => x.Date.Month == month && x.Date.Year == year)
                    .ToList();

                var incomeData = incomeSource
                    .GroupBy(x => x.Branch)
                    .Select(g => new
                    {
                        Branch = g.Key,
                        TotalAmount = g.Sum(x => x.Amount).ToString("N0") + " ₺"
                    })
                    .OrderByDescending(x => ParseCurrency(x.TotalAmount))
                    .ToList();

                var expenseData = expenseSource
                    .GroupBy(x => x.Category)
                    .Select(g => new
                    {
                        Category = g.Key,
                        TotalAmount = g.Sum(x => x.Amount).ToString("N0") + " ₺"
                    })
                    .OrderByDescending(x => ParseCurrency(x.TotalAmount))
                    .ToList();

                dgIncomeReport.ItemsSource = incomeData;
                dgExpenseReport.ItemsSource = expenseData;

                decimal totalIncome = incomeSource.Sum(x => x.Amount);
                decimal totalExpense = expenseSource.Sum(x => x.Amount);
                decimal net = totalIncome - totalExpense;

                txtMonthlyIncome.Text = totalIncome.ToString("N0") + " ₺";
                txtMonthlyExpense.Text = totalExpense.ToString("N0") + " ₺";
                txtMonthlyNet.Text = net.ToString("N0") + " ₺";

                if (net < 0)
                {
                    borderMonthlyNet.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444"));
                }
                else
                {
                    borderMonthlyNet.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6"));
                }
            }
        }

        private decimal ParseCurrency(string value)
        {
            string cleaned = value.Replace("₺", "").Trim().Replace(".", "");
            decimal.TryParse(cleaned, out decimal result);
            return result;
        }

        private class MonthItem
        {
            public int Value { get; set; }
            public string Name { get; set; } = "";
        }
    }
}