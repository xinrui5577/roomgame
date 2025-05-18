using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.TaskWindows
{
    /// <summary>
    /// 签到视图
    /// </summary>
    public class TaskSignView : TaskBasseView
    {
        /// <summary>
        /// 签到按钮
        /// </summary>
        [Tooltip("签到按钮")]
        public UIButton BtnSign;
        /// <summary>
        /// 当前签到日期
        /// </summary>
        [Tooltip("当前签到日期")]
        public UILabel CurDayLabel;
        /// <summary>
        /// 高亮
        /// </summary>
        [Tooltip("高亮,可无")]
        public GameObject HighlightMark;
        /// <summary>
        /// 当前签到日子的格式
        /// </summary>
        [Tooltip("当前签到日子的格式")]
        public string CurDayFormat = "{0}";
        /// <summary>
        /// 背景种类
        /// </summary>
        [Tooltip(" 背景种类，true 表示连续，与SignBgName、NoSignBgName、HasSignBgName配合使用")]
        public bool Sequent = false;
        /// <summary>
        /// 可以签到的背景
        /// </summary>
        [Tooltip("可以签到的背景")]
        public string SignBgName = "sign_box";
        /// <summary>
        /// 不可签到的背景
        /// </summary>
        [Tooltip("不可签到的背景")]
        public string NoSignBgName = "sign_box_on";
        /// <summary>
        /// 已经签到背景
        /// </summary>
        [Tooltip("已经签到背景")]
        public string HasSignBgName = "sign_box_on";
        /// <summary>
        /// 可签状态
        /// </summary>
        [Tooltip("可签状态")]
        public string SignState = "";
        /// <summary>
        /// 没有签到状态
        /// </summary>
        [Tooltip("没有签到状态")]
        public string NoSignState = "";
        /// <summary>
        /// 已经签到
        /// </summary>
        [Tooltip("已经签到")]
        public string HasSignState = "";
        /// <summary>
        /// 一个item预制体
        /// </summary>
        [Tooltip("一个item预制体")]
        public TaskSignItemView ItemPerfab;
        /// <summary>
        /// item Grid预制体
        /// </summary>
        [Tooltip("一个item预制体")]
        public UIGrid ItemGridPerfab;
        /// <summary>
        /// 数据
        /// </summary>
        [Tooltip("数据")]
        public SignItemData[] ItemDatas;
        private readonly List<TaskSignItemView> _items = new List<TaskSignItemView>();

        private UIGrid _curItemParent;

        protected override void OnStart()
        {
            base.OnStart();
            if (BtnSign != null) BtnSign.isEnabled = false;
            Facade.Instance<TwManager>().SendAction("signInDays", new Dictionary<string, object>(), OnGetInitData);
        }

        /// <summary>
        /// 发送签到
        /// </summary>
        public void OnSignBtn()
        {
            Facade.Instance<TwManager>().SendAction("signIn", new Dictionary<string, object>(), OnSignSuccess);
        }

        /// <summary>
        /// 成功签到
        /// </summary>
        /// <param name="msg"></param>
        protected virtual void OnSignSuccess(object msg)
        {
            OnGetInitData(msg);
            UserController.Instance.GetUserDate();
            var pram = (IDictionary)msg;//pram参数记忆体
            if (pram == null) return;
            if (!pram.Contains("signInfo")) return;
            var signInfo = pram["signInfo"];
            if (signInfo == null) return;
            YxMessageBox.Show(signInfo.ToString());
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="msg"></param>
        protected virtual void OnGetInitData(object msg)
        {
            var parm = (IDictionary)msg;
            if (parm == null) return;
            var today = int.Parse(parm["today"].ToString());
            var totalDay = int.Parse(parm["dayCount"].ToString());
            var dayDatas = parm.Contains("awardCfg") ? (List<object>)parm["awardCfg"] : null;

            _items.Clear();
            CreateItemParent();
            if (dayDatas != null) ParseData(dayDatas);
            Bind(totalDay, today); 
            if (CurDayLabel != null) CurDayLabel.text = string.Format(CurDayFormat,totalDay);
            _curItemParent.repositionNow = true;
            _curItemParent.Reposition();
        }

        /// <summary>
        /// 绑定事件
        /// </summary>
        /// <param name="curDay"></param>
        /// <param name="today"></param>
        protected virtual void Bind(int curDay, int today)
        {
            var count = ItemDatas.Length;
            var lastSign = Mathf.Min(curDay, count);
             
            var index = 0;
            //已签
            for (; index < lastSign; index++)
            {
                var item = CreateItem();
                var data = ItemDatas[index];
                data.BgName = Sequent ? string.Format("{0}{1}", HasSignBgName, index) : HasSignBgName;
                data.Type = SignItemData.YxESignType.HasSign;
                data.SignState = HasSignState;
                data.CanSign = false;
                item.UpdateView(data);
                _items.Add(item);
            }
            //可签 
            if (today == 0 && index < count)
            {
                if (BtnSign != null) BtnSign.isEnabled = true;
                var itemSelf = CreateItem();
                var dataSelf = ItemDatas[index];
                dataSelf.BgName = Sequent ? string.Format("{0}{1}", SignBgName, index) : SignBgName;
                dataSelf.Type = SignItemData.YxESignType.CanSign;
                dataSelf.SignState = SignState;
                dataSelf.CanSign = true;
                itemSelf.UpdateView(dataSelf);
                _items.Add(itemSelf);
                AddHighlightMark(itemSelf.transform); 
                index++;
            }
            else
            {
                if (BtnSign != null) BtnSign.isEnabled = false;
                HidHighlightMark();
            }
            
            //不可签
            for (; index < count; index++)
            {
                var item = CreateItem();
                var data = ItemDatas[index];
                data.BgName = NoSignBgName;
                data.CanSign = false;
                data.BgName = Sequent ? string.Format("{0}{1}", NoSignBgName, index) : NoSignBgName;
                data.Type = SignItemData.YxESignType.NotSign;
                data.SignState = NoSignState;
                item.UpdateView(data);
                _items.Add(item);
            } 
        }

        private void AddHighlightMark(Transform parentTs)
        {
            if (HighlightMark == null) return;
            var ts = HighlightMark.transform;
            ts.parent = parentTs;
            ts.localPosition = Vector3.zero;
            ts.localScale = Vector3.one;
            ts.localRotation = Quaternion.identity;
            HighlightMark.gameObject.SetActive(true);
        }

        private void HidHighlightMark()
        {
            if (HighlightMark == null) return;
            HighlightMark.gameObject.SetActive(false);
        }

        protected void ParseData(List<object> datas)
        {
            var count = datas.Count;
            ItemDatas = new SignItemData[count];
            for (var i = 0; i < count; i++)
            {
                var dict = datas[i] as IDictionary;
                if (dict == null) continue;
                var data = new SignItemData
                    {
                        Day = int.Parse(dict["day"].ToString()),
                        Reward = int.Parse(dict["awardCount"].ToString()),
                        RewardType = int.Parse(dict["awardType"].ToString())
                    };
                ItemDatas[i] = data;
            }
        }

        protected TaskSignItemView CreateItem()
        {
            var item = Instantiate(ItemPerfab);
            var ts = item.transform;
            ts.parent = _curItemParent.transform;
            ts.localPosition = Vector3.zero;
            ts.localRotation = Quaternion.identity;
            ts.localScale = Vector3.one;
            item.gameObject.SetActive(true);
            return item;
        }

        private void CreateItemParent()
        {
            if (_curItemParent != null)
            {
                Destroy(_curItemParent.gameObject);
            }
            var perfabTs = ItemGridPerfab.transform;
            _curItemParent = Instantiate(ItemGridPerfab);
            var ts = _curItemParent.transform;
            ts.parent = perfabTs.parent;
            ts.gameObject.SetActive(true);
            ts.localPosition = perfabTs.localPosition;
            ts.localScale = perfabTs.localScale;
            ts.localRotation = perfabTs.localRotation;
        }
    }

    
}
