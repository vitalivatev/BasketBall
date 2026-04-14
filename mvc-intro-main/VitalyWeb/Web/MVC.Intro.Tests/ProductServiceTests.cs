using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MVC.Intro.Data;
using MVC.Intro.Models;
using MVC.Intro.Services;

namespace MVC.Intro.Tests
{
    public class ProductServiceTests
    {
        [Fact]
        public void AddProduct_AddsPrefixAndPersistsProduct()
        {
            using var fixture = CreateFixture();

            var added = fixture.Service.AddProduct(new Product
            {
                Name = "Bulls Hoodie",
                Price = 99.99m
            });

            var fetched = fixture.Service.GetProductById(added.Id);
            Assert.Equal("PRD_Bulls Hoodie", fetched.Name);
            Assert.Equal(99.99m, fetched.Price);
        }

        [Fact]
        public void AddProduct_NormalizesImagePath_WhenImagesPrefixIsProvided()
        {
            using var fixture = CreateFixture();

            var added = fixture.Service.AddProduct(new Product
            {
                Name = "Team Jersey",
                Price = 120.00m,
                ImagePath = "images/team.jpg"
            });

            Assert.Equal("team.jpg", added.ImagePath);
        }

        [Fact]
        public void DeleteProduct_RemovesProductFromStorage()
        {
            using var fixture = CreateFixture();
            var added = fixture.Service.AddProduct(new Product
            {
                Name = "Legacy Cap",
                Price = 49.99m
            });

            fixture.Service.DeleteProduct(added.Id);

            Assert.Throws<ArgumentNullException>(() => fixture.Service.GetProductById(added.Id));
        }

        private static TestFixture CreateFixture()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new AppDbContext(options);
            context.Database.EnsureCreated();

            var service = new ProductService(NullLogger<ProductService>.Instance, context);
            return new TestFixture(service, context, connection);
        }

        private sealed class TestFixture : IDisposable
        {
            public ProductService Service { get; }
            private readonly AppDbContext _context;
            private readonly SqliteConnection _connection;

            public TestFixture(ProductService service, AppDbContext context, SqliteConnection connection)
            {
                Service = service;
                _context = context;
                _connection = connection;
            }

            public void Dispose()
            {
                _context.Dispose();
                _connection.Dispose();
            }
        }
    }
}