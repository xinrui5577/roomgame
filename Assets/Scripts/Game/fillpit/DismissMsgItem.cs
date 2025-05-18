using UnityEngine;
using Assets.Scripts.Game.fillpit.Tool;
using YxFramwork.Common.Model;

// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace Assets.Scripts.Game.fillpit
{
    public class DismissMsgItem : MonoBehaviour {

        /// <summary>
        /// 玩家名Label
        /// </summary>
        [SerializeField]
        private UILabel _playerName = null;

        /// <summary>
        /// 玩家结算Label
        /// </summary>
        [SerializeField]
        private UILabel _playerResult = null;

        /// <summary>
        /// 玩家ID
        /// </summary>
        [HideInInspector]
        public int PlayerId;
     

        public virtual void SetItemData(YxBaseGameUserInfo userInfo)
        {
            _playerName.text = userInfo.NickM;
            PlayerId = userInfo.Id;
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
                        _playerResult.text = "选择中";
                        _playerResult.color = Tools.ChangeToColor(0xFFFF00);
                        _playerResult.effectColor = Tools.ChangeToColor(0xD04F29);
                        break;
                    case 3:
                        _playerResult.text = "同意";
                        _playerResult.color = Tools.ChangeToColor(0x00FF00);
                        _playerResult.effectColor = Tools.ChangeToColor(0x023201);
                        break;
                    case -1:
                        _playerResult.text = "拒绝";
                        _playerResult.color = Tools.ChangeToColor(0xFF4C4C);
                        _playerResult.effectColor = Tools.ChangeToColor(0x300101);
                        break;
                    default:
                        Debug.Log("玩家态度不是很明确啊,请查验玩家Type");
                        break;
                }
            }
        }

        //void Update()
        //{

        //    if (Input.GetKeyDown(KeyCode.Space))
        //    {
        //        PlayerType = -1;
        //    }

        //    if (Input.GetKeyDown(KeyCode.F2))
        //    {
        //        PlayerType = 2;
        //    }

        //    if (Input.GetKeyDown(KeyCode.F3))
        //    {
        //        PlayerType = 3;
        //    }
        //}
    }
}
