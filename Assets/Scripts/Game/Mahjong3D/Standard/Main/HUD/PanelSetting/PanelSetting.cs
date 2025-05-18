using UnityEngine.UI;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [UIPanelData(typeof(PanelSetting), UIPanelhierarchy.System)]
    public class PanelSetting : UIPanelBase
    {
        public Slider Sound;
        public Slider Effect;
        public event Action<MahjongUIToggleType, int> BtnSwitchAction;

        public override void OnReadyUpdate()
        {
            Close();
        }

        public override void OnContinueGameUpdate()
        {
            Close();
        }

        public override void OnInit()
        {
            //设置游戏声音      
            Sound.value = MahjongUtility.MusicSound;
            Effect.value = MahjongUtility.MusicEffect;          
        }

        public void OnSoundChange() { MahjongUtility.MusicSound = Sound.value; }

        public void OnEffectChagne() { MahjongUtility.MusicEffect = Effect.value; }

        /// <summary>
        /// 切换按钮事件
        /// </summary>
        /// <param name="type">按钮类型</param>
        /// <param name="index">按钮编号</param>
        public void SwitchAction(MahjongUIToggleType type, int index)
        {
            BtnSwitchAction(type, index);
        }

        public override void Close()
        {
            base.Close();
            SetHandMjTouch(true);
        }

        public override void Open()
        {
            base.Open();
            SetHandMjTouch(false);
        }
    }
}