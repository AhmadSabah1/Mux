using Core.ExternalDependencies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped<ITwitterApiClient>(s =>
{
    var apiKey = builder.Configuration["TwitterApiSettings:ApiKey"];
    var apiKeySecret = builder.Configuration["TwitterApiSettings:ApiKeySecret"];
    var tokenEndpoint = builder.Configuration["TwitterApiSettings:TokenEndpoint"];
    var twitterApiClient = new TwitterApiClient(apiKey, apiKeySecret, tokenEndpoint);
    
    return twitterApiClient;
});

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