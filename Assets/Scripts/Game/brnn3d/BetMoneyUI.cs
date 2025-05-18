using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.brnn3d
{
    public class BetMoneyUI : MonoBehaviour
    {
        public Text[] SelfNoteTexts = new Text[4];
        public Text[] QiyuNoteTexts = new Text[4];
        public Transform[] NoteAreas = new Transform[4];
        private int[] _selfBetValues = new int[4];
        private int[] _otherBetValues = new int[4];
        //设置钱数的总界面的状态
        public void SetBetMoneyUI(bool isSelf)
        {
            for (var i = 0; i < 4; i++)
            {
                NoteAreas[i].gameObject.SetActive(true);
            }
            if (isSelf)
            {
                App.GameData.GStatus = YxEGameStatus.PlayAndConfine;
                SetBetMoneyUI_Self();
            }
            SetBetMoneyUI_Other();
        }
        //设置玩家自己的下注钱数的面板
        public void SetBetMoneyUI_Self()
        {
            var gdata = App.GetGameData<Brnn3dGameData>();
            gdata.GetPlayerInfo().CoinA -= SetBetMoney(SelfNoteTexts, _selfBetValues);
        }
        //设置其他玩家下注的钱数面板
        public void SetBetMoneyUI_Other()
        {
            SetBetMoney(QiyuNoteTexts, _otherBetValues);
        }

        private int SetBetMoney(Text[] notes,int[] values)
        {
            var gdata = App.GetGameData<Brnn3dGameData>();
            var betPos = gdata.BetPos;
            var note = notes[betPos];
            values[betPos]+= gdata.BetMoney;
            var newGold = values[betPos];
            note.text = YxUtiles.GetShowNumberForm(newGold);
            if (!note.transform.parent.gameObject.activeSelf)
            {
                note.transform.parent.gameObject.SetActive(true);
            }
            return gdata.BetMoney;
        }

        //清空下注的钱数
        public void BetMoneyQingKongInfo()
        {
            for (var i = 0; i < 4; i++)
            {
                SelfNoteTexts[i].text = "";
                QiyuNoteTexts[i].text = "";
                SelfNoteTexts[i].transform.parent.gameObject.SetActive(false);
                QiyuNoteTexts[i].transform.parent.gameObject.SetActive(false);
                _selfBetValues[i] = 0;
                _otherBetValues[i] = 0;
                NoteAreas[i].gameObject.SetActive(false);
            }
        }
    }
}

