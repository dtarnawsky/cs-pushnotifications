import { Component, OnInit } from '@angular/core';
import { NotificationsService } from '../notification.service';

@Component({
  selector: 'app-home',
  templateUrl: 'home.page.html',
  styleUrls: ['home.page.scss'],
})
export class HomePage implements OnInit {
  result: string;

  constructor(private notificationService: NotificationsService) { }

  async ngOnInit() {
    this.result = 'Registering push notifications...';
    const status = await this.notificationService.register();
    this.result = 'Push Notifications: ' + status;
  }

}
