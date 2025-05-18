//
//  IAPManager.h
//  Unity-iPhone
//
//  Created by AimTi on 16/3/11.
//
//

#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>

@interface IAPManager : NSObject

-(void)appPayment:(NSString *)id;
-(void)getProductInfo:(NSString *)id;
//-(void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response;
-(void)buyRequest:(NSString *)id;
-(void)provingCloseTransaction:(int)index ArrayIndex:(int)arrayIndex;
-(void)InitPayment:(NSString *) gameObject second: (NSString *) method;

@end
