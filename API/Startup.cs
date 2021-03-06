using System.Text;
using API.Middleware;
using Application.Activities;
using Application.Interfaces;
using Domain;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Infrastructure.Security;
using Persistence;
using AutoMapper;
using Infrastructure.Photos;
using Application.Photos;
using API.SignalR;
using System.Threading.Tasks;
using Application.Profiles;

namespace API
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

            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseLazyLoadingProxies();
                opt.UseSqlite(Configuration.GetConnectionString("DefaultConnextion"));
            });


            services.AddControllers()
            .AddFluentValidation(cfe =>
            {
                cfe.RegisterValidatorsFromAssemblyContaining<Application.Activities.Create>();
                cfe.RegisterValidatorsFromAssemblyContaining<Application.Activities.Edit>();
            });

            var builder = services.AddIdentityCore<AppUser>();
            var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);
            identityBuilder.AddEntityFrameworkStores<DataContext>();
            identityBuilder.AddSignInManager<SignInManager<AppUser>>();

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("IsActivityHost", policy =>
                {
                    policy.Requirements.Add(new IsHostReqirement());
                });
            });

            services.AddTransient<IAuthorizationHandler, IsHostReqirementHandler>();

            services.AddMediatR(typeof(List.Handler).Assembly);
            services.AddMediatR(typeof(Add.Handler).Assembly);
            services.AddMediatR(typeof(Add).Assembly);

            services.AddAutoMapper(typeof(List.Handler));

            services.AddSignalR();

            services.AddMvc(opt =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
            });
            // services.AddSwaggerGen(c =>{c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });});
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenKey"]));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                };
                opt.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {

                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });
            services.AddScoped<IJwtGenerator, JwtGenerator>();
            services.AddScoped<IUserAccessor, UserAccessor>();
            services.AddScoped<IPhotoAccessor, PhotoAccessor>();
            services.AddScoped<IProfileReader, ProfileReader>();

            services.Configure<CloudinarySettings>(Configuration.GetSection("Coudinary"));


            // var configuration = new MapperConfiguration(cfg => 
            // {
            //     cfg.CreateMap<Activity, FooDto>();
            //     cfg.CreateMap<Bar, BarDto>();
            // });
            // configuration.AssertConfigurationIsValid();
            // var mapper = configuration.CreateMapper();

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ErrorHandlerMiddleWare>();
            if (env.IsDevelopment())
            {
                // app.UseDeveloperExceptionPage();
                // app.UseSwagger();
                // app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            }

            // app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chat");
            });
        }
    }
}
