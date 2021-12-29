using IkeaNotifier.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using System.Globalization;

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

		services.AddOptions<MongoOptions>();
		services.AddOptions<BotOptions>();

		services.AddSingleton(provider => new MongoClient(provider.GetService<MongoOptions>().ConnectionString));
	}

	// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();

			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
		}

		app.UseRouting();

		app.UseEndpoints(endpoints => endpoints.MapControllers());
	}
}