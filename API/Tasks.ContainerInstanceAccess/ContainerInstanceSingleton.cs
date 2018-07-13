using Microsoft.Azure.Management.ContainerInstance.Fluent;
using Microsoft.Azure.Management.ContainerInstance.Fluent.ContainerGroup.Definition;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tasks.ContainerInstanceAccess
{
    public sealed class ContainerInstanceSingleton
    {
        private static readonly Region _region = Region.EuropeWest;

        private const string ResourceGroupName = "ResourceGroupName";
        private const string ContainerGroupName = "ContainerGroupName";

        private const string ClientId = "ClientIdServicePrincipal";
        private const string ClientSecret = "ClientSecretServicePrincipal";
        private const string TenantId = "TenantId";

        private static readonly Lazy<ContainerInstanceSingleton> lazy =
            new Lazy<ContainerInstanceSingleton>(() => new ContainerInstanceSingleton());

        public IWithFirstContainerInstance ContainerGroup { get; private set; }

        public static ContainerInstanceSingleton Instance { get { return lazy.Value; } }

        private ContainerInstanceSingleton()
        {
            //=================================================================
            // Authenticate
            //AzureCredentials credentials = SdkContext.AzureCredentialsFactory.FromFile(@"~/azureauth.txt");
            var credentials = SdkContext.AzureCredentialsFactory
                .FromServicePrincipal(ClientId,
                ClientSecret,
                TenantId,
                AzureEnvironment.AzureGlobalCloud);

            var azure = Azure
                .Configure()
                .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                .Authenticate(credentials)
                .WithDefaultSubscription();

            ContainerGroup = azure.ContainerGroups.Define(ContainerGroupName)
                    .WithRegion(_region)
                    .WithExistingResourceGroup(ResourceGroupName)
                    .WithWindows()
                    .WithPublicImageRegistryOnly()
                    .WithoutVolume();
        }
    }
}
