using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SalesDashBoardApplication;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddHttpClient<SalesDashBoardClient>(options =>
{
    options.BaseAddress = new Uri("https://localhost:7079/");
});

builder.Services.AddTransient<SalesDashBoardClient>(options =>
{
    var httpClient = options.GetRequiredService<HttpClient>();
    return new SalesDashBoardClient("https://localhost:7079/", httpClient);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("SpecificOrigin",
        builder => builder.WithOrigins("http://localhost:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader());
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };

    //options.Events = new JwtBearerEvents
    //{
    //    OnMessageReceived = context =>
    //    {
    //        Console.WriteLine("\n" + context.Request.Cookies + "\n");
    //        var token = context.Request.Cookies["jwtToken"];
    //        if (!string.IsNullOrEmpty(token))
    //        {
    //            context.Token = token;
    //        }
    //        return Task.CompletedTask;
    //    }
    //};
});

builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Please insert JWT token into field"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
        new OpenApiSecurityScheme {
            Reference = new OpenApiReference {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] { }
    }});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("SpecificOrigin");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
