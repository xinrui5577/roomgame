/** 
 *文件名称:     YxSameReturnView.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-10-13 
 *描述:        处理sameReturn接口的消息与界面。即后台配置什么，前台就显示什么的接口，通常只是显示一个Label
 *历史记录: 
*/

using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Hall.View
{
    public class YxSameReturnView : YxRequestView
    {
        [Tooltip("通用显示Label,后台配就咋显示")]
        public UILabel ShowLabel;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if(Data==null)
            {
                return;
            }
            DealShowData();
        }

        protected virtual void DealShowData()
        {
            if (ShowLabel)
            {
                ShowLabel.text = Data.ToString();
            }
        }

        protected void ClearTrans(Transform trans)
        {
            while (trans.childCount>0)
            {
                DestroyImmediate(trans.GetChild(0).gameObject);
            }
        }
    }
}
