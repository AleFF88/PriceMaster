using PriceMaster.Application.Requests;
using PriceMaster.Application.Validators;
using PriceMaster.IntegrationTests.Seeds;

namespace PriceMaster.IntegrationTests.Scenarios.Validation {
    [TestClass]
    public class ValidationTests {
        private readonly CreateProductRequestValidator _productValidator = new();
        private readonly GetProductDetailedReportRequestValidator _reportValidator = new();

        #region Product Service: Single Field Validation (Boundary Tests)

        /// <summary>
        /// Ensures ProductCode failure for empty, whitespace, length exceeding 10 characters or null values.
        /// Verifies that exactly one error is produced when all other fields are valid.
        /// </summary>
        [DataTestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow("12345678901")] // More than 10 chars
        [DataRow(null)]
        public void CreateProductRequest_InvalidProductCode_ShouldHaveSingleError(string? invalidCode) {
            // 1. Arrange
            var request = TestDataFactory.CreateValidRequestWithProductCode(invalidCode);

            // 2. Act
            var result = _productValidator.Validate(request);

            // 3. Assert
            Assert.IsFalse(result.IsValid, $"Validation should fail for ProductCode: '{invalidCode}'");

            Assert.AreEqual(1, result.Errors.Count,
                $"Expected exactly 1 error for ProductCode, but found {result.Errors.Count}. " +
                $"Check if other fields in TestDataFactory remain valid.");

            Assert.IsTrue(result.Errors.Any(e => e.PropertyName == nameof(request.ProductCode)),
                "The validation error must be associated with the 'ProductCode' property.");
        }

        /// <summary>
        /// Validates that numeric fields trigger errors when they are zero or negative.
        /// </summary>
        [DataTestMethod]
        [DataRow(0.0)]
        [DataRow(-1.0)]
        public void CreateProductRequest_NonPositivePrice_ShouldHaveSingleError(double invalidPrice) {
            // Arrange
            var request = TestDataFactory.CreateValidRequestWithRecomendedPrice((decimal)invalidPrice);

            // Act
            var result = _productValidator.Validate(request);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);

            Assert.AreEqual(1, result.Errors.Count,
                $"Expected exactly 1 error for RecommendedPrice, but found {result.Errors.Count}. " +
                $"Check if other fields in TestDataFactory remain valid.");
            Assert.IsTrue(result.Errors.Any(e => e.PropertyName == nameof(request.RecommendedPrice)));
        }

        #endregion

        #region Product Service: Complex Validation

        /// <summary>
        /// Verifies that an empty request object results in a complete list of validation errors.
        /// Expected 6 errors: ProductCode, SeriesId, SizeWidth, SizeHeight, RecommendedPrice, and BomItems.
        /// </summary>
        [TestMethod]
        public void CreateProductRequest_EmptyObject_ShouldReportAllSixRequiredFields() {
            // Arrange
            // Initializing with nulls/defaults to trigger all validation rules simultaneously
            var request = new CreateProductRequest {
                ProductCode = null!,
                BomItems = null!
            };

            // Act
            var result = _productValidator.Validate(request);

            // Assert
            Assert.IsFalse(result.IsValid, "Validation must fail for an empty request object.");

            // Checking total count as seen in the debugger output
            Assert.AreEqual(6, result.Errors.Count,
                $"Expected exactly 6 validation errors, but found {result.Errors.Count}.");

            // Extracting property names to verify the presence of each specific error
            var errorFields = result.Errors.Select(e => e.PropertyName).ToList();

            CollectionAssert.Contains(errorFields, nameof(request.ProductCode), "Missing error for ProductCode.");
            CollectionAssert.Contains(errorFields, nameof(request.SeriesId), "Missing error for SeriesId.");
            CollectionAssert.Contains(errorFields, nameof(request.SizeWidth), "Missing error for SizeWidth.");
            CollectionAssert.Contains(errorFields, nameof(request.SizeHeight), "Missing error for SizeHeight.");
            CollectionAssert.Contains(errorFields, nameof(request.RecommendedPrice), "Missing error for RecommendedPrice.");
            CollectionAssert.Contains(errorFields, nameof(request.BomItems), "Missing error for BomItems (collection itself).");
        }

        /// <summary>
        /// Verifies that errors in nested objects (BOM items) are also collected alongside top-level errors.
        /// </summary>
        [TestMethod]
        public void CreateProductRequest_InvalidTopLevelAndNestedFields_ShouldReportAllErrors() {
            // Arrange
            var request = TestDataFactory.CreateRequestWithInvalidCodeAndBom(
                "", // Top-level error
                new List<BomItemRequest> {
                        new BomItemRequest() { ComponentId = 0, Quantity = -5 } // Two nested errors
                }
            );

            // Act
            var result = _productValidator.Validate(request);

            // Assert
            Assert.IsFalse(result.IsValid);

            var errorFields = result.Errors.Select(e => e.PropertyName).ToList();

            // Check top-level
            CollectionAssert.Contains(errorFields, nameof(request.ProductCode));

            // Check nested errors 
            Assert.IsTrue(errorFields.Any(f => f.Contains("BomItems[0].ComponentId")), "Missing nested error for ComponentId.");
            Assert.IsTrue(errorFields.Any(f => f.Contains("BomItems[0].Quantity")), "Missing nested error for Quantity.");
        }

        #endregion

        #region History Service (Reports) Validation

        [TestMethod]
        public void GetProductDetailedReportRequest_InvalidDateRange_ShouldHaveError() {
            // Arrange
            var request = new GetProductDetailedReportRequest {
                ProductCode = "SKU1",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow
            };

            // Act
            var result = _reportValidator.Validate(request);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(e => e.PropertyName == nameof(request.StartDate)));
        }

        #endregion
    }
}