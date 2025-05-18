using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class FilterOperateMenuAttribute : Attribute
    {
        public FilterOperateMenuAttribute() { }
    }
}
