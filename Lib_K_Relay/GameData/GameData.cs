using Lib_K_Relay.GameData.DataStructures;
using Lib_K_Relay.Properties;
using Lib_K_Relay.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

namespace Lib_K_Relay.GameData
{
    /// <summary>
    /// Represents a mapping of short identifiers to data structures for a given data type
    /// </summary>
    /// <typeparam name="IDType">The type of the short identifier (e.g. byte, ushort, string)</typeparam>
    /// <typeparam name="DataType">The type of the data structure (e.g. PacketStructure, EnemyStructure, ServerStructure)</typeparam>
    public class GameDataMap<IDType, DataType> where DataType : IDataStructure<IDType>
    {
        /// <summary>
        /// Map of short id -> data structure
        /// </summary>
        public Dictionary<IDType, DataType> Map
        {
            get;
            private set;
        }

        public GameDataMap(Dictionary<IDType, DataType> map)
        {
            Map = map;
        }

        /// <summary>
        /// Selects a data structure from this map by short identifier
        /// </summary>
        /// <param name="id">The short identifier</param>
        /// <returns>The data structure</returns>
        /// <example>GameData.Packets.ByID(255) -> Packet: UNKNOWN (255)</example>
        /// <example>GameData.Servers.ByID("USW") -> Server: USWest/USW</example>
        public DataType ByID(IDType id)
        {
            return Map[id];
        }

        /// <summary>
        /// Selects a data structure from this map by full identifier (strings only)
        /// </summary>
        /// <param name="name">The string identifier</param>
        /// <returns>The data structure</returns>
        /// <example>GameData.Packets.ByName("UNKNOWN") -> Packet: UNKNOWN(255)</example>
        /// <example>GameData.Servers.ByName("USWest") -> Server: USWest/USW</example>
        public DataType ByName(string name)
        {
            return Map.First(e => e.Value.Name == name).Value;
        }

        /// <summary>
        /// Selects the first value from this map for which the given function returns true.
        /// </summary>
        /// <param name="f">The expression to evaluate</param>
        /// <returns>The data structure</returns>
        /// <example>GameData.Packets.Match(p => p.Type == typeof(NewTickPacket)) -> NEWTICK (47)</example>
        public DataType Match(Func<DataType, bool> f)
        {
            return Map.First(e => f(e.Value)).Value;
        }
    }

    public static class GameData
    {
        private const string OBJECTS_FILE = "Objects.xml";
        private const string TILES_FILE = "Tiles.xml";
        private const string PACKETS_FILE = "Packets.xml";

        // Cache the XMLs because Resource accessors are slow

        public static string RawObjectsXML { get; } = Resources.Objects;
        public static string RawTilesXML { get; } = Resources.Tiles;
        public static string RawPacketsXML { get; } = Resources.Packets;

        /// <summary>
        /// Maps item data ("type" attribute -> item structure)
        /// </summary>
        public static GameDataMap<ushort, ItemStructure> Items;

        /// <summary>
        /// Maps tile data ("type" attribute -> tile structure)
        /// </summary>
        public static GameDataMap<ushort, TileStructure> Tiles;

        /// <summary>
        /// Maps object data ("type" attribute -> object structure)
        /// </summary>
        public static GameDataMap<ushort, ObjectStructure> Objects;

        /// <summary>
        /// Maps packet data (PacketID -> packet structure)
        /// </summary>
        public static GameDataMap<byte, PacketStructure> Packets;

        /// <summary>
        /// Maps server data (Abbreviation -> server structure) (e.g. USW -> USWest)
        /// </summary>
        public static GameDataMap<string, ServerStructure> Servers;

        public static void Load()
        {
            Parallel.Invoke(
            // Items
            () =>
            {
                try
                {
                    Items = new GameDataMap<ushort, ItemStructure>(ItemStructure.Load(XDocument.Load(OBJECTS_FILE)));
                    GameDataLog($"Loaded items from file \"{OBJECTS_FILE}\".");
                }
                catch
                {
                    Items = new GameDataMap<ushort, ItemStructure>(ItemStructure.Load(XDocument.Parse(RawObjectsXML)));
                    GameDataLog("Loaded items from \"Resources.Objects\".");
                }

                GameDataLog("Mapped {0} items.", Items.Map.Count);
            },
            // Tiles
            () =>
            {
                try
                {
                    Tiles = new GameDataMap<ushort, TileStructure>(TileStructure.Load(XDocument.Load(TILES_FILE)));
                    GameDataLog($"Loaded tiles from file \"{TILES_FILE}\".");
                }
                catch
                {
                    Tiles = new GameDataMap<ushort, TileStructure>(TileStructure.Load(XDocument.Parse(RawTilesXML)));
                    GameDataLog("Loaded tiles from \"Resources.Tiles\".");
                }

                GameDataLog("Mapped {0} tiles.", Tiles.Map.Count);
            },
            // Objects
            () =>
            {
                try
                {
                    Objects = new GameDataMap<ushort, ObjectStructure>(ObjectStructure.Load(XDocument.Load(OBJECTS_FILE)));
                    GameDataLog($"Loaded objects from file \"{OBJECTS_FILE}\".");
                }
                catch
                {
                    Objects = new GameDataMap<ushort, ObjectStructure>(ObjectStructure.Load(XDocument.Parse(RawObjectsXML)));
                    GameDataLog("Loaded objects from \"Resources.Objects\".");
                }

                GameDataLog("Mapped {0} objects.", Objects.Map.Count);
            },
            // Packets
            () =>
            {
                try
                {
                    Packets = new GameDataMap<byte, PacketStructure>(PacketStructure.Load(XDocument.Load(PACKETS_FILE)));
                    GameDataLog($"Loaded packets from file \"{PACKETS_FILE}\".");
                }
                catch
                {
                    Packets = new GameDataMap<byte, PacketStructure>(PacketStructure.Load(XDocument.Parse(RawPacketsXML)));
                    GameDataLog("Loaded packets from \"Resources.Packets\".");
                }

                GameDataLog("Mapped {0} packets.", Packets.Map.Count);
            },
            // char/list
            () =>
            {
                const string CHAR_LIST_FILE = "char_list.xml";

                XDocument charList = null;

                try
                {
                    charList = XDocument.Load("http://realmofthemadgodhrd.appspot.com/char/list");
                }
                catch (Exception)
                {
                }

                // If the char list doesn't contain an error
                if (charList != null && charList.Element("Error") == null)
                {
                    // Make a backup of the char list
                    charList.Save(CHAR_LIST_FILE);
                }
                // If the backup char list file exists
                else if (File.Exists(CHAR_LIST_FILE))
                {
                    charList = XDocument.Load(CHAR_LIST_FILE);
                }
                // The retrieved char list contains an error and a backup char list doesn't exist
                else
                {
                    GameDataLog("Error! Unable to retrieve server list.");
                    return;
                }

                Servers = new GameDataMap<string, ServerStructure>(ServerStructure.Load(charList));
                GameDataLog("Mapped {0} servers.", Servers.Map.Count);
            });

            GameDataLog("Successfully loaded game data.");
        }

        private static void GameDataLog(string message, params object[] list) => PluginUtils.Log("GameData", message, list);
    }
}
