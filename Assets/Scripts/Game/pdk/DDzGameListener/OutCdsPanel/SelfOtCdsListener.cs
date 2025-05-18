using System.Collections;
using Assets.Scripts.Game.pdk.DDz2Common;
using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.pdk.DDzGameListener.OutCdsPanel
{
    /// <summary>
    /// 自己手牌区域
    /// </summary>
    public class SelfOtCdsListener : OutCdsListener
    {
        /// <summary>
        /// 春天例子特效在某个时刻
        /// </summary>
        [SerializeField] protected GameObject ParticalChunTian;
        
        protected override void ClearParticalGob()
        {
            base.ClearParticalGob();
            DestroyImmediate(ParticalChunTian);
        }

        protected override void OnGetLasOutData(int currp,ISFSObject lasOutData)
        {
            var selfSeat = App.GetGameData<GlobalData>().GetSelfSeat;
            if (selfSeat != lasOutData.GetInt(RequestKey.KeySeat) || selfSeat == currp) return;

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
            var playerMaxNum = App.GetGameData<GlobalData>().PlayerMaxNum;
            if (seat == App.GetGameData<GlobalData>().GetSelfSeat)
            {
                AllocateCds(data.GetIntArray(RequestKey.KeyCards));
                PlayPartical(data);
            }
            else if (playerMaxNum == 3 && seat == App.GetGameData<GlobalData>().GetLeftPlayerSeat)
            {
                ClearAllOutCds();
            }
            else if (playerMaxNum == 2 && seat == App.GetGameData<GlobalData>().GetRightPlayerSeat)
            {
                ClearAllOutCds();
            }

        }

        /// <summary>
        /// 如果是自己pass则清除自己之前出的牌
        /// </summary>
        protected override void OnTypePass(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            var seat = data.GetInt(RequestKey.KeySeat);
            var playerMaxNum = App.GetGameData<GlobalData>().PlayerMaxNum;
            if (seat == App.GetGameData<GlobalData>().GetSelfSeat)
                ClearAllOutCds();
            else if (playerMaxNum == 3 && seat == App.GetGameData<GlobalData>().GetLeftPlayerSeat)
                ClearAllOutCds();
            else if(playerMaxNum == 2 && seat == App.GetGameData<GlobalData>().GetRightPlayerSeat)
                ClearAllOutCds();

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
            ShowHandCds(App.GetGameData<GlobalData>().GetSelfSeat, args.IsfObjData.GetSFSArray("users"));
            var isfdata = args.IsfObjData;

            if (!isfdata.ContainsKey(NewRequestKey.KeySpring)) yield break;
            //判断是否显示春天特效
            if (!isfdata.GetBool(NewRequestKey.KeySpring)) yield break;
            ParticalChunTian.SetActive(false);
            ParticalChunTian.SetActive(true);
            ParticalChunTian.GetComponent<ParticleSystem>().Stop();
            ParticalChunTian.GetComponent<ParticleSystem>().Play();
            if (isfdata.GetInt("gold")>0)
            {
                Facade.Instance<MusicManager>().Play("winchuntian");
                //Debug.Log("-><color=#9400D3>" + "春天——WIn春天——WIn春天——WIn春天——WIn春天——WIn春天——WIn春天——WIn春天——WIn春天——WIn春天——WIn春天——WIn" + "</color>");
            }
            else
            {
                Facade.Instance<MusicManager>().Play("losechuntian");
                //Debug.Log("-><color=#9400D3>" + "春天——Lost春天——Lost春天——Lost春天——Lost春天——Lost春天——Lost春天——Lost春天——Lost春天——Lost春天——Lost" + "</color>");
            }
        }
    }
}
