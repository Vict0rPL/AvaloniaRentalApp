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
                        CarId = reader.GetInt32("car_id"),
                        CategoryId = reader.GetInt32("category_id"),
                        Brand = reader.GetString("brand"),
                        Model = reader.GetString("model"),
                        Year = reader.GetInt16("year"),
                        Registration = reader.GetString("registration"),
                        Vin = reader.GetString("vin"),
                        Color = reader.IsDBNull(reader.GetOrdinal("color")) ? null : reader.GetString("color"),
                        FuelType = reader.GetString("fuel_type"),
                        Transmission = reader.GetString("transmission"),
                        Seats = reader.GetByte("seats"),
                        MileageKm = reader.GetInt32("mileage_km"),
                        Status = reader.GetString("status"),
                        InsuranceExpiry = reader.IsDBNull(reader.GetOrdinal("insurance_expiry"))
                        ? null
                        : reader.GetDateTime("insurance_expiry"),
                        InspectionExpiry = reader.IsDBNull(reader.GetOrdinal("inspection_expiry"))
                        ? null
                        : reader.GetDateTime("inspection_expiry"),
                        ImagePath = reader.IsDBNull(reader.GetOrdinal("image_path")) ? null : reader.GetString("image_path"),
                        Notes = reader.IsDBNull(reader.GetOrdinal("notes")) ? null : reader.GetString("notes"),
                        IsActive = reader.GetBoolean("is_active"),

                        CategoryName = reader.IsDBNull(reader.GetOrdinal("category_name")) ? string.Empty : reader.GetString("category_name"),
                        DailyRate = reader.IsDBNull(reader.GetOrdinal("daily_rate")) ? 0m : reader.GetDecimal("daily_rate"),
                        DepositAmount = reader.IsDBNull(reader.GetOrdinal("deposit_amount")) ? 0m : reader.GetDecimal("deposit_amount")
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
                        CategoryId = reader.GetInt32("category_id"),
                        Name = reader.GetString("name"),
                        Description = reader.IsDBNull(reader.GetOrdinal("description"))
                         ? null
                         : reader.GetString("description"),
                        DailyRate = reader.GetDecimal("daily_rate"),
                        WeekendRate = reader.IsDBNull(reader.GetOrdinal("weekend_rate"))
                         ? null
                         : reader.GetDecimal("weekend_rate"),
                        WeeklyRate = reader.IsDBNull(reader.GetOrdinal("weekly_rate"))
                         ? null
                         : reader.GetDecimal("weekly_rate"),
                        DepositAmount = reader.GetDecimal("deposit_amount"),
                        MileageLimit = reader.IsDBNull(reader.GetOrdinal("mileage_limit"))
                         ? null
                         : reader.GetInt32("mileage_limit"),
                        ExtraKmRate = reader.IsDBNull(reader.GetOrdinal("extra_km_rate"))
                         ? null
                         : reader.GetDecimal("extra_km_rate"),
                        IsActive = reader.GetBoolean("is_active")
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
                        CustomerId = reader.GetInt32("customer_id"),
                        FirstName = reader.GetString("first_name"),
                        LastName = reader.GetString("last_name"),
                        Pesel = reader.IsDBNull(reader.GetOrdinal("pesel"))
                        ? null
                        : reader.GetString("pesel"),
                        IdDocument = reader.GetString("id_document"),
                        IdType = reader.GetString("id_type"),
                        LicenseNumber = reader.GetString("license_number"),
                        LicenseExpiry = reader.IsDBNull(reader.GetOrdinal("license_expiry"))
                        ? null
                        : reader.GetDateTime("license_expiry"),
                        Email = reader.IsDBNull(reader.GetOrdinal("email"))
                        ? null
                        : reader.GetString("email"),
                        Phone = reader.GetString("phone"),
                        AddressStreet = reader.IsDBNull(reader.GetOrdinal("address_street"))
                        ? null
                        : reader.GetString("address_street"),
                        AddressCity = reader.IsDBNull(reader.GetOrdinal("address_city"))
                        ? null
                        : reader.GetString("address_city"),
                        AddressZip = reader.IsDBNull(reader.GetOrdinal("address_zip"))
                        ? null
                        : reader.GetString("address_zip"),
                        DateOfBirth = reader.IsDBNull(reader.GetOrdinal("date_of_birth"))
                        ? null
                        : reader.GetDateTime("date_of_birth"),
                        CompanyName = reader.IsDBNull(reader.GetOrdinal("company_name"))
                        ? null
                        : reader.GetString("company_name"),
                        Nip = reader.IsDBNull(reader.GetOrdinal("nip"))
                        ? null
                        : reader.GetString("nip"),
                        Notes = reader.IsDBNull(reader.GetOrdinal("notes"))
                        ? null
                        : reader.GetString("notes"),
                        IsBlacklisted = reader.GetBoolean("is_blacklisted"),
                        ActiveRentals = reader.IsDBNull(reader.GetOrdinal("active_rentals"))
                        ? 0
                        : reader.GetInt32("active_rentals")
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
                        UserId = reader.GetInt32("user_id"),
                        Username = reader.GetString("username"),
                        FullName = reader.GetString("full_name"),
                        Email = reader.GetString("email"),
                        Phone = reader.IsDBNull(reader.GetOrdinal("phone"))
                         ? null
                         : reader.GetString("phone"),
                        Role = reader.GetString("role"),
                        IsActive = reader.GetBoolean("is_active"),
                        LastLogin = reader.IsDBNull(reader.GetOrdinal("last_login"))
                         ? null
                         : reader.GetDateTime("last_login"),
                        FailedAttempts = reader.GetInt32("failed_attempts"),
                        LockedUntil = reader.IsDBNull(reader.GetOrdinal("locked_until"))
                         ? null
                         : reader.GetDateTime("locked_until")
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
                        RentalId = reader.GetInt32("rental_id"),
                        RentalNumber = reader.GetString("rental_number"),
                        CustomerId = reader.GetInt32("customer_id"),
                        CarId = reader.GetInt32("car_id"),
                        UserId = reader.GetInt32("user_id"),

                        DateStart = reader.GetDateTime("date_start"),
                        DateEndPlanned = reader.GetDateTime("date_end_planned"),
                        DateEndActual = reader.IsDBNull(reader.GetOrdinal("date_end_actual"))
                        ? null
                        : reader.GetDateTime("date_end_actual"),

                        MileageStart = reader.GetInt32("mileage_start"),
                        MileageEnd = reader.IsDBNull(reader.GetOrdinal("mileage_end"))
                        ? null
                        : reader.GetInt32("mileage_end"),

                        DailyRate = reader.GetDecimal("daily_rate"),
                        TotalDays = reader.GetInt32("total_days"),
                        BaseCost = reader.GetDecimal("base_cost"),
                        ExtraKmCost = reader.GetDecimal("extra_km_cost"),
                        LateReturnCost = reader.GetDecimal("late_return_cost"),
                        DamageCost = reader.GetDecimal("damage_cost"),
                        DiscountPercent = reader.GetDecimal("discount_percent"),
                        TotalCost = reader.GetDecimal("total_cost"),
                        DepositPaid = reader.GetDecimal("deposit_paid"),
                        DepositReturned = reader.GetBoolean("deposit_returned"),

                        Status = reader.GetString("status"),
                        PaymentStatus = reader.GetString("payment_status"),
                        PaymentMethod = reader.IsDBNull(reader.GetOrdinal("payment_method"))
                        ? null
                        : reader.GetString("payment_method"),
                        Notes = reader.IsDBNull(reader.GetOrdinal("notes"))
                        ? null
                        : reader.GetString("notes"),

                        CustomerName = reader.IsDBNull(reader.GetOrdinal("customer_name"))
                        ? string.Empty
                        : reader.GetString("customer_name"),
                        CustomerPhone = reader.IsDBNull(reader.GetOrdinal("customer_phone"))
                        ? string.Empty
                        : reader.GetString("customer_phone"),
                        CarName = reader.IsDBNull(reader.GetOrdinal("car_name"))
                        ? string.Empty
                        : reader.GetString("car_name"),
                        CarRegistration = reader.IsDBNull(reader.GetOrdinal("car_registration"))
                        ? string.Empty
                        : reader.GetString("car_registration"),
                        EmployeeName = reader.IsDBNull(reader.GetOrdinal("employee_name"))
                        ? string.Empty
                        : reader.GetString("employee_name")
                    });
                }
            }
            catch (Exception ex) { Console.WriteLine($"Error fetching rentals: {ex.Message}"); }
            return list;
        }
    }
}
