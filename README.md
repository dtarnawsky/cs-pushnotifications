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
