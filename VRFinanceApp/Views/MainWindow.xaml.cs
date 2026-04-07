using VRFinanceApp.Data;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace VRFinanceApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadDashboard();
            LoadRecentRecords();
        }

        private void LoadDashboard()
        {
            using (AppDbContext db = new AppDbContext())
            {
                decimal totalIncome = db.Incomes
                    .AsEnumerable()
                    .Sum(x => x.Amount);

                decimal totalExpense = db.Expenses
                    .AsEnumerable()
                    .Sum(x => x.Amount);

                decimal net = totalIncome - totalExpense;

                txtTotalIncome.Text = totalIncome.ToString("N0") + " ₺";
                txtTotalExpense.Text = totalExpense.ToString("N0") + " ₺";
                txtNet.Text = net.ToString("N0") + " ₺";

                if (net < 0)
                {
                    borderNetCard.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444"));
                    txtNetIcon.Text = "↓";
                }
                else
                {
                    borderNetCard.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6"));
                    txtNetIcon.Text = "↑";
                }
            }
        }

        private void LoadRecentRecords()
        {
            using (AppDbContext db = new AppDbContext())
            {
                var incomes = db.Incomes
                    .AsEnumerable()
                    .Select(x => new
                    {
                        Type = "Gelir",
                        CategoryOrBranch = x.Branch,
                        ClassType = x.ClassType,
                        Amount = x.Amount.ToString("N0") + " ₺",
                        Date = x.Date.ToString("dd.MM.yyyy"),
                        Note = string.IsNullOrWhiteSpace(x.Note) ? "-" : x.Note
                    });

                var expenses = db.Expenses
                    .AsEnumerable()
                    .Select(x => new
                    {
                        Type = "Gider",
                        CategoryOrBranch = x.Category,
                        ClassType = "-",
                        Amount = x.Amount.ToString("N0") + " ₺",
                        Date = x.Date.ToString("dd.MM.yyyy"),
                        Note = string.IsNullOrWhiteSpace(x.Note) ? "-" : x.Note
                    });

                var recentRecords = incomes
                    .Concat(expenses)
                    .OrderByDescending(x => DateTime.ParseExact(x.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture))
                    .Take(10)
                    .ToList();

                dgRecentRecords.ItemsSource = recentRecords;
            }
        }

        private void btnIncome_Click(object sender, RoutedEventArgs e)
        {
            IncomeWindow window = new IncomeWindow();
            window.ShowDialog();
            LoadDashboard();
            LoadRecentRecords();
        }

        private void btnExpense_Click(object sender, RoutedEventArgs e)
        {
            ExpenseWindow window = new ExpenseWindow();
            window.ShowDialog();
            LoadDashboard();
            LoadRecentRecords();
        }

        private void btnInventory_Click(object sender, RoutedEventArgs e)
        {
            InventoryWindow window = new InventoryWindow();
            window.ShowDialog();
        }
        private void btnReports_Click(object sender, RoutedEventArgs e)
        {
            ReportsWindow window = new ReportsWindow();
            window.ShowDialog();
        }
    }
}