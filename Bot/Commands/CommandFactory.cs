namespace ASB.Bot.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public class CommandFactory
    {
        /// <summary>
        /// DI Container
        /// </summary>
        private readonly IServiceProvider _storage;

        private readonly Dictionary<(Guid uid, string[] aliases), Type> storage = new Dictionary<(Guid uid, string[] aliases), Type>();



        public CommandFactory(IServiceProvider storage)
        {
            _storage = storage;
            FindReflection();
        }




        protected void FindReflection()
        {
            var cmds = AppDomain.CurrentDomain
                .GetAssemblies()
                // Select ExportedTypes and merge in signle collection
                .SelectMany(x => x.ExportedTypes)
                // if assigned ICmd
                .Where(x => x.IsAssignableFrom(typeof(ICmd)))
                // if assigned IFactoryInjector
                .Where(x => x.IsAssignableFrom(typeof(IFactoryInjector)))
                // if assigned ITgEventExecuter
                .Where(x => x.IsAssignableFrom(typeof(ITgEventExecuter)))
                // if empty .cctor
                .Where(x => x.GetConstructors().Any(z => !z.GetParameters().Any()));

            foreach (var type in cmds)
            {
                var instance = Activator.CreateInstance(type);
                if (instance is ICmd cmd && instance is IFactoryInjector injector)
                {
                    storage.Add((Guid.NewGuid(), cmd.Aliases), type);
                    injector.Inject(this);
                }
                if(instance is IDisposable disposable)
                    disposable.Dispose();
            }
        }

        public T Find<T>(string cmd)
        {
            Expression<Func<KeyValuePair<(Guid uid, string[] array), Type>, bool>>
                raw_exp = x => x.Key.array.Contains(cmd);
            var exp = raw_exp.Compile();

            if (storage.Any(exp))
            {
                var instance = Activator.CreateInstance(storage.First(exp).Value);
                if (instance is T result)
                    return result;
            }
            return default;
        }
    }
}