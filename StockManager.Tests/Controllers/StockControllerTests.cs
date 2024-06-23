using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Controllers;
using api.Dtos.Stock;
using api.Interfaces;
using api.Mappers;
using api.Models;
using api.Helpers;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using api.Data;

namespace StockManager.Tests.Controllers
{
    public class StockControllerTests
    {
        private readonly IStockRepository _fakeStockRepository;
        private readonly StockController _stockController;

        public StockControllerTests()
        {
            _fakeStockRepository = A.Fake<IStockRepository>();
            _stockController = new StockController(null, _fakeStockRepository);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WithListOfStocks()
        {
            // Arrange
            var query = new QueryObject();
            var fakeStocks = new List<Stock> 
            { 
                new Stock { Id = 1, Symbol = "AAPL", CompanyName = "Apple Inc.", Purchase = 150.00m, LastDiv = 0.5m, Industry = "Technology", MarketCap = 2000000000 },
                new Stock { Id = 2, Symbol = "MSFT", CompanyName = "Microsoft Corp.", Purchase = 250.00m, LastDiv = 0.7m, Industry = "Technology", MarketCap = 1800000000 }
            };

            A.CallTo(() => _fakeStockRepository.GetAllAsync(query))
                .Returns(Task.FromResult(fakeStocks));

            // Act
            var result = await _stockController.GetAll(query) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
            var stockDtos = result.Value as List<StockDto>;
            stockDtos.Should().NotBeNull();
            stockDtos.Count.Should().Be(2);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenStockExists()
        {
            // Arrange
            var stockId = 1;
            var fakeStock = new Stock { Id = stockId, Symbol = "AAPL", CompanyName = "Apple Inc.", Purchase = 150.00m, LastDiv = 0.5m, Industry = "Technology", MarketCap = 2000000000 };
            A.CallTo(() => _fakeStockRepository.GetByIdAsync(stockId)).Returns(Task.FromResult(fakeStock));

            // Act
            var result = await _stockController.GetById(stockId) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
            result.Value.Should().BeEquivalentTo(fakeStock.ToStockDto());
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenStockDoesNotExist()
        {
            // Arrange
            var stockId = 1;
            A.CallTo(() => _fakeStockRepository.GetByIdAsync(stockId)).Returns(Task.FromResult<Stock>(null));

            // Act
            var result = await _stockController.GetById(stockId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_ShouldReturnCreated_WhenStockIsValid()
        {
            // Arrange
            var newStockDto = new CreateStockRequestDto { Symbol = "GOOG", CompanyName = "Google LLC", Purchase = 1200.00m, LastDiv = 0.6m, Industry = "Technology", MarketCap = 1500000000 };
            var newStock = newStockDto.ToStockFromCreateDTO();
            A.CallTo(() => _fakeStockRepository.CreateAsync(A<Stock>.Ignored)).Invokes((Stock s) => s.Id = 3);

            // Act
            var result = await _stockController.Create(newStockDto) as CreatedAtActionResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(201);
            var createdStockDto = result.Value as StockDto;
            createdStockDto.Should().NotBeNull();
            createdStockDto.Id.Should().Be(3);
            A.CallTo(() => _fakeStockRepository.CreateAsync(A<Stock>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public async Task Update_ShouldReturnOk_WhenStockIsUpdated()
        {
            // Arrange
            var stockId = 1;
            var updateDto = new UpdateStockRequestDto { Symbol = "AAPL", CompanyName = "Apple Inc.", Purchase = 160.00m, LastDiv = 0.55m, Industry = "Technology", MarketCap = 2200000000 };
            var updatedStock = new Stock { Id = stockId, Symbol = "AAPL", CompanyName = "Apple Inc.", Purchase = 160.00m, LastDiv = 0.55m, Industry = "Technology", MarketCap = 2200000000 };
            A.CallTo(() => _fakeStockRepository.UpdateAsync(stockId, updateDto)).Returns(Task.FromResult(updatedStock));

            // Act
            var result = await _stockController.Update(stockId, updateDto) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
            result.Value.Should().BeEquivalentTo(updatedStock.ToStockDto());
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenStockDoesNotExist()
        {
            // Arrange
            var stockId = 1;
            var updateDto = new UpdateStockRequestDto { Symbol = "AAPL", CompanyName = "Apple Inc.", Purchase = 160.00m, LastDiv = 0.55m, Industry = "Technology", MarketCap = 2200000000 };
            A.CallTo(() => _fakeStockRepository.UpdateAsync(stockId, updateDto)).Returns(Task.FromResult<Stock>(null));

            // Act
            var result = await _stockController.Update(stockId, updateDto);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenStockIsDeleted()
        {
            // Arrange
            var stockId = 1;
            A.CallTo(() => _fakeStockRepository.DeleteAsync(stockId)).Returns(Task.FromResult(new Stock { Id = stockId }));

            // Act
            var result = await _stockController.Delete(stockId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            A.CallTo(() => _fakeStockRepository.DeleteAsync(stockId)).MustHaveHappened();
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenStockDoesNotExist()
        {
            // Arrange
            var stockId = 1;
            A.CallTo(() => _fakeStockRepository.DeleteAsync(stockId)).Returns(Task.FromResult<Stock>(null));

            // Act
            var result = await _stockController.Delete(stockId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
