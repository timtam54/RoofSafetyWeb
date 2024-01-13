using Microsoft.EntityFrameworkCore;
using RoofSafety.Data;
using RoofSafety.Options;
using RoofSafety.Services.Abstract;
using RoofSafety.Services.Concrete;
var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();//.WithOrigins("https://localhost",                "https://localhost:7284/");
        });
});
builder.Services.AddDbContext<dbcontext>(options => options.UseSqlServer("Server=tcp:perthcitycranes.database.windows.net,1433;Initial Catalog=RoofSafetySolutions;Persist Security Info=False;User ID=PerthCityCranes;Password=ABC1234!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"));
builder.Services.Configure<AzureOptions>(builder.Configuration.GetSection("Azure"));

builder.Services.AddTransient<IImageService,ImageService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
//    app.UseSwagger();
  //  app.UseSwaggerUI();

    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints=>endpoints.MapControllers());

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Inspections}/{action=Index}/{id?}");

app.Run();
