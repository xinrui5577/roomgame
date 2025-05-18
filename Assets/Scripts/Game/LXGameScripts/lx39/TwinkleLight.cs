using UnityEngine;

namespace Assets.Scripts.Game.LXGameScripts.lx39
{
    /// <summary>
    /// 闪烁灯光
    /// </summary>
    public class TwinkleLight : MonoBehaviour
    {
        public GameObject[] LightBoards;

        private float _twinkleTime = 1f;

        private float _times = 0;

        void Update()
        {
            _times += Time.deltaTime;
            if (_times >= _twinkleTime)
            {
                foreach (var item in LightBoards)
                {
                    item.SetActive(!item.activeInHierarchy);
                }
                _times = 0;
            }
        }
    }
}