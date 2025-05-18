#import <CoreLocation/CoreLocation.h>
@interface YxLocationTool:NSObject
{ 
}
 
void NsOpenLocationAuthorityView();
void NsOpenLocationSettingView();
bool NsCheckLocationService();
bool NsCheckLocationPermissions();
@end
