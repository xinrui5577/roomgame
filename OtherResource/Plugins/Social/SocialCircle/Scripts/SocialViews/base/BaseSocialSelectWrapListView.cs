using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.View;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base
{
    public class BaseSocialSelectWrapListView : BaseSocialWrapListView
    {
        [Tooltip("选择状态")]
        public SelectItemState SelectState = SelectItemState.SingleSelect;
        [Tooltip("多选状态Action")]
        public List<EventDelegate> MulSelectAction = new List<EventDelegate>();
        public bool InMulSelect
        {
            get { return SelectState == SelectItemState.MulSelect; }
        }

        protected int GetIntSelectType
        {
            get { return (int) SelectState; } 
        }

        /// <summary>
        /// 选中的组Id
        /// </summary>
        public string SelectGroupId
        {
            get
            {
                if (IdsDataDic.ContainsKey(LastSelectID))
                {
                    var itemData = IdsDataDic[LastSelectID];
                    string groupId;
                    itemData.TryGetValueWitheKey(out groupId, SocialTools.KeyId);
                    if (string.IsNullOrEmpty(groupId))
                    {
                        itemData.TryGetValueWitheKey(out groupId, SocialTools.KeyUseInfoGroupId);
                    }
                    return groupId;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// 选中的ImId
        /// </summary>
        public string SelectImId
        {
            get
            {
                if (IdsDataDic.ContainsKey(LastSelectID))
                {
                    var itemData = IdsDataDic[LastSelectID];
                    string imId;
                    itemData.TryGetValueWitheKey(out imId, SocialTools.KeyOwnerId);
                    if (string.IsNullOrEmpty(imId))
                    {
                        itemData.TryGetValueWitheKey(out imId, SocialTools.KeyImId);
                    }
                    return imId;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// 选中的玩家ID
        /// </summary>
        public string SelectUserId
        {
            get
            {
                if (IdsDataDic.ContainsKey(LastSelectID))
                {
                    var itemData = IdsDataDic[LastSelectID];
                    string userId;
                    itemData.TryGetValueWitheKey(out userId, SocialTools.KeyOtherId);
                    if (string.IsNullOrEmpty(userId))
                    {
                        itemData.TryGetValueWitheKey(out userId, SocialTools.KeyUserInfoUserId);
                    }
                    return userId;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// 拉黑某人
        /// </summary>
        /// <param name="id"></param>
        public virtual void Defriend(string id)
        {
            if (id == Manager.UserImId)
            {
                YxMessageBox.Show(SocialTools.KeyNoticeSetSelfIntoBlack);
                return;
            }
            TalkCenter.DeFriend(id);
        }

        public virtual void ChangeMulSelectState()
        {
            SelectState = InMulSelect ? SelectItemState.SingleSelect : SelectItemState.MulSelect;
            if (SelectState == SelectItemState.SingleSelect)
            {
                _lastSelectId =string.Empty;
            }
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(MulSelectAction.WaitExcuteCalls());
            }
            ResetFreshView();
        }

        private void ResetPatchData()
        {
            IdsDataDic.Foreach(delegate (string key,Dictionary<string, object> value)
                {
                    ChangeItemSelectData(key, false,true);
                }, true
            );
        }

        protected override Dictionary<string, object> PatchData(Dictionary<string, object> data)
        {
            var dic =base.PatchData(data);
            if (!dic.ContainsKey(BaseSocialSelectWrapItem.KeySelectStatus))
            {
                dic[BaseSocialSelectWrapItem.KeySelectStatus] = false;
            }
            if (!dic.ContainsKey(BaseSocialSelectWrapItem.KeySelectType))
            {
                dic[BaseSocialSelectWrapItem.KeySelectType] = GetIntSelectType;
            }
            return dic;
        }

        /// <summary>
        /// 更改选中状态数据
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="select"></param>
        /// <param name="typeChange"></param>
        /// <returns></returns>
        protected virtual Dictionary<string, object> SetSelectData(Dictionary<string, object> dic,bool select,bool typeChange)
        {
            dic[BaseSocialSelectWrapItem.KeySelectStatus] = select;
            if (typeChange)
            {
                dic[BaseSocialSelectWrapItem.KeySelectType] = GetIntSelectType;
            }
            return dic;
        }

        protected virtual void ChangeItemSelectData(string id,bool select,bool typeChange = false)
        {
            if (IdsDataDic.ContainsKey(id))
            {
                IdsDataDic[id] = SetSelectData(IdsDataDic[id], select, typeChange);
            }
        }

        protected override void BeforeItemFresh(BaseSocialWrapItem item)
        {
            base.BeforeItemFresh(item);
            var selectItem = (BaseSocialSelectWrapItem)item;
            if (selectItem)
            {
                selectItem.OnSelectStateChange = OnItemSelectChange;
            }
        }

        public string LastSelectID
        {
            get { return _lastSelectId; }
        }
        /// <summary>
        /// 单选状态最后Id
        /// </summary>
        private string _lastSelectId=string.Empty;
        /// <summary>
        /// Item 选中状态变化
        /// </summary>
        /// <param name="onlyId"></param>
        /// <param name="selectValue"></param>
        protected virtual void OnItemSelectChange(string onlyId, bool selectValue)
        {
            ChangeItemSelectData(onlyId, selectValue);
            if (SelectState == SelectItemState.SingleSelect)
            {
                if (selectValue)
                {
                    if(!string.IsNullOrEmpty(_lastSelectId))
                    {
                        ChangeItemSelectData(_lastSelectId, false);
                    }
                    _lastSelectId = onlyId;
                    WrapContent.OnItemModify(ModifyType.Update);
                }
                else
                {
                    _lastSelectId = string.Empty;
                }
            }
        }

        protected override void SetIdsDataDic(Dictionary<string, Dictionary<string, object>> setDic)
        {
            foreach (var item in IdsDataDic)
            {
                if (setDic.ContainsKey(item.Key)&& item.Value.ContainsKey(BaseSocialSelectWrapItem.KeySelectStatus) && item.Value.ContainsKey(BaseSocialSelectWrapItem.KeySelectType))
                {
                    setDic[item.Key][BaseSocialSelectWrapItem.KeySelectStatus] = item.Value[BaseSocialSelectWrapItem.KeySelectStatus];
                    setDic[item.Key][BaseSocialSelectWrapItem.KeySelectType] = item.Value[BaseSocialSelectWrapItem.KeySelectType];
                }
            }
            if (!string.IsNullOrEmpty(_lastSelectId)&&!setDic.ContainsKey(_lastSelectId))
            {
                _lastSelectId = string.Empty;
            }
            base.SetIdsDataDic(setDic);
        }

        protected virtual void ResetFreshView()
        {
            ResetPatchData();
            WrapContent.OnItemModify(ModifyType.Update);
        }

        public List<string> GetSelectItems(string selectKey=SocialTools.KeyId)
        {
            var selectList=new List<string>();
            IdsDataDic.Foreach(delegate(string key, Dictionary<string, object> valueDic)
            {
                bool selectValue;
                valueDic.TryGetValueWitheKey(out selectValue,BaseSocialSelectWrapItem.KeySelectStatus);
                if (selectValue&& valueDic.ContainsKey(selectKey))
                {
                    selectList.Add(valueDic[selectKey].ToString());
                }
            });
            return selectList;
        }


        public void ChangePanelState(UIPanel panel, int left, int right, int bottom, int top)
        {
            panel.leftAnchor.absolute = left;
            panel.rightAnchor.absolute = right;
            panel.bottomAnchor.absolute = bottom;
            panel.topAnchor.absolute = top;
        }
    }
}
