using Assets.Scripts.Common.Windows;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.toubao
{
    public class Result : YxNguiWindow
    {
        public UILabel MyWin;
        public UILabel MyBack;
        public UILabel BankerWin;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var data = GetData<ISFSObject>();
            if (data == null) return;
            if (data.ContainsKey("win"))
            {
                var win = data.GetInt("win");
                if (win < 0)
                {
                    Facade.Instance<MusicManager>().Play("lose");
                    MyWin.text = YxUtiles.ReduceNumber(win);
                }
                else
                {
                    Facade.Instance<MusicManager>().Play("win");
                    MyWin.text = string.Format("+ {0}", YxUtiles.ReduceNumber(win));
                }
            }
            if (data.ContainsKey("bwin"))
            {
                var bwin = data.GetLong("bwin");
                if (bwin < 0)
                {
                    BankerWin.text = YxUtiles.ReduceNumber(bwin);
                }
                else
                {
                    BankerWin.text = string.Format("+ {0}", YxUtiles.ReduceNumber(bwin));
                }
            }
            if (data.ContainsKey("backgold"))
            {
                var backgold = data.GetInt("backgold");
                if (backgold < 0)
                {
                    MyBack.text = YxUtiles.ReduceNumber(backgold);
                }
                else
                {
                    MyBack.text = string.Format("+ {0}", YxUtiles.ReduceNumber(backgold));
                }
            }
            Invoke("Hide", 4);
        }
    }
}
