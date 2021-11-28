using FST.Common.Services;
using FST.Common.Services.Interfaces;
using FST.DataAccess;
using FST.DataAccess.Repositories;
using FST.DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace FST.WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ApplicationDBContext>();
            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddScoped<ILocalFileRepository, LocalFileRepository>();
            services.AddScoped<IDownloadHistoryRepository, DownloadHistoryRepository>();
            services.AddScoped<ISharedSettingService, SharedSettingService>();

            services.AddScoped<IWebServerService, WebServerService>();
            services.AddScoped<IFileThumbnailService, FileThumbnailService>();
            services.AddScoped<IQRCodeGeneratorService, QRCodeGeneratorService>();

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(@"C:\"),
                RequestPath = "/test"
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=File}/{action=Index}/{id?}");
            });
        }
    }
}
