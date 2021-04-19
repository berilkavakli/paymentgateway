using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PaymentGateway.Repositories;
using PaymentGateway.Repositories.Interfaces;
using PaymentGateway.Services;
using PaymentGateway.Services.Interfaces;
using PaymentGateway.Utils;
using PaymentGateway.Validation;
using Microsoft.OpenApi.Models;
using System.Threading;
using PaymentGateway.Attributes;

namespace PaymentGateway
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
            services.AddControllers();
            services.AddMvc().AddFluentValidation();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Payment Gateway API",
                    Version = "v1",
                    Description = string.Empty,
                    Contact = new OpenApiContact
                    {
                        Name = "Beril Kavaklı",
                        Email = string.Empty
                    },
                });
                c.OperationFilter<HeaderParameterOperationFilter>();
            });
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddSingleton<IBankServiceFactory, BankServiceFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment Gateway API V1");

                // To serve SwaggerUI at application's root page, set the RoutePrefix property to an empty string.
                c.RoutePrefix = string.Empty;
            });

            // Wait for db to be ready (Docker compose)
            Thread.Sleep(5000);
            while (1==1)
            {
                try
                {
                    //Creates DB and tables if not exists.
                    var connectionString = Configuration.GetConnectionString("PaymentConnectionString");
                    Console.WriteLine("Connecting to db: " + connectionString);
                    DbInitilization.EnsureDatabase(connectionString);
                    Console.WriteLine("Connected to db.");
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine("DB connection error: " + e);
                    Thread.Sleep(1000);
                }
            }

      
        }
    }
}
