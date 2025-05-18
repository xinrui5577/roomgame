using Assets.Scripts.Game.FishGame.Common.Brains;
using Assets.Scripts.Game.FishGame.ChessCommon;
using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Fishs;
using JetBrains.Annotations;
using Sfs2X.Entities.Data;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    public class Player : MonoBehaviour
    {
        private bool _isStart;
        private int _curType = -1;
        [SerializeField, UsedImplicitly] 
        private Text _userNameLabel;
        /// <summary>
        /// 比分牌
        /// </summary>
        [SerializeField, UsedImplicitly]
        private tk2dTextMesh _boardText;
        private bool _isInit;
        private ISFSObject _data;

        public bool IsNpc;

        public int BulletNum;
        /// <summary>
        /// 是否离开
        /// </summary>
        public bool IsOut;

        public string Username;
        

        [System.NonSerialized] public int Idx;
        [System.NonSerialized] public int CtrllerIdx;

        public Transform GunPlatform;
        public Gun GunInst;
        public Gun[] Prefab_Guns; //待换枪,对应GunType
        public Gun[] Prefab_LiziGuns; //待换枪,对应离子GunType

        public tk2dTextMesh Text_Score;
        public CoinStackManager Ef_CoinStack; //金币位置
        public FlyingCoinManager Ef_FlyCoin;
        public ParticleSystem Prefab_ParChangeGun;
        public Color ColorType;
        public tk2dSprite Spr_GunBottom; //炮台动画
        public GameObject CtrBtns;
        public GameObject SelfMark;

        [System.NonSerialized] public bool CanChangeScore = true; //是否可押分  

        [System.NonSerialized] public bool CanCurrentScoreChange = true; //

        private uint mScoreChangeLowest = 0; //最低分数值
        //private Player_FishLocker mFishLocker;
        private HitProcessor _hitProcessor;
        public Transform KillBlueSharkPos;

        private ISFSObject _gameData;

        public ISFSObject GameData
        {
            get { return _gameData; }
            set
            {
                _gameData = value;
                _hitProcessor.SetPlayerData();
            }
        }

        public HitProcessor HitProcessor
        {
            get { return _hitProcessor; }
        }
         
        void Awake()
        {
            _isInit = true;
        }
        // Use this for initialization
        private void Start()
        {
            var self = App.GameData.SelfSeat % 6;
            Ef_FlyCoin.PlaySound = Idx == self;
            _isStart = true;
            if (_curType > 0)
            {
                ChangeGun(_curType);
            }
        }

        public void CreatGun()
        {
            var gt = 0;
            if (GunInst != null) Destroy(GunInst.gameObject);
            var guns = GetGunArsenal();
            GunInst = Instantiate(guns[gt]);
            GunInst.GunType_ = gt;
            GunInst.Owner = this;
            GunInst.transform.parent = GunPlatform;
            GunInst.transform.localPosition = Vector3.zero;
            GunInst.transform.localRotation = Quaternion.identity;
        }

        private void SetPlayerData(bool isNpc,int nid)
        {
            var bss = GameMain.Singleton.BSSetting;
            if (_hitProcessor != null) Destroy(_hitProcessor);
            _hitProcessor = isNpc ? gameObject.AddComponent<HitProcessor>() : gameObject.AddComponent<HitProcessorPlayer>();
            _hitProcessor.SetHitPlayer(this);
            bss.Dat_PlayersScore[Idx].Val = 0;
            var ai = gameObject.GetComponent<Ai>() ?? gameObject.AddComponent<Ai>();
            ai.Run(isNpc, nid);
        }

        public void BuyCoin(int value)
        {
            _hitProcessor.BuyCoin(value);
            GainScore(value);
        }

        public void RetrieveCoin()
        {
            _hitProcessor.Retrieve();
            SetScore(0);
        }

        public void OnGameDataRecv(ISFSObject data)
        {
            _hitProcessor.OnServerData(data);
        }

        public void HitFish(Bullet bullet, Fish fish)
        {
//            YxDebug.LogWrite("ProcessHit【{0}】", null, bullet.Score * fish.OddBonus * bullet.FishOddsMulti);
//            YxDebug.LogWrite("HitFish【{0}】", null, _hitProcessor);
            _hitProcessor.ProcessHit(bullet, fish);
        }

        /// <summary>
        /// 改变分数(面板分数)
        /// </summary>
        /// <param name="change"></param>
        /// <returns>改变是否成功</returns>
        public bool ChangeScore(int change)
        {
            //if (!CanChangeScore)
            //    return false;

            var bss = GameMain.Singleton.BSSetting; //
            if (bss.Dat_PlayersScore[Idx].Val + change < mScoreChangeLowest)
                return false; 
            bss.Dat_PlayersScore[Idx].Val += change;
            var gdata = App.GetGameData<FishGameData>();
            if (gdata.EvtPlayerScoreChanged != null)
                gdata.EvtPlayerScoreChanged(this, bss.Dat_PlayersScore[Idx].Val, change);

            //面板分数
            // Text_Score.text = bss.Dat_PlayersScore[Idx].Val.ToString();
            //Text_Score.Commit();
            return true;
        }

        public bool SetScore(int value)
        {
            BackStageSetting bss = GameMain.Singleton.BSSetting;
            if (value < mScoreChangeLowest)
                return false;
            bss.Dat_PlayersScore[Idx].Val = value; 
            return true;
        }

        /// <summary>
        /// 赢得的分数(不放入面板)
        /// </summary>
        /// <param name="numWon"></param>
        public void WonScore(int numWon)
        {
            GameMain.Singleton.BSSetting.Dat_PlayersScoreWon[Idx].Val += numWon;
            var gdata = App.GetGameData<FishGameData>();
            if (gdata.EvtPlayerWonScoreChanged != null)
                gdata.EvtPlayerWonScoreChanged(this, GameMain.Singleton.BSSetting.Dat_PlayersScoreWon[Idx].Val);
        }

        /// <summary>
        /// 玩家赢得币,综合ChangeScore和WonScore
        /// </summary>
        /// <param name="scoreTotalGetted"></param>
        /// <param name="fishodd"></param>
        /// <param name="bulletScore"></param>
        public void GainScore(int scoreTotalGetted, int fishodd, int bulletScore)
        {
            var bss = GameMain.Singleton.BSSetting;
            switch (bss.OutBountyType_.Val)
            {
                case OutBountyType.OutTicketButtom:
                case OutBountyType.OutCoinButtom:
                    ChangeScore(scoreTotalGetted);
                    break;
                case OutBountyType.OutTicketImmediate:
                case OutBountyType.OutCoinImmediate:
                    WonScore(scoreTotalGetted);
                    break;
            }
        }

        public void GainScore(int scoreTotalGetted)
        {
            GainScore(scoreTotalGetted, 2, 10);
        }

        public Gun[] GetGunArsenal()
        {
            return PowerType == GunPowerType.Lizi ? Prefab_LiziGuns : Prefab_Guns;
        }

        /// <summary>
        /// 换枪
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool ChangeGun(int type)
        {
            _curType = type;
            if (!_isStart) return false;
            if (type > 4)
            {
                Debug.LogError(type);
            }
            var guns = GetGunArsenal();
            var g = Instantiate(guns[type]);
            g.GunType_ = type;

            g.transform.parent = GunPlatform;
            g.transform.localPosition = GunInst.transform.localPosition;
            g.transform.localRotation = GunInst.transform.localRotation;

            //方位
            g.TsGun.transform.localPosition = GunInst.TsGun.transform.localPosition;
            g.TsGun.transform.localRotation = GunInst.TsGun.transform.localRotation;

            GunInst.CopyDataTo(g);
            Destroy(GunInst.gameObject);
            GunInst = g; 

            //效果
            StartCoroutine(_Coro_Effect_ChangeGun(g));

            //音效
            var self = App.GameData.SelfSeat % 6;
            if (Idx == self)
            {
                g.Change(); 
            }
            var gdata = App.GetGameData<FishGameData>();
            if (gdata.EvtPlayerGunChanged != null)
                gdata.EvtPlayerGunChanged(this, g);
            return true;
        }

        public GunPowerType PowerType { get;private set; }

        public bool ChangeGun(int lvType, GunPowerType pwrType)
        {
            PowerType = pwrType;
            return ChangeGun(lvType);
        }

        private IEnumerator _Coro_Effect_ChangeGun(Gun newGun)
        {
            ParticleSystem ef_particle = null;
            if (Prefab_ParChangeGun != null)
            {
                ef_particle = Instantiate(Prefab_ParChangeGun) as ParticleSystem;
                ef_particle.transform.parent = newGun.transform;
                ef_particle.transform.localPosition = new Vector3(0F, -18.55325184F, -10F);
                ef_particle.transform.localRotation = Quaternion.identity;
                ef_particle.Play(true);

            }
            float time = 0.1F;
            float elapse = 0F;
            Transform tsAniGun = newGun.AniSpr_GunPot.transform;
            //Vector3 oriPos = tsAniGun.localPosition;
            float scale = 1F;
            while (elapse < time)
            {
                if (tsAniGun == null) //同时两个换枪指令会使延时失效
                    break;
                //tsAniGun.localPosition = oriPos + tsAniGun.localRotation * Vector3.up *(Curve_GunShakeOffset.Evaluate(elapse/time));
                scale = 1F + 0.5F*(1F - elapse/time);
                tsAniGun.localScale = new Vector3(scale, scale, scale);

                elapse += Time.deltaTime;
                yield return 0;
            }

            if (tsAniGun != null)
                tsAniGun.localScale = Vector3.one;

            if (ef_particle != null)
            {
                yield return new WaitForSeconds(3F);
                if (ef_particle != null)
                    Destroy(ef_particle.gameObject);
            }

        }

        //GunType mCurGunType = GunType.Normal;
        //void OnGUI()
        //{
        //    if (Idx == 0 && GUILayout.Button("change gun"))
        //    {
        //        if (mCurGunType == GunType.Normal)
        //        {
        //            ChangeGun(GunType.Lizi);
        //            mCurGunType = GunType.Lizi;
        //        }
        //        else if (mCurGunType == GunType.Lizi)
        //        {
        //            ChangeGun(GunType.Normal);
        //            mCurGunType = GunType.Normal;
        //        }
        //    }
        //}


        public void ScoreUp(bool isMultiTen)
        {
            BackStageSetting bss = GameMain.Singleton.BSSetting;

            int numCoin = 10*(isMultiTen ? 10 : 1);
            int score = bss.InsertCoinScoreRatio.Val*numCoin;

            //判断是否爆机
            if (bss.Dat_PlayersScore[Idx].Val + score >= Defines.NumScoreUpMax)
            {
                return;
            }
            ChangeScore(score);

            //上分
            bss.His_CoinUp.Val += numCoin;
            bss.His_GainCurrent.Val += numCoin;
            bss.UpdateGainCurrentAndTotal();

            //声音
//            GameMain.Singleton.SoundMgr.PlayOneShot(GameMain.Singleton.SoundMgr.SndShangFen);
//            Facade.Instance<MusicManager>().Play("onscore");
        }

        public void ChangeNeedScore(int newScore)
        {
            var main = GameMain.Singleton;
            if (main == null) return;
            var bss = main.BSSetting;
 
            if (GunInst == null) CreatGun();
            if (GunInst != null)
            {
                GunInst.SetNeedScore(newScore, Idx);
                GunInst.NormalSpeed = Defines.FireSpeed;
            }
            bss.Dat_PlayersGunScore[Idx].Val = newScore;
            var preType = main.PlayersBatterys.GunNeedScoreToLevelType(newScore);
            On_GunLevelTypeChanged(preType);
        }

        /// <summary>
        /// 押分改变(自身触发)
        /// </summary>
        /// <param name="curType"></param>
        private void On_GunLevelTypeChanged(int curType)
        {
            ChangeGun(curType);
        }

//        /// <summary>
//        /// 是否有人
//        /// </summary>
//        public void SetHasUser(bool has)
//        {
//            IsOut = !has;
//            GunPlatform.gameObject.SetActive(has);
//            _boardText.gameObject.SetActive(has);
//            var locker = GetComponent<Player_FishLocker>();
//            if (locker != null) locker.TsTargeter.gameObject.SetActive(has);
//        }

        private void Update()
        {
            if (_boardText == null) return;
            if (GameMain.Singleton == null) return;
            var coin = GameMain.Singleton.BSSetting.Dat_PlayersScore[Idx].Val;
            SetCoinLabel(coin);
        }

        /// <summary>
        /// 开火效果
        /// </summary>
        /// <param name="bulletScore">子弹分数</param>
        /// <param name="isLock">是否为锁鱼子弹</param>
        public void FireEffect(int bulletScore, bool isLock = false)
        {
            _hitProcessor.FireEffect(bulletScore, isLock);
        }

        public int CurGunStylesIndex;
        public void ChangeGunStyle(int fluctuate)
        {
            var newScore = GameMain.Singleton.PlayersBatterys.GetGunStyle(ref CurGunStylesIndex, fluctuate);
            ChangeNeedScore(newScore);
        }

        /// <summary>
        /// 向上切换炮的样式
        /// </summary>
        public void ChangePriorGunStyle()
        {
            ChangeGunStyle(-1);
        }

        /// <summary>
        /// 向下切换炮的样式
        /// </summary>
        public void ChangeNextGunStyle()
        {
            ChangeGunStyle(1);
        }
             
        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
         
        public void Init(ISFSObject data,bool isRobot = false)
        {
            IsNpc = isRobot;
            _data = data;
            Fresh();
        }

        private void Fresh()
        {
            if (_data == null) return;
            if (!_isInit) return;
            Display();
            var nid = _data.ContainsKey("nid")?_data.GetInt("nid"):-1;
            SetPlayerData(IsNpc, nid);
            CreatGun();
            //设置名字
            Username = _data.GetUtfString(RequestKey.KeyName);
            _userNameLabel.text = Username;
            //设置金币
            var coin = _data.GetInt(RequestKey.KeyCoin);
            BuyCoin(coin);
            SetCoinLabel(coin);
        }

        public void SetCoinLabel(long coin)
        {
            _boardText.text = YxUtiles.GetShowNumberToString(coin);
        }

        public void Display(bool isShow=true)
        {
            gameObject.SetActive(isShow);
        }

        public bool IsHide()
        {
            return !gameObject.activeSelf;
        }

        public void SetColor(Color colorType)
        {

        }

        public void ShowCtrBtn(bool isShow=true)
        {
            if (CtrBtns != null) CtrBtns.SetActive(isShow);
        }

        public void DisplaySelfMark(bool isShow = false)
        {
            if (SelfMark == null) { return;}
            SelfMark.SetActive(isShow);
        }
    }
}
