using System.Windows;
using VRFinanceApp.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace VRFinanceApp
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