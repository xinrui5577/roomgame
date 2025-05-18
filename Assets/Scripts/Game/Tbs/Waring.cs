using System;
using UnityEngine;

namespace Assets.Scripts.Game.Tbs
{
    public class Waring : MonoBehaviour
    {
        /// <summary>
        /// 信息显示
        /// </summary>
        public UILabel InforLabel;

        public GameObject[] Buttons;

        public GameObject Bg;

       protected void Awake()
        {
            InitOnClickListener();
        }

        /// <summary>
        /// 初始化监听
        /// </summary>
        private void InitOnClickListener()
        {
            foreach (Buttonid btnid in Enum.GetValues(typeof(Buttonid)))
            {
                foreach (GameObject btn in Buttons)
                {
                    if (btn != null && btn.name.Equals(btnid.ToString()))
                    {
                        //添加点击事件
                        Tools.NguiAddOnClick(btn.gameObject, OnClickListener, (int)btnid);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 按键监听
        /// </summary>
        /// <param name="gob"></param>
        private void OnClickListener(GameObject gob)
        {
            var bi = (Buttonid)UIEventListener.Get(gob).parameter;

            switch (bi)
            {
                case Buttonid.Close:
                    if (_closeD == null)
                    {
                        DefaultEvent();
                    }
                    else
                    {
                        _closeD();
                    }
                    break;
                case Buttonid.Enter:
                    if (_enterD == null)
                    {
                        DefaultEvent();
                    }
                    else
                    {
                        _enterD();
                    }
                    break;
                case Buttonid.Cancel:
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

            GameObject enterBtn = Tools.GobSelectName(Buttons, "Enter");
            GameObject cancelBtn = Tools.GobSelectName(Buttons, "Cancel");
            GameObject closeBtn = Tools.GobSelectName(Buttons, "Close");

            enterBtn.SetActive(true);
            cancelBtn.SetActive(true);
            closeBtn.SetActive(true);
            Vector3 v3;

            const float defaultY = -176f;

            switch (t)
            {
                case WaringType.YorN:
                    v3 = new Vector3(-126f, defaultY);
                    enterBtn.transform.localPosition = v3;
                    v3 = new Vector3(126f, defaultY);
                    cancelBtn.transform.localPosition = v3;
                    break;
                case WaringType.Y:
                    v3 = new Vector3(0, defaultY);
                    enterBtn.transform.localPosition = v3;
                    cancelBtn.SetActive(false);
                    break;
                case WaringType.YorNNoClose:
                    v3 = new Vector3(-126f, defaultY);
                    enterBtn.transform.localPosition = v3;
                    v3 = new Vector3(126f, defaultY);
                    cancelBtn.transform.localPosition = v3;
                    closeBtn.SetActive(false);
                    break;
                case WaringType.NoBtn:
                    enterBtn.SetActive(false);
                    cancelBtn.SetActive(false);
                    closeBtn.SetActive(false);
                    break;
            }

            InforLabel.text = infor;
            _enterD = enter;
            _closeD = close;
            _cancelD = cancel;
            Bg.SetActive(true);
        }


        private enum Buttonid
        {
            Close,
            Enter,
            Cancel,
        }
    }

    public enum WaringType
    {
        YorN,
        Y,
        YorNNoClose,
        NoBtn,
    }
}
