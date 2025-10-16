import { Component } from '@angular/core';
import { UserManagerComponent } from "../user-manager/user-manager.component";

@Component({
  selector: 'app-admin-panel',
  imports: [UserManagerComponent],
  templateUrl: './admin-panel.component.html',
  styleUrl: './admin-panel.component.scss'
})
export class AdminPanelComponent {

}
