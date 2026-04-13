using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddRouting(map => map.LowercaseUrls = true);
builder.Services.AddEndpointsApiExplorer(); //swagger
builder.Services.AddSwaggerGen(); //swagger

var app = builder.Build();


app.MapOpenApi();
app.UseSwagger(); //swagger
app.UseSwaggerUI(); //swagger

//scalar
app.MapScalarApiReference(options =>
{
    options.WithTheme(ScalarTheme.Purple);
});

app.UseAuthorization();

app.MapControllers();

app.Run();