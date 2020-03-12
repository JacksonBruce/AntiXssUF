using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AntiXssUF.TestSite.Binders;
using Ufangx.Xss;
using System.IO;

namespace AntiXssUF.TestSite
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IHostEnvironment HostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddXssFilter(opt=>opt.DefaultSchemeName= "DefaultPolicy")
                .AddScheme<AntisamyPolicy>("antisamy", () => File.ReadAllTextAsync(Path.Combine(HostEnvironment.ContentRootPath, "resources/antisamy.xml")))
                .AddScheme<AntisamyPolicy>("anythinggoes", () => File.ReadAllTextAsync(Path.Combine(HostEnvironment.ContentRootPath, "resources/antisamy-anythinggoes.xml")))
                .AddScheme<AntisamyPolicy>("ebay", () => File.ReadAllTextAsync(Path.Combine(HostEnvironment.ContentRootPath, "resources/antisamy-ebay.xml")))
                .AddScheme<AntisamyPolicy>("myspace", () => File.ReadAllTextAsync(Path.Combine(HostEnvironment.ContentRootPath, "resources/antisamy-myspace.xml")))
                .AddScheme<AntisamyPolicy>("slashdot", () => File.ReadAllTextAsync(Path.Combine(HostEnvironment.ContentRootPath, "resources/antisamy-slashdot.xml")))
                .AddScheme<AntisamyPolicy>("test", () => File.ReadAllTextAsync(Path.Combine(HostEnvironment.ContentRootPath, "resources/antisamy-test.xml")))
                .AddScheme<JsonFilterPolicy>("DefaultPolicy", () => File.ReadAllTextAsync(Path.Combine(HostEnvironment.ContentRootPath, "resources/DefaultPolicy.json")));
            ;
            services.AddControllers(options =>
            {
                options.ModelBinderProviders.Insert(0, new RichTextBinderProvider());
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

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
