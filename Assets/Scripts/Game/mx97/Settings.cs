using UnityEngine;
using System.Collections;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.mx97
{
    public class Settings : MonoBehaviour
    {
        public GameObject pref_settings;
        public Transform trans;
        public PrizeInfoWindow ThePrizeInfoWindow;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnClickCreatePrefab()
        {
            Facade.Instance<MusicManager>().Play("button");
            Instantiate(pref_settings, trans);
            GameObject.FindWithTag("MainCamera").GetComponent<UICamera>().enabled = false;  //屏蔽NGUI事件
        }
        /// <summary>
        /// 查看
        /// </summary>
        public void OnClickLookPrize()
        {
            Facade.Instance<MusicManager>().Play("button");
            if (ThePrizeInfoWindow != null) ThePrizeInfoWindow.Show();
        }

    }

}
