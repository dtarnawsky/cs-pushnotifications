import { Component, OnInit } from '@angular/core';
import { NotificationsService } from '../notification.service';

let that: any;

@Component({
  selector: 'app-home',
  templateUrl: 'home.page.html',
  styleUrls: ['home.page.scss'],
})
export class HomePage implements OnInit {
  result: string;
  logs: string[] = [];

  constructor(private notificationService: NotificationsService) { }

  async ngOnInit() {
    that = this;
    this.result = 'Registering push notifications...';
    const status = await this.notificationService.register(this.onLog);
    this.result = 'Push Notifications: ' + status;
  }

  onLog(message: string) {
    that.logs.push(message);
  }

}
