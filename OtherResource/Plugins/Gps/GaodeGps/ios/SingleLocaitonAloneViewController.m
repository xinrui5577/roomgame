//
//  SingleLocaitonAloneViewController.m
//  officialDemoLoc
//
//  Created by 郑德发 on 17/4/1.
//  Copyright © 2017年 AutoNavi. All rights reserved.
//
#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "AMapFoundationKit.h"
#import "AMapLocationKit.h"

#define DefaultLocationTimeout 10
#define DefaultReGeocodeTimeout 5

@interface SingleLocaitonAloneViewController : NSObject

@property (nonatomic, copy) AMapLocatingCompletionBlock completionBlock;

@property (nonatomic, strong) UILabel *displayLabel;

@property (nonatomic, strong) AMapLocationManager *locationManager;


//@end

//@implementation AMapLocationKit
@end
@implementation SingleLocaitonAloneViewController

#pragma mark - Action Handle

float latpos;
float longpos;
NSString *locationDespos=@"请重新获取地址";

- (void)configLocationManager
{
    [AMapServices sharedServices].apiKey =@"YX_BUILD:REPLACE{0}";
    self.locationManager = [[AMapLocationManager alloc] init];
    
    //[self.locationManager setDelegate:self];
    
    //设置期望定位精度
    [self.locationManager setDesiredAccuracy:kCLLocationAccuracyHundredMeters];
    
    //设置不允许系统暂停定位
    [self.locationManager setPausesLocationUpdatesAutomatically:NO];
    
    //设置允许在后台定位
    //[self.locationManager setAllowsBackgroundLocationUpdates:YES];
    
    //设置定位超时时间
    [self.locationManager setLocationTimeout:DefaultLocationTimeout];
    
    //设置逆地理超时时间
    [self.locationManager setReGeocodeTimeout:DefaultReGeocodeTimeout];
}

- (void)cleanUpAction
{
    //停止定位
    [self.locationManager stopUpdatingLocation];
    
    [self.locationManager setDelegate:nil];
    
    [self.displayLabel setText:nil];
}

- (void)reGeocodeAction
{
    //进行单次带逆地理定位请求
    [self.locationManager requestLocationWithReGeocode:YES completionBlock:self.completionBlock];
}

- (void)locAction
{
    //进行单次定位请求
    [self.locationManager requestLocationWithReGeocode:NO completionBlock:self.completionBlock];
}
- (void)initCompleteBlock
{
    [self.locationManager requestLocationWithReGeocode:YES completionBlock:^(CLLocation *location, AMapLocationReGeocode *regeocode, NSError *error) {
        
        if (error)
        {
            NSLog(@"locError:{%ld - %@};", (long)error.code, error.localizedDescription);
            
            if (error.code == AMapLocationErrorLocateFailed)
            {
                return;
            }
        }
        NSLog(@"location对象:%@", location);
        //NSLog(@"location:%f%f", location.verticalAccuracy,location.horizontalAccuracy);
        //NSLog(@"location---纬度纬度:%f", location.coordinate.latitude);
        //NSLog(@"location---经度经度:%f", location.coordinate.longitude);
        latpos = location.coordinate.latitude;
        longpos = location.coordinate.longitude;
        locationDespos = [NSString stringWithFormat:@"%@", regeocode.formattedAddress];
        //locationDespos = [NSString stringWithFormat:@"%@%@%@", regeocode.formattedAddress,regeocode.citycode, regeocode.adcode];
        //UnitySendMessage("Canvas", "LatPos", [[NSString stringWithFormat:@"%f",latpos]UTF8String]);
        //UnitySendMessage("Canvas", "LongPos", [[NSString stringWithFormat:@"%f",longpos]UTF8String]);
        
        if (regeocode)
        {
            NSLog(@"reGeocode:%@", regeocode);
        }
    }];
}
- (float)latV{
    return latpos;
}
- (float)longV{
    return longpos;
}
-(NSString *)locationDesposV
{
    return locationDespos;
}
@end
#if defined(__cplusplus)
extern "C"{
#endif
#import <Foundation/Foundation.h>
    
#import "SingleLocaitonAloneViewController.m"
    //字符串转化的工具函数
    
    NSString* _CreateNSString (const char* string)
    {
        if (string)
            return [NSString stringWithUTF8String: string];
        else
            return [NSString stringWithUTF8String: ""];
    }
    
    char* _MakeStringCopy( const char* string)
    {
        if (NULL == string) {
            return NULL;
        }
        char* res = (char*)malloc(strlen(string)+1);
        strcpy(res, string);
        return res;
    }
    
    //static MyIOSSdk *mySDK;
    
    static SingleLocaitonAloneViewController *AMapManager;
    //供u3d调用的c函数
    
    void _GaoDePlatformInit()
    {
        AMapManager = [[SingleLocaitonAloneViewController alloc] init];
        [AMapManager configLocationManager];
        //[AMapManager reGeocodeAction];
        //[AMapManager initCompleteBlock];
    }
    void _GaoDePlatformStart()
    {
        [AMapManager initCompleteBlock];
    }
    void _GaoDePlatformEnd()
    {
        [AMapManager cleanUpAction];
    }
    const float _Latitude()
    {
        return [AMapManager latV];
    }
    const float _Longitude()
    {
        return [AMapManager longV];
    }
    void* _locationDespos(int *len)
    {
        NSString *retStr = locationDespos;
        //NSString *retStr = @"无论是给国外的朋友写2还是填写申请护照y签证等一系列表格都需要用英文书写居住地址办公地";
        //一个字节3字节一个英文2字节一个数字2字节
        int strlen =0;
        strlen =(int)retStr.length*4+1;
        *len = strlen;
        NSLog(@"%s%@","aaaaaaaaa", locationDespos);
        char *nameBuffer = malloc(strlen);
        
        NSLog(@"%s%d%s%lu","QWE",strlen,"rty", retStr.length);
        strcpy(nameBuffer,retStr.UTF8String);
        for (int i=0; i<strlen; i++) {
            if(nameBuffer[i]==0){
                *len = i+1;
                NSLog(@"%s%d","len",i);
                break;
            }
        }
        //memcpy(nameBuffer, retStr.UTF8String, sizeof(char) * (retStr.length + 1)*4);
        NSLog(@"%s%s","bbbbbbbb", nameBuffer);
        NSLog(@"%s%s","retStr.UTF8String", retStr.UTF8String);
        return nameBuffer;
    }
#if defined(__cplusplus)
}
#endif
