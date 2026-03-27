using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace AvaloniaRentalApp.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json");
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using var connection = GetConnection();
                await connection.OpenAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database connection failed: {ex.Message}");
                return false;
            }
        }
        public async Task<System.Collections.Generic.List<Models.Car>> GetCarsAsync()
        {
            var list = new System.Collections.Generic.List<Models.Car>();
            try
            {
                using var connection = GetConnection();
                await connection.OpenAsync();
                string sql = "SELECT * FROM cars";
                using var command = new MySqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new Models.Car
                    {
                        Id = reader.GetInt32("id"),
                        CategoryId = reader.GetInt32("category_id"),
                        Brand = reader.GetString("brand"),
                        Model = reader.GetString("model"),
                        RegistrationNumber = reader.GetString("registration_number"),
                        Vin = reader.GetString("vin"),
                        ProductionYear = reader.GetInt32("production_year"),
                        Mileage = reader.GetInt32("mileage"),
                        Status = reader.GetString("status"),
                        CreatedAt = reader.GetDateTime("created_at"),
                        UpdatedAt = reader.GetDateTime("updated_at")
                    });
                }
            }
            catch (Exception ex) { Console.WriteLine($"Error fetching cars: {ex.Message}"); }
            return list;
        }

        public async Task<System.Collections.Generic.List<Models.Category>> GetCategoriesAsync()
        {
            var list = new System.Collections.Generic.List<Models.Category>();
            try
            {
                using var connection = GetConnection();
                await connection.OpenAsync();
                string sql = "SELECT * FROM categories";
                using var command = new MySqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new Models.Category
                    {
                        Id = reader.GetInt32("id"),
                        Name = reader.GetString("name"),
                        Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString("description"),
                        BaseDailyRate = reader.GetDecimal("base_daily_rate"),
                        CreatedAt = reader.GetDateTime("created_at"),
                        UpdatedAt = reader.GetDateTime("updated_at")
                    });
                }
            }
            catch (Exception ex) { Console.WriteLine($"Error fetching categories: {ex.Message}"); }
            return list;
        }

        public async Task<System.Collections.Generic.List<Models.Customer>> GetCustomersAsync()
        {
            var list = new System.Collections.Generic.List<Models.Customer>();
            try
            {
                using var connection = GetConnection();
                await connection.OpenAsync();
                string sql = "SELECT * FROM customers";
                using var command = new MySqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new Models.Customer
                    {
                        Id = reader.GetInt32("id"),
                        FirstName = reader.GetString("first_name"),
                        LastName = reader.GetString("last_name"),
                        Email = reader.GetString("email"),
                        Phone = reader.GetString("phone"),
                        LicenseNumber = reader.GetString("license_number"),
                        CreatedAt = reader.GetDateTime("created_at"),
                        UpdatedAt = reader.GetDateTime("updated_at")
                    });
                }
            }
            catch (Exception ex) { Console.WriteLine($"Error fetching customers: {ex.Message}"); }
            return list;
        }

        public async Task<System.Collections.Generic.List<Models.User>> GetUsersAsync()
        {
            var list = new System.Collections.Generic.List<Models.User>();
            try
            {
                using var connection = GetConnection();
                await connection.OpenAsync();
                string sql = "SELECT * FROM users";
                using var command = new MySqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new Models.User
                    {
                        Id = reader.GetInt32("id"),
                        Username = reader.GetString("username"),
                        PasswordHash = reader.GetString("password_hash"),
                        Email = reader.GetString("email"),
                        Role = reader.GetString("role"),
                        CreatedAt = reader.GetDateTime("created_at"),
                        UpdatedAt = reader.GetDateTime("updated_at")
                    });
                }
            }
            catch (Exception ex) { Console.WriteLine($"Error fetching users: {ex.Message}"); }
            return list;
        }

        public async Task<System.Collections.Generic.List<Models.Rental>> GetRentalsAsync()
        {
            var list = new System.Collections.Generic.List<Models.Rental>();
            try
            {
                using var connection = GetConnection();
                await connection.OpenAsync();
                string sql = "SELECT * FROM rentals";
                using var command = new MySqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new Models.Rental
                    {
                        Id = reader.GetInt32("id"),
                        CarId = reader.GetInt32("car_id"),
                        CustomerId = reader.GetInt32("customer_id"),
                        EmployeeId = reader.GetInt32("employee_id"),
                        StartDate = reader.GetDateTime("start_date"),
                        EndDate = reader.GetDateTime("end_date"),
                        ActualReturnDate = reader.IsDBNull(reader.GetOrdinal("actual_return_date")) ? null : reader.GetDateTime("actual_return_date"),
                        BasePriceAtRental = reader.GetDecimal("base_price_at_rental"),
                        TotalAmount = reader.GetDecimal("total_amount"),
                        Status = reader.GetString("status"),
                        CreatedAt = reader.GetDateTime("created_at"),
                        UpdatedAt = reader.GetDateTime("updated_at")
                    });
                }
            }
            catch (Exception ex) { Console.WriteLine($"Error fetching rentals: {ex.Message}"); }
            return list;
        }
    }
}
