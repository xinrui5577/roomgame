using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.LXGameScripts.lx39
{
    /// <summary>
    /// 播放特效
    /// </summary>
    public class PlayLx39Effect : MonoBehaviour
    {
        public UISprite TargetSprite;//蛋中的图片
        public UISprite ZaDanSprite;//砸蛋的图片
        public UISprite BaoZa;//爆炸效果的图片
        private int bzNum = 8;//爆炸需要播放图片的数量
        private int haveNum = 16;//砸蛋需要播放的图片数量
        private string _bzName = "BaoZa_";
        private string _pictureName = "zadan_effect_";

        public List<GameObject> JiuLians = new List<GameObject>();//九连图片
        public UISprite JiuLian;//九连图片的父物体
        public Transform NineGrid;//九连排序数字的Grid
        public GameObject NineItem;//九连的数字预设体
        private int jiulianNum = 12;//九连需要播放图片的数量

        public GameObject CaiDai;//彩带特效父物体
        public Transform CaiDaiGrid;//彩带排序数字的Grid
        public GameObject CaiDaiItem;//彩带的数字预设体
        public Transform Icon;
        public List<GameObject> AwardIcons = new List<GameObject>();//奖励类型图片
        public List<GameObject> XxIcons = new List<GameObject>();//星星图片


        private string numName = "num_";//数字图集的图片名字
        private string jettonName = "main_";//
        private List<GameObject> Items = new List<GameObject>();//克隆出来的数字物体
        private Coroutine _playEnable;

        /// <summary>
        /// 在这里判断显示那个特效
        /// </summary>
        public void ChooseEffectPlay()
        {
            ResponseData data = App.GetGameData<OverallData>().Response;
            if (!data.IsWin) return;
            if (data.IsSame)
            {
                JiuLian.spriteName = jettonName + data.JettonList[0];
                StartCoroutine("PlayJiuLian");
                return;
            }
            SureAwardTypeIcon(data.MaxLine, data.JettonList);
            StartCoroutine("PlayXingXingScale");
            Facade.Instance<MusicManager>().Play("win");//在这里播放中奖的声音
        }

        #region 播放砸蛋特效
        public void PlayZaDanEffect(EventData data)
        {
            if (data == null) return;
            string inName = data.data1.ToString();
            ZaDanSprite.gameObject.SetActive(true);
            _playEnable = StartCoroutine(PlayEnable(inName));
        }
        /// <summary>
        /// 更改图集图片来播放砸蛋动画
        /// </summary>
        private IEnumerator PlayEnable(string inDanName)
        {
            var wait = new WaitForSeconds(0.1f);
            for (var i = 0; i <= haveNum; i++)
            {
                if (i == 12)
                    StartCoroutine("PlayBaoZa");
                if (i == 14)
                {
                    TargetSprite.gameObject.SetActive(true);
                    TargetSprite.spriteName = inDanName;//确定蛋中的图片
                }
                ZaDanSprite.spriteName = _pictureName + i;
                yield return wait;
                if (i == haveNum)
                {
                    yield return new WaitForSeconds(0.5f);
                    ZaDanSprite.gameObject.SetActive(false);
                    TargetSprite.gameObject.SetActive(false);
                }
            }
        }
        /// <summary>
        /// 播放爆炸效果
        /// </summary>
        /// <returns></returns>
        private IEnumerator PlayBaoZa()
        {
            BaoZa.gameObject.SetActive(true);
            var wait = new WaitForSeconds(0.05f);
            for (var i = 0; i <= bzNum; i++)
            {
                BaoZa.spriteName = _bzName + i;
                yield return wait;
                if (i == bzNum)
                    BaoZa.gameObject.SetActive(false);
            }
        }
        /// <summary>
        /// 隐藏砸蛋特效
        /// </summary>
        public void HideZaDan()
        {
            StopCoroutine("PlayBaoZa");
            if (_playEnable != null) StopCoroutine(_playEnable);
            TargetSprite.gameObject.SetActive(false);
            ZaDanSprite.gameObject.SetActive(false);
            BaoZa.gameObject.SetActive(false);
        }

        #endregion

        #region 播放彩带特效

        /// <summary>
        /// 播放星星特效
        /// </summary>
        private IEnumerator PlayXingXingScale()
        {
            yield return new WaitForSeconds(1f);
            CaiDai.SetActive(true);
            Icon.gameObject.SetActive(true);
            ShowScoreByNumAtlas(CaiDaiItem, CaiDaiGrid);
            for (int i = 0; i < 2; i++)
            {
                XxIcons[i].SetActive(true);
                iTween.ScaleFrom(XxIcons[i], Vector3.zero, 1f);
            }
            yield return new WaitForSeconds(0.5f);
            for (int i = 2; i < XxIcons.Count; i++)
            {
                XxIcons[i].SetActive(true);
                iTween.ScaleFrom(XxIcons[i], Vector3.zero, 1f);
            }
        }
        /// <summary>
        /// 确定奖励类型,做的只适用于3*3
        /// </summary>
        private void SureAwardTypeIcon(int line, List<int> targetList)
        {
            int temp1;
            int temp2;
            int temp3;
            switch (line)
            {
                case 0:
                case 1:
                case 2:
                    temp1 = targetList[line];
                    temp2 = targetList[line + 3];
                    temp3 = targetList[line + 6];
                    SureAwardIconName(temp1, temp2, temp3);
                    break;
                case 3:
                case 4:
                case 5:
                    temp1 = targetList[3 * line - 9];
                    temp2 = targetList[3 * line - 8];
                    temp3 = targetList[3 * line - 7];
                    SureAwardIconName(temp1, temp2, temp3);
                    break;
                case 6:
                    temp1 = targetList[line];
                    temp2 = targetList[line - 2];
                    temp3 = targetList[line - 4];
                    SureAwardIconName(temp1, temp2, temp3);
                    break;
                case 7:
                    temp1 = targetList[line + 1];
                    temp2 = targetList[line - 3];
                    temp3 = targetList[line - 7];
                    SureAwardIconName(temp1, temp2, temp3);
                    break;
            }
        }

        private void SureAwardIconName(int temp1, int temp2, int temp3)
        {
            if (temp1 == temp2 && temp1 == temp3)
            {
                foreach (var icon in AwardIcons)
                {
                    icon.GetComponent<UISprite>().spriteName = jettonName + temp1;
                }
            }
            else if ((temp1 <= 52 && temp1 >= 50) && (temp2 <= 52 && temp2 >= 50) && (temp3 <= 52 && temp3 >= 50))
            {
                foreach (var icon in AwardIcons)
                {
                    icon.GetComponent<UISprite>().spriteName = jettonName + "three";
                }
            }
            else if (temp1 == temp2 && temp1 == 1000)
            {
                for (int i = 0; i < AwardIcons.Count; i++)
                {
                    if (i != AwardIcons.Count - 1)
                        AwardIcons[i].GetComponent<UISprite>().spriteName = jettonName + temp1;
                    else
                        AwardIcons[i].GetComponent<UISprite>().spriteName = jettonName + "any";
                }
            }
            else if (temp1 == 1000)
            {
                for (int i = 0; i < AwardIcons.Count; i++)
                {
                    if (i == 0)
                        AwardIcons[i].GetComponent<UISprite>().spriteName = jettonName + temp1;
                    else
                        AwardIcons[i].GetComponent<UISprite>().spriteName = jettonName + "any";
                }
            }
        }

        public void HideCaiDaiEffect()
        {
            HideEffect("PlayXingXingScale", XxIcons, CaiDai);
        }
        #endregion

        #region 播放九连特效
        /// <summary>
        /// 播放九连背景变大的图片
        /// </summary>
        private IEnumerator PlayJiuLian()
        {
            yield return new WaitForSeconds(1f);
            ShowScoreByNumAtlas(NineItem, NineGrid);
            JiuLian.gameObject.SetActive(true);
            var wait = new WaitForSeconds(0.3f);
            for (var i = 0; i < JiuLians.Count; i++)
            {
                JiuLians[i].SetActive(true);
                yield return wait;
                if (i != JiuLians.Count - 1)
                    JiuLians[i].SetActive(false);
            }
        }
        /// <summary>
        /// 隐藏九连特效
        /// </summary>
        public void HideNineEffect()
        {
            HideEffect("PlayJiuLian", JiuLians, JiuLian.gameObject);
        }


        #endregion
        /// <summary>
        /// 显示所得分数
        /// </summary>
        private void ShowScoreByNumAtlas(GameObject item, Transform grid)
        {
            Items.Clear();
            var gold = App.GetGameData<OverallData>().Response.GetJackpotGold;
            var NineScore = YxUtiles.GetShowNumber(gold);
            var NineNum = NineScore.ToString().Length;
            for (var i = 0; i < NineNum; i++)
            {
                var go = Instantiate(item, grid.transform) as GameObject;
                Items.Add(go);
                var goSprite = go.GetComponent<UISprite>();
                goSprite.spriteName = numName + NineScore.ToString()[i];
                go.SetActive(true);
            }
            grid.GetComponent<UIGrid>().enabled = true;
        }
        /// <summary>
        /// 隐藏九连或彩带特效
        /// </summary>
        /// <param name="play">播放特效的协程</param>
        /// <param name="gos">初始的游戏物体列表</param>
        /// <param name="par">父物体</param>
        protected void HideEffect(string play, List<GameObject> gos, GameObject par)
        {
            StopCoroutine(play);
            par.SetActive(false);
            foreach (var go in gos)
            {
                go.SetActive(false);
            }
            while (Items.Count != 0)
            {
                Destroy(Items[0]);
                Items.RemoveAt(0);
            }
        }
    }
}