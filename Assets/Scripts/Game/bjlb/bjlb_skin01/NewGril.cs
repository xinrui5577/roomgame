using System.Collections;
using UnityEngine;
using com.yxixia.utile.YxDebug;
using YxFramwork.Common;
using YxFramwork.View;
using Assets.Scripts.Game.bjlb;


namespace Assets.Scripts.Game.bjlb.bjlb_skin01
{
    public class NewGril : MonoBehaviour
    {

        [SerializeField] private GameObject _grilObj = null;

        private UISpriteAnimation _spriteAnim = null;

        private UISprite _grilSprite = null;

        private const string AnimNormal = "Normal_";

        private const string AnimReward = "Reward_";

        private const int AnimSpeed = 14;

        void Start()
        {

            _spriteAnim = _grilObj.GetComponent<UISpriteAnimation>() ?? _grilObj.AddComponent<UISpriteAnimation>();
            //_spriteAnim.enabled = false;
            _spriteAnim.framesPerSecond = AnimSpeed;
            _grilSprite = _grilObj.GetComponent<UISprite>() ?? _grilObj.AddComponent<UISprite>();
            _grilSprite.name = AnimNormal + "0";
            StartCoroutine(DoNormal());
        }

        IEnumerator DoNormal()
        {
            _spriteAnim.enabled = true;
            while (true)
            {
                yield return new WaitForSeconds(8f);

                if (_spriteAnim.isPlaying) continue;


                _spriteAnim.namePrefix = AnimNormal;
                _spriteAnim.Play();

            }
            // ReSharper disable once FunctionNeverReturns
        }
 

        public void OnClickGift()
        {

            //if(_spriteAnim.isPlaying)
            //    return;
            if (App.GameData.GetPlayerInfo().CoinA < 100)
            {
                YxMessageBox.DynamicShow(new YxMessageBoxData { Msg = "金币不够,不能打赏.", });
                return;
            }

            //App.GetRServer<GameServer>().Reward();
            _spriteAnim.namePrefix = AnimReward;
            _spriteAnim.ResetToBeginning();
            _spriteAnim.Play();
        }

        void OnDisable()
        {
            StopAllCoroutines();
        }
        
        void OnDestroy()
        {
            StopAllCoroutines();
        }
    }


}