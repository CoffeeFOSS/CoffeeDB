import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { LoadingService } from '../../../services/loading.service';
import { UsersService } from '../../../services/users.service';

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
  user?: any;

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    // After a user goes to a route, this snapshot is stored for this instance
    // TODO: need actual usernames param
    const id = this.route.snapshot.paramMap.get('username');
    if (!id) return;
    this.usersService.getUserById(id).subscribe({
      next: (user) => {
        this.user = user;
        console.log(user);
      },
    });
  }
}
