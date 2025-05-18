using Assets.Scripts.Common.Windows;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.bjlb
{
    public class ResultWindow : YxNguiWindow
    {

        public UILabel[] ResultLabel;
        public UILabel SumLabel;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var response = GetData<ISFSObject>();
            if (response == null) return; 
            var pg = response.GetIntArray("pg");
            var minCount = Mathf.Min(pg.Length, ResultLabel.Length);
            for (var i = 0; i < minCount; i++)
            {
                ResultLabel[i].text = YxUtiles.ReduceNumber(pg[i]);     // pg[i] >= 0 ? "+ " + pg[i] : "" + pg[i];
                
            }
            if (response.ContainsKey("win"))
            {
                var win = response.GetInt("win");
                if (win < 0)
                {
                    Facade.Instance<MusicManager>().Play("lose");
                    SumLabel.text = YxUtiles.ReduceNumber(win);
                }
                else
                {
                    Facade.Instance<MusicManager>().Play("win");
                    SumLabel.text = string.Format("+ {0}", YxUtiles.ReduceNumber(win));
                }
            }
            else
            {
                SumLabel.text = "0";
            }


            Invoke("Hide", 5);
        }
    }
}
