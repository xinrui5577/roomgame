#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

#endif

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.NativeGallery.Editor
{
    public class NgPostProcessBuild
    {
#if UNITY_IOS
        //[PostProcessBuild]
        //public static void OnPostprocessBuild( BuildTarget target, string buildPath )
        //{
        //    if( target == BuildTarget.iOS )
        //    {
                //string pbxProjectPath = PBXProject.GetPBXProjectPath(buildPath);
                //PBXProject pbxProject = new PBXProject();
                //pbxProject.ReadFromFile(pbxProjectPath);
                //string targetGuid = pbxProject.TargetGuidByName(PBXProject.GetUnityTargetName());
                //pbxProject.AddFrameworkToProject(targetGuid, "MobileCoreServices.framework", true);
                //pbxProject.AddFrameworkToProject(targetGuid, "ImageIO.framework", true);
                //pbxProject.AddFrameworkToProject(targetGuid, "Photos.framework", true);
                //pbxProject.AddFrameworkToProject(targetGuid, "AssetsLibrary.framework", true);
                //PlistDocument plist = new PlistDocument();
                //string plistPath= pbxProjectPath + "/Info.plist";
                //plist.ReadFromString(File.ReadAllText(plistPath));
                //PlistElementDict rootDict = plist.root;
                //rootDict.SetString("NSPhotoLibraryUsageDescription", "是否允许此游戏获得相册权限来进行玩家交流");
                //File.WriteAllText(pbxProjectPath, pbxProject.WriteToString());
        //    }
        //}
#endif
    }
}