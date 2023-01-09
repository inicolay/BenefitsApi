using Amazon.DynamoDBv2;
using Benefits.Api.Services;
using Benefits.Api.Settings;
using Dynamo.Common;

namespace Benefits.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;

        var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("api.appsettings.json", optional: false)
                .AddEnvironmentVariables();

        Configuration = builder.Build();
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        var appSettings = new AppSettings();
        Configuration.Bind("AppSettings", appSettings);
        services.AddControllers();
        services.AddSwaggerGen();

        var dynamoClient = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USWest2);
        services.AddSingleton<AmazonDynamoDBClient>(dynamoClient);
        services.AddSingleton<BenefitsSettings>(appSettings.BenefitsSettings);
        services.AddSingleton<IDynamoRepository, DynamoRepository>();
        services.AddTransient<IBenefitsService, BenefitsService>();
        services.AddTransient<IEmployeeService, EmployeeService>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("v1/swagger.json", "Benefits API V1");
        });

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
            });
        });
    }
}