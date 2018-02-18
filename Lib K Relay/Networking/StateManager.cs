using Lib_K_Relay.Networking.Packets.Client;
using Lib_K_Relay.Networking.Packets.DataObjects;
using Lib_K_Relay.Networking.Packets.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib_K_Relay.Networking
{
    public class StateManager
    {
        private Proxy _proxy;

        public void Attach(Proxy proxy)
        {
            _proxy = proxy;
            proxy.HookPacket<CreateSuccessPacket>(OnCreateSuccess);
            proxy.HookPacket<MapInfoPacket>(OnMapInfo);
            proxy.HookPacket<UpdatePacket>(OnUpdate);
            proxy.HookPacket<NewTickPacket>(OnNewTick);
            proxy.HookPacket<PlayerShootPacket>(OnPlayerShoot);
            proxy.HookPacket<MovePacket>(OnMove);
        }

        private void OnMove(Client client, MovePacket packet)
        {
            client.PreviousTime = packet.Time;
            client.LastUpdate = Environment.TickCount;
            client.PlayerData.Pos = packet.NewPosition;
        }

        private void OnPlayerShoot(Client client, PlayerShootPacket packet)
        {
            client.PlayerData.Pos = new Location()
            {
                X = packet.Position.X - 0.3f * (float)Math.Cos(packet.Angle),
                Y = packet.Position.Y - 0.3f * (float)Math.Sin(packet.Angle)
            };
        }

        private void OnNewTick(Client client, NewTickPacket packet)
        {
            client.PlayerData.Parse(packet);
        }

        private void OnMapInfo(Client client, MapInfoPacket packet)
        {
            client.State["MapInfo"] = packet;
        }

        private void OnCreateSuccess(Client client, CreateSuccessPacket packet)
        {
            client.PlayerData = new PlayerData(packet.ObjectId, client.State.Value<MapInfoPacket>("MapInfo"));
        }

        private void OnUpdate(Client client, UpdatePacket packet)
        {
            client.PlayerData.Parse(packet);
            if (client.State.ACCID != null)
            {
                return;
            }

            State resolvedState = null;
            State randomRealmState = null;

            foreach (State cstate in _proxy.States.Values.ToList())
            {
                if (cstate.ACCID == client.PlayerData.AccountId)
                {
                    resolvedState = cstate;
                    randomRealmState = _proxy.States.Values.FirstOrDefault(x => x.LastHello != null && x.LastHello.GameId == -3);

                    if (randomRealmState != null)
                    {
                        resolvedState.ConTargetAddress = randomRealmState.LastRealm.Host;
                        resolvedState.LastRealm = randomRealmState.LastRealm;
                        _proxy.States.Remove(randomRealmState.GUID);
                    }
                    else if (resolvedState.LastHello.GameId == -2)
                    {
                        resolvedState.ConTargetAddress = Proxy.DefaultServer;
                    }
                }
            }

            if (resolvedState == null)
            {
                client.State.ACCID = client.PlayerData.AccountId;
            }
            else
            {
                foreach (var pair in client.State.States)
                {
                    resolvedState[pair.Key] = pair.Value;
                }

                foreach (var pair in client.State.States)
                {
                    resolvedState[pair.Key] = pair.Value;
                }

                client.State = resolvedState;
            }
        }
    }
}