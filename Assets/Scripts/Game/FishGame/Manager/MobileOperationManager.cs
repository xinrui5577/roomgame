using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Manager
{
    public class MobileOperationManager : FishOperationManager
    { 
        public override void Listening()
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase != TouchPhase.Ended) StartFire();
            else StopFire();

            if (Application.platform == RuntimePlatform.Android && (Input.GetKeyDown(KeyCode.Escape)))
            {
                GameMain.OnQuitGame();
            }

            OnMouseMove(Input.mousePosition); 
        }

        /// <summary>
        /// 锁鱼
        /// </summary>
        public void OnLockFish()
        {
            LockFish();
            StartFire();
        }

        /// <summary>
        /// 解锁
        /// </summary>
        public void OnLockFishUp()
        {
            UnLockFish();
            StopFire();
        } 
    }
}
