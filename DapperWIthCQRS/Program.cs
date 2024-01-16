using DapperWIthCQRS.Application.Command;
using DapperWIthCQRS.Application.Handlers;
using DapperWIthCQRS.Application.Queries;
using DapperWIthCQRS.Domain.Models;
using DapperWIthCQRS.Infrastructure.Context;
using DapperWIthCQRS.Infrastructure.Helper.RabbitMQHelper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Data;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddTransient<IDbConnection>(_ => new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
builder.Services.AddTransient<IRequestHandler<GetProductQueries, IEnumerable<Product>>, GetProductQueriesHandler>();
builder.Services.AddTransient<IRequestHandler<GetProductByIdQueries, Product>, GetProductByIdQueriesHandler>();
builder.Services.AddTransient<IRequestHandler<CreateProductCommand, Product>, CreateProductCommandHandler>();
builder.Services.AddTransient<IRequestHandler<UpdateProductCommand>, UpdateProductCommandHandler>();
builder.Services.AddTransient<IRequestHandler<DeleteProductCommand>, DeleteProductCommandHandler>();
builder.Services.AddTransient<IRequestHandler<GetUserQueries, Users>, GetUserQueriesHandler>();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DapperWithCQRS.API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Jwt Authorization",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"

    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference= new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
             new string[] { }
        }
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]))
    };
});
//builder.Services.AddSingleton<RabbitMQHelper>(_ =>
//{
//    var configuration = builder.Configuration.GetSection("RabbitMQ");
//    return new RabbitMQHelper(
//        configuration["Host"],
//        int.Parse(configuration["Port"]),
//        configuration["Username"],
//        configuration["Password"],
//        configuration["Exchange"],
//        configuration["Queue"],
//        configuration["RoutingKey"]
//    );
//});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.jason", "DapperWithCQRS.API v1"));
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
