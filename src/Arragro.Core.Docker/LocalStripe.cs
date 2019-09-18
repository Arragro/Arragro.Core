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
            const string ImageName = "mikejewell/localstripe";
            const string ImageTag = "v0.0.6-alpha";

            await DockerExtentions.EnsureImageExistsAndCleanupAsync(client, ImageName, ImageTag, ContainerName);

            var config = new Config();

            var hostConfig = new HostConfig
            {
                PublishAllPorts = true,
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    { "8420/tcp", new List<PortBinding> { new PortBinding { HostIP = "", HostPort = "8420" } } }
                }
            };

            await client.Containers.CreateContainerAsync(new CreateContainerParameters(config)
            {
                Image = ImageName + ":" + ImageTag,
                Name = ContainerName,
                ExposedPorts = new Dictionary<string, object>() {
                    { "8420/tcp", new { HostPort = 8420.ToString() } }
                } as IDictionary<string, EmptyStruct>,
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
