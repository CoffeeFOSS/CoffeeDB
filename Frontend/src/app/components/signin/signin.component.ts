import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountService } from '../../services/account.service';

@Component({
  selector: 'app-signin',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './signin.component.html',
  styleUrl: './signin.component.scss',
})
export class SignInComponent {
  private accountService = inject(AccountService);

  model = {
    username: '',
    password: '',
  };

  redirectUrl: string = '/';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
  ) {
    this.redirectUrl = this.route.snapshot.queryParams['redirectUrl'] || '/';
  }

  onSignIn() {
    this.accountService.signIn(this.model).subscribe({
      next: () => {
        this.router.navigateByUrl(this.redirectUrl);
      },
      error: (error) => {
        console.error(error);
      },
    });
  }

  onNavigateRegister() {
    this.router.navigate(['/register'], {
      queryParams: { redirectUrl: this.redirectUrl },
    });
  }
}
