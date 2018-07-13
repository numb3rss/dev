using Microsoft.Azure.Management.ContainerInstance.Fluent;
using Microsoft.Azure.Management.ContainerInstance.Fluent.ContainerGroup.Definition;
using Microsoft.Azure.Management.ContainerInstance.Fluent.Models;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks.ContainerInstanceAccess
{
    public interface IContainerInstanceProvider
    {
        void CreateContainerImage(string taskId);
    }

    public class ContainerInstanceProvider : IContainerInstanceProvider
    {
        private static readonly Region _region = Region.EuropeWest;
        
        private const string ContainerGroupName = "ContainerGroupName";
        private const string ContainerImageName = "ContainerImageName";

        //private Action<string> _createContainerInstance = DefaultCreateContainerImage;
        public void CreateContainerImage(object taskId)
        {
            // Configure some environment variables in the container which the
            // wordcount.py or other script can read to modify its behavior.
            Dictionary<string, string> envVars = new Dictionary<string, string>
            {
                { "TASKID", taskId.ToString() },
                { "DELETEDURATION", "1" }
            };

            IWithFirstContainerInstance withFirstContainerInstance = ContainerInstanceSingleton.Instance.ContainerGroup;
            
            IContainerGroup containerGroup = withFirstContainerInstance
                    .DefineContainerInstance(taskId.ToString())
                        .WithImage(ContainerImageName)
                        .WithExternalTcpPort(80)
                        .WithCpuCoreCount(1.0)
                        .WithMemorySizeInGB(2)
                        .WithEnvironmentVariables(envVars)
                        .Attach()
                        .WithRestartPolicy(ContainerGroupRestartPolicy.Never)
                    .WithDnsPrefix(ContainerGroupName)
                    .Create();
        }

        public void CreateContainerImage(string taskId)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(CreateContainerImage));

            thread.Start(taskId);
        }
    }
}
