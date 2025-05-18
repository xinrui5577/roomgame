/** 
 *文件名称:     GameBegin.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-21 
 *描述:         控制显示游戏开始的动画的脚本
 *历史记录: 
*/

using Assets.Scripts.Game.lyzz2d.Utils.Single;
using UnityEngine;

namespace Assets.Scripts.Game.lyzz2d.Game.UI
{
    public class GameBegin : MonoSingleton<GameBegin>
    {
        private Animation _ani;
        private UIPlayAnimation _play;
        private Transform aniTrans;
        private EventDelegate dele;

        public override void Awake()
        {
            base.Awake();
            dele = new EventDelegate(OnFinished);
            aniTrans = transform.GetChild(0);
            _ani = aniTrans.GetComponent<Animation>();
            _play = aniTrans.GetComponent<UIPlayAnimation>();
            _play.resetOnPlay = true;
            _play.onFinished.Add(dele);
        }

        public void PlayAni()
        {
            aniTrans.gameObject.SetActive(true);
            _play.PlayForward();
        }

        private void OnFinished()
        {
            _ani.Rewind();
            aniTrans.gameObject.SetActive(false);
        }

        public override void OnDestroy()
        {
            _play.onFinished.Remove(dele);
            base.OnDestroy();
        }
    }
}