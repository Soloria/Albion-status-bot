namespace ASB.Bot.Commands
{
    using System.Threading.Tasks;
    using Telegram.Bot.Types;

    public interface ITgEventExecuter
    {
        /// <summary>
        /// Execute command statament
        /// </summary>
        /// <param name="metadata">
        /// Telegram metadata update event
        /// </param>
        /// @awaitable
        Task ExecuteAsync(Update metadata);
    }
}