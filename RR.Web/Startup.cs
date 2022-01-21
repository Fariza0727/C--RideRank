using AppWeb.SignalRHubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RR.AdminData;
using RR.Core;
using RR.Data;
using RR.Data.Membership.Data;
using RR.Repo;
using RR.Service;
using RR.Service.BlobStorage;
using RR.Service.CMS;
using RR.Service.Email;
using RR.Service.News;
using RR.Service.User;
using RR.StaticData;
using RR.ThirdParty;
using RR.Web.Helpers;
using RR.Web.SignalRHubs;
using System;

namespace RR.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public AppSettings appSettings { get; set; }

        // This method gets called by the runtime. Use this method 
        // to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user 
                // consent for non-essential cookies is needed 
                // for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            appSettings = appSettingsSection.Get<AppSettings>();

            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.Configure<PayPalSettings>(Configuration.GetSection("PayPalSettings"));

            services.AddDbContext<RankRideContext>(options => options.UseSqlServer(Configuration.GetConnectionString("RankRideContext")));
            services.AddDbContext<RankRideAdminContext>(options => options.UseSqlServer(Configuration.GetConnectionString("RankRideAdminContext")));
            services.AddDbContext<RankRideStaticContext>(options => options.UseSqlServer(Configuration.GetConnectionString("RankRideStaticContext")));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            
            services.Configure<IdentityOptions>(options =>
            {
                // Default Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(1);
                options.Lockout.MaxFailedAccessAttempts = 4;
                options.Lockout.AllowedForNewUsers = true;
            });
            
            services.AddIdentity<IdentityUser, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 7;
                opt.Password.RequireDigit = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireNonAlphanumeric = false;
                opt.User.RequireUniqueEmail = true;
            })
             .AddEntityFrameworkStores<ApplicationDbContext>()
             .AddDefaultTokenProviders();
            services.Configure<DataProtectionTokenProviderOptions>(opt =>
                    opt.TokenLifespan = TimeSpan.FromDays(2));

            services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
            opt =>
            {
                //configure your other properties
                opt.LoginPath = "/Login";
                opt.AccessDeniedPath = "/AccessDenined";
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSignalR().AddHubOptions<ChatHub>(options =>
            {
                options.EnableDetailedErrors = true;
            });
            services
                .AddMvc()
                .AddSessionStateTempDataProvider();

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                // Make the session cookie essential
                options.Cookie.IsEssential = true;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            services.AddControllers().AddNewtonsoftJson();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowMyOrigin",
                builder => builder.WithOrigins(
                    "http://rankridefantasy.com/",
                    "https://localhost:44340/",
                    "http://103.207.168.162:8001/"
                    ));
            });

            services.AddCors(o => o.AddPolicy("SignalROrigin", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            //Services
            services.AddTransient<IBlobStorageService, BlobStorageService>();
            services.AddTransient<IRiderService, RiderService>();
            services.AddTransient<IBullService, BullService>();
            services.AddTransient<IEventService, EventService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<ICMSService, CMSService>();
            services.AddTransient<INewsService, NewsService>();
            services.AddTransient<ICityService, CityService>();
            services.AddTransient<IStateService, StateService>();
            services.AddTransient<ICountryService, CountryService>();
            services.AddTransient<ITeamService, TeamService>();
            services.AddTransient<IContestService, ContestService>();
            services.AddTransient<ISearchResultService, SearchResultService>();
            services.AddTransient<INewsLetterService, NewsLetterService>();
            services.AddTransient<PayPalAPI, PayPalAPI>();
            services.AddTransient<ITransactionService, TransactionService>();
            services.AddTransient<IPrivateContestService, PrivateContestService>();
            services.AddTransient<IContestWinnerService, ContestWinnerService>();
            services.AddTransient<IContestUserWinnerService, ContestUserWinnerService>();
            services.AddTransient<IBannerService, BannerService>();
            services.AddTransient<IFavoriteBullsRidersService, FavoriteBullsRidersService>();
            services.AddTransient<IUserChatsService, UserChatsService>();
            services.AddTransient<ILongTermTeamService, LongTermTeamService>();
            services.AddTransient<IUserRequestsServices, UserRequestsServices>();
            services.AddTransient<IStoreShopifyService, StoreShopifyService>();
            services.AddTransient<IBullRiderPicturesService, BullRiderPicturesService>();
            services.AddTransient<IPointDataService, PointDataService>();
            services.AddTransient<ICalcuttaService, CalcuttaService>();
            services.AddScoped(typeof(Microsoft.ApplicationInsights.TelemetryClient));
            services.AddScoped(typeof(SessionHelperService));
        }

        // This method gets called by the runtime. 
        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
                              IHostingEnvironment env)
        {
            if (env.IsDevelopment() || appSettings.IsDebugMode)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {

                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. 
                // You may want to change this for production scenarios, 
                // see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.Use(async (ctx, next) =>
            {
                await next();

                if (ctx.Response.StatusCode == 404 && !ctx.Response.HasStarted)
                {
                    //Re-execute the request so the user gets the error page
                    string originalPath = ctx.Request.Path.Value;
                    ctx.Items["originalPath"] = originalPath;
                    ctx.Request.Path = "/error/404";
                    await next();
                }

            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseCors("AllowMyOrigin");
            app.UseCors("SignalROrigin");
            app.UseRouting();
            app.UseAuthorization();
            app.UseRewriter(new RewriteOptions()
                .AddRedirectToNonWwwPermanent()
            );
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});
            app.UseEndpoints(endpoints => {
                endpoints.MapHub<ChatHub>("/chathub");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
            //Chathub is disabled, and it fixed the 60 second delay issue on startup
            // app.UseSignalR(routes =>
            // {

            //     routes.MapHub<ChatHub>("/chathub");

            // });

        }
    }
}