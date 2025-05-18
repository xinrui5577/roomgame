using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MotionGroupManager : SceneManagerBase
    {
        public List<HandMotionCtrl> HandMotionCtrls;

        /// <summary>
        /// 获取手状态机
        /// </summary>
        /// <param name="chair">客户端座位号</param>
        /// <returns></returns>
        public HandMotionCtrl this[int clientChair]
        {
            get
            {
                var tableChair = clientChair.ExChairC2T();
                return HandMotionCtrls[tableChair];
            }
        }  

        public void PlayMotion(int chair, HandMotionType type)
        {
            this[chair].PlayMotion(chair, type);
        }
    }
}