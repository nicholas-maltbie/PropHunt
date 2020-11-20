using Unity.Burst;
using Unity.NetCode;

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