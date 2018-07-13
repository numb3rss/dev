using Autofac;
using System;

namespace TestAppService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(args[0]);
            Console.WriteLine(args.Length);

            if (args.Length == 0)
            {
                throw new ArgumentException(nameof(args));
            }

            Guid guid;
            if (!Guid.TryParse(args[0], out guid))
            {
                throw new FormatException("argument has to be a guid");
            }

            var container = ContainerConfig.Configure();
            using (var scope = container.BeginLifetimeScope())
            {
                var app = scope.Resolve<IApplication>();
                app.Run(args[0], int.Parse(args[1]));
            }
        }
    }
}
