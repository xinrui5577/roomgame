using Assets.Scripts.Game.FishGame.Common.core;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Manager
{
    public class PcOperationManager : FishOperationManager
    {
        protected const int StateKeyLeft = 0x01;
        protected const int StateKeyRight = 0x10;
        protected const int StateKeyNormal = 0x0;
        protected static int _btnState = 0;
        /// <summary>
        /// 加炮
        /// </summary>
        /// <param name="isKeyDown"></param>
        private void OnUp(bool isKeyDown)
        {
            if (!isKeyDown) return;
            ChangeNextGunStyle();
        }

        /// <summary>
        /// 减炮
        /// </summary>
        /// <param name="isKeyDown"></param>
        private void OnDown(bool isKeyDown)
        {
            if (!isKeyDown) return;
            ChangePriorGunStyle();
        }

        /// <summary>
        /// 左转炮
        /// </summary>
        /// <param name="isKeyDown"></param>
        private void OnLeft(bool isKeyDown)
        {
            CanMoveWithMouse = !isKeyDown;
            if (isKeyDown)
            {
                RotateGun(Gun.RotateState.Left);
                _btnState |= StateKeyLeft;
            }
            else
            {
                _btnState &= (StateKeyRight);
                if (_btnState == 0)
                {
                    StopRotateGun();
                }
                else
                {
                    RotateGun(Gun.RotateState.Right);
                }
            }
        }

        /// <summary>
        /// 右转炮
        /// </summary>
        /// <param name="isKeyDown"></param>
        private void OnRight(bool isKeyDown)
        {
            CanMoveWithMouse = !isKeyDown;
            if (isKeyDown)
            {
                RotateGun(Gun.RotateState.Right);
                _btnState |= StateKeyRight;
            }
            else
            {
                _btnState &= (StateKeyLeft);
                if (_btnState == 0)
                {
                    StopRotateGun();
                }
                else
                {
                    RotateGun(Gun.RotateState.Left);
                }
            }
        }

        /// <summary>
        /// 锁鱼
        /// </summary>
        /// <param name="isKeyDown"></param>
        private void OnLock(bool isKeyDown)
        {
            CanMoveWithMouse = !isKeyDown;
            if (isKeyDown) LockFish();
            else
            {
                _oldMousePos = Vector3.zero;
                UnLockFish();
            }
        }

        /// <summary>
        /// 监听
        /// </summary>
        public override void Listening()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.F3)) ChangeNextGunStyle();

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.F4))
                ChangePriorGunStyle();

            if (Input.GetKeyDown(KeyCode.LeftArrow)) OnLeft(true);
            else if (Input.GetKeyUp(KeyCode.LeftArrow)) OnLeft(false);

            if (Input.GetKeyDown(KeyCode.RightArrow)) OnRight(true);
            else if (Input.GetKeyUp(KeyCode.RightArrow)) OnRight(false);

            if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) StartFire();
            else if (Input.GetKeyUp(KeyCode.F) || Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0)) StopFire();

            if (Input.GetKeyDown(KeyCode.F1)) BuyCoin();
            if (Input.GetKeyDown(KeyCode.F2)) Retrieve();

            if (Input.GetKeyDown(KeyCode.S)) OnLock(true);
            else if (Input.GetKeyUp(KeyCode.S)) OnLock(false);

            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Z)) ChangeQuicken();

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                FullScreen(!Screen.fullScreen);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                FullScreen(false);
            }

            if (Input.GetKeyDown(KeyCode.G)) ChangeAutoFire();

            if (CanMoveWithMouse) OnMouseMove(Input.mousePosition);
        }

        public override void FullScreen(bool isFull)
        {
            Screen.fullScreen = isFull; 
        }
    }
}
