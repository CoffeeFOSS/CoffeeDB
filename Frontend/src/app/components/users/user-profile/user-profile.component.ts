import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { LoadingService } from '../../../services/loading.service';
import { UsersService } from '../../../services/users.service';
import { Member } from '../../../models/member';

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [],
  templateUrl: './user-profile.component.html',
  styleUrl: './user-profile.component.scss',
})
export class UserProfileComponent implements OnInit {
  private usersService = inject(UsersService);
  private route = inject(ActivatedRoute);
  loadingService = inject(LoadingService);
  user?: Member;

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    // After a user goes to a route, this snapshot is stored for this instance
    const username = this.route.snapshot.paramMap.get('username');
    if (!username) return;
    this.usersService.getUser(username).subscribe({
      next: (user: Member) => {
        this.user = user;
      },
    });
  }
}
