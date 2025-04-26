using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SotrageHub.Application;
using StorageHub.Infrastructure;
using StorageHub.API.Helper;
using Microsoft.OpenApi.Models;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

//Apply migrations at runtime
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // Applies any pending migrations
}


app.Run();
