USE PharmacyDB;
GO

IF OBJECT_ID('addMed', 'P') IS NOT NULL DROP PROCEDURE addMed;
IF OBJECT_ID('getAllMeds', 'P') IS NOT NULL DROP PROCEDURE getAllMeds;
IF OBJECT_ID('updateMed', 'P') IS NOT NULL DROP PROCEDURE updateMed;
IF OBJECT_ID('recSale', 'P') IS NOT NULL DROP PROCEDURE recSale;
GO

CREATE PROCEDURE addMed
    @Name VARCHAR(100),
    @Category VARCHAR(100),
    @Price DECIMAL(10, 2),
    @Quantity INT
AS
BEGIN
    INSERT INTO Medicines (Name, Category, Price, Quantity)
    VALUES (@Name, @Category, @Price, @Quantity);
END
GO

CREATE PROCEDURE getAllMeds
    @SearchTerm VARCHAR(100) = ''
AS
BEGIN
    IF @SearchTerm = '' OR @SearchTerm IS NULL
    BEGIN
        SELECT * FROM Medicines;
    END
    ELSE
    BEGIN
        SELECT * FROM Medicines
        WHERE Name LIKE '%' + @SearchTerm + '%'
        OR Category LIKE '%' + @SearchTerm + '%';
    END
END
GO

CREATE PROCEDURE updateMed
    @Name VARCHAR(100),
    @Category VARCHAR(100),
    @Price DECIMAL(10, 2),
    @Quantity INT,
    @ID INT
AS
BEGIN
    UPDATE Medicines
    SET Name = @Name,
        Category = @Category,
        Price = @Price,
        Quantity = @Quantity
    WHERE MedicineID = @ID;
END
GO

CREATE PROCEDURE recSale
    @MedicineID INT,
    @QuantitySold INT,
    @SaleDate DATETIME
AS
BEGIN
    BEGIN TRANSACTION;
    
    INSERT INTO Sales (MedicineID, QuantitySold, SaleDate)
    VALUES (@MedicineID, @QuantitySold, @SaleDate);
    
    UPDATE Medicines
    SET Quantity = Quantity - @QuantitySold
    WHERE MedicineID = @MedicineID;
    
    COMMIT TRANSACTION;
END
GO