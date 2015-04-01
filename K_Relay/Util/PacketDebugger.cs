﻿using Lib_K_Relay;
using Lib_K_Relay.Interface;
using Lib_K_Relay.Networking;
using Lib_K_Relay.Networking.Packets;
using Lib_K_Relay.Networking.Packets.Server;
using Lib_K_Relay.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K_Relay.Util
{
    class PacketDebugger : IPlugin
    {
        private List<PacketType> _reportId = new List<PacketType>()
        {
        };

        private List<PacketType> _printString = new List<PacketType>()
        {
        };

        public string GetAuthor()
        { return "KrazyShank / Kronks"; }

        public string GetName()
        { return "Packet Debugger"; }

        public string GetDescription()
        { return "Helps track what packets have been received."; }

        public void Initialize(Proxy proxy)
        {
            proxy.ClientPacketRecieved += OnPacket;
            proxy.ServerPacketRecieved += OnPacket;
            proxy.HookPacket(PacketType.UPDATE, OnUpdatePacket);
        }

        private void OnPacket(ClientInstance client, Packet packet)
        {
            if (_reportId.Contains(packet.Type)) Console.WriteLine("[Packet Debugger] Received {0} packet.", packet.Type);
            if (_printString.Contains(packet.Type)) Console.WriteLine("[Packet Debugger] {0}", packet);
        }

        private void OnUpdatePacket(ClientInstance client, Packet packet)
        {
            UpdatePacket update = (UpdatePacket)packet;
            
            for (int i = 0; i < update.Tiles.Length; i++)
            {
                update.Tiles[i].Type = Serializer.Tiles["SpiderDirt"];
            }
        }
    }
}