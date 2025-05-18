using Assets.Scripts.Game.lswc.Data;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using YxFramwork.Common;
using com.yxixia.utile.YxDebug;
using YxFramwork.Tool;

public class LSResultControl : MonoBehaviour
{

    private Transform _resultsParent;

    private Text _betNum;

    private Text _winNum;

    private GameObject _fireworks;

    private Image _background;

    private float _backgroundTweenTime = 3;

    private float _scaleTweenTime=1;

    private GridLayoutGroup _grid;

    private Vector3 tweenScale=new Vector3(1.1f,1.1f,1.1f);

    private float _endValue=0.5f;

    private float _waitShowFireworkTime = 1;


    private void Awake()
    {

        Find();
    }

    private void Find()
    {
        _resultsParent = transform.FindChild("Result");
        _betNum = transform.FindChild("BETnum").GetComponent<Text>();
        _winNum = transform.FindChild("WINnum").GetComponent<Text>();
        _fireworks = transform.FindChild("firworks").gameObject;
        _background = transform.GetComponent<Image>();
        _grid = _resultsParent.GetComponent<GridLayoutGroup>();
    }

    public void ShowResultInfo(List<int> list ,long betNum,long winNum,int multiple)
    {
        ResetResultPanel();
        for (int i = 0; i < list.Count; i++)
        {
            _resultsParent.GetChild(i).gameObject.SetActive(true);
            //animal
            _resultsParent.GetChild(i).GetChild(0).GetComponent<Image>().sprite =
                App.GetGameData<LswcGameData>().GetSprite(((LSAnimalSpriteType)list[i]).ToString());
            //倍率
            int peiNum = 1;
            if(multiple==0)
            {
               
            }
            else
            {
                peiNum = multiple;
            }
            YxDebug.Log(list[i] + "号位的倍率是" + App.GetGameData<LswcGameData>().PeiLvs[list[i]] + ",当前的倍数是：" + peiNum);
            peiNum = App.GetGameData<LswcGameData>().PeiLvs[list[i]] * peiNum;
            _resultsParent.GetChild(i).GetChild(1).GetChild(0).GetComponent<Text>().text ="X"+peiNum.ToString();
        }
        _betNum.text = YxUtiles.GetShowNumberToString(betNum);
        _winNum.text = YxUtiles.GetShowNumberToString(winNum);
        PlayAnimation();
    }

    private void PlayAnimation()
    {
        Sequence mySequence = DOTween.Sequence();
        Tweener tScale = transform.DOScale(tweenScale, _scaleTweenTime);
        Tweener tColor = _background.DOFade(_endValue, _backgroundTweenTime);
        tColor.SetEase(Ease.InQuart);///在四分之一的时候处理？？？不知道
        mySequence.Append(tScale);
        mySequence.Join(tColor);    //执行时间为大小变化的同时，播放颜色变化。
        mySequence.AppendInterval(_waitShowFireworkTime);
        mySequence.AppendCallback(delegate()
            {
                _fireworks.SetActive(true);
            });
    }

    private void ResetResultPanel()
    {
        _background.color = Color.clear;
        transform.localScale = Vector3.zero;
    }


    private void Show(bool isShow)
    {
        for (int i = 0; i < App.GetGameData<LswcGameData>().LastResult.ShowResults.Count; i++)
        {
            _resultsParent.GetChild(i).gameObject.SetActive(isShow);
        }
    }

    public void Hide()
    {
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
            _fireworks.SetActive(false);
            Show(false);
        }
    }

}
