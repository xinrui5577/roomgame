using System;
using System.Collections.Generic;
using Assets.Scripts.Game.Fishing.entitys;
using Assets.Scripts.Game.Fishing.enums;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Game.Fishing.Managers
{
    public class FishingOperationManager : MonoBehaviour
    {
        /// <summary>
        /// 未射击时间
        /// </summary>
        public int NoShotTime;

        /// <summary>
        /// 可以控制
        /// </summary>
        [NonSerialized]
        public bool CanMoveWithMouse;
        /// <summary>
        /// 可控制玩家
        /// </summary>
        public FishingOperationPlayer Player;
        /// <summary>
        /// Ui
        /// </summary>
        public GraphicRaycaster UiRaycaster;
        public EventSystem UiEventSystem;

        private Camera _camera;


        void Awake()
        {
            var center = Facade.EventCenter;
            center.AddEventListener<EOperationEventTpe, bool>(EOperationEventTpe.Fire, OnFire);
            center.AddEventListener<EOperationEventTpe, bool>(EOperationEventTpe.Auto, OnAutoFire);
//            center.AddEventListener<EOperationEventTpe, bool>(EOperationEventTpe.GunBarrelType, OnChangeBarrelType);
            center.AddEventListener<EOperationEventTpe, bool>(EOperationEventTpe.BulletType, OnChangeBulletType);
//            center.AddEventListener<EOperationEventTpe, bool>(EOperationEventTpe.Lock, OnChangeLock);
            center.AddEventListener<EOperationEventTpe, int>(EOperationEventTpe.FireMissile, OnFireMissile);
//            center.AddEventListener<EOperationEventTpe, int>(EOperationEventTpe.ChangeBet, OnChangeBet);
            center.AddEventListener<EOperationEventTpe, FishingOperationPlayer>(EOperationEventTpe.InitPlayer, InitPlayer);
        }

        private void InitPlayer(FishingOperationPlayer player)
        {
            Player = player;
            CanMoveWithMouse = true;
        }

        void Start()
        {
            _camera = App.GameManager.GameUiCamera;
        }

        private Fish _hitTarget;
        void LateUpdate()
        {
            if (YxWindowManager.CurWindow != null) return;
            if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                var eventData = new PointerEventData(UiEventSystem)
                {
                    pressPosition = Input.mousePosition,
                    position = Input.mousePosition
                };
                var list = new List<RaycastResult>();
                UiRaycaster.Raycast(eventData, list);
                if (list.Count < 1)
                {
                    if (CheckLockPattern())
                    {
                        var ray = _camera.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                        {
                            var hitTarget = hit.collider.GetComponentInParent<Fish>();
                            if (hitTarget != null && hitTarget.TheSwimmer.enabled)
                            {
                                //锁定目标
                                OnLockFire(hitTarget);
                            }
                            else
                            {
                                OnLockFire(null);
                            }
                        }
                        else
                        {
                            OnLockFire(null);
                        }
                    }
                    else
                    {
                        OnLockFire(null);
                    }
                    CanMoveWithMouse = true;
                    OnFire(true);
                }

            }
            else if (Input.GetKeyUp(KeyCode.F) || Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0))
            {
                OnFire(false);
                CanMoveWithMouse = false;
            }
            if (CanMoveWithMouse && _hitTarget==null) OnMouseMove();
        }

        /// <summary>
        /// 检测是否瞄准状态
        /// </summary>
        /// <returns></returns>
        private bool CheckLockPattern()
        {
            if (Player == null) return false;
            var gun = Player.TheGun;
            return gun != null && gun.Data.IsLock;
        }

        protected Vector3 OldMousePos;

        /// <summary>
        /// 方向控制
        /// </summary>
        public void OnMouseMove()
        {
            if (Player == null) return;
            var mousePos = Input.mousePosition;
            if (OldMousePos == mousePos) return;
            var direction = _camera.ScreenToWorldPoint(mousePos);
            Player.TurnGunManger(direction);
        }

        /// <summary>
        /// 开火
        /// </summary>
        public void OnFire(bool enable)
        { 
            if (Player == null) return;
            if (enable)
            {
                Player.OnStartFire();
            }
            else
            {
                Player.OnStopFire();
            }
        }

        /// <summary>
        /// 瞄准开火
        /// </summary>
        public void OnLockFire(Fish fish)
        {
            if (Player == null) return;
            _hitTarget = fish;
            Player.OnLockFire(fish);
        }


        /// <summary>
        /// 发射导弹
        /// </summary>
        public void OnFireMissile(int enable)
        {

        }

        /// <summary>
        /// 自动
        /// </summary>
        public void OnAutoFire(bool enable)
        {

        }

        /// <summary>
        /// 切换穿透/一次性状态
        /// </summary>
        public void OnChangeBulletType(bool isPenetrate)
        {
            if (Player == null) return;
            Player.TheGun.Data.IsPenetrate = isPenetrate;
        }

        /// <summary>
        /// 切换瞄准状态
        /// </summary>
        /// <param name="toggle"></param>
        public void OnChangeLockClick(Toggle toggle)
        {
            if (Player == null) return;
            Player.LockModel(toggle);
            if (toggle.isOn)
            {
                Player.OnCancelPenetrate();
                YxMessageTip.Show("点击鱼之后可以锁定目标", 3);
            }
        }

        /// <summary>
        /// 切换枪管状态
        /// </summary>
        /// <param name="toggle"></param>
        public void OnChangeBarrelTypeClick(Toggle toggle)
        {
            if (Player == null) return;
            //            Player.TheGun.Data.IsDouble = toggle.isOn;
            var isOn = toggle.isOn;
            Player.ChangeBarreTypeClick(isOn);
            if (isOn)
            {
                YxMessageTip.Show("消耗双倍金币，发射两颗子弹", 3);
            } 
        }
    }
}
