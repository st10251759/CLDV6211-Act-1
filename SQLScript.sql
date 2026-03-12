-- 1. CREATE DATABASE
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'SnackMVCAppDb')
BEGIN
    CREATE DATABASE SnackMVCAppDb;
END
GO

-- 2. USE DATABASE
USE SnackMVCAppDb;
GO


-- 3. CREATE SNACKS TABLE
CREATE TABLE Snacks (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Brand NVARCHAR(100) NOT NULL,
    Type INT NOT NULL,  -- Enum as INT (0=Chocolate,1=Drink,etc.)
    Description NVARCHAR(200) NOT NULL,
    Rating INT NOT NULL CHECK (Rating >= 1 AND Rating <= 5),
    Price DECIMAL(10,2) NOT NULL CHECK (Price >= 0 AND Price <= 1000),
    CreatedDate DATETIME2 DEFAULT GETDATE()
);
GO

-- 4. INSERT 15 SAMPLE RECORDS
INSERT INTO Snacks (Name, Brand, Type, Description, Rating, Price) VALUES
-- Chocolate (Type=0)
('TopDeck', 'Cadbury', 0, 'Layers of white and milk chocolate', 5, 18.00),
('Dairy Milk', 'Cadbury', 0, 'Classic milk chocolate bar', 4, 22.50),
('Crunchie', 'Cadbury', 0, 'Honeycomb toffee covered in chocolate', 4, 15.00),

-- Drink (Type=1)
('Bubble Tea', 'KungFu Tea', 1, 'Sweet tea with tapioca pearls', 4, 35.00),
('Coke Can', 'Coca-Cola', 1, 'Classic carbonated soft drink', 3, 12.00),
('Red Bull', 'Red Bull', 1, 'Energy drink with taurine', 4, 28.50),

-- Chips (Type=2)
('Doritos', 'Doritos', 2, 'Nachos cheese flavored tortilla chips', 5, 20.00),
('Pringles', 'Pringles', 2, 'Original sour cream & onion crisps', 4, 25.00),
('Nik Naks', 'Simba', 2, 'Spicy hot chili crisps', 4, 18.50),

-- ProteinSnack (Type=3)
('Protein Bar', 'USN', 3, 'Chocolate protein bar 20g protein', 4, 45.00),
('Whey Shake', 'Optimum Nutrition', 3, 'Vanilla whey protein powder', 5, 550.00),

-- FastFood (Type=4)
('McFlurry', 'McDonalds', 4, 'Oreo ice cream dessert', 5, 32.00),
('Big Mac', 'McDonalds', 4, 'Two beef patties special sauce', 4, 65.00),

-- Candy (Type=5)
('Jelly Tots', 'Allan Gray', 5, 'Colorful fruit flavored candies', 3, 10.50),
('Fizzer', 'Beacon', 5, 'Fizzy sherbet filled candy', 4, 12.00);

