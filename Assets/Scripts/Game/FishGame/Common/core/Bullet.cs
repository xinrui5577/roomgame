using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Common.external.NemoPoolGOs;
using Assets.Scripts.Game.FishGame.Fishs;
using Assets.Scripts.Game.FishGame.Lite;
using UnityEngine;
using YxFramwork.Common;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    public class Bullet : MonoBehaviour,IPoolObj {
    
        public static readonly Vector3 ColliderOffsetZ = Vector3.forward * 3000F;
        public int Id = -1;
        public float Speed = 1F;
        public int Score = 1;
        public int FishOddsMulti = 1;//鱼赔率再加倍.(用于离子炮X2)
        public tk2dTextMesh Text_CoinNum;
    
        public float RadiusStandardBoom = 0.175F;//爆炸最大半径,标准值: 普通子弹:0.175F, 粒子炮:0.35
        public float ScaleCollider = 0.5F;//网碰撞默认大小,当IsScaleWeb为true是无效,跟gamemain设置有关
        public GameObject Prefab_GoAnisprBullet;//子弹sprite prefab
        public StringSet Prefab_SpriteNameSet;//子弹spr名字集合
        [System.NonSerialized]
        public Player Owner;//所属player
        [System.NonSerialized]
        public Rect MoveArea;//移动区域
        /// <summary>
        /// 是否失效
        /// </summary>
        public bool isAbate; 
    
    
        private Fish _mTargetFish;//锁定鱼
        [System.NonSerialized]
        public bool IsLockingFish = false;//是否锁定鱼子弹
        private Vector3 _mPosSaved;
        private bool _mIsDestroyed;
         
        private Transform _mTs;
        private GameObject _mBulletGo;
 

        private GameObject mPrefab;

        public bool IsBezierWhenLocking = true;
        private bool mBezierMoveLR;//left = false ; right = true;
        private Vector3 mSLineCurPostion;//跟踪弹当前的位置,用于计算移动百分比
        //private Vector3 mSLineDistance;//跟踪弹方向

        public GameObject Prefab
        {
            get { return mPrefab; }
            set { mPrefab = value; }

        }
        void Awake()
        {
            _mTs = transform;
        }

        void CopyDataTo(Bullet tar)
        {
            tar.Speed = Speed;
            tar.Score = Score;
            tar.FishOddsMulti = FishOddsMulti;
            tar.RadiusStandardBoom = RadiusStandardBoom;
            //tar.IsScaleWeb = IsScaleWeb;
            tar.ScaleCollider = ScaleCollider; 
            tar._mIsDestroyed = _mIsDestroyed;
            tar.MoveArea = MoveArea;
        }

        public void On_Reuse(GameObject prefab)
        {
            gameObject.SetActive(true);
            prefab.GetComponent<Bullet>().CopyDataTo(this);
 
        }

        public void On_Recycle()
        {
            gameObject.SetActive(false);
            //gameObject.active = false;
            //gameObject.SetActiveRecursively(false);

            if (_mBulletGo != null)
            {
                _mBulletGo.SetActive(false);
                Pool_GameObj.RecycleGO(Prefab_GoAnisprBullet, _mBulletGo);

                _mBulletGo = null;
            }
            --Owner.GunInst.NumBulletInWorld;
            if (Owner.GunInst.NumBulletInWorld < 0)
                YxDebug.LogError("在场子弹数不能为负" + Owner.GunInst.NumBulletInWorld.ToString());
        } 
 
        public void SelfDestroy()
        { 
            if (_mIsDestroyed) return;
            var gdata = App.GetGameData<FishGameData>();
            if (gdata.EvtBulletDestroy != null)
                gdata.EvtBulletDestroy(this);

            Pool_GameObj.RecycleGO(null, gameObject);
            _mIsDestroyed = true;//Destroy不会立即使得OnTrigger失效,防止多次物理碰撞,
        }
 
        public void Fire(Player from, Fish tar,Quaternion dir)
        { 
            GameMain gm = GameMain.Singleton;
            _mTargetFish = tar;
            IsLockingFish = tar != null; 
            _mTs.rotation = dir;
            _mTs.parent = gm.TopLeftTs;
            var old = _mTs.localPosition;
            old.z = 0;
            _mTs.localPosition = old;
            Text_CoinNum.text = Score.ToString();
            Text_CoinNum.Commit();

            Owner = from;
            ++Owner.GunInst.NumBulletInWorld;

            BackStageSetting bss = gm.BSSetting;

            MoveArea = gm.WorldDimension;
 
            if (_mBulletGo == null)
            {
                _mBulletGo = Pool_GameObj.GetObj(Prefab_GoAnisprBullet);
                tk2dSpriteAnimator aniSprBullet = _mBulletGo.GetComponent<tk2dSpriteAnimator>();
                if (aniSprBullet != null)
                {
                    //aniSprBullet.clipId = aniSprBullet.GetClipIdByName(Prefab_SpriteNameSet.Texts[from.Idx % Prefab_SpriteNameSet.Texts.Length]);
                    //YxDebug.Log(Prefab_SpriteNameSet.Texts[from.Idx % Prefab_SpriteNameSet.Texts.Length]);
                    aniSprBullet.Play(Prefab_SpriteNameSet.Texts[from.Idx % Prefab_SpriteNameSet.Texts.Length]);
                    //aniSprBullet.PlayFrom("1",0.1F);
                }

                _mBulletGo.SetActive(true);
                _mBulletGo.transform.parent = _mTs;
                var pos = Vector3.zero;
                _mBulletGo.transform.localPosition = pos;
                pos.z -= 10;
                Text_CoinNum.transform.localPosition = pos;
                _mBulletGo.transform.localRotation = Quaternion.identity;  
            } 
        } 

        void Update () {
            if (_mIsDestroyed)
                return; 
            _mPosSaved = _mTs.position;  
            _mTs.position += Speed * Time.deltaTime * _mTs.up;
            var curPos = _mTs.position;
            if (IsLockingFish)
            {
                LockingDir();
            }
            if (curPos.y < MoveArea.yMin || curPos.y > MoveArea.yMax)
            {  
                _mTs.position = _mPosSaved;
                var euler = _mTs.localEulerAngles;
                euler.z = 180 - euler.z;
                _mTs.localEulerAngles = euler;
                IsLockingFish = false;
            }
            if (curPos.x < MoveArea.xMin || curPos.x > MoveArea.xMax)
            { 
                _mTs.position = _mPosSaved;
                var euler = _mTs.localEulerAngles;
                euler.z = -euler.z;
                _mTs.localEulerAngles = euler;
                IsLockingFish = false;
            } 
        }

        private void LockingDir()
        {
            if(_mTargetFish == null)return;
            var upToward = (_mTargetFish.transform.position) - transform.position;//loclLocal != null ? loclLocal.LockBulletPos :
            upToward.z = 0F;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.FromToRotation(Vector3.up, upToward),
                                              10F * Time.deltaTime); 
        }

        void OnTriggerEnter(Collider other)
        { 
            if (_mIsDestroyed) return;
            var fishFirst = other.GetComponent<Fish>();//被子弹击中的鱼
            if (fishFirst == null)
            {
                YxDebug.LogError("Fish在这里不可能是null!" + other.name);
                return;
            }
            if (IsLockingFish && _mTargetFish != null && _mTargetFish.ID != fishFirst.ID) return; //锁目标  
            SelfDestroy();
            Owner.HitFish(this, fishFirst);
            
        } 
    }
}
