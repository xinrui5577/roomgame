using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Manager
{
    public class InterfaceButtonManager : MonoBehaviour
    {
        public GameObject QuickenStyle;
        public GameObject NoQuickenStyle;
        public GameObject BtnQuicken;
        public GameObject BtnLock;
        public static InterfaceButtonManager Instanse;

        private void Awake()
        {
            Instanse = this;
        }

        void Start()
        {
#if UNITY_STANDALONE_WIN
            BtnQuicken.SetActive(false);
            BtnLock.SetActive(false);
#endif
        }

        /// <summary>
        /// 向上切换炮的样式
        /// </summary>
        public void OnChangePriorGunStyle()
        {
            GameMain.Singleton.Operation.ChangePriorGunStyle();
        }

        /// <summary>
        /// 向下切换炮的样式
        /// </summary>
        public void OnChangeNextGunStyle()
        { 
            GameMain.Singleton.Operation.ChangeNextGunStyle();
        }
       
        /// <summary>
        /// 锁鱼
        /// </summary>
        public void OnLockFish()
        {
            var operation = GameMain.Singleton.Operation;
            operation.LockFish();
            operation.StartFire();
        }

        /// <summary>
        /// 解锁
        /// </summary>
        public void OnLockFishUp()
        {
            var operation = GameMain.Singleton.Operation;
            operation.UnLockFish();
            operation.StopFire();
        } 
    }
}
