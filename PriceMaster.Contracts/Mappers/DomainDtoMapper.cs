using PriceMaster.Contracts.DTOs.Products;
using PriceMaster.Contracts.DTOs.Reports;
using PriceMaster.Domain.Entities;
using PriceMaster.Domain.Reports;

namespace PriceMaster.Contracts.Mappers {
    public static class DomainDtoMapper {

        // Product: DTO -> Domain
        public static Product ToDomain(this CreateProductRequest request) {
            return new Product {
                ProductCode = request.ProductCode,
                SeriesId = request.SeriesId,
                SizeWidth = request.SizeWidth,
                SizeHeight = request.SizeHeight,
                RecommendedPrice = request.RecommendedPrice,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow,
                BomItems = request.BomItems.Select(b => new BomItem {
                    ComponentId = b.ComponentId,
                    Quantity = b.Quantity
                }).ToList()
            };
        }

        // Product: Domain -> DTO Response
        public static CreateProductResponse ToResponse(this Product product, bool success, string message) {
            return new CreateProductResponse {
                Success = success,
                Message = message,
                ProductCode = product.ProductCode
            };
        }

        // ProductDetailedReport: Domain -> DTO Response
        public static ProductDetailedReportResponse ToResponse(this ProductDetailedReport report) {
            return new ProductDetailedReportResponse {
                ProductCode = report.ProductCode,
                Count = report.Count,
                TotalValue = report.TotalValue,
                WorkCost = report.WorkCost,
                PeriodFrom = report.PeriodFrom,
                PeriodTo = report.PeriodTo
            };
        }
    }
}
