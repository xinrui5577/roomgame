using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jsys
{
    public class AnimationManager : MonoBehaviour
    {
        //下注按钮的图片数组
        public Image[] BetAnimationImages;

        //显示倍数的图片
        public Image BeishuImage;

        //倍数的数组
        public Sprite[] BeishuSprites;

        /// <summary>
        /// 收到转动结果消息处理,显示获奖动画
        /// </summary>
        public void ShowAnimation()
        {
            //Debug.Log("收到转动结果消息处理,显示获奖动画");
            var gdata = App.GetGameData<JsysGameData>();
            App.GetGameManager<JsysGameManager>().BetPanelMgr.ShowiWiningText(gdata.Winning);
            SetBeishuSprite(gdata.Multiplying[gdata.EndAnimal]);
            Facade.Instance<MusicManager>().Play("Animal" + gdata.EndAnimal + "");
//            StartCoroutine(PlaySound(App.GetGameData<GlobalData>().EndAnimal));
            //显示此局游戏所出现的小动物按钮的闪动
            Invoke("ShowBetPanel", 1.0f);
        }

        IEnumerator PlaySound(int num)
        {
            if (num == 8 || num == 9)
            {
                yield return new WaitForSeconds(1f);
                Facade.Instance<MusicManager>().Play("Dajiang");
            }
        }

        public void ShowBetPanel()
        {
            var gdata = App.GetGameData<JsysGameData>();
            var turnGroupsMgr = App.GetGameManager<JsysGameManager>().TurnGroupsMgr;
            if (gdata.IsShark)
            {
                if (8 == gdata.SharkPos)
                {
                    turnGroupsMgr.GameConfig.IsSliverShark = true;

                }
                if (9 == gdata.SharkPos)
                {
                    turnGroupsMgr.GameConfig.IsGoldShark = true;
                }
                BetAnimationImages[gdata.SharkPos].gameObject.SetActive(true);
            }
            else
            {
                if (0 <= gdata.EndAnimal && gdata.EndAnimal <= 3)
                {
                    BetAnimationImages[10].gameObject.SetActive(true);
                }
                if (3 < gdata.EndAnimal && gdata.EndAnimal < 8)
                {
                    BetAnimationImages[11].gameObject.SetActive(true);
                }
                if (turnGroupsMgr.GameConfig.IsGoldShark)
                {
                    BetAnimationImages[9].gameObject.SetActive(true);
                    turnGroupsMgr.GameConfig.IsGoldShark = false;
                }
                if (turnGroupsMgr.GameConfig.IsSliverShark)
                {
                    BetAnimationImages[8].gameObject.SetActive(true);
                    turnGroupsMgr.GameConfig.IsSliverShark = false;
                }
                BetAnimationImages[gdata.EndAnimal].gameObject.SetActive(true);
            }
        }

        public void HideBetPanel()
        {
            foreach (var image in BetAnimationImages)
            {
                if (image.gameObject.activeSelf)
                {
                    image.gameObject.SetActive(false);
                }
            }
        }

        //隐藏获奖动画
        public void HideAnimation()
        {
            BeishuImage.gameObject.SetActive(false);
        }
        //转动前显示金鲨银鲨动画
        public void ShowGoldSharkAnimation()
        {
            Invoke("HidGoldSharkAnimation", 4.0f);
        }
        //隐藏金鲨银鲨动画
        public void HidGoldSharkAnimation()
        {
            App.GetGameManager<JsysGameManager>().TurnGroupsMgr.ChangeState();
            Facade.Instance<MusicManager>().Stop();
            Facade.Instance<MusicManager>().Play("Paodeng");
        }
        //设置倍数动画
        public void SetBeishuSprite(int beishu)
        {
            switch (beishu)
            {
                case 3:
                    {
                        BeishuImage.sprite = BeishuSprites[0];
                    }
                    break;
                case 4:
                    {
                        BeishuImage.sprite = BeishuSprites[1];
                    }
                    break;
                case 5:
                    {
                        BeishuImage.sprite = BeishuSprites[1];
                    }
                    break;
                case 6:
                    {
                        BeishuImage.sprite = BeishuSprites[2];
                    }
                    break;
                case 8:
                    {
                        BeishuImage.sprite = BeishuSprites[3];
                    }
                    break;
                case 12:
                    {
                        BeishuImage.sprite = BeishuSprites[4];
                    }
                    break;
                case 24:
                    {
                        BeishuImage.sprite = BeishuSprites[5];
                    }
                    break;
            }
            BeishuImage.gameObject.SetActive(true);
        }
    }
}
