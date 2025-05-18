using com.yxixia.utile.Utiles;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.Shuihuzhuan
{
    public class WinPanelControl : MonoBehaviour
    {
        public static WinPanelControl instance;

        public GameObject winPanel;

        public Text winText;

        public Sprite[] sprites;

        public Image titleImage;

        private float curtime = 0f;

        private void Awake()
        {
            instance = this;
        }

        private void ChangeSprite()
        {
            titleImage.sprite = titleImage.sprite == sprites[0] ? sprites[1] : sprites[0];
        }

        public void ShowWinPanel()
        {
            winPanel.SetActive(true);
            SetWinText(App.GetGameData<WmarginGameData>().iWinMoney);
            Invoke("HideWinPanel", 10);
        }

        public void SetWinText(int coin)
        {
            winText.text = YxUtiles.GetShowNumberToString(coin);
        }

        public void HideWinPanel()
        {
            if (winPanel.activeSelf == true)
                winPanel.SetActive(false);
        }

        void Update()
        {
            curtime += Time.deltaTime;
            if (curtime > 1)
            {
                ChangeSprite();
                curtime = 0;

            }

        }
    }


}
