import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { HotToastService } from '@ngxpert/hot-toast';
import { catchError } from 'rxjs';
import { AccountService } from '../services/account.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const toast = inject(HotToastService);
  const accountService = inject(AccountService);

  // To do things after next, use pipe
  return next(req).pipe(
    catchError((error) => {
      if (error) {
        switch (error.status) {
          case 400:
            // We are intentionally only displaying first out of possible multiple errors here
            if (error.error.errors) {
              const errorMap = error.error.errors;
              const firstKey = Object.keys(errorMap)[0];
              const firstError = errorMap[firstKey][0];
              toast.error(firstError);
              throw firstError;
            }
            // Happens for returned error strings via ServiceResult from the backend
            if (error.error) {
              toast.error(error.error);
              throw error.error;
            }
            break;

          case 401:
            if (error.error) {
              toast.error(error.error);
              throw error.error;
            }
            toast.error('Unauthorized');
            break;

          case 404:
            router.navigateByUrl('/not-found');
            break;

          case 500:
            if (accountService.roles().includes('Admin')) {
              const navigationExtras: NavigationExtras = {
                state: { error: error.error },
              };
              router.navigateByUrl('/admin/server-error', navigationExtras);
            } else {
              toast.error('Something went wrong');
            }
            break;

          default:
            toast.error('Something went wrong');
            break;
        }
      }
      throw error;
    }),
  );
};
