using UnityEngine;

namespace Assets.Scripts.Game.GangWu
{

    public class DismissMsgItem : MonoBehaviour
    {

        [SerializeField]
        private UILabel _playerName = null;

        [SerializeField]
        private UISprite _resultMark = null;

        [SerializeField]
        private UISprite _chooseMark = null;

        [HideInInspector] public int PlayerId = 0;

        private string _name = string.Empty;




        /// <summary>
        /// 玩家的名字
        /// </summary>
        public string PlayerName
        {
            set
            {
                _name = value;
                _playerName.text = _name;
            }
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
                        _chooseMark.gameObject.SetActive(true);
                        _resultMark.gameObject.SetActive(false);
                        break;
                    case 3:
                        _chooseMark.gameObject.SetActive(false);
                        _resultMark.gameObject.SetActive(true);
                        _chooseMark.spriteName = "yes";
                        break;
                    case -1:
                        _chooseMark.gameObject.SetActive(false);
                        _resultMark.gameObject.SetActive(true);
                        _chooseMark.spriteName = "no";
                        break;
                    default:
                        Debug.Log("玩家态度不是很明确啊,请查验玩家Type");
                        break;
                }
            }
        }
    }
}