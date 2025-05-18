namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class VarFloat : Variable<float>
    {
        public VarFloat() { }

        public VarFloat(float value) : base(value) { }

        public static implicit operator VarFloat(float value)
        {
            return new VarFloat(value);
        }

        public static implicit operator float(VarFloat value)
        {
            if (null != value)
            {
                return value.Value;
            }
            return 0;
        }
    }

    public class VarFloatArray : Variable<float[]>
    {
        public VarFloatArray() { }

        public VarFloatArray(float[] value) : base(value) { }

        public static implicit operator VarFloatArray(float[] value)
        {
            return new VarFloatArray(value);
        }

        public static implicit operator float[] (VarFloatArray value)
        {
            if (null != value)
            {
                return value.Value;
            }
            return null;
        }
    }
}