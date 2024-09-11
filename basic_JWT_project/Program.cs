
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using japanese_resturant_project.services;
using japanese_resturant_project.services.implement;
using Microsoft.Extensions.FileProviders;

//IConfiguration configuration = new ConfigurationBuilder()
//                            .AddJsonFile("appsettings.json")
//                            .Build();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddAuthentication();
builder.Services.AddControllers();

 builder.Services.AddCors(options =>
 {
     //options.AddPolicy("AllowLocalhost3000", builder => builder.WithOrigins("http://localhost:3003").AllowAnyMethod().AllowAnyHeader());
     options.AddPolicy("AllowLocalhost3001", builder => builder.WithOrigins("http://localhost:3001").AllowAnyMethod().AllowAnyHeader());
 });
//builder.Services.AddScoped<serviceFactory>();
// Register AuthService
builder.Services.AddScoped<AuthService>();

// Register ServiceFactory
builder.Services.AddSingleton<ServiceFactory>();

//builder.Services.AddIdentityAoiEndpoint<IdentityUser>().AddEntityFrameworkStores<DataContext>();
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.MapIdentityApi<IdentityUser>();
app.UseHttpsRedirection();
//app.UseCors(options => options.WithOrigins(http://localhost:3000).AllowAnyMethod().AllowAnyHeader());
app.UseCors("AllowLocalhost3001");

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads")),
    RequestPath = "/Image"
});
app.UseAuthorization();

app.MapControllers();

app.Run();
