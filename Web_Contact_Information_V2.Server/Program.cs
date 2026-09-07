using Web_Contact_Information_V2.Server.DataAccess;

var builder = WebApplication.CreateBuilder(args); //Creates application builder and loads configuration services

// Add services to the container.

builder.Services.AddControllers(); //Register controllers 
builder.Services.AddScoped<ContactDA>(); //Create a ContactDA instance and provide it to classes that request it.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure CORS to allow the React frontend hosted on Azure
// to make HTTP requests to this API.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("https://lemon-river-02550df10.3.azurestaticapps.net") 
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();


app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowReact");

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
