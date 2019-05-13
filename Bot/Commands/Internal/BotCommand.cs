namespace ASB.Bot.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Telegram.Bot.Types;

    public abstract class BotCommand : ICmd, IDisposable, IFactoryInjector, ITgEventExecuter
    {
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


            Aliases = aliases;
            source = new CancellationTokenSource();
        }

        /// <summary>
        /// Aliases of command
        /// </summary>
        public string[] Aliases { get; protected set; }

        async Task ITgEventExecuter.ExecuteAsync(Update metadata)
        {
            if(Factory is null)
                throw new AccessViolationException($"There was an attempt to create an instance of '{nameof(BotCommand)}' without using the factory.");
            await ExecuteImpAsync(metadata, source.Token);
        }
        /// <summary>
        /// Execute command statament
        /// </summary>
        /// <param name="metadata">
        /// Telegram metadata update event
        /// </param>
        /// @awaitable
        protected abstract Task ExecuteImpAsync(Update metadata, CancellationToken token);

        /// <summary>
        /// Dispose this command
        /// </summary>
        void IDisposable.Dispose()
        {
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