truncate table document;
truncate table documentsign;

insert into [User](Username, Fullname, Email, Password, EmployeeID, DepartmentID, PositionID)
values ('tunguyen', 'Nguyen Van Tuyen', 'tuyen.nguyenvan@mmcv.mektec.com', 'anhtuyen1997', 'MV220770', 1, 1);
insert into [User](Username, Fullname, Email, Password, EmployeeID, DepartmentID, PositionID)
values ('phamvithanh', 'Pham Vi Thanh', 'thanh.phamvi@mmcv.mektec.com', 'test', 'MV220829', 1, 1);
insert into [User](Username, Fullname, Email, Password, EmployeeID, DepartmentID, PositionID)
values ('test', 'test', 'test@mmcv.mektec.com', 'test', 'MV000000', 1, 1);
insert into [User](Username, Fullname, Email, Password, EmployeeID, DepartmentID, PositionID)
values ('test1', 'test1', 'test1@mmcv.mektec.com', 'test1', 'MV000001', 1, 1);

insert into DocumentType (DocumentTypeName, Description)
values ('Labor Contract', 'Labor Contract');
insert into DocumentType (DocumentTypeName, Description)
values ('Other', 'Other');

insert into Department(DepartmentName) values ('IT');
insert into Department(DepartmentName) values ('Other');

insert into Position(PositionName) values ('Technician');
insert into Position(PositionName) values ('Staff');
insert into Position(PositionName) values ('Leader');
insert into Position(PositionName) values ('Supervisor');
insert into Position(PositionName) values ('Manager');


select * from [User];
select * from document;
select * from documentsign;
select * from UserSignature;
select * from Department;
select * from Position;

declare @me int = 0;
declare @sent int = 0;
declare @completed int = 0;

SELECT @me = count(1)
FROM Document d
JOIN DocumentSign ds ON ds.DocumentID = d.DocumentID
WHERE ds.Email = 'tuyen.nguyenvan@mmcv.mektec.com'
AND d.[Status] IN (0, 1);

SELECT @sent = count(1)
FROM Document d
WHERE d.Issuer = 'tuyen.nguyenvan@mmcv.mektec.com' 
AND d.[Status] != 4;

SELECT @completed = count(1)
FROM Document d
JOIN DocumentSign ds ON ds.DocumentID = d.DocumentID
WHERE ds.Email = 'tuyen.nguyenvan@mmcv.mektec.com'
AND d.[Status] = 1 
AND ds.[Status] = 1;

SELECT @me as MeSign, @sent as [Sent], @completed as Completed;

exec USP_Dashboard_Count @Email = 'tuyen.nguyenvan@mmcv.mektec.com';

select * from DocumentSign where DocumentID = 8

SELECT u.*, d.DepartmentName, p.PositionName 
FROM [User] u
LEFT JOIN Department d ON d.DepartmentID = u.DepartmentID
LEFT JOIN Position p ON p.PositionID = u.PositionID