using SnakesAndLadders.Application.Startup;

namespace SnakesAndLadders.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            _ = builder.Services.AddControllers();
            _ = builder.Services.Configure<RouteOptions>(opt => opt.LowercaseUrls = true);
            builder.Services.AddSnakesAndLadders();

            // Configure Swagger/OpenAPI.
            _ = builder.Services.AddEndpointsApiExplorer();
            _ = builder.Services.AddSwaggerGen();

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                _ = app.UseDeveloperExceptionPage();

                _ = app.UseSwagger();
                _ = app.UseSwaggerUI();
            }

            _ = app.UseHttpsRedirection();

            _ = app.MapControllers();

            app.Run();
        }
    }
}