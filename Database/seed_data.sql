-- Sample Data for Car Rental System (MariaDB)
USE CarRentalDB;

-- Disable checks to allow truncating referenced tables
SET FOREIGN_KEY_CHECKS = 0;

-- Clear tables (Optional, for testing purposes)
TRUNCATE TABLE rentals;
TRUNCATE TABLE cars;
TRUNCATE TABLE categories;
TRUNCATE TABLE customers;
TRUNCATE TABLE users;

-- Seed categories
INSERT INTO categories (name, description, base_daily_rate) VALUES
('Economy', 'Small hatchback, fuel efficient', 100.00),
('SUV', 'Family sized, off-road capable', 250.00),
('Luxury', 'Premium sedan, top of the line features', 500.00);

-- Seed users (Employee/Admin)
-- Password hash for 'admin123' (Example, should be properly hashed in reality)
INSERT INTO users (username, password_hash, email, role) VALUES
('admin', 'PLACEHOLDER_HASH', 'admin@rental.com', 'Admin'),
('staff_user', 'PLACEHOLDER_HASH', 'staff@rental.com', 'Staff');

-- Seed cars
INSERT INTO cars (category_id, brand, model, registration_number, vin, production_year, mileage, status) VALUES
(1, 'Toyota', 'Yaris', 'WA12345', 'VIN000000001YARIS', 2022, 15000, 'Available'),
(1, 'Volkswagen', 'Polo', 'WA54321', 'VIN000000002POLO', 2021, 25000, 'Available'),
(2, 'Hyundai', 'Tucson', 'WB98765', 'VIN000000003TUCSO', 2023, 5000, 'Available'),
(3, 'Mercedes-Benz', 'C-Class', 'WC55555', 'VIN000000004MEBNC', 2023, 1200, 'Available');

-- Seed customers
INSERT INTO customers (first_name, last_name, email, phone, license_number) VALUES
('Jan', 'Kowalski', 'jan.kowalski@email.com', '123-456-789', 'LIC-123-456'),
('Anna', 'Nowak', 'anna.nowak@email.com', '987-654-321', 'LIC-987-654');

-- Seed rentals (Planned and Active)
INSERT INTO rentals (car_id, customer_id, employee_id, start_date, end_date, base_price_at_rental, status) VALUES
(1, 1, 1, '2026-03-20 10:00:00', '2026-03-25 10:00:00', 100.00, 'Finished'),
(3, 2, 2, '2026-03-27 12:00:00', '2026-04-01 12:00:00', 250.00, 'Active');

-- Update status for active car
UPDATE cars SET status = 'Rented' WHERE id = 3;

-- Re-enable checks
SET FOREIGN_KEY_CHECKS = 1;
