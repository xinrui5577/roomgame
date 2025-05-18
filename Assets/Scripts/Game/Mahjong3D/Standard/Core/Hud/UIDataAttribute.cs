using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    class UIPanelDataAttribute : Attribute
    {
        public string AssetsBundleName;
        public UIPanelhierarchy Hierarchy;

        //类型名 = assetsBundleName
        public UIPanelDataAttribute(Type type, UIPanelhierarchy hierarchy)
        {
            AssetsBundleName = type.Name;
            Hierarchy = hierarchy;
        }

        public UIPanelDataAttribute(string assetsName, UIPanelhierarchy hierarchy)
        {
            AssetsBundleName = assetsName;
            Hierarchy = hierarchy;
        }
    }

    public enum UIPanelhierarchy
    {
        Base,
        EffectAndTip,
        Popup,
        System,
    }
}
