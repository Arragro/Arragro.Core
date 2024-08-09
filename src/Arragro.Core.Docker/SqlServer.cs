using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.Docker
{
    public static class SqlServer
    {
        public static async Task<ContainerListResponse> StartSqlServer(DockerClient client, string imageTag = "2022-latest")
        {
            const string ContainerName = "sqlserver-integration-tests";
            const string ImageName = "mcr.microsoft.com/mssql/server";

            await DockerExtentions.EnsureImageExistsAndCleanupAsync(client, ImageName, imageTag, ContainerName);

            var config = new Config();

            var hostConfig = new HostConfig
            {
                PortBindings = 
                    new Dictionary<string, IList<PortBinding>>
                    {
                        { "1433/tcp", new List<PortBinding> { new PortBinding { HostIP = "127.0.0.1", HostPort = "1435" } } }
                    }
            };

            await client.Containers.CreateContainerAsync(new CreateContainerParameters(config)
            {
                Image = ImageName + ":" + imageTag,
                Name = ContainerName,
                Tty = false,
                HostConfig = hostConfig,
                Env = new List<string>
                {
                    "ACCEPT_EULA=Y",
                    "SA_PASSWORD=P@ssword123"
                }
            });

            var container = await DockerExtentions.GetContainerAsync(client, ContainerName);
            if (container == null)
                throw new Exception("No SqlServer container.");

            if (container.State != "running")
            {
                var started = await client.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());
                if (!started)
                    throw new Exception("Cannot start the SqlServer docker container.");
            }

            var isContainerReady = false;
            var isReadyCounter = 0;

            Exception exception = null;

            while (!isContainerReady)
            {
                isReadyCounter++;

                try
                {
                    var commandTokens = "/opt/mssql-tools/bin/sqlcmd -U sa -P P@ssword123 -Q".Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    commandTokens.Add("SELECT TOP 1 name FROM master.sys.databases");

                    var createdExec = await client.Exec.ExecCreateContainerAsync(container.ID, new ContainerExecCreateParameters
                    {
                        AttachStderr = true,
                        AttachStdout = true,
                        Cmd = commandTokens
                    });

                    var multiplexedStream = await client.Exec.StartAndAttachContainerExecAsync(createdExec.ID, false);

                    var result = await multiplexedStream.ReadOutputToEndAsync(CancellationToken.None);

                    if (string.IsNullOrEmpty(result.stderr))
                    {
                        isContainerReady = true;
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                if (isReadyCounter == 10)
                    throw new Exception("SqlServer container never ready.", exception);

                if (!isContainerReady)
                    Thread.Sleep(2000);
            }

            return container;
        }
    }
}
