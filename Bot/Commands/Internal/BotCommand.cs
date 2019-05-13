namespace ASB.Bot.Commands
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Etc;
    using MoreLinq;
    using NLog;
    using Telegram.Bot.Types;

    public abstract class BotCommand : ICmd, IDisposable, IFactoryInjector, ITgEventExecuter
    {
        // TODO move to DI create
        private readonly ILogger log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Cancellation token source 
        /// </summary>
        private CancellationTokenSource source { get; }
        /// <summary>
        /// Telegram command factory
        /// </summary>
        protected CommandFactory Factory { private set; get; }


        protected BotCommand(params string[] aliases)
        {
            if(!aliases.Any())
                throw new Exception("Command aliases is not allowed empty.");

            Aliases = aliases
                .Pipe(x => 
                    x.If(s => s.StartsWith("/"))
                        .Else(c => throw new Exception($"Command '{c}' is not start with '/'")))
                .ToArray();
            source = new CancellationTokenSource();
            log.Trace($"Success create bot command '{aliases.First()},...'.");
        }

        /// <summary>
        /// Aliases of command
        /// </summary>
        public string[] Aliases { get; protected set; }

        async Task ITgEventExecuter.ExecuteAsync(Update metadata)
        {
            if(Factory is null)
                throw new AccessViolationException($"There was an attempt to create an instance of '{nameof(BotCommand)}' without using the factory.");
            log.Trace($"Command '{Aliases.First()}' start awake stage...");
            await AwakeAsync((Factory as IServiceProviderBridge).GetProvider(), source.Token);
            log.Trace($"Command '{Aliases.First()}' start execute stage...");
            await ExecuteImpAsync(metadata, source.Token);
        }

        /// <summary>
        /// Execute command statament
        /// </summary>
        /// <param name="metadata">
        /// Telegram metadata update event
        /// </param>
        /// <param name="token">
        /// async\await token
        /// </param>
        /// @awaitable
        protected abstract Task ExecuteImpAsync(Update metadata, CancellationToken token);
        /// <summary>
        /// Awake start before call <see cref="ExecuteImpAsync"/>
        /// </summary>
        protected abstract Task AwakeAsync(IServiceProvider provider, CancellationToken token);
        /// <summary>
        /// Complete event after call <see cref="ExecuteImpAsync"/>
        /// </summary>
        protected virtual Task CompleteAsync(CancellationToken token) => Task.CompletedTask;

        /// <summary>
        /// Dispose this command
        /// </summary>
        void IDisposable.Dispose()
        {
            log.Trace($"Command '{Aliases.First()}' start complete stage...");
            CompleteAsync(source.Token).Wait(source.Token);
            Aliases = Array.Empty<string>();
            source.Cancel();
            source.Dispose();
        }
        /// <summary>
        /// Inject factory to this structure
        /// </summary>
        void IFactoryInjector.Inject(CommandFactory factory) => Factory = factory;
    }
}