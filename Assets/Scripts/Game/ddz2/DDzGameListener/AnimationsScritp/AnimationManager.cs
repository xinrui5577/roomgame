using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.AnimationsScritp
{
    public class AnimationManager : ServEvtListener
    {

        public Anim StartAnim;

        protected void Awake()
        {
            OnAwake();
        }

        protected override void OnAwake()
        {
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeAllocate, OnTypeAllocate);
        }
      
        /// <summary>
        /// 发牌阶段,播放动画
        /// </summary>
        /// <param name="args"></param>
        private void OnTypeAllocate(DdzbaseEventArgs args)
        {
            StartAnim.Play();
        }


        public override void RefreshUiInfo()
        {
        }
    }
}