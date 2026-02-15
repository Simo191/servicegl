// ══════════════════════════════════════════════════════
// API RESPONSE WRAPPERS — matches backend ApiResponse<T>
// ══════════════════════════════════════════════════════
export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message: string;
  errors: string[];
}

export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

// ══════════════════════════════════════════════════════
// DASHBOARD — matches DashboardDto
// ══════════════════════════════════════════════════════
export interface DashboardStats {
  totalOrders: number;
  totalOrdersToday: number;
  totalRevenue: number;
  revenueToday: number;
  activeClients: number;
  activeRestaurants: number;
  activeServiceProviders: number;
  activeGroceryStores: number;
  activeDeliverers: number;
  pendingApprovals: number;
  openTickets: number;
  revenueChart: RevenueChartData[];
  orderChart: OrderChartData[];
  restaurantStats: ModuleStats;
  serviceStats: ModuleStats;
  groceryStats: ModuleStats;
}

export interface RevenueChartData {
  date: string;
  restaurant: number;
  service: number;
  grocery: number;
}

export interface OrderChartData {
  date: string;
  restaurant: number;
  service: number;
  grocery: number;
}

export interface ModuleStats {
  totalOrders: number;
  revenue: number;
  commission: number;
  avgRating: number;
}

// ══════════════════════════════════════════════════════
// USERS — matches AdminUserListDto / UserListDto
// ══════════════════════════════════════════════════════
export interface AdminUserDto {
  id: string;
  fullName: string;
  email: string;
  phone: string | null;
  isActive: boolean;
  isVerified: boolean;
  roles: string[];
  createdAt: string;
  lastLoginAt: string | null;
  totalOrders: number;
}
export interface UserDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  photoUrl: string;
  role: string;
  isActive: boolean;
  emailVerified: boolean;
  phoneVerified: boolean;
  createdAt: string;
  lastLoginAt: string;
  totalOrders: number;
  totalSpent: number;
}
// ══════════════════════════════════════════════════════
// APPROVALS — matches ApprovalDto
// ══════════════════════════════════════════════════════
export interface ApprovalDto {
  id: string;
  entityType: string;
  name: string;
  ownerName: string | null;
  status: string;
  createdAt: string;
}

export interface ApproveEntityRequest {
  entityType: string;
  entityId: string;
  approved: boolean;
  rejectionReason?: string;
}

// ══════════════════════════════════════════════════════
// ORDERS — matches AdminOrderListDto
// ══════════════════════════════════════════════════════
export interface AdminOrderDto {
  id: string;
  orderNumber: string;
  orderType: string;
  customerName: string;
  providerName: string;
  delivererName: string | null;
  status: string;
  totalAmount: number;
  commissionAmount: number;
  paymentStatus: string;
  createdAt: string;
}

// ═══════════════════════════════════════════════════════════
// MODÈLES ADAPTÉS À L'API RÉELLE - RESTAURANTS
// ═══════════════════════════════════════════════════════════

 // ═══════════════════════════════════════════════════════════
// MODÈLES API RESTAURANTS - VERSION FINALE
// Inclut coverImageUrl et logoUrl
// ═══════════════════════════════════════════════════════════

/**
 * DTO complet retourné par GET /api/v1/Restaurants
 */
export interface RestaurantDto {
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
}

/**
 * Request body pour POST /api/v1/Restaurants (création)
 * INCLUT coverImageUrl et logoUrl
 */
export interface CreateRestaurantRequest {
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
  coverImageUrl?: string;    // ✅ AJOUTÉ
  logoUrl?: string;           // ✅ AJOUTÉ
}

/**
 * Request body pour PUT /api/v1/Restaurants/{id} (modification)
 * INCLUT coverImageUrl et logoUrl
 */
export interface UpdateRestaurantRequest {
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
  coverImageUrl?: string;    // ✅ AJOUTÉ
  logoUrl?: string;           // ✅ AJOUTÉ
}

/**
 * Mapping des types de cuisine
 */
export enum CuisineType {
  Marocain = 0,
  Italien = 1,
  Asiatique = 2,
  Burger = 3,
  Pizza = 4,
  Sushi = 5,
  Indien = 6,
  Mexicain = 7,
  Français = 8
}

/**
 * Mapping des gammes de prix
 */
export enum PriceRange {
  Economy = 1,
  Medium = 2,
  Premium = 3
}

/**
 * Helper pour obtenir le label d'une cuisine
 */
export const getCuisineLabel = (type: number): string => {
  const labels: Record<number, string> = {
    0: 'Marocain',
    1: 'Italien',
    2: 'Asiatique',
    3: 'Burger',
    4: 'Pizza',
    5: 'Sushi',
    6: 'Indien',
    7: 'Mexicain',
    8: 'Français'
  };
  return labels[type] || 'Autre';
};

/**
 * Helper pour obtenir le symbole de prix
 */
