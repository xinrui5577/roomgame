using Assets.Scripts.Common.Adapters;
using UnityEngine;

// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace Assets.Scripts.Game.sssjp
{
    public class DismissItem : MonoBehaviour
    {
        /// <summary>
        /// 玩家名Label
        /// </summary>
        [SerializeField]
        private UILabel _playerName = null;

        /// <summary>
        /// 玩家名Label
        /// </summary>
        [SerializeField]
        private UILabel _playerId = null;

        /// <summary>
        /// 人物头像
        /// </summary>
        [SerializeField]
        private NguiTextureAdapter _headImage = null;


        /// <summary>
        /// 玩家选择图标
        /// </summary>
        [SerializeField]
        private UISprite _choiseSprite = null;

        private string _name = string.Empty;

        /// <summary>
        /// 玩家ID
        /// </summary>
        [HideInInspector]
        public int PlayerId = 0;

        /// <summary>
        /// 玩家的名字
        /// </summary>
        public string PlayerName
        {
            set { _name = value; _playerName.text = _name; }
            get { return _name; }
        }

        /// <summary>
        /// 玩家的名字
        /// </summary>
        public string PlayerIdLabel
        {
            set { _playerId.text = value; }
        }

        public NguiTextureAdapter HeadImage
        {
            get { return _headImage; }
        }

        private int _PlayerType = 0;

        /// <summary>
        /// 设置玩家的决定,2为等待,-1为拒绝,3为同意
        /// </summary>
        public int PlayerType
        {
            get { return _PlayerType; }
            set
            {
                _PlayerType = value;
                switch (value)
                {
                    case 2:
                        _choiseSprite.gameObject.SetActive(false);
                        //_playerResult.effectColor = Tools.ChangeToColor(0xD04F29);  //描边颜色
                        break;
                    case 3:
                        SetChoiseSprite("yes");
                        //_playerResult.effectColor = Tools.ChangeToColor(0x023201);
                        break;
                    case -1:
                        SetChoiseSprite("no");
                        //_playerResult.effectColor = Tools.ChangeToColor(0x300101);
                        break;
                    default:
                        Debug.Log("玩家态度不是很明确啊,请查验玩家Type");
                        break;
                }
            }
        }

        public void ResetPlayerType()
        {
            _PlayerType = 0;
        }

        void SetChoiseSprite(string spriteName)
        {
            _choiseSprite.gameObject.SetActive(!string.IsNullOrEmpty(spriteName));
            _choiseSprite.spriteName = spriteName;
            _choiseSprite.MakePixelPerfect();
        }
    }
}
