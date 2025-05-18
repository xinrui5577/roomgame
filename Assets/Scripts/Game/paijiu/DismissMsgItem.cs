using UnityEngine;
#pragma warning disable 649

namespace Assets.Scripts.Game.paijiu
{
    public class DismissMsgItem : MonoBehaviour
    {

        /// <summary>
        /// 玩家名Label
        /// </summary>
        [SerializeField]
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private UILabel _playerName = null;

        /// <summary>
        /// 玩家结算Label
        /// </summary>
        [SerializeField]
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private UISprite _playerChoise = null;

        /// <summary>
        /// 正在选择图标
        /// </summary>
        [SerializeField]
        private GameObject _choisingMark;

        /// <summary>
        /// 玩家ID
        /// </summary>
        [HideInInspector]
        public int PlayerId = 0;


        private string _name = string.Empty;




        /// <summary>
        /// 玩家的名字
        /// </summary>
        public string PlayerName
        {
            set { _name = value; _playerName.text = _name; }
            get { return _name; }
        }

        /// <summary>
        /// 设置玩家的决定,2为等待,-1为拒绝,3为同意
        /// </summary>
        public int PlayerType
        {
            set
            {
                switch (value)
                {
                    case 2:
                        _choisingMark.SetActive(true);
                        _playerChoise.gameObject.SetActive(false);
                        break;
                    case 3:
                        GetUserChoise("icon_yes");
                        break;
                    case -1:
                        GetUserChoise("icon_no");
                        break;
                    default:
                        Debug.Log("玩家态度不是很明确啊,请查验玩家Type");
                        break;
                }
            }
        }

        /// <summary>
        /// 获取玩家选择
        /// </summary>
        /// <param name="sprName">选择图标名称</param>
        void GetUserChoise(string sprName)
        {
            _choisingMark.SetActive(false);
            _playerChoise.spriteName = sprName;
            _playerChoise.gameObject.SetActive(true);
            _playerChoise.MakePixelPerfect();
        }
    }
}
