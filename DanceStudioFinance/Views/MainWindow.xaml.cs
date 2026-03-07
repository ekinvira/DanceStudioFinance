using System.Windows;

namespace DanceStudioFinance.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void btnIncome_Click(object sender, RoutedEventArgs e)
        {
            IncomeWindow incomeWindow = new IncomeWindow();
            incomeWindow.ShowDialog();
        }

        private void btnExpense_Click(object sender, RoutedEventArgs e)
        {
            ExpenseWindow expenseWindow = new ExpenseWindow();
            expenseWindow.ShowDialog();
        }

       
    }
}