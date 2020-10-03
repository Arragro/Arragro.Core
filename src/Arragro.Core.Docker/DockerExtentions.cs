using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.Docker
{
    public static class DockerExtentions
    {
        public static ConcurrentBag<DockerContainerResult> DockerContainerResults = new ConcurrentBag<DockerContainerResult>();

        public static async Task<(string stdout, string stderr)> RunCommandInContainerAsync(this DockerClient client, string containerId, string command)
        {
            var commandTokens = command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var createdExec = await client.Exec.ExecCreateContainerAsync(containerId, new ContainerExecCreateParameters
            {
                AttachStderr = true,
                AttachStdout = true,
                Cmd = commandTokens
            });

            var multiplexedStream = await client.Exec.StartAndAttachContainerExecAsync(createdExec.ID, false);

            return await multiplexedStream.ReadOutputToEndAsync(CancellationToken.None);
        }

        public static async Task EnsureImageExistsAsync(DockerClient client, string imageName, string imageTag)
        {
            var images = await client.Images.ListImagesAsync(new ImagesListParameters { All = true, MatchName = imageName });
            if (!images.Any(x => x.RepoTags.Any(y => y == $"{imageName}:{imageTag}")))
            {
                // Download image
                await client.Images.CreateImageAsync(new ImagesCreateParameters() { FromImage = imageName, Tag = imageTag }, new AuthConfig(), new Progress<JSONMessage>());
            }
        }

        public static async Task EnsureImageExistsAndCleanupAsync(DockerClient client, string imageName, string imageTag, string containerName)
        {
            await DockerExtentions.EnsureImageExistsAsync(client, imageName, imageTag);
            var container = await DockerExtentions.GetContainerAsync(client, containerName);
            if (container != null)
            {
                // await client.Containers.StopContainerAsync(container.ID, new ContainerStopParameters());
                await client.Containers.RemoveContainerAsync(container.ID, new ContainerRemoveParameters { Force = true });
            }
        }

        public static async Task<ContainerListResponse> GetContainerAsync(DockerClient client, string containerName)
        {
            var containers = await client.Containers.ListContainersAsync(new ContainersListParameters() { All = true });
            var container = containers.FirstOrDefault(c => c.Names.Contains("/" + containerName));
            return container;
        }

        private static Uri LocalDockerUri()
        {
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            return isWindows ? new Uri("npipe://./pipe/docker_engine") : new Uri("unix:/var/run/docker.sock");
        }

        public static async Task StartDockerServicesAsync(List<Func<DockerClient, Task<ContainerListResponse>>> actions)
        {
            using (var conf = new DockerClientConfiguration(LocalDockerUri())) // localhost
            using (var client = conf.CreateClient())
            {
                await actions.ForEachAsync(4, async action =>
                {
                    var container = await action(client);
                    var inspectResponse = await client.Containers.InspectContainerAsync(container.ID);
                    var dockerContainerResult = new DockerContainerResult(container, inspectResponse);
                    DockerContainerResults.Add(dockerContainerResult);
                });
            }
        }

        public static async Task RemoveDockerServicesAsync(bool gracefulShutdown = false)
        {
            using (var conf = new DockerClientConfiguration(LocalDockerUri())) // localhost
            using (var client = conf.CreateClient())
            {
                await DockerContainerResults.ForEachAsync(4, async containerResults =>
                {
                    if (gracefulShutdown)
                        await client.Containers.StopContainerAsync(containerResults.ContainerListResponse.ID, new ContainerStopParameters());
                    await client.Containers.RemoveContainerAsync(containerResults.ContainerListResponse.ID, new ContainerRemoveParameters { Force = true });
                });
            }
            DockerContainerResults = new ConcurrentBag<DockerContainerResult>();
        }
    }
}
