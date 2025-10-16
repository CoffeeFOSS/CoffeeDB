import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AccountService } from '../services/account.service';
import { HotToastService } from '@ngxpert/hot-toast';

export const adminGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const toastr = inject(HotToastService);

  if (accountService.roles().includes('Admin')) {
    return true;
  }
  toastr.error('You cannot access this page.');
  return false;
};
