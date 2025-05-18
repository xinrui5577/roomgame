using System;
using System.Collections.Generic;
using Assets.Scripts.Game.BlackJackCs.Tool;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.BlackJackCs
{
    public class BetBtnMgr : MonoBehaviour
    {


        [SerializeField] private UIGrid _grid = null;

        /// <summary>
        /// 下注按钮的个数
        /// </summary>
        [SerializeField] private List<AddBetBtnItem> _addBetBtns;
        
        /// <summary>
        /// 显示下注按钮的个数
        /// </summary>
        [Tooltip("最多显示的筹码按钮个数")]
        [SerializeField] private int _showBetCount = 7;

        /// <summary>
        /// 背景或设置筹码展开的设置宽度
        /// </summary>
        [Tooltip("背景或设置的设置宽度")]
        [SerializeField] private int _bgWidth = 1050;

        /// <summary>
        /// 玩家说话的界面
        /// </summary>
        public GameObject[] SpeakViews;

        /// <summary>
        /// 说话按键,下注按钮除外
        /// </summary>
        public GameObject[] SpeakBtns;


        protected void Start()
        {
            InitBtns();
        }



        /// <summary>
        /// 解析cargs2下的内容;
        /// 初始化下注按钮的面值,如果包含-rateconfig,由服务器控制面值和个数,否则按本地设置个数
        /// </summary>
        /// <param name="cargs2">cargs2的上层数据或cargs2数据</param>
        public void SetAddBetBtns(ISFSObject cargs2)
        {
            ISFSObject data = cargs2.ContainsKey("cargs2") ? cargs2.GetSFSObject("cargs2") : cargs2;

            int ante;
            if (!Int32.TryParse(data.GetUtfString("-ante"), out ante) || ante <= 0)
            {
                YxDebug.LogError("=== Ante is wrong!! ===");
                ante = 1;
            }

            if (data.ContainsKey("-anteconfig") && ante > 0) //如果包含此字段,则是由用户设置筹码的面值和个数
            {
                bool couldSet = true;
                string str = data.GetUtfString("-anteconfig");

                string[] tempStr = str.Split('#');

                int[] array = new int[tempStr.Length > _showBetCount ? _showBetCount : tempStr.Length];

                for (int i = 0; i < array.Length; i++)
                {
                    if (!Int32.TryParse(tempStr[i], out array[i]))
                    {
                        YxDebug.LogError("=== There is some string could not be changed to int!! === ");
                        couldSet = false;
                        break; 
                    }
                }

                if (couldSet)
                {
                    SetChip(array, ante);
                }
                else
                {
                    SetChip(ante);
                }
            }
            else
            {
                SetChip(ante);
            }   
        }

        /// <summary>
        /// 当采用默认情况的时候,筹码只跟默认倍数有关
        /// </summary>
        /// <param name="ante"></param>
        void SetChip(int ante)
        {
            App.GetGameData<BlackJackGameData>().Ante = ante;  //设置基础下注值
            foreach( var btn in _addBetBtns )
            {
                btn.Ante = ante;
            }
        }


        /// <summary>
        /// 设置Ante值,设置每个按钮的倍数
        /// </summary>
        /// <param name="array">传入倍数数组</param>
        /// <param name="ante">ante值,基础倍数</param>
        private void SetChip(int[] array,int ante)
        {
            SetChip(ante);
            int count = array.Length;

            for (int i = 0; i < _addBetBtns.Count;)
            {

                if (i >= count)
                {
                    var obj = _addBetBtns[i];
                    _addBetBtns.Remove(obj);
                    Destroy(obj.gameObject);    //销毁多出的筹码
                    continue;
                }

                _addBetBtns[i].AddTimes = array[i];
                i++;

            }

            GridReposition(count);
        }

        /// <summary>
        /// 重制Grid
        /// </summary>
        /// <param name="count"></param>
        private void GridReposition(int count)
        {
            // ReSharper disable once PossibleLossOfFraction
            _grid.cellWidth = _bgWidth / count;
            _grid.hideInactive = true;
            _grid.repositionNow = true;
            _grid.Reposition();
        }


        void InitBtns()
        {

            foreach (SpeakButton btnid in Enum.GetValues(typeof (SpeakButton)))
            {
                foreach (GameObject btn in SpeakBtns)
                {
                    if (btn.name.Equals(btnid.ToString()))
                    {
                        Tools.NguiAddOnClick(btn, OnClickListener, (int) btnid);
                    }
                }
            }
        }


        private void OnClickListener(GameObject gob)
        {
            SpeakButton btnid = (SpeakButton)UIEventListener.Get(gob).parameter;
            var gserver = App.GetRServer<BlackJackGameServer>();

            switch (btnid)
            {
                case SpeakButton.BuyInsurance:
                    gserver.SendRequest(GameRequestType.Insurance);
                    break;
                case SpeakButton.Double:
                    gserver.SendRequest(GameRequestType.AddRate);
                    break;
                case SpeakButton.Hit:
                    gserver.SendRequest(GameRequestType.Hit);
                    break;
                case SpeakButton.Stand:
                    gserver.SendRequest(GameRequestType.Stand);
                    break;
                default:
                    YxDebug.LogError("No this button in you enum!!");
                    break;
            }
        }

        public void CheckColdClickAnte()
        {
            foreach(var bet in _addBetBtns)
            {
                bet.GetComponent<AddBetBtnItem>().CheckCouldClick();
            }
        }


        protected enum SpeakButton
        {
            BuyInsurance,
            Double,
            Hit,
            Stand,
        }
    }

}
