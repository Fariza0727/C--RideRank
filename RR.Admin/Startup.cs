
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RR.Admin.Authorization;
using RR.Admin.Models;
using RR.AdminData;
using RR.AdminService;
using RR.Core;
using RR.Data;
using RR.Data.Membership.Data;
using RR.Dto;
using RR.Repo;
using RR.Service.Email;
using RR.StaticData;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;

namespace RR.Admin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public AppSettings appSettings { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Serilog.Log.Logger = new LoggerConfiguration()
            //// Adding the enricher
            //.Enrich.FromLogContext()
            //.WriteTo.File(new JsonFormatter(","), string.Concat("/Logs/log.json"), rollingInterval: RollingInterval.Day)
            //.CreateLogger();

            services.Configure<CookiePolicyOptions>(options =>
            {

                // This lambda determines whether user 
                // consent for non-essential cookies is needed 
                // for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            appSettings = appSettingsSection.Get<AppSettings>();

            services.AddDbContext<RankRideContext>(options =>
            options.UseSqlServer(Configuration
            .GetConnectionString("RankRideContext")));

            services.AddDbContext<RankRideStaticContext>(options =>
              options.UseSqlServer(Configuration
              .GetConnectionString("RankRideStaticContext")));

            services.AddDbContext<RankRideAdminContext>(options =>
               options.UseSqlServer(Configuration
               .GetConnectionString("RankRideAdminContext")));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            //services.AddDefaultIdentity<IdentityUser>()
            //.AddRoles<IdentityRole>()
            //.AddEntityFrameworkStores<ApplicationDbContext>();
            services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
            opt =>
            {
                //configure your other properties
                opt.LoginPath = "/Login";
                opt.AccessDeniedPath = "/AccessDenied";
            });
            services.AddMvc(x => x.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddControllers().AddNewtonsoftJson();
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

            //Services
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IRiderService, RiderService>();
            services.AddTransient<IBullService, BullService>();
            services.AddTransient<IEventService, EventService>();
            services.AddTransient<ICmsService, CmsService>();
            services.AddTransient<INewsService, NewsService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ICommonService, CommonService>();
            services.AddTransient<ICountryService, CountryService>();
            services.AddTransient<IStateService, StateService>();
            services.AddTransient<ICityService, CityService>();
            services.AddTransient<ISponsorService, SponsorService>();
            services.AddTransient<IAwardTypeService, AwardTypeService>();
            services.AddTransient<IAwardService, AwardService>();
            services.AddTransient<IContestService, ContestService>();
            services.AddTransient<IContestCategoryService, ContestCategoryService>();
            services.AddTransient<IContestWinnerService, ContestWinnerService>();
            services.AddTransient<INewsLetterService, NewsLetterService>();
            services.AddTransient<IUserContestService, UserContestService>();
            services.AddTransient<ITransactionService, TransactionService>();
            services.AddTransient<IBannerService, BannerService>();
            services.AddTransient<IPageDetailService, PageDetailService>();
            services.AddTransient<IUserRequestsServices, UserRequestsServices>();
            services.AddTransient<IPictureManagerService, PictureManagerService>();
            services.AddTransient<IPointTableService, PointTableService>();
            services.AddTransient<IVideoSliderService, VideoSliderService>();
            services.AddScoped<AdminExceptionFilter>();
            //Authorizaiton 

            services.AddAuthorization(options =>
            {
                options.AddPolicy("PagePermission",
                                  policy => policy.Requirements.Add(new PageAuthorizationDto()));
            });

            services.AddScoped<IAuthorizationHandler, Authorizehandler>();
            //  services.AddSingleton<IAuthorizationHandler, Authorizehandler>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment() || appSettings.IsDebugMode)
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
            app.UseAuthentication();
            app.UseCookiePolicy();
            app.UseMiddleware<LogUserNameMiddleware>();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
