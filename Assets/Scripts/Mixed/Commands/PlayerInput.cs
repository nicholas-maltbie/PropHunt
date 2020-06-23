using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

namespace PropHunt.Mixed.Commands 
{

    /// <summary>
    /// Player Input to control a user character. 
    /// </summary>
    public struct PlayerInput : ICommandData<PlayerInput>
    {
        public uint Tick => tick;

        /// <summary>
        /// Tick on which this input ocurred
        /// </summary>
        public uint tick;

        /// <summary>
        /// Horizontal (strafe) movement value (between 0-1).
        /// </summary>
        public float horizMove;
        /// <summary>
        /// Vertical (forward backward) movement value (between 0-1).
        /// </summary>
        public float vertMove;

        /// <summary>
        /// Change in pitch (up and down) of the player's view.
        /// </summary>
        public float pitchChange;
        /// <summary>
        /// Change in yaw (left and right) of the player's view.
        /// </summary>
        public float yawChange;

        /// <summary>
        /// Is the player attempting to jump at this tick.
        /// 1 is jumping, 0 is not jumping.
        /// </summary>
        public byte jump;
        /// <summary>
        /// Is the character currently attempting to jump as a boolean.
        /// </summary>
        public bool IsJumping => jump == 1;

        /// <summary>
        /// Is the player attempting to interact at this tick.
        /// 1 is interacting, 0 is not interacting.
        /// </summary>
        public byte interact;
        /// <summary>
        /// Is the character currently attempting to interact as a boolean.
        /// </summary>
        public bool IsInteracting => jump == 1;

        /// <summary>
        /// Is the player currently attempting to sprint.
        /// 1 is sprinting, 0 is not sprinting;
        /// </summary>
        public byte sprint;
        /// <summary>
        /// Is the character currently attempting to jump as a boolean
        /// </summary>
        public bool IsSprinting => sprint == 1;

        public void Deserialize(uint tick, ref DataStreamReader reader)
        {
            this.tick = tick;
            this.horizMove      = reader.ReadFloat();
            this.vertMove       = reader.ReadFloat();
            this.pitchChange    = reader.ReadFloat();
            this.yawChange      = reader.ReadFloat();
            this.jump           = reader.ReadByte();
            this.interact       = reader.ReadByte();
            this.sprint         = reader.ReadByte();
        }

        public void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteFloat(this.horizMove);
            writer.WriteFloat(this.vertMove);
            writer.WriteFloat(this.pitchChange);
            writer.WriteFloat(this.yawChange);
            writer.WriteByte(this.jump);
            writer.WriteByte(this.interact);
            writer.WriteByte(this.sprint);
        }

        public void Deserialize(uint tick, ref DataStreamReader reader, PlayerInput baseline,
            NetworkCompressionModel compressionModel)
        {
            Deserialize(tick, ref reader);
        }

        public void Serialize(ref DataStreamWriter writer, PlayerInput baseline, NetworkCompressionModel compressionModel)
        {
            Serialize(ref writer);
        }
    }

    public class NetCubeSendCommandSystem : CommandSendSystem<PlayerInput>
    {
    }

    public class NetCubeReceiveCommandSystem : CommandReceiveSystem<PlayerInput>
    {
    }

}
