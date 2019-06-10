using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arragro.Core.Docker
{
    public static class PgAdmin
    {
        public static async Task<ContainerListResponse> StartPgAdmin(DockerClient client)
        {
            const string ContainerName = "pgadmin-integration-tests";
            const string ImageName = "dpage/pgadmin4";
            const string ImageTag = "latest";

            await DockerExtentions.EnsureImageExistsAndCleanupAsync(client, ImageName, ImageTag, ContainerName);

            var config = new Config()
            {
                Hostname = "localhost"
            };

            var hostConfig = new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    { "80/tcp", new List<PortBinding> { new PortBinding { HostIP = "127.0.0.1", HostPort = "5081" } } }
                },
                Binds = new List<string>
                {
                    "/tmp/var/lib/pgadmin:/var/lib/pgadmin",
                    "/tmp/servers.json:/servers.json"
                }
            };

            var response = await client.Containers.CreateContainerAsync(new CreateContainerParameters(config)
            {
                Image = ImageName + ":" + ImageTag,
                Name = ContainerName,
                Tty = false,
                HostConfig = hostConfig,
                Env = new List<string>
                {
                    "PGADMIN_DEFAULT_EMAIL=support@arragro.com",
                    "PGADMIN_DEFAULT_PASSWORD=@rr@gr0",

                }
            });

            var container = await DockerExtentions.GetContainerAsync(client, ContainerName);
            if (container == null)
                throw new Exception("No PgAdmin container.");

            if (container.State != "running")
            {
                var started = await client.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());
                if (!started)
                    throw new Exception("Cannot start the PgAdmin docker container.");
            }

            return container;
        }
    }
}
