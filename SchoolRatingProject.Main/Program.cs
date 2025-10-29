using System;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SchoolRating.Data;
using MySqlConnector;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Read connection string
var conn = builder.Configuration.GetConnectionString("DefaultConnection")
           ?? builder.Configuration.GetConnectionString("CinemaDb");

// Helper: append safe dev options when missing
static string EnsureMySqlConnOptions(string connectionString)
{
    if (string.IsNullOrWhiteSpace(connectionString)) return connectionString;
    var lower = connectionString.ToLowerInvariant();
    if (!lower.Contains("allowpublickeyretrieval"))
        connectionString += (connectionString.Trim().EndsWith(";") ? "" : ";") + "AllowPublicKeyRetrieval=True;";
    if (!lower.Contains("sslmode"))
        connectionString += "SslMode=Preferred;";
    return connectionString;
}

// Configure DbContext
if (!string.IsNullOrWhiteSpace(conn))
{
    var explicitServerVersion = new MySqlServerVersion(new Version(8, 0, 33));

    try
    {
        builder.Services.AddDbContext<SchoolRatingDbContext>(opts =>
            opts.UseMySql(conn, explicitServerVersion));
    }
    catch (MySqlException)
    {
        var connWithOptions = EnsureMySqlConnOptions(conn);
        builder.Services.AddDbContext<SchoolRatingDbContext>(opts =>
            opts.UseMySql(connWithOptions, explicitServerVersion));
    }
    catch (Exception)
    {
        builder.Services.AddDbContext<SchoolRatingDbContext>(opts =>
            opts.UseInMemoryDatabase("SchoolRatingFallback"));
    }
}
else
{
    builder.Services.AddDbContext<SchoolRatingDbContext>(opts =>
        opts.UseInMemoryDatabase("SchoolRatingDb"));
}

// ✅ Add controllers and JSON options with cycle support
builder.Services.AddControllers().AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SchoolRating / Cinema API", Version = "v1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// CORS - permissive for development only
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SchoolRating / Cinema API v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }
