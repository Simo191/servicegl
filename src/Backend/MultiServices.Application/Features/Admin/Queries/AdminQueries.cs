using MediatR;
using MultiServices.Application.Common.Models;
using MultiServices.Application.DTOs.Admin;

namespace MultiServices.Application.Features.Admin.Queries;

public record GetDashboardQuery(DateTime? From, DateTime? To) : IRequest<ApiResponse<DashboardDto>>;
public record GetPendingApprovalsQuery(string? EntityType, int Page = 1, int PageSize = 20) : IRequest<ApiResponse<PaginatedList<ApprovalDto>>>;
public record GetAllUsersQuery(string? Role, string? Search, bool? IsActive, int Page = 1, int PageSize = 20) : IRequest<ApiResponse<PaginatedList<UserListDto>>>;
public record GetFinancialReportQuery(DateTime StartDate, DateTime EndDate) : IRequest<ApiResponse<FinancialReportDto>>;
public record GetAllOrdersQuery(string? Type, string? Status, DateTime? From, DateTime? To, int Page = 1, int PageSize = 20) : IRequest<ApiResponse<PaginatedList<object>>>;
public record GetSystemLogsQuery(string? Level, DateTime? From, DateTime? To, int Page = 1, int PageSize = 50) : IRequest<ApiResponse<PaginatedList<object>>>;
