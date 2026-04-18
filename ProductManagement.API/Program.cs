using Microsoft.EntityFrameworkCore;
using ProductManagement.Infra.Data.MongoDB.Contexts;
using ProductManagement.Infra.Data.SqlServer.Contexts;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddRouting(map => map.LowercaseUrls = true);

builder.Services.AddEndpointsApiExplorer(); //Swagger
builder.Services.AddSwaggerGen(); //Swagger

//Configuração para injeção de dependência do MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
});

//Configuração para injeção de dependência do Entity Framework
builder.Services.AddDbContext<SqlServerContext>
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("ProductsDB")));

//Configuração para injeção de dependência do MongoDB
builder.Services.AddSingleton
    (ctx => new MongoDbContext(
            builder.Configuration.GetSection("MongoDbSettings:ConnectionString").Value!,
            builder.Configuration.GetSection("MongoDbSettings:DatabaseName").Value!
        ));

var app = builder.Build();

app.MapOpenApi();

app.UseSwagger(); //Swagger
app.UseSwaggerUI(); //Swagger

//Scalar
app.MapScalarApiReference(options =>
{
    options.WithTheme(ScalarTheme.Mars);
});

app.UseAuthorization();

app.MapControllers();

app.Run();