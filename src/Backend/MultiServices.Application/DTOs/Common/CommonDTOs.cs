using MultiServices.Domain.Enums;

namespace MultiServices.Application.DTOs.Common;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}

//public class ApiResponse<T>
//{
//    public bool Success { get; set; }
//    public string Message { get; set; } = string.Empty;
//    public T? Data { get; set; }
//    public List<string> Errors { get; set; } = new();
//    public static ApiResponse<T> Ok(T data, string msg = "Success") => new() { Success = true, Data = data, Message = msg };
//    public static ApiResponse<T> Fail(string msg, List<string>? errors = null) => new() { Success = false, Message = msg, Errors = errors ?? new() };
//}

//public class ApiResponse
//{
//    public bool Success { get; set; }
//    public string Message { get; set; } = string.Empty;
//    public List<string> Errors { get; set; } = new();
//    public static ApiResponse Ok(string msg = "Success") => new() { Success = true, Message = msg };
//    public static ApiResponse Fail(string msg) => new() { Success = false, Message = msg };
//}

public class AddressDto
{
    public Guid Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string? Apartment { get; set; }
    public string? Floor { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsDefault { get; set; }
}

public class CreateAddressRequest
{
    public string Label { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string? Apartment { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsDefault { get; set; }
}

public class ReviewDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? UserImageUrl { get; set; }
    public int Stars { get; set; }
    public string? Comment { get; set; }
    public string? Reply { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class NotificationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class DelivererDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public double AverageRating { get; set; }
    public double CurrentLatitude { get; set; }
    public double CurrentLongitude { get; set; }
}

public class UpdateProfileRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public Gender? Gender { get; set; }
    public Language? PreferredLanguage { get; set; }
}

public class CreateSupportTicketRequest
{
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? OrderType { get; set; }
    public Guid? OrderId { get; set; }
}
