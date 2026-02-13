import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const t = inject(AuthService).getToken();
  return next(t ? req.clone({ setHeaders: { Authorization: `Bearer ${t}` } }) : req);
};
