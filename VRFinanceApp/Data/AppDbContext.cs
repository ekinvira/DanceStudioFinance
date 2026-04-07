using VRFinanceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace VRFinanceApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Income> Incomes { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=finance.db");
        }

        public DbSet<InventoryItem> InventoryItems { get; set; }
    }
}