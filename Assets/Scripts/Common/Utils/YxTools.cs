/** 
 *文件名称:     YxTools.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2018-02-05 
 *描述:         简单工具类,将一些常用的方法放在这里，减少代码冗余
 *历史记录: 
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Common.Utils
{
    public class YxTools
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
            return cacheData.Aggregate("", (current, item) => current + item);
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
            Facade.Instance<TwManger>().SendAction
                (
                    mainCode, param, successCall, hasMessageBox, errorCall, hasWait, cacheKey
                );
        }

        /// <summary>
        /// 微信邀请好友
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="roomInfo"></param>
        public static void ShareFriend(string roomId, string roomInfo)
        {
            Facade.Instance<WeChatApi>().InitWechat();
            var dic = new Dictionary<string, object>();
            dic.Add("type", 0);
            dic.Add("roomid", roomId);
            dic.Add("event", "findroom");
            dic.Add("roomRule", roomInfo);
            dic.Add("sharePlat", 0);
            UserController.Instance.GetShareInfo(dic, info =>
            {
                Facade.Instance<WeChatApi>().ShareContent(info);
            }, ShareType.Website, SharePlat.WxSenceSession, null, App.GameKey);
        }

        /// <summary>
        /// 解析int类型数据
        /// </summary>
        /// <param name="dic">数据源</param>
        /// <param name="value">接收源</param>
        /// <param name="key">解析Key</param>
        public static void TryGetValueWitheKey(Dictionary<string, object> dic, out int value, string key)
        {
            value = dic.ContainsKey(key) ? int.Parse(dic[key].ToString()) : 0;
        }

        /// <summary>
        /// 解析short类型数据
        /// </summary>
        /// <param name="dic">数据源</param>
        /// <param name="value">接收源</param>
        /// <param name="key">解析Key</param>
        public static void TryGetValueWitheKey(Dictionary<string, object> dic, out short value, string key)
        {
            value = dic.ContainsKey(key) ? short.Parse(dic[key].ToString()) : (short)0;
        }

        /// <summary>
        /// 解析long类型数据
        /// </summary>
        /// <param name="dic">数据源</param>
        /// <param name="value">接收源</param>
        /// <param name="key">解析Key</param>
        public static void TryGetValueWitheKey(Dictionary<string, object> dic, out long value, string key)
        {
            value = dic.ContainsKey(key) ? long.Parse(dic[key].ToString()) : 0;
        }

        /// <summary>
        /// 解析string类型数据
        /// </summary>
        /// <param name="dic">数据源</param>
        /// <param name="value">接收源</param>
        /// <param name="key">解析Key</param>
        public static void TryGetValueWitheKey(Dictionary<string, object> dic, out string value, string key)
        {
            value = dic.ContainsKey(key) ? dic[key].ToString() : "";
        }

        /// <summary>
        /// 解析bool类型数据
        /// </summary>
        /// <param name="dic">数据源</param>
        /// <param name="value">接收源</param>
        /// <param name="key">解析Key</param>
        public static void TryGetValueWitheKey(Dictionary<string, object> dic, out bool value, string key)
        {
            value = dic.ContainsKey(key) && bool.Parse(dic[key].ToString());
        }

        /// <summary>
        /// 解析float类型数据
        /// </summary>
        /// <param name="dic">数据源</param>
        /// <param name="value">接收源</param>
        /// <param name="key">解析Key</param>
        public static void TryGetValueWitheKey(Dictionary<string, object> dic, out float value, string key)
        {
            value = dic.ContainsKey(key) ? float.Parse(dic[key].ToString()) : 0;
        }

        /// <summary>
        /// 解析double类型数据
        /// </summary>
        /// <param name="dic">数据源</param>
        /// <param name="value">接收源</param>
        /// <param name="key">解析Key</param>
        public static void TryGetValueWitheKey(Dictionary<string, object> dic, out double value, string key)
        {
            value = dic.ContainsKey(key) ? double.Parse(dic[key].ToString()) : 0;
        }

        /// <summary>
        /// 解析字典型数据（string,obj）类型数据
        /// </summary>
        /// <param name="dic">数据源</param>
        /// <param name="value">接收源</param>
        /// <param name="key">解析Key</param>
        public static void TryGetValueWitheKey(Dictionary<string, object> dic, out Dictionary<string, object> value, string key)
        {
            if (dic.ContainsKey(key))
            {
                var objects = dic[key] as Dictionary<string, object>;
                value = objects ?? new Dictionary<string, object>();
            }
            else
            {
                value = new Dictionary<string, object>();
            }
        }

        /// <summary>
        /// 解析list(obj)类型数据
        /// </summary>
        /// <param name="dic">数据源</param>
        /// <param name="value">接收源</param>
        /// <param name="key">解析Key</param>
        public static void TryGetValueWitheKey(Dictionary<string, object> dic, out List<object> value, string key)
        {
            if (dic.ContainsKey(key))
            {
                var objects = dic[key] as List<object>;
                value = objects ?? new List<object>();
            }
            else
            {
                value = new List<object>();
            }
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
        /// <returns></returns>
        public static YxBaseLabelAdapter.YxELabelType GetLabelTypeByCostType(string costType)
        {
            YxBaseLabelAdapter.YxELabelType returnType;
            switch (costType)
            {
                case "coin_a":
                case "1":
                    returnType = YxBaseLabelAdapter.YxELabelType.ReduceNumberWithUnit;
                    break;
                default:
                    returnType = YxBaseLabelAdapter.YxELabelType.Normal;
                    break;
            }
            return returnType;
        }



        #endregion

        #region Engine Function:引擎相关

        /// <summary>
        /// 设置UISprite Name
        /// </summary>
        /// <param name="component">sprite</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool TrySetComponentValue(UISprite component, string value)
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
        public static bool TrySetComponentValue(UILabel component, string value)
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
        /// <returns></returns>
        public static bool TrySetComponentValue(YxBaseLabelAdapter component, long value, string costType = "", string format = "{0}")
        {
            if (component)
            {
                component.LabelType = GetLabelTypeByCostType(costType);
                component.ContentFormat = format;
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
        public static bool TrySetComponentValue(UIInput component, string value)
        {
            if (component)
            {
                component.value = value;
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
        public static bool TrySetComponentValue(GameObject component, bool value)
        {
            if (component)
            {
                component.SetActive(value);
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
        public static IEnumerator WaitExcuteCalls(List<EventDelegate> callBacks, bool waitAFrame = false)
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
        /// 清除父级所有子物体
        /// </summary>
        /// <param name="trans"></param>
        public static void ClearChildren(Transform trans)
        {
            while (trans.childCount > 0)
            {
                UnityEngine.Object.DestroyImmediate(trans.GetChild(0));
            }
        }

        /// <summary>
        /// 获取对应父级的子物体
        /// </summary>
        /// <param name="index"></param>
        /// <param name="prefabView"></param>
        /// <param name="tranParent"></param>
        /// <returns></returns>
        public static YxView GetChildView(int index, YxView prefabView, Transform tranParent)
        {
            if (tranParent.childCount > index)
            {
                var returnView = tranParent.GetChild(index).GetComponent<YxView>();
                returnView.gameObject.SetActive(true);
                return returnView;
            }
            return YxWindowUtils.CreateItem(prefabView, tranParent);
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
