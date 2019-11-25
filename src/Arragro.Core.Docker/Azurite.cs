using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arragro.Core.Docker
{
    public static class Azurite
    {
        public static async Task<ContainerListResponse> StartAzurite(DockerClient client)
        {
            const string ContainerName = "azurite";
            const string ImageName = "touchify/azurite";
            const string ImageTag = "2.7.1";

            await DockerExtentions.EnsureImageExistsAndCleanupAsync(client, ImageName, ImageTag, ContainerName);

            var config = new Config();

            var hostConfig = new HostConfig
            {
                PublishAllPorts = true,
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    { "10000/tcp", new List<PortBinding> { new PortBinding { HostIP = "", HostPort = "10000" } } },
                    { "10001/tcp", new List<PortBinding> { new PortBinding { HostIP = "", HostPort = "10001" } } },
                    { "10002/tcp", new List<PortBinding> { new PortBinding { HostIP = "", HostPort = "10002" } } }
                }
            };

            await client.Containers.CreateContainerAsync(new CreateContainerParameters(config)
            {
                Image = ImageName + ":" + ImageTag,
                Name = ContainerName,
                ExposedPorts = new Dictionary<string, object>() {
                    { "10000/tcp", new { HostPort = 10000.ToString() } },
                    { "10001/tcp", new { HostPort = 10001.ToString() } },
                    { "10002/tcp", new { HostPort = 10002.ToString() } }
                } as IDictionary<string, EmptyStruct>,
                HostConfig = hostConfig
            });

            var container = await DockerExtentions.GetContainerAsync(client, ContainerName);
            if (container == null)
                throw new Exception("No azurite container.");

            if (container.State != "running")
            {
                var started = await client.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());
                if (!started)
                    throw new Exception("Cannot start the azurite docker container.");
            }

            return container;
        }
    }
}
