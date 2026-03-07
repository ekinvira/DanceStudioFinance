using DanceStudioFinance.Models;
using Microsoft.EntityFrameworkCore;

namespace DanceStudioFinance.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Income> Incomes { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=finance.db");
        }
    }
}