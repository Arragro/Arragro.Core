using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arragro.Core.Docker
{
    public static class LocalStripe
    {
        public static async Task<ContainerListResponse> StartLocalStripe(DockerClient client)
        {
            const string ContainerName = "localstripe";
            const string ImageName = "adrienverge/localstripe";
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
                    { "8420/tcp", new List<PortBinding> { new PortBinding { HostIP = "127.0.0.1", HostPort = "8420" } } }
                }
            };

            await client.Containers.CreateContainerAsync(new CreateContainerParameters(config)
            {
                Image = ImageName + ":" + ImageTag,
                Name = ContainerName,
                Tty = false,
                HostConfig = hostConfig
            });

            var container = await DockerExtentions.GetContainerAsync(client, ContainerName);
            if (container == null)
                throw new Exception("No localstripe container.");

            if (container.State != "running")
            {
                var started = await client.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());
                if (!started)
                    throw new Exception("Cannot start the localstripe docker container.");
            }

            return container;
        }
    }
}
