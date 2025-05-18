using UnityEngine;
using System.Collections;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.bjl3d
{
    public class Pai : MonoBehaviour
    {
        private Hashtable _args;

        public static Pai GetInstance(int num,string path,float times,int rOl = -1)
        {
            if (num == 0) return null;
            var go = ResourceManager.LoadAsset("Pai_0" +num, "pai/Pai_0" + num).transform;
            //var go1 = Instantiate(go);
            var go1 = (Transform)Instantiate(go, App.GetGameManager<Bjl3DGameManager>().TheGameScene.PaiList);
            var obj = go1.transform;
            if (obj == null) return null;
            obj.gameObject.SetActive(true);
            obj.localScale = new Vector3(1.3f,0.1f,1.3f);
            var pai = obj.GetComponent<Pai>();
            pai.Init(path,times,rOl);
            return pai;
        }

        void Init(string path,float times,int rol = -1)
        {
            StartCoroutine(SetFishPath(path, times,rol));
        }

        //产生路径
        IEnumerator SetFishPath(string pathId, float s,int rol)
        {
            yield return new WaitForSeconds(s);

            //发牌动作
            App.GetGameManager<Bjl3DGameManager>().TheGameScene.GirlSendCard(rol);
            Facade.Instance<MusicManager>().Play("Sendcard"); 
            var path = iTweenPath.GetPath("PaiPath" + pathId/*Random.Range(0, iTweenPath.paths.Count)*/);
             transform.position = path[0];
            _args = new Hashtable {{"path", path}, {"easeType", iTween.EaseType.linear}};
            //设置路径的点
            //设置类型为线性，线性效果会好一些。
            //设置寻路的速度 尽量把速度区别开来
            var data = App.GetGameData<Bjl3DGameData>();
            if (data!=null)
            {
                var gameCfg = data.GameConfig;
                if (rol == 0)
                {
                    var speed = 11.3f + (gameCfg.XFapaiSpeedflag * 7 / 10.0f);
                    _args.Add("speed", speed);
                    gameCfg.XFapaiSpeedflag++;
                }
                else
                    _args.Add("speed", 14);
                //是否先从原始位置走到路径中第一个点的位置
                _args.Add("movetopath", true);
                //是否让模型始终面朝当面目标的方向，拐弯的地方会自动旋转模型
                //如果你发现你的模型在寻路的时候始终都是一个方向那么一定要打开这个
                _args.Add("oncomplete", "ItweenAnimationEnd");
                //让模型开始寻路
                iTween.MoveTo(gameObject, _args);
            }

           
        }

        

    }
}