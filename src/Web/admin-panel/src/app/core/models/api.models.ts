export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message: string;
  errors: string[];
}

export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface DashboardStats {
  totalRevenue: number;
  revenueChange: number;
  totalOrders: number;
  ordersChange: number;
  activeClients: number;
  clientsChange: number;
  activeProviders: number;
  providersChange: number;
  restaurantOrders: number;
  serviceInterventions: number;
  groceryOrders: number;
  activeDeliverers: number;
  revenueChart: ChartData[];
  ordersChart: ChartData[];
  topRestaurants: TopItem[];
  topProviders: TopItem[];
  recentOrders: RecentOrder[];
}

export interface ChartData {
  label: string;
  value: number;
}

export interface TopItem {
  id: string;
  name: string;
  orders: number;
  revenue: number;
  rating: number;
}

export interface RecentOrder {
  id: string;
  orderNumber: string;
  type: 'Restaurant' | 'Service' | 'Grocery';
  customerName: string;
  amount: number;
  status: string;
  createdAt: string;
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

export interface RestaurantDto {
  id: string;
  name: string;
  slug: string;
  description: string;
  ownerName: string;
  cuisineType: string;
  address: string;
  city: string;
  phone: string;
  email: string;
  logoUrl: string;
  coverUrl: string;
  isVerified: boolean;
  isActive: boolean;
  rating: number;
  totalOrders: number;
  totalRevenue: number;
  commissionRate: number;
  createdAt: string;
}

export interface ServiceProviderDto {
  id: string;
  companyName: string;
  ownerName: string;
  category: string;
  description: string;
  address: string;
  city: string;
  phone: string;
  email: string;
  logoUrl: string;
  isVerified: boolean;
  isActive: boolean;
  rating: number;
  totalInterventions: number;
  totalRevenue: number;
  commissionRate: number;
  yearsExperience: number;
  createdAt: string;
}

export interface GroceryStoreDto {
  id: string;
  name: string;
  brand: string;
  address: string;
  city: string;
  phone: string;
  isActive: boolean;
  totalProducts: number;
  totalOrders: number;
  totalRevenue: number;
  commissionRate: number;
  createdAt: string;
}

export interface OrderDto {
  id: string;
  orderNumber: string;
  type: string;
  providerName: string;
  customerName: string;
  customerPhone: string;
  delivererName: string;
  status: string;
  subtotal: number;
  deliveryFee: number;
  total: number;
  paymentMethod: string;
  paymentStatus: string;
  createdAt: string;
  deliveredAt: string;
}

export interface DelivererDto {
  id: string;
  firstName: string;
  lastName: string;
  phone: string;
  email: string;
  photoUrl: string;
  vehicleType: string;
  isActive: boolean;
  isOnline: boolean;
  isVerified: boolean;
  rating: number;
  totalDeliveries: number;
  totalEarnings: number;
  latitude: number;
  longitude: number;
  createdAt: string;
}

export interface FinanceOverview {
  totalRevenue: number;
  totalCommissions: number;
  totalPayouts: number;
  pendingPayouts: number;
  revenueByModule: { module: string; amount: number }[];
  monthlyRevenue: ChartData[];
  recentTransactions: TransactionDto[];
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

export interface PromoCodeDto {
  id: string;
  code: string;
  type: string;
  value: number;
  minOrder: number;
  maxDiscount: number;
  usageLimit: number;
  usedCount: number;
  startDate: string;
  endDate: string;
  isActive: boolean;
}
