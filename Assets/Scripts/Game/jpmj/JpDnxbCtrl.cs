using Assets.Scripts.Game.jpmj.MahjongScripts.GameTable;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;

namespace Assets.Scripts.Game.jpmj
{
    public class JpDnxbCtrl : DnxbCtl
    {
/*        private bool _hasSetDnxb = false;
        public override void SetPlayerDnxb(EnDnxbDir dir)
        {
          //  if(_hasSetDnxb) return;

            base.SetPlayerDnxb(dir);

          //  _hasSetDnxb = true;
        }*/

        public static void SetDnxb(DnxbCtl dnxbCtl,int bankerSeat, int playerSeat)
        {
            if (dnxbCtl != null)
            {

                if (bankerSeat == playerSeat)
                {
                    dnxbCtl.SetPlayerDnxb(EnDnxbDir.Dong);
                }
                else
                {
                    EnDnxbDir[] dir = null;
                    switch (bankerSeat)
                    {
                        case 0:
                            {
                                switch (UtilData.CurrGamePalyerCnt)
                                {
                                    case 2:
                                        dir = new[] { EnDnxbDir.Dong,EnDnxbDir.Xi };
                                        break;
                                    case 3:
                                        dir = new[] { EnDnxbDir.Dong, EnDnxbDir.Nan, EnDnxbDir.Xi};
                                        break;
                                    case 4:
                                        dir = new[] { EnDnxbDir.Dong, EnDnxbDir.Nan, EnDnxbDir.Xi, EnDnxbDir.Bei, };
                                        break;
                                }                              
                                break;
                            }
                        case 1:
                            {
                                switch (UtilData.CurrGamePalyerCnt)
                                {
                                    case 2:
                                        dir = new [] { EnDnxbDir.Xi, EnDnxbDir.Dong };
                                        break;
                                    case 3:
                                        dir = new [] { EnDnxbDir.Bei,EnDnxbDir.Dong, EnDnxbDir.Nan };
                                        break;
                                    case 4:
                                        dir = new [] { EnDnxbDir.Bei, EnDnxbDir.Dong, EnDnxbDir.Nan, EnDnxbDir.Xi, };
                                        break;
                                }
                                break;
                            }
                        case 2:
                            {
                                switch (UtilData.CurrGamePalyerCnt)
                                {
                                    case 3:
                                        dir = new [] { EnDnxbDir.Xi,EnDnxbDir.Bei, EnDnxbDir.Dong};
                                        break;
                                    case 4:
                                        dir = new [] { EnDnxbDir.Xi, EnDnxbDir.Bei, EnDnxbDir.Dong, EnDnxbDir.Nan};
                                        break;
                                }
                                break;
                            }
                        case 3:
                            {
                                switch (UtilData.CurrGamePalyerCnt)
                                {
                                    case 4:
                                        dir = new [] {EnDnxbDir.Nan , EnDnxbDir.Xi, EnDnxbDir.Bei, EnDnxbDir.Dong};
                                        break;
                                }
                                break;
                            }
                    }

                    if (dir != null && dir.Length > playerSeat)
                        dnxbCtl.SetPlayerDnxb(dir[playerSeat]);
                }


/*                switch (UtilData.CurrGamePalyerCnt)
                {
                    case 2:
                        {
                            dnxbCtl.SetPlayerDnxb(bankerSeat == playerSeat ? EnDnxbDir.Dong : EnDnxbDir.Xi);
                            break;
                        }

                    case 3:
                        {
                            break;
                        }
                       
                    case 4:
                        {
                            break;
                        }
                      
                }

                if (UtilData.CurrGamePalyerCnt == 2)
                {

                }
                else
                {
                    if (Chair == UtilData.PlayerSeat)
                    {
                        DnxbCtlGob.SetPlayerDnxb(EnDnxbDir.Dong);
                    }
                    else
                    {

                        DnxbCtlGob.SetPlayerDnxb((EnDnxbDir)UtilData.PlayerSeat);
                    }

                    //DnxbCtlGob.SetPlayerDnxb((EnDnxbDir)UtilData.PlayerSeat);
                }*/
            }
        }
    }
}
