using System;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Tbs
{
    public class GameOverMgr : MonoBehaviour
    {
        public UILabel CurentTime;
        public UIGrid GameOverGrid;
        public GameOverItem GameOverItem;
        /// <summary>
        /// 显示的控制开关
        /// </summary>
        [SerializeField]
        private readonly GameObject _showParent;

        public GameOverMgr(GameObject showParent)
        {
            _showParent = showParent;
        }

        public void SetData(ISFSObject requestData)
        {
            _showParent.SetActive(true);
            CurentTime.text = DateTime.Now.ToString("yyyy-MM-dd  HH:mm");
            var currentRound = requestData.ContainsKey("round") ? requestData.GetInt("round") : 0;
            var maxRound = requestData.ContainsKey("maxRound") ? requestData.GetInt("maxRound") : 0;
            var users = requestData.ContainsKey("users") ? requestData.GetSFSArray("users") : null;
            if (users == null) return;
            for (var i = 0; i < users.Count; i++)
            {
                var user = users.GetSFSObject(i);
                var nick = user.ContainsKey("nick") ? user.GetUtfString("nick") : null;
                if (nick == null) continue;
                var gold = user.ContainsKey("gold") ? user.GetInt("gold") : 0;
                var id = user.ContainsKey("id") ? user.GetInt("id") : 0;

                var obj = Instantiate(GameOverItem);
                obj.transform.parent = GameOverGrid.transform;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                obj.GetComponent<GameOverItem>().InitData(nick, gold);
            }
        }

        /// <summary>
        /// 点击返回大厅按钮
        /// </summary>
        public void OnBackHall()
        {
            App.QuitGame();
        }
    }
}
