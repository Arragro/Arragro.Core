using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arragro.Core.Docker
{
    public static class ImageService
    {
        public static async Task<ContainerListResponse> StartImageService(DockerClient client)
        {
            const string ContainerName = "imageservice";
            const string ImageName = "docker.arragro.com/imageservice";
            const string ImageTag = "v0.0.8-alpha";

            await DockerExtentions.EnsureImageExistsAndCleanupAsync(client, ImageName, ImageTag, ContainerName);

            var config = new Config();

            var hostConfig = new HostConfig
            {
                PublishAllPorts = true,
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    { "3000/tcp", new List<PortBinding> { new PortBinding { HostIP = "", HostPort = "3000" } } }
                }
            };

            await client.Containers.CreateContainerAsync(new CreateContainerParameters(config)
            {
                Image = ImageName + ":" + ImageTag,
                Name = ContainerName,
                ExposedPorts = new Dictionary<string, object>() {
                    { "3000/tcp", new { HostPort = 3000.ToString() } }
                } as IDictionary<string, EmptyStruct>,
                HostConfig = hostConfig
            });

            var container = await DockerExtentions.GetContainerAsync(client, ContainerName);
            if (container == null)
                throw new Exception("No imageservice container.");

            if (container.State != "running")
            {
                var started = await client.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());
                if (!started)
                    throw new Exception("Cannot start the imageservice docker container.");
            }

            return container;
        }
    }
}
