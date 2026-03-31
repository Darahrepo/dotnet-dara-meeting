using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MeetingScheduler.Domain.Interfaces;
using MeetingScheduler.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using MeetingScheduler.Domain.Common.Interfaces;
using MeetingScheduler.Infrastructure.Interfaces;
using EmployeeScheduler.Infrastructure.Interfaces;
using MeetingScheduler.Infrastructure.Persistance;
using MeetingScheduler.Infrastructure.Services.Items;
using MeetingScheduler.Infrastructure.Common.Services;
using MeetingScheduler.Infrastructure.Services.Meetings;
using EmployeeScheduler.Infrastructure.Services.Employees;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MeetingScheduler.Infrastructure.Services.SampleUsers;
using MeetingScheduler.Infrastructure.Services.MeetingRooms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using MeetingScheduler.Identity;
using MeetingScheduler.Infrastructure.Services.Webinars;
using MeetingScheduler.Infrastructure.Services.Languages;
using Microsoft.AspNetCore.Authorization;

namespace MeetingScheduler.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("MeetingScheduler"));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(
                        configuration.GetConnectionString("DefaultConnection"),
                        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

                    if (webHostEnvironment.IsDevelopment())
                        options.EnableSensitiveDataLogging();
                });
            }

            services.AddSingleton<IUserProvider, AdUserProvider>();
            services.AddTransient<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());


            //******** Services ********//
            services.AddTransient<ISampleUserRepository, SampleUserRepository>();
            services.AddTransient<ISampleUserService, SampleUserService>();

            services.AddTransient<IEmployeeRepository, EmployeeRepository>();
            services.AddTransient<IEmployeeService, EmployeeService>();

            services.AddTransient<IMeetingRoomRepository, MeetingRoomRepository>();
            services.AddTransient<IMeetingRoomService, MeetingRoomServices>();

            services.AddTransient<IItemRepository, ItemRepository>();
            services.AddTransient<IItemService, ItemServices>();

            services.AddTransient<IMeetingRepository, MeetingRepository>();
            services.AddTransient<IMeetingService, MeetingService>();


            services.AddTransient<IWebinarRepository, WebinarRepository>();
            services.AddTransient<IWebinarService, WebinarService>();

            services.AddTransient<ILanguageService, LanguageService>();
            services.AddTransient<ILanguageRepository, LanguageRepository>();


            services.AddTransient<ICalendarService, CalendarService>();

            services.AddTransient<IZoomService, ZoomService>();
            services.AddTransient<IWebexService, WebexService>();

            services.AddTransient<IMeetingValidationService, MeetingValidationService>();

            services.AddTransient<IMeetingAttendeesRepository, MeetingAttendeesRepository>();
            services.AddTransient<IEmailService, EmailServices>();
            services.AddTransient<IEmailBuilderService, EmailBuilderService>();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IUserClaimsService, UserClaimsService>();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddTransient<IDateTimeService, DateTimeServices>();


            services.AddScoped<IFileServices, FileServices>(
            file =>
            {
                return new FileServices(webHostEnvironment.WebRootPath, file.GetRequiredService<IHttpContextAccessor>());
            });

            return services;
        }
    }
}
