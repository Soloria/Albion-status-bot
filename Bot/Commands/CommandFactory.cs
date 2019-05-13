namespace ASB.Bot.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public class CommandFactory : IServiceProviderBridge
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
                // ignore dynamic assembly
                .Where(x => !x.IsDynamic)
                // Select ExportedTypes and merge in signle collection
                .SelectMany(x => x.ExportedTypes)
                // if assigned ICmd
                .Where(x => typeof(ICmd).IsAssignableFrom(x))
                // if assigned IFactoryInjector
                .Where(x => typeof(IFactoryInjector).IsAssignableFrom(x))
                // if assigned ITgEventExecuter
                .Where(x => typeof(ITgEventExecuter).IsAssignableFrom(x))
                // ignore abstract and interface types
                .Where(x => !x.IsInterface && !x.IsAbstract)
                // if empty .cctor
                .Where(x => x.GetConstructors().Any(z => !z.GetParameters().Any()));

            foreach (var type in cmds)
            {
                var instance = Activator.CreateInstance(type);
                if (instance is ICmd cmd )
                    storage.Add((Guid.NewGuid(), cmd.Aliases), type);
                if (instance is IDisposable disposable)
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
                if (instance is T result && instance is IFactoryInjector injector)
                {
                    injector.Inject(this);
                    return result;
                }
            }
            return default;
        }

        IServiceProvider IServiceProviderBridge.GetProvider() => this._storage;
    }
}