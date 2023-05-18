# Push Notifications
Using Azure Notification:
* Create a Azure Notification Hub resource in `portal.azure.com`.
* Choose `Apple (APNS)`, choose `Token` for Authentication mode using the guide [here](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-push-notification-http2-token-authentication)

## Capture a Device Token
When the application starts up it will request permissions for Push Notifications and all register. This returns a DeviceToken for iOS and an FCM Token for Android. This is logged on line 22 of `notification.service.ts`. Run the application and capture the token, it needs to be registered with Azure Notification Hub so that you can send test push notifications. 

This operation would normally be done with an API call to your backend. A sample of the registration process can be found in the folder `csharp-backend` which contains a .net console application that when compiled and run can register a token (see `program.cs` for more information).

## Send a test notification
* Click `Overview` in the side panel of the Azure portal
* Click `Test Send`
* Choose `Apple` from the platforms and click the `Send` button

If you have successfully registered a token with Azure Notification then it should show as a result that 1 notification has passed and you should see the push notification on your device.

Note: If you have built your app as a `debug` then your notification hub must be set to `sandbox`. If you build your app as `release` then your notification hub must be set to `production`. You should have 2 instances of notifications hubs to separate these modes.

## Images in Push Notifications
The Firebase SDK can send an image with a notification simply by adding a `ImageUrl` property that contains a url that begins with `https://` and is for an image that is less than 300kb.
Android will automatically handle images but for iOS a Notification Service Extension needs to be added.

### Images with iOS
In XCode:
- Click `File` > `New` > `Target`
- Choose `Notification Service Extension` and click `Next`
- Enter a `Product Name` (for example `pushextension`)
- Select the Team
- Click `Finish`
- When asked click `Activate`

Choose `pushextension` from the list of Targets. Then:
- Click `Signing & Capabilities`
- Click `+ Capability`
- Choose `Push Notifications`

Important: Change the Deployment target from `iOS 16.4` (or whatever Xcode chose) to `iOS 13.0`. This ensures you get images with a push notification on older versioned devices.

Open `Podfile` and add the following and save:
```pod
target 'pushextension' do
  pod 'Firebase/Messaging'
end
```

- Run `npx cap update ios` on the command line to update Cocoapods

Now open `NotificationService.swift` (it will be in the folder named `pushextension`)

Replace the file contents with the following:
```swift
import UserNotifications
import FirebaseMessaging

class NotificationService: UNNotificationServiceExtension {
    var contentHandler: ((UNNotificationContent) -> Void)?
    var bestAttemptContent: UNMutableNotificationContent?

    override func didReceive(_ request: UNNotificationRequest, withContentHandler contentHandler: @escaping (UNNotificationContent) -> Void) {
        print("didReceive called")
        guard let content = request.content.mutableCopy() as? UNMutableNotificationContent else { return }
        print("content")
        self.contentHandler = contentHandler
        self.bestAttemptContent = content
        
        FIRMessagingExtensionHelper().populateNotificationContent(content, withContentHandler: contentHandler)
        print("didReceive done")
    }
    
    override func serviceExtensionTimeWillExpire() {
        guard let contentHandler = contentHandler,
              let bestAttemptContent =  bestAttemptContent else { return }
        
        contentHandler(bestAttemptContent)
    }
}
```


