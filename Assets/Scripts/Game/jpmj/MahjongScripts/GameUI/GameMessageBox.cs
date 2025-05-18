using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public enum EnMessageBoxType
    {
        OkOnly,
        OkAndCancel,
    }

    public class GameMessageBox : MonoBehaviour 
    {
        public GameObject BtnMiddle;
        public GameObject BtnLeft;
        public GameObject BtnRight;
        public GameObject Body;


        public Text Content;

        private static GameMessageBox Self;

        private DVoidBool _callBack;
        void Awake()
        {
            Self = this;
        }

        public static void Show(EnMessageBoxType type,string content,DVoidBool callBack = null)
        {
            Self.ShowBox(type, content, callBack);
        }

        public void ShowBox(EnMessageBoxType type, string content, DVoidBool callBack)
        {
            switch (type)
            {
                case EnMessageBoxType.OkOnly:
                    BtnMiddle.SetActive(true);
                    BtnLeft.SetActive(false);
                    BtnRight.SetActive(false);
                    break;
                case EnMessageBoxType.OkAndCancel:
                    BtnMiddle.SetActive(false);
                    BtnLeft.SetActive(true);
                    BtnRight.SetActive(true);
                    break;
            }

            _callBack = callBack;

            Content.text = content;

            Body.SetActive(true);
        }


        public void OnOkClick()
        {
            if(_callBack!=null)_callBack(true);
            Body.SetActive(false);
        }

        public void OnCancelClick()
        {
            if (_callBack != null) _callBack(false);
            Body.SetActive(false);
        }
    }
}