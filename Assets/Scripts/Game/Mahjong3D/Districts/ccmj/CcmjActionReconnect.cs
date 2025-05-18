using com.yxixia.utile.YxDebug;
using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class CcmjActionReconnect : ActionReconnect
    {
        public override void ReconnectAction(ISFSObject data)
        {
            base.ReconnectAction(data);

            MahjongUserInfo userInfo;
            MahjongGroupsManager group = Game.MahjongGroups;
            ISFSObject cpgData = new SFSObject();
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                userInfo = DataCenter.Players[i];

                var ccmjUser = userInfo.ConverType<CcmjMahjongUserInfo>();
                var user = ccmjUser.OtherUserData;

                if (!user.ContainsKey("danlist")) continue;
                var danList = user.GetSFSArray("danlist");
                for (int j = 0; j < danList.Count; j++)
                {
                    var obj = danList.GetSFSObject(j);
                    int[] cards = obj.GetIntArray("cards");
                    if (obj.ContainsKey("addDan"))
                    {
                        int[] add = obj.GetIntArray("addDan");
                        int[] temp = new int[cards.Length + add.Length];
                        for (int k = 0; k < cards.Length; k++)
                        {
                            temp[k] = cards[k];
                        }
                        for (int k = 0; k < add.Length; k++)
                        {
                            temp[k + cards.Length] = add[k];
                        }
                        cards = temp;
                    }
                    int type = (int)EnGroupType.XiaoJi;
                    for (int k = 0; k < cards.Length; k++)
                    {
                        int card = cards[k];
                        int checkType = CheckCardType(card);
                        if (checkType == (int)EnGroupType.XiaoJi)
                        {
                            if (k == cards.Length - 1)
                                type = (int)EnGroupType.YaoDan;
                            continue;
                        }
                        type = checkType;
                        break;
                    }
                    cpgData.PutInt(AnalysisKeys.KeyTType, type);
                    cpgData.PutInt(RequestKey.KeySeat, userInfo.Seat);
                    cpgData.PutIntArray(RequestKey.KeyCards, cards);
                    group.PopMahFromCurrWall(cards.Length);
                    var xjfd = MahjongUtility.CreateCpg(cpgData);
                    userInfo.CpgDatas.Add(xjfd);
                    group.MahjongCpgs[userInfo.Chair].SetCpg(xjfd);
                }
            }
        }

        /// <summary>
        /// 检查杠出的类型
        /// </summary>
        /// <param name="card">牌值</param>
        /// <returns></returns>
        private int CheckCardType(int card)
        {
            int cardType = 0;
            switch (card)
            {
                case 33:
                    if (GameCenter.DataCenter.ConfigData.FeiDan)
                    {
                        cardType = (int)EnGroupType.XiaoJi;
                    }
                    else
                    {
                        cardType = (int)EnGroupType.YaoDan;
                    }
                    break;
                case 17:
                case 49:
                    cardType = (int)EnGroupType.YaoDan;
                    break;
                case 25:
                case 57:
                case 41:
                    cardType = (int)EnGroupType.JiuDan;
                    break;
                case 81:
                case 84:
                case 87:
                    cardType = (int)EnGroupType.ZFBDan;
                    break;
                case 65:
                case 68:
                case 71:
                case 74:
                    cardType = (int)EnGroupType.XFDan;
                    break;
                default:
                    YxDebug.LogError("给的牌非幺牌，无法杠出");
                    break;
            }
            return cardType;
        }
    }
}
