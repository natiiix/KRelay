namespace Lib_K_Relay.GameData.DataStructures
{
    public interface IDataStructure<IDType>
    {
        string Name { get; }
        IDType ID { get; }
    }
}
