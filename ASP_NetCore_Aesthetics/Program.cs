
using Aesthetics.DataAccess.NetCore.Dapper;
using Aesthetics.DataAccess.NetCore.DBContext;
using Aesthetics.DataAccess.NetCore.Repositories.Implement;
using Aesthetics.DataAccess.NetCore.Repositories.Impliment;
using Aesthetics.DataAccess.NetCore.Repositories.Interface;
using Aesthetics.DataAccess.NetCore.Repositories.Interfaces;
using Aesthetics.DTO.NetCore.DataObject;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddDbContext<DB_Context>(options =>
			   options.UseSqlServer(configuration.GetConnectionString("Aesthetics_ConnString")));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = false,
		ValidateAudience = false,
		ValidateLifetime = false,
		ValidateIssuerSigningKey = false,
		ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],
		ValidAudience = builder.Configuration["Jwt:ValidAudience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
	};
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddTransient<IAccountRepository,AccountRepository>();
builder.Services.AddTransient<IUserSessionRepository,UserSessionRepository>();
builder.Services.AddTransient<IApplicationDbConnection,ApplicationDbConnection>();
builder.Services.AddTransient<IUserRepository,UserRepository>();
builder.Services.AddTransient<ISupplierRepository, SupplierRepository>();
builder.Services.AddTransient<ITypeProductsOfServicesRepository, TypeProductsOfServicesRepository>();
builder.Services.AddTransient<IServicessRepository, ServicessRepository>();
builder.Services.AddTransient<IBookingsRepository, BookingsRepository>();
builder.Services.AddTransient<IClinicRepository, ClinicRepository>();
builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = configuration["RedisCacheUrl"]; });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles(new StaticFileOptions
{
	OnPrepareResponse = ctx =>
	{
		ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
		ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers",
		  "Origin, X-Requested-With, Content-Type, Accept");
	},
	FileProvider = new PhysicalFileProvider(
		   Path.Combine(builder.Environment.ContentRootPath, "FilesImages/Servicess")),
	RequestPath = "/FilesImages/Servicess"
});
app.MapControllers();
app.Run();
