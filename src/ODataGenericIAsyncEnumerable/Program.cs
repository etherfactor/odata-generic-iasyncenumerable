using Asp.Versioning;
using Microsoft.AspNetCore.OData;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//Replace the log with Serilog
builder.Logging.ClearProviders();
builder.Services.AddSerilog((services, logger) =>
    logger.ReadFrom.Configuration(services.GetRequiredService<IConfiguration>()));

//Add services to the container

builder.Services
    .AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .AddOData();

builder.Services
    .AddApiVersioning(opt =>
    {
        opt.ReportApiVersions = true;
        opt.DefaultApiVersion = new ApiVersion(1.0);
        opt.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddMvc()
    .AddApiExplorer(opt =>
    {
        opt.GroupNameFormat = "'v'VVV";
        opt.SubstituteApiVersionInUrl = true;
    })
    .AddOData(opt =>
    {
        opt.AddRouteComponents("api/v{version:apiVersion}");
    })
    .AddODataApiExplorer(opt =>
    {
        opt.GroupNameFormat = "'v'VVV";
        opt.SubstituteApiVersionInUrl = true;
    });

//Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseODataRouteDebug();

app.UseAuthorization();

app.MapControllers();

app.Run();
