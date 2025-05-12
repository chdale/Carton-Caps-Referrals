using CartonCapsAPI.Services;
using CartonCapsAPI.Services.TestServices;

var builder = WebApplication.CreateBuilder(args);

// Use test services for development
builder.Services.AddSingleton<ISmsService, TestSmsService>();
builder.Services.AddSingleton<IUserService, TestUserService>();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
