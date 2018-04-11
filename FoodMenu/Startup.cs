using FoodMenu.AuthFilter;
using FoodMenu.Jobs;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.Owin;
using Owin;
using System;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(FoodMenu.Startup))]
namespace FoodMenu
{

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration
                .UseSqlServerStorage("Default");
            // to start the notification service
            RecurringJob.AddOrUpdate(() => SendLunchMenu(), Cron.Daily(7, 14));
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuth() }
            });
            app.UseHangfireServer();
        }

        public void SendLunchMenu()
        {
            var date = DateTime.Now;
            if (date.DayOfWeek != DayOfWeek.Sunday || date.DayOfWeek != DayOfWeek.Saturday)
            {
                var job = new SendMenuJob();
                job.SendMenuAsync();
            }
        }
    }

}
