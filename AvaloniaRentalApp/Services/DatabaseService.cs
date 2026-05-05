using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Dapper;
using AvaloniaRentalApp.Models;

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
                               ?? throw new InvalidOperationException(
                                   "Connection string 'DefaultConnection' not found in appsettings.json");
        }

        public MySqlConnection GetConnection() => new(_connectionString);

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database connection failed: {ex.Message}");
                return false;
            }
        }


        // Cars + JOIN categories for CategoryName, DailyRate, DepositAmount
        public async Task<List<Models.Car>> GetCarsAsync()
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                const string sql = @"
                    SELECT 
                        c.car_id            AS CarId,
                        c.category_id       AS CategoryId,
                        c.brand             AS Brand,
                        c.model             AS Model,
                        c.year              AS Year,
                        c.registration      AS Registration,
                        c.vin               AS Vin,
                        c.color             AS Color,
                        c.fuel_type         AS FuelType,
                        c.transmission      AS Transmission,
                        c.seats             AS Seats,
                        c.mileage_km        AS MileageKm,
                        c.status            AS Status,
                        c.insurance_expiry  AS InsuranceExpiry,
                        c.inspection_expiry AS InspectionExpiry,
                        c.image_path        AS ImagePath,
                        c.notes             AS Notes,
                        c.is_active         AS IsActive,
                        cat.name            AS CategoryName,
                        cat.daily_rate      AS DailyRate,
                        cat.deposit_amount  AS DepositAmount
                    FROM cars c
                    JOIN categories cat ON c.category_id = cat.category_id";

                return (await conn.QueryAsync<Models.Car>(sql)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching cars: {ex.Message}");
                return new();
            }
        }


        // Categories
        public async Task<List<Models.Category>> GetCategoriesAsync()
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                const string sql = @"
                    SELECT 
                        category_id    AS CategoryId,
                        name           AS Name,
                        description    AS Description,
                        daily_rate     AS DailyRate,
                        weekend_rate   AS WeekendRate,
                        weekly_rate    AS WeeklyRate,
                        deposit_amount AS DepositAmount,
                        mileage_limit  AS MileageLimit,
                        extra_km_rate  AS ExtraKmRate,
                        is_active      AS IsActive
                    FROM categories";

                return (await conn.QueryAsync<Models.Category>(sql)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching categories: {ex.Message}");
                return new();
            }
        }


        // Customers + subquery for ActiveRentals
        public async Task<List<Models.Customer>> GetCustomersAsync()
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                const string sql = @"
                    SELECT 
                        cu.customer_id    AS CustomerId,
                        cu.first_name     AS FirstName,
                        cu.last_name      AS LastName,
                        cu.pesel          AS Pesel,
                        cu.id_document    AS IdDocument,
                        cu.id_type        AS IdType,
                        cu.license_number AS LicenseNumber,
                        cu.license_expiry AS LicenseExpiry,
                        cu.email          AS Email,
                        cu.phone          AS Phone,
                        cu.address_street AS AddressStreet,
                        cu.address_city   AS AddressCity,
                        cu.address_zip    AS AddressZip,
                        cu.date_of_birth  AS DateOfBirth,
                        cu.company_name   AS CompanyName,
                        cu.nip            AS Nip,
                        cu.notes          AS Notes,
                        cu.is_blacklisted AS IsBlacklisted,
                        cu.is_active      AS IsActive,
                        COALESCE((
                            SELECT COUNT(*) FROM rentals r 
                            WHERE r.customer_id = cu.customer_id 
                              AND r.status = 'aktywna'
                        ), 0) AS ActiveRentals
                    FROM customers cu";

                return (await conn.QueryAsync<Customer>(sql)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching customers: {ex.Message}");
                return new();
            }
        }


        // Users
        public async Task<List<User>> GetUsersAsync()
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                const string sql = @"
                    SELECT 
                        user_id         AS UserId,
                        username        AS Username,
                        full_name       AS FullName,
                        email           AS Email,
                        phone           AS Phone,
                        role            AS Role,
                        is_active       AS IsActive,
                        last_login      AS LastLogin,
                        failed_attempts AS FailedAttempts,
                        locked_until    AS LockedUntil
                    FROM users";

                return (await conn.QueryAsync<User>(sql)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching users: {ex.Message}");
                return new();
            }
        }


        // Rentals + triple JOIN for CustomerName, CarName, EmployeeName
        public async Task<List<Rental>> GetRentalsAsync()
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                const string sql = @"
                    SELECT 
                        r.rental_id        AS RentalId,
                        r.rental_number    AS RentalNumber,
                        r.customer_id      AS CustomerId,
                        r.car_id           AS CarId,
                        r.user_id          AS UserId,
                        r.date_start       AS DateStart,
                        r.date_end_planned AS DateEndPlanned,
                        r.date_end_actual  AS DateEndActual,
                        r.mileage_start    AS MileageStart,
                        r.mileage_end      AS MileageEnd,
                        r.daily_rate       AS DailyRate,
                        r.total_days       AS TotalDays,
                        r.base_cost        AS BaseCost,
                        r.extra_km_cost    AS ExtraKmCost,
                        r.late_return_cost AS LateReturnCost,
                        r.damage_cost      AS DamageCost,
                        r.discount_percent AS DiscountPercent,
                        r.total_cost       AS TotalCost,
                        r.deposit_paid     AS DepositPaid,
                        r.deposit_returned AS DepositReturned,
                        r.status           AS Status,
                        r.payment_status   AS PaymentStatus,
                        r.payment_method   AS PaymentMethod,
                        r.notes            AS Notes,
                        CONCAT(cu.first_name, ' ', cu.last_name) AS CustomerName,
                        cu.phone           AS CustomerPhone,
                        CONCAT(c.brand, ' ', c.model)            AS CarName,
                        c.registration     AS CarRegistration,
                        u.full_name        AS EmployeeName
                    FROM rentals r
                    JOIN customers cu ON r.customer_id = cu.customer_id
                    JOIN cars c       ON r.car_id = c.car_id
                    JOIN users u      ON r.user_id = u.user_id";

                return (await conn.QueryAsync<Rental>(sql)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching rentals: {ex.Message}");
                return new();
            }
        }


        // Single user by username (for AuthService login)
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                const string sql = @"
                    SELECT 
                        user_id         AS UserId,
                        username        AS Username,
                        password_hash   AS PasswordHash,
                        full_name       AS FullName,
                        email           AS Email,
                        phone           AS Phone,
                        role            AS Role,
                        is_active       AS IsActive,
                        last_login      AS LastLogin,
                        failed_attempts AS FailedAttempts,
                        locked_until    AS LockedUntil
                    FROM users 
                    WHERE username = @Username";

                return await conn.QuerySingleOrDefaultAsync<User>(sql, new { Username = username });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user: {ex.Message}");
                return null;
            }
        }


        // Add a new customer, returns the new customer_id (0 on failure)
        public async Task<int> AddCustomerAsync(Customer c)
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                const string sql = @"
                    INSERT INTO customers
                        (first_name, last_name, pesel, id_document, id_type,
                         license_number, license_expiry, email, phone,
                         address_street, address_city, address_zip, date_of_birth, notes)
                    VALUES
                        (@FirstName, @LastName, @Pesel, @IdDocument, @IdType,
                         @LicenseNumber, @LicenseExpiry, @Email, @Phone,
                         @AddressStreet, @AddressCity, @AddressZip, @DateOfBirth, @Notes);
                    SELECT LAST_INSERT_ID();";

                return await conn.ExecuteScalarAsync<int>(sql, c);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding customer: {ex.Message}");
                return 0;
            }
        }


        // Update user login state (lockout, failed attempts, last login)
        public async Task UpdateUserLoginStateAsync(int userId, int failedAttempts, DateTime? lockedUntil, DateTime? lastLogin)
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                const string sql = @"
                    UPDATE users 
                    SET failed_attempts = @FailedAttempts,
                        locked_until    = @LockedUntil,
                        last_login      = @LastLogin
                    WHERE user_id = @UserId";

                await conn.ExecuteAsync(sql, new { FailedAttempts = failedAttempts, LockedUntil = lockedUntil, LastLogin = lastLogin, UserId = userId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user state: {ex.Message}");
            }
        }


        // Fleet stats by category (for dashboard)
        public async Task<List<FleetStatusRow>> GetFleetStatsAsync()
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                const string sql = @"
                    SELECT 
                        cat.name AS CategoryName,
                        COUNT(c.car_id) AS TotalVehicles,
                        SUM(CASE WHEN c.status = 'dostepny'    THEN 1 ELSE 0 END) AS Available,
                        SUM(CASE WHEN c.status = 'wypozyczony' THEN 1 ELSE 0 END) AS Rented,
                        SUM(CASE WHEN c.status = 'serwis'      THEN 1 ELSE 0 END) AS InService,
                        cat.daily_rate AS DailyRate
                    FROM cars c
                    JOIN categories cat ON c.category_id = cat.category_id
                    WHERE c.is_active = TRUE
                    GROUP BY cat.category_id, cat.name, cat.daily_rate";

                return (await conn.QueryAsync<FleetStatusRow>(sql)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching fleet stats: {ex.Message}");
                return new();
            }
        }

        // Add a new car
        public async Task<int> AddCarAsync(Car car)
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                const string sql = @"
                    INSERT INTO cars (
                        category_id, brand, model, year, registration, vin, 
                        color, fuel_type, transmission, seats, mileage_km, 
                        status, insurance_expiry, inspection_expiry, image_path, notes, is_active
                    ) VALUES (
                        @CategoryId, @Brand, @Model, @Year, @Registration, @Vin,
                        @Color, @FuelType, @Transmission, @Seats, @MileageKm,
                        @Status, @InsuranceExpiry, @InspectionExpiry, @ImagePath, @Notes, @IsActive
                    );
                    SELECT LAST_INSERT_ID();";

                return await conn.ExecuteScalarAsync<int>(sql, car);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding car: {ex.Message}");
                return -1;
            }
        }

        // Update an existing car
        public async Task<bool> UpdateCarAsync(Car car)
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                const string sql = @"
                    UPDATE cars SET 
                        category_id = @CategoryId,
                        brand = @Brand,
                        model = @Model,
                        year = @Year,
                        registration = @Registration,
                        vin = @Vin,
                        color = @Color,
                        fuel_type = @FuelType,
                        transmission = @Transmission,
                        seats = @Seats,
                        mileage_km = @MileageKm,
                        status = @Status,
                        insurance_expiry = @InsuranceExpiry,
                        inspection_expiry = @InspectionExpiry,
                        image_path = @ImagePath,
                        notes = @Notes,
                        is_active = @IsActive
                    WHERE car_id = @CarId";

                var rowsAffected = await conn.ExecuteAsync(sql, car);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating car: {ex.Message}");
                return false;
            }
        }

        // Soft delete a car
        public async Task<bool> DeleteCarAsync(int carId)
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                const string sql = @"
                    UPDATE cars SET is_active = FALSE WHERE car_id = @CarId";

                var rowsAffected = await conn.ExecuteAsync(sql, new { CarId = carId });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting car: {ex.Message}");
                return false;
            }
        }

        // Restore a soft-deleted car
        public async Task<bool> RestoreCarAsync(int carId)
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                const string sql = @"
                    UPDATE cars SET is_active = TRUE WHERE car_id = @CarId";

                var rowsAffected = await conn.ExecuteAsync(sql, new { CarId = carId });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error restoring car: {ex.Message}");
                return false;
            }
        }

        // Update an existing customer
        public async Task<bool> UpdateCustomerAsync(Customer c)
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                const string sql = @"
                    UPDATE customers SET
                        first_name     = @FirstName,
                        last_name      = @LastName,
                        pesel          = @Pesel,
                        id_document    = @IdDocument,
                        id_type        = @IdType,
                        license_number = @LicenseNumber,
                        license_expiry = @LicenseExpiry,
                        email          = @Email,
                        phone          = @Phone,
                        address_street = @AddressStreet,
                        address_city   = @AddressCity,
                        address_zip    = @AddressZip,
                        date_of_birth  = @DateOfBirth,
                        notes          = @Notes
                    WHERE customer_id = @CustomerId";

                var rowsAffected = await conn.ExecuteAsync(sql, c);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating customer: {ex.Message}");
                return false;
            }
        }

        // Soft delete a customer
        public async Task<bool> DeleteCustomerAsync(int customerId)
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                const string sql = @"
                    UPDATE customers SET is_active = FALSE WHERE customer_id = @CustomerId";

                var rowsAffected = await conn.ExecuteAsync(sql, new { CustomerId = customerId });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting customer: {ex.Message}");
                return false;
            }
        }

        // Restore a soft-deleted customer
        public async Task<bool> RestoreCustomerAsync(int customerId)
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                const string sql = @"
                    UPDATE customers SET is_active = TRUE WHERE customer_id = @CustomerId";

                var rowsAffected = await conn.ExecuteAsync(sql, new { CustomerId = customerId });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error restoring customer: {ex.Message}");
                return false;
            }
        }

        // Cars with status='dostepny' for the add-rental picker
        public async Task<List<Car>> GetAvailableCarsAsync()
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                const string sql = @"
                    SELECT
                        c.car_id            AS CarId,
                        c.category_id       AS CategoryId,
                        c.brand             AS Brand,
                        c.model             AS Model,
                        c.year              AS Year,
                        c.registration      AS Registration,
                        c.mileage_km        AS MileageKm,
                        c.status            AS Status,
                        c.is_active         AS IsActive,
                        cat.name            AS CategoryName,
                        cat.daily_rate      AS DailyRate,
                        cat.deposit_amount  AS DepositAmount
                    FROM cars c
                    JOIN categories cat ON c.category_id = cat.category_id
                    WHERE c.status = 'dostepny'
                      AND c.is_active = TRUE
                    ORDER BY c.brand, c.model";

                return (await conn.QueryAsync<Car>(sql)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching available cars: {ex.Message}");
                return new();
            }
        }

        // Add a new rental, returns the new rental_id (-1 on failure)
        public async Task<int> AddRentalAsync(Rental r)
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                const string sql = @"
                    INSERT INTO rentals (
                        customer_id, car_id, user_id,
                        date_start, date_end_planned,
                        mileage_start,
                        daily_rate, total_days, base_cost, total_cost,
                        deposit_paid, payment_method, notes
                    ) VALUES (
                        @CustomerId, @CarId, @UserId,
                        @DateStart, @DateEndPlanned,
                        @MileageStart,
                        @DailyRate, @TotalDays, @BaseCost, @TotalCost,
                        @DepositPaid, @PaymentMethod, @Notes
                    );
                    SELECT LAST_INSERT_ID();";

                return await conn.ExecuteScalarAsync<int>(sql, r);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding rental: {ex.Message}");
                return -1;
            }
        }
    }
}