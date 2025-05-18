using System.Collections;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class PlayerOther : MonoBehaviour
    {
        public Text[] PlayerAddGold;
        public CpgEffect[] PlayerEffect;
        public GameObject[] PlayerReady;
        public UserTalkItem[] PlayerTalker;

        public void ResetReadyState()
        {
            for (int i = 1; i < PlayerReady.Length; i++)
            {
                PlayerReady[i].gameObject.SetActive(false);
            }
        }

		public virtual void SetUserAddGold(int chair, int glod)
        {
            PlayerAddGold[chair].gameObject.SetActive(true);
            PlayerAddGold[chair].text = "" + glod;

            HideObjWithTime(PlayerAddGold[chair].gameObject, 3);
        }

        private void HideObjWithTime(GameObject txt, float time)
        {
            StartCoroutine(HideObjWithTimeIEnumerator(txt,time));
        }

        private IEnumerator HideObjWithTimeIEnumerator(GameObject txt,float time)
        {
            yield return new WaitForSeconds(time);

            txt.gameObject.SetActive(false);
        }

        public void PlayEffect(int chair, EnCpgEffect effect)
        {
            PlayerEffect[chair].PlayEffect(effect);
        }

        public void PlayReady(int chair,bool isReady)
        {
            PlayerReady[chair].SetActive(isReady);
            PlayerReady[chair].GetComponent<TweenPosition>().PlayForward();
        }

        //表情
        public void UserTalk(int chair,EnChatType type,object Content)
        {
            switch (type)
            {
                case EnChatType.exp:
                    int keyExp = (int)Content;
                    GameObject objExp = ChatManager.Instance.GetExpress(keyExp);
                    PlayerTalker[chair].SetContent(objExp);
                    break;
                case EnChatType.sorttalk:
                    int keyTalk = (int)Content;
                    GameObject objTalk = ChatManager.Instance.GetSortTalk(keyTalk);
                    PlayerTalker[chair].SetContent(objTalk);
                    SoundManager.Instance.PlaySortTalk(chair,keyTalk);
                    break;
                case EnChatType.text:
                    string text = (string)Content;
                    GameObject objText = ChatManager.Instance.GetText(text);
                    PlayerTalker[chair].SetContent(objText);
                    break;
            }
        }

        public void SetEmpty(int chair)
        {
            PlayerAddGold[chair].gameObject.SetActive(false);
            PlayerReady[chair].gameObject.SetActive(false);
            PlayerTalker[chair].Reset();
        }
                  
        public void SetUsersAddGold(int chair, int gold)
        {
            UsersAddGold UsersAddGold = GetComponent<UsersAddGold>();

            if (null != UsersAddGold)
            {
                UsersAddGold.SetUsersAddGold(chair, gold);                
            }      
        }

    }
}
