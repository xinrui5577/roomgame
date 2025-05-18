using System.Collections.Generic;
using Assets.Scripts.Common.Windows.TabPages;
using UnityEngine;

namespace Assets.Scripts.Common.Windows.MatchWindows
{
    /// <summary>
    /// 比赛界面
    /// </summary>
    public class YxMatchPageWindow : YxTabPageWindow
    {
        protected override void ActionCallBack()
        {
            base.ActionCallBack();
            var data = GetData<Dictionary<string,object>>();
            if (data == null) return;
            /*
             * {
                    "tabs" : [ { name: ""} ],
                    "matchList": [
                                    {
                                        id:       //比赛id
                                        name:"",  //比赛名字
                                        pcount:"",//人数
                                        gamename:""//游戏名称
                                        time:""//比赛时间
                                        state: // 报名状态
                                    }
                                  ]
                                 
             
                }
             */
        }
    }
}
