using Microsoft.EntityFrameworkCore;
using GalleryNote.DAL;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ImageDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ImageDbContextConnection' not found.");


builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<ImageDbContext>(options => {
    options.UseSqlite(
        builder.Configuration["ConnectionStrings:ImageDbContextConnection"]);
});


builder.Services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<ImageDbContext>();

builder.Services.AddScoped<IImageRepository, ImageRepository>();

builder.Services.AddScoped<INoteRepository, NoteRepository>();


builder.Services.AddRazorPages(); //order of adding services does not matter
builder.Services.AddSession();


var loggerConfiguration = new LoggerConfiguration()
    .MinimumLevel.Information() // evels: Trace< Information < Warning < Erorr < Fatal
    .WriteTo.File($"Logs/app_{DateTime.Now:yyyyMMdd_HHmmss}.log");

loggerConfiguration.Filter.ByExcluding(e => e.Properties.TryGetValue("SourceContext", out var value) &&
                            e.Level == LogEventLevel.Information &&
                            e.MessageTemplate.Text.Contains("Executed DbCommand"));

var logger = loggerConfiguration.CreateLogger();
builder.Logging.AddSerilog(logger);


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    DBInit.Seed(app);
}


app.UseStaticFiles();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultControllerRoute();
app.MapRazorPages();
app.Run();