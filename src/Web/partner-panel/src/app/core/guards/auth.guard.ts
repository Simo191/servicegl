import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
export const authGuard: CanActivateFn = () => { const a = inject(AuthService); if (a.isAuthenticated()) return true; inject(Router).navigate(['/auth']); return false; };
