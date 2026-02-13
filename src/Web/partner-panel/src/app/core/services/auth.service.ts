import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '@env/environment';

interface PartnerUser { id: string; email: string; name: string; role: string; businessName: string; businessType: 'Restaurant' | 'ServiceProvider' | 'GroceryStore'; logoUrl: string; }
interface LoginResponse { accessToken: string; refreshToken: string; user: PartnerUser; }
interface ApiResp<T> { success: boolean; data: T; message: string; }

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;
  currentUser = signal<PartnerUser | null>(null);
  isAuthenticated = computed(() => !!this.currentUser());
  businessType = computed(() => this.currentUser()?.businessType || 'Restaurant');

  constructor(private http: HttpClient, private router: Router) {
    const u = localStorage.getItem('partner_user');
    if (u && localStorage.getItem('partner_token')) { try { this.currentUser.set(JSON.parse(u)); } catch { this.logout(); } }
  }

  login(email: string, password: string): Observable<ApiResp<LoginResponse>> {
    return this.http.post<ApiResp<LoginResponse>>(`${this.apiUrl}/partner-login`, { email, password })
      .pipe(tap(r => { if (r.success) { localStorage.setItem('partner_token', r.data.accessToken); localStorage.setItem('partner_refresh', r.data.refreshToken); localStorage.setItem('partner_user', JSON.stringify(r.data.user)); this.currentUser.set(r.data.user); } }));
  }

  logout(): void { localStorage.removeItem('partner_token'); localStorage.removeItem('partner_refresh'); localStorage.removeItem('partner_user'); this.currentUser.set(null); this.router.navigate(['/auth']); }
  getToken(): string | null { return localStorage.getItem('partner_token'); }
}
