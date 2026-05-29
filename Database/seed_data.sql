-- ============================================================
-- Seed Data for Car Rental System (MariaDB)
-- Uses cars (car_id), rentals (car_id) to match DatabaseService.cs
-- ============================================================

USE CarRentalDB;

SET FOREIGN_KEY_CHECKS = 0;

TRUNCATE TABLE maintenance;
TRUNCATE TABLE fuel_prices;
TRUNCATE TABLE rentals;
TRUNCATE TABLE cars;
TRUNCATE TABLE categories;
TRUNCATE TABLE customers;
TRUNCATE TABLE users;

SET FOREIGN_KEY_CHECKS = 1;

-- ============================================================
-- Fuel prices (PLN per litre; per kWh for 'elektryczny')
-- ============================================================
INSERT INTO fuel_prices (fuel_type, price_per_unit, unit) VALUES
('benzyna',     6.50, 'l'),
('diesel',      6.70, 'l'),
('LPG',         3.00, 'l'),
('hybryda',     6.50, 'l'),
('elektryczny', 1.20, 'kWh');

-- ============================================================
-- Categories
-- ============================================================
INSERT INTO categories (name, description, daily_rate, weekend_rate, weekly_rate, deposit_amount, mileage_limit, extra_km_rate) VALUES
('Ekonomiczny',  'Małe, oszczędne auta miejskie',        89.00,  99.00,  530.00,  500.00,  200, 0.50),
('Kompaktowy',   'Średniej wielkości, uniwersalne',       129.00, 149.00, 770.00,  800.00,  250, 0.60),
('Komfortowy',   'Sedany i kombi klasy średniej',        179.00, 209.00, 1070.00, 1200.00, 300, 0.70),
('SUV',          'Samochody terenowe i crossovery',      219.00, 259.00, 1310.00, 1500.00, 300, 0.80),
('Premium',      'Auta klasy wyższej i luksusowe',       349.00, 399.00, 2090.00, 3000.00, 200, 1.50),
('Dostawczy',    'Samochody dostawcze do 3.5t',          159.00, 179.00, 950.00,  1000.00, 300, 0.60),
('Minibus',      'Busy 7-9 osobowe',                    199.00, 229.00, 1190.00, 1500.00, 300, 0.70);

-- ============================================================
-- Users  (password: Admin123! — BCrypt hash)
-- ============================================================
INSERT INTO users (username, password_hash, full_name, email, phone, role) VALUES
('admin',          '$2a$12$H/5XX1pJjO2a63dxK3DmqeKrFkkJd2ikHcp6Z6OOxOUILfyuLoUQS', 'Administrator Systemu', 'admin@wynajem.pl',  '500000000', 'admin'),
('anna.kowalska',  '$2a$12$H/5XX1pJjO2a63dxK3DmqeKrFkkJd2ikHcp6Z6OOxOUILfyuLoUQS', 'Anna Kowalska',        'anna@wynajem.pl',   '500000001', 'employee'),
('jan.nowak',      '$2a$12$H/5XX1pJjO2a63dxK3DmqeKrFkkJd2ikHcp6Z6OOxOUILfyuLoUQS', 'Jan Nowak',             'jan@wynajem.pl',    '500000002', 'employee');

