using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Game.pdk.DDz2Common;
using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using Assets.Scripts.Game.pdk.PokerCdCtrl;
using Assets.Scripts.Game.pdk.PokerRule;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.pdk.DDzGameListener.OutCdsPanel
{
    /// <summary>
    /// 出牌监听脚本
    /// </summary>
    public abstract class OutCdsListener : ServEvtListener
    {

        /// <summary>
        /// 顺子的粒子特效
        /// </summary>
        [SerializeField]
        protected GameObject ParticalShunzi;

        /// <summary>
        /// 连对的粒子特效
        /// </summary>
        [SerializeField]
        protected GameObject ParticalLianDui;

        /// <summary>
        /// 飞机的粒子特效
        /// </summary>
        [SerializeField]
        protected GameObject ParticalFeiji;
        /// <summary>
        /// 记录飞机特效的播放时间
        /// </summary>
        const float FeijiDurTime = 5f;

        /// <summary>
        /// 炸弹的粒子特效
        /// </summary>
        [SerializeField]
        protected GameObject ParticalZhadan;


        /// <summary>
        /// 炸弹的粒子特效
        /// </summary>
        [SerializeField]
        protected GameObject ParticalWangZha;

        /// <summary>
        /// 定义牌的宽高
        /// </summary>
        protected const int CdWith = 120;
        protected const int CdHeight = 194;

        protected override void OnAwake()
        {
            PdkGameManager.AddOnGetRejoinDataEvt(OnRejoinGame);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeOutCard, OnTypeOutCard);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypePass, OnTypePass);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeGameOver, OnTypeGameOver);
            Ddz2RemoteServer.AddOnUserReadyEvt(OnUserReady);
        }

        void Start()
        {
            App.GetGameData<GlobalData>().OnClearParticalGob = ClearParticalGob;
            Facade.EventCenter.AddEventListener<string, object>(YxMessageName.ConnectionLost, OnConnectionLost);

        }

        protected  void OnConnectionLost(object msg)
        {
            ClearParticalGob();
        }

        protected virtual void ClearParticalGob()
        {
            DestroyImmediate(ParticalShunzi);     
            DestroyImmediate(ParticalLianDui);
            DestroyImmediate(ParticalFeiji);
            DestroyImmediate(ParticalZhadan);
            DestroyImmediate(ParticalWangZha);
        }

        /// <summary>
        /// 卡牌的原型
        /// </summary>
        [SerializeField]
        private OutCdItem _outcdItemOrg;

        /// <summary>
        /// outcds的gird容器
        /// </summary>
        [SerializeField] protected GameObject Table;//UIGrid Table;

        /// <summary>
        /// 存储已经存在的出的牌
        /// </summary>
        protected readonly List<GameObject> OutcdsTemp = new List<GameObject>();

        public override void RefreshUiInfo()
        {
            
        }


        protected virtual void OnRejoinGame(object sender, DdzbaseEventArgs args)
        {
            ClearAllOutCds();

            var data = args.IsfObjData;
            if (!data.ContainsKey(NewRequestKey.KeyCurrp) || !data.ContainsKey(NewRequestKey.KeyLastOut)) return;


            //如果存在最后一次出牌的信息
            if (data.ContainsKey(NewRequestKey.KeyLastOut) && data.ContainsKey(NewRequestKey.KeyLastoutP))
            {
                var lasOutData = new SFSObject();
                lasOutData.PutInt(RequestKey.KeySeat, data.GetInt(NewRequestKey.KeyLastoutP));
                lasOutData.PutIntArray(RequestKey.KeyCards, data.GetIntArray(NewRequestKey.KeyLastOut));

                OnGetLasOutData(data.GetInt(NewRequestKey.KeyCurrp), lasOutData);
            }

        }

        /// <summary>
        /// 当重练时获得 最后一次出牌信息时候
        /// </summary>
        /// <param name="currP">最后一次</param>
        /// <param name="lasOutData"></param>
        protected abstract void OnGetLasOutData(int currP,ISFSObject lasOutData);


        /// <summary>
        /// 有人出牌后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected abstract void OnTypeOutCard(object sender, DdzbaseEventArgs args);

        /// <summary>
        /// 有人pass后
        /// </summary>
        protected abstract void OnTypePass(object sender, DdzbaseEventArgs args);


    

        /// <summary>
        /// 当游戏结算时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void OnTypeGameOver(object sender, DdzbaseEventArgs args)
        {
            //ClearAllOutCds();
        }

        /// <summary>
        /// 当玩家准备时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnUserReady(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            if (data.ContainsKey(RequestKey.KeySeat) && data.GetInt(RequestKey.KeySeat) == App.GetGameData<GlobalData>().GetSelfSeat)
            {
                ClearAllOutCds();
                Table.transform.localPosition = new Vector3(47.7f, 20f, 0);
            }

            
        }

        protected void ShowHandCds(int seat, ISFSArray users)
        {
            if (seat == App.GetGameData<GlobalData>().SelfSeat)
            {
                Table.transform.localPosition = new Vector3(47.7f, -40f, 0);
            }

            var len = users.Count;
            for (int i = 0; i < len; i++)
            {
                var obj = users.GetSFSObject(i);

                if (seat == obj.GetInt(RequestKey.KeySeat))
                {
                    var cds = obj.GetIntArray(RequestKey.KeyCards);

                    AllocateCds(cds);
                    break;
                }
            }
        }




        /// <summary>
        /// 清理旧的出牌
        /// </summary>
        protected void ClearAllOutCds()
        {
            var oldCds = OutcdsTemp.ToArray();
            var len = oldCds.Length;
            for (int i = 0; i < len; i++)
            {
               DestroyImmediate(oldCds[i]);
            }
            OutcdsTemp.Clear();
            
        }
    
        /// <summary>
        /// 给手牌发牌
        /// </summary>
        /// <param name="cds"></param>
        protected virtual void AllocateCds(int[] cds)
        {
            if (cds == null || cds.Length < 1) return;
            ClearAllOutCds();
            cds = HdCdsCtrl.SortCds(cds);
            var cdsLen = cds.Length;
            for (int i = 0; i < cdsLen; i++)
            {
                AddCdGob(_outcdItemOrg.gameObject, i + 2, cds[i]);
            }

            SortCdsGobPos();
        }

        /// <summary>
        /// 出牌时 检查播放例子特效
        /// </summary>
        /// <param name="lasOutData"></param>
        protected void PlayPartical(ISFSObject lasOutData)
        {
            if(!lasOutData.ContainsKey(RequestKey.KeyCards)) return;

            var outCds = lasOutData.GetIntArray(RequestKey.KeyCards);
            if (!lasOutData.ContainsKey(RequestKey.KeyCardType))
            {
                CheckPartiCalPlay(outCds);
            }
            else
            {
                var type = (CardType)lasOutData.GetInt(RequestKey.KeyCardType);
                if (type == CardType.None || type == CardType.Exception) CheckPartiCalPlay(outCds);
                else CheckPartiCalPlay(type);
            }
        }

        /// <summary>
        /// 检查出的牌，播放相应的粒子特效
        /// </summary>
        /// <param name="outCds">已经排序好的扑克</param>
        private void CheckPartiCalPlay(int[] outCds)
        {
            CheckPartiCalPlay(PokerRuleUtil.GetCdsType(outCds));
        }

        /// <summary>
        /// 检查出的牌，播放相应的粒子特效
        /// </summary>
        /// <param name="cdType">卡牌类型</param>
        private void CheckPartiCalPlay(CardType cdType)
        {
            switch (cdType)
            {
                case CardType.C123:
                    {
                        ParticalShunzi.SetActive(true);
                        ParticalShunzi.GetComponent<ParticleSystem>().Stop();
                        ParticalShunzi.GetComponent<ParticleSystem>().Play();
                        break;
                    }
                case CardType.C1122:
                    {
                        ParticalLianDui.SetActive(true);
                        ParticalLianDui.GetComponent<ParticleSystem>().Stop();
                        ParticalLianDui.GetComponent<ParticleSystem>().Play();
                        break;
                    }
                case CardType.C111222:
                    {
                        ParticalFeiji.SetActive(false);
                        StopCoroutine(CornameHideFeiji);
                        ParticalFeiji.SetActive(true);
                        StartCoroutine(CornameHideFeiji);
                        break;
                    }
                case CardType.C1112223434:
                    {
                        ParticalFeiji.SetActive(false);
                        StopCoroutine(CornameHideFeiji);
                        ParticalFeiji.SetActive(true);
                        StartCoroutine(CornameHideFeiji);
                        break;
                    }
                case CardType.C11122234:
                    {
                        ParticalFeiji.SetActive(false);
                        StopCoroutine(CornameHideFeiji);
                        ParticalFeiji.SetActive(true);
                        StartCoroutine(CornameHideFeiji);
                        break;
                    }
                case CardType.C4:
                    {
                        ParticalZhadan.SetActive(true);
                        ParticalZhadan.GetComponent<ParticleSystem>().Stop();
                        ParticalZhadan.GetComponent<ParticleSystem>().Play();
                        Facade.Instance<MusicManager>().Play("k_zhadan");
                        break;
                    }
                case CardType.C42:
                    {
                        ParticalWangZha.SetActive(true);
                        ParticalWangZha.GetComponent<ParticleSystem>().Stop();
                        ParticalWangZha.GetComponent<ParticleSystem>().Play();
                        break;
                    }
            }
        }


        private const string CornameHideFeiji = "HideFeiji";
        private IEnumerator HideFeiji()
        {
            yield return new WaitForSeconds(FeijiDurTime);
            ParticalFeiji.SetActive(false);
        }


        /// <summary>
        /// 排序卡牌位置
        /// </summary>
        protected virtual void SortCdsGobPos()
        {
           // Table.repositionNow = true;
            Table.GetComponent<UITable>().repositionNow = true;
        }

        private void AddCdGob(GameObject cdItemOrg, int layerIndex, int cdValueData)
        {
            var gob = NGUITools.AddChild(Table, cdItemOrg);
            gob.name = layerIndex.ToString(CultureInfo.InvariantCulture);
            gob.SetActive(true);
            var pokerItem = gob.GetComponent<OutCdItem>();
            pokerItem.SetLayer(layerIndex);
            pokerItem.SetCdValue(cdValueData);

            OutcdsTemp.Add(gob);
        }

    }
}
