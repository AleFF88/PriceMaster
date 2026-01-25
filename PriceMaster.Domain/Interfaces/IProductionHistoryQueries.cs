using PriceMaster.Domain.Reports;

namespace PriceMaster.Domain.Interfaces {
    public interface IProductionHistoryQueries {

        /// <summary>
        /// Calculates the total value of all manufactured products
        /// based on their recommended prices stored in production history.
        /// </summary>
        /// <returns>The aggregated total value as a decimal.</returns>
        Task<decimal> GetTotalProductionValueReportAsync();

        /// <summary>
        /// Retrieves a detailed production report for a specific product within the given period.
        /// If no period is specified, the report covers the entire available history.
        /// Returns null if no records are found.
        /// </summary>
        /// <param name="productCode">Unique product code identifying the product.</param> 
        /// <param name="startDate">Optional start date of the reporting period; if null, uses the earliest available record.</param> 
        /// <param name="endDate">Optional end date of the reporting period; if null, uses the latest available record.</param> 
        /// <returns>
        /// A <see cref="ProductDetailedReport"/> containing aggregated data for the product, 
        /// or null if no matching records are found. 
        /// </returns>
        Task<ProductDetailedReport?> GetProductDetailedReportAsync(string productCode, DateTime? startDate = null, DateTime? endDate = null);
    }
}
