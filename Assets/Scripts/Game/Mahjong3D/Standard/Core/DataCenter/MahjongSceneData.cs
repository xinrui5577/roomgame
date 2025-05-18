using System.Collections.Generic;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    /// <summary>
    /// 游戏中需要的数据
    /// </summary>
    public class MahjongSceneData : IRuntimeData
    {
        public int BaoCard;//宝牌     
        public int FanCard;//翻牌     
        public int LaiziCard;//癞子       
        public int CurrOpSeat;//当前操作用户服务器座位号       
        public int OldOpSeat;//上一次操作用户服务器座位号      
        public int OwnerSeat;//房主       
        public int BankerSeat;//庄家       
        public int GetInMahjong;//抓到的麻将     
        public int ThrowoutCard;//打出的麻将      
        public int ReconnectTime;//重连时当前用户的时间 
        public bool IsOutPutCard;//是否玩家已经打过牌       
        public int FristBankerSeat;//第一次庄家位子，随机庄，计算圈用      
        public int ReconectState;//重连时手中牌状态,在手中0,已经打出1      
        public int LeaveMahjongCnt;//剩余麻将个数    
        public int[] BaoSaizisList;//翻宝时打的骰子
        public int[] BaoIndexList;//翻宝时移除的麻将牌index
        public int[] SaiziPoint = new int[2];//塞子的点数     
        public bool FirstGetCard = true;//第一次抓牌
        public bool FenzhangFlag;//分张
        public bool HuangZhuang;
        public List<TotalResult> TotalResult = new List<TotalResult>();

        //wmszmj
        public int Diling;//滴零     
        public int Baozi;//豹子   

        //wmbbmj
        public int Laozhuang;//牢庄
        public int LaozhuangId;

        /// <summary>
        /// 娱乐房时，上一局庄家退出， 新进入玩家牢庄应该是1开始
        /// </summary>
        /// <returns></returns>
        public void YuleSetLaozhuang()
        {
            var db = GameCenter.DataCenter;
            if (db.Room.RoomType == MahRoomType.YuLe)
            {
                var chair = db.BankerChair;
                var bank = db.Players[chair];
                if (LaozhuangId != bank.Id)
                {
                    Laozhuang = 1;
                }
                LaozhuangId = bank.Id;
            }
        }

        public void ResetData()
        {
            FirstGetCard = true;
            IsOutPutCard = false;
            FenzhangFlag = false;

            BaoCard = DefaultUtils.DefValue;
            FanCard = DefaultUtils.DefValue;
            LaiziCard = DefaultUtils.DefValue;
            OldOpSeat = DefaultUtils.DefValue;
            CurrOpSeat = DefaultUtils.DefValue;
            BankerSeat = DefaultUtils.DefValue;
            ThrowoutCard = DefaultUtils.DefValue;
            GetInMahjong = DefaultUtils.DefValue;
            ReconnectTime = DefaultUtils.DefValue;
            ReconectState = DefaultUtils.DefValue;

            Diling = DefaultUtils.DefValue;
            Baozi = DefaultUtils.DefValue;
        }

        public void ClearTotalResult()
        {
            TotalResult.Clear();
        }

        public void SetData(ISFSObject data)
        {
            FanCard = data.TryGetInt(AnalysisKeys.CardFan);
            BaoCard = data.TryGetInt(AnalysisKeys.CardBao);
            LaiziCard = data.TryGetInt(AnalysisKeys.CardLaizi);
            GetInMahjong = data.TryGetInt(AnalysisKeys.KeyLastIn);
            FristBankerSeat = data.TryGetInt(AnalysisKeys.Keybank);
            LeaveMahjongCnt = data.TryGetInt(AnalysisKeys.KeyCardLen);
            ThrowoutCard = data.TryGetInt(AnalysisKeys.KeyLastOutCard);
            SaiziPoint = data.TryGetIntArray(AnalysisKeys.KeyDiceArray);
            BaoSaizisList = data.TryGetIntArray(AnalysisKeys.KeySaiziList);
            BaoIndexList = data.TryGetIntArray(AnalysisKeys.KeyBaoIndexList);
            HuangZhuang = data.TryGetBool(AnalysisKeys.KeyHuangZhuang);
            GameCenter.DataCenter.BankerSeat = data.TryGetInt(AnalysisKeys.KeyStartP);
            GameCenter.DataCenter.CurrOpSeat = data.TryGetInt(AnalysisKeys.KeyCurrentP);

            Laozhuang = data.TryGetInt("lzcnt");
            Diling = data.TryGetInt("diling");
            Baozi = data.TryGetInt("baozi");
        }

        public void SetTotalResult(ISFSObject data)
        {
            ISFSArray userDatas = data.TryGetSFSArray("users");
            if (userDatas == null) return;
            TotalResult result;
            for (int i = 0; i < userDatas.Size(); i++)
            {
                result = new TotalResult();
                ISFSObject obj = userDatas.GetSFSObject(i);
                result.Id = obj.TryGetInt("id");
                result.Hu = obj.TryGetInt("hu");
                result.Pao = obj.TryGetInt("pao");
                result.Zimo = obj.TryGetInt("zimo");
                result.Gang = obj.TryGetInt("gang");
                result.Seat = obj.TryGetInt("seat");
                result.Glod = obj.TryGetInt("gold");
                result.ZhaNiao = obj.TryGetInt("niao");
                result.AnGang = obj.TryGetInt("anGang");
                result.Gangkais = obj.TryGetInt("gangkai");
                result.Name = obj.TryGetString("nick");
                result.MoBao = obj.TryGetInt("mobao");
                result.ChBao = obj.TryGetInt("chbao");
                TotalResult.Add(result);
            }
        }
    }

    public class MahjongResult
    {
        public int Id;
        public int Seat;
        public int Chair;
        public int HuSeat = DefaultUtils.DefInt;
        public int Gold;
        public int CType;//胡牌类型  
        public int HuCard;
        public int HuType;
        public int HuGold;
        public int PuGlod;
        public int GangGlod;
        public int PiaoGlod;
        public int NiaoGold;
        public int UserHuType;
        public int FujingArray;
        public int ZhengjingArray;
        public long TotalGold;
        public string Name;
        public string HuInfo;
        public bool HuFlag;
        public List<ScoreDetail> Deatils;

        public MahjongResult(int seat)
        {
            Seat = seat;
            Chair = MahjongUtility.GetChair(seat);
            MahjongUserInfo data = GameCenter.DataCenter.Players[Chair];
            if (null != data)
            {
                Id = data.Id;
                Name = data.NickM;
            }
        }

        public void SetDeatil(ISFSObject sfsObject)
        {
            if (sfsObject.ContainsKey("huDetails"))
            {
                Deatils = new List<ScoreDetail>();
                var array = sfsObject.GetSFSArray("huDetails");

                for (int i = 0; i < array.Count; i++)
                {
                    var data = array.GetSFSObject(i);
                    Deatils.Add(new ScoreDetail(data));
                }
            }
        }

        public class ScoreDetail
        {
            public int[] MatterSeats;
            public int AmountScore;
            public string Description;

            public ScoreDetail(ISFSObject obj)
            {
                AmountScore = obj.TryGetInt("gold");
                Description = obj.TryGetString("hname");
                MatterSeats = obj.TryGetIntArray("otherPlayer");
            }
        }
    }

    public class TotalResult
    {
        public int Id;
        public int Hu;
        public int Pao;
        public int Seat;
        public int Glod;
        public int Gang;
        public int Zimo;
        public int AnGang;
        public int ZhaNiao;
        public int Gangkais;
        public string Name;
        public int MoBao;
        public int ChBao;
    }
}