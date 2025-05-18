#import "YxLocationTool.h"

@implementation YxLocationTool
#define SYSTEM_VERSION_GREATER_THAN(v) ([[[UIDevice currentDevice] systemVersion] compare:v options:NSNumericSearch] == NSOrderedDescending)

//权限页面
void NsOpenLocationAuthorityView()
{
    NSURL *url = [NSURL URLWithString:UIApplicationOpenSettingsURLString];
    if ([[UIApplication sharedApplication] canOpenURL:url]) {
        [[UIApplication sharedApplication] openURL:url];
    }
}

//服务页面
void NsOpenLocationSettingView()
{
    if (SYSTEM_VERSION_GREATER_THAN(@"8.0")) {
		NSURL *url = [NSURL URLWithString:UIApplicationOpenSettingsURLString]; 
		if ([[UIApplication sharedApplication] canOpenURL:url]) { 
			[[UIApplication sharedApplication] openURL:url]; 
		}
	}
}

//检测位置服务是否开启
bool NsCheckLocationService()
{
    return [CLLocationManager locationServicesEnabled];
}

//检测是否有位置权限
bool NsCheckLocationPermissions()
{
    if ([ CLLocationManager authorizationStatus] == kCLAuthorizationStatusDenied) {
        return NO;
    } else{
        return YES;
	}
}
@end
