#import "PhoneState.h"
@implementation PhoneState

float getiOSBatteryLevel()
{
　[[UIDevice currentDevice] setBatteryMonitoringEnabled:YES];
　return [[UIDevice currentDevice] batteryLevel];
}

@end