using UnityEngine;

namespace Assets.Scripts.Game.mdx
{
    public class MdxTools : MonoBehaviour {

        const string Small = "small";
        const string Big = "big";

        public static int GetP(string p)
        {
            switch (p)
            {
                case Small:
                    return 1;
                case Big:
                    return 0;
                default:
                    return -1;
            }
        }

        public static string GetSide(int p)
        {
            switch (p)
            {
                case 1:
                    return Small;
                case 0:
                    return Big;
                default:
                    return string.Empty;
            }
        }
    }
}
