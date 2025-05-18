using UnityEngine;
using System.Collections;
using YxFramwork.Common;

namespace Assets.Scripts.Game.brnn3d
{
    public class DicMode : MonoBehaviour
    { 
        public Transform DicTf;

        public Transform BoultTf;
        //点数
        int _pN = 0; 

        //玩骰子
        public void PlayDic()
        {
            _pN = App.GetGameData<Brnn3dGameData>().DicNum;
            if (DicTf.gameObject.activeSelf)
                DicTf.gameObject.SetActive(false);
            DicTf.gameObject.SetActive(true);
            ShowPNumber();
        }

        //显示骰子点数
        public void ShowPNumber()
        {
            switch (_pN)
            {
                case 1:
                    BoultTf.localEulerAngles = new Vector3(0, 0, 0);
                    DicTf.localEulerAngles = new Vector3(0, 0, 0);
                    break;
                case 2:
                    BoultTf.localEulerAngles = new Vector3(0, 0, 90);
                    DicTf.localEulerAngles = new Vector3(0, 90, 0);
                    break;
                case 3:
                    BoultTf.localEulerAngles = new Vector3(-90, 0, 0);
                    DicTf.localEulerAngles = new Vector3(0, -90, 0);
                    break;
                case 4:
                    BoultTf.localEulerAngles = new Vector3(0, 0, 180);
                    DicTf.localEulerAngles = new Vector3(0, 180, 0);
                    break;
                case 5:
                    BoultTf.localEulerAngles = new Vector3(0, 0, 270);
                    DicTf.localEulerAngles = new Vector3(0, 270, 0);
                    break;
            }
        }

        //停止骰子
        public void StopDic()
        {
            StartCoroutine(HideDic(2f));
        }

        IEnumerator HideDic(float s)
        {
            yield return new WaitForSeconds(s);
            if (DicTf.gameObject.activeSelf)
                DicTf.gameObject.SetActive(false);
        }
    }
}

