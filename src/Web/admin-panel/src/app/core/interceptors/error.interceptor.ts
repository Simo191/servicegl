import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { ToastService } from '../services/toast.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const toast = inject(ToastService);

  return next(req).pipe(
    catchError(error => {
      if (error.status === 401) {
        localStorage.clear();
        router.navigate(['/auth']);
      } else if (error.status === 403) {
        toast.error('Accès refusé');
      } else if (error.status === 500) {
        toast.error('Erreur serveur. Veuillez réessayer.');
      } else if (error.status === 0) {
        toast.error('Connexion impossible au serveur');
      }
      return throwError(() => error);
    })
  );
};
