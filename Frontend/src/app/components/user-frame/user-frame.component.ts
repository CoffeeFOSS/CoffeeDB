import { Component, inject, OnInit } from '@angular/core';
import {
  ActivatedRoute,
  RouterLink,
  RouterLinkActive,
  RouterOutlet,
} from '@angular/router';
import { Member } from '../../models/member';
import { LoadingService } from '../../services/loading.service';
import { UsersService } from '../../services/users.service';
import { AccountService } from '../../services/account.service';
import { UserFrameService } from '../../services/user-frame.service';

@Component({
  selector: 'app-user-frame',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './user-frame.component.html',
  styleUrl: './user-frame.component.scss',
})
export class UserFrameComponent implements OnInit {
  private usersService = inject(UsersService);
  accountService = inject(AccountService);
  private route = inject(ActivatedRoute);
  loadingService = inject(LoadingService); // TODO: implement loading
  userFrameService = inject(UserFrameService);

  ngOnInit() {
    let username = this.userFrameService.username();
    if (!username) {
      const usernameFromRoute = this.route.snapshot.paramMap.get('username');
      if (!usernameFromRoute) return;
      this.userFrameService.username.set(usernameFromRoute);
      username = usernameFromRoute;
    }
    // Need this here to retrieve at least the ID of the user
    if (!this.userFrameService.user()) {
      this.usersService.getUser(username).subscribe({
        next: (user: Member) => {
          this.userFrameService.user.set(user);
        },
      });
    }
  }
}
