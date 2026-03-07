using System.Windows;
using DanceStudioFinance.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace DanceStudioFinance
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                using (AppDbContext db = new AppDbContext())
                {
                    db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Startup Hatası");
            }
        }
    }
}