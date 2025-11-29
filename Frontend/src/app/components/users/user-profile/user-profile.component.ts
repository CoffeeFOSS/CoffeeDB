import { Component, inject, OnInit } from '@angular/core';
import { LoadingService } from '../../../services/loading.service';
import { UserFrameService } from '../../../services/user-frame.service';
import { ActivatedRoute } from '@angular/router';
import { UsersService } from '../../../services/users.service';
import { Member } from '../../../models/member';

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [],
  templateUrl: './user-profile.component.html',
  styleUrl: './user-profile.component.scss',
})
export class UserProfileComponent {
  userFrameService = inject(UserFrameService);
  loadingService = inject(LoadingService);
}
