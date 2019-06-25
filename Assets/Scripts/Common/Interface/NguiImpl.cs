using UnityEngine;
using YxFramwork.Common.Interface;

namespace Assets.Scripts.Common.Interface
{
    public class NguiImpl : IUI
    {

        public Transform CreateUI(int layer)
        {
            return NGUITools.CreateUI(false, layer).transform;
        }
    }
}
