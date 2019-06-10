using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arragro.Core.Docker
{
    public static class RedisCommander
    {
        public static async Task<ContainerListResponse> StartRedisCommander(DockerClient client)
        {
            const string ContainerName = "redis-commander-integration-tests";
            const string ImageName = "rediscommander/redis-commander";
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
                    { "8081/tcp", new List<PortBinding> { new PortBinding { HostIP = "127.0.0.1", HostPort = "5082" } } }
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
                    "REDIS_HOSTS=local:localhost:6379"
                }
            });

            var container = await DockerExtentions.GetContainerAsync(client, ContainerName);
            if (container == null)
                throw new Exception("No Redis Commander container.");

            if (container.State != "running")
            {
                var started = await client.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());
                if (!started)
                    throw new Exception("Cannot start the Redis Commander docker container.");
            }

            return container;
        }
    }
}
