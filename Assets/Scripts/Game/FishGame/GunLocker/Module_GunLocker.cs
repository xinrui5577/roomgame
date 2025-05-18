using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.Common.external.NemoPoolGOs;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using System.Collections;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.GunLocker
{
    /// <summary>
    /// 枪锁定模块
    /// </summary>
    public class Module_GunLocker : MonoBehaviour
    {
        public float RotateRange = 180F; //角度

        public Module_Bullet ModuleBullet; //子弹模块
        private Player_FishLocker[] mFishLocker;
        private Fish[] mLockedTarget;
        private GameMain mGM;

        private void Start()
        {
            //Debug.Log("Awake");
            if (ModuleBullet == null)
                ModuleBullet = FindObjectOfType(typeof (Module_Bullet)) as Module_Bullet;
            mGM = GameMain.Singleton;
            var gdata = App.GetGameData<FishGameData>();
            gdata.EvtPlayerGunChanged += Handle_PlayerGunChanged;
            gdata.EvtPlayerGunFired += Handle_GunFire;
            HitProcessor.Evt_Hit += Handle_FishBulletHit;
            gdata.EvtMainProcessStartGame += Handle_StartGame;
            mFishLocker = new Player_FishLocker[Defines.MaxNumPlayer];
            mLockedTarget = new Fish[Defines.MaxNumPlayer];


        }

        private void Handle_StartGame()
        {
            var batterys = mGM.PlayersBatterys;
            if (batterys == null) return;
            foreach (var p in batterys)
            {
                mFishLocker[p.Idx] = p.GetComponent<Player_FishLocker>();
                mFishLocker[p.Idx].EvtRelock += (f, p2) => LockAt(p2, f); //重锁定指定鱼
                mFishLocker[p.Idx].EvtUnlock += Handle_FishLockerUnlock;
            }
        }

        private void Handle_FishLockerUnlock(Player p)
        {
            UnLock(p);
        }

        private void Handle_FishBulletHit(bool isMiss, Player p, Bullet b, Fish f)
        {
            if (isMiss && b.IsLockingFish)
            {
                //目标无效.解除锁定
                b.IsLockingFish = false;
            }
        }

        private void Handle_PlayerGunChanged(Player p, Gun newGun)
        {
            if (IsPlayerLockingFish(p.Idx))
            {
                //StartCoroutine(_Coro_Locking(p));
            }
        }

        /// <summary>
        /// 创建锁鱼子弹
        /// </summary>
        /// <param name="p">开枪者</param>
        /// <param name="g">枪</param>
        /// <param name="score">分数</param>
        /// <param name="isLock"></param>
        /// <param name="bulletId">子弹id</param>
        private void Handle_GunFire(Player p, Gun g, int score, bool isLock, int bulletId)
        {
            if (!isLock) return;
            var prefabBullet = g.Prefab_BulletNormal;//ModuleBullet.Get_PrefabBullet_Used(g);
            var b = Pool_GameObj.GetObj(prefabBullet.gameObject).GetComponent<Bullet>();
            b.Id = bulletId;
            b.transform.position = g.local_GunFire.position;
            b.Score = score;
            /*var upToward = mLockedTarget[p.Idx].transform.position - g.TsGun.position;
            upToward.z = 0F;
            var targetDir = Quaternion.FromToRotation(Vector3.up, upToward);*/
            b.Fire(p, mLockedTarget[p.Idx], g.TsGun.rotation);
        }

        public void LockAt(Player p, Fish f)
        {
            //Debug.Log("LockAt " + f.name + "  p.idx = " + p.Idx);
            mLockedTarget[p.Idx] = f;
            p.GunInst.Rotatable = false;
            ModuleBullet.Fireable[p.Idx] = false;
            //StartCoroutine(_Coro_Locking(p));
        }

        public void UnLock(Player p)
        {
            //Debug.Log("UnLock");
            mLockedTarget[p.Idx] = null;
            if (p.GunInst == null) return;
            p.GunInst.Rotatable = true;
            ModuleBullet.Fireable[p.Idx] = true;
        }

        private IEnumerator _Coro_Locking(Player p)
        {
            var rotateRrangeHalf = RotateRange*0.5f;
            var curTargetFish = mLockedTarget[p.Idx];
            if (curTargetFish == null) yield break;
            var tsTarget = curTargetFish.transform;
            //var loclLocal = curTargetFish.GetComponent<FishEx_LockPos>();
            //while (tsTarget != null && mLockedTarget[p.Idx] != null)//第一判断用于判断鱼是否游出,第二判断用于是否解锁
            while (true)
            {
                var playerIndex = p.Idx;
                if (mLockedTarget[playerIndex] == null || tsTarget == null) yield break;

                if (curTargetFish.ID != mLockedTarget[playerIndex].ID)
                {
                    curTargetFish = mLockedTarget[playerIndex];
                    if (curTargetFish == null || !curTargetFish.Attackable)
                        yield break;
                    //loclLocal = curTargetFish.GetComponent<FishEx_LockPos>();
                    //tsTarget = curTargetFish.transform;
                }
                yield return new WaitForEndOfFrame();
                //枪炮转头
                var gunTs = p.GunInst.TsGun;
                var upToward = (tsTarget.position) - gunTs.position;//loclLocal != null ? loclLocal.LockBulletPos :
                upToward.z = 0F;
                gunTs.rotation = Quaternion.Slerp(gunTs.rotation, Quaternion.FromToRotation(Vector3.up, upToward),
                                                  10F*Time.deltaTime);

                //临界
                if (gunTs.localEulerAngles.z > rotateRrangeHalf && gunTs.localEulerAngles.z < (360F - rotateRrangeHalf))
                {
                    if (gunTs.localEulerAngles.z < 180F)
                    {
                        gunTs.RotateAroundLocal(Vector3.forward,
                                                -1.0F*(gunTs.localEulerAngles.z - rotateRrangeHalf)*Mathf.Deg2Rad);
                    }
                    else
                    {
                        gunTs.RotateAroundLocal(Vector3.forward,
                                                (360F - rotateRrangeHalf - gunTs.localEulerAngles.z)*Mathf.Deg2Rad);
                    }
                }
                yield return 0;
            }
        }

        public bool IsPlayerLockingFish(int pIdx)
        {
            return mLockedTarget[pIdx] != null;
        }

        public void LockFish(Player player)
        {
            if (mFishLocker[player.Idx] == null) return;
            mFishLocker[player.Idx].Lock();
        }

        public void UnLockFish(Player player)
        {
            if (mFishLocker == null) return;
            if (mFishLocker[player.Idx] == null) return;
            mFishLocker[player.Idx].UnLock();
            UnLock(player);
        }

        public bool HasLock(Player player)
        {
            return mFishLocker[player.Idx] != null && mFishLocker[player.Idx].HasLock();
        }

        public void SetLockFish(Fish target,Player player)
        {
            if (mFishLocker == null) return;
            if (mFishLocker[player.Idx] == null) return;
            mFishLocker[player.Idx].SetLockTarget(target);
        }

    }
}