export const getPriceRangeSymbol = (range: number): string => {
  const symbols: Record<number, string> = {
    1: '€',
    2: '€€',
    3: '€€€'
  };
  return symbols[range] || '€';
};

// ══════════════════════════════════════════════════════
// SERVICE PROVIDER — matches ServiceProviderListDto
// ══════════════════════════════════════════════════════
export interface ServiceProviderDto {
  id: string;
  companyName: string;
  ownerName: string;
  description: string | null;
  logoUrl: string | null;
  rating: number;
  reviewCount: number;
  yearsOfExperience: number;
  isAvailable: boolean;
  isActive: boolean;
  isVerified: boolean;
  category: string;
  serviceCategories: string[];
  startingPrice: number | null;
  serviceZones: string[];
  city: string;
  phone: string;
  email: string;
  totalInterventions: number;
  totalRevenue: number;
  commissionRate: number;
  createdAt: string;
}

// ══════════════════════════════════════════════════════
// GROCERY STORE — matches StoreListDto / StoreDetailDto
// ══════════════════════════════════════════════════════
export interface GroceryStoreDto {
  id: string;
  name: string;
  brand: string;
  logoUrl: string | null;
  rating: number;
  reviewCount: number;
  minOrderAmount: number;
  deliveryFee: number;
  freeDeliveryThreshold: number;
  distanceKm: number;
  isOpen: boolean;
  isActive: boolean;
  hasPromotion: boolean;
  address: string;
  city: string;
  phone: string;
  totalProducts: number;
  totalOrders: number;
  totalRevenue: number;
  commissionRate: number;
  createdAt: string;
}

// ══════════════════════════════════════════════════════
// DELIVERY DRIVER — matches DeliveryDriverDto
// ══════════════════════════════════════════════════════
export interface DelivererDto {
  id: string;
  name: string;
  firstName: string;
  lastName: string;
  phone: string;
  email: string;
  photoUrl: string | null;
  vehicleType: string | null;
  isActive: boolean;
  isOnline: boolean;
  isAvailable: boolean;
  isVerified: boolean;
  rating: number;
  totalDeliveries: number;
  totalEarnings: number;
  latitude: number;
  longitude: number;
  createdAt: string;
}

// ══════════════════════════════════════════════════════
// FINANCE — matches AdminFinanceDto / FinancialReportDto
// ══════════════════════════════════════════════════════
export interface AdminFinanceDto {
  totalRevenue: number;
  totalCommissions: number;
  totalPayoutsRestaurants: number;
  totalPayoutsServices: number;
  totalPayoutsGroceries: number;
  totalPayoutsDeliverers: number;
  totalRefunds: number;
  monthlyData: FinanceChartData[];
}

export interface FinancialReportDto {
  totalRevenue: number;
  restaurantRevenue: number;
  groceryRevenue: number;
  serviceRevenue: number;
  totalCommissions: number;
  totalRefunds: number;
  totalOrders: number;
  totalPaidOrders: number;
}

export interface FinanceChartData {
  month: string;
  revenue: number;
  commissions: number;
  payouts: number;
}

export interface TransactionDto {
  id: string;
  type: string;
  description: string;
  amount: number;
  status: string;
  date: string;
  reference: string;
}

// ══════════════════════════════════════════════════════
// PROMOTIONS — matches CreatePromotionDto
// ══════════════════════════════════════════════════════
export interface CreatePromotionRequest {
  code: string;
  title: string;
  description?: string;
  discountType: string;
  discountValue: number;
  minOrderAmount?: number;
  maxDiscount?: number;
  startDate: string;
  endDate: string;
  maxUsages?: number;
  applicableModule: string;
  freeDelivery: boolean;
}

export interface PromoCodeDto {
  id: string;
  code: string;
  title: string;
  discountType: string;
  discountValue: number;
  minOrderAmount: number;
  maxDiscount: number;
  maxUsages: number;
  usedCount: number;
  startDate: string;
  endDate: string;
  isActive: boolean;
  applicableModule: string;
  freeDelivery: boolean;
}

// ══════════════════════════════════════════════════════
// COMMISSIONS — matches CommissionSettingsDto
// ══════════════════════════════════════════════════════
export interface CommissionSettingsDto {
  entityType: string;
  entityId: string;
  newRate: number;
}

// ══════════════════════════════════════════════════════
// SYSTEM LOGS
// ══════════════════════════════════════════════════════
export interface SystemLogDto {
  id: string;
  level: string;
  message: string;
  source: string;
  timestamp: string;
  details: string | null;
}

// ══════════════════════════════════════════════════════
// SHARED
// ══════════════════════════════════════════════════════
export interface ChartData {
  label: string;
  value: number;
}

export interface RecentOrder {
  id: string;
  orderNumber: string;
  type: 'Restaurant' | 'Service' | 'Grocery';
  customerName: string;
  providerName: string;
  amount: number;
  status: string;
  createdAt: string;
}
