using AOT;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;

namespace PropHunt.Mixed.Commands
{

    /// <summary>
    /// RPC request from client to server for game to go "in game" and send snapshots / inputs
    /// </summary>
    [BurstCompile]
    public struct JoinGameRequest : IRpcCommand
    {
    }
}