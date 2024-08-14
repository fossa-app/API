using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation;
using Fossa.API.Core;
using Fossa.API.Core.Messages;
using Fossa.API.Core.Services;
using Fossa.API.Infrastructure;
using Fossa.API.Persistence;
using Fossa.API.Web;
using Fossa.API.Web.DependencyInjection;
using Fossa.Licensing;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Serilog;
using TIKSN.DependencyInjection;
using TIKSN.Mapping;
using static LanguageExt.Prelude;

var initialReleaseDate = new DateOnly(2023, 07, 15);

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

builder.Services.Configure<PagingQueryOptions>(builder.Configuration.GetSection("Paging"));
builder.Services.AddIdGen(builder.Configuration, initialReleaseDate);
builder.Services.AddSingleton<IdGenSetupLogger>();

builder.Services.AddAuthentication(options => options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
  var identitySection = builder.Configuration.GetSection("Identity");
  options.Authority = identitySection.GetValue<string>("RootAddress");
  options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
  options.Audience = identitySection.GetValue<string>("Audience");
  options.TokenValidationParameters.ValidateAudience = !builder.Environment.IsDevelopment();
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddProblemDetails(ProblemDetailsHelper.ConfigureProblemDetails);
builder.Services.AddControllers()
                .AddProblemDetailsConventions();
builder.Services.AddApiVersioning(
                    options =>
                    {
                      options.ReportApiVersions = true;
                      options.AssumeDefaultVersionWhenUnspecified = false;
                    })
                .AddMvc()
                .AddApiExplorer(
                    options =>
                    {
                      options.AssumeDefaultVersionWhenUnspecified = false;
                      options.SubstituteApiVersionInUrl = true;
                      options.GroupNameFormat = "'v'VVV";
                    });
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "FossaApp API", Version = "v1" });
  c.EnableAnnotations();
});

builder.Services.AddFrameworkCore();
builder.Services.AddLicense();
builder.Services
  .AddHealthChecks()
  .AddMongoDb(builder.Configuration.GetConnectionString("MongoDB") ?? string.Empty)
  .AddIdentityServer(new Uri(
    builder.Configuration.GetSection("Identity").GetValue<string>("RootAddress") ?? string.Empty))
  .AddSystemLicense();

var assemblies = Seq(
    typeof(DefaultCoreModule),
    typeof(DefaultInfrastructureModule),
    typeof(DefaultPersistenceModule),
    typeof(DefaultWebModule))
    .Map(x => x.Assembly)
    .ToArray();

builder.Services.AddMediatR(cfg =>
{
  cfg.Lifetime = ServiceLifetime.Scoped;
  cfg.RegisterServicesFromAssemblies(assemblies);
  cfg.AddOpenBehavior(typeof(TenantRequestBehavior<,,,>));
  cfg.AddOpenBehavior(typeof(PagingQueryBehavior<,>));
  cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddValidatorsFromAssemblies(assemblies);

builder.Services.Scan(scan => scan
    .FromAssemblies(assemblies)
        .AddClasses(classes => classes.AssignableTo(typeof(IMapper<,>)))
            .AsImplementedInterfaces());

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
  containerBuilder.RegisterModule<CoreModule>();
  containerBuilder.RegisterModule(new DefaultCoreModule());
  containerBuilder.RegisterModule(new DefaultInfrastructureModule(string.Equals(builder.Environment.EnvironmentName, "Development", StringComparison.OrdinalIgnoreCase)));
  containerBuilder.RegisterModule<DefaultPersistenceModule>();
  containerBuilder.RegisterModule<DefaultWebModule>();
});

//builder.Logging.AddAzureWebAppDiagnostics(); add this if deploying to Azure

var app = builder.Build();

app.UseProblemDetails();
app.UseRouting();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();
app.UseHealthChecks("/healthchecks");

app.UseAuthentication();
app.UseAuthorization();

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();

app.UseSwaggerUI(
    c =>
    {
      var descriptions = app.DescribeApiVersions();

      for (var i = 0; i < descriptions.Count; i++)
      {
        var description = descriptions[i];
        var url = $"/swagger/{description.GroupName}/swagger.json";
        var name = description.GroupName.ToUpperInvariant();
        c.SwaggerEndpoint(url, name);
      }
    });

app.MapDefaultControllerRoute();

app.Services.GetRequiredService<IdGenSetupLogger>().LogIdGenSetup();

await app.Services.GetRequiredService<ISystemInitializer>()
  .InitializeAsync(default)
  .ConfigureAwait(false);

await app.RunAsync().ConfigureAwait(false);
