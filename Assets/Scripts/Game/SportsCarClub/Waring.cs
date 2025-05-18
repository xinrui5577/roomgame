using System;
using UnityEngine;

namespace Assets.Scripts.Game.SportsCarClub
{
    public class Waring : MonoBehaviour
    {
        private static Waring _instance;

        /// <summary>
        /// 信息显示
        /// </summary>
        public UILabel InforLabel;

        public GameObject[] Buttons;

        public GameObject Bg;

        public static Waring GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Waring();
            }

            return _instance;
        }

        void Awake()
        {
            this.InitOnClickListener();
            _instance = this;

            //this.gameObject.SetActive(false);
        }

        /// <summary>
        /// 初始化监听
        /// </summary>
        private void InitOnClickListener()
        {
            foreach (BUTTONID btnid in Enum.GetValues(typeof(BUTTONID)))
            {
                foreach (GameObject btn in Buttons)
                {
                    if (btn != null && btn.name.Equals(btnid.ToString()))
                    {
                        //添加点击事件
                        Game.SportsCarClub.Tools.NguiAddOnClick(btn.gameObject, OnClickListener, (int)btnid);
                        break;
                    }
                }
            }

            //foreach (UIButton button in Buttons)
            //{
            //    for (int i = 0; i < (int)BUTTONID.Lenght; i++)
            //    {
            //        BUTTONID bi = (BUTTONID)((int)BUTTONID.Close + i);

            //        if (button.name != bi.ToString()) continue;
            //        UIEventListener uie = UIEventListener.Get(button.gameObject);
            //        uie.onClick = OnClickListener;
            //        uie.parameter = bi;
            //    }
            //}
        }

        /// <summary>
        /// 按键监听
        /// </summary>
        /// <param name="gob"></param>
        private void OnClickListener(GameObject gob)
        {
            BUTTONID bi = (BUTTONID)UIEventListener.Get(gob).parameter;

            switch (bi)
            {
                case BUTTONID.Close:
                    if (_closeD == null)
                    {
                        DefaultEvent();
                    }
                    else
                    {
                        _closeD();
                    }
                    break;
                case BUTTONID.Enter:
                    if (_enterD == null)
                    {
                        DefaultEvent();
                    }
                    else
                    {
                        _enterD();
                    }
                    break;
                case BUTTONID.Cancel:
                    if (_cancelD == null)
                    {
                        DefaultEvent();
                    }
                    else
                    {
                        _cancelD();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 无参代理定义
        /// </summary>
        public delegate void NoParameterD();
        /// <summary>
        /// 确认代理
        /// </summary>
        private NoParameterD _enterD;
        /// <summary>
        /// 关闭代理
        /// </summary>
        private NoParameterD _closeD;
        /// <summary>
        /// 取消代理
        /// </summary>
        private NoParameterD _cancelD;

        /// <summary>
        /// 默认按键处理事件
        /// </summary>
        public void DefaultEvent()
        {
            Bg.SetActive(false);
        }

        /// <summary>
        /// 打开警告面板
        /// </summary>
        public void OpenWaring(string infor,WaringType t = WaringType.Y,NoParameterD enter = null, NoParameterD cancel = null, NoParameterD close = null)
        {

            GameObject enterBtn = Game.SportsCarClub.Tools.GobSelectName(Buttons, "Enter");
            GameObject cancelBtn = Game.SportsCarClub.Tools.GobSelectName(Buttons, "Cancel");
            GameObject closeBtn = Game.SportsCarClub.Tools.GobSelectName(Buttons, "Close");

            enterBtn.SetActive(true);
            cancelBtn.SetActive(true);
            closeBtn.SetActive(true);
            Vector3 v3;

            switch (t)
            {
                case WaringType.YorN:
                    v3 = new Vector3(-126f, -98f);
                    enterBtn.transform.localPosition = v3;
                    v3 = new Vector3(126f, -98f);
                    cancelBtn.transform.localPosition = v3;
                    break;
                case WaringType.Y:
                    v3 = new Vector3(0, -98f);
                    enterBtn.transform.localPosition = v3;
                    cancelBtn.SetActive(false);
                    break;
                case WaringType.YorN_NoClose:
                    v3 = new Vector3(-126f, -98f);
                    enterBtn.transform.localPosition = v3;
                    v3 = new Vector3(126f, -98f);
                    cancelBtn.transform.localPosition = v3;
                    closeBtn.SetActive(false);
                    break;
                case WaringType.NoBtn:
                    enterBtn.SetActive(false);
                    cancelBtn.SetActive(false);
                    closeBtn.SetActive(false);
                    break;
                default:
                    break;
            }

            InforLabel.text = infor;
            _enterD = enter;
            _closeD = close;
            _cancelD = cancel;
            Bg.SetActive(true);
        }


        private enum BUTTONID
        {
            Close,
            Enter,
            Cancel,
            Lenght,
        }
    }

    public enum WaringType
    {
        YorN,
        Y,
        YorN_NoClose,
        NoBtn,
    }
}
