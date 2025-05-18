using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Game.ddz2.InheritCommon;
using Assets.Scripts.Game.ddz2.PokerCdCtrl;
using Assets.Scripts.Game.ddz2.PokerRule;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.OutCdsPanel
{
    /// <summary>
    /// 出牌监听脚本
    /// </summary>
    public class OutCdsListener : ServEvtListener
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


        protected void Start()
        {
            App.GetGameData<DdzGameData>().OnClearParticalGob = ClearParticalGob;
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
        protected OutCdItem OutcdItemOrg;

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
 

        public void ShowHandCds(int seat, ISFSArray users)
        {
            var len = users.Count;
            for (int i = 0; i < len; i++)
            {
                var obj = users.GetSFSObject(i);

                if (seat == obj.GetInt(RequestKey.KeySeat))
                {
                    AllocateCds(obj.GetIntArray(RequestKey.KeyCards));
                    break;
                }
            }
        }
       


        /// <summary>
        /// 清理旧的出牌
        /// </summary>
        public void ClearAllOutCds()
        {
            var len = OutcdsTemp.Count;
            for (int i = 0; i < len; i++)
            {
               DestroyImmediate(OutcdsTemp[i]);
            }
            OutcdsTemp.Clear();
        }

        /// <summary>
        /// 出牌
        /// </summary>
        /// <param name="cds"></param>
        /// <param name="isDizhu">标记是不是地主发牌</param>
        public virtual void AllocateCds(int[] cds,bool isDizhu=false)
        {
            if (cds == null || cds.Length < 1) return;

            ClearAllOutCds();

            cds = HdCdsCtrl.SortCds(cds);
            var cdsLen = cds.Length;
            var lastIndex = cdsLen - 1;
            for (int i = 0; i < cdsLen; i++)
            {
                AddCdGob(OutcdItemOrg.gameObject, 100 + i + 2, cds[i], i == lastIndex && isDizhu);
            }
            SortCdsGobPos();
        }

        /// <summary>
        /// 出牌时 检查播放例子特效
        /// </summary>
        /// <param name="lasOutData"></param>
        public void PlayPartical(ISFSObject lasOutData)
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
        public void CheckPartiCalPlay(CardType cdType)
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
                        StopCoroutine("HideFeiji");
                        ParticalFeiji.SetActive(true);
                        StartCoroutine(HideFeiji());
                        break;
                    }
                case CardType.C1112223344:
                    {
                        ParticalFeiji.SetActive(false);
                        StopCoroutine("HideFeiji");
                        ParticalFeiji.SetActive(true);
                        StartCoroutine(HideFeiji());
                        break;
                    }
                case CardType.C11122234:
                    {
                        ParticalFeiji.SetActive(false);
                        StopCoroutine("HideFeiji");
                        ParticalFeiji.SetActive(true);
                        StartCoroutine(HideFeiji());
                        break;
                    }
                case CardType.C4:
                    {
                        ParticalZhadan.SetActive(true);
                        ParticalZhadan.GetComponent<ParticleSystem>().Stop();
                        ParticalZhadan.GetComponent<ParticleSystem>().Play();
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

        private void AddCdGob(GameObject cdItemOrg, int layerIndex, int cdValueData,bool isDizhu =false)
        {
            var gob = Table.AddChild(cdItemOrg);
            gob.name = layerIndex.ToString(CultureInfo.InvariantCulture);
            gob.SetActive(true);
            var pokerItem = gob.GetComponent<OutCdItem>();
            pokerItem.SetLayer(layerIndex);
            pokerItem.SetCdValue(cdValueData);
            
            //添加地主标记
            pokerItem.DizhuLogo.SetActive(isDizhu);
            pokerItem.DizhuLogo.GetComponent<UISprite>().depth = layerIndex + 1;

            OutcdsTemp.Add(gob);
        }

    }
}
