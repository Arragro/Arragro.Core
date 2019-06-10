using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arragro.Core.Docker
{
    public static class Smtp4Dev
    {
        public static async Task<ContainerListResponse> StartSmtp4Dev(DockerClient client)
        {
            const string ContainerName = "smtp4dev-integration-tests";
            const string ImageName = "rnwood/smtp4dev";
            const string ImageTag = "linux-amd64-3.1.0-ci0552";

            await DockerExtentions.EnsureImageExistsAndCleanupAsync(client, ImageName, ImageTag, ContainerName);

            var config = new Config()
            {
                Hostname = "localhost"
            };

            var hostConfig = new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    { "80/tcp", new List<PortBinding> { new PortBinding { HostIP = "127.0.0.1", HostPort = "5080" } } },
                    { "25/tcp", new List<PortBinding> { new PortBinding { HostIP = "127.0.0.1", HostPort = "25" } } }
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
                throw new Exception("No Smtp4Dev container.");

            if (container.State != "running")
            {
                var started = await client.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());
                if (!started)
                    throw new Exception("Cannot start the Smtp4Dev docker container.");
            }

            return container;
        }
    }
}
