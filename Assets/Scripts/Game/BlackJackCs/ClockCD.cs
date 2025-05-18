using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.BlackJackCs
{
    public class ClockCD : MonoBehaviour
    {


        private int _cdTime = 12;

        [SerializeField]
        private UILabel _timeLabel;

    
        /// <summary>
        /// 开始倒计时
        /// </summary>
        /// <param name="time"></param>
        public void BeginCountDown(int time)
        {
            if(time <= 0)
            {
                com.yxixia.utile.YxDebug.YxDebug.Log("Time should be a positive number!!");
                return;
            }
            _cdTime = time;
            gameObject.SetActive(true);
            StartCoroutine(DoCd());
        }

        /// <summary>
        /// 停止倒计时
        /// </summary>
        public void StopCountDown()
        {
            _timeLabel.text = "0";
            StopAllCoroutines();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 协程倒计时
        /// </summary>
        /// <returns></returns>
        IEnumerator DoCd()
        {
            while (_cdTime >= 0)
            {
                _timeLabel.text = _cdTime.ToString();
                yield return new WaitForSeconds(1f);
                _cdTime--;
                //Debug.Log(_cdTime);
            }
            StopCountDown();
        }


        void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}