import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { HotToastService } from '@ngxpert/hot-toast';
import { catchError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const toast = inject(HotToastService);

  // To do things after next, use pipe
  return next(req).pipe(
    catchError((error) => {
      if (error) {
        switch (error.status) {
          case 400:
            if (error.error.errors) {
              const errorMap = error.error.errors;
              const res = [];
              for (const key in errorMap) {
                res.push(errorMap[key]);
              }
              toast.error('Bad Request');
              throw res.join(' | ');
            }
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
            const navigationExtras: NavigationExtras = {
              state: { error: error.error },
            };
            router.navigateByUrl('/server-error', navigationExtras);
            break;

          default:
            toast.error('Something unexpected went wrong');
            break;
        }
      }
      throw error;
    }),
  );
};
