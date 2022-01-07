import { Injectable } from '@angular/core';
import {
  ActionPerformed,
  PermissionStatus,
  PushNotifications,
  PushNotificationSchema,
  Token
} from '@capacitor/push-notifications';

@Injectable({
  providedIn: 'root'
})
export class NotificationsService {

  constructor() { }

  public async register(): Promise<string> {
    PushNotifications.addListener('pushNotificationActionPerformed', (notification: ActionPerformed) => {
      console.log('pushNotificationActionPerformed', notification);
    });
    PushNotifications.addListener('registration', (token: Token) => {
      console.log('Register ' + JSON.stringify(token));
    });
    PushNotifications.addListener('registrationError', (error: any) => {
      console.log('registrationError ', error);
    });
    PushNotifications.addListener('pushNotificationReceived', (notification: PushNotificationSchema) => {
      console.log('pushNotificationReceived ', JSON.stringify(notification));
    });
    await PushNotifications.register();
    console.log('Push notifications registered');
    const status: PermissionStatus = await PushNotifications.requestPermissions();
    return (status.receive);
  }
}
