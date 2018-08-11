namespace Lib_K_Relay.Interface
{
    public interface IPlugin
    {
        string GetAuthor();

        string GetName();

        string GetDescription();

        string[] GetCommands();

        void Initialize(Proxy proxy);
    }
}
