//
//  Payment.h
//  Unity-iPhone
//
//  Created by AimTi on 16/3/10.
//
//

#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>

@interface Payment : NSObject

void appPayment(const char* id);
void closeTransaction(int index,int indexArray);
void InitPayment(const char* gameObject,const char* method);

@end


