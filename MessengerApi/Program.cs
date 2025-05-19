using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MessengerApi.Bots;
using MessengerApi.Hubs;
using MessengerApi.Middleware;
using MessengerApiDomain.RepositoryInterfaces;
using MessengerApiInfrasctructure.Data;
using MessengerApiInfrasctructure.Repositories;
using MessengerApiServices.Interfaces;
using MessengerApiServices.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("BearerAuth", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "BearerAuth" }
                },
                Array.Empty<string>()
            }
        });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IOperationLogRepository, OperationLogRepository>();
builder.Services.AddScoped<ICalendarEventRepository, CalendarEventRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<ICalendarEventService, CalendarEventService>();

builder.Services.AddScoped<EventBot>();

builder.Services.AddHostedService<EventNotificationService>();

builder.Services.AddDbContextPool<DataContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    options
        .UseLazyLoadingProxies()
        .UseSqlServer(connectionString);
});

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            builder.Configuration.GetSection("Jwt:Token").Value!
        ))
    };
});

builder.Services.AddSingleton(provider =>
{
    var connectionString = builder.Configuration.GetValue<string>("AzureBlobStorage:ConnectionString");

    return new BlobServiceClient(connectionString);
});

builder.Services.AddSignalR();

// TODO: Add exception handling middleware and change methods to throw an exception if not found

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.UseMiddleware<BannedUserAuthorizationBlockerMiddleware>();

app.MapControllers();

app.MapHub<ChatHub>("api/chat-hub");
app.MapHub<UserChatsHub>("api/user-chats-hub");

await CreateContainerIfNotExistsAsync();

app.Run();



async Task CreateContainerIfNotExistsAsync()
{
    var connectionString = builder.Configuration.GetValue<string>("AzureBlobStorage:ConnectionString");

    var blobServiceClient = new BlobServiceClient(connectionString);

    var blobContainerName = builder.Configuration.GetSection("AzureBlobStorage:ContainerName").Value!;
    var containerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
    var createdContainer = containerClient.CreateIfNotExists();
    containerClient.SetAccessPolicy(PublicAccessType.Blob);

    if (createdContainer is null)
    {
        return;
    }

    var imagePath = @"./Images/default-avatar.png";
    var fileName = Path.GetFileName(imagePath);
    var blobClient = containerClient.GetBlobClient(fileName);
    using var stream = File.OpenRead(imagePath);
    await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = "image/png" });

    imagePath = @"./Images/event-bot.png";
    fileName = Path.GetFileName(imagePath);
    blobClient = containerClient.GetBlobClient(fileName);
    using var stream2 = File.OpenRead(imagePath);
    await blobClient.UploadAsync(stream2, new BlobHttpHeaders { ContentType = "image/png" });
}