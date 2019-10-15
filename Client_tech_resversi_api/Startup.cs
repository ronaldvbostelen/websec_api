using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Client_tech_resversi_api.Assets;
using Client_tech_resversi_api.Assets.Interfaces;
using Client_tech_resversi_api.Models;
using Client_tech_resversi_api.Models.Non_DB_models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Client_tech_resversi_api
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
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddFile(AppContext.BaseDirectory + "app.log", append: true);
            });

            services.AddDbContext<ReversiContext>(
                opt =>
                    opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                        providerOptions => providerOptions.EnableRetryOnFailure()));

            services.AddCors(opt => opt.AddPolicy("AllowMyOrigin", builder =>
            {
                builder
                    .WithOrigins("https://drysolidkiss.nl", "http://localhost:5050")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            }));
            
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(Configuration.GetSection("AuthCookiePath").Value))
                .SetApplicationName("Reversi");

            services.AddAuthentication("Identity.Application")
                .AddCookie("Identity.Application", opts =>
                {
                    opts.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                    opts.SlidingExpiration = true;
                    opts.AccessDeniedPath = "/api/Forbidden";
                    opts.LoginPath = "/api/Forbidden";
                    opts.Cookie.Name = "whoami";
                    opts.EventsType = typeof(UserChangedAuthenticationEvents);
                });
            services.AddScoped<UserChangedAuthenticationEvents>();
            services.AddScoped<IPasswordManager, PasswordManager>();
            services.AddScoped<IStringValidator, StringValidator>();
            services.AddScoped<IHashGenerator, HashGenerator>();
            services.AddScoped<IDataSeeder, DataSeeder>();
            services.AddScoped<IMailTransferAgent, MailClient>();
            services.AddScoped<IEmailer, Emailer>();
            
            services.Configure<MvcOptions>(opt => { opt.Filters.Add(new CorsAuthorizationFilterFactory("AllowMyOrigin")); });
            services.AddMvc()
                .AddSessionStateTempDataProvider()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddHttpContextAccessor();

            services.Configure<SmtpUser>(Configuration.GetSection("smtp"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseHsts();
            app.UseAuthentication();
            app.UseCors("AllowMyOrigin");
            app.UseMvc();
        }
    }
}
