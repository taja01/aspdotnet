
using Microsoft.AspNetCore.Authentication;
using MyApp.Authorization.Dummy;
using MyApp.Authorization.Handlers;
using MyApp.Authorization.Requirements;

namespace MyApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddSwaggerGen();

            //--------------
            // Configure the Dummy authentication scheme
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Dummy";
                options.DefaultAuthenticateScheme = "Dummy";
                options.DefaultChallengeScheme = "Dummy";
            })
            .AddScheme<AuthenticationSchemeOptions, DummyAuthenticationHandler>("Dummy", null);

            builder.Services.Configure<ApiKeyOptions>(builder.Configuration.GetSection("ApiKey"));
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiKeyPolicy", policy =>
                    policy.Requirements.Add(new ApiKeyRequirement()));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }


            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapControllers();

            app.Run();
        }
    }
}
