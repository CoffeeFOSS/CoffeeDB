import { inject } from '@angular/core';
import { CanActivateChildFn } from '@angular/router';
import { AccountService } from '../services/account.service';
import { HotToastService } from '@ngxpert/hot-toast';

export const moderatorGuard: CanActivateChildFn = (childRoute, state) => {
  const accountService = inject(AccountService);
  const toastr = inject(HotToastService);

  if (accountService.roles().includes('Mod')) {
    return true;
  }
  toastr.error('You cannot access this page.');
  return false;
};
