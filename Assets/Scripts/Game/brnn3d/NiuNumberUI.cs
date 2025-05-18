using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.brnn3d
{
    public class NiuNumberUI : MonoBehaviour
    {
        public Transform bg;
        public Image[] niuImg = new Image[5];
        public Transform[] NiuAreas = new Transform[5];
        public Transform[] niuEffects = new Transform[5];

        //牛几的图片
        public Sprite[] niuNImgs;

        private bool isCanShow = false;

        private int[] paiNiuJi;

        //显示牛数的界面
        public void ShowNumberUI(int[] paiShape)
        {
            paiNiuJi = paiShape;
            if (!bg.gameObject.activeSelf)
                bg.gameObject.SetActive(true);
            for (int i = 0; i < paiShape.Length; i++)
            {
                niuImg[i].sprite = niuNImgs[paiShape[i]];
            }
        }


        //显示牛数的区域
        public void ShowAreaNiu(int iareaId)
        {
            if (!NiuAreas[iareaId].gameObject.activeSelf)
                NiuAreas[iareaId].gameObject.SetActive(true);
            if (paiNiuJi[iareaId] == 10)
            {
                niuEffects[iareaId].gameObject.SetActive(true);
            }

        }
        //播放牛数的声音
        public void PlayAudioNiuJi(int iareaId, int[] niu)
        {
            Facade.Instance<MusicManager>().Play("n" + niu[iareaId]);
        }
        //隐藏显示牛数的界面
        public void HideNiuNumberUI()
        {
            if (bg.gameObject.activeSelf)
                bg.gameObject.SetActive(false);
            for (int i = 0; i < 5; i++)
            {
                NiuAreas[i].gameObject.SetActive(false);
                niuEffects[i].gameObject.SetActive(false);
            }
            App.GetGameData<Brnn3dGameData>().PaiAllShow.Clear();
        }

    }

}
