 using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using Microsoft.EntityFrameworkCore;
using LaundryApi.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using LaundryApi.Interfaces;
using System.Text;
using AutoMapper;
using LaundryApi.Repositories;
using System.Net.Http.Headers;
using LaundryApi.Services;
using LaundryApi.Models;
using Microsoft.AspNetCore.Identity;

namespace LaundryApi
{
    public class Startup
    {
        private readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ApplicationDbContext >(options =>
                        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<ApplicationUser, IdentityRole>()
               .AddEntityFrameworkStores<ApplicationDbContext>();


            //allow my react app
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  build =>
                                  {
                                      build.WithOrigins("http://localhost:3000", "https://04c62d284968.ngrok.io", "https://avone.netlify.app")
                                           .AllowAnyHeader()
                                           .AllowAnyMethod()
                                           .AllowCredentials();
                                  });
            });

            //add the jwt interface to enable injection
            var key = Configuration["LaundryManagerApi:jwtKey"]; //this is the key used during the hashing 

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });

            services.AddHttpClient<IPaymentService, PaymentService>( client=>
                client.DefaultRequestHeaders.Authorization= new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(Encoding.UTF8.GetBytes(Configuration["LaundryManagerApi:monnifyApiKey"])))
            );
            

            services.AddSingleton<IJwtAuthenticationManager>(new JwtAuthenticationManager(key));
            services.AddScoped<IManagerRepository, ManagerRepository>();
            services.AddScoped<IMailService,MailService>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<ILaundryRepository, LaundryRepository>();
            services.AddScoped<ICustomerRepository, CustomerRespository>();
            services.AddScoped<IInvoiceRepository,InvoiceRepository>();
            services.AddTransient<IUnitOfWork,UnitOfWork>();
            services.AddScoped<IRepositoryHelper,RepositoryHelper>();
            services.AddScoped<IServiceRepository,ServiceRepository>();
            services.AddScoped<IDashboardRepo,DashboardRepo>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "LaundryApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LaundryApi v1"));
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            //added by me
            app.UseCors(MyAllowSpecificOrigins);
            //#####################
            app.UseAuthentication(); 

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
