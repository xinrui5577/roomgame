using UnityEngine;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.bjl3d
{
    public class DownUI : MonoBehaviour//G  11.15
    {
        private Transform _yanTf;
        protected void Awake()
        {
            _yanTf = transform.Find("ss1");
            if (_yanTf == null)
                YxDebug.LogError("没有该物体");//没有该物体 
        }

        /// <summary>
        /// 闪动图片
        /// </summary>
        public void ShowYanEff()
        {
            if (_yanTf.gameObject.activeSelf)
                _yanTf.gameObject.SetActive(false);
            _yanTf.gameObject.SetActive(true);
        }


    }
}
