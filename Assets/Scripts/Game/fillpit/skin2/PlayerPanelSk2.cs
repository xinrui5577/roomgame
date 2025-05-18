using System.Linq;
using Assets.Scripts.Game.fillpit.ImgPress.Main;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.fillpit.skin2
{
    public class PlayerPanelSk2 : PlayerPanel
    {
        /// <summary>
        /// 有特殊语音的下注
        /// </summary>
        public int[] BetGoldSound;

        /// <summary>
        /// 通过名字显示玩家状态
        /// </summary>
        /// <param name="type"></param>
        /// <param name="betgold"></param>
        public override void PlayTypeSound(GameRequestType type, int betgold = 0)
        {
            switch (type)
            {
                case GameRequestType.BetSpeak:

                    if (BetGoldSound.Contains(betgold))
                    {
                        PlaySound(betgold + "f");
                    }
                    else
                    {
                        PlaySound("BetSpeak");
                    }
                    break;

                case GameRequestType.Bet:
                    break;

                case GameRequestType.Fold:
                    PlaySound("Fold_" + Random.Range(1, 4));  //三个声音随机一个
                    break;

                case GameRequestType.FollowSpeak:
                    PlaySound("Follow_" + Random.Range(1, 3));       //两个声音随机
                    break;
                case GameRequestType.KickSpeak:
                    PlaySound("KickSpeak_" + Random.Range(1, 3));    //两个声音随机
                    break;

                case GameRequestType.NotKick:
                    //PlaySound("NotKick_" + Random.Range(1, 4));      //三个声音随机
                    break;

                case GameRequestType.BackKick:

                    PlaySound("BackKick_" + Random.Range(1, 3));     //两个声音随机
                    break;
            }
        }

        /// <summary>
        /// 玩家播放声音(区别性别)
        /// </summary>
        /// <param name="clipName">声音名字</param>
        public override void PlaySound(string clipName)
        {
            Facade.Instance<MusicManager>().Play(clipName, Info.SexI != 1 ? "Woman":"Man");
        }

    }
}
