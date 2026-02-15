import { Component, OnInit, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DecimalPipe, DatePipe } from '@angular/common';
import { AdminService } from '../../core/services/admin.service';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';
import { PaginationComponent } from '../../shared/components/pagination.component';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner.component';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';
import { ConfirmModalComponent } from '../../shared/components/confirm-modal.component';

// ═══════════════════════════════════════════════════════════
// INTERFACES ADAPTÉES À L'API RÉELLE
// ═══════════════════════════════════════════════════════════

interface RestaurantDto {
  id: string;
  name: string;
  description: string | null;
  logoUrl: string | null;
  coverImageUrl: string | null;
  cuisineType: number;
  priceRange: number;
  rating: number;
  reviewCount: number;
  minOrderAmount: number;
  deliveryFee: number;
  estimatedDeliveryMinutes: number;
  distanceKm: number;
  isOpen: boolean;
  isActive: boolean;
  hasPromotion: boolean;
  street: string;
  city: string;
  postalCode: string;
  latitude: number;
  longitude: number;
  phone: string;
  // Champs optionnels admin
  ownerName?: string;
  totalOrders?: number;
  totalRevenue?: number;
  commissionRate?: number;
  isVerified?: boolean;
  createdAt?: string;
}

interface CreateRestaurantRequest {
  name: string;
  description?: string;
  cuisineType: number;
  priceRange: number;
  minOrderAmount: number;
  deliveryFee: number;
  estimatedDeliveryMinutes: number;
  maxDeliveryDistanceKm: number;
  street: string;
  city: string;
  postalCode: string;
  latitude: number;
  longitude: number;
  phone: string;
  coverImageUrl: string;
  logoUrl: string;
}

@Component({
  selector: 'app-restaurants',
  standalone: true,
  imports: [
    FormsModule, 
    DecimalPipe, 
    DatePipe, 
    PaginationComponent, 
    LoadingSpinnerComponent, 
    EmptyStateComponent, 
    ConfirmModalComponent
  ],
  templateUrl: './restaurants.component.html',
  styleUrls: ['./restaurants.component.scss']
})
export class RestaurantsComponent implements OnInit {
  // ═══════════════════════════════════════════════════════════
  // SIGNALS & STATE
  // ═══════════════════════════════════════════════════════════
  
  restaurants = signal<RestaurantDto[]>([]);
  loading = signal(true);
  page = signal(1);
  totalPages = signal(1);
  totalCount = signal(0);

  // Filtres
  search = '';
  cuisineTypeFilter: number | null = null;
  cityFilter = '';
  statusFilter = '';
  viewMode: 'list' | 'card' = 'list';
  private searchTimeout: any;

  // Modals
  showDeleteModal = signal(false);
  showEditModal = signal(false);
  showAddModal = signal(false);
  showSuspendModal = signal(false);
  showActivateModal = signal(false);

  selectedRestaurant = signal<RestaurantDto | null>(null);

  // Formulaire (adapté à l'API) - AVEC IMAGES
  restaurantForm: CreateRestaurantRequest = {
    name: '',
    description: '',
    cuisineType: 0,           // Marocain par défaut
    priceRange: 2,            // €€ par défaut
    minOrderAmount: 50,
    deliveryFee: 15,
    estimatedDeliveryMinutes: 30,
    maxDeliveryDistanceKm: 10,
    street: '',
    city: 'Casablanca',
    postalCode: '',
    latitude: 0,
    longitude: 0,
    phone: '',
    coverImageUrl: '',        // ✅ AJOUTÉ
    logoUrl: ''               // ✅ AJOUTÉ
  };

  // ═══════════════════════════════════════════════════════════
  // DONNÉES DE RÉFÉRENCE
  // ═══════════════════════════════════════════════════════════

  // Types de cuisine (adaptés à votre enum backend)
  cuisineTypes = [
    { value: 0, label: 'Marocain', icon: '🇲🇦' },
    { value: 1, label: 'Italien', icon: '🇮🇹' },
    { value: 2, label: 'Asiatique', icon: '🍜' },
    { value: 3, label: 'Burger', icon: '🍔' },
    { value: 4, label: 'Pizza', icon: '🍕' },
    { value: 5, label: 'Sushi', icon: '🍣' },
    { value: 6, label: 'Indien', icon: '🇮🇳' },
    { value: 7, label: 'Mexicain', icon: '🇲🇽' },
    { value: 8, label: 'Français', icon: '🇫🇷' }
  ];

  // Villes principales du Maroc
  cities = [
    'Casablanca', 'Rabat', 'Marrakech', 'Fès', 'Tanger', 
    'Agadir', 'Meknès', 'Oujda', 'Kenitra', 'Tétouan'
  ];

  // Gammes de prix
  priceRanges = [
    { value: 1, label: '€ - Économique', symbol: '€' },
    { value: 2, label: '€€ - Moyen', symbol: '€€' },
    { value: 3, label: '€€€ - Premium', symbol: '€€€' }
  ];

