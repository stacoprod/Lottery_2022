using DataLayer;
using Lottery_2022.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<LotteryDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IGameService, GameService>();


var app = builder.Build();

/*builder = new BackgroundTaskBuilder();
builder.Name = "My Background Trigger";
builder.SetTrigger(new TimeTrigger(15, true));
// Do not set builder.TaskEntryPoint for in-process background tasks
// Here we register the task and work will start based on the time trigger.
BackgroundTaskRegistration task = builder.Register();
*/
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

