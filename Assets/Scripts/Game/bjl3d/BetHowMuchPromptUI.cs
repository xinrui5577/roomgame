using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Manager;
using com.yxixia.utile.YxDebug;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.bjl3d
{
    public class BetHowMuchPromptUI : MonoBehaviour//  g 11.15
    {
        private Text _brankerText;
        private Text _freeHomeText;
        private Text _flatText;
        private Transform _brankMode;
        private Transform _freeMode;
        private Text[] BrankModeTexts = new Text[12];
        private Text[] FreeModeTexts = new Text[12];
         
        /// <summary>
        /// 获取UI操作控件
        /// </summary>
        protected void Awake()
        {
            Transform tf = transform.FindChild("Branker_Text");
            if (tf == null)
                YxDebug.LogError("No Such Object");//没有该物体    
            if (tf != null) _brankerText = tf.GetComponent<Text>();
            if (_brankerText == null)
                YxDebug.LogError("No Such  Component");//没有该组件

            tf = transform.FindChild("FreeHome_Text");
            if (tf == null)
                YxDebug.LogError("No Such Object");//没有该物体    
            if (tf != null) _freeHomeText = tf.GetComponent<Text>();
            if (_freeHomeText == null)
                YxDebug.LogError("No Such  Component");//没有该组件

            tf = transform.FindChild("Flat_Text");
            if (tf == null)
                YxDebug.LogError("No Such Object");//没有该物体    
            if (tf != null) _flatText = tf.GetComponent<Text>();
            if (_flatText == null)
                YxDebug.LogError("No Such  Component");//没有该组件


            _brankMode = transform.FindChild("BrankMode");
            if (_brankMode == null)
                YxDebug.LogError("No Such Object");//没有该物体 
            for (int i = 0; i < 12; i++)
            {
                int index = i + 1;
                if (_brankMode != null) tf = _brankMode.FindChild("Text_" + index);
                if (tf == null)
                    YxDebug.LogError("No Such Object");//没有该物体 
                if (tf != null) BrankModeTexts[i] = tf.GetComponent<Text>();
                if (BrankModeTexts[i] == null)
                    YxDebug.LogError("No Such  Component");//没有该组件
            }

            _freeMode = transform.FindChild("FreeMode");
            if (_freeMode == null)
                YxDebug.LogError("No Such Object");//没有该物体 
            for (int i = 0; i < 12; i++)
            {
                int index = i + 1;
                if (_freeMode != null) tf = _freeMode.FindChild("Text_" + index);
                if (tf == null)
                    YxDebug.LogError("No Such Object");//没有该物体 
                FreeModeTexts[i] = tf.GetComponent<Text>();
                if (FreeModeTexts[i] == null)
                    YxDebug.LogError("No Such  Component");//没有该组件
            }
        }

        private readonly int[] _brankNum = new int[12];
        private readonly int[] _freeNum = new int[12];

        /// <summary>
        /// 初始化历史纪录的点数
        /// </summary>
        /// <param name="historys"></param>
        public void SetLuziInfo(int[] historys)
        {
            var len = historys.Length;
            var hisidx = App.GetGameData<Bjl3DGameData>().Hisidx;
            for (var i = 0; i < len; i++)
            {
                var history = historys[(i + hisidx) % 12];
                var free = history & 0xf;
                var brank = history >> 4 & 0xf;
                _freeNum[i] = free;
                _brankNum[i] = brank;
                BrankModeTexts[i].text = brank.ToString();
                FreeModeTexts[i].text = free.ToString();
                //Debug.Log("闲家点数" + i + ":" + FreeNum[i]);
                //Debug.Log("庄家点数" + i + ":" + BrankNum[i]);
                //Debug.Log("历史记录中的数据" + i + ":" + history[i]);
            }
        }
        public int TotolIndex;
        private bool _isInit=true;
        
        /// <summary>
        /// 游戏底部的初始化历史纪录显示
        /// </summary>
        public void BottomLuzi()
        {
            var gdata = App.GetGameData<Bjl3DGameData>();
            var gameCfg = gdata.GameConfig;
            var historyData=0;
            if (_isInit)
            {
                for (int i = 0; i < 12; i++)
                {
                    int HistoryData1 = 0;
                    if (_freeNum[i] > _brankNum[i])
                    {
                        HistoryData1 = 1;
                    }
                    if (_freeNum[i] < _brankNum[i])
                    {
                        HistoryData1 = 2;
                    }
                    if (_freeNum[i] == _brankNum[i])
                    {
                        HistoryData1 = 3;
                    }
                    gameCfg.LuziInfo.Add(HistoryData1);
                    _isInit = false;
                }
                return;
            }
                if (gdata.XianValue > gdata.ZhuangValue)
                {

                    StartCoroutine(PlaySoundDianShu(gdata.XianValue, gdata.ZhuangValue, false));
                    historyData = 1;
                }
                if (gdata.XianValue < gdata.ZhuangValue)
                {

                    StartCoroutine(PlaySoundDianShu(gdata.XianValue, gdata.ZhuangValue, true));
                    historyData = 2;
                }
                if (gdata.XianValue == gdata.ZhuangValue)
                {
                    historyData = 3;
                }
            gameCfg.LuziInfo.Add(historyData);
          
        }

        IEnumerator PlaySoundDianShu(int xianValue,int zhuangValue,bool isZhuang)
        {
            yield return new WaitForSeconds(6f);
            var musicMgr = Facade.Instance<MusicManager>();
            if (isZhuang)
            {
                musicMgr.Play("zhuangbet");
                yield return new WaitForSeconds(2f);
                musicMgr.Play("zhuang" + zhuangValue);
                yield return new WaitForSeconds(2f);
                musicMgr.Play("xian" + xianValue);
            }
            else
            {
                musicMgr.Play("xianbet");
                yield return new WaitForSeconds(1f);
                musicMgr.Play("xian" + xianValue);
                yield return new WaitForSeconds(1f);
                musicMgr.Play("zhuang" + zhuangValue);
            }
               
        }
        private int _myIndex = 1;

        /// <summary>
        /// 历史纪录的信息现实的点数
        /// </summary>
        public void Data()
        {
            var gdata = App.GetGameData<Bjl3DGameData>();
            _freeNum[(_myIndex - 1) % 12] = gdata.XianValue;
            _brankNum[(_myIndex - 1) % 12] = gdata.ZhuangValue;
            for (int i = 0; i < _brankNum.Length; i++)
            {
                BrankModeTexts[i].text = _brankNum[(_myIndex + i) % 12] + "";
                FreeModeTexts[i].text = _freeNum[(_myIndex + i) % 12] + "";
            }
            _myIndex++;

        }
        /// <summary>
        /// 初始化筹码面板上的可下注钱数
        /// </summary>
        public void HowMuchPrompt()
        {
            var gdata = App.GetGameData<Bjl3DGameData>();
            if (gdata.Allow[3] == 0)
            {
                _brankerText.text = YxUtiles.GetShowNumberToString(gdata.Maxante);
            }
            else
            {
                _brankerText.text = YxUtiles.GetShowNumberToString(gdata.Allow[3]);
            }
            if (gdata.Allow[0] == 0)
            {
                _freeHomeText.text = YxUtiles.GetShowNumberToString(gdata.Maxante);
            }
            else
            {
                _freeHomeText.text = YxUtiles.GetShowNumberToString(gdata.Allow[0]);
            }
            if (gdata.Allow[6] == 0)
            {
                _flatText.text = YxUtiles.GetShowNumberToString(gdata.Maxante);
            }
            else
            {
                _flatText.text = YxUtiles.GetShowNumberToString(gdata.Allow[6]);
            }
        }
    }
}