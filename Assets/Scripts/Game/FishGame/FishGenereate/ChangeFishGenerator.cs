using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using System.Collections;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.FishGenereate
{
    public class ChangeFishGenerator: MonoBehaviour
    {

        // Use this for initialization
        public float WaitTime = 5F;
        public int FishQuantity = 10;

        private FishGenerator MyFishGenerator;
        private Swimmer MySwimmer;

        public Fish MyFish;
        private float iSp;
        private bool isChangGenerator = false;

        private int mMaxFishAtWorldOrigin;//原来的最大鱼数
        void Start( )
        {

            MyFishGenerator = GameMain.Singleton.FishGenerator;
            var gdata = App.GetGameData<FishGameData>();
            gdata.EvtFishInstance += Handle_Change;
            gdata.EvtFishClear += Handle_Resumme;
            //Debug.Log( "inStart");  


        }
        void Handle_Change( Fish f )
        {
            if(f.TypeIndex == MyFish.TypeIndex&&GameMain.State_ == GameMain.State.Normal&&!isChangGenerator)
            {
                MySwimmer = f.GetComponent<Swimmer>( );
                //Debug.Log( "inchang" );
                iSp = MySwimmer.Speed;

                StartCoroutine( "Waite" );

            }
        }

        void Handle_Resumme( Fish f )
        {
            if(f.TypeIndex == MyFish.TypeIndex&& isChangGenerator)
            {
                //Debug.Log( "inresumme" );
                // float iSp = MySwimmer.Speed;
                MyFishGenerator.MaxFishAtWorld = mMaxFishAtWorldOrigin;//恢复原来最大鱼数目
                isChangGenerator = false; 
            }
        }

        IEnumerator Waite( )
        {
            MySwimmer.Speed = 0F;
            mMaxFishAtWorldOrigin = MyFishGenerator.MaxFishAtWorld;//记录原来的最大鱼限制数
            MyFishGenerator.MaxFishAtWorld = FishQuantity * GameMain.Singleton.ScreenNumUsing;
            isChangGenerator = true;
            yield return new WaitForSeconds( WaitTime );
            MySwimmer.Speed = iSp;
        }
    }
}

