using UnityEngine;

namespace Assets.Scripts.Game.bjl3d
{
    public class StateUI : MonoBehaviour//G 11.15
    {
        private Transform _waittf;
        private Transform _bettf;
        private Transform _sendCardtf;
        private Transform _freetf;

        /// <summary>
        /// 获取UI操作控件
        /// </summary>
        protected void Awake()
        { 
            _waittf = transform.FindChild("Text");
            if (_waittf == null) return;

            _bettf = transform.FindChild("Text (1)");
            if (_bettf == null) return;

            _sendCardtf = transform.FindChild("Text (2)");
            if (_sendCardtf == null) return;

            _freetf = transform.FindChild("Text (3)");
            if (_freetf == null) return;

        }

        /// <summary>
        /// type 0:无庄等待 1：下注时间 2：开牌时间 3：空闲时间
        /// </summary>
        /// <param name="type"></param>
        public void StateShow(int type)
        {
            switch (type)
            {
                case 0:
                    {
                        if (!_waittf.gameObject.activeSelf)
                            _waittf.gameObject.SetActive(true);
                        if (_bettf.gameObject.activeSelf)
                            _bettf.gameObject.SetActive(false);
                        if (_sendCardtf.gameObject.activeSelf)
                            _sendCardtf.gameObject.SetActive(false);
                        if (_freetf.gameObject.activeSelf)
                            _freetf.gameObject.SetActive(false);
                    }
                    break;
                case 5:
                    {
                        if (_waittf.gameObject.activeSelf)
                            _waittf.gameObject.SetActive(false);
                        if (!_bettf.gameObject.activeSelf)
                            _bettf.gameObject.SetActive(true);
                        if (_sendCardtf.gameObject.activeSelf)
                            _sendCardtf.gameObject.SetActive(false);
                        if (_freetf.gameObject.activeSelf)
                            _freetf.gameObject.SetActive(false);
                    }
                    break;
                case 7:
                    {
                        if (_waittf.gameObject.activeSelf)
                            _waittf.gameObject.SetActive(false);
                        if (_bettf.gameObject.activeSelf)
                            _bettf.gameObject.SetActive(false);
                        if (!_sendCardtf.gameObject.activeSelf)
                            _sendCardtf.gameObject.SetActive(true);
                        if (_freetf.gameObject.activeSelf)
                            _freetf.gameObject.SetActive(false);
                    }
                    break;
                case 8:
                    {
                        if (_waittf.gameObject.activeSelf)
                            _waittf.gameObject.SetActive(false);
                        if (_bettf.gameObject.activeSelf)
                            _bettf.gameObject.SetActive(false);
                        if (_sendCardtf.gameObject.activeSelf)
                            _sendCardtf.gameObject.SetActive(false);
                        if (!_freetf.gameObject.activeSelf)
                            _freetf.gameObject.SetActive(true);
                    }
                    break;
                default:
                    break;
            }
        }

    }
}
