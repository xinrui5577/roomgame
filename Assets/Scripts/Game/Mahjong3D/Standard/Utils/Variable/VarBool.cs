namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class VarBool : Variable<bool>
    {
        public VarBool() { }

        public VarBool(bool value) : base(value) { }

        public static implicit operator VarBool(bool value)
        {
            return new VarBool(value);
        }

        public static implicit operator bool(VarBool value)
        {
            if (null != value)
            {
                return value.Value;
            }
            return false;
        }
    }
}