-- ============================================================
-- Cars  (car_id auto-incremented 1–14)
-- ============================================================
INSERT INTO cars (category_id, brand, model, year, registration, vin, color, fuel_type, transmission, seats, mileage_km, status, insurance_expiry, inspection_expiry) VALUES
-- Ekonomiczne (category_id = 1)  →  car_id 1,2,3
(1, 'Toyota',     'Yaris',       2024, 'TKI 1001', 'JTDKN3DU5A0000001', 'Biały',    'benzyna',     'manualna',     5, 15200, 'dostepny',     '2027-01-15', '2026-11-20'),
(1, 'Fiat',       '500',         2023, 'TKI 1002', 'JTDKN3DU5A0000002', 'Czerwony', 'benzyna',     'manualna',     4, 22100, 'dostepny',     '2026-12-01', '2026-09-15'),
(1, 'Skoda',      'Fabia',       2024, 'TKI 1003', 'JTDKN3DU5A0000003', 'Szary',    'benzyna',     'manualna',     5, 8900,  'dostepny',     '2027-03-10', '2027-01-05'),
-- Kompaktowe (category_id = 2)  →  car_id 4,5,6
(2, 'Volkswagen', 'Golf',        2024, 'TKI 2001', 'JTDKN3DU5A0000004', 'Czarny',   'benzyna',     'manualna',     5, 12300, 'dostepny',     '2027-02-20', '2026-12-10'),
(2, 'Toyota',     'Corolla',     2023, 'TKI 2002', 'JTDKN3DU5A0000005', 'Srebrny',  'hybryda',     'automatyczna', 5, 28700, 'dostepny',     '2026-11-30', '2026-10-15'),
(2, 'Mazda',      '3',           2024, 'TKI 2003', 'JTDKN3DU5A0000006', 'Czerwony', 'benzyna',     'manualna',     5, 5400,  'dostepny',     '2027-04-05', '2027-02-28'),
-- Komfortowe (category_id = 3)  →  car_id 7,8
(3, 'Skoda',      'Octavia',     2024, 'TKI 3001', 'JTDKN3DU5A0000007', 'Granatowy','diesel',      'automatyczna', 5, 31200, 'dostepny',     '2027-01-25', '2026-11-30'),
(3, 'Toyota',     'Camry',       2023, 'TKI 3002', 'JTDKN3DU5A0000008', 'Czarny',   'hybryda',     'automatyczna', 5, 19800, 'wypozyczony',  '2026-12-15', '2026-10-01'),
-- SUV (category_id = 4)  →  car_id 9,10
(4, 'Hyundai',    'Tucson',      2024, 'TKI 4001', 'JTDKN3DU5A0000009', 'Biały',    'hybryda',     'automatyczna', 5, 14500, 'dostepny',     '2027-03-01', '2027-01-15'),
(4, 'Toyota',     'RAV4',        2023, 'TKI 4002', 'JTDKN3DU5A0000010', 'Szary',    'hybryda',     'automatyczna', 5, 35200, 'serwis',       '2026-11-10', '2026-09-20'),
-- Premium (category_id = 5)  →  car_id 11
(5, 'BMW',        'Seria 5',     2024, 'TKI 5001', 'JTDKN3DU5A0000011', 'Czarny',   'diesel',      'automatyczna', 5, 8200,  'dostepny',     '2026-05-01', '2027-02-15'),
-- Dostawcze (category_id = 6)  →  car_id 12,13
(6, 'Fiat',       'Ducato',      2023, 'TKI 6001', 'JTDKN3DU5A0000012', 'Biały',    'diesel',      'manualna',     3, 45200, 'dostepny',     '2026-12-20', '2026-10-30'),
(6, 'Renault',    'Master',      2024, 'TKI 6002', 'JTDKN3DU5A0000013', 'Biały',    'diesel',      'manualna',     3, 12300, 'dostepny',     '2027-05-15', '2027-03-10'),
-- Minibus (category_id = 7)  →  car_id 14
(7, 'Volkswagen', 'Transporter', 2024, 'TKI 7001', 'JTDKN3DU5A0000014', 'Srebrny',  'diesel',      'manualna',     9, 21000, 'dostepny',     '2027-02-10', '2026-12-20');

-- ============================================================
-- Customers
-- ============================================================
INSERT INTO customers (first_name, last_name, pesel, id_document, id_type, license_number, license_expiry, email, phone, address_street, address_city, address_zip, date_of_birth) VALUES
('Marek',     'Wiśniewski',  '85050512345', 'ABC 123456', 'dowod',       'WI/12345/2020', '2030-05-05', 'marek.w@email.pl',  '600100200', 'ul. Lipowa 15',      'Kielce', '25-001', '1985-05-05'),
('Katarzyna', 'Zielińska',   '90071567890', 'DEF 789012', 'dowod',       'KA/67890/2021', '2031-07-15', 'kasia.z@email.pl',  '600200300', 'ul. Sienkiewicza 8', 'Kielce', '25-002', '1990-07-15'),
('Piotr',     'Kowalczyk',   '78032298765', 'GHI 345678', 'paszport',    'PI/34567/2019', '2029-03-22', 'piotr.k@email.pl',  '600300400', 'ul. Krakowska 42',   'Kielce', '25-003', '1978-03-22'),
('Agnieszka', 'Lewandowska', '95120143210', 'JKL 901234', 'dowod',       'AG/90123/2022', '2032-12-01', 'aga.l@email.pl',    '600400500', 'ul. Warszawska 3',   'Kielce', '25-004', '1995-12-01'),
('Tomasz',    'Wójcik',      '82091587654', 'MNO 567890', 'prawo_jazdy', 'TO/56789/2018', '2028-09-15', 'tomek.w@email.pl',  '600500600', 'ul. Bodzentyńska 7', 'Kielce', '25-005', '1982-09-15');

