
namespace MyAutodeskAPS
{
    using MyAutodeskAPS.Models;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var clientId = builder.Configuration["APS_CLIENT_ID"];
            var clientSecret = builder.Configuration["APS_CLIENT_SECRET"];
            var bucket = builder.Configuration["APS_BUCKET"];

            builder.Services.AddSingleton<Aps>(new Aps(clientId!, clientSecret!,bucket!));
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentException("Missing required environment variables APS_CLIENT_ID or APS_CLIENT_SECRET");
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseHttpsRedirection();
            app.MapControllers();

            app.Run();
        }
    }
}