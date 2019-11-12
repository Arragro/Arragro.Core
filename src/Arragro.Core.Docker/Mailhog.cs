using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arragro.Core.Docker
{
    public static class Mailhog
    {
        public static async Task<ContainerListResponse> StartMailhog(DockerClient client)
        {
            const string ContainerName = "mailhog-integration-tests";
            const string ImageName = "mailhog/mailhog";
            const string ImageTag = "latest";

            await DockerExtentions.EnsureImageExistsAndCleanupAsync(client, ImageName, ImageTag, ContainerName);

            var config = new Config();

            var hostConfig = new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    { "8025/tcp", new List<PortBinding> { new PortBinding { HostIP = "", HostPort = "5080" } } },
                    { "1025/tcp", new List<PortBinding> { new PortBinding { HostIP = "", HostPort = "25" } } }
                }
            };

            var response = await client.Containers.CreateContainerAsync(new CreateContainerParameters(config)
            {
                Image = ImageName + ":" + ImageTag,
                Name = ContainerName,
                Tty = false,
                HostConfig = hostConfig
            });

            var container = await DockerExtentions.GetContainerAsync(client, ContainerName);
            if (container == null)
                throw new Exception("No Mailhog container.");

            if (container.State != "running")
            {
                var started = await client.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());
                if (!started)
                    throw new Exception("Cannot start the Mailhog docker container.");
            }

            return container;
        }
    }
}
