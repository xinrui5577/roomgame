using Assets.Scripts.Game.FishGame.Common.Utils;
using UnityEngine;
using System.Collections;
using YxFramwork.Common;
using com.yxixia.utile.YxDebug;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    public class Gun : MonoBehaviour
    { 
        //定义
        public enum RotateState
        {
            Idle,
            Left,
            Right
        }

        public enum ControlMode
        {
            Manual,//手动控制
            Auto//自动瞄准
        }

        //变量
        public tk2dSpriteAnimator AniSpr_GunPot;
        public Transform TsGun;
        public tk2dTextMesh Text_NeedCoin;

        public Transform local_GunFire;
        public Transform local_EffectGunFire;
        public GameObject Prefab_GunFire;
        public Bullet Prefab_BulletNormal;

        public float RotateSpeed = 100F;//角度/秒
        public float RotateRrangeHalf = 90F;//角度 
        public float NormalSpeed = 0.32f; 
        private float _cooldownMultiFix = 0.32F;//发射冷却调整
        public int NumBulletLimit = 20;//子弹限制
        [System.NonSerialized]
        public int GunType_;
        [System.NonSerialized]
        public int NumBulletInWorld = 0;//在场面上的子弹数量

        [System.NonSerialized]
        public bool Fireable = true;//是否可发射
        [System.NonSerialized]
        public bool Rotatable = true;//是否可转动
        public int NumDivide = 1;//子弹分裂数目
    
        public AnimationCurve Curve_GunShakeOffset;
        public AudioClip Snd_Fire;
        public AudioClip Snd_Equip;


        private RotateState _rotateState; 

        private float _cooldownRemain = 0F;//冷却剩余时间
        private float _lastFireTime;//最后一次发炮的时间
        private bool _firing = false; //是否开火中
        private Player _owner;
        public Player Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }
        private bool _isFastFirale = false;//是否可以快速开火

        /// <summary>
        /// 加速开火
        /// </summary>
        public bool IsFastFirable
        {
            get { return _isFastFirale; }
            set
            {
                _isFastFirale = value;
                _cooldownMultiFix = _isFastFirale ? NormalSpeed * 0.6f : NormalSpeed;
            }
        }

        public bool IsAutoFire;//自动开火
        private bool _isFastFire = false;//快速开火状态
        private bool _mEffectShakeCoroLock = false;//效果是否在播放中
    

        public void CopyDataTo(Gun tar)
        {
            tar.RotateSpeed = RotateSpeed;
            tar.RotateRrangeHalf = RotateRrangeHalf; 
            tar.NumBulletLimit = NumBulletLimit;
            tar.NumBulletInWorld = NumBulletInWorld;
            tar.Fireable = Fireable; 
            tar._rotateState = _rotateState; 
            tar._cooldownRemain = _cooldownRemain;
            tar._lastFireTime = _lastFireTime;
            tar._firing = _firing; 
            tar._owner = _owner; 
            tar.Rotatable = Rotatable;   
            tar._isFastFire = _isFastFire;
            tar._cooldownMultiFix = _cooldownMultiFix;
            tar.NormalSpeed = NormalSpeed;
            tar.IsFastFirable = IsFastFirable;
            tar.IsAutoFire = IsAutoFire;
        }

        void Start()
        {
            _cooldownMultiFix = _isFastFirale ? NormalSpeed * 0.6f : NormalSpeed;
            _owner = transform.parent.parent.GetComponent<Player>();
            SetNeedCoinLabel(GameMain.Singleton.BSSetting.Dat_PlayersGunScore[_owner.Idx].Val);
            Text_NeedCoin.Commit();  
        } 

        public void StartFire()
        {  
            Fire(true);
        }

        /// <summary>
        /// 是否开火中
        /// </summary>
        /// <returns></returns>
        public bool IsFiring()
        {
            return _firing || IsAutoFire;
        }

        public void Fire(bool isShoot)
        {
            _firing = isShoot; 
        } 
     
        /// <summary>
        /// 停止所有开火
        /// </summary>
        public void StopFire()
        {
            Fire(false);  
        }
          
        private void LateUpdate()
        {
            CheckRotate();
            if (!Fireable) return;
            if (!IsFiring()) return; 
            if (NumBulletInWorld > NumBulletLimit) return; //子弹限制  
            if (Time.time - _lastFireTime < _cooldownMultiFix) return; //cd限制   
            //没币
            var gameMain = GameMain.Singleton;
            var bsSet = gameMain.BSSetting;
            var playerIndex = _owner.Idx;
            var curScore = bsSet.Dat_PlayersScore[playerIndex].Val;
            var useScore = bsSet.Dat_PlayersGunScore[playerIndex].Val;
            if (curScore < useScore) return;
            var locker = gameMain.Gunlocker;
            var isLock = locker != null && !locker.ModuleBullet.GetFireable(playerIndex);
            _owner.FireEffect(useScore, isLock);
            _lastFireTime = Time.time; 
        }
         
        /// <summary>
        /// 开火指令
        /// </summary>
        /// <param name="useScore">子弹分数</param>
        /// <param name="bulletId">子弹id</param>
        /// <param name="isLock"></param>
        public void OnFire(int useScore, bool isLock = false, int bulletId = -1)
        { 
            if (!_owner.ChangeScore(-useScore)) return;  
            //动画
            //AniSpr_GunPot.Play(AniSpr_GunPot.clipId, 0F);
            //AniSpr_GunPot.PlayFrom(AniSpr_GunPot.DefaultClip, 0F);
            if (!_mEffectShakeCoroLock)
                StartCoroutine(_Coro_Effect_GunShake());
            //效果
            ShowFireEffect(_owner.transform);
            var self = App.GameData.SelfSeat%6;
            if (_owner.Idx == self) MakeFireSound();
            var gdata = App.GetGameData<FishGameData>();
            if (gdata.EvtPlayerGunFired != null)
                gdata.EvtPlayerGunFired(_owner, this, useScore, isLock, bulletId);  
        }
          
        /// <summary>
        /// 发出射击声音
        /// </summary>
        public void MakeFireSound()
        {
            //音效
            Facade.Instance<MusicManager>().Play(Snd_Fire);
        }

        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="parent"></param>
        public void ShowFireEffect(Transform parent)
        {
            var gunfire = Instantiate(Prefab_GunFire);
            gunfire.transform.parent = parent;
            gunfire.transform.localScale = local_EffectGunFire.localScale;
            gunfire.transform.position = local_EffectGunFire.position;
            gunfire.transform.rotation = local_EffectGunFire.rotation;
        }

        public void AdvanceNeedScore()
        {  
            BackStageSetting bss = GameMain.Singleton.BSSetting;
            int gunScoreCurrent = bss.Dat_PlayersGunScore[_owner.Idx].Val;
            gunScoreCurrent += bss.ScoreChangeValue.Val;
            if (gunScoreCurrent > bss.ScoreMax.Val)
            {
                gunScoreCurrent = bss.GetScoreMin(); 
            }
            SetNeedCoinLabel(gunScoreCurrent);
            Text_NeedCoin.Commit();

            bss.Dat_PlayersGunScore[_owner.Idx].Val = gunScoreCurrent;//记录

            //音效 
            if (_owner.Idx == App.GameData.SelfSeat % 6)
            {
                Facade.Instance<MusicManager>().Play("changegun");
            }
        }

        public void SetNeedCoinLabel(int coin)
        {
            Text_NeedCoin.text = YxUtiles.GetShowNumberToString(coin);
        }

        /// <summary>
        /// 直接设置押分
        /// </summary>
        public void SetNeedScore(int newScore,int playerIndex)
        {
            BackStageSetting bss = GameMain.Singleton.BSSetting; 
            SetNeedCoinLabel(newScore);
            Text_NeedCoin.Commit(); 
            bss.Dat_PlayersGunScore[playerIndex].Val = newScore;//记录 
            //音效
            Facade.Instance<MusicManager>().Play("changegun");
        }

        public void RotateTo(RotateState dir)
        {
            _rotateState = dir;   
        }

        public void StopRotate()
        {
            _rotateState = RotateState.Idle;  
        }
     
        private void CheckRotate()
        {  
            if (!Rotatable) return;
            if (_rotateState == RotateState.Idle) return;
            var direct = _rotateState == RotateState.Left ? 1F : -1F;

//        TsGun.RotateAroundLocal(Vector3.forward, direct * Time.deltaTime * RotateSpeed * Mathf.Deg2Rad);
//        if (!(TsGun.localEulerAngles.z > RotateRrangeHalf) || !(TsGun.localEulerAngles.z < (-RotateRrangeHalf))) return; 
//        TsGun.RotateAroundLocal(Vector3.forward, (direct * RotateRrangeHalf - TsGun.localEulerAngles.z) * Mathf.Deg2Rad);  
            TsGun.RotateAroundLocal(Vector3.forward, direct * Time.deltaTime * RotateSpeed * Mathf.Deg2Rad);
            if (TsGun.localEulerAngles.z > RotateRrangeHalf && TsGun.localEulerAngles.z < (360F - RotateRrangeHalf))
            {
                if (direct > 0)
                {
                    TsGun.RotateAroundLocal(Vector3.forward, -1.0F * (TsGun.localEulerAngles.z - RotateRrangeHalf) * Mathf.Deg2Rad);
                }
                else
                {
                    TsGun.RotateAroundLocal(Vector3.forward, (360F - RotateRrangeHalf - TsGun.localEulerAngles.z) * Mathf.Deg2Rad);
                }
            }
        }

        public IEnumerator _Coro_Effect_GunShake()
        {
            _mEffectShakeCoroLock = true;
            float time = 0.1F;
            float elapse = 0F;
            Transform tsAniGun = AniSpr_GunPot.transform;
            Vector3 oriPos = tsAniGun.localPosition;
            while (elapse <time)
            {
                tsAniGun.localPosition = oriPos + (Curve_GunShakeOffset.Evaluate(elapse/time)) *(tsAniGun.localRotation * Vector3.up ) ;

                elapse += Time.deltaTime;
                yield return 0;
            }
            tsAniGun.localPosition = oriPos;
            _mEffectShakeCoroLock = false;
        }

        public bool ChangeAutoFire()
        { 
            return IsAutoFire = !IsAutoFire;
        }

        public bool ChangeQuicken()
        { 
            return IsFastFirable = !IsFastFirable;
        }

        public void Turn(Quaternion quaternion)
        {
            var old = TsGun.localEulerAngles;
            TsGun.rotation = quaternion;
            var angles = TsGun.localEulerAngles;
            if (angles.z > 90 && angles.z < 270)
            {
                angles = old;
            }
            angles.x = angles.y = 0;
            TsGun.localEulerAngles = angles;
        }

        public void Change()
        {
            if (Snd_Equip != null) return;
            Facade.Instance<MusicManager>().Play(Snd_Equip);
        }
    }
}
