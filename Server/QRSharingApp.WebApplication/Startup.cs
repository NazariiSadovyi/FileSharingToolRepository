using QRSharingApp.Common.Services;
using QRSharingApp.Common.Services.Interfaces;
using QRSharingApp.DataAccess;
using QRSharingApp.DataAccess.Repositories;
using QRSharingApp.DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SignalR;
using QRSharingApp.WebApplication.Services;

namespace QRSharingApp.WebApplication
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
            //Note: disable thread safe db repository
            BaseRepository.IsThreadSafeAnabled = false;

            services.AddScoped<ApplicationDBContext>();
            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddScoped<IHotFolderRepository, HotFolderRepository>();
            services.AddScoped<ILocalFileRepository, LocalFileRepository>();
            services.AddScoped<IDownloadHistoryRepository, DownloadHistoryRepository>();
            services.AddScoped<ISharedSettingService, SharedSettingService>();

            services.AddScoped<IWebServerService, WebServerService>();
            services.AddScoped<IFileThumbnailService, FileThumbnailService>();
            services.AddScoped<IQRCodeGeneratorService, QRCodeGeneratorService>();

            services.AddScoped<IFileHubService, FileHubService>();
            services.AddSignalR(option =>
            {
                option.MaximumReceiveMessageSize = 102400000;
            });

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
                FileProvider = new PhysicalFileProvider(@"C:\")
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=File}/{action=Index}/{id?}");

                endpoints.MapHub<FileHub>("/fileHub");
            });
        }
    }
}
