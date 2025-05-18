using Assets.Scripts.Game.FishGame.Common.Utils;
using UnityEngine;
using System.Collections;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    /// <summary>
    /// 定时停止恢复
    /// </summary>
    /// <remarks>几种情况需要处理恢复问题:
    /// 1.恢复的时间点在正常游戏时间内.
    /// 2.恢复的时间点在过场(sweep)中,:恢复出鱼,因为会有子弹不能消除BUG
    /// 4.开始的时间点在过场中,等待子弹消失过程中->再有子弹击中定时炸弹 : 直接恢复,原因同上
    /// </remarks>
    public class FreezeBombRecover : MonoBehaviour
    { 
        void Awake()
        {
            StartCoroutine(_Coro_DelayRecover());  
        } 

        /// <summary>
        /// 定身炸弹恢复出鱼和鱼的移动
        /// </summary>
        /// <returns></returns>
        IEnumerator _Coro_DelayRecover()
        { 
            yield return new WaitForSeconds(10F); 

            Recover();
        
            Destroy(gameObject);
        }

        //恢复出鱼,和鱼的移动
        void Recover()
        { 
            GameMain.IsMainProcessPause = false;
            var gdata = App.GetGameData<FishGameData>();
            if (gdata.EvtFreezeBombDeactive != null)
                gdata.EvtFreezeBombDeactive(); 
        }
    }
}
