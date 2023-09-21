using Docker.DotNet;
using Docker.DotNet.Models;
using InfoSafe.Read.Data;
using InfoSafe.Write.Data;
using InfoSafe.Write.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using SharedKernel.Utils;
using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace InfoSafe.IntegrationTests
{
    public class ApiWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly DockerClient _dockerClient;
        private const string ContainerImageUri = "mcr.microsoft.com/mssql/server:2022-latest";
        private string _containerId;

        string DBConnectionString = "Server=.,1434;Database=TestAlintaPoCDb_Test;User ID=sa;Password=Admin1234;TrustServerCertificate=True";

        public ApiWebApplicationFactory()
        {
            _dockerClient = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine")).CreateClient();
        }

        public async Task InitializeAsync()
        {
            await PullImage();

            await StartContainer();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var readDbContexttDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(ReadDbContext));
                services.Remove(readDbContexttDescriptor);

                var writeDbContexttDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<WriteDbContext>));
                services.Remove(writeDbContexttDescriptor);

                services.AddScoped(x =>
                    new ReadDbContext(new SqlConnection(DBConnectionString))
                );

                services.AddDbContext<WriteDbContext>(options =>
                    options.UseSqlServer(DBConnectionString, builder =>
                    {
                        builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                    })
                );


                // Build the service provider.
                var serviceProvider = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                using var scope = serviceProvider.CreateScope();

                var db = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<WebApplicationFactory<Program>>>();

                // Ensure the database is created.
                db.Database.EnsureCreated();

                try
                {
                    // Seed the database with test data.
                    SeedDb(db);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"An error occurred seeding the database with test messages.Error: {ex.Message} ");
                }
            });

            builder.UseEnvironment("IntegrationTest");
        }

        private async Task PullImage()
        {
            await _dockerClient.Images.CreateImageAsync(new ImagesCreateParameters
            {
                FromImage = ContainerImageUri
            },
            new AuthConfig(),
            new Progress<JSONMessage>());
        }

        private async Task StartContainer()
        {
            var response = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = ContainerImageUri,
                AttachStderr = true,
                AttachStdin = true,
                AttachStdout = true,
                Env = new[] { "ACCEPT_EULA=Y", $"SA_PASSWORD=Admin1234" },
                ExposedPorts = new Dictionary<string, EmptyStruct>
                {
                    {
                        "1433", default(EmptyStruct)
                    }
                },
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        { "1433", new List<PortBinding> { new PortBinding { HostPort = "1434" } } }
                    },
                    PublishAllPorts = true
                }
            });

            _containerId = response.ID;

            await _dockerClient.Containers.StartContainerAsync(_containerId, null);
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            if (_containerId != null)
            {
                await _dockerClient.Containers.KillContainerAsync(_containerId, new ContainerKillParameters());

                await _dockerClient.Containers.RemoveContainerAsync(_containerId, new ContainerRemoveParameters());
            }
        }

        private void SeedDb(WriteDbContext context)
        {
            var data = JsonFileReader.Read<Contact>(@"contact_seed.json", @"IntegrationTestsData\");

            context.Contacts.Add(data);

            context.SaveChanges();
        }

    }
}