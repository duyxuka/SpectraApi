using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Spectra.Models;
using Spectra.Services;

namespace Spectra
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
            
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddMemoryCache();
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            // xác thực người dùng
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option =>
                {
                    option.Events.OnRedirectToLogin = (context) =>
                    {
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    };

                });

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Spectra", Version = "1.0" });
                c.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Path,
                    Description = "JWT Authorization header using the bearer scheme",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference= new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "bearerAuth",
                            }
                        },
                        new string[]{}
                    }
                });
            });

            services.AddCors(c => c.AddPolicy("AddCors", builder => builder.WithOrigins("https://spectrababy.com.vn", "https://spectra.vn", "https://admin.spectrababy.com.vn", "https://spectrababy.vn", "https://spectra.com.vn", "https://adicon.vn", "http://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials()));
            services.AddCors(c => c.AddPolicy("AddCorsIPN", builder => builder.SetIsOriginAllowed(origin =>
            {
                 //Convert domain to IP 
                 //be careful, domains can have multiple IPs
                var host = new Uri(origin).Host;
                var ipAddresses = Dns.GetHostAddresses(host);

                 //List of allowed IPs
                var allowedIPs = new List<IPAddress>
            {
                IPAddress.Parse("113.160.92.202"),  //Replace with your allowed IPs
                IPAddress.Parse("113.52.45.78"),   //Another IP
                IPAddress.Parse("116.97.245.130"),
                IPAddress.Parse("42.118.107.252"),
                IPAddress.Parse("113.20.97.250"),
                IPAddress.Parse("203.171.19.146"),
                IPAddress.Parse("103.220.87.4"),
                IPAddress.Parse("103.220.86.4")
            };

                return ipAddresses.Any(ip => allowedIPs.Contains(ip));
            }).WithOrigins("https://spectrababy.com.vn","https://spectra.vn", "https://admin.spectrababy.com.vn", "https://spectrababy.vn", "https://spectra.com.vn", "https://adicon.vn").AllowAnyHeader().AllowAnyMethod().AllowCredentials()));
            services.AddScoped<FileServices>();
            services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });
            services.AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "http://localhost:50925/",
                    ValidAudience = "http://localhost:50925/",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this is my custom Secret key for authentication"))
                };
            });

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
            services.AddResponseCaching();
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes =
                    ResponseCompressionDefaults.MimeTypes.Concat(
                        new[] { "image/svg+xml" });
                options.EnableForHttps = true;

            });
            services.AddScoped<IVnPayService, VnPayService>();
            services.AddScoped<IServiceManagercs, ServiceManager>();
            services.AddScoped<IServiceVoucher, ServiceVoucher>();
            services.AddScoped<IServiceItem, ServiceItem>();
            services.AddDbContext<AppDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DbConnect")));
            services.AddHangfire((sp, config) =>
            {
                var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("DbConnect");
                config.UseSqlServerStorage(connectionString);
            });
            services.AddHangfireServer();
            services.AddHangfireServer(options =>
            {
                options.WorkerCount = 100; // Số lượng worker tùy theo nhu cầu của bạn
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            
            app.UseResponseCompression();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseCors("AddCors");
            app.UseCors("AddCorsIPN");
            app.UseDefaultFiles();

            app.UseAuthentication();
            app.UseIpRateLimiting();
            //app.UseAuthorization(); 
            app.UseHangfireDashboard("/job-spectra", new DashboardOptions
            {
                DashboardTitle = "Hangfire Job Spectra",
                DarkModeEnabled = false,
                DisplayStorageConnectionString = false,
                Authorization = new[]
            {
                new HangfireCustomBasicAuthenticationFilter{
                    User = "admin@spectra",
                    Pass = "Spectra@2022"
                }
            }

            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "1.0");
            });
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
