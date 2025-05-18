namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class VarInt : Variable<int>
    {
        public VarInt() { }

        public VarInt(int value) : base(value) { }

        public static implicit operator VarInt(int value)
        {
            return new VarInt(value);
        }

        public static implicit operator int(VarInt value)
        {
            if (null != value)
            {
                return value.Value;
            }
            return 0;
        }
    }

    public class VarIntArray : Variable<int[]>
    {
        public VarIntArray() { }

        public VarIntArray(int[] value) : base(value) { }

        public static implicit operator VarIntArray(int[] value)
        {
            return new VarIntArray(value);
        }

        public static implicit operator int[](VarIntArray value)
        {
            if (null != value)
            {
                return value.Value;
            }
            return null;
        }
    }
}