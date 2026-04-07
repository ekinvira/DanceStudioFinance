using System;

namespace VRFinanceApp.Models
{
    public class Expense
    {
        public int Id { get; set; }

        public DateTime Date { get; set; } = DateTime.Today;

        // Kira, Elektrik, Maaş vs.
        public string Category { get; set; } = "";

        public decimal Amount { get; set; }

        public string? Note { get; set; }
    }
}