using Azure.Storage.Blobs;
using MessengerApi.Hubs;
using MessengerApi.Middleware;
using MessengerApiDomain.RepositoryInterfaces;
using MessengerApiInfrasctructure.Data;
using MessengerApiInfrasctructure.Repositories;
using MessengerApiServices.Interfaces;
using MessengerApiServices.Services;
using Microsoft.EntityFrameworkCore;
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

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IImageService, ImageService>();

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

app.Run();
