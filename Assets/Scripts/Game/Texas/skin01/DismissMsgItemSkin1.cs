using UnityEngine;
#pragma warning disable 649


namespace Assets.Scripts.Game.Texas.skin01
{
    public class DismissMsgItemSkin1 : DismissMsgItem
    {

        /// <summary>
        /// 显示玩家选择结果
        /// </summary>
        [SerializeField]
        private UISprite _playerChoise;

        /// <summary>
        /// 玩家正在选择标签
        /// </summary>
        [SerializeField]
        private GameObject _choisingMark;

        public override int PlayerType
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
                        OnUserChoise("icon_yes");
                        break;
                    case -1:
                        OnUserChoise("icon_no");
                        break;
                    default:
                        Debug.Log("玩家态度不是很明确啊,请查验玩家Type");
                        break;
                }
            }
        }

        void OnUserChoise(string sprName)
        {
            _choisingMark.SetActive(false);
            _playerChoise.spriteName = sprName;
            _playerChoise.gameObject.SetActive(true);
            _playerChoise.MakePixelPerfect();
        }
    }
}