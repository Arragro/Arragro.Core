using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.Docker
{
    public static class Postgres
    {
        public static async Task<ContainerListResponse> StartPostgres(DockerClient client)
        {
            const string ContainerName = "postgres-integration-tests";
            const string ImageName = "postgres";
            const string ImageTag = "10.7-alpine";

            await DockerExtentions.EnsureImageExistsAndCleanupAsync(client, ImageName, ImageTag, ContainerName);

            var config = new Config();

            var hostConfig = new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    { "5432/tcp", new List<PortBinding> { new PortBinding { HostIP = "", HostPort = "5432" } } }
                }
            };

            await client.Containers.CreateContainerAsync(new CreateContainerParameters(config)
            {
                Image = ImageName + ":" + ImageTag,
                Name = ContainerName,
                Tty = false,
                HostConfig = hostConfig,
                Env = new List<string>
                {
                    "POSTGRES_PASSWORD=password1"
                },
                Cmd = new List<string>
                {
                    "--max_prepared_transactions=100"
                }
            });

            var container = await DockerExtentions.GetContainerAsync(client, ContainerName);
            if (container == null)
                throw new Exception("No Postgres container.");

            if (container.State != "running")
            {
                var started = await client.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());
                if (!started)
                    throw new Exception("Cannot start the postgres docker container.");
            }

            var isContainerReady = false;
            var isReadyCounter = 0;

            while (!isContainerReady)
            {
                isReadyCounter++;
                var result = await client.Containers.RunCommandInContainerAsync(container.ID, "pg_isready -U postgres");
                if (result.stdout.TrimEnd('\n') == "/var/run/postgresql:5432 - accepting connections")
                {
                    isContainerReady = true;
                }

                if (isReadyCounter == 20)
                    throw new Exception("Postgres container never ready.");

                if (!isContainerReady)
                    Thread.Sleep(1000);
            }

            return container;
        }
    }
}
