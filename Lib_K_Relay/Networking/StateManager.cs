using Lib_K_Relay.Networking.Packets.Client;
using Lib_K_Relay.Networking.Packets.DataObjects;
using Lib_K_Relay.Networking.Packets.Server;
using System;
using System.Collections.Generic;
using System.Linq;

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

            proxy.ClientDisconnected += c => c.State.RenderedEntities.Clear();
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

            foreach (Status status in packet.Statuses)
            {
                Entity ent = client.State.RenderedEntities.FirstOrDefault(x => x.Status.ObjectId == status.ObjectId);
                if (ent == null) continue;
                ent.Status.Position = status.Position;
                foreach (StatData stat in status.Data)
                {
                    StatData oldStat = ent.Status.Data.FirstOrDefault(x => x.Id == stat.Id);
                    if (oldStat != null)
                    {
                        oldStat.IntValue = stat.IntValue;
                        oldStat.StringValue = stat.StringValue;
                    }
                }
            }
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
                    else if (resolvedState.LastHello.GameId == -2 && ((MapInfoPacket)client.State["MapInfo"]).Name == "Nexus")
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
                foreach (var pair in client.State.States.ToList())
                {
                    resolvedState[pair.Key] = pair.Value;
                }

                foreach (var pair in client.State.States.ToList())
                {
                    resolvedState[pair.Key] = pair.Value;
                }

                client.State = resolvedState;
            }

            if (packet.NewObjs.Select(x => x.Status.ObjectId).Contains(client.PlayerData.OwnerObjectId))
            {
                _proxy.FireOnTouchDown(client);
                client.State.RenderedEntities.Clear();
            }
            client.State.RenderedEntities.AddRange(packet.NewObjs.ToList());
            client.State.RenderedEntities = client.State.RenderedEntities.Where(x => !packet.Drops.Contains(x.Status.ObjectId)).ToList();
        }
    }
}
