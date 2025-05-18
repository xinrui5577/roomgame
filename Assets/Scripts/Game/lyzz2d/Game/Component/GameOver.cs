using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.lyzz2d.Game.ImgPress;
using Assets.Scripts.Game.lyzz2d.Game.Item.GameOverItem;
using Assets.Scripts.Game.lyzz2d.Utils;
using Assets.Scripts.Game.lyzz2d.Utils.Single;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.lyzz2d.Game.Component
{
    public class GameOver : MonoSingleton<GameOver>
    {
        /// <summary>
        ///     玩家数据UI
        /// </summary>
        [SerializeField] private GameOverItem[] _playerItems;

        private GameObject _showParent;

        [SerializeField] private CompressImg Img;

        /// <summary>
        ///     玩家数据
        /// </summary>
        private OverData[] playerData = new OverData[4];

        public override void Awake()
        {
            base.Awake();
            _showParent = transform.GetChild(0).gameObject;
        }

        public void SetUserOverData(ISFSObject ob)
        {
            playerData = new OverData[4];
            var users = ob.GetSFSArray(RequestKey.KeyUsers);
            for (var i = 0; i < users.Count; i++)
            {
                var obj = users.GetSFSObject(i);
                playerData[i] = new OverData(obj);
            }
            int numYing = 0, numPao = 0;

            for (var i = 0; i < 4; i++)
            {
                if (numPao <= playerData[i].pao)
                {
                    numPao = playerData[i].pao;
                }

                if (playerData[i].gold >= numYing)
                {
                    numYing = playerData[i].gold;
                }
            }
            for (var i = 0; i < 4; i++)
            {
                var data = playerData[i];
                data.isYingJia = numYing == data.gold && numYing > 0;
                data.isPaoShou = numPao == data.pao && numPao > 0;
            }
        }

        public void OnBack2HallClick()
        {
            YxDebug.Log("返回大厅");
            App.QuitGame();
        }

        public void ShowGameOverPanel()
        {
            _showParent.SetActive(true);
            for (var i = 0; i < _playerItems.Length; i++)
            {
                _playerItems[i].InitInfo(playerData[i]);
            }
        }

        public void OnClickShare()
        {
            YxWindowManager.ShowWaitFor();

            Facade.Instance<WeChatApi>().InitWechat();

            UserController.Instance.GetShareInfo(info =>
            {
                YxWindowManager.HideWaitFor();
                Img.DoScreenShot(new Rect(0, 0, Screen.width, Screen.height), imageUrl =>
                {
                    if (Application.platform == RuntimePlatform.Android)
                    {
                        imageUrl = "file://" + imageUrl;
                    }
                    info.ImageUrl = imageUrl;
                    info.ShareType = ShareType.Image;
                    Facade.Instance<WeChatApi>().ShareContent(info, str =>
                    {
                        var parm = new Dictionary<string, object>
                        {
                            {"option", 2},
                            {"bundle_id", Application.bundleIdentifier},
                            {"share_plat", SharePlat.WxSenceTimeLine.ToString()}
                        };
                        Facade.Instance<TwManager>().SendAction("shareAwards", parm, null);
                    });
                });
            });
        }
    }
}