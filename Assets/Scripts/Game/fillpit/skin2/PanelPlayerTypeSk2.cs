using System;
using System.Collections.Generic;
using Assets.Scripts.Game.fillpit.ImgPress.Main;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.fillpit.skin2
{
    public class PanelPlayerTypeSk2 : PanelPlayerType
    {

        public GameObject FoldMark;

        public UILabel TypeLabel;

        public GetTypeLabelText GetTypeLabelText;

        public override void ShowGameType(GameRequestType type)
        {
            switch (type)
            {
                case GameRequestType.Fold:
                    FoldMark.SetActive(true);
                    break;
                case GameRequestType.KickSpeak:
                case GameRequestType.NotKick:
                case GameRequestType.BackKick:
                case GameRequestType.FollowSpeak:
                    string text = GetTypeLabelText.GetLabeText(type);
                    TypeLabel.text = text;
                    TypeLabel.gameObject.SetActive(true);
                    break;
                case GameRequestType.Bet:
                    int gold = App.GetGameData<FillpitGameData>().LastBetValue;
                    string format = GetTypeLabelText.GetLabeText(type);
                    TypeLabel.text = string.Format(format, YxUtiles.ReduceNumber(gold));
                    TypeLabel.gameObject.SetActive(true);
                    break;
            }
        }


        public override void HideGameType()
        {
            TypeLabel.text = string.Empty;
            TypeLabel.gameObject.SetActive(false);
        }


        public override void Reset()
        {
            FoldMark.SetActive(false);
            TypeLabel.gameObject.SetActive(false);
        }

    }

    [Serializable]
    public class GetTypeLabelText
    {
        public List<string> TextLabelKey = new List<string> {"KickSpeak", "NotKick", "BackKick", "FollowSpeak", "Bet"};
        public List<string> TextLabelVal = new List<string> {"踢", "不踢", "反踢", "跟注", "下{0}注"};

        public string GetLabeText(GameRequestType type)
        {
            int index = TextLabelKey.FindIndex(a => a.Equals(type.ToString()));

            return TextLabelVal[index];
        }
    }
}
