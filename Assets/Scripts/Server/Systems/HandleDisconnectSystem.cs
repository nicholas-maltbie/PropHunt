
using System;
using PropHunt.Mixed.Commands;
using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.NetCode;

namespace PropHunt.Server.Systems
{
    /// <summary>
    /// Whenever a player disconnects, handle the event
    /// </summary>
    [UpdateInGroup(typeof(ServerSimulationSystemGroup))]
    public class HandleDisconnectSystem : ComponentSystem
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<EnableProphuntGhostSendSystemComponent>();
        }

        protected override void OnUpdate()
        {
            // According to the unity docs
            // The connection also has a NetworkStreamDisconnected component for one frame,
            // after it disconnects and before the entity is destroyed.
            // https://docs.unity3d.com/Packages/com.unity.netcode@0.0/manual/network-connection.html

            Entities.WithNone<SendRpcCommandRequestComponent>().ForEach(
                (Entity reqEnt,
                ref NetworkStreamDisconnected disconnect,
                ref NetworkIdComponent networkId) =>
            {
                NetworkStreamDisconnectReason reason = disconnect.Reason;
                int sourceId = networkId.Value;
                UnityEngine.Debug.Log($"Player {sourceId} has disconnected due to reason {reason.ToString()}");

                Entities.ForEach((Entity ent, ref PlayerId playerId) => {
                    if (playerId.playerId == sourceId) {
                        PostUpdateCommands.DestroyEntity(ent);
                    }
                });
            });
        }
    }
}
