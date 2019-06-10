﻿using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.Docker
{
    public static class DockerExtentions
    {
        public static List<DockerContainerResult> DockerContainerResults = new List<DockerContainerResult>();

        public static async Task<(string stdout, string stderr)> RunCommandInContainerAsync(this IContainerOperations source, string containerId, string command)
        {
            var commandTokens = command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var createdExec = await source.ExecCreateContainerAsync(containerId, new ContainerExecCreateParameters
            {
                AttachStderr = true,
                AttachStdout = true,
                Cmd = commandTokens
            });

            var multiplexedStream = await source.StartAndAttachContainerExecAsync(createdExec.ID, false);

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
                await client.Containers.StopContainerAsync(container.ID, new ContainerStopParameters());
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
                foreach (var action in actions)
                {
                    var container = await action(client);
                    var inspectResponse = await client.Containers.InspectContainerAsync(container.ID);
                    var dockerContainerResult = new DockerContainerResult(container, inspectResponse);
                    DockerContainerResults.Add(dockerContainerResult);
                }
            }
        }

        public static async Task RemoveDockerServicesAsync()
        {
            using (var conf = new DockerClientConfiguration(LocalDockerUri())) // localhost
            using (var client = conf.CreateClient())
            {
                foreach (var containerResults in DockerContainerResults)
                {
                    await client.Containers.StopContainerAsync(containerResults.ContainerListResponse.ID, new ContainerStopParameters());
                    await client.Containers.RemoveContainerAsync(containerResults.ContainerListResponse.ID, new ContainerRemoveParameters { Force = true });
                }
            }
            DockerContainerResults.Clear();
        }
    }
}
