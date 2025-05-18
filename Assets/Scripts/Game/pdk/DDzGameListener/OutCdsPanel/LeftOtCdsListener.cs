using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using System.Collections;

namespace Assets.Scripts.Game.pdk.DDzGameListener.OutCdsPanel
{
    /// <summary>
    /// 自己手牌区域
    /// </summary>
    public class LeftOtCdsListener : OutCdsListener {

        protected override void OnGetLasOutData(int currp,ISFSObject lasOutData)
        {
            var leftSeat = App.GetGameData<GlobalData>().GetLeftPlayerSeat;
            if (leftSeat != lasOutData.GetInt(RequestKey.KeySeat) || leftSeat==currp) return;


            var outCds = lasOutData.GetIntArray(RequestKey.KeyCards);
            AllocateCds(outCds);
            PlayPartical(lasOutData);
        }
        /// <summary>
        /// 如果是自己出牌则出牌
        /// </summary>
        protected override void OnTypeOutCard(object sender ,DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            var seat = data.GetInt(RequestKey.KeySeat);
            if (seat == App.GetGameData<GlobalData>().GetLeftPlayerSeat)
            {
                AllocateCds(data.GetIntArray(RequestKey.KeyCards));
                PlayPartical(data);
            }
            else if (seat == App.GetGameData<GlobalData>().GetRightPlayerSeat)
            {
                ClearAllOutCds();
            }
            
        }

        /// <summary>
        /// 如果是自己pass则情况自己之前出的牌
        /// </summary>
        protected override void OnTypePass(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            var seat = data.GetInt(RequestKey.KeySeat);
            if (seat == App.GetGameData<GlobalData>().GetLeftPlayerSeat ||
                seat == App.GetGameData<GlobalData>().GetRightPlayerSeat)
            {
                ClearAllOutCds();
            }
            
        }


        /// <summary>
        /// 当游戏结算时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnTypeGameOver(object sender, DdzbaseEventArgs args)
        {
            //ClearAllOutCds();

            StartCoroutine(ShowHdCdOnGameOver(args));

        }

        private IEnumerator ShowHdCdOnGameOver(DdzbaseEventArgs args)
        {
            yield return new WaitForSeconds(2f);
            ClearAllOutCds();
            ShowHandCds(App.GetGameData<GlobalData>().GetLeftPlayerSeat, args.IsfObjData.GetSFSArray("users"));
        }


        protected override void AllocateCds(int[] cds)
        {
            base.AllocateCds(cds);

            Table.transform.localScale = cds.Length > 10 ? new Vector3(0.8f, 0.8f, 1f) : new Vector3(1.1f, 1.1f, 1f);
        }
    }
}
