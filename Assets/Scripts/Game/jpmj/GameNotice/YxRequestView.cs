/** 
 *文件名称:     YxRequestView.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-10-13 
 *描述:         继承于YxView，且启动后需要发送参数并对返回数据进行处理。类似结构可以继承于该类
 *              该类在OnStart和OnEnableEx都会发送请求          
 *历史记录: 
*/

using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jpmj.GameNotice
{
    public class YxRequestView : YxView
    {
        [Tooltip("请求参数Action名称")]
        public string ActionName = "sameReturn";
        [Tooltip("请求参数Key集合")]
        public List<string> ParamKeys = new List<string>()
        {
            "config_n"
        };
        [Tooltip("请求参数Value集合，需要与key的index对齐")]
        public List<string> ParamValues = new List<string>();

        protected Dictionary<string, object> Param
        {
            get
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                if (ParamKeys==null)
                {
                    YxDebug.LogError("ParamKeys is null");
                    return param;
                }
                if (ParamValues == null)
                {
                    YxDebug.LogError("ParamValues is null");
                    return param;
                }
                for (int i = 0, lenth = ParamKeys.Count,valueLenth=ParamValues.Count; i < lenth; i++)
                {
                    var cacheKey = ParamKeys[i];
                    if (param.ContainsKey(cacheKey))
                    {
                        continue;
                    }
                    if(i< valueLenth)
                    {
                        if (i>= ParamValues.Count)
                        {
                            YxDebug.LogError("i>= ParamValues.Count");
                            return param;
                        }
                        param.Add(cacheKey, ParamValues[i]);
                    }
                    
                }
                return param;
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            SendAction();
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if(Data==null)
            {
                return;
            }
        }

        public void SendAction()
        {
            Facade.Instance<TwManager>().SendAction(ActionName, Param, UpdateView, true, null, false);
        }
    }
}