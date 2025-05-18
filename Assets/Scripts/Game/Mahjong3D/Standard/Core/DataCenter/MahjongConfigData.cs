using System.Collections.Generic;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongConfigData : IRuntimeData
    {
        public string Cargs = "";

        /// <summary>
        /// 记录房间信息cargs2
        /// </summary>
        private ISFSObject mCargs2;
        private MahjongLocalConfig mLocalConfig;
        private Dictionary<string, int> mCargs = new Dictionary<string, int>();

        public MahjongConfigData()
        {
            mCargs["-jue"] = 0; //绝
            mCargs["-piao"] = 0; //加漂
            mCargs["-pph"] = 0; //碰碰胡
            mCargs["-anbao"] = 0; //暗宝
            mCargs["-niubi"] = 0; //牛听         
            mCargs["-xuanfeng"] = 0; //旋风杠
            mCargs["-cdtime"] = 0;
            mCargs["-huantime"] = 0;
            mCargs["-duantime"] = 0;
            mCargs["-autoready"] = 1;
            mCargs["-changeCnt"] = 3;
            mCargs["-hrule"] = 1; //游戏规则窗口显隐控制    
            mCargs["-tptout"] = 300;
            mCargs["-friends"] = 0;//麻友
            mCargs["-kinFriends"] = 0;//亲友
            mCargs["-puscore"] = 0; //后台配置飘分的数组索引
            mCargs["-playAgain"] = 0;
            mCargs["-feidan"] = 0; //小鸡飞蛋
            mCargs["-zhandan"] = 0; //攒蛋
            mCargs["-dancnt"] = 4; //4为四风蛋;3为三风蛋
            mCargs["-yaojiudan"] = 0; //幺九蛋
            mCargs["-chaTingShow"] = 0;
            mCargs["-hasbao"] = 0;
            mCargs["-lf"] = 0;
            mCargs["-bird"] = 0;
            mCargs["-huangzhuang"] = 0;
            mCargs["-jia"] = 0;//夹
        }

        /// <summary>
        /// 默认配置
        /// 先加载默认config
        /// 没有默认的,生成gamekeyconfig
        /// </summary>
        public MahjongLocalConfig LocalConfig
        {
            get
            {
                if (null == mLocalConfig)
                {
                    mLocalConfig = GameUtils.GetInstanceAssets<MahjongLocalConfig>("MahjongLocalConfig");
                }
                return mLocalConfig;
            }
        }

        public void ResetData() { }

        public void SetData(ISFSObject data)
        {
            //解析cargs
            Cargs = data.GetUtfString(AnalysisKeys.KeyCargs);
            string[] strkey = Cargs.Split(',');
            for (int i = 0; i < strkey.Length; i++)
            {
                string key = strkey[i];
                if (mCargs.ContainsKey(key))
                {
                    mCargs[key] = int.Parse(strkey[i + 1]);
                }
            }
            //解析cargs2
            if (data.ContainsKey("cargs2"))
            {
                mCargs2 = data.GetSFSObject("cargs2");
            }
            //游戏规则
            if (data.ContainsKey(AnalysisKeys.KeyRule))
            {
                LocalConfig.DefaultGameRule = data.GetUtfString(AnalysisKeys.KeyRule);
            }
            //玩家总数
            MaxPlayerCount = data.TryGetInt(AnalysisKeys.KeyPlayerNum);
            //lisi--增加换三张跟断门--start--
            if (data.ContainsKey("huansanzhang"))
            {
                HasHuanZhang = data.GetBool("huansanzhang");
            }
            if (data.ContainsKey("duanmen"))
            {
                HasDingQue = data.GetBool("duanmen");
            }
            //--lisi--end--
            ParseConfig();
        }

        #region 配置信息 - 由server控制
        public bool Pph;//碰碰胡  
        public bool XFGang;//旋风杠   
        public bool ScoreDouble;//下注(加漂)
        public bool _GameRuleHide;//玩法界面开关   
        public bool InviteMahFriends;//邀请麻友
        public bool InviteKinFriends;//邀请亲友
        public bool ContinueNewGame;//支持继续游戏（创建新房间）
        public bool MahjongQuery;//查胡牌功能  
        public bool NiuBi;  //牛逼胡 
        public bool LiangBian;//胡两边  
        public bool GameRuleHide;//玩法界面开关    
        public bool Jue;//绝玩法,与杠互斥        
        public bool AnBao;//明宝        
        public bool FeiDan;//小鸡飞蛋        
        public bool SanFeng;//三风蛋       
        public bool ZanDan; //攒蛋        
        public bool YaoJiuDan;//幺九蛋
        public bool AutoReady;//自动准备
        public bool HasBao;//有宝牌玩法
        public bool Luanfeng;//乱风 
        public bool XiaoJiManTianFei;//小鸡满天飞
        public bool HuangZhuang;//黄庄
        public bool HasJiaHu;//夹
        public bool HasHuanZhang;//是否有换张
        public bool HasDingQue;//是否有定缺

        public int HuanTime;//换张时间
        public int DingqueTime;//定缺时间
        public int MaxPlayerCount = 4;//本局最大人数 
        public int PuScore;//大连麻将扑分(飘分)
        #endregion

        private void ParseConfig()
        {
            Pph = mCargs["-pph"] == 1;
            AnBao = mCargs["-anbao"] == 1;
            NiuBi = mCargs["-niubi"] == 1;
            XFGang = mCargs["-xuanfeng"] == 1;
            ScoreDouble = mCargs["-piao"] == 1;
            GameRuleHide = mCargs["-hrule"] == 0;
            AutoReady = mCargs["-autoready"] != 0;
            InviteMahFriends = mCargs["-friends"] > 0;
            InviteKinFriends = mCargs["-kinFriends"] > 0;
            Jue = mCargs["-jue"] == 1;
            PuScore = mCargs["-puscore"];
            ContinueNewGame = mCargs["-playAgain"] == 1;
            FeiDan = mCargs["-feidan"] == 1;
            SanFeng = mCargs["-dancnt"] == 3;
            ZanDan = mCargs["-zhandan"] == 1;
            YaoJiuDan = mCargs["-yaojiudan"] == 1;
            //-checkting,1 开启服务器查听功能
            MahjongQuery = mCargs["-chaTingShow"] == 1;
            HasBao = mCargs["-hasbao"] == 1;
            Luanfeng = mCargs["-lf"] == 1;//乱风
            XiaoJiManTianFei = mCargs["-bird"] == 1;
            HuangZhuang = mCargs["-huangzhuang"] >= 1;
            HasJiaHu = mCargs["-jia"] == 1;

            //动画时间
            int time = mCargs["-cdtime"];
            LocalConfig.TimeOutcard = (time > 0) && (time < 100) ? time : 15;
            //LocalConfig.TimeHuancard = mCargs["-huantime"] > 0 ? mCargs["-huantime"] : 15;
            //LocalConfig.TimeDingque = mCargs["-duantime"] > 0 ? mCargs["-duantime"] : 15;
            LocalConfig.TimeHandup = mCargs["-tptout"];
        }
    }
}