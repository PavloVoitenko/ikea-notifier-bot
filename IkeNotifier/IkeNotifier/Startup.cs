using IkeaNotifier.Options;
using IkeaNotifier.Services;
using IkeaNotifier.Services.Repositories;
using IkeaNotifier.Services.TgApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Globalization;
using System.Net.Http;

namespace IkeaNotifier;

public class Startup
{
	public Startup(IConfiguration configuration)
	{
		Configuration = configuration;
	}

	public IConfiguration Configuration { get; }

	// This method gets called by the runtime. Use this method to add services to the container.
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddControllers();

		services.Configure<MongoOptions>(Configuration.GetSection(nameof(MongoOptions)));
		services.Configure<BotOptions>(Configuration.GetSection(nameof(BotOptions)));

		services.AddSingleton(provider => new MongoClient(provider.GetService<IOptions<MongoOptions>>().Value.ConnectionString));
		services.AddSingleton<BotStatsRepository>();
		services.AddSingleton<UserRepository>();
		services.AddSingleton<HttpClient>();

		services.AddSingleton<MessageTypeService>();
		services.AddSingleton<RequestService>();
		services.AddSingleton<UpdateService>();
		services.AddSingleton<UpdateContainerService>();
		services.AddSingleton<PollingService>();
		services.AddSingleton<UserEndpointService>();

		services.AddSwaggerGen();
	}

	// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();

			app.UseSwagger();
			app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"));

			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
		}

		app.UseRouting();

		app.UseEndpoints(endpoints => endpoints.MapControllers());
	}
}