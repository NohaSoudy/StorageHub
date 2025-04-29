using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SotrageHub.Application;
using StorageHub.Infrastructure;
using StorageHub.API;
using Microsoft.OpenApi.Models;
using Amazon.Runtime;
using Amazon.S3;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Setup MinIO connection
builder.Services.Configure<MinioSettings>(builder.Configuration.GetSection("MinioSettings"));
var credentials = new BasicAWSCredentials(
builder.Configuration["MinioSettings:AccessKey"], // AccessKey
builder.Configuration["MinioSettings:SecretKey"] // SecretKey
);

var config = new AmazonS3Config
{
    ServiceURL = builder.Configuration["MinioSettings:Endpoint"],
    ForcePathStyle = true
};

var s3Client = new AmazonS3Client(credentials, config);
builder.Services.AddSingleton<IAmazonS3>(s3Client);

// Setup database connection
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));
var databaseProvider = builder.Configuration.GetValue<string>("DatabaseSettings:Provider");
var databaseName = builder.Configuration.GetValue<string>("DatabaseSettings:InMemory");



if (databaseProvider == DatabaseProvider.SQLite.ToString())
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("SQLite")));
}
else if (databaseProvider == DatabaseProvider.SQLServer.ToString())
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));
}
else if (databaseProvider == DatabaseProvider.InMemory.ToString())
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase(databaseName));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("SQLite")));
}

//DI
builder.Services.AddScoped<IFileHubService, FileHubService>();
builder.Services.AddScoped<IFileHubRepository, FileHubRepository>();


//Configure JWT
// Register JWT settings into DI (optional but useful)
builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWT"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = false;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]))
    };
}
    );
//Configure Swagger
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    //c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Your API", Version = "v1" });
    c.CustomOperationIds(type => type.ToString());
    //Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token.\n\nExample: Bearer 12345abcdef"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Name = "Bearer",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 52428800; // 50 MB
});
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//Apply migrations at runtime for SQLite and SQLServer
if (databaseProvider != DatabaseProvider.InMemory.ToString())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated(); // Applies any pending migrations
    }
}


// Auto create MinIO bucket
using (var scope = app.Services.CreateScope())
{
    var minioClient = scope.ServiceProvider.GetRequiredService<IAmazonS3>();
    var bucketName = builder.Configuration["MinioSettings:BucketName"];

    var listBucketsResponse = await minioClient.ListBucketsAsync();
    if (!listBucketsResponse.Buckets.Any(b => b.BucketName == bucketName))
    {
        await minioClient.PutBucketAsync(bucketName);
    }
}

app.Run();
