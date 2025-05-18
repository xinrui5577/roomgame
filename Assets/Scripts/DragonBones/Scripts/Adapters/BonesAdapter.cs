using DragonBones;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;

namespace Assets.Scripts.DragonBones.Scripts.Adapters
{
    [RequireComponent(typeof(UnityArmatureComponent))]
    public class BonesAdapter : YxBasePanelAdapter
    {
        private UnityArmatureComponent _component;
        protected UnityArmatureComponent Component
        {
            get { return _component == null ? _component = GetComponent<UnityArmatureComponent>() : _component; }
        }

        public override YxEUIType UIType
        {
            get { return YxEUIType.Default; }
        }

        protected override void OnSortingOrder(int order)
        {
            Component.sortingOrder = order + Order; ;
        }

        public override Vector4 GetBound()
        {
            return Vector4.one;
        }

        public override int Depth
        {
            get { return (int)Component.zSpace; }
            set { Component.zSpace = value; }
        }
    }
}
