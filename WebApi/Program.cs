using Application.DTOs.Account;
using Application.DTOs.Branch;
using Application.DTOs.Car;
using Application.DTOs.Customer;
using Application.DTOs.Rental;
using Application.Filters;
using Application.Filters.BranchFilter;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Seeds;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Контроллеры + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger с JWT авторизацией
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FleetMaster API",
        Version = "v1",
        Description = "Автопарк — CRUD + Авторизация + Статистика",
        Contact = new OpenApiContact
        {
            Name = "Nozim Qoziev",
            Email = "nozimjonqoziev@gmail.com"
        }
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите JWT токен: Bearer {ваш токен}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Настройка PostgreSQL
builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 4;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<DataContext>()
.AddDefaultTokenProviders();

// JWT
var jwtConfig = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig["Issuer"],
        ValidAudience = jwtConfig["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]!)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Регистрация Email / Account сервисов
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAccountService, AccountService>();

// Регистрация репозиториев
builder.Services.AddScoped<IBaseRepository<Car, int>, CarRepository>();
builder.Services.AddScoped<IBaseRepository<Customer, int>, CustomerRepository>();
builder.Services.AddScoped<IBaseRepository<Branch, int>, BranchRepository>();
builder.Services.AddScoped<IBaseRepository<Rental, int>, RentalRepository>();

// Регистрация бизнес-сервисов
builder.Services.AddScoped<IBaseService<CreateCarDTO, GetCarDTO, UpdateCarDTO, FilterCar>, CarService>();
builder.Services.AddScoped<IBaseService<CreateCustomerDTO, GetCustomerDTO, UpdateCustomerDTO, FilterCustomer>, CustomerService>();
builder.Services.AddScoped<IBaseService<CreateBranchDTO, GetBranchDTO, UpdateBranchDTO, FilterBranch>, BranchService>();
builder.Services.AddScoped<IBaseService<CreateRentalDTO, GetRentalDTO, UpdateRentalDTO, FilterRental>, RentalService>();

// Регистрация статистики
builder.Services.AddScoped<IStatisticsService, StatisticsService>();

var app = builder.Build();

// Сидим роли и пользователей
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();

    await DefaultRoles.SeedRolesAsync(roleManager);
    await DefaultUsers.SeedUsersAsync(userManager, roleManager, context);
}

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opt =>
    {
        opt.SwaggerEndpoint("/swagger/v1/swagger.json", "FleetMaster API v1");
        opt.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Добавляем свой middleware (вызывается через pipeline)
app.UseMiddleware<RequestLoggingMiddleware>();

app.MapControllers();

app.Run();
