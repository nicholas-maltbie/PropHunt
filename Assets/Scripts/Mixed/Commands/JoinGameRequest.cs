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
    public struct JoinGameRequest : IComponentData, IRpcCommandSerializer<JoinGameRequest>
    {
        [BurstCompile]
        [MonoPInvokeCallback(typeof(RpcExecutor.ExecuteDelegate))]
        private static void InvokeExecute(ref RpcExecutor.Parameters parameters)
        {
            RpcExecutor.ExecuteCreateRequestComponent<JoinGameRequest, JoinGameRequest>(ref parameters);
        }

        public void Serialize(ref DataStreamWriter writer, in JoinGameRequest data)
        {
            // Empty request (as of now) do not pass data
        }

        public void Deserialize(ref DataStreamReader reader, ref JoinGameRequest data)
        {
            // Empty request (as of now) do not pass data
        }

        public PortableFunctionPointer<RpcExecutor.ExecuteDelegate> CompileExecute()
        {
            return InvokeExecuteFunctionPointer;
        }

        static PortableFunctionPointer<RpcExecutor.ExecuteDelegate> InvokeExecuteFunctionPointer =
            new PortableFunctionPointer<RpcExecutor.ExecuteDelegate>(InvokeExecute);
    }

    /// <summary>
    /// The system that makes the RPC request component transfer
    /// </summary>
    public class JoinGameRequestSystem : RpcCommandRequestSystem<JoinGameRequest, JoinGameRequest>
    {
    }

}
