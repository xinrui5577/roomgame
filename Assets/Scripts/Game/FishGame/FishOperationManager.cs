using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.Common.Managers;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Utils;
using YxFramwork.Framework.Core;
using YxFramwork.View;

namespace Assets.Scripts.Game.FishGame
{
    public abstract class FishOperationManager : OperationManager
    {
        protected bool CanMoveWithMouse = true;
        private Camera _mainCamera;
        /// <summary>
        /// 未开枪时间（时间到会退出房间）
        /// </summary>
        public int NoShootingTime = 60;

        public Player UserSelf { get; set; }

        public tk2dUIToggleButton AutoFireToggle;
        public tk2dUIToggleButton QuickToggle;
        public tk2dUIToggleButton LockBlueToggle;

        private void Start()
        {
            _mainCamera = Util.GetMainCamera();
        }

        void LateUpdate()
        {
            var data = App.GetGameData<FishGameData>();
            if (data == null) return;
            if (GameMain.Singleton == null) return;
            if (data.GameState != YxGameState.Run) return;
            if (tk2dUIManager.Instance.PressedUIItem != null) return;
            Listening();
        }
 
        /// <summary>
        /// 改变玩家分数
        /// </summary>
        /// <param name="numAdd">增减量</param>
        public void ChangePlayerScore(int numAdd)
        {
            if (UserSelf == null) return;
            UserSelf.ChangeScore(numAdd); 
        }

        protected Vector3 _oldMousePos;
        public void OnMouseMove(Vector3 mousePos)
        {
            if (_oldMousePos == mousePos) return;
            var worldpos = Camera.main.ScreenToWorldPoint(mousePos);
            if (worldpos.y < -340) return;
            Aim(worldpos);
            _oldMousePos = Input.mousePosition;
        }

        /// <summary>
        /// 锁定指定鱼
        /// </summary>
        public bool OnLockDesignated()
        {
            if (UserSelf == null) return false;
            var gmain = GameMain.Singleton;
            if (gmain == null) return false;
            var gunlocker = gmain.Gunlocker;
            if (gunlocker == null) return false;
            if (!gunlocker.HasLock(UserSelf)) return false;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var hitTarget = hit.collider.gameObject;
                if (hitTarget != null)
                {
                    var fish = hitTarget.GetComponent<Fish>();
                    if (fish != null && fish.IsLockable)
                    {
                        gunlocker.SetLockFish(fish,UserSelf);
                        return true;
                    }
                }
            }
            LockFish();
            return true;
        }

        /// <summary>
        /// 获得玩家当前分数
        /// </summary>
        /// <returns></returns>
        public int GetPlayerScore()
        {
            return GameMain.Singleton.BSSetting.Dat_PlayersScore[0].Val;
        }

        /// <summary>
        /// 设置玩家当前分数
        /// </summary>
        /// <param name="newScore"></param>
        public void SetPlayerFireScore(int newScore)
        {
            if (UserSelf == null) return;
            if (newScore <= 0) return;
            UserSelf.ChangeNeedScore(newScore);
        }

        /// <summary>
        /// 向上切换炮的样式
        /// </summary>
        public void ChangePriorGunStyle()
        {
            if (UserSelf == null) return;
            UserSelf.ChangePriorGunStyle(); 
        }

        /// <summary>
        /// 向下切换炮的样式
        /// </summary>
        public void ChangeNextGunStyle()
        {
            if (UserSelf == null) return;
            UserSelf.ChangeNextGunStyle(); 
        }
            
        public void Aim(Vector3 worldPos)
        {
            if (UserSelf == null) return;
            var gun = UserSelf.GunInst;
            if (gun==null) return;
            var tsGun = gun.TsGun;
            worldPos.z = tsGun.position.z;
            var lookDirect = worldPos - tsGun.position; 
            var quaternion = Quaternion.LookRotation(Vector3.forward, lookDirect);
            gun.Turn(quaternion); 
        }

        public void RotateGun(Gun.RotateState dir)
        {
            if (UserSelf == null) return;
            var gun = UserSelf.GunInst;
            if (gun == null) return;
            gun.RotateTo(dir);
        }

        public void StopRotateGun()
        {
            if (UserSelf == null) return;
            var gun = UserSelf.GunInst;
            if (gun == null) return;
            gun.StopRotate();
        }

        /// <summary>
        /// 玩家向当前方向 开始开火 (开始后需要调用UserSelfStopFire才会停止)
        /// </summary>
        public void StartFire()
        {
            OnLockDesignated();
            if (UserSelf == null) return;
            var gun = UserSelf.GunInst;
            if (gun == null) return;
            gun.StartFire();
            StopCheck();
        }

