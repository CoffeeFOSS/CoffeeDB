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
  providers: [],
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
  loadingKey?: string;

  ngOnInit() {
    this.route.paramMap.subscribe((params) => {
      const usernameFromRoute = params.get('username');
      if (!usernameFromRoute) return;

      const currentUsername = this.userFrameService.username();
      if (currentUsername !== usernameFromRoute) {
        this.userFrameService.reset();
        this.userFrameService.username.set(usernameFromRoute);
        this.loadingKey = `user-${usernameFromRoute}`;
        this.loadingService.busy(this.loadingKey);

        this.usersService.getUser(usernameFromRoute).subscribe({
          next: (user: Member) => {
            this.userFrameService.user.set(user);
            this.loadingService.idle(this.loadingKey);
          },
        });
      }
    });
  }
}
