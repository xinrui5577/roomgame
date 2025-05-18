using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Game.pdk.DDz2Common;
using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using Assets.Scripts.Game.pdk.PokerCdCtrl;
using Sfs2X.Entities.Data;
using UnityEngine;

namespace Assets.Scripts.Game.pdk.DDzGameListener
{
    public class LeftIdleCesListener : ServEvtListener
    {

        protected override void OnAwake()
        {
            PdkGameManager.AddOnGameInfoEvt(OnGameInfo);
            PdkGameManager.AddOnGetRejoinDataEvt(OnRejoinGame);
            Ddz2RemoteServer.AddOnFindRoomEvt(OnFindRoom);
            Ddz2RemoteServer.AddOnUserReadyEvt(OnUserReady);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeGameOver, OnTypeGameOver);

        }

        private void OnUserReady(object sender, DdzbaseEventArgs e)
        {
            ClearAllOutCds();
        }


        // Use this for initialization
        void Start () {
	
        }
	
        // Update is called once per frame
        void Update () {
	            
        }

        /// <summary>
        /// 存储已经存在的出的牌
        /// </summary>
        protected readonly List<GameObject> OutcdsTemp = new List<GameObject>();
        /// <summary>
        /// 卡牌的原型
        /// </summary>
        [SerializeField]
        private OutCdItem _outcdItemOrg;

        /// <summary>
        /// outcds的gird容器
        /// </summary>
        [SerializeField]
        protected GameObject Table;//UIGrid Table;

        /// <summary>
        /// 剩余牌数
        /// </summary>
        [SerializeField]
        protected UILabel LeftCdsLabel;
        
        /// <summary>
        /// 空闲扑克gob
        /// </summary>
        [SerializeField]
        protected GameObject IdlePorkGob;

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

            Table.GetComponent<UITable>().repositionNow = true;
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

        private void OnGameInfo(object sender, DdzbaseEventArgs args)
        {
            CheckModelUpdateLftCds(args.IsfObjData, NewRequestKey.KeyClientArgs2);
        }

        /// <summary>
        /// 当玩家准备时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnFindRoom(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            if (data.ContainsKey("r"))
            {
                CheckModelUpdateLftCds(data, NewRequestKey.KeyClientArgs);
            }
        }

        private void OnRejoinGame(object sender, DdzbaseEventArgs args)
        {
            CheckModelUpdateLftCds(args.IsfObjData, NewRequestKey.KeyClientArgs2);
        }


        private void OnTypeGameOver(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            if(data.ContainsKey(NewRequestKey.KeyOtherCds))
                AllocateCds(data.GetIntArray(NewRequestKey.KeyOtherCds));
        }


        private void CheckModelUpdateLftCds(ISFSObject data,string key)
        {
            IdlePorkGob.SetActive(false);
            //判断玩家人数决定显示leftIdleCesListener的显示
            if (data.ContainsKey(NewRequestKey.KeyPlayerNum))
            {
                if (data.GetInt(NewRequestKey.KeyPlayerNum) != 2) return;
            }
            else
                return;

            IdlePorkGob.SetActive(true);
            if (data.ContainsKey(key))
            {
                var roomData = data.GetSFSObject(key);
                if (roomData.ContainsKey(NewRequestKey.KeyModel))
                {
                    LeftCdsLabel.text = roomData.GetUtfString(NewRequestKey.KeyModel).Equals("2") ? "15张" : "16张";
                }
            }
        }



        public override void RefreshUiInfo()
        {
           // throw new System.NotImplementedException();
        }

    }
}
