using Assets.Scripts.Game.FishGame.Common.external.NemoFileIO;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    public class BackStageSetting : MonoBehaviour {
        /// <summary>
        /// 游戏难度 
        /// </summary>
        public GameDifficult _Def_GameDifficult = GameDifficult.Easy;
        public int _Def_CoinTicketRatio_Coin = 1;
        public int _Def_CoinTicketRatio_Ticket = 10;
        public int _Def_ScoreChangeValue = 10;
        public int _Def_ScoreMax = 1000;
        public int _Def_ScoreMin = 10;
        public OutBountyType _Def_OutBountyType = OutBountyType.OutCoinButtom;
        public bool _Def_IsBulletCrossWhenScreenNet = false;

        public int _Def_InsertCoinScoreRatio = 10;
        public ArenaType _Def_ArenaType = ArenaType.Small;
        public int _Def_CodePrintDay = 5;
        public bool _Def_IsViewCodebeatSuccess = true;
        public uint _Def_FormulaCode = 0xffffffff;

        public Language _Def_LanguageUsing = Language.Cn;
        public int _Digit_IdLine = 3;
        public int _Digit_IdTable = 8;
        public int _Digit_FormulaCode = 8;

        public bool _Def_ShowLanguage = true; 

        public static GameDifficult Def_GameDifficult
        {
            get
            {
                return GameMain.Singleton.BSSetting._Def_GameDifficult;
            }
        }
        public static int Def_CoinTicketRatio_Coin
        {
            get
            {
                return GameMain.Singleton.BSSetting._Def_CoinTicketRatio_Coin;
            }
        }
        public static int Def_CoinTicketRatio_Ticket
        {
            get
            {
                return GameMain.Singleton.BSSetting._Def_CoinTicketRatio_Ticket;
            }
        }
        public static int Def_ScoreChangeValue
        {
            get
            {
                return GameMain.Singleton.BSSetting._Def_ScoreChangeValue;
            }
        }
        public static int Def_ScoreMax
        {
            get
            {
                return GameMain.Singleton.BSSetting._Def_ScoreMax;
            }
        }
        public static int Def_ScoreMin
        {
            get
            {
                return GameMain.Singleton.BSSetting._Def_ScoreMin;
            }
        }
        public static OutBountyType Def_OutBountyType
        {
            get
            {
                return GameMain.Singleton.BSSetting._Def_OutBountyType;
            }
        } 
        public static bool Def_IsBulletCrossWhenScreenNet
        {
            get
            {
                return GameMain.Singleton.BSSetting._Def_IsBulletCrossWhenScreenNet;
            }
        }

        public static int Def_InsertCoinScoreRatio
        {
            get
            {
                return GameMain.Singleton.BSSetting._Def_InsertCoinScoreRatio;
            }
        }
        public static ArenaType Def_ArenaType
        {
            get
            {
                return GameMain.Singleton.BSSetting._Def_ArenaType;
            }
        }
        public static int Def_CodePrintDay
        {
            get
            {
                return GameMain.Singleton.BSSetting._Def_CodePrintDay;
            }
        }
        public static bool Def_IsViewCodebeatSuccess
        {
            get
            {
                return GameMain.Singleton.BSSetting._Def_IsViewCodebeatSuccess;
            }
        }
        public static uint Def_FormulaCode
        {
            get
            {
                return GameMain.Singleton.BSSetting._Def_FormulaCode;
            }
        }

        public static Language Def_LanguageUsing
        {
            get
            {
                return GameMain.Singleton.BSSetting._Def_LanguageUsing;
            }
        }
        public static int Digit_IdLine
        {
            get
            {
                return GameMain.Singleton.BSSetting._Digit_IdLine;
            }
        }
        public static int Digit_IdTable
        {
            get
            {
                return GameMain.Singleton.BSSetting._Digit_IdTable;
            }
        }
        public static int Digit_FormulaCode
        {
            get
            {
                return GameMain.Singleton.BSSetting._Digit_FormulaCode;
            }
        }

        public static bool Def_ShowLanguage
        {
            get
            {
                return GameMain.Singleton.BSSetting._Def_ShowLanguage;
            }
        }

        ///后台数据
        public PersistentData<GameDifficult, int> GameDifficult_;//游戏难度
        public PersistentData<int, int> CoinTicketRatio_Coin;//币票兑换率:币
        public PersistentData<int, int> CoinTicketRatio_Ticket;//币票兑换率:票
        public PersistentData<int, int> ScoreChangeValue;//押分切换值
        public PersistentData<int, int> ScoreMax;//最大押分
        public PersistentData<int, int> ScoreMin;//最小押分
        public PersistentData<OutBountyType, int> OutBountyType_;//即中即退类型
        public PersistentData<GunLayoutType, int> GunLayoutType_;//炮台布局
        public PersistentData<bool, bool> IsBulletCrossWhenScreenNet;//联屏时子弹是否穿过

        public PersistentData<int, int> InsertCoinScoreRatio;//投币分比率
        public PersistentData<ArenaType, int> ArenaType_;//场地类型
        public PersistentData<int, int> CodePrintDay;//打码天数(天)
        public PersistentData<bool, bool> IsViewCodebeatSuccess;//是否显示打码成功信息

        public PersistentData<bool, bool> CodePrintCurrentAction;//codePrint当前操作(false:归到总账,true:全部数据清0)
        public PersistentData<bool, bool> IsCodePrintClearAllData;//打码报账是否全部归零
        public PersistentData<bool, bool> IsNeedPrintCodeAtGameStart;//在开机时是否进入打码
        public PersistentData<int, int> TicketOutFragment;//本期出票的零头,不够一个币的部分..打码清空

        public PersistentData<Language, int> LaguageUsing;
        ///历史数据
        public PersistentData<int, int> His_GainTotal;//总盈利
        public PersistentData<int, int> His_GainPrevious;//上次盈利
        public PersistentData<int, int> His_GainCurrent;//本次盈利
        public PersistentData<int, int> His_CoinUp;//上分.(币)
        public PersistentData<int, int> His_CoinDown;//下分(币)
        public PersistentData<int, int> His_CoinInsert;//投币
        public PersistentData<int, int> His_CoinOut;//退币
        public PersistentData<int, int> His_TicketOut;//已退的票数
        public PersistentData<int, int> His_NumCodePrint;//打码次数

        public PersistentData<int, int> Dat_GainAdjustIdx;//抽放水索引(0-20) "保持现状","放水一千","放水二千","放水三千","放水四千","放水五千","放水八千","放水一万","放水两万","放水五万","放水十万" ,"抽水一千","抽水二千","抽水三千","抽水四千","抽水五千","抽水八千","抽水一万","抽水两万","抽水五万","抽水十万" 
        public PersistentData<uint, uint> Dat_FormulaCode;//公式码(十进制)
        public PersistentData<int, int> Dat_IdLine;//线号
        public PersistentData<int, int> Dat_IdTable;//台号
     
        public PersistentData<long, long> Dat_CodePrintDateTime;//打码(即延时)日期,dateTime.tick
        public PersistentData<uint, uint> Dat_RemoteDiffucltFactor;//远程难度系数
        ///系统设置
        public PersistentData<float, float> Dat_SoundVolum;//声音大小
        public PersistentData<float, float> Dat_BGMVolum;//背景音大小
        public PersistentData<bool, bool> Dat_GameShowLanguageSetup;//显示语言设置

        ///玩家数据
    
        public PersistentData<int, int>[] Dat_PlayersScore;//玩家分值
        public PersistentData<int, int>[] Dat_PlayersGunScore;//玩家当前炮的分值
        public PersistentData<int, int>[] Dat_PlayersScoreWon;//玩家已经赢得的分数(用于即使退奖)
        public PersistentData<int, int>[] Dat_PlayersBulletScore;//玩家子弹分数(用于断电退回分数)
        private PersistentData<bool, bool> mResetSign;//初始化标记(false表示需要reset,true表示不需要)

        /// <summary>
        /// 获得最少押分值
        /// </summary>
        /// <remarks>因为在设置中最小押分值可以大于最大押分值</remarks>
        /// <returns></returns>
        public int GetScoreMin()
        {
            return ScoreMin.Val < ScoreMax.Val ? ScoreMin.Val : ScoreMax.Val;
        }

        /// <summary>
        /// 获得剩余运行时间(分钟)
        /// </summary>
        /// <returns></returns>
        public int GetRemainRuntime()
        { 
            System.TimeSpan elapse = System.DateTime.Now - new System.DateTime(Dat_CodePrintDateTime.Val); 
            int nRemainRuntime = CodePrintDay.Val * 24 * 60 - (int)elapse.TotalMinutes;
            return nRemainRuntime < 0 ? 0 : nRemainRuntime;
        }

        /// <summary>
        /// 根据,上分,下分,投币,退币,退票 更新当期盈利和总盈利
        /// </summary>
        public void UpdateGainCurrentAndTotal()
        {
            //His_GainCurrent.Val = His_CoinUp.Val + His_CoinInsert.Val - His_CoinDown.Val - His_CoinOut.Val - His_TicketOut.Val * CoinTicketRatio_Coin.Val / CoinTicketRatio_Ticket.Val;
            His_GainTotal.Val = His_GainCurrent.Val + His_GainPrevious.Val;
        }
     
    
        public void TryNewPersistentDatas()
        {
       
            if (mResetSign == null) mResetSign = new PersistentData<bool, bool>("ResetSign");
        
            if (GameDifficult_ == null) GameDifficult_ = new PersistentData<GameDifficult, int>("GameDifficult_");
        
            if (CoinTicketRatio_Coin == null) CoinTicketRatio_Coin = new PersistentData<int, int>("CoinTicketRatio_Coin");
        
            if (CoinTicketRatio_Ticket == null) CoinTicketRatio_Ticket = new PersistentData<int, int>("CoinTicketRatio_Ticket");
            if (ScoreChangeValue == null) ScoreChangeValue = new PersistentData<int, int>("ScoreChangeValue");
            if (ScoreMax == null) ScoreMax = new PersistentData<int, int>("ScoreMax");
            if (ScoreMin == null) ScoreMin = new PersistentData<int, int>("ScoreMin");
        
            if (OutBountyType_ == null) OutBountyType_ = new PersistentData<OutBountyType, int>("OutBountyType_");
            if (GunLayoutType_ == null) GunLayoutType_ = new PersistentData<GunLayoutType, int>("GunLayoutType_");
            if (IsBulletCrossWhenScreenNet == null) IsBulletCrossWhenScreenNet = new PersistentData<bool, bool>("IsBulletCrossWhenScreenNet");
         
            if (InsertCoinScoreRatio == null) InsertCoinScoreRatio = new PersistentData<int, int>("InsertCoinScoreRatio");
            if (ArenaType_ == null) ArenaType_ = new PersistentData<ArenaType, int>("ArenaType_");
            if (CodePrintDay == null) CodePrintDay = new PersistentData<int, int>("CodePrintDay");
            if (IsViewCodebeatSuccess == null) IsViewCodebeatSuccess = new PersistentData<bool, bool>("IsViewCodebeatSuccess");
            if (IsCodePrintClearAllData == null) IsCodePrintClearAllData = new PersistentData<bool, bool>("IsCodePrintClearAllData");
            if (CodePrintCurrentAction == null) CodePrintCurrentAction = new PersistentData<bool, bool>("CodePrintCurrentAction");
        
            if (IsNeedPrintCodeAtGameStart == null) IsNeedPrintCodeAtGameStart = new PersistentData<bool, bool>("IsNeedPrintCodeAtGameStart");
        
            if (TicketOutFragment == null) TicketOutFragment = new PersistentData<int, int>("TicketOutFragment");
            if (LaguageUsing == null) LaguageUsing = new PersistentData<Language, int>("LaguageUsing");
            if (His_GainTotal == null) His_GainTotal = new PersistentData<int, int>("His_GainTotal");
            if (His_GainPrevious == null) His_GainPrevious = new PersistentData<int, int>("His_GainPrevious");
            if (His_GainCurrent == null) His_GainCurrent = new PersistentData<int, int>("His_GainCurrent");
            if (His_CoinUp == null) His_CoinUp = new PersistentData<int, int>("His_CoinUp");
            if (His_CoinDown == null) His_CoinDown = new PersistentData<int, int>("His_CoinDown");
            if (His_CoinInsert == null) His_CoinInsert = new PersistentData<int, int>("His_CoinInsert");
            if (His_CoinOut == null) His_CoinOut = new PersistentData<int, int>("His_CoinOut");
            if (His_TicketOut == null) His_TicketOut = new PersistentData<int, int>("His_TicketOut");
            if (His_NumCodePrint == null) His_NumCodePrint = new PersistentData<int, int>("His_NumCodePrint");
            if (Dat_GainAdjustIdx == null) Dat_GainAdjustIdx = new PersistentData<int, int>("Dat_GainAdjustIdx");
            if (Dat_FormulaCode == null) Dat_FormulaCode = new PersistentData<uint, uint>("Dat_FormulaCode");
            if (Dat_IdLine == null) Dat_IdLine = new PersistentData<int, int>("Dat_IdLine");
            if (Dat_IdTable == null) Dat_IdTable = new PersistentData<int, int>("Dat_IdTable"); 
            if (Dat_CodePrintDateTime == null) Dat_CodePrintDateTime = new PersistentData<long, long>("Dat_CodePrintDateTime");
            if (Dat_SoundVolum == null) Dat_SoundVolum = new PersistentData<float, float>("Dat_SoundVolum");
            if (Dat_BGMVolum == null) Dat_BGMVolum = new PersistentData<float, float>("Dat_BGMVolum");
        
            if (Dat_GameShowLanguageSetup == null) Dat_GameShowLanguageSetup = new PersistentData<bool,bool>("Dat_GameShowLanguageSetup");
        
            if (Dat_RemoteDiffucltFactor == null) Dat_RemoteDiffucltFactor = new PersistentData<uint, uint>("Dat_RemoteDiffucltFactor");
        
            if (Dat_PlayersScore == null)
            {
                Dat_PlayersScore = new PersistentData<int, int>[Defines.MaxNumPlayer];
                for (int i = 0; i != Defines.MaxNumPlayer; ++i)
                    Dat_PlayersScore[i] = new PersistentData<int, int>("Dat_PlayersScore" + i.ToString());
            }

            if (Dat_PlayersGunScore == null)
            {
                Dat_PlayersGunScore = new PersistentData<int, int>[Defines.MaxNumPlayer];
                for (int i = 0; i != Defines.MaxNumPlayer; ++i)
                    Dat_PlayersGunScore[i] = new PersistentData<int, int>("Dat_PlayersGunScore" + i.ToString());
            }
       
            if (Dat_PlayersScoreWon == null)
            {
                Dat_PlayersScoreWon = new PersistentData<int, int>[Defines.MaxNumPlayer];
                for (int i = 0; i != Defines.MaxNumPlayer; ++i)
                    Dat_PlayersScoreWon[i] = new PersistentData<int, int>("Dat_PlayersScoreWon" + i.ToString());
            }

            if (Dat_PlayersBulletScore == null)
            {
                Dat_PlayersBulletScore = new PersistentData<int, int>[Defines.MaxNumPlayer];
                for (int i = 0; i != Defines.MaxNumPlayer; ++i)
                    Dat_PlayersBulletScore[i] = new PersistentData<int, int>("Dat_PlayersBulletScore" + i.ToString());
            }
            if (!mResetSign.Val)//由标记得知游戏配置未初始化
            {
                FirstInitDatas();
                mResetSign.SetImmdiately(true);
            }
      
        }

        //void Awake()
        //{
         
        //    if (!mResetSign.Val)//由标记得知游戏配置未初始化
        //    {
        //        FirstInitDatas();
        //        mResetSign.SetImmdiately(true);
        //    }
        //}

        private void FirstInitDatas()
        { 
            GameDifficult_.SetImmdiately(Def_GameDifficult);
            CoinTicketRatio_Coin.SetImmdiately(Def_CoinTicketRatio_Coin);
            CoinTicketRatio_Ticket.SetImmdiately(Def_CoinTicketRatio_Ticket);
            ScoreChangeValue.SetImmdiately(Def_ScoreChangeValue);
            ScoreMax.SetImmdiately(Def_ScoreMax);
            ScoreMin.SetImmdiately(Def_ScoreMin);
            OutBountyType_.SetImmdiately(Def_OutBountyType);
        
            InsertCoinScoreRatio.SetImmdiately(Def_InsertCoinScoreRatio);
            ArenaType_.SetImmdiately(Def_ArenaType);
            CodePrintDay.SetImmdiately(Def_CodePrintDay);
            IsViewCodebeatSuccess.SetImmdiately(Def_IsViewCodebeatSuccess);
        
            Dat_GainAdjustIdx.SetImmdiately(GameOdds.CoinPresents[(int)Def_ArenaType]);
            Dat_FormulaCode.SetImmdiately(Def_FormulaCode);
            Dat_IdLine.SetImmdiately(388);
            Dat_IdTable.SetImmdiately(88111111);
            Dat_CodePrintDateTime.SetImmdiately(System.DateTime.Now.Ticks);
            Dat_RemoteDiffucltFactor.SetImmdiately(0);
            Dat_SoundVolum.SetImmdiately(1F);
            Dat_BGMVolum.SetImmdiately(1F);
            Dat_GameShowLanguageSetup.SetImmdiately(Def_ShowLanguage);
            IsBulletCrossWhenScreenNet.SetImmdiately(Def_IsBulletCrossWhenScreenNet);

            LaguageUsing.SetImmdiately(Def_LanguageUsing);
            for (int i = 0; i != Defines.MaxNumPlayer; ++i)
                Dat_PlayersGunScore[i].SetImmdiately(Def_ScoreMin);
        }
    }
}
