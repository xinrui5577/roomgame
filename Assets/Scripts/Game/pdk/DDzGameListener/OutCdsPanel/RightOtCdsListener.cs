using System.Collections;
using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.pdk.DDzGameListener.OutCdsPanel
{
    /// <summary>
    /// 自己手牌区域
    /// </summary>
    public class RightOtCdsListener : OutCdsListener
    {

/*        /// <summary>
        /// 如果牌组太长则分2组
        /// </summary>
        /// <returns></returns>    
        private IEnumerator LefcdsSort()
        {

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            const short cdsMaxLen = 9;
            var cdsLen = OutcdsTemp.Count;
            if (cdsLen > cdsMaxLen)
            {
                const int xmove = CdWith*cdsMaxLen/2;
                const int ymove = CdHeight/2;
                for (int i = cdsMaxLen; i < cdsLen; i++)
                {
                    OutcdsTemp[i].transform.localPosition -= new Vector3(xmove, ymove, 0);
                }
            }

        }*/



        protected override void OnGetLasOutData(int currp, ISFSObject lasOutData)
        {
            var rightSeat = App.GetGameData<GlobalData>().GetRightPlayerSeat;
            if (rightSeat != lasOutData.GetInt(RequestKey.KeySeat) || rightSeat==currp) return;

             AllocateCds(lasOutData.GetIntArray(RequestKey.KeyCards));
             PlayPartical(lasOutData);
        }

        /// <summary>
        /// 如果是自己出牌则出牌
        /// </summary>
        protected override void OnTypeOutCard(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            var seat = data.GetInt(RequestKey.KeySeat);
            if (seat == App.GetGameData<GlobalData>().GetRightPlayerSeat)
            {
                AllocateCds(data.GetIntArray(RequestKey.KeyCards));
                PlayPartical(data);
            }
            else if (seat == App.GetGameData<GlobalData>().GetSelfSeat)
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
            if (seat == App.GetGameData<GlobalData>().GetRightPlayerSeat || seat == App.GetGameData<GlobalData>().GetSelfSeat)
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
            ShowHandCds(App.GetGameData<GlobalData>().GetRightPlayerSeat, args.IsfObjData.GetSFSArray("users"));
        }

        protected override void AllocateCds(int[] cds)
        {
            base.AllocateCds(cds);

            Table.transform.localScale = cds.Length > 10 ? new Vector3(0.8f, 0.8f, 1f) : new Vector3(1.1f, 1.1f, 1f);
        }
    }
}
