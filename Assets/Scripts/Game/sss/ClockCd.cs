using UnityEngine;

namespace Assets.Scripts.Game.sss
{
    public class ClockCd : MonoBehaviour
    {
        private int _cdTime;

        private bool _isCounting;

        [SerializeField]
#pragma warning disable 649
        private UILabel _timeLabel;
#pragma warning restore 649

        /// <summary>
        /// 开始倒计时
        /// </summary>
        /// <param name="time"></param>
        public void BeginCountDown(int time = -1)
        {
            if (time <= 0) return;
            
            _cdTime = time;
            gameObject.SetActive(true);

            if (_isCounting) return;

            InvokeRepeating("DoCd", 0, 1);      //使用协程会引起计时误差
        }

        //protected void OnEnable()
        //{
        //    if (_isCounting)
        //        return;
            
        //    //StartCoroutine(DoCd());
        //    _isCounting = true;
        //}

        protected void DoCd()
        {
            _timeLabel.text = _cdTime.ToString();
            _cdTime--;
            if (_cdTime >= 0) return;
            StopCountDown();
        }

        /// <summary>
        /// 停止倒计时
        /// </summary>
        public void StopCountDown()
        {
            _timeLabel.text = "0";
            StopAllCoroutines();
            gameObject.SetActive(false);
            _isCounting = false;
            CancelInvoke();
        }

        /// <summary>
        /// 协程倒计时
        /// </summary>
        /// <returns></returns>
        //IEnumerator DoCd()
        //{
        //    while (_cdTime >= 0)
        //    {
        //        _timeLabel.text = _cdTime.ToString();
        //        yield return new WaitForSeconds(1f);
        //        _cdTime--;
        //    }
        //    StopCountDown();
        //}

        /// <summary>
        /// 时钟移动到目标父层级下
        /// </summary>
        /// <param name="parent">目标父层级</param>
        public void MoveClock(Transform parent)
        {
            transform.parent = parent;
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
        }

    }
}