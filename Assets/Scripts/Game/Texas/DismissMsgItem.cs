using UnityEngine;
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace Assets.Scripts.Game.Texas
{
    public class DismissMsgItem : MonoBehaviour {

        [SerializeField]
        private UILabel _playerName = null;

        [SerializeField]
        private UILabel _playerResult = null;
    
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
        public virtual int PlayerType
        {
            set 
            {
                switch (value)
                { 
                    case 2:
                        _playerResult.text = "正在选择";
                        _playerResult.color = Color.white;
                        break;
                    case 3:
                        _playerResult.text = "同意";
                        _playerResult.color = Tool.Tools.ChangeToColor(0x00FF24);
                        break;
                    case -1:
                        _playerResult.text = "拒绝";
                        _playerResult.color = Tool.Tools.ChangeToColor(0xDD0F0F);
                        break;
                    default:
                        Debug.Log("玩家态度不是很明确啊,请查验玩家Type");
                        break;
                }
            }
        }
    }
}
