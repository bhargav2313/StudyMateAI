using Application.Services;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔹 Services
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddHttpClient<AiService>();

// 🔹 Controllers
builder.Services.AddControllers();

// 🔥 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// 🔹 Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 🔥 VERY IMPORTANT ORDER
app.UseHttpsRedirection();

app.UseCors("AllowAll");   // ✅ AFTER HTTPS

app.MapControllers();     // ✅ AFTER CORS

app.Run();