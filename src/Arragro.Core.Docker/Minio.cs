using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arragro.Core.Docker
{
    public static class Minio
    {
        public static async Task<ContainerListResponse> StartMinio(DockerClient client)
        {
            const string ContainerName = "minio";
            const string ImageName = "minio/minio";
            const string ImageTag = "latest";

            await DockerExtentions.EnsureImageExistsAndCleanupAsync(client, ImageName, ImageTag, ContainerName);

            var config = new Config();

            var hostConfig = new HostConfig
            {
                PublishAllPorts = true,
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    { "9000/tcp", new List<PortBinding> { new PortBinding { HostIP = "", HostPort = "9000" } } }
                }
            };

            await client.Containers.CreateContainerAsync(new CreateContainerParameters(config)
            {
                Image = ImageName + ":" + ImageTag,
                Name = ContainerName,
                ExposedPorts = new Dictionary<string, object>() {
                    { "9000/tcp", new { HostPort = 9000.ToString() } }
                } as IDictionary<string, EmptyStruct>,
                HostConfig = hostConfig,
                Env = new List<string>
                {      
                    "MINIO_REGION=us-east-1",
                    "MINIO_ACCESS_KEY=minio_access_key",
                    "MINIO_SECRET_KEY=minio_secret_key"
                },
                Cmd = new List<string>
                {
                    "server",
                    "/data"
                }
            });

            var container = await DockerExtentions.GetContainerAsync(client, ContainerName);
            if (container == null)
                throw new Exception("No minio container.");

            if (container.State != "running")
            {
                var started = await client.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());
                if (!started)
                    throw new Exception("Cannot start the minio docker container.");
            }

            return container;
        }
    }
}