-- ============================================================
-- Rentals  (car_id references cars.car_id)
-- ============================================================
INSERT INTO rentals (rental_number, customer_id, car_id, user_id, date_start, date_end_planned, date_end_actual, mileage_start, mileage_end, daily_rate, total_days, base_cost, extra_km_cost, late_return_cost, damage_cost, discount_percent, total_cost, deposit_paid, deposit_returned, status, payment_status, payment_method) VALUES
-- Active: Katarzyna → Toyota Camry (car_id=8), by Anna (user_id=2)
('WYP-2026-000001', 2, 8, 2,
 '2026-03-25 10:00:00', '2026-03-30 10:00:00', NULL,
 19800, NULL,
 179.00, 5, 895.00, 0.00, 0.00, 0.00, 0.00, 895.00,
 1200.00, FALSE,
 'aktywna', 'oplacona', 'karta'),

-- Finished: Marek → VW Golf (car_id=4), by Anna (user_id=2)
('WYP-2026-000002', 1, 4, 2,
 '2026-03-20 09:00:00', '2026-03-23 09:00:00', '2026-03-23 11:00:00',
 12300, 12720,
 129.00, 3, 387.00, 0.00, 0.00, 0.00, 0.00, 387.00,
 800.00, TRUE,
 'zakonczona', 'oplacona', 'gotowka'),

-- Finished: Piotr → Hyundai Tucson (car_id=9), by Jan (user_id=3)
('WYP-2026-000003', 3, 9, 3,
 '2026-03-15 08:00:00', '2026-03-18 08:00:00', '2026-03-18 09:30:00',
 14500, 14980,
 219.00, 3, 657.00, 0.00, 0.00, 0.00, 0.00, 657.00,
 1500.00, TRUE,
 'zakonczona', 'oplacona', 'przelew'),

-- Finished with outstanding payment: Tomasz → BMW Seria 5 (car_id=11), by Anna (user_id=2)
('WYP-2026-000004', 5, 11, 2,
 '2026-03-10 10:00:00', '2026-03-12 10:00:00', '2026-03-12 16:00:00',
 8200, 8540,
 349.00, 2, 698.00, 0.00, 0.00, 0.00, 0.00, 698.00,
 3000.00, TRUE,
 'zakonczona', 'zalegla', 'przelew');

-- ============================================================
-- Fuel consumption (L/100km, or kWh/100km for electric) by fuel type
-- and purchase price (PLN) by category — used by cost/recommender algorithms
-- ============================================================
UPDATE cars SET fuel_consumption = CASE fuel_type
    WHEN 'benzyna'     THEN 7.50
    WHEN 'diesel'      THEN 6.00
    WHEN 'LPG'         THEN 9.50
    WHEN 'hybryda'     THEN 4.50
    WHEN 'elektryczny' THEN 16.00
    ELSE 7.50 END;

UPDATE cars SET purchase_price = CASE category_id
    WHEN 1 THEN 70000.00    -- Ekonomiczny
    WHEN 2 THEN 100000.00   -- Kompaktowy
    WHEN 3 THEN 140000.00   -- Komfortowy
    WHEN 4 THEN 180000.00   -- SUV
    WHEN 5 THEN 320000.00   -- Premium
    WHEN 6 THEN 150000.00   -- Dostawczy
    WHEN 7 THEN 200000.00   -- Minibus
    ELSE 100000.00 END;

-- ============================================================
-- Maintenance log (sample records)
-- ============================================================
INSERT INTO maintenance (car_id, type, date, cost, odometer_km, description) VALUES
(10, 'serwis',   '2026-03-05', 1250.00, 35000, 'Przegląd okresowy + wymiana oleju i filtrów'),
(10, 'naprawa',  '2026-03-06',  680.00, 35050, 'Wymiana klocków hamulcowych przód'),
(8,  'przeglad', '2026-02-20',  320.00, 19500, 'Badanie techniczne'),
(4,  'serwis',   '2026-01-15',  540.00, 11800, 'Wymiana oleju, filtr kabinowy'),
(11, 'naprawa',  '2026-03-12', 2100.00,  8500, 'Naprawa zawieszenia');