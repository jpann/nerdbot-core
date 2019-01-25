namespace NerdBotCommon.Messengers.Factory
{
    public interface IMessengerFactory
    {
        IMessenger Create();
        IMessenger Create(string name);
    }
}