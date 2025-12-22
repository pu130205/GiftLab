using GiftLab.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<GiftLabDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql =>
        {
            sql.CommandTimeout(60);
            sql.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null
            );
        }
    )
);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ================= AUTH: COOKIE + GOOGLE =================
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "GiftLab.Auth";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;

        // Callback mặc định:
        // https://localhost:<PORT>/signin-google
        // options.CallbackPath = "/signin-google"; // (không cần nếu dùng mặc định)

        // (Tuỳ chọn) luôn xin email
        options.Scope.Add("email");
        options.Scope.Add("profile");

        
    });

builder.Services.AddAuthorization();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GiftLabDbContext>();

    try
    {
        var dbName = db.Database.GetDbConnection().Database;
        Console.WriteLine("[SEED] DB = " + dbName);
        System.Diagnostics.Debug.WriteLine("[SEED] DB = " + dbName);

        db.Database.ExecuteSqlRaw(@"
            IF OBJECT_ID('dbo.SeedHistory', 'U') IS NULL
            BEGIN
                CREATE TABLE dbo.SeedHistory
                (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    SeedKey NVARCHAR(200) NOT NULL UNIQUE,
                    CreatedAt DATETIME NOT NULL DEFAULT(GETDATE())
                );
            END
        ");

        var seedKey = "sql_insert_data_v1";

        
        var sqlFile = Path.Combine("Data", "SQL-Insert-Data.sql");

       
        var conn = (SqlConnection)db.Database.GetDbConnection();
        if (conn.State != System.Data.ConnectionState.Open) conn.Open();

        using var cmd = new SqlCommand("SELECT COUNT(1) FROM dbo.SeedHistory WHERE SeedKey = @k", conn);
        cmd.Parameters.AddWithValue("@k", seedKey);

        var alreadySeeded = (int)cmd.ExecuteScalar() > 0;
        Console.WriteLine("[SEED] AlreadySeeded = " + alreadySeeded);
        System.Diagnostics.Debug.WriteLine("[SEED] AlreadySeeded = " + alreadySeeded);

        if (!alreadySeeded)
        {
            Console.WriteLine("[SEED] START: " + sqlFile);
            System.Diagnostics.Debug.WriteLine("[SEED] START: " + sqlFile);

        
            SqlSeedRunner.RunSqlFile(db, sqlFile);

            db.Database.ExecuteSqlRaw("INSERT INTO dbo.SeedHistory(SeedKey) VALUES ({0})", seedKey);

            Console.WriteLine("[SEED] === SEED OK ===");
            System.Diagnostics.Debug.WriteLine("[SEED] === SEED OK ===");
        }
        else
        {
            Console.WriteLine("[SEED] Skip (SeedHistory đã có seedKey)");
            System.Diagnostics.Debug.WriteLine("[SEED] Skip (SeedHistory đã có seedKey)");
        }

        var productCount = db.Products.Count();
        Console.WriteLine("[SEED] ProductCount = " + productCount);
        System.Diagnostics.Debug.WriteLine("[SEED] ProductCount = " + productCount);
    }
    catch (Exception ex)
    {
        Console.WriteLine("[SEED] === SEED FAIL ===");
        Console.WriteLine(ex.ToString());
        System.Diagnostics.Debug.WriteLine("[SEED] === SEED FAIL ===");
        System.Diagnostics.Debug.WriteLine(ex.ToString());
        throw;
    }
}

app.UseStaticFiles();
app.UseRouting();

app.UseSession();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
