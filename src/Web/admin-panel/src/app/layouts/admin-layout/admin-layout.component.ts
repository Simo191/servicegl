import { Component, signal, computed, OnInit, OnDestroy, HostListener } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive, Router, NavigationEnd } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { ToastContainerComponent } from '../../shared/components/toast-container.component';
import { filter, Subscription } from 'rxjs';

interface NavItem {
  label: string;
  icon: string;
  route: string;
  badge?: number;
}

interface NavSection {
  title: string;
  items: NavItem[];
}

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, ToastContainerComponent],
  templateUrl: './admin-layout.component.html',
  styleUrls: ['./admin-layout.component.scss']
})
export class AdminLayoutComponent implements OnInit, OnDestroy {
  sidebarCollapsed = signal(false);
  mobileOpen = signal(false);
  currentTime = signal('');
  notificationCount = signal(3);
  searchQuery = signal('');
  profileMenuOpen = signal(false);
  currentPageTitle = signal('Dashboard');

  private timerInterval: any;
  private routerSub!: Subscription;

  navSections: NavSection[] = [
    {
      title: 'Principal',
      items: [
        { label: 'Dashboard', icon: 'bi-grid-1x2-fill', route: '/dashboard' }
      ]
    },
    {
      title: 'Gestion',
      items: [
        { label: 'Utilisateurs', icon: 'bi-people-fill', route: '/users' },
        { label: 'Restaurants', icon: 'bi-shop', route: '/restaurants' },
        { label: 'Services', icon: 'bi-tools', route: '/services' },
        { label: 'Courses', icon: 'bi-cart3', route: '/grocery' },
        { label: 'Commandes', icon: 'bi-receipt-cutoff', route: '/orders', badge: 5 },
        { label: 'Livreurs', icon: 'bi-truck', route: '/delivery' }
      ]
    },
    {
      title: 'Finance & Marketing',
      items: [
        { label: 'Finances', icon: 'bi-wallet2', route: '/finance' },
        { label: 'Marketing', icon: 'bi-megaphone-fill', route: '/marketing' }
      ]
    },
    {
      title: 'Système',
      items: [
        { label: 'Configuration', icon: 'bi-gear-fill', route: '/settings' }
      ]
    }
  ];

  userInitials = computed(() => {
    const u = this.auth.user();
    return u ? (u.firstName[0] + u.lastName[0]).toUpperCase() : 'AD';
  });

  userFullName = computed(() => {
    const u = this.auth.user();
    return u ? `${u.firstName} ${u.lastName}` : 'Admin';
  });

  userRole = computed(() => {
    const u = this.auth.user();
    return u?.role ?? 'Administrateur';
  });

  constructor(public auth: AuthService, private router: Router) {}

  ngOnInit(): void {
    this.updateTime();
    this.timerInterval = setInterval(() => this.updateTime(), 60_000);

    this.routerSub = this.router.events
      .pipe(filter((e): e is NavigationEnd => e instanceof NavigationEnd))
      .subscribe(e => {
        const flat = this.navSections.flatMap(s => s.items);
        const match = flat.find(i => e.urlAfterRedirects.startsWith(i.route));
        this.currentPageTitle.set(match?.label ?? 'Dashboard');
        this.closeMobile();
      });
  }

  ngOnDestroy(): void {
    clearInterval(this.timerInterval);
    this.routerSub?.unsubscribe();
  }

  @HostListener('window:resize')
  onResize(): void {
    if (window.innerWidth >= 992) {
      this.mobileOpen.set(false);
    }
  }

  toggleSidebar(): void {
    if (window.innerWidth < 992) {
      this.mobileOpen.update(v => !v);
    } else {
      this.sidebarCollapsed.update(v => !v);
    }
  }

  closeMobile(): void {
    this.mobileOpen.set(false);
  }

  toggleProfileMenu(): void {
    this.profileMenuOpen.update(v => !v);
  }

  logout(): void {
    this.auth.logout();
  }

  private updateTime(): void {
    this.currentTime.set(
      new Date().toLocaleTimeString('fr-FR', { hour: '2-digit', minute: '2-digit' })
    );
  }
}