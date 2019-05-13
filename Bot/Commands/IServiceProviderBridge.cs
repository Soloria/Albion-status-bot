namespace ASB.Bot.Commands
{
    using System;

    public interface IServiceProviderBridge
    {
        IServiceProvider GetProvider();
    }
}