using CardValidation.Core.Enums;
using CardValidation.Core.Services;

namespace CardValidation.Core.Tests
{
    public class CardValidationServiceTests
    {
        private readonly CardValidationService _sut = new CardValidationService();

        #region Owner Validation Tests

        [Theory]
        [InlineData("John Doe")] // Standard two-word name
        [InlineData("J")] // Single letter name
        [InlineData("John Fitzgerald Doe")] // Three-word name
        [InlineData("Namewithoutspaces")] // Single word
        public void ValidateOwner_ShouldReturnTrue_ForValidOwner(string owner)
        {
            // Act
            var result = _sut.ValidateOwner(owner);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("John  Doe")] // Double space
        [InlineData("John Doe4")] // Contains number
        [InlineData("John-Doe")] // Contains hyphen
        [InlineData("John Fitzgerald Kennedy Doe")] // Four words
        [InlineData("")] // Empty string
        [InlineData(" John Doe")] // Leading space

        public void ValidateOwner_ShouldReturnFalse_ForInvalidOwner(string owner)
        {
            // Act
            var result = _sut.ValidateOwner(owner);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Expiration Date Tests

        [Fact]
        public void ValidateIssueDate_ShouldReturnTrue_ForValidFutureDate()
        {
            // Arrange
            var futureDate = DateTime.UtcNow.AddMonths(6);
            var dateStringYY = futureDate.ToString("MM/yy");
            var dateStringYYYY = futureDate.ToString("MM/yyyy");

            // Act
            var resultYY = _sut.ValidateIssueDate(dateStringYY);
            var resultYYYY = _sut.ValidateIssueDate(dateStringYYYY);

            // Assert
            Assert.True(resultYY, "MM/yy format should be valid");
            Assert.True(resultYYYY, "MM/yyyy format should be valid");
        }

        [Fact]
        public void ValidateIssueDate_ShouldReturnFalse_ForPastDate()
        {
            // Arrange
            var pastDate = DateTime.UtcNow.AddMonths(-1);
            var dateString = pastDate.ToString("MM/yy");

            // Act
            var result = _sut.ValidateIssueDate(dateString);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateIssueDate_ShouldReturnFalse_ForCurrentMonthAndYear()
        {
            // Arrange
            var currentDate = DateTime.UtcNow;
            var dateString = currentDate.ToString("MM/yy");

            // Act
            var result = _sut.ValidateIssueDate(dateString);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("13/25")]     // Invalid month
        [InlineData("00/25")]     // Invalid month
        [InlineData("1/25")]      // Invalid month format (requires leading zero)
        [InlineData("12/2")]      // Invalid year format
        [InlineData("12-2025")]   // Invalid separator
        [InlineData("12/20255")]  // Invalid year length
        [InlineData("abc")]       // Not a date
        [InlineData("")]          // Empty string
        public void ValidateIssueDate_ShouldReturnFalse_ForInvalidFormat(string date)
        {
            // Act
            var result = _sut.ValidateIssueDate(date);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region CVC/CVV Tests

        [Theory]
        [InlineData("123")]
        [InlineData("4567")]
        public void ValidateCvc_ShouldReturnTrue_ForValidCvc(string cvc)
        {
            // Act
            var result = _sut.ValidateCvc(cvc);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("12")]      // Too short
        [InlineData("12345")]   // Too long
        [InlineData("abc")]     // Not a number
        [InlineData("12a")]     // Contains letter
        [InlineData("")]        // Empty string
        public void ValidateCvc_ShouldReturnFalse_ForInvalidCvc(string cvc)
        {
            // Act
            var result = _sut.ValidateCvc(cvc);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Card Number Tests

        [Theory]
        // Visa
        [InlineData("4111111111111", true)]      // 13 digits
        [InlineData("4999999999999999", true)]   // 16 digits
        [InlineData("411111111111", false)]      // 12 digits (invalid)
        // MasterCard
        [InlineData("5111111111111111", true)]   // Starts with 51
        [InlineData("5599999999999999", true)]   // Starts with 55
        [InlineData("2221111111111111", true)]   // Starts with 2221
        [InlineData("2720999999999999", true)]   // Starts with 2720
        [InlineData("511111111111111", false)]   // 15 digits (invalid)
        [InlineData("5611111111111111", false)]  // Invalid prefix
        // American Express
        [InlineData("341111111111111", true)]    // 15 digits, starts with 34
        [InlineData("371111111111111", true)]    // 15 digits, starts with 37
        [InlineData("351111111111111", false)]   // Invalid prefix
        // General Invalid
        [InlineData("123456789012345", false)]
        [InlineData("", false)]
        public void ValidateNumber_ShouldReturnExpectedResult_ForCardNumbers(string cardNumber, bool expected)
        {
            // Act
            var result = _sut.ValidateNumber(cardNumber);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region GetPaymentSystemType Tests

        [Theory]
        [InlineData("4111111111111111", PaymentSystemType.Visa)]
        [InlineData("5111111111111111", PaymentSystemType.MasterCard)]
        [InlineData("371111111111111", PaymentSystemType.AmericanExpress)]
        public void GetPaymentSystemType_ShouldReturnCorrectType_ForValidNumbers(string cardNumber, PaymentSystemType expected)
        {
            // Act
            var result = _sut.GetPaymentSystemType(cardNumber);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetPaymentSystemType_ShouldThrowException_ForInvalidCardNumber()
        {
            // Arrange
            var invalidCardNumber = "1234567890123456";

            // Act & Assert
            Assert.Throws<NotImplementedException>(() => _sut.GetPaymentSystemType(invalidCardNumber));
        }

        #endregion
    }
}