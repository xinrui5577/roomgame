using System;

namespace Assets.Scripts.Game.fruit
{
    public class UiTeventArgs : EventArgs
    {
        public LightItemCtrl.ItemAnimState AnimState { get; set; }
        //goodluck类型
        private int _randType = -1;//初始化-1表示没有goodluck状态
        public int RandType {
            get { return _randType; }
            set { _randType = value; }
        }

        public bool HasWin { get; set;}
    }
}
