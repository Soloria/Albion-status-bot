namespace ASB.Bot.Commands
{
    public interface IFactoryInjector
    {
        void Inject(CommandFactory factory);
    }
}