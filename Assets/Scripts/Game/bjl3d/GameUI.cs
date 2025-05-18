using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common; 
using YxFramwork.Enums;

namespace Assets.Scripts.Game.bjl3d
{
    public class GameUI : MonoBehaviour//改11 15
    {
        public Button SuperBtn; 
        public Transform SuperUI;
        public Transform GameBackUItf;
        public Transform SettleMentUItf;

        public SettingPnl SettingWindow;

        private Transform note_textTF;
        int aliasingValue = 1;

        protected void Awake()
        {
            note_textTF = transform.Find("NoteText");
        }

        /// <summary>
        /// 游戏结算面板
        /// </summary>
        public void GameResult()
        {
            StartCoroutine("Wait");
        }
        IEnumerator Wait()
        {
            yield return new WaitForSeconds(7);
            if (!SettleMentUItf.gameObject.activeSelf)
            {
                SettleMentUItf.gameObject.SetActive(true);
            }
            var st = SettleMentUItf.GetComponent<SettleMentUI>();

            if (st != null)
            {
                st.GameResultFun();
            }
            App.GameData.GetPlayer().UpdateView();//刷新玩家自己的信息
            Invoke("HideSettleMentUI", 7f);
            App.GetGameManager<Bjl3DGameManager>().TheCoinTypeInfoUI.DisplaySelected(false);
        }
        /// <summary>
        /// 推出房间
        /// </summary>
        public void ReturnToHall()
        {
            if (App.GameData.GStatus != YxEGameStatus.PlayAndConfine)
            {
                if (!GameBackUItf.gameObject.activeSelf)
                    GameBackUItf.gameObject.SetActive(true);
            }
            else
            {
               NoteText_Show("游戏正在进行中，请稍后！！！");
            }
            
        }
        /// <summary>
        /// 结果调用
        /// </summary>
        public void HideSettleMentUI()
        {
            if (SettleMentUItf.gameObject.activeSelf)
            {
                SettleMentUItf.gameObject.SetActive(false);
            }
            var gameMgr = App.GetGameManager<Bjl3DGameManager>();
            gameMgr.TheGameScene.ClearPai();
            gameMgr.TheCoinTypeInfoUI.DisplaySelected(true);
        }
        //
        /// <summary>
        ///退出大厅
        /// </summary>
        public void SurReturnHall()//按钮
        {
            App.QuitGame();
        }
        /// <summary>
        /// 取消返回游戏
        /// </summary>
        public void CancleReturnHall()//按钮
        {
            if (GameBackUItf.gameObject.activeSelf)
                GameBackUItf.gameObject.SetActive(false);
        }
        /// <summary>
        /// 隐藏列表UI
        /// </summary>
        public void HideBtmUI()
        {
            App.GetGameManager<Bjl3DGameManager>().TheUerInfoCountDownLuziUI.ClickeUIFun();
        }
        /// <summary>
        /// 下不了注文本显示
        /// </summary>
        /// <param name="str"></param>
        public void NoteText_Show(string str = "")
        {
            if (note_textTF.gameObject.activeSelf)
                note_textTF.gameObject.SetActive(false);
            note_textTF.gameObject.SetActive(true);
            if (str != "")
            {
                Text _text = note_textTF.GetComponent<Text>();
                if (_text == null) return;
                _text.text = str;
            }
        }
        public void OnSetting()
        {
            SettingWindow.Show(true);
        }

    }
}

