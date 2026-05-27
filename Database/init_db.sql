-- ============================================================
-- Database Schema for Car Rental System (MariaDB)
-- Table/column naming matches DatabaseService.cs:
--   cars (car_id), categories (category_id), customers (customer_id),
--   users (user_id), rentals (rental_id, car_id)
-- ============================================================
DROP DATABASE IF EXISTS CarRentalDB;

CREATE DATABASE IF NOT EXISTS CarRentalDB
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_polish_ci;

USE CarRentalDB;

-- ============================================================
-- Table: categories  →  Category.cs
-- ============================================================
CREATE TABLE IF NOT EXISTS categories (
    category_id     INT AUTO_INCREMENT PRIMARY KEY,
    name            VARCHAR(60)    NOT NULL UNIQUE,
    description     VARCHAR(255)   NULL,
    daily_rate      DECIMAL(10,2)  NOT NULL DEFAULT 0.00,
    weekend_rate    DECIMAL(10,2)  NULL,
    weekly_rate     DECIMAL(10,2)  NULL,
    deposit_amount  DECIMAL(10,2)  NOT NULL DEFAULT 0.00,
    mileage_limit   INT            NULL,
    extra_km_rate   DECIMAL(6,2)   NULL,
    is_active       BOOLEAN        NOT NULL DEFAULT TRUE,
    created_at      TIMESTAMP      DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMP      DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- ============================================================
-- Table: cars  →  Car.cs
-- ============================================================
CREATE TABLE IF NOT EXISTS cars (
    car_id          INT AUTO_INCREMENT PRIMARY KEY,
    category_id     INT            NOT NULL,
    brand           VARCHAR(50)    NOT NULL,
    model           VARCHAR(50)    NOT NULL,
    year            SMALLINT       NOT NULL,
    registration    VARCHAR(20)    NOT NULL UNIQUE,
    vin             VARCHAR(17)    NOT NULL UNIQUE,
    color           VARCHAR(30)    NULL,
    fuel_type       ENUM('benzyna','diesel','LPG','elektryczny','hybryda') NOT NULL DEFAULT 'benzyna',
    transmission    ENUM('manualna','automatyczna') NOT NULL DEFAULT 'manualna',
    seats           TINYINT        NOT NULL DEFAULT 5,
    mileage_km      INT            NOT NULL DEFAULT 0,
    status          ENUM('dostepny','wypozyczony','serwis','wycofany') NOT NULL DEFAULT 'dostepny',
    insurance_expiry  DATE         NULL,
    inspection_expiry DATE         NULL,
    image_path      VARCHAR(255)   NULL,
    notes           TEXT           NULL,
    is_active       BOOLEAN        NOT NULL DEFAULT TRUE,
    created_at      TIMESTAMP      DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMP      DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    CONSTRAINT fk_cars_category
        FOREIGN KEY (category_id) REFERENCES categories(category_id)
        ON UPDATE CASCADE ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE INDEX idx_cars_status ON cars(status);
CREATE INDEX idx_cars_category ON cars(category_id);

-- ============================================================
-- Table: customers  →  Customer.cs
-- ============================================================
CREATE TABLE IF NOT EXISTS customers (
    customer_id     INT AUTO_INCREMENT PRIMARY KEY,
    first_name      VARCHAR(60)    NOT NULL,
    last_name       VARCHAR(80)    NOT NULL,
    pesel           VARCHAR(11)    NULL UNIQUE,
    id_document     VARCHAR(30)    NOT NULL,
    id_type         ENUM('dowod','paszport','prawo_jazdy') NOT NULL DEFAULT 'dowod',
    license_number  VARCHAR(30)    NOT NULL,
    license_expiry  DATE           NULL,
    email           VARCHAR(150)   NULL,
    phone           VARCHAR(20)    NOT NULL,
    address_street  VARCHAR(120)   NULL,
    address_city    VARCHAR(60)    NULL,
    address_zip     VARCHAR(10)    NULL,
    date_of_birth   DATE           NULL,
    company_name    VARCHAR(120)   NULL,
    nip             VARCHAR(13)    NULL,
    notes           TEXT           NULL,
    is_blacklisted  BOOLEAN        NOT NULL DEFAULT FALSE,
    is_active       BOOLEAN        NOT NULL DEFAULT TRUE,
    created_at      TIMESTAMP      DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMP      DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;


CREATE INDEX idx_customers_name ON customers(last_name, first_name);
CREATE INDEX idx_customers_phone ON customers(phone);

-- ============================================================
-- Table: users  →  User.cs
-- ============================================================
CREATE TABLE IF NOT EXISTS users (
    user_id         INT AUTO_INCREMENT PRIMARY KEY,
    username        VARCHAR(50)    NOT NULL UNIQUE,
    password_hash   VARCHAR(255)   NOT NULL,
    full_name       VARCHAR(120)   NOT NULL,
    email           VARCHAR(150)   NOT NULL UNIQUE,
    phone           VARCHAR(20)    NULL,
    role            ENUM('admin','employee') NOT NULL DEFAULT 'employee',
    is_active       BOOLEAN        NOT NULL DEFAULT TRUE,
    last_login      DATETIME       NULL,
    failed_attempts INT            NOT NULL DEFAULT 0,
    locked_until    DATETIME       NULL,
    created_at      TIMESTAMP      DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMP      DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- ============================================================
-- Table: rentals  →  Rental.cs
-- FK is car_id (matching DatabaseService.cs reader.GetInt32("car_id"))
-- ============================================================
CREATE TABLE IF NOT EXISTS rentals (
    rental_id        INT AUTO_INCREMENT PRIMARY KEY,
    rental_number    VARCHAR(20)    NOT NULL UNIQUE,
    customer_id      INT            NOT NULL,
    car_id           INT            NOT NULL,
    user_id          INT            NOT NULL,

    -- Dates
    date_start       DATETIME       NOT NULL,
    date_end_planned DATETIME       NOT NULL,
    date_end_actual  DATETIME       NULL,

    -- Mileage
    mileage_start    INT            NOT NULL DEFAULT 0,
    mileage_end      INT            NULL,

    -- Cost breakdown
    daily_rate       DECIMAL(10,2)  NOT NULL,
    total_days       INT            NOT NULL,
    base_cost        DECIMAL(10,2)  NOT NULL,
    extra_km_cost    DECIMAL(10,2)  NOT NULL DEFAULT 0.00,
    late_return_cost DECIMAL(10,2)  NOT NULL DEFAULT 0.00,
    damage_cost      DECIMAL(10,2)  NOT NULL DEFAULT 0.00,
    discount_percent DECIMAL(5,2)   NOT NULL DEFAULT 0.00,
    total_cost       DECIMAL(10,2)  NOT NULL,
    deposit_paid     DECIMAL(10,2)  NOT NULL DEFAULT 0.00,
    deposit_returned BOOLEAN        NOT NULL DEFAULT FALSE,

    -- Status
    status           ENUM('aktywna','zakonczona','anulowana','przeterminowana') NOT NULL DEFAULT 'aktywna',
    payment_status   ENUM('oczekuje','oplacona','czesciowa','zalegla') NOT NULL DEFAULT 'oczekuje',
    payment_method   ENUM('gotowka','karta','przelew') NULL,

    notes            TEXT           NULL,
    created_at       TIMESTAMP      DEFAULT CURRENT_TIMESTAMP,
    updated_at       TIMESTAMP      DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    CONSTRAINT fk_rentals_customer
        FOREIGN KEY (customer_id) REFERENCES customers(customer_id)
        ON UPDATE CASCADE ON DELETE RESTRICT,
    CONSTRAINT fk_rentals_car
        FOREIGN KEY (car_id) REFERENCES cars(car_id)
        ON UPDATE CASCADE ON DELETE RESTRICT,
    CONSTRAINT fk_rentals_user
        FOREIGN KEY (user_id) REFERENCES users(user_id)
        ON UPDATE CASCADE ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE INDEX idx_rentals_status ON rentals(status);
CREATE INDEX idx_rentals_dates ON rentals(date_start, date_end_planned);
CREATE INDEX idx_rentals_customer ON rentals(customer_id);
CREATE INDEX idx_rentals_car ON rentals(car_id);

-- ============================================================
-- Triggers
-- ============================================================
DELIMITER //

CREATE TRIGGER trg_rental_before_insert
BEFORE INSERT ON rentals
FOR EACH ROW
BEGIN
    DECLARE next_num INT;
    SELECT COALESCE(MAX(rental_id), 0) + 1 INTO next_num FROM rentals;
    IF NEW.rental_number IS NULL OR NEW.rental_number = '' THEN
        SET NEW.rental_number = CONCAT('WYP-', YEAR(NOW()), '-', LPAD(next_num, 6, '0'));
    END IF;
END//

-- trg_rental_after_insert and trg_rental_after_update removed:
-- car status transitions are handled in application code (AddRentalAsync, ReturnRentalAsync, CancelRentalAsync)
-- to avoid redundant updates and potential conflicts.

DELIMITER ;

-- ============================================================
-- Views
-- Column aliases match DatabaseService.cs reader field names:
--   category_name, daily_rate, deposit_amount  (GetCarsAsync)
--   active_rentals                              (GetCustomersAsync)
--   customer_name, customer_phone,
--   car_name, car_registration, employee_name   (GetRentalsAsync)
-- ============================================================

-- Cars with category join (use instead of SELECT * FROM cars in GetCarsAsync)
CREATE OR REPLACE VIEW vw_cars_full AS
SELECT
    c.*,
    cat.name AS category_name,
    cat.daily_rate,
    cat.deposit_amount
FROM cars c
JOIN categories cat ON c.category_id = cat.category_id;

-- Customers with active rental count (use in GetCustomersAsync)
CREATE OR REPLACE VIEW vw_customers_full AS
SELECT
    cu.*,
    COALESCE((SELECT COUNT(*) FROM rentals r
              WHERE r.customer_id = cu.customer_id AND r.status = 'aktywna'), 0) AS active_rentals
FROM customers cu;

-- Rentals with all joined names (use in GetRentalsAsync)
CREATE OR REPLACE VIEW vw_rentals_full AS
SELECT
    r.*,
    CONCAT(cu.first_name, ' ', cu.last_name) AS customer_name,
    cu.phone AS customer_phone,
    CONCAT(c.brand, ' ', c.model) AS car_name,
    c.registration AS car_registration,
    u.full_name AS employee_name
FROM rentals r
JOIN customers cu ON r.customer_id = cu.customer_id
JOIN cars c ON r.car_id = c.car_id
JOIN users u ON r.user_id = u.user_id;

-- Active rentals with days remaining
CREATE OR REPLACE VIEW vw_active_rentals AS
SELECT vr.*, DATEDIFF(vr.date_end_planned, NOW()) AS days_remaining
FROM vw_rentals_full vr
WHERE vr.status = 'aktywna';

-- Available cars
CREATE OR REPLACE VIEW vw_available_cars AS
SELECT
    c.car_id, c.brand, c.model, c.year, c.registration,
    c.color, c.fuel_type, c.transmission, c.seats, c.mileage_km,
    cat.name AS category_name, cat.daily_rate, cat.deposit_amount
FROM cars c
JOIN categories cat ON c.category_id = cat.category_id
WHERE c.status = 'dostepny' AND c.is_active = TRUE AND cat.is_active = TRUE;

-- Fleet stats by category
CREATE OR REPLACE VIEW vw_fleet_stats AS
SELECT
    cat.name AS category_name,
    COUNT(c.car_id) AS total_vehicles,
    SUM(CASE WHEN c.status = 'dostepny' THEN 1 ELSE 0 END) AS available,
    SUM(CASE WHEN c.status = 'wypozyczony' THEN 1 ELSE 0 END) AS rented,
    SUM(CASE WHEN c.status = 'serwis' THEN 1 ELSE 0 END) AS in_service,
    cat.daily_rate
FROM cars c
JOIN categories cat ON c.category_id = cat.category_id
WHERE c.is_active = TRUE
GROUP BY cat.category_id, cat.name, cat.daily_rate;