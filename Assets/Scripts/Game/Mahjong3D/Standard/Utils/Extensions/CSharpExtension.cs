namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public static partial class CSharpExtension
    {
        public static T Do<T>(this T self, System.Action action)
        {
            if (self == null) return self;
            if (action != null) action();
            return self;
        }

        public static T Do<T>(this T self, System.Action<T> action)
        {
            if (self == null) return self;
            if (action != null) action(self);
            return self;
        }

        /// <summary>
        /// 判断空对象
        /// </summary>
        public static bool ExIsNullOjbect<T>(this T instance) where T : class
        {
            return instance == null;
        }
    }
}
