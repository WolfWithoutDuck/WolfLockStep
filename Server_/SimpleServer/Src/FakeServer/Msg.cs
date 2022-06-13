using Lockstep.Math;
using Lockstep.Serialization;

public class PlayerServerInfo : BaseFormater
{
    public string name;
    public int Id;
    public int localId;
    public LVector3 initPos;
    public LFloat initDeq;
    public int PrefabId;

    public override void Serialize(Serializer writer)
    {
        writer.Write(initPos);
        writer.Write(initDeq);
        writer.Write(PrefabId);
    }

    public override void Deserialize(Deserializer reader)
    {
        initPos = reader.ReadLVector2();
        initDeq = reader.ReadLFloat();
        PrefabId = reader.ReadInt32();
    }
}

public class PlayerInput : BaseFormater
{
    public LVector2 mousePos;
    public LVector2 inputUV;
    public bool isInputFire;
    public int skillId;
    public bool isSpeedUp;


    public override void Serialize(Serializer writer)
    {
        writer.Write(mousePos);
        writer.Write(inputUV);
        writer.Write(isInputFire);
        writer.Write(skillId);
        writer.Write(isSpeedUp);
    }

    public override void Deserialize(Deserializer reader)
    {
        mousePos = reader.ReadLVector2();
        inputUV = reader.ReadLVector2();
        isInputFire = reader.ReadBoolean();
        skillId = reader.ReadInt32();
        isSpeedUp = reader.ReadBoolean();
    }

    public PlayerInput Clone()
    {
        var tThis = this;
        return new PlayerInput()
        {
            mousePos = tThis.mousePos,
            inputUV = tThis.inputUV,
            isInputFire = tThis.isInputFire,
            skillId = tThis.skillId,
            isSpeedUp = tThis.isSpeedUp
        };
    }
}

public class FrameInput : BaseFormater
{
    public int tick;
    public PlayerInput[] inputs;

    public override void Serialize(Serializer writer)
    {
        writer.Write(tick);
        writer.Write(inputs);
    }

    public override void Deserialize(Deserializer reader)
    {
        tick = reader.ReadInt32();
        inputs = reader.ReadArray(inputs);
    }

    public FrameInput Clone()
    {
        var tThis = this;
        var val = new FrameInput()
        {
            tick = tThis.tick
        };

        val.inputs = new PlayerInput[tThis.inputs.Length];

        for (int i = 0; i < val.inputs.Length; i++)
        {
            val.inputs[i] = this.inputs[i].Clone();
        }

        return val;
    }
}

 