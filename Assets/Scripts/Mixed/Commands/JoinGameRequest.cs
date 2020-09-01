using AOT;
using Unity.Burst;
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
        // Unused integer for demonstration
        public int value;
        public void Deserialize(ref DataStreamReader reader)
        {
            value = reader.ReadInt();
        }

        public void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteInt(value);
        }

        [BurstCompile]
        [MonoPInvokeCallback(typeof(RpcExecutor.ExecuteDelegate))]
        private static void InvokeExecute(ref RpcExecutor.Parameters parameters)
        {
            RpcExecutor.ExecuteCreateRequestComponent<JoinGameRequest>(ref parameters);
        }

        static PortableFunctionPointer<RpcExecutor.ExecuteDelegate> InvokeExecuteFunctionPointer =
            new PortableFunctionPointer<RpcExecutor.ExecuteDelegate>(InvokeExecute);

        public PortableFunctionPointer<RpcExecutor.ExecuteDelegate> CompileExecute()
        {
            return InvokeExecuteFunctionPointer;
        }
    }

    /// <summary>
    /// The system that makes the RPC request component transfer
    /// </summary>
    public class JoinGameRequestSystem : RpcCommandRequestSystem<JoinGameRequest>
    {
    }

}
