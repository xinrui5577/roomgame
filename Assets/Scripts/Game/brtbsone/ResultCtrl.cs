using Assets.Scripts.Common.Windows;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.brtbsone
{
    public class ResultCtrl : YxNguiWindow
    {

        //public UILabel[] ResultLabel;
        //public UILabel SumLabel;

        /// <summary>
        /// 本人战绩
        /// </summary>
        [SerializeField]
        private UILabel _selfWin = null;

        /// <summary>
        /// 庄家战绩
        [SerializeField]
        private UILabel _bankerWin = null;

        /// <summary>
        /// 黑色背景
        /// </summary>
        [SerializeField]
        private GameObject _backGround = null;

        /// <summary>
        /// 结算窗口
        /// </summary>
        [SerializeField]
        GameObject _resultView = null;

        /// <summary>
        /// 关闭结算窗口
        /// </summary>
        public void CloseWindow()
        {
            _resultView.SetActive(false);
            _backGround.SetActive(false);
            CancelInvoke();
        }
        //public void RefreshResultInfo(ISFSObject responseData)
        //{ 
        //    gameObject.SetActive(true); 
        //    int win = responseData.GetInt("win");
        //    if (win < 0)
        //    {
        //        AudioPlay.Instance.PlaySounds("lose");
        //    }
        //    else
        //    {
        //        AudioPlay.Instance.PlaySounds("win");
        //    }
        //    var gdata = App.GetGameData<GlobalData>();
        //    gdata.CurrentUser.Gold = responseData.GetLong("total");
        //    gdata.CurrentUser.TodayWin = responseData.GetInt("todayWin");
        //    gdata.CurrentUser.Forbiden = responseData.ContainsKey("forbiden") && responseData.GetBool("forbiden");
        //    gdata.CurrentUser.Msg = responseData.ContainsKey("msg") ? responseData.GetUtfString("msg") : "--";
        //    int[] pg = responseData.GetIntArray("pg");
            
        //    var minCount = Mathf.Min(pg.Length, ResultLabel.Length);
        //    for (var i = 0; i < minCount; i++)
        //    { 
        //        ResultLabel[i].text = pg[i] >= 0 ? "+ " + pg[i] : "" + pg[i];
        //    }
        //    SumLabel.text = win >= 0 ? "+ " + win : "" + win;

        //    Invoke("ShowWindow",5f);
        //}


        /// <summary>
        /// 刷新结算界面数据
        /// </summary>
        /// <param name="responseData"></param>
        public void RefreshResultInfo(ISFSObject responseData)
        {
            
            _selfWin.text = responseData.GetInt("win").ToString();
            _selfWin.MakePixelPerfect();
            _bankerWin.text = responseData.GetLong("bwin").ToString();
            _bankerWin.MakePixelPerfect();

        }


        /// <summary>
        /// 显示结算窗口
        /// </summary>
        public void ShowWindow()
        {
            _backGround.SetActive(true);
            _resultView.SetActive(true);

           _resultView.transform.localScale = Vector3.zero;
           TweenScale.Begin(_resultView, 0.2f, Vector3.one);

            Invoke("CloseWindow", 5f);
        }

    }
}
