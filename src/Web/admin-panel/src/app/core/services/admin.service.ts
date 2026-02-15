import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import {
  ApiResponse, PaginatedResult, DashboardStats, AdminUserDto, ApprovalDto,
  ApproveEntityRequest, AdminOrderDto, AdminFinanceDto, FinancialReportDto,
  CreatePromotionRequest, CommissionSettingsDto, SystemLogDto
} from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class AdminService {
  constructor(private api: ApiService) {}

  // ── Dashboard ──
  getDashboard(from?: string, to?: string): Observable<ApiResponse<DashboardStats>> {
    return this.api.get<DashboardStats>('admin/dashboard', { from, to });
  }

  // ── Users ──
  getUsers(params: { role?: string; search?: string; isActive?: boolean; page?: number; pageSize?: number }): Observable<ApiResponse<PaginatedResult<AdminUserDto>>> {
    return this.api.getPaginated<AdminUserDto>('admin/users', params);
  }

  suspendUser(id: string, reason: string): Observable<ApiResponse<any>> {
    return this.api.post('admin/users/' + id + '/suspend', JSON.stringify(reason));
  }

  activateUser(id: string): Observable<ApiResponse<any>> {
    return this.api.post('admin/users/' + id + '/activate', {});
  }

  // ── Approvals ──
  getPendingApprovals(entityType?: string, page = 1, pageSize = 20): Observable<ApiResponse<PaginatedResult<ApprovalDto>>> {
    return this.api.getPaginated<ApprovalDto>('admin/approvals', { entityType, page, pageSize });
  }

  approveEntity(dto: ApproveEntityRequest): Observable<ApiResponse<any>> {
    return this.api.post('admin/approvals', dto);
  }

  // ── Orders ──
  getOrders(params: { type?: string; status?: string; from?: string; to?: string; page?: number; pageSize?: number }): Observable<ApiResponse<PaginatedResult<AdminOrderDto>>> {
    return this.api.getPaginated<AdminOrderDto>('admin/orders', params);
  }

  cancelOrder(id: string, orderType: string, reason: string): Observable<ApiResponse<any>> {
    return this.api.post('admin/orders/' + id + '/cancel?orderType=' + orderType, JSON.stringify(reason));
  }

  reassignDriver(id: string, orderType: string, newDriverId: string): Observable<ApiResponse<any>> {
    return this.api.post('admin/orders/' + id + '/reassign-driver?orderType=' + orderType, JSON.stringify(newDriverId));
  }

  refundOrder(id: string, orderType: string, amount: number): Observable<ApiResponse<any>> {
    return this.api.post('admin/orders/' + id + '/refund?orderType=' + orderType, { orderId: id, amount });
  }

  // ── Finance ──
  getFinancialReport(startDate: string, endDate: string): Observable<ApiResponse<FinancialReportDto>> {
    return this.api.get<FinancialReportDto>('admin/finance/report', { startDate, endDate });
  }

  exportFinancialReport(startDate: string, endDate: string, format = 'xlsx'): Observable<ApiResponse<any>> {
    return this.api.post('admin/finance/export?startDate=' + startDate + '&endDate=' + endDate + '&format=' + format, {});
  }

  // ── Commissions ──
  updateCommissions(dto: CommissionSettingsDto): Observable<ApiResponse<any>> {
    return this.api.put('admin/commissions', dto);
  }

  // ── Promotions ──
  createPromotion(dto: CreatePromotionRequest): Observable<ApiResponse<any>> {
    return this.api.post('admin/promotions', dto);
  }

  // ── Notifications ──
  sendBulkNotification(dto: { title: string; message: string; targetAudience: string }): Observable<ApiResponse<any>> {
    return this.api.post('admin/notifications/bulk', dto);
  }

  // ── Logs ──
  getLogs(params: { level?: string; from?: string; to?: string; page?: number; pageSize?: number }): Observable<ApiResponse<PaginatedResult<SystemLogDto>>> {
    return this.api.getPaginated<SystemLogDto>('admin/logs', params);
  }
}
