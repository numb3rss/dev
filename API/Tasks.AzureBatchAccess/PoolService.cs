using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Common;
using Tasks.BO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks.AzureBatchAccess
{
    public class PoolService
    {
        private const int PoolTargetNodeCount = 1;

        private const string PoolNodeVirtualMachineSize = "small";

        private const string PoolOsFamily = "4";

        /// <summary>
        /// Creates a <see cref="CloudPool"/> associated with the specified Batch account. If an existing pool with the
        /// specified ID is found, the pool is resized to match the specified node count.
        /// </summary>
        /// <param name="batchClient">A fully initialized <see cref="BatchClient"/>.</param>
        /// <param name="poolId">The ID of the <see cref="CloudPool"/>.</param>
        /// <param name="nodeSize">The size of the nodes within the pool.</param>
        /// <param name="nodeCount">The number of nodes to create within the pool.</param>
        /// <param name="maxTasksPerNode">The maximum number of tasks to run concurrently on each node.</param>
        /// <returns>A bound <see cref="CloudPool"/> with the specified properties.</returns>
        public async Task<CloudPool> CreatePoolIfNotExistAsync(BatchClient batchClient, string poolId, string nodeSize = null, int nodeCount = 0, int maxTasksPerNode = 0)
        {
            try
            {
                // Provide a reference to an Azure Marketplace image for
                // "Windows Server 2016 Datacenter with Containers"
                ImageReference imageReference = new ImageReference(
                  publisher: "MicrosoftWindowsServer",
                  offer: "WindowsServer",
                  sku: "2016-Datacenter-with-Containers",
                  version: "latest");

                ContainerConfiguration containerConfig = new ContainerConfiguration
                {
                    ContainerImageNames = new List<string> { "ContainerImage" }
                };

                // VM configuration
                VirtualMachineConfiguration virtualMachineConfiguration = new VirtualMachineConfiguration(
                    imageReference: imageReference,
                    nodeAgentSkuId: "batch.node.windows amd64");

                virtualMachineConfiguration.ContainerConfiguration = containerConfig;
                
                // You can learn more about os families and versions at:
                // https://azure.microsoft.com/en-us/documentation/articles/cloud-services-guestos-update-matrix/
                CloudPool pool = batchClient.PoolOperations.CreatePool();
                pool.Id = poolId;
                pool.TargetDedicatedComputeNodes = 1;
                pool.VirtualMachineSize = "Standard_A1";
                pool.VirtualMachineConfiguration = virtualMachineConfiguration;
                pool.StartTask = new StartTask
                {
                    CommandLine = "cmd /C echo start task"
                };

                await this.CreatePoolIfNotExistAsync(batchClient, pool).ConfigureAwait(continueOnCapturedContext: false);
            }
            catch (Exception ex)
            {

            }

            return await batchClient.PoolOperations.GetPoolAsync(poolId).ConfigureAwait(continueOnCapturedContext: false);
        }

        /// <summary>
        /// Creates a pool if it doesn't already exist.  If the pool already exists, this method resizes it to meet the expected
        /// targets specified in settings.
        /// </summary>
        /// <param name="batchClient">The BatchClient to create the pool with.</param>
        /// <param name="pool">The pool to create.</param>
        /// <returns>An asynchronous <see cref="Task"/> representing the operation.</returns>
        private async Task<CreatePoolResult> CreatePoolIfNotExistAsync(BatchClient batchClient, CloudPool pool)
        {
            bool successfullyCreatedPool = false;

            int targetDedicatedNodeCount = pool.TargetDedicatedComputeNodes ?? 0;
            int targetLowPriorityNodeCount = pool.TargetLowPriorityComputeNodes ?? 0;
            string poolNodeVirtualMachineSize = pool.VirtualMachineSize;
            string poolId = pool.Id;

            // Attempt to create the pool
            try
            {
                // Create an in-memory representation of the Batch pool which we would like to create.  We are free to modify/update 
                // this pool object in memory until we commit it to the service via the CommitAsync method.
                Console.WriteLine("Attempting to create pool: {0}", pool.Id);

                // Create the pool on the Batch Service
                await pool.CommitAsync().ConfigureAwait(continueOnCapturedContext: false);

                successfullyCreatedPool = true;
                Console.WriteLine("Created pool {0} with {1} dedicated and {2} low priority {3} nodes",
                    poolId,
                    targetDedicatedNodeCount,
                    targetLowPriorityNodeCount,
                    poolNodeVirtualMachineSize);
            }
            catch (BatchException e)
            {
                // Swallow the specific error code PoolExists since that is expected if the pool already exists
                if (e.RequestInformation?.BatchError?.Code == BatchErrorCodeStrings.PoolExists)
                {
                    // The pool already existed when we tried to create it
                    successfullyCreatedPool = false;
                    Console.WriteLine("The pool already existed when we tried to create it");
                }
                else
                {
                    throw; // Any other exception is unexpected
                }
            }

            // If the pool already existed, make sure that its targets are correct
            if (!successfullyCreatedPool)
            {
                CloudPool existingPool = await batchClient.PoolOperations.GetPoolAsync(poolId).ConfigureAwait(continueOnCapturedContext: false);

                // If the pool doesn't have the right number of nodes, isn't resizing, and doesn't have
                // automatic scaling enabled, then we need to ask it to resize
                if ((existingPool.CurrentDedicatedComputeNodes != targetDedicatedNodeCount || existingPool.CurrentLowPriorityComputeNodes != targetLowPriorityNodeCount) &&
                    existingPool.AllocationState != AllocationState.Resizing &&
                    existingPool.AutoScaleEnabled == false)
                {
                    // Resize the pool to the desired target. Note that provisioning the nodes in the pool may take some time
                    await existingPool.ResizeAsync(targetDedicatedNodeCount, targetLowPriorityNodeCount).ConfigureAwait(continueOnCapturedContext: false);
                    return CreatePoolResult.ResizedExisting;
                }
                else
                {
                    return CreatePoolResult.PoolExisted;
                }
            }

            return CreatePoolResult.CreatedNew;
        }
    }
}
