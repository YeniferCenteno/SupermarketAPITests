using Microsoft.EntityFrameworkCore;
using SupermarketAPI.DTOs;
using SupermarketAPI.Endpoints;
using SupermarketAPI.Models;
using SupermarketAPI.Services.Products;
using SupermarketAPI.Services.Users;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT en el siguiente formato: Bearer {token}"
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
            new string[]{ }
        }
    });
});

//Conexion a SQLServer
builder.Services.AddDbContext<SupermarketDbContext>(
    o => o.UseSqlServer(builder.Configuration.GetConnectionString("SupermarketDbConnection"))
);

//Registrando el MappingProfile
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

//Registrar Servicios
builder.Services.AddScoped<IProductServices, ProductServices>();
builder.Services.AddScoped<IUserServices, UserServices>();

//Acceder a la configuracion de la aplicaicon
var jwtSetting = builder.Configuration.GetSection("JwtSetting");
var secretKey = jwtSetting.GetValue<string>("SecretKey");


// Servicios de autorizacion y autenticacion
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(
    options =>
    {
        //Esquema por defecto
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(
    options =>
    {
        //Permite usar HTTP en lugar de HTTPS
        options.RequireHttpsMetadata = false;
        //Guardar el token en el contexto de autenticacion
        options.SaveToken = true;
        //Parametros de configuracion
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSetting.GetValue<string>("Issuer"),
            ValidAudience = jwtSetting.GetValue<string>("Audience"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

//
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Usar la autenticacion y verificacion
app.UseAuthentication();
app.UseAuthorization();

//Incorporar los Endpoints
app.UseEndpoints();


app.Run();

public partial class Program { }