        public void StopFire()
        {
            if (UserSelf == null) return;
            var gun = UserSelf.GunInst; 
            if (gun == null) return;
            gun.StopFire(); 
            CheckNoFire();
        }

        private void ShowSelfMark(bool show)
        {
            CancelInvoke("OnShowSelfMark");
            if (show)
            {
                Invoke("OnShowSelfMark", 3);
                return;
            }
            UserSelf.DisplaySelfMark();
        }

        private void OnShowSelfMark()
        {
            UserSelf.DisplaySelfMark(true);
        }


        /// <summary>
        /// 是否开火中
        /// </summary>
        /// <returns></returns>
        public bool IsFiring()
        {
            return UserSelf != null && UserSelf.GunInst.IsFiring();
        }

        /// <summary>
        /// 是否加速开火
        /// </summary>
        /// <returns></returns>
        public bool IsQuickenFire()
        {
            return UserSelf != null && UserSelf.GunInst.IsFastFirable;
        }

        /// <summary>
        /// 设置加速状态
        /// </summary>
        /// <param name="isFast"></param>
        public void SetQuickenFire(bool isFast)
        {
            if (UserSelf == null) return;
            UserSelf.GunInst.IsFastFirable = isFast;
        }

        /// <summary>
        /// 锁鱼
        /// </summary>
        public void LockFish()
        { 
            if (UserSelf == null) return;
            var gmain = GameMain.Singleton;
            if (gmain == null) return;
            var gunlocker = gmain.Gunlocker;
            if (gunlocker == null) return; 
            gunlocker.LockFish(UserSelf);
            if (LockBlueToggle != null) LockBlueToggle.IsOn = true;
        }

        /// <summary>
        ///  接触锁鱼
        /// </summary>
        public void UnLockFish()
        {
            if (UserSelf == null) return;
            var gmain = GameMain.Singleton;
            if (gmain == null) return;
            var gunlocker = gmain.Gunlocker;
            if (gunlocker == null) return; 
            gunlocker.UnLockFish(UserSelf);
            if (LockBlueToggle != null) LockBlueToggle.IsOn = false;
        }

        /// <summary>
        /// 自动开火
        /// </summary> 
        public void ChangeAutoFire()
        {
            var isAuto = UserSelf != null && UserSelf.GunInst.ChangeAutoFire();
            if (AutoFireToggle!=null) AutoFireToggle.IsOn = isAuto;
            if (isAuto)
            {
                StopCheck();
            }
            CheckNoFire();
        } 

        /// <summary>
        /// 加速
        /// </summary>
        /// <returns></returns>
        public void ChangeQuicken()
        {
            if (UserSelf == null) return;
            var isChange = UserSelf.GunInst.ChangeQuicken();
            if (QuickToggle != null) QuickToggle.IsOn = isChange;
        }

        public void BuyCoin()
        { 
            GameMain.Singleton.BuyCoin();
        }

        public void Retrieve()
        {
            GameMain.Singleton.Retrieve();
        }

        private bool _isChecking;
        private void CheckNoFire()
        {
            if (IsFiring()) return;
            ShowSelfMark(true);
            if (_isChecking) return;
            _isChecking = true;
            Invoke("NoFireEvent", NoShootingTime);
        }

        private void StopCheck()
        {
            _isChecking = false;
            CancelInvoke("NoFireEvent");
            ShowSelfMark(false);
        }

        private void NoFireEvent()
        {
            YxMessageBox.Show("您很久没有开火了，还有[ff0000] {0} [-]秒，您将自动离开房间！是否继续火拼？",10,(msgBox, btnName) =>
            {
                _isChecking = false;
                if (btnName == YxMessageBox.BtnRight)
                {
                    App.QuitGame();
                    return;
                }
                CheckNoFire();
            }, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle, msgBox =>
                {
                    _isChecking = false;
                    App.QuitGame();
                });
        }

        public virtual void FullScreen(bool isFull)
        {
        }

        /// <summary>
        /// 锁鱼
        /// </summary>
        public void OnChangeLockFish(tk2dUIToggleButton tbtn)
        {
            if (tbtn.IsOn)
            {
                LockFish();
                StartFire();
                CanMoveWithMouse = false;
                return;
            }
            CanMoveWithMouse = true;
            UnLockFish();
            StopFire();
        }

        public void Reset()
        {
            UnLockFish();
        }
    }
}
