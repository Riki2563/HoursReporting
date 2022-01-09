using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Text;
using Malam.Digital.Base.Entities;
using log4net;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Reflection;
using AutoMapper;
using log4net.Config;
using Malam.Digital.Base.Entities.Model;
using Malam.Digital.Base.Entities.Dto;
using Malam.Digital.Base.Bll.Services;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Malam.Digital.BaseWebApi
{
    public class Startup
    {
        private ILog _log;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConfigLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                                  \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header,

                            },
                            new List<string>()
                        }
                    });
            });
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddCors();
            services.AddMvc(config =>
            {
                config.Filters.Add<LogAttribute>();
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddDbContext<BaseExampleContext>(options =>
         options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));//, opts => opts.UseNetTopologySuite()));
            _log  = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            services.AddSingleton(_log);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(x =>
           {
               //x.Events = new JwtBearerEvents
               //{
               //    //OnTokenValidated = context =>
               //    //{
               //    //     // var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
               //    //     var userId = int.Parse(context.Principal.Identity.Name);
               //    //     //UserCtrl ctrl = new UserCtrl(_appSettings.DBConnection, _log);
               //    //     //var user= ctrl.GetById(userId);
               //    //     //if (user == null)
               //    //     //{
               //    //     //    // return unauthorized if user no longer exists
               //    //     //    context.Fail("Unauthorized");
               //    //     //}
               //    //     return System.Threading.Tasks.Task.CompletedTask;
               //    //}
               //};
               x.RequireHttpsMetadata = false;
               x.SaveToken = true;
               x.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(key),
                   ValidateIssuer = false,
                   ValidateAudience = false
               };
           });
            // Automapper
            try
            {
                services.AddAutoMapper(typeof(WebApiProfile));
            }
            catch (Exception ex)
            {
                _log.Error("mapper", ex);
            }
            //services
            services.AddScoped<UserService>();
            services.AddScoped<AuthService>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("../swagger/v1/swagger.json", "MyAPI V1");
            });
        }
        private static void ConfigLogger()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }
    }
    public class WebApiProfile : Profile
    {
        public WebApiProfile()
        {
            try
            {
                CreateMap<Base.Entities.Model.User, UserDto>();
                //.ForMember(dest => dest.FamilyName, opt => opt.MapFrom(src => src.Family != null ? src.Family.FamilyName : string.Empty))
                //.ForMember(dest => dest.UserStatusId, opt => opt.MapFrom(src => src.User.UserStatusId));
                CreateMap<UserDto, Base.Entities.Model.User>();
                //CreateMap<Family, FamiliesDto>().ForMember(dest => dest.CountUserApp, opt => opt.MapFrom(src => src.UserApp.Where(u => u.User.UserStatusId == (int)UserStatusEnum.Active).Count()))
                //    .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.UserApp.FirstOrDefault(f => f.Address != null && (f.UserAppTypeId == (int)UserType.Father || f.UserAppTypeId == (int)UserType.Mother)).Address));

            
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class LogAttribute : IActionFilter //where T : class, IEntity
    {
        private readonly BaseExampleContext _context;

        public LogAttribute(BaseExampleContext context)
        {
            _context = context;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {            
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                int id = int.Parse(context.HttpContext.User.Identity.Name);
                var user = _context.User.Find(id).LastActivity = DateTime.Now;
                _context.SaveChanges();
            }          
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
