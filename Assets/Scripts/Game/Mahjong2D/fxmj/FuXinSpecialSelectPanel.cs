using Assets.Scripts.Game.Mahjong2D.Game.Component.Piao;
using Assets.Scripts.Game.Mahjong2D.Game.Data;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Mahjong2D.fxmj
{
    public class FuXinSpecialSelectPanel : PiaoSelectPanel
    {
        public override void ShowGameObject(bool state = true)
        {
            base.ShowGameObject(state);
            var num = App.GetGameData<Mahjong2DGameData>().XiaZhiValue;
            FuXinShow(num, state);
        }
        /// <summary>
        /// 飘的显示的处理
        /// </summary>
        /// <param name="state"></param>
        /// <param name="num"></param>
        private void FuXinShow(int num = 0, bool state = true)
        {
            if (state)
            {
                if (num == 2)
                {
                    transform.GetChild(2).gameObject.SetActive(false);
                }
                else if (num == 5)
                {
                    transform.GetChild(1).gameObject.SetActive(false);
                }
                else
                {
                    gameObject.SetActive(false);
                }
                gameObject.GetComponent<UIGrid>().repositionNow = true;
            }
        }
    }
}
