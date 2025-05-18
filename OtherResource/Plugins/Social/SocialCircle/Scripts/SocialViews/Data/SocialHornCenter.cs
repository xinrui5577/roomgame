using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Manager;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data
{
    //喇叭信息中心
    public class SocialHornCenter : BaseMono
    {
        /// <summary>
        /// 是否初始化
        /// </summary>
        private bool _hasInit;
        /// <summary>
        /// 喇叭消息列表
        /// </summary>
        private List<string> _hornList = new List<string>();
        /// <summary>
        /// 未读消息列表
        /// </summary>
        private List<string> _unReadMessageList = new List<string>();
        /// <summary>
        /// 喇叭消息数据
        /// </summary>
        private Dictionary<string, Dictionary<string, object>> _hornInfos =new Dictionary<string, Dictionary<string, object>>();
        public SocialHornCenter InitCenter()
        {
            if (!_hasInit)
            {
                InitListeners();
                _hasInit = true;
            }
            return this;
        }

        private SocialMessageManager _manager;
        private SocialMessageManager Manager
        {
            get
            {
                if (_manager == null)
                {
                    _manager = Facade.Instance<SocialMessageManager>().InitManager();
                }
                return _manager;
            }
        }
        /// <summary>
        /// 回去喇叭消息数量
        /// </summary>
        public int GetHornUnReadCount
        {
            get
            {
                return _unReadMessageList.Count;
            }
        }

        void InitListeners()
        {
            //离线喇叭消息
            Manager.AddEventListeners<Dictionary<string, object>>(SocialTools.KeyActionOutLineHornList, OnReceiveOutLineHornList);
            //喇叭消息推送
            Manager.AddEventListeners<Dictionary<string, object>>(SocialTools.KeyActionHornUpdate, OnHornInfoUpdate);
        }

        void RemoveListeners()
        {
            //离线喇叭消息
            Manager.RemoveEventListeners<Dictionary<string, object>>(SocialTools.KeyActionOutLineHornList, OnReceiveOutLineHornList);
            //喇叭消息推送
            Manager.RemoveEventListeners<Dictionary<string, object>>(SocialTools.KeyActionHornUpdate, OnHornInfoUpdate);
        }

        /// <summary>
        /// 获得离线喇叭消息
        /// </summary>
        public void GetOutLineList()
        {
            _hornList.Clear();
            _hornInfos.Clear();
            _unReadMessageList.Clear();
            if (Manager.EntryNum==0)//亲友圈首次处理
            {
                SetNewHornMessage(_hornList.Count.ToString(), SocialTools.KeyNoticeSocialFirstContent, new List<object>() { UserInfoModel.Instance.UserInfo.NickM });
            }
            Manager.SendRequest(SocialTools.KeyActionOutLineHornList);
        }
        /// <summary>
        /// 获取显示数据
        /// </summary>
        public void GetShowList()
        {
            SendDatasToLocal();
        }

        /// <summary>
        /// 离线消息
        /// </summary>
        /// <param name="hornListdData"></param>
        private void OnReceiveOutLineHornList(Dictionary<string, object> hornListdData)
        {
            List<object> hornData;
            hornListdData.TryGetValueWitheKey(out hornData, SocialTools.KeyHornData);
            List<string> hornFormat;
            hornListdData.TryGetStringListWithKey(out hornFormat, SocialTools.KeyHornFormat);
            for (int i = 0,length= hornData.Count; i < length; i++)
            {
                var itemDic= hornData[i] as Dictionary<string,object>;
                if (itemDic!=null)
                {
                    var key = _hornList.Count.ToString();
                    List<object> nickNames;
                    itemDic.TryGetValueWitheKey(out nickNames, SocialTools.KeyNickName);
                    int type;
                    itemDic.TryGetValueWitheKey(out type, SocialTools.KeyHornType);
                    if (type<hornFormat.Count)
                    {
                        SetNewHornMessage(key, hornFormat[type], nickNames);
                    }
                }
            }
            SendDatasToLocal();
        }
        /// <summary>
        /// 推送喇叭消息
        /// </summary>
        /// <param name="hornInfo"></param>
        private void OnHornInfoUpdate(Dictionary<string, object> hornInfo)
        {
            var key = _hornList.Count.ToString();
            string hornFormat;
            hornInfo.TryGetValueWitheKey(out hornFormat, SocialTools.KeyHornFormat);
            List<object> nickNames;
            hornInfo.TryGetValueWitheKey(out nickNames, SocialTools.KeyNickName);
            SetNewHornMessage(key, hornFormat,nickNames);
            SendDatasToLocal();
        }

        /// <summary>
        /// 新增红点消息
        /// </summary>
        /// <param name="key">主键</param>
        /// <param name="hornFormat">红点消息格式</param>
        /// <param name="nickNames">昵称集合</param>
        private void SetNewHornMessage(string key,string hornFormat, List<object> nickNames)
        {
            _hornList.Add(key);
            _unReadMessageList.Add(key);
            _hornInfos[key] = new Dictionary<string, object>()
            {
                { SocialTools.KeyId,key},{ SocialTools.KeyMessageUnRead,true}, {SocialTools.KeyMessageContent,string.Format(hornFormat, nickNames.ToArray())}
            };
        }

        /// <summary>
        /// 设置小喇叭列表消息已读
        /// </summary>
        public void SetHornListReadFinish(string readFinishId)
        {
            if (_hornInfos.ContainsKey(readFinishId))
            {
                _hornInfos[readFinishId][SocialTools.KeyMessageUnRead] = false;
                if (_unReadMessageList.Contains(readFinishId))
                {
                    _unReadMessageList.Remove(readFinishId);
                }
                SendDatasToLocal();
            }
            else
            {
                Debug.LogError("本地喇叭消息数据中不存在对应ID:" + readFinishId);
            }
        }

        /// <summary>
        /// 发送喇叭消息数据到本地
        /// </summary>
        private void SendDatasToLocal()
        {
            Manager.DispatchLocalEvent(SocialTools.KeyActionHornList,GetUnReadMessage());
        }

        private Dictionary<string,object> GetUnReadMessage()
        {
            var count = _unReadMessageList.Count;
            if (count>0)
            {
                var lastKey = _unReadMessageList.Last();
                if(_hornInfos.ContainsKey(lastKey))
                {
                    return _hornInfos[lastKey];
                }
            }
            return new Dictionary<string, object>();
        }

        /// <summary>
        /// 消息重置
        /// </summary>
        public void ClearLocalHorn()
        {
            ResetData();
            SendDatasToLocal();
        }

        /// <summary>
        /// 读取新的喇叭消息
        /// </summary>
        public void ReadNewHornInfo()
        {
            SendDatasToLocal();
        }

        /// <summary>
        /// 重置小喇叭列表
        /// </summary>
        public void ResetData()
        {
            _hornList.Clear();
            _hornInfos.Clear();
            _unReadMessageList.Clear();
        }

        void OnApplicationQuit()
        {
            ResetData();
        }

        public override void OnDestroy()
        {
            RemoveListeners();
            base.OnDestroy();
        }
    }
}
