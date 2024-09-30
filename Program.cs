
using st10158209.Services;

namespace st10158209.Services
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Register HttpClient in the DI container
            builder.Services.AddHttpClient();

            // Register your custom services
            builder.Services.AddSingleton<BlobService>();
            builder.Services.AddSingleton<TableService>();
            builder.Services.AddSingleton<QueueService>(); // If IQueueService is implemented by QueueService
            builder.Services.AddSingleton<FileService>();

            // If QueueService is implementing an interface, use this:
            // builder.Services.AddSingleton<IQueueService, QueueService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}