  // ═══════════════════════════════════════════════════════════
  // COMPUTED PROPERTIES
  // ═══════════════════════════════════════════════════════════

  activeCount = computed(() => 
    this.restaurants().filter(r => r.isActive).length
  );

  openCount = computed(() => 
    this.restaurants().filter(r => r.isOpen).length
  );

  avgRating = computed(() => {
    const rated = this.restaurants().filter(r => r.rating > 0);
    return rated.length 
      ? (rated.reduce((s, r) => s + r.rating, 0) / rated.length).toFixed(1) 
      : '0.0';
  });

  constructor(
    private admin: AdminService, 
    private api: ApiService, 
    private toast: ToastService
  ) {}

  ngOnInit(): void {
    this.loadRestaurants();
  }

  // ═══════════════════════════════════════════════════════════
  // RECHERCHE & FILTRES
  // ═══════════════════════════════════════════════════════════

  onSearch(): void {
    clearTimeout(this.searchTimeout);
    this.searchTimeout = setTimeout(() => {
      this.page.set(1);
      this.loadRestaurants();
    }, 400);
  }

  resetFilters(): void {
    this.search = '';
    this.cuisineTypeFilter = null;
    this.cityFilter = '';
    this.statusFilter = '';
    this.page.set(1);
    this.loadRestaurants();
  }

  hasActiveFilters(): boolean {
    return this.search !== '' || 
           this.cuisineTypeFilter !== null || 
           this.cityFilter !== '' ||
           this.statusFilter !== '';
  }

  // ═══════════════════════════════════════════════════════════
  // CHARGEMENT DES DONNÉES (READ)
  // ═══════════════════════════════════════════════════════════

  loadRestaurants(): void {
    this.loading.set(true);

    const params: any = {
      page: this.page(),
      pageSize: 20
    };

    // Ajout conditionnel des filtres
    if (this.search) params.search = this.search;
    if (this.cuisineTypeFilter !== null) params.cuisineType = this.cuisineTypeFilter;
    if (this.cityFilter) params.city = this.cityFilter;
    if (this.statusFilter === 'active') params.isActive = true;
    if (this.statusFilter === 'inactive') params.isActive = false;

    this.api.getPaginated<RestaurantDto>('Restaurants', params)
      .subscribe({
        next: res => {
          if (res.success) {
            console.log('res.data.items');
                        console.log(res.data.items);

            this.restaurants.set(res.data.items);
            this.totalPages.set(res.data.totalPages);
            this.totalCount.set(res.data.totalCount);
          }
          this.loading.set(false);
        },
        error: (err) => {
          console.error('Erreur chargement restaurants:', err);
          this.toast.error('Erreur lors du chargement');
          this.loading.set(false);
        }
      });
  }

  goToPage(p: number): void {
    this.page.set(p);
    this.loadRestaurants();
  }

  // ═══════════════════════════════════════════════════════════
  // CRÉATION (CREATE)
  // ═══════════════════════════════════════════════════════════

  openAddModal(): void {
    // Réinitialiser le formulaire
    this.restaurantForm = {
      name: '',
      description: '',
      cuisineType: 0,
      priceRange: 2,
      minOrderAmount: 50,
      deliveryFee: 15,
      estimatedDeliveryMinutes: 30,
      maxDeliveryDistanceKm: 10,
      street: '',
      city: 'Casablanca',
      postalCode: '',
      latitude: 33.5731,
      longitude: -7.5898,
      phone: '',
      coverImageUrl: '',      // ✅ AJOUTÉ
      logoUrl: ''             // ✅ AJOUTÉ
    };
    this.showAddModal.set(true);
  }

  createRestaurant(): void {
    // Validation
    if (!this.restaurantForm.name || !this.restaurantForm.street) {
      this.toast.warning('Veuillez remplir tous les champs obligatoires');
      return;
    }

    if (!this.restaurantForm.phone) {
      this.toast.warning('Le numéro de téléphone est obligatoire');
      return;
    }

    this.api.post('Restaurants', this.restaurantForm).subscribe({
      next: (res) => {
        if (res.success) {
          this.toast.success('Restaurant créé avec succès');
          this.showAddModal.set(false);
          this.loadRestaurants();
        }
      },
      error: (err) => {
        console.error('Erreur création:', err);
        this.toast.error('Erreur lors de la création');
      }
    });
  }

  // ═══════════════════════════════════════════════════════════
  // MODIFICATION (UPDATE)
  // ═══════════════════════════════════════════════════════════

