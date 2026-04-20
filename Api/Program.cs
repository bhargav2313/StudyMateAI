using Application.Services;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Swagger add cheyyali (IMPORTANT 🔥)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔹 Register services
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddHttpClient<AiService>();

// 🔹 Controllers
builder.Services.AddControllers();

var app = builder.Build();

// 🔹 Swagger enable
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 🔹 Controllers map
app.MapControllers();

app.UseHttpsRedirection();

app.Run();