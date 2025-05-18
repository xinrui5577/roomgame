using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class CcmjActionCpg : ActionCpg
    {
        private int mOldThrowoutCard;

        public override void OnResponseCpgXjfd(ISFSObject data)
        {
            if (data.ContainsKey(AnalysisKeys.KeyTType))
            {
                var ttype = data.GetInt(AnalysisKeys.KeyTType);
                switch (ttype)
                {
                    case 1:
                        ttype = (int)EnGroupType.XFDan;
                        break;
                    case 2:
                        ttype = (int)EnGroupType.ZFBDan;
                        break;
                    case 3:
                        ttype = (int)EnGroupType.YaoDan;
                        break;
                    case 4:
                        ttype = (int)EnGroupType.JiuDan;
                        break;
                }
                data.PutInt(AnalysisKeys.KeyTType, ttype);
            }

            SetData(data);
            mOldThrowoutCard = 0;
            var xjfdData = (CpgXfdGang)mCpgData;       
            MahjongGroupsManager group = Game.MahjongGroups;
            if (xjfdData.GetHardCards().Count >= 3) xjfdData.Ok = true;           

            if (!xjfdData.Ok)
            {
                if (xjfdData.GetHardCards().Count == 1)
                {
                    var targetList = xjfdData.GetHardCards();
                    group.MahjongHandWall[mCurrOpChair].ThrowOut(targetList[0]);
                    group.MahjongThrow[mCurrOpChair].GetInMahjong(targetList[0]);
                    mOldThrowoutCard = DataCenter.ThrowoutCard;
                    DataCenter.ThrowoutCard = targetList[0];
                }
                return;
            }
            if (xjfdData.GetHardCards().Count == 1)
            {
                var cpgList = group.MahjongCpgs[mCurrOpChair].CpgList;
                for (int i = 0; i < cpgList.Count; i++)
                {

                    if (xjfdData.Type == cpgList[i].Data.Type)
                    {
                        var xjfdGang = (MahjongCpgXjfdGang)cpgList[i];
                        var item = group.MahjongThrow[mCurrOpChair].GetLastMj();
                        xjfdGang.AddXjfd(item);
                        group.MahjongThrow[mCurrOpChair].PopMahjong();
                        group.MahjongCpgs[mCurrOpChair].SortGpg();
                        if (mOldThrowoutCard != 0) DataCenter.ThrowoutCard = mOldThrowoutCard;
                        PlayEffect(data);
                        return;
                    }
                }
            }
            else
            {
                var tempList = xjfdData.GetHardCards();
                DataCenter.Players[mCurrOpChair].CpgDatas.Add(xjfdData);
                for (int i = 0; i < tempList.Count; i++)
                {
                    DataCenter.Players[mCurrOpChair].HardCards.Remove(tempList[i]);
                }
                group.MahjongHandWall[mCurrOpChair].RemoveMahjong(xjfdData.GetHardCards());
            }
            //如果是别人打出的牌
            if (xjfdData.GetOutPutCard() != DefaultUtils.DefValue)
            {
                group.MahjongThrow[mOldOpChair].PopMahjong(xjfdData.Card);
                //隐藏箭头
                Game.TableManager.HideOutcardFlag();
            }
            //排序麻将               
            if (mCurrOpChair == 0)
            {
                group.PlayerHand.SortHandMahjong();
            }
            group.MahjongCpgs[mCurrOpChair].SetCpg(xjfdData);
            //如果吃碰杠之后 cpg 加 手牌数量 大于 手牌数量 需要打牌设置最后一张
            if (group.MahjongCpgs[mCurrOpChair].GetHardMjCnt() + group.MahjongHandWall[mCurrOpChair].MahjongList.Count > DataCenter.Config.HandCardCount)
            {
                group.MahjongHandWall[mCurrOpChair].SetLastCardPos(DefaultUtils.DefValue);
            }
            //麻将记录
            RecordMahjong(xjfdData);
            PlayEffect(data);
        }
    }
}
