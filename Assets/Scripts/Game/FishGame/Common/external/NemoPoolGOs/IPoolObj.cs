using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Common.external.NemoPoolGOs
{
    public interface IPoolObj {

        GameObject Prefab
        {
            get;
            set;

        }
        /// <summary>
        /// 重用
        /// </summary>
        void On_Reuse(GameObject prefab);

        /// <summary>
        /// 回收
        /// </summary>
        void On_Recycle();
    }
}
