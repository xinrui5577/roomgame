using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using UnityEngine;
using System.Collections.Generic;
using YxFramwork.Common;
using System.Collections;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.Handup
{
    /// <summary>
    /// 投票解散listener
    /// </summary>
    public class HandUpListener : ServEvtListener
    {
        /// <summary>
        /// 内容Label
        /// </summary>
        [SerializeField] 
        protected UILabel ContentLabel;

        /// <summary>
        /// 倒计时Label
        /// </summary>
        [SerializeField]
        protected UILabel CuntDownLabel;
   

        /// <summary>
        /// 同意按键
        /// </summary>
        [SerializeField]
        protected GameObject ConfirmBtn;

        /// <summary>
        /// 拒绝按钮
        /// </summary>
        [SerializeField]
        protected GameObject RefuseBtn;

        [SerializeField]
        protected GameObject HandupView;

        [SerializeField]
        protected string TimePlan = "{0}";

        //存储投票发起人的名字和对应的handType
        protected Dictionary<string, int> NameToType = new Dictionary<string, int>();

        /// <summary>
        /// 投票需要等待的总时间
        /// </summary>
        private int _cdtime = 300;


        protected override void OnAwake()
        {
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyOnRejoin, OnRejoinGame);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyOnHandUp, OnHandUpEvt);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyRoomGameOver, OnRoomGameOverEvt);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyGetGameInfo, OnGetGameInfo);
        }

        public override void RefreshUiInfo()
        {

        }

        private void OnGetGameInfo(DdzbaseEventArgs args)
        {
            var landRequestData = args.IsfObjData;
            var cargs2 = landRequestData.GetSFSObject("cargs2");
            if (cargs2 == null) return;
            _cdtime = cargs2.ContainsKey("-tptout") ? int.Parse(cargs2.GetUtfString("-tptout")) : 300;
        }



        /// <summary>
        /// 重连游戏
        /// </summary>
        /// <param name="args"></param>
        protected void OnRejoinGame(DdzbaseEventArgs args)
        {

            OnGetGameInfo(args);        //获取房间基本参数

            //初始化解散房间
            var landRequestData = args.IsfObjData;
            //如果接收重连解散信息则不响应
            if (!landRequestData.ContainsKey("hup")) return;
            HandupView.SetActive(true);

            var time = (int)(landRequestData.GetLong("svt") - landRequestData.GetLong("hupstart"));
            time = _cdtime - time;
            time = time < 0 ? 0 : time;
            SetTime(_cdtime);
            StartCoroutine(CuntDownTime(time));

            string[] ids = landRequestData.GetUtfString("hup").Split(',');
            CreateHundItems();
            for (int i = 0; i < ids.Length; i++)
            {
                int userId = int.Parse(ids[i]);
                HideChoiseBtns(userId);
                var item = GetItem(userId);
                if (item == null) continue;
                item.SetItemType(3);
            }
            
        }
     


        /// <summary>
        /// 投票事件激发 2发起 3同意 -1拒绝
        /// </summary>
        /// <param name="args"></param>
        void OnHandUpEvt(DdzbaseEventArgs args)
        {
         
            HandupView.SetActive(true);

            var data = args.IsfObjData;
            if (data.ContainsKey("cdTime"))
            {
                _cdtime = data.GetInt("cdTime");
                SetTime(_cdtime);
            }

            //初始化数据
            var userId = data.GetInt("userId");
            int type = data.GetInt("type");
            string userName = data.GetUtfString("username");

            //发起投票,初始化界面
            if (type == 2)
            {
                CreateHundItems();
                StartCoroutine(CuntDownTime(_cdtime));
                SetChoiseBtnsActive(true);
            }
            else if (type == -1)
            {
                StartCoroutine(HideHandUpPanel(2f));
            }

            HandUpItem item = userId > 0 ? GetItem(userId) : GetItem(userName);

            if (item != null)
            {
                item.SetItemType(type);
            }
            else
            {
                Debug.LogError("未能获得对象");
            }

            //如果handup发来信息的玩家名字是 这个客户端的玩家自己，则隐藏同意 拒绝按钮
            HideChoiseBtns(userId);
        }

        /// <summary>
        /// 隐藏选择按钮
        /// </summary>
        /// <param name="userId"></param>
        void HideChoiseBtns(int userId)
        {
            if (userId != App.GameData.GetPlayerInfo().Id) return;
            SetChoiseBtnsActive(false);
          
        }

        private void SetChoiseBtnsActive(bool active)
        {
            ConfirmBtn.SetActive(active);
            RefuseBtn.SetActive(active);
        }

        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="cdTime"></param>
        void SetTime(int cdTime)
        {
            if (CuntDownLabel == null) return;
            CuntDownLabel.text = string.Format(TimePlan, cdTime);
            CuntDownLabel.gameObject.SetActive(true);
        }

        private HandUpItem GetItem(int id)
        {
            var items = ItemsParent.GetChildList();
            int count = items.Count;
            for (int i = 0; i < count; i++)
            {
                var item = items[i].GetComponent<HandUpItem>();
                if (id == item.Id)
                    return item;
            }
            return null;
        }

        private HandUpItem GetItem(string userName)
        {
            var items = ItemsParent.GetChildList();
            int count = items.Count;
            for (int i = 0; i < count; i++)
            {
                var item = items[i].GetComponent<HandUpItem>();
                if (userName == item.NickName)
                    return item;
            }
            return null;
        }

        public HandUpItem ItemPrefab;

        public UIGrid ItemsParent;

        /// <summary>
        /// 创建和初始化解散Item
        /// </summary>

        void CreateHundItems()
        {
            var playerList = App.GameData.PlayerList;
            int len = playerList.Length;
            int childCount = ItemsParent.transform.childCount;
            if (childCount == len)
            {
                var items = ItemsParent.transform.GetComponentsInChildren<HandUpItem>();
                for (int i = 0; i < childCount; i++)
                {
                    items[i].SetItemType(0);
                }
                return;
            }

            //如果有创建,销毁所有创建物体
            if (childCount > 0) ItemsParent.transform.DestroyChildren();

            var parent = ItemsParent.transform;
            for (int i = 0; i < len; i++)
            {
                var info = playerList[i].Info;
                if (info == null) continue;

                var item = Instantiate(ItemPrefab);
                item.transform.parent = parent;
                item.transform.localScale = Vector3.one;
                item.gameObject.SetActive(true);
                item.SetItem(info);
            }

            ItemsParent.repositionNow = true;
            ItemsParent.Reposition();
        }

        /// <summary>
        /// 当已经进入总结算阶段时，关闭投票界面
        /// </summary>
        /// <param name="args"></param>
        void OnRoomGameOverEvt(DdzbaseEventArgs args)
        {
            StartCoroutine(HideHandUpPanel(2f));
         
        }


        /// <summary>
        /// 开始计时
        /// </summary>
        /// <returns></returns>
        private IEnumerator CuntDownTime(int cdTime)
        {
            var curTime = cdTime;
            while (curTime >= 0)
            {
                if (curTime < 0) curTime = 0;
                SetTime(curTime);
                yield return new WaitForSeconds(1f);
                curTime--;
                if (curTime <= 0) yield break;
            }
        }

        /// <summary>
        /// delaytime时间后隐藏投票界面
        /// </summary>
        /// <param name="delaytime"></param>
        /// <returns></returns>
        private IEnumerator HideHandUpPanel(float delaytime)
        {
            yield return new WaitForSeconds(delaytime);

            StopAllCoroutines();
            HandupView.SetActive(false);
        }

        /// <summary>
        /// 点击同意解散
        /// </summary>
        public void OnClickConFirmBtn()
        {
            App.GetRServer<DdzGameServer>().StartHandsUp(3);
        }
                     
        /// <summary>
        /// 点击拒绝解散
        /// </summary>
        public void OnClickRefuseBtn()
        {
            App.GetRServer<DdzGameServer>().StartHandsUp(-1);
        }

        protected void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}
