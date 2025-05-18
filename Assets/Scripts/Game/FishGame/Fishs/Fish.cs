using System.Collections.Generic;
using Assets.Scripts.Game.FishGame.Common.Brains.FishAI;
using Assets.Scripts.Game.FishGame.Common.Compements;
using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.Common.external.NemoPoolGOs;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.FishGame.Fishs
{
    public class Fish : MonoBehaviour/* ,IPoolObj*/
    {
        public string FishName;
        public int CurCount;
        /// <summary>
        /// 鱼死亡事件
        /// </summary>
        public GameMain.EventFishKilled EvtFishKilled; 
        /// <summary>
        /// 赔率  
        /// </summary>
        public int Odds = 1;
        /// <summary>
        /// 用于奖励的赔率(有hitprocess赋值,该值应该由kill传入,因为不想复杂化代码,而做的临时方案)
        /// </summary>
        [System.NonSerialized]
        public int OddBonus = 1;
        /// <summary>
        /// 死亡动画持续时间(默认:1.35秒)
        /// </summary>
        [System.NonSerialized]
        public float TimeDieAnimation = 1.38F;
        /// <summary>
        /// 死后特效
        /// </summary>
        public GameObject TriggerEffect; 
        /// <summary>
        /// 
        /// </summary>
        public int TypeIndex = 0;
        /// <summary>
        /// 攻击类型
        /// </summary>
        public HittableType HittableType = HittableType.Normal; 

        public bool IsLockable = true;//是否可被锁定
        public string LockSpriteName; 
        //public int FishTypeIdx; 
        //唯一标识,awake处复制
        public uint ID
        {
            get
            {
                if (_mId < 1)
                {
                    _mId = _mIdGenerateNow;
                    ++_mIdGenerateNow;
                    if (_mIdGenerateNow == 0)//保证不存在0的ID
                        ++_mIdGenerateNow;
                }
                return _mId;
            }
        }

        public GameObject Prefab_GoAniSwim;
        public GameObject Prefab_GoAniDead;//pre,漏写

        public AudioClip[] Snds_Die;
        public AudioClip Snds_born;
        public GameObject Back;

        [System.NonSerialized]
        public bool Attackable = true;//是否可攻击

        protected Transform MTs;
        private Renderer _mRenderer;
        private uint _mId = 0;
        private static uint _mIdGenerateNow = 1;// 用于计算当前鱼id
        private Swimmer _mSwimmer;
        protected tk2dSpriteAnimator MAnimationSprite;//mGoAniSprite的tk2dAnimatedSprite或者其子对象的首个tk2dAnimatedSprite
        protected GameObject MGoAniSprite;

        public Swimmer swimmer
        {
            get
            {
                if (_mSwimmer == null)
                    _mSwimmer = GetComponent<Swimmer>();
                return _mSwimmer;
            }
        }

        public bool VisiableFish
        {
            set
            {
                if (_mRenderer == null)
                {
                    _mRenderer = GetComponentInChildren<Renderer>();
                }
                _mRenderer.enabled = value;
            } 
        }

        public virtual tk2dSpriteAnimator AniSprite
        {
            get
            {
                if (MAnimationSprite == null)
                {
//                    mGoAniSprite = Pool_GameObj.GetObj(Prefab_GoAniSwim);
//                    if(mGoAniSprite != null)mGoAniSprite.SetActive(true);
//                    mAnimationSprite = mGoAniSprite.GetComponent<tk2dSpriteAnimator>();
//                    if (mAnimationSprite == null)
//                        mAnimationSprite = mGoAniSprite.GetComponentInChildren<tk2dSpriteAnimator>();
//                    var renderers = mGoAniSprite.GetComponentsInChildren(typeof(Renderer));
//                    foreach (var r in renderers)
//                    {
//                        ((Renderer)r).enabled = true;
//                    }
//
//                    var tsAni = mGoAniSprite.transform;
//                    tsAni.parent = transform;
//                    tsAni.localPosition = Vector3.zero;
//                    tsAni.localRotation = Quaternion.identity;
                    MGoAniSprite = CreateModel(Prefab_GoAniSwim, out MAnimationSprite, Vector3.zero, Colour,transform);
//                    _shadowObj = CreateModel(Prefab_GoAniSwim, out _shadowAnim, ShadowPos, new Color(0, 0, 0, 0.6f),transform);
                    if (_shadow == null) _shadow = YxShadow.AddShadow(MGoAniSprite); //mGoAniSprite.AddComponent<YxShadow>();
                    _shadow.SetShadowModel(Prefab_GoAniSwim, MGoAniSprite.transform);
                } 
//                mAnimationSprite.GetComponent<tk2dSprite>().color = Colour; 
                return MAnimationSprite;
            }
        }
        public void CopyDataTo(Fish tar)
        {
        
            tar.Attackable = Attackable;
            tar._mId = _mId; 
        }

        private YxShadow _shadow;
        void Awake()
        { 
            MTs = transform;
            var id = ID;
            swimmer.EvtSwimOutLiveArea += Handle_SwimOutLiveArea;
            ++GameMain.Singleton.NumFishAlive;
            MAnimationSprite = AniSprite;//调用一下初始化函数  
            var gdata = App.GetGameData<FishGameData>();
            if (gdata!=null && gdata.EvtFishInstance != null)
                gdata.EvtFishInstance(this);
            OnAwake();
        }
          
        void Start()
        {
            if (Snds_born != null)
            {
                Facade.Instance<MusicManager>().Play(Snds_born);
            }
            SetFishBack();
            var co = GetComponent<BoxCollider>();
            if (co != null)
            {
                var si = co.size;
                si.z = 1500;
                co.size = si;
            }
            OnStart();
        }

        protected virtual void SetFishBack()
        {
            if (Back == null) return;
            var back = Instantiate(Back);
            var bts = back.transform;
            bts.parent = transform;
            bts.localPosition = new Vector3(0, 0, swimmer.SwimDepth + 0.01f);
            bts.localRotation = Quaternion.identity;
            bts.localScale = Vector3.one;
            var shadow = YxShadow.AddShadow(gameObject);
            shadow.SetShadowModel(Back,back.transform);
        }

        protected virtual void OnStart(){}

        protected virtual void OnAwake(){}
         
        public Color Colour = Color.white;
  
        void Handle_SwimOutLiveArea()
        {
            if (!Attackable) return;
            Attackable = false;
            Clear();
        }

        /// <summary>
        /// 清除,从屏幕上消失
        /// </summary>
        public void Clear()
        { 
            if (GameMain.Singleton != null) --GameMain.Singleton.NumFishAlive;
            var gdata = App.GetGameData<FishGameData>();
            if (gdata.EvtFishClear != null) gdata.EvtFishClear(this);
            Attackable = false;
            _mId = 0;
            if(_shadow!=null)_shadow.Clear();
            Pool_GameObj.RecycleGO(Prefab_GoAniSwim, MGoAniSprite);
            MGoAniSprite.SetActive(false);
            var tsAniSwim = MGoAniSprite.transform;
            tsAniSwim.position = new Vector3(1000F, 0F, 0F);
            tsAniSwim.rotation = Quaternion.identity;
            tsAniSwim.localScale = Vector3.one;
            MGoAniSprite = null;
            MAnimationSprite = null;
            App.GetGameData<FishGameData>().RmoveFish(TypeIndex);
            Destroy(gameObject); 
        }

        public virtual int GetFishOddBonus(Bullet bullet)
        {
            OddBonus = HitProcessor.GetFishOddBonus(bullet, this, this); ; //显示倍率
            return OddBonus;
        }

        /// <summary>
        /// 被打击
        /// </summary> 
        /// <param name="bullet"></param>
        /// <param name="totalOdd"></param> 
        public List<FishOddsData> BeHit(Bullet bullet,int totalOdd)
        {
            if (!Attackable) return null;
            var dataFish = new FishOddsData(ID, totalOdd); 
            return OnBeHit(bullet, dataFish);
        }

        /// <summary>
        /// 碰撞
        /// </summary>
        /// <param name="bullet"></param>
        /// <param name="oddsData"></param>
        protected virtual List<FishOddsData> OnBeHit(Bullet bullet, FishOddsData oddsData)
        { 
            var hitProcessocr = bullet.Owner.HitProcessor;
            if (hitProcessocr == null) return null;
            var kill = hitProcessocr.OneBulletKillFish(bullet.Score, oddsData); //todo 待删 服务器控制
            var list = new List<FishOddsData>();
            if (kill) list.Add(oddsData);
            return list;
        }

        /// <summary>
        /// 死亡技能特效
        /// </summary>
        /// <param name="killer">击杀者</param>
        /// <param name="bulletScore">子弹分数</param>
        /// <param name="rate">小鱼倍数</param>
        /// <param name="bulletOddsMulti">能量炮倍数</param>
        /// <param name="isLockFishBullet">是否锁鱼子弹</param>
        /// <param name="reward">应该获取的金币</param>
        public void DieSkillEffect(Player killer, int bulletScore, int rate, int bulletOddsMulti, bool isLockFishBullet,int reward)
        {
            OnDieSkillEffect(killer, bulletScore, OddBonus, bulletOddsMulti,reward);
            OnHitFishEvent(killer, this, bulletScore, rate, isLockFishBullet, reward);
        }

        /// <summary>
        /// 碰鱼事件
        /// </summary>
        /// <param name="killer"></param>
        /// <param name="fish"></param>
        /// <param name="bulletScore"></param>
        /// <param name="rate"></param>
        /// <param name="isLockFishBullet"></param>
        /// <param name="reward"></param>
        protected virtual void OnHitFishEvent(Player killer, Fish fish, int bulletScore, int rate, bool isLockFishBullet, int reward)
        {
            HitProcessor.HitFishEvent(killer, this, bulletScore, rate, isLockFishBullet);  
        }

        /// <summary>
        /// 死亡技能特效
        /// </summary>
        /// <param name="killer"></param>
        /// <param name="bulletScore"></param>
        /// <param name="fishOddBonus"></param>
        /// <param name="bulletOddsMulti"></param>
        /// <param name="reward"></param>
        protected virtual void OnDieSkillEffect(Player killer, int bulletScore, int fishOddBonus, int bulletOddsMulti, int reward)
        {
            Kill(killer, bulletScore, fishOddBonus, bulletOddsMulti, 0, reward); //0.5秒间隔  
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="killer"></param>
        /// <param name="bulletScore"></param>
        /// <param name="fishOddBonus"></param>
        /// <param name="bulletOddsMulti"></param>
        /// <param name="delayVisiableAnimation"></param>
        /// <param name="reward">获取金币</param>
        public void Kill(Player killer, int bulletScore, int fishOddBonus, int bulletOddsMulti, float delayVisiableAnimation, int reward)
        {
            if (!Attackable) return; 
            if (EvtFishKilled != null)
                EvtFishKilled(killer, bulletScore, fishOddBonus, bulletOddsMulti, this,reward);
            var gdata = App.GetGameData<FishGameData>();
            if (gdata.EvtFishKilled != null)
                gdata.EvtFishKilled(killer, bulletScore, fishOddBonus, bulletOddsMulti, this,reward);
            Die(killer, delayVisiableAnimation, bulletOddsMulti);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="killer"></param>
        /// <param name="delay"></param>
        /// <param name="oddsMulti">子弹的离子炮 倍数</param>
        private void Die(Player killer, float delay, int oddsMulti)
        {
            Attackable = false;


            //消除碰撞框
            GetComponent<Collider>().enabled = false;
            //隐藏鱼
            var renderers = GetComponentsInChildren(typeof (Renderer));
            foreach (var r in renderers)
            {
                ((Renderer) r).enabled = false;
            }
            var delayTotal = delay + TimeDieAnimation;
            //播放死亡动画
            DeathAnimation(delayTotal);
            //飞币
            CoinFly(killer, oddsMulti, delayTotal);

            //声音
            Facade.Instance<MusicManager>().Play("getscore01");
            var self = App.GameData.SelfSeat % 6;
            if (killer.Idx == self) //Odds <= 10
            {
                if (Snds_Die != null && Snds_Die.Length != 0)
                {
                    Facade.Instance<MusicManager>().Play(Snds_Die[Random.Range(0, Snds_Die.Length)]);
                }
            }
            //除本对象
            Clear();
        }

        protected virtual void CoinFly(Player killer, int oddsMulti, float delayTotal)
        {
            var deadWorldPos = MTs.position;
            if (Odds != 0 && !killer.IsHide()) killer.Ef_FlyCoin.FlyFrom(deadWorldPos, oddsMulti * Odds, delayTotal);
        }

        /// <summary>
        /// 死亡动画
        /// </summary>
        /// <param name="delayTotal"></param>
        protected virtual void DeathAnimation(float delayTotal)
        {
            //播放死亡动画
            if (Prefab_GoAniDead == null) return;
            var goDieAnimation = Pool_GameObj.GetObj(Prefab_GoAniDead);
            goDieAnimation.GetComponent<tk2dSprite>().color = Colour;
            var goDieAniTs = goDieAnimation.transform;
            goDieAnimation.SetActive(true);

            goDieAniTs.parent = GameMain.Singleton.FishGenerator.transform;
            goDieAniTs.position = new Vector3(MTs.position.x, MTs.position.y, MTs.position.z);
            goDieAniTs.rotation = MTs.rotation;
            var dshadow = YxShadow.AddShadow(goDieAnimation);
            dshadow.SetRecycleShadowModel(Prefab_GoAniDead, goDieAniTs, delayTotal);
            var fishRecycleDelay = goDieAnimation.AddComponent<RecycleDelay>();
            fishRecycleDelay.delay = delayTotal;
            fishRecycleDelay.Prefab = Prefab_GoAniDead; 
        }

        public void ClearAi()
        {
            var fishAIs = GetComponents(typeof(IFishAI));
            var len = fishAIs.Length;
            for (var i = 0; i<len; ++i)
            {
                Destroy(fishAIs[i]);
            }
        }

        private static GameObject CreateModel(GameObject modelPerf,out tk2dSpriteAnimator animator,Vector3 posOff,Color color,Transform parentTs)
        {
            var model = Pool_GameObj.GetObj(modelPerf);
            model.SetActive(true);
            animator = model.GetComponent<tk2dSpriteAnimator>() ?? model.GetComponentInChildren<tk2dSpriteAnimator>();
            var crenderers = model.GetComponentsInChildren<Renderer>();
            foreach (var r in crenderers)
            {
                r.enabled = true;
            }
            var aniTs = model.transform;
            aniTs.parent = parentTs;
            aniTs.localPosition = posOff;
            aniTs.localRotation = Quaternion.identity;
            var tk2Ds = model.GetComponentsInChildren<tk2dSprite>();
            foreach (var tks in tk2Ds)
            {
                tks.color = color;
            }
            return model;
        } 
    }
}
