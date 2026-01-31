namespace PriceMaster.Application.DTOs {
    public class ServiceResult {
        public bool IsSuccess { get; init; }
        public string? Message { get; init; } 

        public static ServiceResult Success(string message = "") {
            return new ServiceResult {
                IsSuccess = true,
                Message = message
            };
        }

        public static ServiceResult Failure(string message) {
            return new ServiceResult {
                IsSuccess = false,
                Message = message
            };
        }
    }
}