  openEditModal(restaurant: RestaurantDto): void {
    this.selectedRestaurant.set(restaurant);
    console.log(restaurant);
    // Remplir le formulaire avec les données existantes
    this.restaurantForm = {
      name: restaurant.name,
      description: restaurant.description || '',
      cuisineType: restaurant.cuisineType,
      priceRange: restaurant.priceRange,
      minOrderAmount: restaurant.minOrderAmount,
      deliveryFee: restaurant.deliveryFee,
      estimatedDeliveryMinutes: restaurant.estimatedDeliveryMinutes,
      maxDeliveryDistanceKm: 10, // Valeur par défaut si non disponible
      street: restaurant.street,
      city: restaurant.city,
      postalCode: restaurant.postalCode,
      latitude: restaurant.latitude,
      longitude: restaurant.longitude,
      phone: restaurant.phone,
      coverImageUrl: restaurant.coverImageUrl || '',  // ✅ AJOUTÉ
      logoUrl: restaurant.logoUrl || ''                // ✅ AJOUTÉ
    };
    
    this.showEditModal.set(true);
  }

  updateRestaurant(): void {
    const restaurant = this.selectedRestaurant();
    if (!restaurant) return;

    // Validation
    if (!this.restaurantForm.name || !this.restaurantForm.street) {
      this.toast.warning('Veuillez remplir tous les champs obligatoires');
      return;
    }

    this.api.put(`Restaurants/${restaurant.id}`, this.restaurantForm)
      .subscribe({
        next: (res) => {
          if (res.success) {
            this.toast.success('Restaurant mis à jour');
            this.showEditModal.set(false);
            this.loadRestaurants();
          }
        },
        error: (err) => {
          console.error('Erreur modification:', err);
          this.toast.error('Erreur lors de la mise à jour');
        }
      });
  }

  // ═══════════════════════════════════════════════════════════
  // ACTIVATION / DÉSACTIVATION
  // ═══════════════════════════════════════════════════════════

  prepareSuspend(restaurant: RestaurantDto): void {
    this.selectedRestaurant.set(restaurant);
    this.showSuspendModal.set(true);
  }

  suspendRestaurant(): void {
    const restaurant = this.selectedRestaurant();
    if (!restaurant) return;

    // Utiliser l'endpoint de suspension si disponible
    // Sinon, utiliser PUT pour changer isActive
    this.api.put(`Restaurants/${restaurant.id}`, {
      ...restaurant,
      isActive: false
    }).subscribe({
      next: () => {
        this.toast.success('Restaurant suspendu');
        this.showSuspendModal.set(false);
        this.loadRestaurants();
      },
      error: (err) => {
        console.error('Erreur suspension:', err);
        this.toast.error('Erreur lors de la suspension');
      }
    });
  }

  prepareActivate(restaurant: RestaurantDto): void {
    this.selectedRestaurant.set(restaurant);
    this.showActivateModal.set(true);
  }

  activateRestaurant(): void {
    const restaurant = this.selectedRestaurant();
    if (!restaurant) return;

    this.api.put(`Restaurants/${restaurant.id}`, {
      ...restaurant,
      isActive: true
    }).subscribe({
      next: () => {
        this.toast.success('Restaurant activé');
        this.showActivateModal.set(false);
        this.loadRestaurants();
      },
      error: (err) => {
        console.error('Erreur activation:', err);
        this.toast.error('Erreur lors de l\'activation');
      }
    });
  }

  // ═══════════════════════════════════════════════════════════
  // SUPPRESSION (DELETE)
  // ═══════════════════════════════════════════════════════════

  prepareDelete(restaurant: RestaurantDto): void {
    this.selectedRestaurant.set(restaurant);
    this.showDeleteModal.set(true);
  }

  deleteRestaurant(): void {
    const restaurant = this.selectedRestaurant();
    if (!restaurant) return;

    this.api.delete(`Restaurants/${restaurant.id}`).subscribe({
      next: () => {
        this.toast.success('Restaurant supprimé');
        this.showDeleteModal.set(false);
        this.loadRestaurants();
      },
      error: (err) => {
        console.error('Erreur suppression:', err);
        this.toast.error('Erreur lors de la suppression');
      }
    });
  }

  // ═══════════════════════════════════════════════════════════
  // HELPERS
  // ═══════════════════════════════════════════════════════════

  getCuisineLabel(type: number): string {
    const cuisine = this.cuisineTypes.find(c => c.value === type);
    return cuisine ? cuisine.label : 'Autre';
  }

  getCuisineIcon(type: number): string {
    const cuisine = this.cuisineTypes.find(c => c.value === type);
    return cuisine ? cuisine.icon : '🍽️';
  }

  getPriceRangeSymbol(range: number): string {
    const price = this.priceRanges.find(p => p.value === range);
    return price ? price.symbol : '€';
  }

  getPriceRangeLabel(range: number): string {
    const price = this.priceRanges.find(p => p.value === range);
    return price ? price.label : 'Moyen';
  }

  // ═══════════════════════════════════════════════════════════
  // EXPORT
  // ═══════════════════════════════════════════════════════════

  exportRestaurants(): void {
    this.toast.info('Export en cours de génération...');
    
    // Si vous avez un endpoint d'export
    // this.api.get('Restaurants/export', { format: 'xlsx' })
    //   .subscribe({
    //     next: () => this.toast.success('Export téléchargé'),
    //     error: () => this.toast.error('Erreur lors de l\'export')
    //   });
  }
}