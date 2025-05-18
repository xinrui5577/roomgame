//using System.Collections.Generic;
//using System.Globalization;
//using UnityEngine;
//using YxFramwork.Common;
//using Assets.Scripts.Common.components;
//using YxFramwork.Manager;
//using System.Collections;
//using Assets.Scripts.Common.Adapters;
//using Assets.Scripts.Game.paijiu.Base;
//using Assets.Scripts.Game.paijiu.ImgPress.Main;
//using YxFramwork.Common.DataBundles;
//using YxFramwork.Framework.Core;
//
//
//namespace Assets.Scripts.Game.paijiu
//{
//
//    public class PlayCtrl : MonoBehaviour
//    {
//        public GameObject ParentObj;
//        /// <summary>
//        /// 背景图
//        /// </summary>
//        public GameObject Bg;
//        /// <summary>
//        /// 黑色背景图
//        /// </summary>
//        public GameObject BlackBg;
//        /// <summary>
//        /// 调整显示位置的Grid
//        /// </summary>
//        public UIGrid PosGrid;
//        /// <summary>
//        /// 界面中显示动画的区域
//        /// </summary>
//        public GameObject AnimationBg;
//        /// <summary>
//        /// 人物头像
//        /// </summary>
//        public NguiTextureAdapter UserIcon;
//        /// <summary>
//        /// 玩家的昵称
//        /// </summary>
//        public UILabel NameLabel;
//        /// <summary>
//        /// 玩家ID
//        /// </summary>
//        public UILabel IdLabel;
//        /// <summary>
//        /// 玩家IP
//        /// </summary>
//        public UILabel AddressLabel;
//        /// <summary>
//        /// 被点及玩家的座位号
//        /// </summary>
//        private int _clickUserSeat;
//        /// <summary>
//        /// 动画的图集数组
//        /// </summary>
//        public UIAtlas[] AnimationAtlas;
//        /// <summary>
//        /// 播放动画的图片
//        /// </summary>
//        public UISprite AnimationPicture;
//
//
//        public UIGrid Grid;
//
//        public List<GameObject> MovePos;
//        /// <summary>
//        /// 动画图片的移动时间
//        /// </summary>
//        public float MoveTime = 2;
//        private TweenPosition _pos;
//
//        /// <summary>
//        /// 是否显示礼物界面
//        /// </summary>
//        [SerializeField]
//        private bool _showGifts = false;
//        protected void Start()
//        {
//            if (Bg.activeSelf) Bg.SetActive(false);
//            if (BlackBg.activeSelf) BlackBg.SetActive(false);
//        }
//        /// <summary>
//        /// 点击关闭按钮
//        /// </summary>
//        public void Close()
//        {
//            Bg.SetActive(false);
//            BlackBg.SetActive(false);
//        }
//        /// <summary>
//        /// 点击头像之后显示人物信息
//        /// </summary>
////        public void ShowUserInfo(bool isSelf, PaiJiuUserInfo info)
////        {
////            Bg.SetActive(true);
////            BlackBg.SetActive(true);
////
////            if(_showGifts && !isSelf) //不是自己显示礼物选项，是自己不显示礼物选项
////            {
////                AnimationBg.SetActive(true);
////                //Grid.transform.localPosition = new Vector3(Grid.transform.localPosition.x, 6, 0);   //强制控制坐标变换
////                Grid.Reposition();
////            }
////            else
////            {
////                AnimationBg.SetActive(false);
////                //Grid.transform.localPosition = new Vector3(Grid.transform.localPosition.x, 42, 0); //强制控制坐标变换
////                Grid.Reposition();
////            }
////           
////            PosGrid.repositionNow = true;
////            PortraitDb.SetPortrait(info.AvatarX, UserIcon, info.SexI);
////            NameLabel.text = info.NickM;
////            IdLabel.text = info.Id.ToString(CultureInfo.InvariantCulture);
////
////            AddressLabel.text = info.Country;
////            _clickUserSeat = info.Seat;
////        }
//        /// <summary>
//        /// 点击动画的图标
//        /// </summary>
////        public void OnClickAnimateCard(GameObject obj)
////        {
////            App.GetRServer<PaiJiuGameServer>().UserAnimate(int.Parse(obj.name), _clickUserSeat);
////        }
//
//        /// <summary>
//        /// 接受服务器发过来的动画的信息
//        /// </summary>
////        public void ReceiveAni(int index, int seat, int otherSeat)
////        {
////            float offestY = 0;
////            float offestX = 0;
////            Bg.SetActive(false);
////            BlackBg.SetActive(false);
////            var instantiate = Instantiate(AnimationPicture.gameObject);
////            instantiate.transform.parent = ParentObj.transform;
////            instantiate.transform.localPosition = Vector3.zero;
////            instantiate.transform.localScale = Vector3.one;
////            instantiate.transform.localEulerAngles = Vector3.one;
////            instantiate.gameObject.SetActive(true);
////            instantiate.GetComponent<UISprite>().atlas = AnimationAtlas[index];
////            instantiate.GetComponent<UISprite>().spriteName = "00";
////
////            switch (index)
////            {
////                case 2:
////                    offestX = 0.05f;
////                    offestY = 0.08f;
////                    break;
////                case 4:
////                    offestX = -0.07f;
////                    offestY = 0.12f;
////                    break;
////                case 5:
////                    offestY = 0.02f;
////                    break;
////            }
////
////            _index = index;
////            instantiate.GetComponent<UISprite>().GetAtlasSprite();
////            instantiate.GetComponent<UISprite>().MakePixelPerfect();
////            var selfPos = MovePos[seat].transform.position;
////            instantiate.transform.position = new Vector3(selfPos.x, selfPos.y, 0);
////            var otherPos = MovePos[otherSeat].transform.position;
////            _pos = TweenPosition.Begin(instantiate.gameObject, MoveTime, new Vector3(otherPos.x + offestX, otherPos.y + offestY), true);
////            var del = new EventDelegate(this, "Finised");
////            del.parameters[0].obj = instantiate;
////            if (!_pos.onFinished.Contains(del))
////            {
////                _pos.onFinished.Add(del);
////            }
////        }
//
//        private int _index;
//        /// <summary>
//        /// 播放动画的图片移动到位置之后
//        /// </summary>
//        // ReSharper disable once UnusedMember.Local
////        private void Finised(GameObject obj)
////        {
////            switch (_index)
////            {
////                case 0:
////                    Facade.Instance<MusicManager>().Play("flower");
////                    break;
////                case 1:
////                    Facade.Instance<MusicManager>().Play("tomato");
////                    break;
////                case 2:
////                    Facade.Instance<MusicManager>().Play("chicken");
////                    break;
////                case 3:
////                    Facade.Instance<MusicManager>().Play("cup");
////                    break;
////                case 4:
////                    Facade.Instance<MusicManager>().Play("bucket");
////                    break;
////                case 5:
////                    Facade.Instance<MusicManager>().Play("bomb");
////                    break;
////            }
////
////            UISpriteAnimation ua = obj.GetComponent<UISpriteAnimation>();
////            ua.enabled = true;
////            float atlasCount = obj.GetComponent<UISprite>().atlas.spriteList.Count;
////            float framesRate = ua.framesPerSecond;
////            ua.loop = false;
////            float fd = atlasCount / framesRate;
////            StartCoroutine(Disappear(obj, fd));
////        }
//        /// <summary>
//        /// 动画播放完毕之后消失
//        /// </summary>
////        IEnumerator Disappear(GameObject obj, float time)
////        {
////            yield return new WaitForSeconds(time);
////            obj.gameObject.SetActive(false);
////            Destroy(obj);
////        }
//    }
//}
