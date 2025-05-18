using System.Linq;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using YxFramwork.Common;

namespace Assets.Scripts.Game.hg
{
    public class HgRankWindow : YxNguiWindow
    {
        public HgRankItem HgRankItem;
        public HgOnLineItem HgOnLineItem;
        public UIGrid RankGrid;

        private HgGameData _gdata
        {
            get { return App.GetGameData<HgGameData>(); }
        }
        protected override void OnFreshView()
        {
            var aboutAround = _gdata.PlayerRecordNum;
            var count = _gdata.GoldRank.Count;
            var index=0;
            var userPos=new System.Collections.Generic.List<int>();

            Item obj = null;
            if (HgRankItem)
            {
                obj = HgRankItem;
            }

            if (HgOnLineItem)
            {
                obj = HgOnLineItem;
            }


            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < _gdata.AllUserInfos.Count; j++)
                {
                    if (_gdata.GoldRank[i] == _gdata.AllUserInfos[j].Seat)
                    {
                        var item = YxWindowUtils.CreateItem(obj, RankGrid.transform);
                        item.SetRankData(i, _gdata.AllUserInfos[j], aboutAround);
                        userPos.Add(_gdata.AllUserInfos[j].Seat);
                        index++;
                    }
                }
            }

            HgUserInfo[] infos = new HgUserInfo[_gdata.AllUserInfos.Count];
            _gdata.AllUserInfos.CopyTo(infos);
            var userInfos = infos.ToList();
//            Debug.LogError("userInfos----------------"+ userInfos.Count);
            if (userInfos.Count > count)
            {
                for (int i = 0; i < userPos.Count-1; i++)
                {
                    for (int j = 0; j < userInfos.Count-1; j++)
                    {
                        if (userPos[i] == userInfos[j].Seat)
                        {
                            userInfos.RemoveAt(j);
                        }
                    }
//                    Debug.LogError("userPos.Count"+ userPos.Count + "userPos[i]"+ userPos[i]+"i-------"+i);
                }

                for (int i = 0; i < userInfos.Count-1; i++)
                {
                    var item = YxWindowUtils.CreateItem(obj, RankGrid.transform);
                    item.SetRankData(index, userInfos[i], aboutAround);
                    index++;
                }
            }

            RankGrid.repositionNow = true;
        }
    }
}
