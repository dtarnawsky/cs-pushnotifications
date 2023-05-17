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

  onLog: (msg: string) => void;
  constructor() { }

  public async register(logCallback: (msg: string) => void): Promise<string> {
    this.onLog = logCallback;
    PushNotifications.addListener('pushNotificationActionPerformed', (notification: ActionPerformed) => {
      console.log('pushNotificationActionPerformed', notification);
      this.onLog(`pushNotificationActionPerformed ${JSON.stringify(notification)}`);
    });
    PushNotifications.addListener('registration', (token: Token) => {
      console.log('Register ' + JSON.stringify(token));
      this.onLog(`registration ${JSON.stringify(token)}`);
    });
    PushNotifications.addListener('registrationError', (error: any) => {
      console.log('registrationError ', error);
      this.onLog(`registrationError ${error}`);
    });
    PushNotifications.addListener('pushNotificationReceived', (notification: PushNotificationSchema) => {
      console.log('pushNotificationReceived ', JSON.stringify(notification));
      this.onLog(`pushNotificationReceived ${JSON.stringify(notification)}`);
    });
    await PushNotifications.register();
    console.log('Push notifications registered');
    const status: PermissionStatus = await PushNotifications.requestPermissions();
    return (status.receive);
  }
}
