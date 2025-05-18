using UnityEngine;
using System.Collections;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.LXGameScripts.lx39
{
    /// <summary>
    /// 播放投币动画
    /// </summary>
    public class PlayInserIcon : MonoBehaviour
    {
        private GameObject[] _icons;
        private string _iconName = "icon_0";
        // Use this for initialization
        void Start()
        {
            FindObjectByName();
        }

        public void OnPlayInsert()
        {
            StartCoroutine(PlayInsert());
        }

        protected virtual IEnumerator PlayInsert()
        {
            WaitForSeconds wait = new WaitForSeconds(0.1f);
            for (int i = 0; i < _icons.Length; i++)
            {
                if (i == _icons.Length - 1) Facade.Instance<MusicManager>().Play("insert");//播放投币声音
                _icons[i].SetActive(true);
                yield return wait;
                _icons[i].SetActive(false);
            }
        }

        private void FindObjectByName()
        {
            int num = transform.childCount;
            _icons = new GameObject[num];
            for (int i = 0; i < num; i++)
            {
                _icons[i] = transform.FindChild(_iconName + i).gameObject;
            }
        }
    }
}