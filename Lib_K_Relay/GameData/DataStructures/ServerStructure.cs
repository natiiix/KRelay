using System.Collections.Generic;
using System.Xml.Linq;

namespace Lib_K_Relay.GameData.DataStructures
{
    public struct ServerStructure : IDataStructure<string>
    {
        internal static Dictionary<string, ServerStructure> Load(XDocument doc)
        {
            Dictionary<string, ServerStructure> map = new Dictionary<string, ServerStructure>();

            doc.Element("Chars")
                .Element("Servers")
                .Elements("Server")
                .ForEach(server =>
                {
                    ServerStructure s = new ServerStructure(server);
                    map[s.ID] = s;
                });

            return map;
        }

        public static readonly Dictionary<string, string> abbreviations = new Dictionary<string, string>
        {
            { "AsiaEast", "AE" },
            { "AsiaSouthEast", "ASE" },
            { "Australia", "AUS" },
            { "EUEast", "EUE" },
            { "EUNorth2", "EUN2" },
            { "EUNorth", "EUN" },
            { "EUSouthWest", "EUSW" },
            { "EUSouth", "EUS" },
            { "EUWest2", "EUW2" },
            { "EUWest", "EUW" },
            { "USEast2", "USE2" },
            { "USEast3", "USE3" },
            { "USEast", "USE" },
            { "USMidWest2", "USMW2" },
            { "USMidWest", "USMW" },
            { "USNorthWest", "USNW" },
            { "USSouth2", "USS2" },
            { "USSouth3", "USS3" },
            { "USSouthWest", "USSW" },
            { "USSouth", "USS" },
            { "USWest2", "USW2" },
            { "USWest3", "USW3" },
            { "USWest", "USW" }
        };

        /// <summary>
        /// The complete name of this server
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// The abbreviation of this server
        /// </summary>
        public string Abbreviation;

        public string ID
        {
            get
            {
                return Abbreviation;
            }
        }

        /// <summary>
        /// The IP address of this server
        /// </summary>
        public string Address;

        public ServerStructure(XElement server)
        {
            Name = server.ElemDefault("Name", "");
            Abbreviation = abbreviations.ContainsKey(Name) ? abbreviations[Name] : "";
            Address = /*Dns.GetHostEntry(*/server.ElemDefault("DNS", "")/*).AddressList[0].ToString()*/;
        }

        public override string ToString()
        {
            return string.Format("Server: {0}/{1} ({2})", Name, Abbreviation, Address);
        }
    }
}
