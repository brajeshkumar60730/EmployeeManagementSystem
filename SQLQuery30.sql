
CREATE TABLE Employees (
    EmployeeId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) ,
    DateOfBirth DATE,
    Email NVARCHAR(100),
    PhoneNumber NVARCHAR(15),
    Gender NVARCHAR(10),
    Country NVARCHAR(50),
    ProfileImage NVARCHAR(200),
    CreatedAt DATETIME DEFAULT GETDATE()
);


CREATE or alter PROCEDURE sp_GetEmployees  
AS  
BEGIN  
    SELECT * FROM Employees  
END  



CREATE or alter  PROCEDURE sp_InsertEmployee  
    @FullName NVARCHAR(100),  
    @DateOfBirth DATE,  
    @Email NVARCHAR(100),  
    @PhoneNumber NVARCHAR(20),  
    @Gender NVARCHAR(10),  
    @Country NVARCHAR(50),  
    @ProfileImage NVARCHAR(200),  
    @NewId INT OUTPUT  
AS  
BEGIN  
    INSERT INTO Employees (FullName, DateOfBirth, Email, PhoneNumber, Gender, Country, ProfileImage, CreatedAt)  
    VALUES (@FullName, @DateOfBirth, @Email, @PhoneNumber, @Gender, @Country, @ProfileImage, GETDATE())  
  
    SET @NewId = SCOPE_IDENTITY()  
END  



CREATE or alter PROCEDURE sp_GetEmployeeById  
(  
    @EmployeeId INT  
)  
AS  
BEGIN  
    SELECT * FROM Employees WHERE EmployeeId = @EmployeeId  
END  




CREATE or alter PROCEDURE sp_UpdateEmployee  
(  
    @EmployeeId INT,  
    @FullName NVARCHAR(100),  
    @DateOfBirth DATE,  
    @Email NVARCHAR(100),  
    @PhoneNumber NVARCHAR(15),  
    @Gender NVARCHAR(10),  
    @Country NVARCHAR(50),  
    @ProfileImage NVARCHAR(200)  
)  
AS  
BEGIN  
    UPDATE Employees SET  
        FullName=@FullName,  
        DateOfBirth=@DateOfBirth,  
        Email=@Email,  
        PhoneNumber=@PhoneNumber,  
        Gender=@Gender,  
        Country=@Country,  
        ProfileImage=@ProfileImage  
    WHERE EmployeeId=@EmployeeId  
END  


CREATE or alter PROCEDURE sp_DeleteEmployee  
(  
    @EmployeeId INT  
)  
AS  
BEGIN  
    DELETE FROM Employees WHERE EmployeeId=@EmployeeId  
END  