using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using Assets.Scripts.Game.ddz2.PokerCdCtrl;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.ddz2
{
    public class ErrorRevise : MonoBehaviour {


        /// <summary>
        /// 手牌控制脚本
        /// </summary>
        [SerializeField]
        protected HdCdsCtrl HdCdctrlInstance;

        private void Awake()
        {
            //发牌,叫到地主需要检测手牌
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeAllocate, InvokeCheckHandCards);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeFirstOut, InvokeCheckHandCards);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyOnRejoin, OnGetRejionGameInfo);
        }

        private void OnGetRejionGameInfo(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            if (!data.ContainsKey("hcrejion")) return;
            var serverState = data.GetSFSObject("hcrejion");

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                HdCdctrlInstance.ReSetHandCds(new int[0]);
            }
        }


        void InvokeCheckHandCards(DdzbaseEventArgs args)
        {
            CancelInvoke();
            Invoke("CheckHandCards", 5f);
        }

        private void CheckHandCards()
        {
            if (!HdCdctrlInstance.HaveHandCard)
            {
                var data = new SFSObject();
                data.PutBool("hcrejion", true);
                Debug.LogError("手牌无数据,重连");
                App.RServer.SendReJoinGame();
            }
        }

    }
}
