using System.Collections;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameTable
{
    public class GameTimer : MonoBehaviour
    {
        public U3DNum TimeNum;

        private int _time;
        private Coroutine TimeUpdataCor;
        void Start()
        {
            TimeNum.Num = 0;
        }

        public void StartTime(int time,DVoidNoParam callBack = null)
        {
            YxDebug.Log("开始 计时器");
            TimeNum.gameObject.SetActive(true);
            _time = time;
            TimeNum.Num = time;
            if (TimeUpdataCor != null)
            {
                StopCoroutine(TimeUpdataCor);
            }
            TimeUpdataCor = StartCoroutine(TimeUpdata(callBack));
        }

        private IEnumerator TimeUpdata(DVoidNoParam callBack = null)
        {
            while (_time >= 0)
            {
                yield return new WaitForSeconds(1);
                _time--;
                TimeNum.Num = _time;
                if (_time<=3&&_time>=1)
                {
                    Facade.Instance<MusicManager>().Play("clock");
                }
                if (_time==0)
                {
                    Facade.Instance<MusicManager>().Play("naozhong");
                }
            }

            //TimeNum.gameObject.SetActive(false);
            if (callBack != null)
            {
                callBack();
            }
        }

        public void StopAndHideTimer()
        {
            if (TimeUpdataCor!=null)
            {
                StopCoroutine(TimeUpdataCor);
            }
            if (TimeNum!=null)
            {
                TimeNum.gameObject.SetActive(false);
            }
        }

        public void Reset()
        {
            TimeNum.gameObject.SetActive(false);
            if (TimeUpdataCor != null)
            {
                StopCoroutine(TimeUpdataCor);
            }
        }

        public void StopTimer()
        {
            TimeNum.Num = 0;            
            if (TimeUpdataCor != null)
            {
                StopCoroutine(TimeUpdataCor);
            }
        }

    }
}
