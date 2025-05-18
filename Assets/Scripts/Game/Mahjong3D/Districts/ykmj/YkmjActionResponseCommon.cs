using System.Collections.Generic;
using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class YkmjActionResponseCommon : ActionCommonResponse
    {
        protected List<MahjongContainer> mPlayerHand;
        protected List<MahjongCpg> mPengList;

        public override void CustomLogicAction(ISFSObject data)
        {
            if (!ConfigData.Jue) return;

            var type = data.ContainsKey(RequestKey.KeyType) ? data.GetInt(RequestKey.KeyType) : -1;
            var seat = data.ContainsKey(RequestKey.KeySeat) ? data.GetInt(RequestKey.KeySeat) : -1;
            var value = data.ContainsKey(RequestKey.KeyOpCard) ? data.GetInt(RequestKey.KeyOpCard) : -1;
            if (seat == -1 && DataCenter.IsReconect) seat = DataCenter.SelfSeat;
            if (DataCenter.SelfSeat != seat) return;
            mPlayerHand = Game.MahjongGroups.MahjongHandWall[0].MahjongList;
            mPengList = Game.MahjongGroups.MahjongCpgs[0].CpgList;
            switch (type)
            {
                case NetworkProls.CPG:
                    EnGroupType cpgType = data.ContainsKey(AnalysisKeys.KeyTType)
                        ? (EnGroupType)data.GetInt(AnalysisKeys.KeyTType)
                        : EnGroupType.None;
                    if (cpgType == EnGroupType.Peng)
                    {
                        int card = data.ContainsKey(RequestKey.KeyCard) ? data.GetInt(RequestKey.KeyCard) : -1;
                        OnPengCard(card);
                    }
                    break;
                case NetworkProls.GetInCard:
                    OnGetCard(value);
                    break;
                case NetworkProls.ThrowoutCard:
                    OnOutPutCard(value);
                    break;
                case -1:
                    if (DataCenter.IsReconect) CheckJueCard();
                    break;
            }
        }

        protected void OnPengCard(int value)
        {
            for (int i = 0; i < mPlayerHand.Count; i++)
            {
                if (mPlayerHand[i].Value == value && !DataCenter.IsLaizi(value))
                {
                    SetJue(mPlayerHand[i], true);
                }
            }
        }

        protected void OnGetCard(int card)
        {
            var dic = GetCardAmount(mPlayerHand);
            foreach (var item in dic)
            {
                if (item.Value == 4 && item.Key == card)
                {
                    int count = mPlayerHand.Count - 1;
                    for (int i = count; i >= 0; i--)
                    {
                        if (mPlayerHand[i].Value == item.Key)
                        {
                            SetJue(mPlayerHand[i], true);
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < mPengList.Count; i++)
            {
                var type = mPengList[i].Data.Type;
                if (type == EnGroupType.Peng)
                {
                    int value = mPengList[i].Data.Card;
                    if (value == card)
                    {
                        for (int j = 0; j < mPengList.Count; j++)
                        {
                            if (mPlayerHand[j].Value == value)
                            {
                                SetJue(mPlayerHand[j], true);
                            }
                        }
                    }
                }
            }
        }

        protected void OnOutPutCard(int value)
        {
            for (int j = 0; j < mPlayerHand.Count; j++)
            {
                if (mPlayerHand[j].Value == value)
                {
                    if (mPlayerHand[j].GetOther())
                    {
                        //取消绝图标
                        mPlayerHand[j].SetOtherSign(Anchor.TopRight, false);
                    }
                }
            }
        }

        protected Dictionary<int, int> GetCardAmount(List<MahjongContainer> mahjongs)
        {
            Dictionary<int, int> typeDic = new Dictionary<int, int>();
            for (int i = 0; i < mahjongs.Count; i++)
            {
                var value = mahjongs[i].Value;
                if (typeDic.ContainsKey(value))
                {
                    typeDic[value] += 1;
                }
                else
                {
                    typeDic[value] = 1;
                }
            }
            return typeDic;
        }

        protected void CheckJueCard()
        {
            var dic = GetCardAmount(mPlayerHand);
            foreach (var item in dic)
            {
                if (item.Value == 4)
                {
                    int count = mPlayerHand.Count - 1;
                    for (int i = count; i >= 0; i--)
                    {
                        if (mPlayerHand[i].Value == item.Key)
                        {
                            SetJue(mPlayerHand[i], true);
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < mPengList.Count; i++)
            {
                var type = mPengList[i].Data.Type;
                if (type == EnGroupType.Peng)
                {
                    int card = mPengList[i].Data.Card;
                    for (int j = 0; j < mPengList.Count; j++)
                    {
                        if (mPlayerHand[j].Value == card)
                        {
                            SetJue(mPlayerHand[j], true);
                        }
                    }
                }
            }
        }

        protected void SetJue(MahjongContainer mahjong, bool isOn)
        {
            mahjong.SetOtherSign(Anchor.TopRight, isOn);
        }
    }
}
