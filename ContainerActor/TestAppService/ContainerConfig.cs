using Autofac;
using TestAppService.BP;

namespace TestAppService
{
    public static class ContainerConfig
    {
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<Application>().As<IApplication>();

            builder.RegisterType<MessageService>().As<IMessageService>();
            builder.RegisterType<StorageQueueService>().As<IStorageQueueService>();

            return builder.Build();
        }
    }
}
