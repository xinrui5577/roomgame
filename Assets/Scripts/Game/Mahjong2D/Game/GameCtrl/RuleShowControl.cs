/** 
 *文件名称:     RuleShowControl.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-04-24 
 *描述:         游戏规则显示处理
 *历史记录: 
*/

using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using Assets.Scripts.Game.Mahjong2D.Game.Item;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Game.GameCtrl
{
    public class RuleShowControl : MonoSingleton<RuleShowControl>
    {
        /// <summary>
        /// 规则项的prefab
        /// </summary>
        [SerializeField]
        private GameObject _ruleItemPre;
        /// <summary>
        /// 规则的父类
        /// </summary>
        [SerializeField]
        private GameObject _rulesParent;
        /// <summary>
        /// 显示按钮
        /// </summary>
        [SerializeField]
        private GameObject _showBtn;
        /// <summary>
        /// 规则面板
        /// </summary>
        [SerializeField]
        private GameObject _ruleInfoPanel;
        /// <summary>
        /// 打开起始位置
        /// </summary>
        [SerializeField]
        private Transform _fromPos;
        /// <summary>
        /// 关闭起始位置
        /// </summary>
        [SerializeField]
        private Transform _toPos;
        /// <summary>
        /// 移动时间
        /// </summary>
        [SerializeField]
        private float _moveTime=3f;
        /// <summary>
        ///关闭玩法面板时，显示玩法按钮的提前时间
        /// </summary>
        [SerializeField]
        private float _showPlayBtnTime=0.3f;
        /// <summary>
        /// 是否需要关闭显示按钮（这个控制玩法按钮是否继续显示）
        /// </summary>
        [SerializeField]
        private bool _isNeedClose = true;
        /// <summary>
        /// 玩法项名称prefab
        /// </summary>
        [SerializeField]
        private char _spliteFlag=';';

        /// <summary>
        /// Item间隙
        /// </summary>
        private int _cellHeight=20;
        public override void Awake()
        {
            base.Awake();
            if (_ruleInfoPanel!=null)
            {
                _ruleInfoPanel.SetActive(false);
            }
        }

        public void SetRuleInfo(string ruleInfo)
        {
            string[] items = ruleInfo.Split(_spliteFlag);
            if(items.Length==1)
            {
                return;
            }
            if (_ruleItemPre && _rulesParent)
            {
                ClearParent();
                foreach (var item in items)
                {
                    if (string.IsNullOrEmpty(item))
                    {
                        continue;
                    }
                    RuleItem ruleItem = NGUITools.AddChild(_rulesParent, _ruleItemPre).GetComponent<RuleItem>();
                    ruleItem.InitData(item);
                }
            }
        }

        public void OnCloseRuleWindow()
        {
            Show(false);
        }

        public void OnOpenRuleWindow()
        {
           Show(true);
        }


        private void Show(bool state)
        {
            if (state)
            {
                _ruleInfoPanel.SetActive(true);
                if (_isNeedClose)
                {
                    _showBtn.SetActive(false);
                }
                int height = 0;
                for (int i = 0,lenth=_rulesParent.transform.childCount; i <lenth; i++)
                {
                    RuleItem ruleItem=_rulesParent.transform.GetChild(i).GetComponent<RuleItem>();
                    ruleItem.transform.localPosition=new Vector3(0,height);
                    height -= (ruleItem.ItemHeight+ _cellHeight);
                }
                iTween.MoveTo(_ruleInfoPanel.gameObject,_toPos.position,_moveTime);
            }
            else
            {
                iTween.MoveTo(_ruleInfoPanel.gameObject, _fromPos.position, _moveTime);
                float showBtnTime=_moveTime- _showPlayBtnTime;
                if (showBtnTime<=0)
                {
                    showBtnTime = 0;
                }
                Invoke("OnBackFinished", showBtnTime);
            }
        }

        private void OnBackFinished()
        {
            _ruleInfoPanel.SetActive(false);
            if (_isNeedClose)
            {
                _showBtn.SetActive(true);
            } 
        }

        private void ClearParent()
        {
            while (_rulesParent.transform.childCount>0)
            {
                DestroyImmediate(_rulesParent.transform.GetChild(0).gameObject);
            }
        }
    }
}
