/** 
 *文件名称:     YxTools.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2018-02-05 
 *描述:         简单扩展类
 *历史记录: 
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using com.yxixia.utile.Utiles;
using com.yxixia.utile.YxDebug;
using LitJson;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.Controller;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Common.Utils
{
    public static class YxTools
    {

        #region Data Function :处理数据

        /// <summary>
        /// 获得请求的cacheKey
        /// </summary>
        /// <param name="mainAction"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string GetCacahKey(string mainAction, Dictionary<string, object> param)
        {
            List<string> cacheData = new List<string>();
            var id = App.UserId;
            if (!string.IsNullOrEmpty(id))
            {
                cacheData.Add(id);
            }
            cacheData.Add(mainAction);
            cacheData.AddRange(param.Keys);
            cacheData.AddRange(param.Values.Select(item => item.ToString()));
            return cacheData.Aggregate(string.Empty, (current, item) => current + item);
        }
        /// <summary>
        /// 发送请求时带缓存，原来的格式太长了,将某些可省参数放到后面，简化请求输入
        /// </summary>
        /// <param name="mainCode"></param>
        /// <param name="param"></param>
        /// <param name="successCall"></param>
        /// <param name="cacheKey"></param>
        /// <param name="hasMessageBox"></param>
        /// <param name="hasWait"></param>
        /// <param name="errorCall"></param>
        public static void SendActionWithCacheKey(string mainCode, Dictionary<string, object> param, TwCallBack successCall, string cacheKey = null, bool hasMessageBox = true, bool hasWait = true, TwCallBack errorCall = null)
        {
            Facade.Instance<TwManager>().SendAction
                (
                    mainCode, param, successCall, hasMessageBox, errorCall, hasWait, cacheKey
                );
        }

        /// <summary>
        /// Key分享类型
        /// </summary>
        private const string KeyShareType = "type";
        /// <summary>
        /// Key房间ID
        /// </summary>
        private const string KeyRoomId = "roomid";
        /// <summary>
        /// Key 微信分享事件
        /// </summary>
        private const string KeyShareEvent = "event";
        /// <summary>
        /// Key 房间玩法
        /// </summary>
        private const string KeyRoomRule = "roomRule";
        /// <summary>
        /// Key 分享平台
        /// </summary>
        public const string KeySharePlat = "sharePlat";
        /// <summary>
        /// Key 茶馆ID
        /// </summary>
        public const string KeyTeaId = "TeaId";
        /// <summary>
        /// Value微信邀请好友type值
        /// </summary>
        private const int ValueShareType = 0;
        /// <summary>
        /// Value微信分享事件
        /// </summary>
        private const string ValueShareEvent = "findroom";
        /// <summary>
        /// Value微信邀请好友type值
        /// </summary>
        private const int ValueSharePlatType = 0;

        /// <summary>
        /// 微信邀请好友(分享游戏数据)
        /// </summary>
        /// <param name="roomId">房间号</param>
        /// <param name="roomInfo">房间信息</param>
        /// <param name="gameKey">gamekey</param>
        /// <param name="teaId">茶馆ID</param>
        public static void ShareFriend(string roomId, string roomInfo,string gameKey="",string teaId="")
        {
            if (CheckWeChat())
            {
                if (string.IsNullOrEmpty(gameKey))
                {
                    gameKey = App.GameKey;
                }
                var dic = new Dictionary<string, object>
                {
                    {KeyShareType, ValueShareType},
                    {KeyRoomId, roomId},
                    {KeyShareEvent, ValueShareEvent},
                    {KeyRoomRule, roomInfo},
                    {KeySharePlat,ValueSharePlatType}
                };
                if (!string.IsNullOrEmpty(KeyTeaId))
                {
                    dic.Add(KeyTeaId, teaId);
                }
                UserController.Instance.GetShareInfo(dic, info =>
                {
                    Facade.Instance<WeChatApi>().ShareContent(info);
                }, ShareType.Website, SharePlat.WxSenceSession, null, gameKey);
            }
        }

        /// <summary>
        /// 截图分享
        /// </summary>
        /// <param name="plat">分享平台</param>
        /// <param name="waitInstruction"></param>
        /// <param name="gameKey">gameKey</param>
        public static IEnumerator ShareSceenImage(SharePlat plat, YieldInstruction waitInstruction = null,string gameKey="")
        {
            var path = App.UI.CaptureScreenshot();
            yield return waitInstruction;
            var startTime = DateTime.Now;
            while (!File.Exists(path))
            {
                yield return null;
                if ((DateTime.Now - startTime).Seconds > 5)
                {
                    yield break;
                }
            }
            ShareImageWithGameKey(path, plat,gameKey);
        }

        /// <summary>
        /// 分享截图
        /// </summary>
        /// <param name="path"></param>
        /// <param name="plat"></param>
        /// <param name="gameKey"></param>
        public static void ShareImageWithGameKey(string path,SharePlat plat,string gameKey = "")
        {
            if (string.IsNullOrEmpty(gameKey))
            {
                gameKey = App.GameKey;
            }
            if (CheckWeChat())
            {
                UserController.Instance.GetShareInfo((info) =>
                {
                    Facade.Instance<WeChatApi>().ShareContent(info);
                }, ShareType.Image, plat, path, gameKey);
            }
        }

        /// <summary>
        /// 检测微信是否可用
        /// </summary>
        /// <returns></returns>
        private static bool CheckWeChat()
        {
            var api = Facade.Instance<WeChatApi>();
            return api.InitWechat()&& api.CheckWechatValidity();
        }

        /// <summary>
        /// Key 加入娱乐房房间类型
        /// </summary>
        private const string KeyTypeId= "typeId";
        /// <summary>
        /// Key 加入娱乐房房间GameKey
        /// </summary>
        private const string KeyGameKey = "gameKey";
        /// <summary>
        /// Key 加入娱乐房房间门槛校验
        /// </summary>
        private const string KeyGoldJoin = "room.goldJoinRoom";

        /// <summary>
        /// 金币房加入游戏
        /// </summary>
        /// <param name="gamekey"></param>
        /// <param name="roomId"></param>
        /// <param name="success"></param>
        public static void GoldJoinRoom(string gamekey,string roomId,TwCallBack success)
        {
            var dic=new Dictionary<string,object>();
            dic[KeyTypeId] = roomId;
            dic[KeyGameKey] = gamekey;
            Facade.Instance<TwManager>().SendAction(KeyGoldJoin, dic,success);
        }

        /// <summary>
        /// 解析int类型数据
        /// </summary>
        /// <param name="dic">数据源</param>
        /// <param name="value">接收源</param>
        /// <param name="key">解析Key</param>
        /// <param name="defaultValue">默认值</param>
        public static void TryGetValueWitheKey(this Dictionary<string, object> dic, out int value, string key,int defaultValue=0)
        {
            value = dic.ContainsKey(key) ? int.Parse(dic[key] == null ? defaultValue.ToString() : dic[key].ToString()) : defaultValue;
        }

        /// <summary>
        /// 解析short类型数据
        /// </summary>
        /// <param name="dic">数据源</param>
        /// <param name="value">接收源</param>
        /// <param name="key">解析Key</param>
        /// <param name="defaultValue">默认值</param>
        public static void TryGetValueWitheKey(this Dictionary<string, object> dic, out short value, string key, short defaultValue = 0)
        {
            value = dic.ContainsKey(key) ? short.Parse(dic[key] == null ? defaultValue .ToString(): dic[key].ToString()) : defaultValue;
        }

        /// <summary>
        /// 解析long类型数据
        /// </summary>
        /// <param name="dic">数据源</param>
        /// <param name="value">接收源</param>
        /// <param name="key">解析Key</param>
        /// <param name="defaultValue">默认值</param>
        public static void TryGetValueWitheKey(this Dictionary<string, object> dic, out long value, string key,long defaultValue=0)
        {
            value = dic.ContainsKey(key) ? long.Parse(dic[key] == null ? defaultValue.ToString() : dic[key].ToString()) : defaultValue;
        }

        /// <summary>
        /// 解析string类型数据
        /// </summary>
        /// <param name="dic">数据源</param>
        /// <param name="value">接收源</param>
        /// <param name="key">解析Key</param>
        /// <param name="defaultValue">默认值</param>
        public static void TryGetValueWitheKey(this Dictionary<string, object> dic,  out string value, string key,string defaultValue="")
        {
            value = dic.ContainsKey(key) ? (dic[key] == null ? defaultValue : dic[key].ToString()) : defaultValue;
        }

        /// <summary>
        /// 解析bool类型数据
        /// </summary>
        /// <param name="dic">数据源</param>
        /// <param name="value">接收源</param>
        /// <param name="key">解析Key</param>
        /// <param name="defaultValue">默认值</param>
        public static void TryGetValueWitheKey(this Dictionary<string, object> dic, out bool value, string key,bool defaultValue=false)
        {
            value = dic.ContainsKey(key)?bool.Parse(dic[key] == null ? defaultValue.ToString() : dic[key].ToString()):defaultValue;
        }

        /// <summary>
        /// 解析float类型数据
        /// </summary>
        /// <param name="dic">数据源</param>
        /// <param name="value">接收源</param>
        /// <param name="key">解析Key</param>
        /// <param name="defaultValue">默认值</param>
        public static void TryGetValueWitheKey(this Dictionary<string, object> dic, out float value, string key,float defaultValue=0)
        {
            value = dic.ContainsKey(key) ? float.Parse(dic[key] == null ? defaultValue.ToString(CultureInfo.InvariantCulture) : dic[key].ToString()) : defaultValue;
        }

        /// <summary>
        /// 解析double类型数据
        /// </summary>
        /// <param name="dic">数据源</param>
        /// <param name="value">接收源</param>
        /// <param name="key">解析Key</param>
        /// <param name="defaultValue">默认值</param>
        public static void TryGetValueWitheKey(this Dictionary<string, object> dic, out double value, string key,double defaultValue=0)
        {
            value = dic.ContainsKey(key) ? double.Parse(dic[key] == null ? defaultValue.ToString(CultureInfo.InvariantCulture) : dic[key].ToString()) : defaultValue;
        }

        /// <summary>
        /// 解析字典型数据（string,obj）类型数据
        /// </summary>
        /// <param name="dic">数据源</param>
        /// <param name="value">接收源</param>
        /// <param name="key">解析Key</param>
        /// <param name="compatble">数据兼容模式（服务器返回数据默认为object类型，兼容模式直接将对应数据批量转换为需求数据，如dic<string,int>, dic<string,float>等）</param>
        public static bool TryGetValueWitheKey<T>(this Dictionary<string, object> dic, out Dictionary<string, T> value, string key, bool compatble = false)
        {
            var getState = false;
            if (dic.ContainsKey(key))
            {
                var data = dic[key] as Dictionary<string, T>;
                if (compatble)
                {
                    data = ((Dictionary<string, object>) dic[key]).ToDictionary(item=>item.Key,item=>(T)item.Value);
                }
                if (data!=null)
                {
                    value  = data;
                    getState = true;
                }
                else
                {
                    value = new Dictionary<string, T>();
                }
                
            }
            else
            {
                value = new Dictionary<string, T>();
            }

            return getState;
        }

        /// <summary>
        /// 解析list(obj)类型数据
        /// </summary>
        /// <param name="dic">数据源</param>
        /// <param name="value">接收源</param>
        /// <param name="key">解析Key</param>
        /// <param name="compatble">数据兼容模式（服务器返回数据默认为object类型，兼容模式直接将对应数据批量转换为需求数据，如list<int>, list<float>等）</param>
        public static bool TryGetValueWitheKey<T>(this Dictionary<string, object> dic, out List<T> value, string key,bool compatble=false)
        {
            var getState=false;
            if (dic.ContainsKey(key))
            {
                var changeData = dic[key] as List<T>;
                if (compatble)
                {
                    changeData= ((List<object>) dic[key]).ConvertAll(item=>(T)item);
                }
                if (changeData  == null)
                {
                    value = new List<T>();
                }
                else
                {
                   
                    value = changeData;
                    getState = true;
                }
            }
            else
            {
                value = new List<T>();
            }
            return getState;
        }

        /// <summary>
        /// 获取消耗类型（忽略大小写，目前数据库中的部分Gold字段使用了gold来存储，本地转换时做兼容，忽略大小写问题）
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static EnumCostType GetCostTypeByString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return EnumCostType.TempCoin;
            }
            return (EnumCostType)Enum.Parse(typeof(EnumCostType), value, true);
        }

        /// <summary>
        /// 金币需要做显示特殊处理
        /// </summary>
        /// <param name="costType">coin_a或1时为金币</param>
        /// <param name="type"></param>
        /// <param name="defaultType"></param>
        /// <returns></returns>
        public static YxBaseLabelAdapter.YxELabelType GetLabelTypeByCostType(string costType, YxBaseLabelAdapter.YxELabelType type = YxBaseLabelAdapter.YxELabelType.ReduceNumberWithUnit, YxBaseLabelAdapter.YxELabelType defaultType = YxBaseLabelAdapter.YxELabelType.Normal)
        {
            switch (costType)
            {
                case "coin_a":
                case "1":
                    return type; 
            }
            return defaultType; 
        }



        #endregion

        #region Engine Function:引擎相关

        /// <summary>
        /// 设置UISprite Name
        /// </summary>
        /// <param name="component">sprite</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool TrySetComponentValue(this UISprite component, string value)
        {
            if (component)
            {
                component.spriteName = value;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置UILabel text
        /// </summary>
        /// <param name="component">label</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TrySetComponentValue(this UILabel component, string value)
        {
            if (component)
            {
                component.text = value;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="component"></param>
        /// <param name="value"></param>
        /// <param name="costType"></param>
        /// <param name="format"></param>
        /// <param name="type"></param>
        /// <param name="defaultType"></param>
        /// <returns></returns>
        public static bool TrySetComponentValue(this YxBaseLabelAdapter component, long value, string costType = "", string format = "{0}", YxBaseLabelAdapter.YxELabelType defaultType = YxBaseLabelAdapter.YxELabelType.Normal,YxBaseLabelAdapter.YxELabelType type = YxBaseLabelAdapter.YxELabelType.ReduceNumberWithUnit)
        {
            if (component)
            {
                component.LabelType = GetLabelTypeByCostType(costType,type,defaultType);
                component.ContentFormat = format;
                component.Text(value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 将prefab 放置到对应父级下
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="item"></param>
        /// <param name="fresh"></param>
        public static void AddChildToParent(this GameObject parent, GameObject item, bool fresh = false)
        {
            if (item != null && parent != null)
            {
                Transform t = item.transform;
                t.parent = parent.transform;
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                if (fresh)
                {
                    item.SetActive(false);
                    item.SetActive(true);
                }
            }
        }

        /// <summary>
        /// 设置文本值
        /// </summary>
        /// <param name="component"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TrySetComponentValue(this YxBaseLabelAdapter component, string value)
        {
            if (component)
            {
                component.Text(value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置UIInput value
        /// </summary>
        /// <param name="component">input</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TrySetComponentValue(this UIInput component, string value)
        {
            if (component)
            {
                component.value = value;
                return true;
            }
            return false;
        }
        /// <summary>
        /// YxBaseSpriteAdapter value
        /// </summary>
        /// <param name="component">input</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TrySetComponentValue(this YxBaseSpriteAdapter component, string value)
        {
            if (component)
            {
                component.SetSpriteName(value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置UIInput Name
        /// </summary>
        /// <param name="component">gameobject</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TrySetComponentValue(this GameObject component, bool value)
        {
            if (component)
            {
                component.SetActive(value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置 图片内容
        /// </summary>
        /// <param name="component"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TrySetComponentValue(this YxBaseTextureAdapter component, Texture2D value)
        {
            if (component)
            {
                component.SetTexture(value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 等待本帧结束执行
        /// </summary>
        /// <param name="callBacks"></param>
        /// <param name="waitAFrame">是否需要等待本帧结束执行</param>
        /// <returns></returns>
        public static IEnumerator WaitExcuteCalls(this List<EventDelegate> callBacks, bool waitAFrame = false)
        {
            if (waitAFrame)
            {
                yield return new WaitForEndOfFrame();
            }
            if (callBacks != null && callBacks.Count > 0)
            {
                foreach (var callBack in callBacks)
                {
                    callBack.Execute();
                }
            }
        }

        /// <summary>
        /// 获取对应父级的子物体
        /// </summary>
        /// <param name="index"></param>
        /// <param name="prefabView"></param>
        /// <param name="tranParent"></param>
        /// <returns></returns>
        public static YxView GetChildView(this Transform tranParent, int index, YxView prefabView )
        {
            if (tranParent.childCount > index)
            {
                var returnView = tranParent.GetChild(index).GetComponent<YxView>();
                if (returnView)
                {
                    returnView.gameObject.SetActive(true);
                }
                else
                {
                    YxDebug.LogError(string.Format("Child Item is not a YxView,index is :{0},please check again!",index));
                    returnView = YxWindowUtils.CreateItem(prefabView, tranParent);
                }
                return returnView;
            }
            return YxWindowUtils.CreateItem(prefabView, tranParent);
        }

        /// <summary>
        /// 打开窗口并传递数据
        /// </summary>
        /// <param name="mainView">主窗口</param>
        /// <param name="windowName">打开窗口名称</param>
        /// <param name="data">传递数据</param>
        /// <param name="callBack">数据回调</param>
        public static YxWindow OpenWindowWithData(this YxView mainView,string windowName,object data, Action<object> callBack = null)
        {
            if (string.IsNullOrEmpty(windowName))
            {
                return null;
            }
            var mainWindow = mainView as YxWindow;
            var window = mainWindow ? mainWindow.CreateChildWindow(windowName) : YxWindowManager.OpenWindow(windowName);
            if (window)
            {
                window.UpdateViewWithCallBack(data, callBack);
            }
            return window;
        }
        /// <summary>
        /// 值复制
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TTarget Copy<TTarget>(this object source)
        {
            return JsonMapper.ToObject<TTarget>(JsonMapper.ToJson(source));
        }

        /// <summary>
        /// 比较两个对象的值是否相等
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool ValueEqual(this object first, object second)
        {
            if (first == second)
            {
                return true;
            }
            else
            {
                if (first != null && second != null)
                {
                    if (first.GetType() != second.GetType())
                    {
                        return false;
                    }
                    else
                    {
                        return JsonMapper.ToJson(first).Equals(JsonMapper.ToJson(second));
                    }
                }
                return false;
            }
        }
        #endregion
    }

    /// <summary>
    /// 资源消耗类型
    /// </summary>
    public enum EnumCostType
    {
        Gold,                       //金币
        Cash,                       //元宝
        TempCoin,                   //临时积分（开房游戏内默认值）
        GroupCoin                   //群积分
    }
}
