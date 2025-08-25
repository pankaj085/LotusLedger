using Microsoft.EntityFrameworkCore;
using ProductInventory.AppDataContext;
using ProductInventory.Interface;
using ProductInventory.Services;
using ProductInventory.MappingProfiles;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext with PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Services
builder.Services.AddScoped<IProductService, ProductService>();

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(ProductMappingProfile).Assembly);

// Add Controllers
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();

// show docstings in swagger
builder.Services.AddSwaggerGen(options =>
{
    // Get the XML comments file path
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    // Include the XML comments in Swagger
    options.IncludeXmlComments(xmlPath);

    // Set custom metadata for the Swagger document
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "LotusLeger",
        Version = "v1.1.0",
        Description = "An API that's both \'Elegant and Foundational\', provides a clear and reliable foundation for all your product management needs. \"Lotus\" represents cleanliness and order, suggesting a well-structured inventory system, while \"Ledger\" represents its core function of meticulously tracking product data.",
    });
});

var app = builder.Build();

// Apply Migrations automatically (optional)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Inventory API V1");
        c.RoutePrefix = string.Empty; // Swagger at root (optional)
    });
}


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
