using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public abstract class UIPanelBase : MonoBehaviour, IUIPanelControl
    {
        protected DataCenterComponent DataCenter
        {
            get { return GameCenter.DataCenter; }
        }

        public virtual void OnInit() { }

        public virtual void Close()
        {
            transform.SetAsFirstSibling();
            gameObject.SetActive(false);
        }

        public virtual void Open()
        {
            transform.SetAsLastSibling();
            gameObject.SetActive(true);
        }

        public void ClickEffect()
        {           
            MahjongUtility.PlayEnvironmentSound("uiclick");
        }

        /// <summary>
        /// 准备panel
        /// </summary>
        public virtual void OnReadyUpdate() { }

        /// <summary>
        /// 接收到GameInfo之后刷新数据
        /// </summary>
        public virtual void OnGetInfoUpdate() { }

        /// <summary>
        /// 开始游戏时刷新
        /// </summary>
        public virtual void OnStartGameUpdate() { }

        /// <summary>
        /// 結束游戏时刷新
        /// </summary>
        public virtual void OnEndGameUpdate() { }

        /// <summary>
        /// 重连时刷新
        /// </summary>
        public virtual void OnReconnectedUpdate() { }

        /// <summary>
        /// 继续开局时刷新
        /// </summary>
        public virtual void OnContinueGameUpdate() { }

        /// <summary>
        /// 打开panel时,对手牌控制
        /// </summary>
        /// <param name="isOn"></param>
        protected void SetHandMjTouch(bool isOn) { GameCenter.Scene.HandMahTouchEnable = isOn; }
    }
}