using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using PIMS_MS.Common.Interfaces;
using PIMS_MS.Modules.Identity.Services;
using PIMS_MS.Modules.Identity.Database;
using PIMS_MS.Modules.Inventory.Database;
using PIMS_MS.Modules.Logistics.Database;
using PIMS_MS.Modules.FieldService.Database;
using PIMS_MS.Common.Behaviors;
using FluentValidation;
using PIMS_MS.Common.Exceptions;
using PIMS_MS.Api.Service;
using Microsoft.AspNetCore.Identity;
using PIMS_MS.Modules.Identity.Domain.Constants;
using PIMS_MS.Common.Interceptors;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "Multi-Southcart Provincial Inventory Management System", 
        Version = "v1" 
    });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando el esquema Bearer. \n\nEscribe 'Bearer' [espacio] y luego tu token.\nEjemplo: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document), 
        
            new List<string>()
        }
    });

    c.CustomSchemaIds(type =>
    {
        if (type.DeclaringType != null)
        {
            return $"{type.DeclaringType.Name}{type.Name}";
        }

        return type.Name;
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDbContext<InventoryDbContext>((sp, options) =>
{
    var interceptor = sp.GetRequiredService<UpdateAuditableEntitiesInterceptor>();
    options.UseNpgsql(connectionString)
           .AddInterceptors(interceptor);
});
builder.Services.AddDbContext<LogisticDbContext>((sp, options) =>
{
    var interceptor = sp.GetRequiredService<UpdateAuditableEntitiesInterceptor>();
    options.UseNpgsql(connectionString)
           .AddInterceptors(interceptor);
});
builder.Services.AddDbContext<FieldServiceDbContext>(options =>
    options.UseNpgsql(connectionString));
    
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddMediatR(cfg => 
{
    // 1. Registra todos los ensamblados donde hay Handlers (API + Módulos)
    cfg.RegisterServicesFromAssemblies(
        typeof(Program).Assembly,
        PIMS_MS.Modules.Inventory.AssemblyReference.Assembly,
        PIMS_MS.Modules.Identity.AssemblyReference.Assembly,
        PIMS_MS.Modules.Logistics.AssemblyReference.Assembly,
        PIMS_MS.Modules.FieldService.AssemblyReference.Assembly,
        PIMS_MS.Modules.Notifications.AssemblyReference.Assembly
    );

    // 2. Activa los Behaviors globales (Se ejecutan en el orden en que se declaran)
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(PerformanceBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ICurrentService, CurrentService>();
builder.Services.AddScoped<UpdateAuditableEntitiesInterceptor>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin() // O pon la URL de tu React en producción
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secret = builder.Configuration["JwtSettings:Secret"] 
            ?? throw new InvalidOperationException("El secreto JWT no se encontró en la configuración del servidor.");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),

            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization(options =>
{
    var adminPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole(Roles.Administrator)
        .Build();

    options.FallbackPolicy = adminPolicy;
});

var assembliesToScan = new[]
{
    typeof(Program).Assembly,
    PIMS_MS.Modules.Inventory.AssemblyReference.Assembly,
    PIMS_MS.Modules.Identity.AssemblyReference.Assembly,
    PIMS_MS.Modules.Logistics.AssemblyReference.Assembly,
    PIMS_MS.Modules.FieldService.AssemblyReference.Assembly,
    PIMS_MS.Modules.Notifications.AssemblyReference.Assembly  
};

var endpoints = assembliesToScan
    .SelectMany(assembly => assembly.DefinedTypes)
    .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                   type.IsAssignableTo(typeof(IEndpoint)))
    .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
    .ToArray();


builder.Services.TryAddEnumerable(endpoints);

builder.Services.AddHttpContextAccessor();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); 

    app.UseSwaggerUI(options =>
    {
        // 2. Apuntamos a la ruta por defecto donde Swashbuckle crea el archivo
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "PIMS-MS V1");

        // ESTA LÍNEA HACE QUE SWAGGER CARGUE EN LA RAÍZ (localhost:puerto/)
        options.RoutePrefix = string.Empty;
    });
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

var registeredEndpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();
foreach (var endpoint in registeredEndpoints)
{
    endpoint.MapEndpoint(app);
}

app.Run();
