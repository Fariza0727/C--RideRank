using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using RR.AdminData;
using RR.Core;
using RR.Data;
using RR.Data.Membership.Data;
using RR.Repo;
using RR.Service;
using RR.Service.CMS;
using RR.Service.Email;
using RR.Service.News;
using RR.Service.User;
using RR.StaticData;
using RR.WebApi.Helper;
using RR.WebApi.Models;

namespace RR.WebApi
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

            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            appSettings = appSettingsSection.Get<AppSettings>();

            services.AddDbContext<RankRideContext>(options => options.UseSqlServer(Configuration.GetConnectionString("RankRideContext")));
            services.AddDbContext<RankRideAdminContext>(options => options.UseSqlServer(Configuration.GetConnectionString("RankRideAdminContext")));
            services.AddDbContext<RankRideStaticContext>(options => options.UseSqlServer(Configuration.GetConnectionString("RankRideStaticContext")));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            //services.AddDefaultIdentity<IdentityUser>()
            //    .AddRoles<IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

            services.AddTransient<ITransactionService, TransactionService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ICountryService, CountryService>();
            services.AddTransient<IStateService, StateService>();
            services.AddTransient<ICityService, CityService>();

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IPasswordRequestService, PasswordRequestService>();

            services.AddTransient<ICMSService, CMSService>();
            services.AddTransient<INewsService, NewsService>();
            services.AddTransient<INewsLetterService, NewsLetterService>();
            services.AddTransient<IRiderService, RiderService>();
            services.AddTransient<IBullService, BullService>();
            services.AddTransient<IEventService, EventService>();
            services.AddTransient<ITeamService, TeamService>();
            services.AddTransient<IContestService, ContestService>();
            services.AddTransient<ISearchResultService, SearchResultService>();
            services.AddTransient<IPrivateContestService, PrivateContestService>();
            services.AddTransient<IContestWinnerService, ContestWinnerService>();
            services.AddTransient<IContestUserWinnerService, ContestUserWinnerService>();
            services.AddTransient<IFavoriteBullsRidersService, FavoriteBullsRidersService>();
            services.AddTransient<ILongTermTeamService, LongTermTeamService>();
            services.AddTransient<IStoreShopifyService, StoreShopifyService>();
            services.AddTransient<IBullRiderPicturesService, BullRiderPicturesService>();
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.DateFormatString = appSettings.DateTimeFormate; // month must be capital. otherwise it gives minutes.
            }).ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problems = new CustomBadRequest(context);
                    return new ObjectResult(problems);
                };
            }); ;
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "RankRide API",
                    //Description = "A simple example ASP.NET Core Web API",
                    //TermsOfService = new Uri("https://google.com/terms"),
                    //Contact = new OpenApiContact
                    //{
                    //    Name = "Shayne Boyer",
                    //    Email = string.Empty,
                    //    Url = new Uri("https://freelancer.com/spboyer"),
                    //},
                    //License = new OpenApiLicense
                    //{
                    //    Name = "Use under LICX",
                    //    Url = new Uri("https://google.com/license"),
                    //}
                });
            });

            //services.AddMvc()
            //    //.SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            //    .AddJsonOptions(options =>
            //       {
            //           //Set date configurations
            //           //options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            //           options.SerializerSettings.DateFormatString = appSettings.DateTimeFormate; // month must be capital. otherwise it gives minutes.

            //       }).ConfigureApiBehaviorOptions(options =>
            //       {
            //           options.InvalidModelStateResponseFactory = context =>
            //           {
            //               var problems = new CustomBadRequest(context);
            //               return new ObjectResult(problems);
            //           };
            //       }); ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseAuthentication();
            app.UseSwaggerAuthorized();
            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                //c.RoutePrefix = string.Empty;
                c.RoutePrefix = "swagger";
            });
        }
    }
}
