import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account.service';

export const signedInGuard: CanActivateFn = (route, state) => {
  // Guard to check if user is simply logged in / authenticated.
  const accountService = inject(AccountService);
  const router = inject(Router);

  if (!accountService.currentUser()) {
    router.navigate(['/signin'], {
      queryParams: { redirectUrl: state.url },
    });
    return false;
  }
  return true;
};
