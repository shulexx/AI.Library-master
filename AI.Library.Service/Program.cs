using AI.Library.Service.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins(
                "https://localhost:7111",
                "http://localhost:7111",
                "https://localhost:5016",
                "http://localhost:5016"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});


builder.Services.AddSingleton<EmbedService>();
builder.Services.AddSingleton<RagService>();
builder.Services.AddSingleton<LlmService>();
builder.Services.AddSingleton<LlmRouter>();
builder.Services.AddSingleton<TtsService>();
builder.Services.AddSingleton<SttService>();
builder.Services.AddSingleton<BookSummaryService>();
builder.Services.AddSingleton<OcrService>();
builder.Services.AddSingleton<TopicService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AI Library Service", Version = "v1" });
});

var app = builder.Build();
app.UseCors("AllowFrontend");

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
