using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.Utils;
using Assets.Scripts.Game.FishGame.Common.external.NemoFileIO;
using Assets.Scripts.Game.FishGame.Fishs;
using Assets.Scripts.Game.FishGame.GunLocker;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.Common.Brains
{
    public class Ai : MonoBehaviour
    {
        /// <summary>
        /// 炮的等级
        /// </summary>
        private int _fireLevel = 0;
        /// <summary>
        /// 攻击目标
        /// </summary>
        private Transform _target;

        public int Id;
        /// <summary>
        /// Ai需要知道自己是谁
        /// </summary>
        private Player _self; 

        private Module_GunLocker _gunLocker;
        private bool _hasLocked;
        private PersistentData<int, int> curScoreData;
        private Gun _curGun;

        private void Start()
        {
            //enabled = false;
            _self = GetComponent<Player>();
            _gunLocker = FindObjectOfType(typeof(Module_GunLocker)) as Module_GunLocker;  
        }

        private void OnEnable()
        {
            _seatTime = Random.Range(3600, 9800);
        }

        void Update()
        {
            var gdata = App.GetGameData<FishGameData>();
            if (gdata == null) return;
            if (gdata.GameState != YxGameState.Run) return;
            if (curScoreData==null) curScoreData = GameMain.Singleton.BSSetting.Dat_PlayersScore[_self.Idx];
            _curGun = _self.GunInst; 
            if (_curGun==null) return;
            CheckNeedOutRoom();
            if (_self.IsOut)
            {
                _curGun.StopFire(); 
                if (_gunLocker != null) _gunLocker.UnLockFish(_self);
                return;
            } 
            _hasLocked = HasLocked();
            FindTarget(); 
            ChangeGun(); 
        }

        void LateUpdate()
        {   
            _curGun = _self.GunInst;
            if (_curGun==null) return;
            if (_self.IsOut) return; 
            _hasLocked = HasLocked(); 
            RotateGun();
            ChangeShotSpeed(); 
            Fire();     
        }

        private float _passSpeedTime = 0;
        private float _changeSpeedTime = 0;
        //改变速度 todo 按需变化
        private void ChangeShotSpeed()
        {
            _changeGunPassTime += Time.deltaTime;
            if (_changeGunPassTime < _changeGunTime) return;
            _changeGunTime = Random.Range(7, 20);
            _curGun.IsFastFirable = Random.Range(1, 100) > 50;
        }

        /// <summary>
        /// 开火
        /// </summary>
        void Fire()
        {  
            _curGun.Fire(_target != null || _hasLocked);
        }

        private int _targetWeight = 50;
        /// <summary>
        /// 寻找目标
        /// </summary>
        private void FindTarget()
        { 
            if (_target != null || _hasLocked) return;
            if (!_curGun.Fireable)
            {
                if (_gunLocker != null) _gunLocker.UnLockFish(_self);
                return;
            }
            if (_gunLocker == null || Random.Range(1, 100) > _targetWeight)
            {  
                if (_gunLocker != null) _gunLocker.UnLockFish(_self);
                var fishs = FindObjectsOfType<Fish>(); 
                var count = fishs.Length;
                if (count > 0)
                {
                    var random = Random.Range(0, count - 1);
                    _target = fishs[random].transform; 
                    _targetWeight++; 
                }
            }
            else
            {
                _target = null;
                _gunLocker.LockFish(_self); 
                _targetWeight--; 
            } 
        }

        /// <summary>
        ///  转炮
        /// </summary>
        void RotateGun()
        {
            var gun = _curGun.TsGun;
            var oldAngles = gun.localEulerAngles;
            if (!_hasLocked)
            {
                if (_target == null) return; 
                var upToward = (_target.position) - gun.position;
                upToward.z = 0F;
                gun.rotation = Quaternion.Slerp(gun.rotation, Quaternion.FromToRotation(Vector3.up, upToward), 10F * Time.deltaTime);
            }

            var angles = gun.localEulerAngles; 
            if (angles.z > 80 && angles.z < 280)
            {
                gun.localEulerAngles = oldAngles; //rotat;
                _target = null;
            }
            else
            {
                gun.localEulerAngles = new Vector3(0, 0, angles.z);
            } 
        }
     
        private float _changeGunPassTime = 0;
        private float _changeGunTime = 0;
        //private int _curIndex = 0;
        /// <summary>
        /// 换炮 todo 按需变化
        /// </summary>
        void ChangeGun()
        {
            _changeGunPassTime += Time.deltaTime;
            if (_changeGunPassTime < _changeGunTime) return;
            _changeGunTime = Random.Range(5, 20);
            _changeGunPassTime = 0;

            var range = Random.Range(0, 10) > 4 ? 1 : -1 ;
            _self.ChangeGunStyle(range); 
        } 
         
        /// <summary>
        /// 是否有锁定
        /// </summary>
        /// <returns></returns>
        private bool HasLocked()
        {
            return _gunLocker != null && _gunLocker.IsPlayerLockingFish(_self.Idx);
        }
        private float _seatPassTime;
        private float _seatTime;
        /// <summary>
        /// 退出房间
        /// </summary>
        private void CheckNeedOutRoom()
        {
            if (_self.IsOut) return;
            var batterys = GameMain.Singleton.PlayersBatterys;
            if (batterys == null) return;
            if (curScoreData == null || curScoreData.Val <= batterys.MaxGunStyle)
            {
                if (Random.Range(0, 100) < 10)
                {
                    Display();
                    return;
                }
                var gmr = GameMain.Singleton;
                if (gmr != null)
                {
                    _self.GainScore(gmr.PlayersBatterys.MaxGunStyle * 10);
                }
            }
            _seatPassTime += Time.deltaTime;
            if (_seatPassTime < _seatTime) return;
            _seatPassTime = 0;
            Display();
        }

        public void Display()
        {
            _self.Display(false);
            GameMain.Singleton.RobotOut(Id);
        }

        public void Run(bool isStart,int nid)
        {
            enabled = isStart;
            if (isStart)
            {
                Id = nid;
                return;
            }
            if (_self == null) return;
            if (_curGun != null) _curGun.StopFire();
            if (_gunLocker != null) _gunLocker.UnLockFish(_self);
        }

    }
}
