using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using FluentValidation.AspNetCore;
using Ganss.Xss;
using MeetingScheduler.Domain.Common.Models;
using MeetingScheduler.Domain.Enums;
using MeetingScheduler.Domain.Interfaces;
using MeetingScheduler.Domain.Repositories;
using MeetingScheduler.Identity;
using MeetingScheduler.Infrastructure;
using MeetingScheduler.Infrastructure.Common.Mappings;
using MeetingScheduler.Infrastructure.Interfaces;
using MeetingScheduler.Infrastructure.Persistance;
using MeetingScheduler.Infrastructure.Services;
using MeetingScheduler.Infrastructure.Services.Employees;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;

namespace MeetingScheduler.UI
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;

        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddScoped<IHtmlSanitizer>(_ => new HtmlSanitizer());

            services.AddInfrastructure(Configuration, WebHostEnvironment);
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddDbContext<ApplicationDbContext>();
            services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)), Assembly.GetAssembly(typeof(MeetingScheduler.UI.Common.Mappings.MappingProfile)));
            services.AddRazorPages();
			services.AddMemoryCache();
			// Customise default API behaviour
			services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.Configure<IISOptions>(options =>
            {
                options.AutomaticAuthentication = true;
            });

            services.AddControllersWithViews()
            .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>())
            .AddRazorRuntimeCompilation();

            string connectionString = Configuration.GetConnectionString("DefaultConnection");

            //1.1 Authorization 
            services.AddSingleton<IUserProvider, AdUserProvider>();
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.Configure<ADSettings>(Configuration.GetSection("ADSettings"));
			services.Configure<ZoomSettings>(Configuration.GetSection("ZoomSettings"));
            services.Configure<WebexSettings>(Configuration.GetSection("WebexSettings"));

            // Authorization settings.  
            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.Cookie.MaxAge = options.ExpireTimeSpan; // optional
                options.SlidingExpiration = false;
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.Cookie.Name = "MeetingSchedulerCookie";
                options.LoginPath = "/Account/Index";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanPurge", policy => policy.RequireRole("Admin"));
                options.AddPolicy("FullAccess", policy => policy.RequireRole("Admin"));
                options.AddPolicy("CeoAccess", policy => policy.RequireRole("Ceo", "CeoAssistant", "Admin"));
                options.AddPolicy("CeoAssistantAccess", policy => policy.RequireRole("CeoAssistant", "Admin"));
                options.DefaultPolicy = new AuthorizationPolicyBuilder().RequireClaim("Authenticated").Build();
            });


            services.AddHttpContextAccessor();
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireClaim("Authenticated")
                .Build();
            });

            services.AddMvc()
            .AddRazorPagesOptions(options =>
            {
                options.Conventions.AddPageRoute("/Home/Index", "");
                options.Conventions.AllowAnonymousToPage("/Account/Index");

            });

            /////////////// Localization /////////////////
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddScoped<IStringLocalizer, StringLocalizer<SharedResource>>();

            services.Configure<RequestLocalizationOptions>(opt =>
            {
                var supportedCultures = new List<CultureInfo> {
                    new CultureInfo("en")   
                };

                var ar = new CultureInfo("ar");
                ar.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
                ar.DateTimeFormat.DateSeparator = "/";
                ar.DateTimeFormat.Calendar = new GregorianCalendar();
                supportedCultures.Add(ar);


                opt.DefaultRequestCulture = new RequestCulture("ar");
                opt.SupportedCultures = supportedCultures;
                opt.SupportedUICultures = supportedCultures;
            });

            services.AddMvc()
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization(
            );

            services.AddMvc().AddNToastNotifyNoty(new NToastNotify.NotyOptions
            {
                ProgressBar = true,
                Timeout = 2000
            });
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
				//app.UseExceptionHandler("/Home/Error");
				app.UseDeveloperExceptionPage();

				//app.UseExceptionHandler("/Error/Error");
                //app.UseStatusCodePagesWithRedirects("/Error/Error{0}");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();

            ////1.1 Authorization 
            app.UseAuthorization();

            app.UseAdMiddleware();

            //NOTE this line must be above .UseMvc() line.
            //app.UseNotyf();
            app.UseNToastNotify();

            app.UseRequestLocalization(app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
