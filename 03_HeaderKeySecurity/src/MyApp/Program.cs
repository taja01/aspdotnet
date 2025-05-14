using FluentValidation;
using LotteryApp.Authorization.Dummy;
using LotteryApp.Authorization.Handlers;
using LotteryApp.Authorization.Requirements;
using LotteryApp.RequestDto;
using LotteryApp.Validations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace LotteryApp
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
                // options.DefaultChallengeScheme = "Dummy";
                // Set a default challenge scheme if you intend to use standard challenge behavior later
                // options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
.AddScheme<AuthenticationSchemeOptions, DummyAuthenticationHandler>("Dummy", null);

            //------------------------
            builder.Services.Configure<ApiKeyOptions>(builder.Configuration.GetSection(nameof(ApiKeyOptions)));
            builder.Services.AddAuthorizationBuilder()
                .AddPolicy("ApiKeyPolicy", policy => policy.Requirements.Add(new ApiKeyRequirement()));

            builder.Services.AddScoped<IAuthorizationHandler, ApiKeyHandler>();

            builder.Services.AddScoped<IValidator<LotteryRequest>, RequestLotteryTicketValidator>();
            ////builder.Services.AddValidatorsFromAssemblyContaining<RequestLotteryTicketValidator>();

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
