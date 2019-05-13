namespace ASB.Bot.Commands
{
    using System.Threading.Tasks;
    using Telegram.Bot.Types;

    public interface ICmd
    {
        string[] Aliases { get; }
    }
}