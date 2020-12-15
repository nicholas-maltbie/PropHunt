using Unity.Mathematics;
using Unity.NetCode;
using Unity.Networking.Transport;

namespace PropHunt.Mixed.Commands
{

    /// <summary>
    /// Player Input to control a user character. 
    /// </summary>
    public struct PlayerInput : ICommandData
    {
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
        /// Current value for player view Yaw.
        /// </summary>
        public float targetYaw;

        /// <summary>
        /// Current value for player view Pitch.
        /// </summary>
        public float targetPitch;

        /// <summary>
        /// Is the player attempting to jump at this tick.
        /// 1 is jumping, 0 is not jumping.
        /// </summary>
        public byte jump;

        /// <summary>
        /// Is the character currently attempting to jump as a boolean.
        /// </summary>
        public bool IsJumping => this.jump == 1;

        /// <summary>
        /// Is the player attempting to interact at this tick.
        /// 1 is interacting, 0 is not interacting.
        /// </summary>
        public byte interact;

        /// <summary>
        /// Is the character currently attempting to interact as a boolean.
        /// </summary>
        public bool IsInteracting => this.interact == 1;

        /// <summary>
        /// Is the player currently attempting to sprint.
        /// 1 is sprinting, 0 is not sprinting;
        /// </summary>
        public byte sprint;

        /// <summary>
        /// Is the character currently attempting to jump as a boolean
        /// </summary>
        public bool IsSprinting => this.sprint == 1;

        /// <summary>
        /// Movement of the floor
        /// </summary>
        public float3 floorMovement;

        public uint Tick
        {
            get { return this.tick; }
            set { this.tick = value; }
        }

        public void Deserialize(uint tick, ref DataStreamReader reader)
        {
            this.tick = tick;
            this.horizMove = reader.ReadFloat();
            this.vertMove = reader.ReadFloat();
            this.targetPitch = reader.ReadFloat();
            this.targetYaw = reader.ReadFloat();
            this.jump = reader.ReadByte();
            this.interact = reader.ReadByte();
            this.sprint = reader.ReadByte();
            this.floorMovement.x = reader.ReadFloat();
            this.floorMovement.y = reader.ReadFloat();
            this.floorMovement.z = reader.ReadFloat();
        }

        public void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteFloat(this.horizMove);
            writer.WriteFloat(this.vertMove);
            writer.WriteFloat(this.targetPitch);
            writer.WriteFloat(this.targetYaw);
            writer.WriteByte(this.jump);
            writer.WriteByte(this.interact);
            writer.WriteByte(this.sprint);
            writer.WriteFloat(this.floorMovement.x);
            writer.WriteFloat(this.floorMovement.y);
            writer.WriteFloat(this.floorMovement.z);
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
}