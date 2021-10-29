/* 
* Caution : This Script Will drop the database and recreate it.
* Author : hany samir at 07/09/2021.
*/
if db_id(N'EInvoice_Test') is not null
	drop database [EInvoice_Test]
go
create database [EInvoice_Test]
go
use [EInvoice_Test]
go
set nocount on;
go
if object_id(N'[dbo].[Report]','U') is not null
	drop table [dbo].[Report]
go
create table [dbo].Report(
	[Id] int not null identity(1,1) primary key,
	[Name] nvarchar(50) not null,
	[Description] nvarchar(500) not null
);
go
if object_id(N'[dbo].[ActivityCode]','U') is not null
	drop table [dbo].[ActivityCode];
go
create table [dbo].[ActivityCode](
	[Code] nvarchar(4) not null,
	[EnglishDescription] nvarchar(200) null,
	[ArabicDescription] nvarchar(200) null,
	constraint PK_ACTIVITYCODE primary key([Code])
);
go
if object_id(N'[dbo].[InsertActivityCode]','P') is not null
	drop procedure [dbo].[InsertActivityCode];
go
create procedure [dbo].[InsertActivityCode]
@Code as nvarchar(4),
@EnglishDescription as nvarchar(200),
@ArabicDescription as nvarchar(200)
as
begin
	insert into [dbo].ActivityCode(Code,EnglishDescription,ArabicDescription)
	values (@Code,@EnglishDescription,@ArabicDescription);
end
go
if object_id(N'[dbo].[InsertCountryCode]','P') is not null
	drop procedure [dbo].[InsertCountryCode];
go
create procedure [dbo].[InsertCountryCode]
@Code as nvarchar(10),
@ArabicDescription as nvarchar(200),
@EnglishDescription as nvarchar(200)
as
begin
	insert into [dbo].[CountryCode](Code,ArabicDescription,EnglishDescription)
	values (@Code,@ArabicDescription,@EnglishDescription);
end
go
if object_id(N'[dbo].[GetAllActivityCodes]','P') is not null
	drop procedure [dbo].[GetAllActivityCodes];
go
create procedure [dbo].[GetAllActivityCodes]
as
begin
	select ActivityCode.Code
	,		ActivityCode.EnglishDescription
	,		ActivityCode.ArabicDescription
	from [dbo].ActivityCode
end
go
if object_id(N'[dbo].[CountryCode]','U') is not null
	drop table [dbo].[CountryCode];
go
create table [dbo].[CountryCode](
	[Code] nvarchar(10) not null ,
	[EnglishDescription] nvarchar(200) null,
	[ArabicDescription] nvarchar(200) null,
	constraint PK_CountryCode primary key([Code])
);
go
if object_id(N'[dbo].[TaxType]','U') is not null
	drop table [dbo].[TaxType];
go
create table [dbo].[TaxType](
	[Code] nvarchar(10) not null,
	[EnglishDescription] nvarchar(200),
	[ArabicDescription] nvarchar(200),
	constraint PK_TaxType primary key ([Code])
);
go
if object_id(N'[dbo].[TaxSubType]','U') is not null
	drop table [dbo].[TaxSubType];
go
create table [dbo].[TaxSubType](
	[Code] nvarchar(10) not null,
	[EnglishDescription] nvarchar(200) null,
	[ArabicDescription] nvarchar(200) null,
	[TaxTypeCode] nvarchar(10) not null,
	constraint PK_TaxSubType primary key ([Code])
);
go
if object_id(N'[dbo].[APIEnvironment]','U') is not null
	drop table [dbo].[APIEnvironment];
go
create table [dbo].[APIEnvironment](
	[Id] int not null identity(1,1),
	[Name] nvarchar(50) not null,
	[LogInUri] nvarchar(100) not null,
	[BaseUri] nvarchar(100) not null,
	[VerCol] rowversion,
	constraint PK_APIEnvironment primary key (Id),
	constraint UNQ_APIEnvironment_Name unique ([Name])
);
go
if object_id(N'[dbo].[Taxpayer]','U') is not null
	drop table [dbo].[Taxpayer];
go
create table [dbo].[TaxPayer](
	[Id] nvarchar(30) not null check(len([Id]) >= 1),
	[Type] nvarchar(2) default 'B',
	[Name] nvarchar(200) not null check (len([Name])>=1),
	-- TaxPayer Address
	[BranchId] nvarchar(50) null,
	[Country] nvarchar(2) not null check (len([Country])>=1),
	[Governate] nvarchar(100) not null check(len([Governate]) >= 1),
	[RegionCity] nvarchar(100) not null check(len([RegionCity]) >= 1),
	[Street] nvarchar(200) not null check (len([Street]) >= 1),
	[BuildingNumber] nvarchar(100) not null check (len([BuildingNumber]) >= 1),
	[PostalCode] nvarchar(30) null default '',
	[Floor] nvarchar(100) null default '',
	[Room] nvarchar(100) null default '',
	[Landmark] nvarchar(500) null default '',
	[AdditionalInformation] nvarchar(500) null default '',
	[VerCol] rowversion,
	constraint PK_TaxPayer primary key ([Id])
);
go
if object_id(N'[dbo].[GetAllTaxpayers]','P') is not null
	drop procedure [dbo].[GetAllTaxpayers];
go
create procedure [dbo].[GetAllTaxpayers]
as
begin
	select  [dbo].[TaxPayer].Id
	,		[dbo].[TaxPayer].Name
	,		[dbo].[TaxPayer].[Type]
	,		[dbo].[TaxPayer].BranchId
	,		[dbo].[TaxPayer].BuildingNumber
	,		[dbo].[TaxPayer].AdditionalInformation
	,		[dbo].[TaxPayer].Country
	,		[dbo].[TaxPayer].[Floor]
	,		[dbo].[TaxPayer].Governate
	,		[dbo].[TaxPayer].Landmark
	,		[dbo].[TaxPayer].PostalCode
	,		[dbo].[TaxPayer].RegionCity
	,		[dbo].[TaxPayer].Room
	,		[dbo].[TaxPayer].Street
	,		[dbo].[TaxPayer].VerCol
	from [dbo].[TaxPayer]
end
go
if object_id(N'[dbo].[TaxPayerAPIEnvAccessDetails]','U') is not null
	drop table [dbo].[TaxPayerAPIEnvAccessDetails]
go
create table [dbo].[TaxPayerAPIEnvAccessDetails](
	TaxPayerId nvarchar(30) not null,
	APIEnvironmentId int not null,
	ClientId nvarchar(50) not null,
	ClientSecret nvarchar(50) not null,
	SecurityToken nvarchar(8) not null
);
go
if object_id(N'[dbo].[Customer]','U') is not null
	drop table [dbo].[Customer];
go
create table [dbo].[Customer](
	[CustomerId] int not null identity(1,1),
	[Type] nvarchar(1) not null check([Type] in ('B','P','F')),
	[Id] nvarchar(30) null,
	[Name] nvarchar(200) null,
	--Receiver Address
	[Country] nvarchar(2) null,
	[Governate] nvarchar(100) null,
	[RegionCity] nvarchar(100) null,
	[Street] nvarchar(200) null,
	[BuildingNumber] nvarchar(100) null,
	[PostalCode] nvarchar(30) null,
	[Floor] nvarchar(100) null,
	[Room] nvarchar(100) null,
	[Landmark] nvarchar(500) null,
	[AdditionalInformation] nvarchar(500) null,
	[VerCol] rowversion,
	constraint PK_Customer primary key ([CustomerId]),
	constraint UNQ_Customer_Type_Name unique ([Type],[Name])
);
go
if object_id(N'[dbo].[TaxableItem]','U') is not null
	drop table [dbo].[TaxableItem];
go
create table [dbo].[TaxableItem](
	[Id] int not null identity(1,1),
	[TaxType] nvarchar(30) not null,
	[SubType] nvarchar(50) not null,
	[Amount] decimal(28,5) not null default 0,
	[Rate] decimal(28,5) not null check([Rate]>=0 and [Rate]<=100) default 0,
	[InvoiceLineId] int not null,
	[VerCol] rowversion,
	constraint PK_TaxableItem primary key ([Id]),
	constraint UNQ_TaxableItem unique ([InvoiceLineId],[TaxType],[SubType])
);
go
alter table [dbo].[taxableItem] alter column [Rate] decimal(4,2);
go
if object_id(N'[dbo].[Document]','U') is not null
	drop table [dbo].[Document];
go
create table dbo.Document(
	[Id] int not null identity(1,1),
	--------------------------------------------------------------------- Document
	[DocumentType] nvarchar(20) not null check(len([DocumentType]) >= 1),
	[DocumentTypeVersion] nvarchar(100) not null check(len([DocumentTypeVersion]) >= 1),
	[DateTimeIssued] datetime not null check(DateTimeIssued <= CURRENT_TIMESTAMP),
	[TaxpayerActivityCode] nvarchar(10) not null check(len([TaxpayerActivityCode]) >= 1),
	[InternalId] nvarchar(50) not null check(len([InternalId]) >= 1),
	[PurchaseOrderReference] nvarchar(100) null default '',
	[PurchaseOrderDescription] nvarchar(500) null default '',
	[SalesOrderReference] nvarchar(100) null default '',
	[SalesOrderDescription] nvarchar(500) null default '',
	[ProformaInvoiceNumber] nvarchar(50) null default '',
	---------------------------------------------------------------- Payment
	[BankName] nvarchar(100) null default '',
	[BankAddress] nvarchar(500) null default '',
	[BankAccountNo] nvarchar(50) null default '',
	[BankAccountIBAN] nvarchar(50) null default '',
	[SwiftCode] nvarchar(50) null default '',
	[PaymentTerms] nvarchar(500) null default '',
	---------------------------------------------------------------- Delivery
	[Approach] nvarchar(100) null default '',
	[Packaging] nvarchar(100) null default '',
	[DateValidity] datetime null default GETDATE(),
	[ExportPort] nvarchar(100) null default '',
	[CountryOfOrigin] nvarchar(100) null default '',
	[GrossWeight] decimal(28,5) null check([GrossWeight]>=0) default 0,
	[NetWeight] decimal(28,5) null check([NetWeight]>=0) default 0,
	[DeliveryTerms] nvarchar(500) null default '',
	---------------------------------------------------------------------------- Docuument
	[TotalSalesAmount] decimal(28,5) not null default 0,
	[TotalDiscountAmount] decimal(28,5) not null default 0,
	[NetAmount] decimal(28,5) not null default 0,
	[ExtraDiscountAmount] decimal(28,5),
	[TotalItemsDiscountAmount] decimal(28,5) not null default 0,
	[TotalAmount] decimal(28,5) not null default 0,
	[TaxpayerId] nvarchar(30) not null,
	[CustomerId] int not null,
	[VerCol] rowversion,
	constraint PK_Document primary key([Id])
);
go
if object_id(N'[dbo].[InvoiceLine]','U') is not null
	drop table [dbo].[InvoiceLine];
go
create table [dbo].[InvoiceLine](
	[Id] int not null identity(1,1),
	-------InvoiceLine
	[Description] nvarchar(500) not null check(len([Description]) >= 1 ),
	[ItemType] nvarchar(30) not null check(len([ItemType]) >= 1),
	[ItemCode] nvarchar(100) not null check(len([ItemCode]) >= 1),
	[UnitType] nvarchar(30) not null check(len([UnitType]) >= 1),
	[Quantity] decimal(28,5) not null check([Quantity] > 0),
	--unit value
	[CurrencySold] nvarchar(3) not null check(len([CurrencySold])>=1),
	[AmountEGP] decimal(28,5) not null check([AmountEGP]>=0) default 0,
	[AmountSold] decimal(28,5) null,
	[CurrencyExchangeRate] decimal(28,5) null,
	---------InvoiceLine
	[SalesTotal] decimal(28,5) not null check([SalesTotal] >= 0) default 0,
	[Total] decimal(28,5) not null check([Total] >= 0) default 0,
	[ValueDifference] decimal(28,5) not null default 0,
	[TotalTaxableFees] decimal(28,5) not null check([TotalTaxableFees] >= 0) default 0,
	[NetTotal] decimal(28,5) not null check([NetTotal] >= 0) default 0,
	[ItemsDiscount] decimal(28,5) not null check([ItemsDiscount] >= 0) default 0,
	[DiscountRate] int null default 0,
	[DiscountAmount] decimal(28,5) not null default 0,
	[InternalCode] nvarchar(50) null default '',
	[DocumentId] int not null,
	[VerCol] rowversion,
	constraint PK_InvoiceLine primary key ([Id])
);
go
if object_id(N'[dbo].[DocumentSubmission]','U') is not null
	drop table [dbo].[DocumentSubmission];
go
create table dbo.DocumentSubmission(
	[APIEnvironmentId] int not null,
	[DocumentId] int not null,
	[UUID] nvarchar(200) not null,
	[SubmissionUUID] nvarchar(200) not null,
	[SubmissionDate] datetime default GETDATE(),
	[Status] nvarchar(10) check([Status] in ('Valid','Invalid','Submitted','Cancelled','')),
	[VerCol] rowversion
);
go
if object_id(N'[dbo].[GetDocumentSubmissionCount]','P') is not null
	drop procedure [dbo].[GetDocumentSubmissionCount];
go
create procedure [dbo].[GetDocumentSubmissionCount]
@EnvId int null,
@DocId int null,
@Count int output
as
begin
	select @Count = ISNULL(count(*),0)
	from dbo.DocumentSubmission
	where dbo.DocumentSubmission.APIEnvironmentId = @EnvId
	and dbo.DocumentSubmission.DocumentId = @DocId;
end
go
if object_id(N'[dbo].[SearchDocuments]','P') is not null
	drop procedure [dbo].[SearchDocuments];
go
create procedure [dbo].[SearchDocuments]
@issuanceDateFrom as datetime  = null,
@issuanceDateTo as datetime = null,
@submissionDateTo as datetime null,
@submissionDateFrom as datetime null,
@CustomerName as nvarchar(200) null = '%',
@TaxpayerId as nvarchar(50),
@APIEnvironmentId int,
@status nvarchar(50) null = '%',
@ProformaInvoiceNumber nvarchar(50) null = '%'
as
begin
	
	if(@submissionDateFrom is null)
		select @submissionDateFrom = min(submissiondate) from dbo.DocumentSubmission;
	if(@submissionDateTo is null)
		select @submissionDateTo = max(submissiondate) from dbo.DocumentSubmission;
	if(@issuanceDateFrom is null)
		select @issuanceDateFrom = min(DateTimeIssued) from dbo.Document;
	if(@issuanceDateTo is null)
		select @issuanceDateTo = max(DateTimeIssued) from dbo.Document;
	--documents
	select	[dbo].Document.Approach
	,		[dbo].Document.BankAccountIBAN
	,		[dbo].Document.BankAccountNo
	,		[dbo].Document.BankAddress
	,		[dbo].Document.BankName
	,		[dbo].Document.CountryOfOrigin
	,		[dbo].Document.CustomerId
	,		[dbo].Customer.AdditionalInformation as RECV_AdditionalInformation
	,		[dbo].Customer.BuildingNumber as RECV_BuildingNumber
	,		[dbo].Customer.Country as RECV_Country
	,		[dbo].Customer.[Floor] as RECV_Floor
	,		[dbo].Customer.Governate as RECV_Governate
	,		[dbo].Customer.Id as RECV_Id
	,		[dbo].Customer.Landmark as RECV_Landmark
	,		[dbo].Customer.Name as RECV_Name
	,		[dbo].Customer.PostalCode as RECV_PostalCode
	,		[dbo].Customer.RegionCity as RECV_RegionCity
	,		[dbo].Customer.Room as RECV_Room
	,		[dbo].Customer.Street as RECV_Street
	,		[dbo].Customer.[Type] as RECV_Type
	,		[dbo].Customer.VerCol as RECV_VerCol
	,		[dbo].Document.DateTimeIssued
	,		[dbo].Document.DateValidity
	,		[dbo].Document.DeliveryTerms
	,		[dbo].Document.DocumentType
	,		[dbo].Document.DocumentTypeVersion
	,		[dbo].Document.ExportPort
	,		[dbo].Document.ExtraDiscountAmount
	,		[dbo].Document.GrossWeight
	,		[dbo].Document.Id
	,		[dbo].Document.InternalId
	,		[dbo].Document.NetAmount
	,		[dbo].Document.NetWeight
	,		[dbo].Document.Packaging
	,		[dbo].Document.PaymentTerms
	,		[dbo].Document.ProformaInvoiceNumber
	,		[dbo].Document.PurchaseOrderDescription
	,		[dbo].Document.PurchaseOrderReference
	,		[dbo].Document.SalesOrderDescription
	,		[dbo].Document.SalesOrderReference
	,		[dbo].Document.SwiftCode
	,		[dbo].Document.TaxpayerActivityCode
	,		[dbo].Document.TaxpayerId
	,		[dbo].TaxPayer.AdditionalInformation as ISS_AdditionalInformation
	,		[dbo].TaxPayer.BranchId as ISS_BranchId
	,		[dbo].TaxPayer.BuildingNumber as ISS_BuildingNumber
	,		[dbo].TaxPayer.Country as ISS_Country
	,		[dbo].TaxPayer.[Floor] as ISS_Floor
	,		[dbo].TaxPayer.Governate as ISS_Governate
	,		[dbo].TaxPayer.Landmark as ISS_Landmark
	,		[dbo].TaxPayer.Name as ISS_Name
	,		[dbo].TaxPayer.PostalCode as ISS_PostalCode
	,		[dbo].TaxPayer.RegionCity as ISS_RegionCity
	,		[dbo].TaxPayer.Room as ISS_Room
	,		[dbo].TaxPayer.Street as ISS_Street
	,		[dbo].TaxPayer.[Type] as ISS_Type
	,		[dbo].Document.TotalAmount
	,		[dbo].Document.TotalDiscountAmount
	,		[dbo].Document.TotalItemsDiscountAmount
	,		[dbo].Document.TotalSalesAmount
	,		[dbo].Document.VerCol as doc_VerCol
	,		[dbo].[DocumentSubmission].APIEnvironmentId
	,		[dbo].[DocumentSubmission].[Status]
	,		[dbo].[DocumentSubmission].SubmissionDate
	,		[dbo].[DocumentSubmission].SubmissionUUID
	,		[dbo].[DocumentSubmission].UUID
	,		[dbo].[DocumentSubmission].VerCol docsubmission_vercol
	from [dbo].Document
	join dbo.Customer
		on [dbo].Document.CustomerId = dbo.Customer.CustomerId
	join [dbo].TaxPayer
		on dbo.Document.TaxpayerId = TaxPayer.Id
	left join [dbo].[DocumentSubmission]
		on dbo.Document.Id = dbo.DocumentSubmission.DocumentId
	where dbo.Document.DateTimeIssued between @issuanceDateFrom and @issuanceDateTo
	and dbo.Document.ProformaInvoiceNumber like @ProformaInvoiceNumber
	and [dbo].Customer.Name like @CustomerName
	and document.TaxpayerId = @TaxpayerId
	and [dbo].[DocumentSubmission].SubmissionDate between @submissionDateFrom and @submissionDateTo
	and dbo.DocumentSubmission.[Status] like @status
	and dbo.DocumentSubmission.APIEnvironmentId = @APIEnvironmentId
	order by [dbo].Document.ProformaInvoiceNumber;
end
go
if object_id(N'[dbo].[GetDocumentSubmissionByDocId_EnvId]','P') is not null
	drop procedure [dbo].[GetDocumentSubmissionByDocId_EnvId]
go
create procedure [dbo].[GetDocumentSubmissionByDocId_EnvId]
@EnvId int,
@DocId int
as
begin
	select  [DocumentId] as DOC_Id
	,		[APIEnvironmentId] as ENV_Id
	,		dbo.APIEnvironment.Name as ENV_Name
	,		dbo.APIEnvironment.BaseUri
	,		dbo.APIEnvironment.LogInUri
	,		dbo.APIEnvironment.VerCol as ENV_VerCol
	,		dbo.DocumentSubmission.SubmissionUUID
	,		dbo.DocumentSubmission.SubmissionDate
	,		dbo.DocumentSubmission.[Status]
	,		dbo.DocumentSubmission.UUID
	,		dbo.DocumentSubmission.VerCol as DOC_SUBMISSION_VerCol
	,		dbo.Document.Approach
	,		dbo.Document.BankAccountIBAN
	,		dbo.Document.BankAccountNo
	,		dbo.Document.BankAddress
	,		dbo.Document.BankName
	,		dbo.Document.CountryOfOrigin
	,		dbo.Document.CustomerId
	,		dbo.Document.DateTimeIssued
	from dbo.DocumentSubmission
	join dbo.APIEnvironment
		on dbo.DocumentSubmission.APIEnvironmentId = dbo.APIEnvironment.Id
	join dbo.Document
		on dbo.Document.Id = dbo.DocumentSubmission.DocumentId
	where APIEnvironmentId = @EnvId and DocumentId = @DocId;
end
go
if object_id(N'[dbo].[InsertDocumentSubmission]','P') is not null
	drop procedure [dbo].[InsertDocumentSubmission]
go
create procedure [dbo].[InsertDocumentSubmission]
@APIEnvironmentId as int,
@DocumentId as int,
@UUID as nvarchar(200),
@SubmissionUUID as nvarchar(200) = GETDATE,
@SubmissionDate as datetime,
@Status nvarchar(10) = '',
@VerCol varbinary(8) output
as
begin
	insert into [dbo].[DocumentSubmission](APIEnvironmentId,DocumentId,SubmissionUUID,UUID,SubmissionDate,[Status])
	values(@APIEnvironmentId,@DocumentId,@SubmissionUUID,@UUID,@SubmissionDate,@Status);
	set @VerCol = @@DBTS;
end
go
alter table [dbo].[DocumentSubmission]
add constraint PK_DocumentSubmission primary key ([APIEnvironmentId],[DocumentId]);
go
if object_id(N'[dbo].[SaveOrUpdateDocumentSubmission]','P') is not null
	drop procedure [dbo].[SaveOrUpdateDocumentSubmission];
go
create procedure [dbo].[SaveOrUpdateDocumentSubmission]
@APIEnvironmentId as int,
@DocumentId as int,
@UUID as nvarchar(200),
@SubmissionUUID nvarchar(200),
@SubmissionDate datetime,
@Status nvarchar(10),
@OldVerCol rowVersion  = null,
@NewVerCol rowVersion output
as
begin
	if exists(select 1 from dbo.DocumentSubmission where APIEnvironmentId = @APIEnvironmentId and DocumentId = @DocumentId)
	begin
		if @OldVerCol is not null
		update dbo.DocumentSubmission
		set UUID = @UUID,SubmissionUUID = @SubmissionUUID,SubmissionDate = @SubmissionDate,[Status] = @Status
		where APIEnvironmentId = @APIEnvironmentId and DocumentId = @DocumentId and VerCol = @OldVerCol;
		else
		update dbo.DocumentSubmission
		set UUID = @UUID,SubmissionUUID = @SubmissionUUID,SubmissionDate = @SubmissionDate,[Status] = @Status
		where APIEnvironmentId = @APIEnvironmentId and DocumentId = @DocumentId;
		if @@ROWCOUNT > 0
		begin
			set @NewVerCol = @@DBTS;
		end
		else
			if @OldVerCol is not null
				throw 50000,'Invalid Operation.',1;
	end
	else
	begin
		insert into [dbo].[DocumentSubmission](APIEnvironmentId,DocumentId,SubmissionUUID,UUID,SubmissionDate,[Status])
		values (@APIEnvironmentId,@DocumentId,@SubmissionUUID,@UUID,@SubmissionDate,@Status);
		set @NewVerCol = @@DBTS;
	end
end
go
if object_id(N'[dbo].[User]','U') is not null
	drop table [dbo].[User];
go
create table [dbo].[User](
	[Id] int not null identity(1,1) primary key,
	[Name] nvarchar(50) not null unique,
	[Password] varbinary(32) not null,
	[TaxPayerId] nvarchar(30) not null,
	[VerCol] rowversion not null
);
go

---------------------------------------------- Create Stored Procedures ------------------------------------
if object_id(N'[dbo].[GetAccessDetailsByIssuerId_APIId]','P') is not null
	drop procedure [dbo].[GetAccessDetailsByIssuerId_APIId];
go
create procedure [dbo].[GetAccessDetailsByIssuerId_APIId]
@TaxPayerId nvarchar(30),
@APIEnvId int
as
begin
	select  [dbo].[TaxPayerAPIEnvAccessDetails].ClientId
	,		[dbo].[TaxPayerAPIEnvAccessDetails].ClientSecret
	,		[dbo].[TaxPayerAPIEnvAccessDetails].SecurityToken
	from [dbo].[TaxPayerAPIEnvAccessDetails]
	where TaxPayerId = @TaxPayerId and APIEnvironmentId = @APIEnvId;
end
go
if object_id(N'[dbo].[GetAllCountryCodes]','P') is not null
	drop procedure [dbo].[GetAllCountryCodes];
go
create procedure [dbo].[GetAllCountryCodes]
as
begin
	select	CountryCode.Code
	,		CountryCode.EnglishDescription
	,		CountryCode.ArabicDescription
	from [dbo].CountryCode
end
go
if object_id(N'[dbo].[GetAllTaxTypes]','P') is not null
	drop procedure [dbo].[GetAllTaxTypes];
go
create procedure [dbo].[GetAllTaxTypes]
as
begin
	select	[dbo].TaxType.Code as TaxTypeCode
	,		[dbo].[TaxType].EnglishDescription as TaxTypeEngDesc
	,		[dbo].[TaxType].ArabicDescription as TaxTypeAraDesc
	from [dbo].TaxType
	order by [dbo].TaxType.Code asc;
	select	[dbo].TaxSubType.Code as SubtypeCode
	,		[dbo].TaxSubType.EnglishDescription as SubtypeEngDesc
	,		[dbo].TaxSubType.ArabicDescription as SubtypeAraDesc
	,		[dbo].TaxSubType.TaxTypeCode
	from [dbo].[TaxSubType]
	order by [dbo].[TaxSubType].TaxTypeCode asc;
end
go
if OBJECT_ID(N'[dbo].[InsertTaxType]','P') is not null
	drop procedure [dbo].[InsertTaxType];
go
create procedure [dbo].[InsertTaxType]
@Code as nvarchar(10),
@ArabicDescription as nvarchar(200),
@EnglishDesciption as nvarchar(200),
@SubTypeCode as nvarchar(10),
@SubTypeArabicDescription as nvarchar(200),
@SubTypeEnglishDescription as nvarchar(200)
as
begin
	begin try
		begin transaction t1;
			insert into [dbo].[TaxType]([Code],[EnglishDescription],[ArabicDescription]) 
			values(@Code,@EnglishDesciption,@ArabicDescription);
			insert into [dbo].[TaxSubType]([Code],[EnglishDescription],[ArabicDescription],[TaxTypeCode])
			values (@SubTypeCode,@SubTypeEnglishDescription,@SubTypeEnglishDescription,@Code);
		commit transaction;
	end try
	begin catch
		rollback transaction t1;
		throw;
	end catch
end
go
if object_id(N'[dbo].[InsertTaxableItem]','P') is not null
	drop procedure [dbo].[InsertTaxableItem];
go
create procedure [dbo].[InsertTaxableItem]
@TaxType nvarchar(30) ,
@SubType nvarchar(50) ,
@Amount decimal(28,5),
@Rate decimal(28,5),
@InvoiceLineId int ,
@Id int output,
@VerCol varbinary(8) output
as
begin
	insert into [dbo].[TaxableItem]([TaxType],[SubType],[Amount],[Rate],[InvoiceLineId])
	values (@TaxType,@SubType,@Amount,@Rate,@InvoiceLineId);
	set @Id = @@IDENTITY;
	set @VerCol = @@DBTS;
end
go
if object_id(N'[dbo].[UpdateTaxableItem]','P') is not null
	drop procedure [dbo].[UpdateTaxableItem];
go
create procedure [dbo].[UpdateTaxableItem]
@Id int,
@TaxType nvarchar(30) ,
@SubType nvarchar(50) ,
@Amount decimal(28,5),
@Rate decimal(28,5),
@InvoiceLineId int ,
@OldVerCol rowVersion,
@NewVerCol rowversion output
as
begin
	update [dbo].[TaxableItem]
	set Amount = @Amount,Rate = @Rate,InvoiceLineId = @InvoiceLineId,SubType = @SubType,TaxType=@TaxType
	where Id = @Id and VerCol = @OldVerCol;
	if exists(select 1 from dbo.TaxableItem where Id = @Id)
	begin
		if @@ROWCOUNT > 0
			set @NewVerCol = @@DBTS;
		else
			throw 5000,'This Record has Been Modified By Another User.',1;
	end
	else
		set @NewVerCol = @OldVerCol;
end
go
if object_id(N'[dbo].[SaveOrUpdateTaxableItem]','P') is not null
	drop procedure [dbo].[SaveOrUpdateTaxableItem];
go
create procedure [dbo].[SaveOrUpdateTaxableItem]
@OldId int null = null,
@TaxType nvarchar(30) ,
@SubType nvarchar(50) ,
@Amount decimal(28,5),
@Rate decimal(28,5),
@InvoiceLineId int ,
@OldVerCol rowversion null = null,
@NewVerCol rowversion output,
@NewId int output
as
begin
	if(exists(select 1 from dbo.TaxableItem where Id = @OldId))
	begin
		exec [dbo].[UpdateTaxableItem] 
			@Id = @OldId,
			@TaxType = @TaxType,
			@SubType = @SubType,
			@Amount = @Amount,
			@Rate = @Rate,
			@InvoiceLineId = @InvoiceLineId,
			@OldVerCol = @OldVerCol,
			@NewVerCol = @NewVerCol output;
		set @NewId = @OldId;
	end
	else
	begin
		exec [dbo].[InsertTaxableItem] 
			@TaxType = @TaxType,
			@SubType = @SubType,
			@Amount = @Amount,
			@Rate = @Rate,
			@InvoiceLineId = @InvoiceLineId,
			@Id = @NewId output,
			@VerCol = @NewVerCol output;
	end
end
go
if object_id(N'[dbo].[InsertInvoiceLine]','P') is not null
	drop procedure [dbo].[InsertInvoiceLine];
go
create procedure [dbo].[InsertInvoiceLine]
@Description as nvarchar(500) ,
@ItemType nvarchar(30) ,
@ItemCode nvarchar(100) ,
@UnitType nvarchar(30) ,
@Quantity decimal(28,5) ,
@SalesTotal decimal(28,5) ,
@CurrencySold nvarchar(3) ,
@AmountEGP decimal(28,5) = 0,
@AmountSold decimal(28,5),
@CurrencyExchangeRate decimal(28,5),
@InternalCode nvarchar(50) null = '',
@ItemsDiscount decimal(28,5)  = 0,
@NetTotal decimal(28,5) = 0,
@TotalTaxableFees decimal(28,5) = 0,
@ValueDifference decimal(28,5) = 0,
@Total decimal(28,5),
@DiscountRate decimal(28,5),
@DiscountAmount decimal(28,5),
@DocumentId int ,
@LineId int output,
@VerCol varbinary(8) output
as
begin
	
	insert into [dbo].[InvoiceLine]([Description],[ItemType],[ItemCode],[UnitType],[Quantity],[SalesTotal],[CurrencySold],
	[AmountEGP],[AmountSold],[CurrencyExchangeRate],[InternalCode],[ItemsDiscount],[NetTotal],[TotalTaxableFees],
	[ValueDifference],[Total],[DiscountRate],[DiscountAmount],[DocumentId])
	values(@Description,@ItemType,@ItemCode,@UnitType,@Quantity,@SalesTotal,@CurrencySold,@AmountEGP,@AmountSold,@CurrencyExchangeRate,@InternalCode,@ItemsDiscount,@NetTotal,@TotalTaxableFees,
	@ValueDifference,@Total,@DiscountAmount,@DiscountRate,@DocumentId)
	set @LineId = @@IDENTITY;
	set @VerCol = @@DBTS;
end
go
if object_id(N'[dbo].[UpdateInvoiceLine]','P') is not null
	drop procedure [dbo].[UpdateInvoiceLine]
go
create procedure [dbo].[UpdateInvoiceLine]
@Id int,
@Description as nvarchar(500) ,
@ItemType nvarchar(30) ,
@ItemCode nvarchar(100) ,
@UnitType nvarchar(30) ,
@Quantity decimal(28,5) ,
@SalesTotal decimal(28,5) ,
@CurrencySold nvarchar(3) ,
@AmountEGP decimal(28,5) = 0,
@AmountSold decimal(28,5),
@CurrencyExchangeRate decimal(28,5),
@InternalCode nvarchar(50),
@ItemsDiscount decimal(28,5),
@NetTotal decimal(28,5),
@TotalTaxableFees decimal(28,5),
@ValueDifference decimal(28,5),
@Total decimal(28,5),
@DiscountRate decimal(28,5),
@DiscountAmount decimal(28,5),
@DocumentId int,
@OldVerCol rowversion,
@NewVerCol rowversion output
as
begin
	
	update [dbo].[InvoiceLine]
	set [Description] = @Description,
		[ItemType] = @ItemType,
		[ItemCode] = @ItemCode,
		[UnitType] = @UnitType,
		[Quantity] = @Quantity,
		[SalesTotal] = @SalesTotal,
		[CurrencySold] = @CurrencySold,
		[AmountEGP] = @AmountEGP,
		[AmountSold] = @AmountSold,
		[CurrencyExchangeRate] = @CurrencyExchangeRate,
		[InternalCode] = @InternalCode,
		[ItemsDiscount] = @ItemsDiscount,
		[NetTotal] = @NetTotal,
		[TotalTaxableFees] = @TotalTaxableFees,
		[ValueDifference] = @ValueDifference,
		[Total] = @Total,
		[DiscountRate] = @DiscountRate,
		[DiscountAmount] = @DiscountAmount,
		[DocumentId] = @DocumentId
	where Id = @Id and [VerCol] = @OldVerCol;
	if @@rowcount = 0 and exists (select 1 from [dbo].[InvoiceLine] where [Id] = @Id)
		throw 5000,'The Record has Been Modified By Another User.',1;
	if @@ROWCOUNT > 0
		set @NewVerCol = @@DBTS;
	else
		set @NewVerCol = @OldVerCol;
end
go
if object_id(N'[dbo].[SaveOrUpdateInvoiceLine]','P') is not null
	drop procedure [dbo].[SaveOrUpdateInvoiceLine];
go
create procedure [dbo].[SaveOrUpdateInvoiceLine]
@OldId int null = null,
@Description as nvarchar(500) ,
@ItemType nvarchar(30) ,
@ItemCode nvarchar(100) ,
@UnitType nvarchar(30) ,
@Quantity decimal(28,5) ,
@SalesTotal decimal(28,5) ,
@CurrencySold nvarchar(3) ,
@AmountEGP decimal(28,5) = 0,
@AmountSold decimal(28,5),
@CurrencyExchangeRate decimal(28,5),
@InternalCode nvarchar(50),
@ItemsDiscount decimal(28,5),
@NetTotal decimal(28,5),
@TotalTaxableFees decimal(28,5),
@ValueDifference decimal(28,5),
@Total decimal(28,5),
@DiscountRate decimal(28,5),
@DiscountAmount decimal(28,5),
@DocumentId int,
@OldverCol rowversion null = null,
@NewId int output,
@NewVerCol rowversion output
as
begin
	if exists(select 1 from dbo.InvoiceLine where Id = @OldId)
	begin
		exec dbo.UpdateInvoiceLine 
			@Id = @OldId,
			@Description = @Description,
			@ItemType = @ItemType,
			@ItemCode = @ItemCode,
			@UnitType = @UnitType,
			@Quantity = @Quantity,
			@SalesTotal = @SalesTotal,
			@CurrencySold = @CurrencySold,
			@AmountEGP = @AmountEGP,
			@AmountSold = @AmountSold,
			@CurrencyExchangeRate = @CurrencyExchangeRate,
			@InternalCode = @InternalCode,
			@ItemsDiscount = @ItemsDiscount,
			@NetTotal = @NetTotal,
			@TotalTaxableFees = @TotalTaxableFees,
			@ValueDifference = @ValueDifference,
			@Total = @Total,
			@DiscountRate = @DiscountRate,
			@DiscountAmount = @DiscountAmount,
			@DocumentId = @DocumentId,
			@OldVerCol = @OldverCol,
			@NewVerCol = @NewVerCol output;
		set @NewId = @OldId;
	end
	else
	begin
		exec dbo.InsertInvoiceLine
			@Description = @Description,
			@ItemType = @ItemType,
			@ItemCode=@ItemCode,
			@UnitType = @UnitType,
			@Quantity = @Quantity,
			@SalesTotal = @SalesTotal,
			@CurrencySold = @CurrencySold,
			@AmountEGP = @AmountEGP,
			@AmountSold = @AmountSold,
			@CurrencyExchangeRate = @CurrencyExchangeRate,
			@InternalCode = @InternalCode,
			@ItemsDiscount = @ItemsDiscount,
			@NetTotal = @NetTotal,
			@TotalTaxableFees = @TotalTaxableFees,
			@ValueDifference = @ValueDifference,
			@Total = @Total,
			@DiscountAmount = @DiscountAmount,
			@DiscountRate = @DiscountRate,
			@DocumentId = @DocumentId,
			@LineId = @NewId output,
			@VerCol = @NewVerCol output
	end
end
go
if object_id('[dbo].[InsertDocument]','P') is not null
	drop procedure [dbo].[InsertDocument]
go
create procedure [dbo].[InsertDocument]
@DocumentType as nvarchar(20) = 'I' ,
@DocumentTypeVersion as nvarchar(100)  = '1.0',
@DateTimeIssued as datetime  = GETDATE,
@TaxpayerActivityCode as nvarchar(10) ='',
@InternalId as nvarchar(50) ='',
@PurchaseOrderReference as nvarchar(100) = null ,
@PurchaseOrderDescription as nvarchar(500) = null,
@SalesOrderReference as nvarchar(100) = null,
@SalesOrderDescription as nvarchar(500) = null,
@ProformaInvoiceNumber as nvarchar(50) = null,
@BankName as nvarchar(100) = '',
@BankAddress as nvarchar(500)  = '',
@BankAccountNo as nvarchar(50) = '',
@BankAccountIBAN as nvarchar(50) = '',
@SwiftCode as nvarchar(50) = '',
@PaymentTerms as nvarchar(500) = '',
@Approach as nvarchar(100) = '',
@Packaging as nvarchar(100) = '',
@DateValidity as datetime = null,
@ExportPort as nvarchar(100) = '',
@CountryOfOrigin as nvarchar(100) ='' ,
@GrossWeight as decimal(28,5) = 0,
@NetWeight as decimal(28,5) = 0,
@DeliveryTerms as nvarchar(500) = '',
@TotalSalesAmount as decimal(28,5)  = 0,
@TotalDiscountAmount as decimal(28,5)  = 0,
@NetAmount as decimal(28,5)  = 0,
@ExtraDiscountAmount as decimal(28,5) = 0,
@TotalAmount as decimal(28,5)  = 0,
@TotalItemsDiscountAmount as decimal(28,5)  = 0,
@TaxpayerId as nvarchar(30) ,
@CustomerId as nvarchar(50) ,
@DocumentId int output,
@VerCol varbinary(8) output
as
begin
	
	insert into [dbo].[Document]
	(
		[DocumentType],[DocumentTypeVersion],[DateTimeIssued],[TaxpayerActivityCode],[InternalId],[PurchaseOrderReference],
		[PurchaseOrderDescription],[SalesOrderDescription],[SalesOrderReference],[ProformaInvoiceNumber],[BankName],[BankAddress],
		[BankAccountNo],[BankAccountIBAN],[SwiftCode],[PaymentTerms],[Approach],[Packaging],[DateValidity],[ExportPort],
		[CountryOfOrigin],[GrossWeight],[NetWeight],[DeliveryTerms],[TotalSalesAmount],[TotalDiscountAmount],[NetAmount],[ExtraDiscountAmount],
		[TotalAmount],[TotalItemsDiscountAmount],[TaxpayerId],[CustomerId]
	)
	
	values
	(
		@DocumentType,@DocumentTypeVersion,@DateTimeIssued,@TaxpayerActivityCode,@InternalId,@PurchaseOrderDescription,
		@PurchaseOrderDescription,@SalesOrderDescription,@SalesOrderReference,@ProformaInvoiceNumber,@BankName,@BankAddress,
		@BankAccountNo,@BankAccountIBAN,@SwiftCode,@PaymentTerms,@Approach,@Packaging,@DateValidity,@ExportPort,@CountryOfOrigin,
		@GrossWeight,@NetWeight,@DeliveryTerms,@TotalSalesAmount,@TotalDiscountAmount,@NetAmount,@ExtraDiscountAmount,@TotalAmount,@TotalItemsDiscountAmount,
		@TaxpayerId,@CustomerId
	)
	set @DocumentId = IDENT_CURRENT('[dbo].[Document]');
	set @VerCol = @@DBTS;
end	
go
if object_id(N'[dbo].[DeleteDocument]','P') is not null
	drop procedure [dbo].[DeleteDocument];
go
create procedure [dbo].[DeleteDocument]
@documentId int 
as
begin
	begin try
		begin transaction t1;
		delete from [dbo].[TaxableItem]
		where [InvoiceLineId] in (select [Id] from [dbo].[InvoiceLine] where [DocumentId] = @documentId);
		delete from [dbo].[InvoiceLine] where [DocumentId] = @documentId;
		delete from [dbo].[Document] where [Id] = @documentId;
		commit transaction t1;
		select @@rowcount;
	end try
	begin catch
		rollback transaction t1;
		throw;
	end catch
end
go
if object_id(N'[dbo].[UpdateDocument]','P') is not null
	drop procedure [dbo].[UpdateDocument];
go
create procedure [dbo].[UpdateDocument]
@Id int,
@DocumentType as nvarchar(20) = 'I' ,
@DocumentTypeVersion as nvarchar(100)  = '1.0',
@DateTimeIssued as datetime  = GETDATE,
@TaxpayerActivityCode as nvarchar(10) ,
@InternalId as nvarchar(50) ,
@PurchaseOrderReference as nvarchar(100) null = '',
@PurchaseOrderDescription as nvarchar(500) null = '',
@SalesOrderReference as nvarchar(100) null = '',
@SalesOrderDescription as nvarchar(500) null = '',
@ProformaInvoiceNumber as nvarchar(50) null = '',
@BankName as nvarchar(100) null = '',
@BankAddress as nvarchar(500) null = '',
@BankAccountNo as nvarchar(50) null = '',
@BankAccountIBAN as nvarchar(50) null = '',
@SwiftCode as nvarchar(50) null = '',
@PaymentTerms as nvarchar(500) null = '',
@Approach as nvarchar(100) null = '',
@Packaging as nvarchar(100) null = '',
@DateValidity as datetime null = GETDATE,
@ExportPort as nvarchar(100) null = '',
@CountryOfOrigin as nvarchar(100) null = '',
@GrossWeight as decimal(28,16) null = 0,
@NetWeight as decimal(28,5) null = 0,
@DeliveryTerms as nvarchar(500) null = '',
@TotalSalesAmount as decimal(28,5)  = 0,
@TotalDiscountAmount as decimal(28,5)  = 0,
@NetAmount as decimal(28,5)  = 0,
@ExtraDiscountAmount as decimal(28,5) = 0,
@TotalAmount as decimal(28,5)  = 0,
@TotalItemsDiscountAmount as decimal(28,5)  = 0,
@TaxpayerId as nvarchar(30) ,
@CustomerId as nvarchar(50) ,
@OldVerCol rowversion,
@NewVerCol rowversion output
as
begin
	update [dbo].[Document]
	set Approach = @Approach, BankAccountIBAN = @BankAccountIBAN, BankAccountNo = @BankAccountNo, BankAddress = @BankAddress,
	BankName = @BankName, ProformaInvoiceNumber = @ProformaInvoiceNumber, CountryOfOrigin = @CountryOfOrigin, DateTimeIssued = @DateTimeIssued, 
	DateValidity = @DateValidity, DeliveryTerms = @DeliveryTerms, DocumentType = @DocumentType, DocumentTypeVersion = @DocumentTypeVersion, 
	InternalId = @InternalId, NetAmount = @NetAmount ,NetWeight = @NetWeight, ExtraDiscountAmount = @ExtraDiscountAmount
	where [Id] = @Id and VerCol = @OldVerCol;
	if exists(select 1 from Document where Id = @Id)
	begin
		if(@@ROWCOUNT <= 0) 
			throw 5000,'The Record has Been Modified By Another User..',1;
		else
			set @NewVerCol = @@DBTS;
	end
	else
		set @NewVerCol = @OldVerCol;
end
go
if object_id(N'[dbo].[GetAllDocuments]','P') is not null
	drop procedure [dbo].[GetAllDocuments]
go
if object_id(N'[dbo].[SaveOrUpdateDocument]','P') is not null
	drop procedure [dbo].[SaveOrUpdateDocument];
go
create procedure [dbo].[SaveOrUpdateDocument]
@OldId int null = null,
@DocumentType as nvarchar(20) = 'I' ,
@DocumentTypeVersion as nvarchar(100)  = '1.0',
@DateTimeIssued as datetime ,
@TaxpayerActivityCode as nvarchar(10) ,
@InternalId as nvarchar(50) ,
@PurchaseOrderReference as nvarchar(100) null = '',
@PurchaseOrderDescription as nvarchar(500) null = '',
@SalesOrderReference as nvarchar(100) null = '',
@SalesOrderDescription as nvarchar(500) null = '',
@ProformaInvoiceNumber as nvarchar(50) null = '',
@BankName as nvarchar(100) null = '',
@BankAddress as nvarchar(500) null = '',
@BankAccountNo as nvarchar(50) null = '',
@BankAccountIBAN as nvarchar(50) null = '',
@SwiftCode as nvarchar(50) null = '',
@PaymentTerms as nvarchar(500) null = '',
@Approach as nvarchar(100) null = '',
@Packaging as nvarchar(100) null = '',
@DateValidity as datetime null = GETDATE,
@ExportPort as nvarchar(100) null = '',
@CountryOfOrigin as nvarchar(100) null = '',
@GrossWeight as decimal(28,16) null = 0,
@NetWeight as decimal(28,5) null = 0,
@DeliveryTerms as nvarchar(500) null = '',
@TotalSalesAmount as decimal(28,5)  = 0,
@TotalDiscountAmount as decimal(28,5)  = 0,
@NetAmount as decimal(28,5)  = 0,
@ExtraDiscountAmount as decimal(28,5) = 0,
@TotalAmount as decimal(28,5)  = 0,
@TotalItemsDiscountAmount as decimal(28,5)  = 0,
@TaxpayerId as nvarchar(30) ,
@CustomerId as nvarchar(50) ,
@OldVerCol rowversion null = null,
@NewVerCol rowversion output,
@NewId int output
as
begin
	if exists(select 1 from dbo.Document where Id = @OldId)
	begin
		exec dbo.UpdateDocument 
			@Id = @OldId,
			@DocumentType = @DocumentType,
			@DocumentTypeVersion = @DocumentTypeVersion,
			@DateTimeIssued = @DateTimeIssued,
			@TaxpayerActivityCode = @TaxpayerActivityCode,
			@InternalId = @InternalId,
			@PurchaseOrderReference = @PurchaseOrderReference,
			@PurchaseOrderDescription = @PurchaseOrderDescription,
			@SalesOrderReference = @SalesOrderReference,
			@SalesOrderDescription = @SalesOrderDescription,
			@ProformaInvoiceNumber = @ProformaInvoiceNumber,
			@BankName = @BankName,
			@BankAddress = @BankAddress,
			@BankAccountNo = @BankAccountNo,
			@BankAccountIBAN = @BankAccountIBAN,
			@SwiftCode = @SwiftCode,
			@PaymentTerms = @PaymentTerms,
			@Approach = @Approach,
			@Packaging = @Packaging,
			@DateValidity = @DateValidity,
			@TotalSalesAmount = @TotalSalesAmount,
			@TotalDiscountAmount = @TotalDiscountAmount,
			@NetAmount = @NetAmount,
			@ExtraDiscountAmount = @ExtraDiscountAmount,
			@TotalAmount = @TotalAmount,
			@TotalItemsDiscountAmount = @TotalItemsDiscountAmount,
			@TaxpayerId = @TaxpayerId,
			@CustomerId = @CustomerId,
			@OldVerCol = @OldVerCol,
			@NewVerCol = @NewVerCol output;
			set @NewId = @OldId;
	end
	else
	begin
		exec [dbo].[InsertDocument] 
			@DocumentType = @DocumentType,
			@DocumentTypeVersion = @DocumentTypeVersion,
			@DateTimeIssued = @DateTimeIssued,
			@TaxpayerActivityCode = @TaxpayerActivityCode,
			@InternalId = @InternalId,
			@PurchaseOrderReference = @PurchaseOrderReference,
			@PurchaseOrderDescription = @PurchaseOrderDescription,
			@SalesOrderReference = @SalesOrderReference,
			@SalesOrderDescription = @SalesOrderDescription,
			@ProformaInvoiceNumber = @ProformaInvoiceNumber,
			@BankName = @BankName,
			@BankAddress = @BankAddress,
			@BankAccountNo = @BankAccountNo,
			@BankAccountIBAN = @BankAccountIBAN,
			@SwiftCode = @SwiftCode,
			@PaymentTerms = @PaymentTerms,
			@Approach = @Approach,
			@Packaging = @Packaging,
			@DateValidity = @DateValidity,
			@TotalSalesAmount = @TotalSalesAmount,
			@TotalDiscountAmount = @TotalDiscountAmount,
			@NetAmount = @NetAmount,
			@ExtraDiscountAmount = @ExtraDiscountAmount,
			@TotalAmount = @TotalAmount,
			@TotalItemsDiscountAmount = @TotalItemsDiscountAmount,
			@TaxpayerId = @TaxpayerId,
			@CustomerId = @CustomerId,
			@DocumentId = @NewId output,
			@VerCol = @NewVerCol output;
	end
end
go
if OBJECT_ID(N'[dbo].[GetAllIssuerDocuments]','P') is not null
	drop procedure [dbo].[GetAllIssuerDocuments]
go
create procedure [dbo].[GetAllIssuerDocuments]
@IssuerId nvarchar(30)
as
begin
	select  [dbo].[Document].Approach
	,		[dbo].[Document].BankAccountIBAN
	,		[dbo].[Document].BankAccountNo
	,		[dbo].[Document].BankAddress
	,		[dbo].[Document].BankName
	,		[dbo].[Document].CountryOfOrigin
	,		[dbo].[Document].CustomerId
	,		[dbo].Document.DateTimeIssued
	,		[dbo].Document.DateValidity
	,		[dbo].[Document].DeliveryTerms
	,		[dbo].[Document].DocumentType
	,		[dbo].[Document].DocumentTypeVersion
	,		[dbo].[Document].ExportPort
	,		[dbo].[Document].GrossWeight
	,		[dbo].[Document].Id
	,		[dbo].[Document].InternalId
	,		[dbo].[Document].NetAmount
	,		[dbo].[Document].ExtraDiscountAmount
	,		[dbo].[Document].NetWeight
	,		[dbo].[Document].Packaging
	,		[dbo].[Document].PaymentTerms
	,		[dbo].[Document].ProformaInvoiceNumber
	,		[dbo].[Document].PurchaseOrderDescription
	,		[dbo].[Document].PurchaseOrderReference
	,		[dbo].[Document].SalesOrderReference
	,		[dbo].[Document].SalesOrderDescription
	,		[dbo].[Document].SwiftCode
	,		[dbo].[Document].TaxpayerActivityCode
	,		[dbo].[Document].TaxpayerId
	,		[dbo].[Document].TotalAmount
	,		[dbo].[Document].TotalDiscountAmount
	,		[dbo].[Document].TotalItemsDiscountAmount
	,		[dbo].[Document].TotalSalesAmount
	,		[dbo].[Document].VerCol
	from [dbo].[Document]
	where [dbo].[Document].TaxpayerId = @IssuerId;
end
go
if object_id(N'[dbo].[GetInvoiceLinesByDocumentId]','P') is not null
	drop procedure [dbo].[GetInvoiceLinesByDocumentId];
go
create procedure [dbo].[GetInvoiceLinesByDocumentId]
@documentId int 
as
begin
	select  [dbo].[InvoiceLine].AmountEGP
	,		[dbo].[InvoiceLine].AmountSold
	,		[dbo].[InvoiceLine].CurrencyExchangeRate
	,		[dbo].[InvoiceLine].CurrencySold
	,		[dbo].[InvoiceLine].[Description]
	,		[dbo].[InvoiceLine].DiscountAmount
	,		[dbo].[InvoiceLine].DiscountRate
	,		[dbo].[InvoiceLine].DocumentId
	,		[dbo].[InvoiceLine].Id
	,		[dbo].[InvoiceLine].InternalCode
	,		[dbo].[InvoiceLine].ItemCode
	,		[dbo].[InvoiceLine].ItemsDiscount
	,		[dbo].[InvoiceLine].ItemType
	,		[dbo].[InvoiceLine].NetTotal
	,		[dbo].[InvoiceLine].Quantity
	,		[dbo].[InvoiceLine].SalesTotal
	,		[dbo].[InvoiceLine].Total
	,		[dbo].[InvoiceLine].TotalTaxableFees
	,		[dbo].[InvoiceLine].UnitType
	,		[dbo].[InvoiceLine].ValueDifference
	,		[dbo].[InvoiceLine].VerCol
	,		[dbo].[InvoiceLine].TotalTaxableFees
	from  [dbo].[InvoiceLine]
	where [DocumentId] = @documentId;
end
go
if object_id(N'[dbo].[GetTaxableItemsByInvoiceLineId]','P') is not null
	drop procedure [dbo].[GetTaxableItemsByInvoiceLineId];
go
create procedure [dbo].[GetTaxableItemsByInvoiceLineId]
@InvoiceLineId int
as
begin
	select  [dbo].[TaxableItem].Amount
	,		[dbo].[TaxableItem].Id
	,		[dbo].[TaxableItem].InvoiceLineId
	,		[dbo].[TaxableItem].Rate
	,		[dbo].[TaxableItem].SubType
	,		[dbo].[TaxableItem].TaxType
	,		[dbo].[TaxableItem].VerCol
	from [dbo].[TaxableItem]
	where InvoiceLineId = @InvoiceLineId;
end
go
if object_id(N'[dbo].[GetUserByName_Password]','P') is not null
	drop procedure [dbo].[GetUserByName_Password];
go
create procedure [dbo].[GetUserByName_Password]
@Name nvarchar(50) ,
@Password nvarchar(50) 
as
begin
	select  [Id]
	,		[Name]
	,		@Password as [Password]
	,		[TaxPayerId]
	,		[VerCol]
	from [dbo].[User]
	where [Name] = @Name
	and   [Password] = HASHBYTES('SHA2_256',@Password)
end
go
if object_id('[dbo].[InsertAPIEnvironment]', 'P') is not null
	drop procedure [dbo].[InsertAPIEnvironment];
go
create procedure InsertAPIEnvironment
@Name as nvarchar(50),
@LogInUri as nvarchar(100),
@BaseUri as nvarchar(100),
@Id int output
as
begin
	set nocount on;
	insert into [dbo].[APIEnvironment]([Name],[LogInUri],[BaseUri])
	values(@Name,@logInUri,@BaseUri);
	select @Id = @@IDENTITY;
end
go
if object_id('[dbo].[UpdateAPIEnvironment]','P') is not null
	drop procedure [dbo].[UpdateAPIEnvironment];
go
create procedure [dbo].[UpdateAPIEnvironment]
@newName nvarchar(50),
@newLogInUri nvarchar(100) ,
@newBaseUri nvarchar(100),
@Id int,
@VerCol rowversion,
@rowCount int output
as
	if(exists(select 1 from [dbo].[APIEnvironment] where [Id] = @Id))
	begin
		update [dbo].[APIEnvironment]
		set [Name] = @newName,[LogInUri] = @newLogInUri,[BaseUri] = @newBaseUri
		where [Id] = @Id and [VerCol] = @VerCol
		set @rowCount = @@rowCount;
		if(@rowCount = 0)
			throw 50000,'The record has been updated by another user...try to reread the record again.',1;
	end
go
if object_id(N'[dbo].[GetAllAPIEnvironments]','P') is not null
	drop procedure [dbo].[GetAllAPIEnvironments];
go
create procedure [dbo].[GetAllAPIEnvironments]
as
begin
	select	[Id]
	,		[Name]
	,		[BaseUri]
	,		[LogInUri]
	,		[VerCol]
	from [dbo].[APIEnvironment];
end
go
if object_id(N'GetTaxPayerById','P') is not null
	drop procedure [dbo].[GetTaxPayerById];
go
create procedure [dbo].[GetTaxPayerById]
@taxPayerId nvarchar(30)
as
begin
	select  [Id]
	,		[Type]
	,		[Name]
	,		[BranchId]
	,		[Country]
	,		[BuildingNumber]
	,		[Room]
	,		[Floor]
	,		[Street]
	,		[Landmark]
	,		[AdditionalInformation]
	,		[Governate]
	,		[RegionCity]
	,		[PostalCode]
	,		[VerCol]
	from [dbo].[TaxPayer]
	where [Id] = @taxPayerId;
end
go
if object_id(N'[dbo].[GetTaxpayerCustomer]','P') is not null
	drop procedure [dbo].[GetTaxpayerCustomer]
go
create procedure [dbo].[GetTaxpayerCustomer]
@TaxpayerId nvarchar(30)
as
begin
	select  dbo.Customer.AdditionalInformation
	,		dbo.Customer.BuildingNumber
	,		dbo.Customer.Country
	,		dbo.Customer.CustomerId
	,		dbo.Customer.[Floor]
	,		dbo.Customer.Governate
	,		dbo.Customer.Id
	,		dbo.Customer.Landmark
	,		dbo.Customer.Name
	,		dbo.Customer.PostalCode
	,		dbo.Customer.RegionCity
	,		dbo.Customer.Room
	,		dbo.Customer.Street
	,		dbo.Customer.[Type]
	,		dbo.Customer.VerCol
	from dbo.Customer
	where exists (select 1 from dbo.Document where dbo.Document.TaxpayerId = @TaxpayerId and dbo.Document.CustomerId = dbo.Customer.CustomerId)
	order by dbo.Customer.Name;
end
go
if object_id(N'[dbo].[GetCustomerById]','P') is not null
	drop procedure [dbo].[GetCustomerById];
go
create procedure [dbo].[GetCustomerById]
@Id nvarchar(30)
as
begin
	select  [CustomerId]
	,		[Type]
	,		[Id]
	,		[Name]
	,		[Country]
	,		[BuildingNumber]
	,		[Room]
	,		[Floor]
	,		[Street]
	,		[Landmark]
	,		[Governate]
	,		[RegionCity]
	,		[PostalCode]
	,		[AdditionalInformation]
	,		[VerCol]
	from [dbo].[Customer]
	where [Id] = @Id;
end
go
if object_id(N'[dbo].[GetCustomerByName]','P') is not null
	drop procedure [dbo].[GetCustomerByName];
go
create procedure [dbo].[GetCustomerByName]
@customerName nvarchar(200)
as
begin
	select  [CustomerId]
	,		[Type]
	,		[Id]
	,		[Name]
	,		[Country]
	,		[BuildingNumber]
	,		[Room]
	,		[Floor]
	,		[Street]
	,		[Landmark]
	,		[Governate]
	,		[RegionCity]
	,		[PostalCode]
	,		[AdditionalInformation]
	,		[VerCol]
	from [dbo].[Customer]
	where [Name] = @customerName;
end
go
if OBJECT_ID(N'[dbo].[GetSubmissionsByEnvId]','P') is not null
	drop procedure [dbo].[GetSubmissionsByEnvId];
go
create procedure [dbo].[GetSubmissionsByEnvId]
@EnvId int
as
begin
	select [dbo].DocumentSubmission.APIEnvironmentId
	,		[dbo].DocumentSubmission.DocumentId
	,		[dbo].DocumentSubmission.SubmissionUUID
	,		[dbo].DocumentSubmission.UUID
	,		[dbo].DocumentSubmission.SubmissionDate
	,		[dbo].DocumentSubmission.[Status]
	,		[dbo].DocumentSubmission.VerCol
	from [dbo].DocumentSubmission
	where [APIEnvironmentId] = @EnvId
end
go
if object_id(N'[dbo].[FindNewDocumentsInOracle]','P') is not null
	drop procedure [dbo].[FindNewDocumentsInOracle];
go
create procedure [dbo].[FindNewDocumentsInOracle]
@IssuerId nvarchar(30),
@APIEnvId int
as 
begin
select cast(ISS_BRANCHID as nvarchar(50)) as ISS_BRANCHID
,	   cast(ISS_COUNTRY as nvarchar(2)) as ISS_COUNTRY
,	   cast(ISS_GOVERNATE as nvarchar(100)) as ISS_GOVERNATE
,	   cast(ISS_REGIONCITY as nvarchar(100)) as ISS_REGIONCITY
,	   cast(ISS_STREET as nvarchar(200)) as ISS_STREET
,	   cast(ISS_BLDGNO as nvarchar(100)) as ISS_BLDGNO
,		cast(ISS_POSTAL as nvarchar(30)) as ISS_POSTAL
,		cast(ISS_FLOOR as nvarchar(100)) as ISS_FLOOR
,		cast(ISS_ROOM as nvarchar(100)) as ISS_ROOM
,		cast(ISS_LANDMARK as nvarchar(500)) as ISS_LANDMARK
,		cast(ISS_ADDINFO as nvarchar(500)) as ISS_ADDINFO
,		cast(ISS_TYPE as nvarchar(2)) as ISS_TYPE
,		cast(ISS_ID as nvarchar(30)) as ISS_ID
,		cast(ISS_NAME as nvarchar(200)) as ISS_NAME
,		cast(RECV_COUNTRY as nvarchar(2)) as RECV_COUNTRY
,		cast(RECV_GOVERNATE as nvarchar(100)) as RECV_GOVERNATE
,		cast(RECV_REGIONCITY as nvarchar(100)) as RECV_REGIONCITY
,		cast(RECV_STREET as nvarchar(200)) as RECV_STREET
,		cast(RECV_BLDGNO as nvarchar(100)) as RECV_BLDGNO
,		cast(RECV_POSTAL as nvarchar(30)) as RECV_POSTAL
,		cast(RECV_FLOOR as nvarchar(100)) as RECV_FLOOR
,		cast(RECV_ROOM as nvarchar(100)) as RECV_ROOM
,		cast(RECV_LANDMARK as nvarchar(500)) as RECV_LANDMARK
,		cast(RECV_ADDINFO as nvarchar(500)) as RECV_ADDINFO
,		cast(RECV_TYPE as nvarchar(1)) as RECV_TYPE
,		cast(RECV_ID as nvarchar(30)) as RECV_ID
,		cast(RECV_NAME as nvarchar(200)) as RECV_NAME
,		cast(DOC_TYPE as nvarchar(20)) as DOC_TYPE
,		cast(DOC_TYPVER as nvarchar(100)) as DOC_TYPVER
,		cast(DOC_DATETIMEISS as datetime) as DOC_DATETIMEISS
,		cast(DOC_TAXPAYERACT as nvarchar(10)) as DOC_TAXPAYERACT
,		cast(DOC_INTERNALID as nvarchar(50)) as DOC_INTERNALID
,		cast(DOC_POREF as nvarchar(100)) as DOC_POREF
,		cast(DOC_PODESC as nvarchar(500)) as DOC_PODESC
,		cast(DOC_SOREF as nvarchar(100)) as DOC_SOREF
,		cast(DOC_SODESC as nvarchar(500)) as DOC_SODESC
,		cast(PROFORMAINVOICENUMBER as nvarchar(50)) as PROFORMAINVOICENUMBER
,		cast(PAY_SWIFTCODE as nvarchar(50)) as PAY_SWIFTCODE
,		cast(PAY_TERMS as nvarchar(500)) as PAY_TERMS
,		cast(DEL_APPROACH as nvarchar(100)) as DEL_APPROACH
,		cast(DEL_PACK as nvarchar(100)) as DEL_PACK
,		cast(DEL_DTVALID as datetime) as DEL_DTVALID
,		cast(DEL_EXP as nvarchar(100)) as DEL_EXP
,		cast(DEL_COUNTRY as nvarchar(100)) as DEL_COUNTRY
,		cast(isnull(DEL_GROSSWGHT,'0') as decimal(28,5)) as DEL_GROSSWGHT
,		cast(isnull(DEL_NETWGHT,'0') as decimal(28,5)) as DEL_NETWGHT
,		cast(DEL_TERMS as nvarchar(500)) as DEL_TERMS
,		cast(TOTALSALES as decimal(28,5)) as TOTALSALES
,		cast(TOTALDISCAMT as decimal(28,5)) as TOTALDISCAMT
,		cast(NETAMT as decimal(28,5)) as NETAMT
,		cast(EXTRADISC as decimal(28,5)) as EXTRADISC
,		cast(TOT_ITEMSDISCAMT as decimal(28,5)) as TOT_ITEMSDISCAMT
,		cast(TAXAMOUNT as decimal(28,5)) as TAXAMOUNT
,		cast(TOTAMT as decimal(28,5)) as TOTAMT
,		SIGNTYP
,		SIGNVAL
,		cast(SOPNUMBER as nvarchar(50)) as SOPNUMBER
,		SOPTYPE
from openquery([PROD],'
select  ''0'' as ISS_BRANCHID --0
 ,		eng_header.ISS_COUNTRY --1
 ,		eng_header.ISS_GOVERNATE --2
 ,		eng_header.ISS_REGIONCITY --3
 ,		eng_header.ISS_STREET --4
 ,		eng_header.ISS_BLDGNO --5
 ,		eng_header.ISS_POSTAL	--6
 ,		eng_header.ISS_FLOOR	--7
 ,		eng_header.ISS_ROOM		--8
 ,		eng_header.ISS_LANDMARK		--9
 ,		eng_header.ISS_ADDINFO		--10
 ,		eng_header.ISS_TYPE		--11
 ,		eng_header.ISS_ID		--12
 ,		eng_header.ISS_NAME		--13
 ,		eng_header.RECV_COUNTRY	--14
 ,		eng_header.RECV_GOVERNATE	--15
 ,		eng_header.RECV_REGIONCITY	--16
 ,		eng_header.RECV_STREET	--17
 ,		eng_header.RECV_BLDGNO	--18
 ,		eng_header.RECV_POSTAL		--19
 ,		eng_header.RECV_FLOOR		--20
 ,		eng_header.RECV_ROOM		--21
 ,		eng_header.RECV_LANDMARK	--22
 ,		eng_header.RECV_ADDINFO		--23
 ,		eng_header.RECV_TYPE		--24
 ,		eng_header.RECV_ID		--25
 ,		eng_header.RECV_NAME	--26
 ,		eng_header.DOC_TYPE		--27
 ,		eng_header.DOC_TYPVER	--28
,		eng_header.DOC_DATETIMEISS	--29
,		eng_header.DOC_TAXPAYERACT	--30
,		eng_header.DOC_INTERNALID	--31
,		eng_header.DOC_POREF		--32
,		eng_header.DOC_PODESC		--33
,		eng_header.DOC_SOREF		--34
,		eng_header.DOC_SODESC		--35
,		eng_header.PROFORMAINVOICENUMBER	--36
,		eng_header.PAY_SWIFTCODE		--37
,		eng_header.PAY_TERMS		--38
,		eng_header.DEL_APPROACH		--39
,		eng_header.DEL_PACK		--40
,		eng_header.DEL_DTVALID		--41
,		eng_header.DEL_EXP		--42
,		eng_header.DEL_COUNTRY		--43
,		eng_header.DEL_GROSSWGHT		--44
,		eng_header.DEL_NETWGHT	--45
,		eng_header.DEL_TERMS	--46
,		eng_header.TOTALSALES	--47
,		eng_header.TOTALSALESAMT	--48
,		eng_header.TOTALDISCAMT		--49
,		eng_header.NETAMT		--50
,		eng_header.EXTRADISC	--51
,		eng_header.TOT_ITEMSDISCAMT	--52
,		eng_header.TAXAMOUNT	--53
,		eng_header.TOTAMT		--54
,		eng_header.SIGNTYP		--55
,		eng_header.SIGNVAL		--56
,		eng_header.SOPNUMBER		--57
,		eng_header.SOPTYPE		--58
,		eng_header.CANCELLEDYN		--59
,		eng_header.TRX_DATE		--60
,		eng_header.CT_REFERENCE		--61
,		eng_header.DOCCURRENCYCODE	--62
,		eng_header.DOCCURRENCYXCHANGERATE	--63
from xxcite_tax_headers_eng_v eng_header	
union all
select  ''0'' as ISS_BRANCHID
 ,		eng_header.ISS_COUNTRY
 ,		eng_header.ISS_GOVERNATE
 ,		eng_header.ISS_REGIONCITY
 ,		eng_header.ISS_STREET
 ,		eng_header.ISS_BLDGNO
 ,		eng_header.ISS_POSTAL
 ,		eng_header.ISS_FLOOR
 ,		eng_header.ISS_ROOM
 ,		eng_header.ISS_LANDMARK
 ,		eng_header.ISS_ADDINFO
 ,		eng_header.ISS_TYPE
 ,		eng_header.ISS_ID
 ,		eng_header.ISS_NAME
,		eng_header.RECV_COUNTRY
,		eng_header.RECV_GOVERNATE
,		eng_header.RECV_REGIONCITY
,		eng_header.RECV_STREET
,		eng_header.RECV_BLDGNO
,		eng_header.RECV_POSTAL
,		eng_header.RECV_FLOOR
,		eng_header.RECV_ROOM
,		eng_header.RECV_LANDMARK
,		eng_header.RECV_ADDINFO
,		eng_header.RECV_TYPE
,		eng_header.RECV_ID
,		eng_header.RECV_NAME
,		eng_header.DOC_TYPE
,		eng_header.DOC_TYPVER
,		eng_header.DOC_DATETIMEISS
,		eng_header.DOC_TAXPAYERACT
,		eng_header.DOC_INTERNALID
,		eng_header.DOC_POREF
,		eng_header.DOC_PODESC
,		eng_header.DOC_SOREF
,		eng_header.DOC_SODESC
,		eng_header.PROFORMAINVOICENUMBER
,		eng_header.PAY_SWIFTCODE
,		eng_header.PAY_TERMS
,		eng_header.DEL_APPROACH
,		eng_header.DEL_PACK
,		eng_header.DEL_DTVALID
,		eng_header.DEL_EXP
,		eng_header.DEL_COUNTRY
,		eng_header.DEL_GROSSWGHT
,		eng_header.DEL_NETWGHT
,		eng_header.DEL_TERMS
,		eng_header.TOTALSALES
,		eng_header.TOTALSALESAMT
,		eng_header.TOTALDISCAMT
,		eng_header.NETAMT
,		eng_header.EXTRADISC
,		eng_header.TOT_ITEMSDISCAMT
,		eng_header.TAXAMOUNT
,		eng_header.TOTAMT
,		eng_header.SIGNTYP
,		eng_header.SIGNVAL
,		eng_header.SOPNUMBER
,		eng_header.SOPTYPE
,		eng_header.CANCELLEDYN
,		eng_header.TRX_DATE
,		eng_header.CT_REFERENCE
,		eng_header.DOCCURRENCYCODE
,		eng_header.DOCCURRENCYXCHANGERATE
from xxcite_tax_headers_home_v eng_header
union all
select  ''0'' as ISS_BRANCHID
 ,		eng_header.ISS_COUNTRY
 ,		eng_header.ISS_GOVERNATE
 ,		eng_header.ISS_REGIONCITY
 ,		eng_header.ISS_STREET
 ,		eng_header.ISS_BLDGNO
 ,		eng_header.ISS_POSTAL
 ,		eng_header.ISS_FLOOR
 ,		eng_header.ISS_ROOM
 ,		eng_header.ISS_LANDMARK
 ,		eng_header.ISS_ADDINFO
 ,		eng_header.ISS_TYPE
 ,		eng_header.ISS_ID
 ,		eng_header.ISS_NAME
,		eng_header.RECV_COUNTRY
,		eng_header.RECV_GOVERNATE
,		eng_header.RECV_REGIONCITY
,		eng_header.RECV_STREET
,		eng_header.RECV_BLDGNO
,		eng_header.RECV_POSTAL
,		eng_header.RECV_FLOOR
,		eng_header.RECV_ROOM
,		eng_header.RECV_LANDMARK
,		eng_header.RECV_ADDINFO
,		eng_header.RECV_TYPE
,		eng_header.RECV_ID
,		eng_header.RECV_NAME
,		eng_header.DOC_TYPE
,		eng_header.DOC_TYPVER
,		eng_header.DOC_DATETIMEISS
,		eng_header.DOC_TAXPAYERACT
,		eng_header.DOC_INTERNALID
,		eng_header.DOC_POREF
,		eng_header.DOC_PODESC
,		eng_header.DOC_SOREF
,		eng_header.DOC_SODESC
,		eng_header.PROFORMAINVOICENUMBER
,		eng_header.PAY_SWIFTCODE
,		eng_header.PAY_TERMS
,		eng_header.DEL_APPROACH
,		eng_header.DEL_PACK
,		eng_header.DEL_DTVALID
,		eng_header.DEL_EXP
,		eng_header.DEL_COUNTRY
,		eng_header.DEL_GROSSWGHT
,		eng_header.DEL_NETWGHT
,		eng_header.DEL_TERMS
,		eng_header.TOTALSALES
,		eng_header.TOTALSALESAMT
,		eng_header.TOTALDISCAMT
,		eng_header.NETAMT
,		eng_header.EXTRADISC
,		eng_header.TOT_ITEMSDISCAMT
,		eng_header.TAXAMOUNT
,		eng_header.TOTAMT
,		eng_header.SIGNTYP
,		eng_header.SIGNVAL
,		eng_header.SOPNUMBER
,		eng_header.SOPTYPE
,		eng_header.CANCELLEDYN
,		eng_header.TRX_DATE
,		eng_header.CT_REFERENCE
,		eng_header.DOCCURRENCYCODE
,		eng_header.DOCCURRENCYXCHANGERATE
from xxcite_tax_headers_v eng_header') as ora_header
where cast(ora_header.doc_internalid as nvarchar(50)) not in (select internalid from dbo.Document join dbo.DocumentSubmission on dbo.Document.Id = dbo.DocumentSubmission.DocumentId and dbo.DocumentSubmission.APIEnvironmentId = @APIEnvId where dbo.DocumentSubmission.DocumentId is not null and dbo.DocumentSubmission.[Status] = 'Valid')
and   ISS_ID = @IssuerId
order by DOC_INTERNALID;

select cast(LIN_INTERNALCODE as nvarchar(50)) as LIN_INTERNALCODE
,		cast(LIN_UNTTYP as nvarchar(30)) as LIN_UNTTYP
,		cast(isnull(LIN_QTY,'0') as decimal(28,5)) as LIN_QTY
,		cast(isnull(LIN_CURSOLD,'0') as nvarchar(3)) as LIN_CURSOLD
,		cast(isnull(LIN_AMTEGP,'0') as decimal(28,5)) as LIN_AMTEGP
,		cast(isnull(LIN_AMTSOLD,'0') as decimal(28,5)) as LIN_AMTSOLD
,		cast(isnull(LIN_CURREXCH,'0') as decimal(28,5)) as LIN_CURREXCH
,		cast(isnull(SAL_TOT,'0') as decimal(28,5)) as SAL_TOT
,		cast(isnull(VAL_DIFF,'0') as decimal(28,5)) as VAL_DIFF
,		cast(isnull(TAXABLE_FEE,'0') as decimal(28,5)) as TAXABLE_FEE
,		cast(isnull(DISC_RATE,'0') as decimal(28,5)) as DISC_RATE
,		cast(isnull(DISC_AMT,'0') as decimal(28,5)) as DISC_AMT
,		cast(isnull(NET_TOT,'0') as decimal(28,5)) as NET_TOT
,		cast(isnull(LIN_TAXAMOUNT,'0') as decimal(28,5)) as LIN_TAXAMOUNT
,		cast(isnull(ITM_DISC,'0') as decimal(28,5)) as ITM_DISC
,		cast(LIN_ERPUNTTYP as nvarchar(30)) as LIN_ERPUNTTYP
,		cast(isnull(TOTAL,'0') as decimal(28,5)) as TOTAL
,		cast(LNITMSEQNM as nvarchar(50)) as LINE_ITEM_SEQ
,		LIN_ITEMCODE
,		cast(SOPNUMBER as nvarchar(50)) as SOPNUMBER
,		LIN_ITEMTYPE
,		cast(LIN_DESC as nvarchar(500)) as LIN_DESC
from openquery([PROD],'
select	eng_line.LIN_INTERNALCODE	--0
,		eng_line.LIN_UNTTYP		--1
,		eng_line.LIN_QTY		--2
,		eng_line.LIN_CURSOLD	--3
,		eng_line.LIN_AMTEGP		--4
,		eng_line.LIN_AMTSOLD	--5
,		eng_line.LIN_CURREXCH	--6
,		eng_line.SAL_TOT		--7
,		eng_line.VAL_DIFF		--8
,		eng_line.TAXABLE_FEE	--9
,		eng_line.DISC_RATE		--10
,		eng_line.DISC_AMT		--11
,		eng_line.NET_TOT		--12
,		eng_line.LIN_TAXAMOUNT	--13
,		eng_line.ITM_DISC		--14
,		eng_line.LIN_ERPUNTTYP	--15
,		eng_line.TOTAL			--16
,		cast(eng_line.LNITMSEQNM as varchar2(40))	as LNITMSEQNM	--17
,		cast(eng_line.LIN_ITEMCODE as varchar2(100)) as LIN_ITEMCODE	--18
,		eng_line.SOPNUMBER	--19
,		eng_line.LIN_ITEMTYPE	--20
,		eng_header.iss_id		--21
,		eng_line.LIN_DESC
from xxcite_tax_lines_eng_v eng_line
join xxcite_tax_headers_eng_v eng_header
	on eng_line.sopnumber = eng_header.sopnumber
union all
select	eng_line.LIN_INTERNALCODE
,		eng_line.LIN_UNTTYP
,		eng_line.LIN_QTY
,		eng_line.LIN_CURSOLD
,		eng_line.LIN_AMTEGP
,		eng_line.LIN_AMTSOLD
,		eng_line.LIN_CURREXCH
,		eng_line.SAL_TOT
,		eng_line.VAL_DIFF
,		eng_line.TAXABLE_FEE
,		eng_line.DISC_RATE
,		eng_line.DISC_AMT
,		eng_line.NET_TOT
,		eng_line.LIN_TAXAMOUNT
,		eng_line.ITM_DISC
,		eng_line.LIN_ERPUNTTYP
,		eng_line.TOTAL
,		cast(eng_line.LNITMSEQNM as varchar2(40))	as LNITMSEQNM
,		cast(eng_line.LIN_ITEMCODE as varchar2(100)) as LIN_ITEMCODE
,		eng_line.SOPNUMBER
,		eng_line.LIN_ITEMTYPE
,		eng_header.iss_id
,		eng_line.LIN_DESC
from xxcite_tax_lines_home_v eng_line
join xxcite_tax_headers_home_v eng_header
	on eng_line.sopnumber = eng_header.sopnumber
union all
select	eng_line.LIN_INTERNALCODE
,		eng_line.LIN_UNTTYP
,		eng_line.LIN_QTY
,		eng_line.LIN_CURSOLD
,		eng_line.LIN_AMTEGP
,		eng_line.LIN_AMTSOLD
,		eng_line.LIN_CURREXCH
,		eng_line.SAL_TOT
,		eng_line.VAL_DIFF
,		eng_line.TAXABLE_FEE
,		eng_line.DISC_RATE
,		eng_line.DISC_AMT
,		eng_line.NET_TOT
,		eng_line.LIN_TAXAMOUNT
,		eng_line.ITM_DISC
,		eng_line.LIN_ERPUNTTYP
,		eng_line.TOTAL
,		cast(eng_line.LNITMSEQNM as varchar2(40))	as LNITMSEQNM
,		cast(eng_line.LIN_ITEMCODE as varchar2(100)) as LIN_ITEMCODE
,		eng_line.SOPNUMBER
,		eng_line.LIN_ITEMTYPE
,		eng_header.iss_id
,		eng_line.LIN_DESC
from xxcite_tax_lines_v eng_line
join xxcite_tax_headers_v eng_header
	on eng_line.sopnumber = eng_header.sopnumber
') ora_lines
where cast(ora_lines.SOPNUMBER as nvarchar(50)) not in (select internalid from dbo.Document join dbo.DocumentSubmission on dbo.Document.Id = dbo.DocumentSubmission.DocumentId and dbo.DocumentSubmission.APIEnvironmentId = @APIEnvId where dbo.DocumentSubmission.DocumentId is not null and dbo.DocumentSubmission.[Status] = 'Valid')
and   ISS_ID = @IssuerId
order by SOPNUMBER,LINE_ITEM_SEQ;

select  cast(TAXPERCENT as decimal(28,5)) as TAXPERCENT
,		cast(TAXTYPE as nvarchar(30)) as TAXTYPE
,		cast(TAXSUBTYPE as nvarchar(50)) as TAXSUBTYPE
,		cast(AMT as decimal(28,5)) as AMT
,		cast(LINEITMSEQ as nvarchar(50)) as LINE_ITEM_SEQ
,		cast(SOPNUMBER as nvarchar(50)) as SOPNUMBER
from openquery([PROD],'
select  cast(eng_details.TAXPERCENT as number)	as TAXPERCENT --0
,		cast(eng_details.TAXTYPE as varchar2(30))	as TAXTYPE --1
,		cast(eng_details.TAXSUBTYPE	as varchar2(50)) as TAXSUBTYPE--2
,		cast(eng_details.AMT as number) as AMT	--3
,		cast(eng_details.LINEITMSEQ as varchar2(50)) as	LINEITMSEQ--4
,		cast(eng_details.sopnumber as varchar2(50)) as	sopnumber --5
,		eng_header.ISS_ID		--6
from xxcite_tax_details_eng_v eng_details
join xxcite_tax_headers_eng_v eng_header
	on eng_header.sopnumber = eng_details.sopnumber
union all
select  cast(eng_details.TAXPERCENT as number)	as TAXPERCENT --0
,		cast(eng_details.TAXTYPE as varchar2(30))	as TAXTYPE --1
,		cast(eng_details.TAXSUBTYPE	as varchar2(50)) as TAXSUBTYPE--2
,		cast(eng_details.AMT as number) as AMT	--3
,		cast(eng_details.LINEITMSEQ as varchar2(50)) as LINEITMSEQ	--4
,		cast(eng_details.sopnumber as varchar2(50))	as sopnumber--5
,		eng_header.ISS_ID		--6
from xxcite_tax_details_home_v eng_details
join xxcite_tax_headers_home_v eng_header
	on eng_details.sopnumber = eng_header.sopnumber
union all
select  cast(eng_details.TAXPERCENT as number)	as TAXPERCENT --0
,		cast(eng_details.TAXTYPE as varchar2(30))	as TAXTYPE --1
,		cast(eng_details.TAXSUBTYPE	as varchar2(50)) as TAXSUBTYPE--2
,		cast(eng_details.AMT as number) as AMT	--3
,		cast(eng_details.LINEITMSEQ as varchar2(50)) as LINEITMSEQ	--4
,		cast(eng_details.sopnumber as varchar2(50))	 as sopnumber--5
,		eng_header.ISS_ID		--6
from xxcite_tax_details_v eng_details
join xxcite_tax_headers_v eng_header
	on eng_details.sopnumber = eng_header.sopnumber
') ora_details
where cast(ora_details.sopnumber as nvarchar(50)) not in (select internalid from dbo.Document join dbo.DocumentSubmission on dbo.Document.Id = dbo.DocumentSubmission.DocumentId and dbo.DocumentSubmission.APIEnvironmentId = @APIEnvId where dbo.DocumentSubmission.DocumentId is not null and dbo.DocumentSubmission.[Status] = 'Valid')
and   ISS_ID = @IssuerId
order by SOPNUMBER,LINE_ITEM_SEQ;
end
go
if object_id(N'[dbo].[InsertCustomer]','P') is not null
	drop procedure [dbo].[InsertCustomer];
go
create procedure [dbo].[InsertCustomer]
@Type nvarchar(2),
@Id nvarchar(30),
@Name nvarchar(200),
@Country nvarchar(2),
@BuildingNumber nvarchar(100),
@Room nvarchar(100),
@Floor nvarchar(100),
@Street nvarchar(200),
@Landmark nvarchar(500),
@AdditionalInformation nvarchar(500),
@Governate nvarchar(100),
@RegionCity nvarchar(100),
@PostalCode nvarchar(30),
@CustomerId int output,
@VerCol rowversion output
as
begin
	insert into [dbo].[Customer]([Type],[Id],[Name],[Country],[BuildingNumber],[Room],[Floor],[Street],[Landmark],[AdditionalInformation],[Governate],[RegionCity],[PostalCode])
	values(@Type,@Id,@Name,@Country,@BuildingNumber,@Room,@Floor,@Street,@Landmark,@AdditionalInformation,@Governate,@RegionCity,@PostalCode);
	set @CustomerId =IDENT_CURRENT(N'[dbo].[Customer]');
	set @VerCol = @@DBTS;
end
go
if object_id(N'[dbo].[GetCustomerIdByName]','P') is not null
	drop procedure [dbo].[GetCustomerIdByName];
go
create procedure [dbo].[GetCustomerIdByName]
@Name nvarchar(100),
@FoundId nvarchar(30) output
as
begin
	set @FoundId = null;
	select  @FoundId = [CustomerId]
	from [dbo].[Customer]
	where [Name] = @Name;
end
go
if object_id(N'[dbo].[GetDocumentByInternalId]','P') is not null
	drop procedure [dbo].[GetDocumentByInternalId];
go
create procedure [dbo].[GetDocumentByInternalId]
@InternalId as nvarchar(50)
as
begin
	select  [dbo].[Document].Approach
	,		[dbo].[Document].BankAccountIBAN
	,		[dbo].[Document].BankAccountNo
	,		[dbo].[Document].BankAddress
	,		[dbo].[Document].BankName
	,		[dbo].[Document].CountryOfOrigin
	,		[dbo].[Document].CustomerId
	,		[dbo].[Customer].Name as RECV_Name
	,		[dbo].[Customer].[Type] as RECV_Type
	,		[dbo].[Customer].VerCol as RECV_VerCol
	,		[dbo].[Customer].AdditionalInformation as RECV_AdditionalInformation
	,		[dbo].[Customer].BuildingNumber as RECV_BuildingNumber
	,		[dbo].[Customer].Country as RECV_Country
	,		[dbo].[Customer].[Floor] as RECV_Floor
	,		[dbo].[Customer].Governate as RECV_Governate
	,		[dbo].[Customer].Id as RECV_Id
	,		[dbo].[Customer].Landmark as RECV_Landmark
	,		[dbo].[Customer].PostalCode as RECV_PostalCode
	,		[dbo].[Customer].RegionCity as RECV_RegionCity
	,		[dbo].[Customer].Room as RECV_Room
	,		[dbo].[Customer].Street as RECV_Street
	,		[dbo].[Document].DateTimeIssued
	,		[dbo].[Document].DateValidity
	,		[dbo].[Document].DeliveryTerms
	,		[dbo].[Document].DocumentType
	,		[dbo].[Document].DocumentTypeVersion
	,		[dbo].[Document].ExportPort
	,		[dbo].[Document].ExtraDiscountAmount
	,		[dbo].[Document].GrossWeight
	,		[dbo].[Document].NetWeight
	,		[dbo].[Document].Id
	,		[dbo].[Document].InternalId
	,		[dbo].[Document].NetAmount
	,		[dbo].[Document].Packaging
	,		[dbo].[Document].PaymentTerms
	,		[dbo].[Document].ProformaInvoiceNumber
	,		[dbo].[Document].PurchaseOrderDescription
	,		[dbo].[Document].PurchaseOrderReference
	,		[dbo].[Document].SalesOrderDescription
	,		[dbo].[Document].SalesOrderReference
	,		[dbo].[Document].SwiftCode
	,		[dbo].[Document].TaxpayerActivityCode
	,		[dbo].[Document].TaxpayerId
	,		[dbo].TaxPayer.Name as ISS_Name
	,		[dbo].TaxPayer.[Type] as ISS_Type
	,		[dbo].TaxPayer.AdditionalInformation as ISS_AdditionalInformation
	,		[dbo].TaxPayer.BranchId as ISS_BranchId
	,		[dbo].TaxPayer.BuildingNumber as ISS_BuildingNumber
	,		[dbo].TaxPayer.Country as ISS_Country
	,		[dbo].TaxPayer.[Floor] as ISS_Floor
	,		[dbo].TaxPayer.Governate as ISS_Governate
	,		[dbo].TaxPayer.Landmark as ISS_Landmark
	,		[dbo].TaxPayer.PostalCode as ISS_PostalCode
	,		[dbo].TaxPayer.RegionCity as ISS_RegionCity
	,		[dbo].TaxPayer.Room as ISS_Room
	,		[dbo].TaxPayer.Street	as ISS_Street
	,		[dbo].[TaxPayer].[VerCol] as ISS_VerCol
	,		[dbo].[Document].TotalAmount
	,		[dbo].[Document].TotalDiscountAmount
	,		[dbo].[Document].TotalItemsDiscountAmount
	,		[dbo].[Document].TotalSalesAmount
	,		[dbo].[Document].VerCol
	from [dbo].[Document]
	join [dbo].TaxPayer
		on [dbo].[Document].TaxpayerId = dbo.TaxPayer.Id
	join [dbo].Customer
		on dbo.Document.CustomerId = dbo.Customer.CustomerId
	where [dbo].[Document].InternalId = @InternalId;
end
go
if OBJECT_ID(N'[dbo].[GetDocumentById]','P') is not null
	drop procedure [dbo].[GetDocumentById];
go
create procedure [dbo].[GetDocumentById]
@Id as int
as
begin
	select	[dbo].[Document].[Id]
	,		[dbo].[Document].[Approach]
	,		[dbo].[Document].[BankAccountIBAN]
	,		[dbo].[Document].[BankAccountNo]
	,		[dbo].[Document].[BankAddress]
	,		[dbo].[Document].[BankName]
	,		[dbo].[Document].[CountryOfOrigin]
	,		[dbo].[Document].[CustomerId]
	,		[dbo].[Customer].[Name] as RECV_Name
	,		[dbo].[Customer].[AdditionalInformation] as Recv_additionalInformation
	,		[dbo].[Customer].[BuildingNumber] as RECV_BuildingNumber
	,		[dbo].[Customer].[Country] as RECV_Country
	,		[dbo].[Customer].[Floor] as RECV_Floor
	,		[dbo].[Customer].[Governate] as RECV_Governate
	,		[dbo].[Customer].[Id] as RECV_Id
	,		[dbo].[Customer].[Landmark] as RECV_Landmark
	,		[dbo].[Customer].[Name] as RECV_Name
	,		[dbo].[Customer].[PostalCode] as RECV_PostalCode
	,		[dbo].[Customer].[RegionCity] as RECV_RegionCity
	,		[dbo].[Customer].[Room] as RECV_Room
	,		[dbo].[Customer].[Street] as RECV_Street
	,		[dbo].[Customer].[Type] as RECV_type
	,		[dbo].[Customer].[VerCol] as RECV_VerCol
	,		[dbo].[Document].[DateTimeIssued]
	,		[dbo].[Document].[DateValidity]
	,		[dbo].[Document].[DeliveryTerms]
	,		[dbo].[Document].[DocumentType]
	,		[dbo].[Document].[DocumentTypeVersion]
	,		[dbo].[Document].[ExportPort]
	,		[dbo].[Document].[GrossWeight]
	,		[dbo].[Document].[InternalId]
	,		[dbo].[Document].[NetAmount]
	,		[dbo].[Document].[NetWeight]
	,		[dbo].[Document].[Packaging]
	,		[dbo].[Document].[PaymentTerms]
	,		[dbo].[Document].[ProformaInvoiceNumber]
	,		[dbo].[Document].[PurchaseOrderDescription]
	,		[dbo].[Document].[PurchaseOrderReference]
	,		[dbo].[Document].[SalesOrderDescription]
	,		[dbo].[Document].[SalesOrderReference]
	,		[dbo].[Document].[SwiftCode]
	,		[dbo].[Document].[TaxpayerActivityCode]
	,		[dbo].[Document].[TaxpayerId]
	,		[dbo].[TaxPayer].[AdditionalInformation] as ISS_AdditionalInformation
	,		[dbo].[TaxPayer].[BranchId] as ISS_BranchId
	,		[dbo].[TaxPayer].[BuildingNumber] as ISS_BuildingNumber
	,		[dbo].[TaxPayer].[Country] as ISS_Country
	,		[dbo].[TaxPayer].[Floor] as ISS_Floor
	,		[dbo].[TaxPayer].[Governate] as ISS_Governate
	,		[dbo].[TaxPayer].[Landmark] as ISS_Landmark
	,		[dbo].[TaxPayer].[Name] as ISS_Name
	,		[dbo].[TaxPayer].[PostalCode] as ISS_PostalCode
	,		[dbo].[TaxPayer].[RegionCity] as ISS_RegionCity
	,		[dbo].[TaxPayer].[Room] as ISS_Room
	,		[dbo].[TaxPayer].[Street] as ISS_Street
	,		[dbo].[TaxPayer].[Type] as ISS_Type
	,		[dbo].[TaxPayer].[VerCol] as ISS_VerCol
	,		[dbo].[Document].[TotalAmount]
	,		[dbo].[Document].[TotalDiscountAmount]
	,		[dbo].[Document].[TotalItemsDiscountAmount]
	,		[dbo].[Document].[TotalSalesAmount]
	,		[dbo].[Document].[VerCol]
	from [dbo].[Document]
	join [dbo].[Customer]
		on [dbo].[Document].[CustomerId] = [dbo].[Customer].[CustomerId]
	join [dbo].[TaxPayer]
		on [dbo].[TaxPayer].Id = [dbo].[Document].[TaxpayerId]
	where [dbo].[Document].[Id] = @Id;
	select *
	from dbo.[InvoiceLine]
	where [DocumentId] = @Id;
	select *
	from [dbo].[TaxableItem] where InvoiceLineId in (select Id from dbo.InvoiceLine where DocumentId = @Id);
end
go
if object_id(N'[dbo].[GetDocumentFromOracleByInternalId]','P') is not null
	drop procedure [dbo].[GetDocumentFromOracleByInternalId];
go
create procedure [dbo].[GetDocumentFromOracleByInternalId]
@DOC_InternalId as nvarchar(50),
@ISS_ID as nvarchar(30)
as
begin
	select cast(ISS_BRANCHID as nvarchar(50)) as ISS_BRANCHID
,	   cast(ISS_COUNTRY as nvarchar(2)) as ISS_COUNTRY
,	   cast(ISS_GOVERNATE as nvarchar(100)) as ISS_GOVERNATE
,	   cast(ISS_REGIONCITY as nvarchar(100)) as ISS_REGIONCITY
,	   cast(ISS_STREET as nvarchar(200)) as ISS_STREET
,	   cast(ISS_BLDGNO as nvarchar(100)) as ISS_BLDGNO
,		cast(ISS_POSTAL as nvarchar(30)) as ISS_POSTAL
,		cast(ISS_FLOOR as nvarchar(100)) as ISS_FLOOR
,		cast(ISS_ROOM as nvarchar(100)) as ISS_ROOM
,		cast(ISS_LANDMARK as nvarchar(500)) as ISS_LANDMARK
,		cast(ISS_ADDINFO as nvarchar(500)) as ISS_ADDINFO
,		cast(ISS_TYPE as nvarchar(2)) as ISS_TYPE
,		cast(ISS_ID as nvarchar(30)) as ISS_ID
,		cast(ISS_NAME as nvarchar(200)) as ISS_NAME
,		cast(RECV_COUNTRY as nvarchar(2)) as RECV_COUNTRY
,		cast(RECV_GOVERNATE as nvarchar(100)) as RECV_GOVERNATE
,		cast(RECV_REGIONCITY as nvarchar(100)) as RECV_REGIONCITY
,		cast(RECV_STREET as nvarchar(200)) as RECV_STREET
,		cast(RECV_BLDGNO as nvarchar(100)) as RECV_BLDGNO
,		cast(RECV_POSTAL as nvarchar(30)) as RECV_POSTAL
,		cast(RECV_FLOOR as nvarchar(100)) as RECV_FLOOR
,		cast(RECV_ROOM as nvarchar(100)) as RECV_ROOM
,		cast(RECV_LANDMARK as nvarchar(500)) as RECV_LANDMARK
,		cast(RECV_ADDINFO as nvarchar(500)) as RECV_ADDINFO
,		cast(RECV_TYPE as nvarchar(1)) as RECV_TYPE
,		cast(RECV_ID as nvarchar(30)) as RECV_ID
,		cast(RECV_NAME as nvarchar(200)) as RECV_NAME
,		cast(DOC_TYPE as nvarchar(20)) as DOC_TYPE
,		cast(DOC_TYPVER as nvarchar(100)) as DOC_TYPVER
,		cast(DOC_DATETIMEISS as datetime) as DOC_DATETIMEISS
,		cast(DOC_TAXPAYERACT as nvarchar(10)) as DOC_TAXPAYERACT
,		cast(DOC_INTERNALID as nvarchar(50)) as DOC_INTERNALID
,		cast(DOC_POREF as nvarchar(100)) as DOC_POREF
,		cast(DOC_PODESC as nvarchar(500)) as DOC_PODESC
,		cast(DOC_SOREF as nvarchar(100)) as DOC_SOREF
,		cast(DOC_SODESC as nvarchar(500)) as DOC_SODESC
,		cast(PROFORMAINVOICENUMBER as nvarchar(50)) as PROFORMAINVOICENUMBER
,		cast(PAY_SWIFTCODE as nvarchar(50)) as PAY_SWIFTCODE
,		cast(PAY_TERMS as nvarchar(500)) as PAY_TERMS
,		cast(DEL_APPROACH as nvarchar(100)) as DEL_APPROACH
,		cast(DEL_PACK as nvarchar(100)) as DEL_PACK
,		cast(DEL_DTVALID as datetime) as DEL_DTVALID
,		cast(DEL_EXP as nvarchar(100)) as DEL_EXP
,		cast(DEL_COUNTRY as nvarchar(100)) as DEL_COUNTRY
,		cast(isnull(DEL_GROSSWGHT,'0') as decimal(28,5)) as DEL_GROSSWGHT
,		cast(isnull(DEL_NETWGHT,'0') as decimal(28,5)) as DEL_NETWGHT
,		cast(DEL_TERMS as nvarchar(500)) as DEL_TERMS
,		cast(TOTALSALES as decimal(28,5)) as TOTALSALES
,		cast(TOTALDISCAMT as decimal(28,5)) as TOTALDISCAMT
,		cast(NETAMT as decimal(28,5)) as NETAMT
,		cast(EXTRADISC as decimal(28,5)) as EXTRADISC
,		cast(TOT_ITEMSDISCAMT as decimal(28,5)) as TOT_ITEMSDISCAMT
,		cast(TAXAMOUNT as decimal(28,5)) as TAXAMOUNT
,		cast(TOTAMT as decimal(28,5)) as TOTAMT
,		SIGNTYP
,		SIGNVAL
,		cast(SOPNUMBER as nvarchar(50)) as SOPNUMBER
,		SOPTYPE
from openquery([PROD],'
select  ''0'' as ISS_BRANCHID --0
 ,		eng_header.ISS_COUNTRY --1
 ,		eng_header.ISS_GOVERNATE --2
 ,		eng_header.ISS_REGIONCITY --3
 ,		eng_header.ISS_STREET --4
 ,		eng_header.ISS_BLDGNO --5
 ,		eng_header.ISS_POSTAL	--6
 ,		eng_header.ISS_FLOOR	--7
 ,		eng_header.ISS_ROOM		--8
 ,		eng_header.ISS_LANDMARK		--9
 ,		eng_header.ISS_ADDINFO		--10
 ,		eng_header.ISS_TYPE		--11
 ,		eng_header.ISS_ID		--12
 ,		eng_header.ISS_NAME		--13
 ,		eng_header.RECV_COUNTRY	--14
 ,		eng_header.RECV_GOVERNATE	--15
 ,		eng_header.RECV_REGIONCITY	--16
 ,		eng_header.RECV_STREET	--17
 ,		eng_header.RECV_BLDGNO	--18
 ,		eng_header.RECV_POSTAL		--19
 ,		eng_header.RECV_FLOOR		--20
 ,		eng_header.RECV_ROOM		--21
 ,		eng_header.RECV_LANDMARK	--22
 ,		eng_header.RECV_ADDINFO		--23
 ,		eng_header.RECV_TYPE		--24
 ,		eng_header.RECV_ID		--25
 ,		eng_header.RECV_NAME	--26
 ,		eng_header.DOC_TYPE		--27
 ,		eng_header.DOC_TYPVER	--28
,		eng_header.DOC_DATETIMEISS	--29
,		eng_header.DOC_TAXPAYERACT	--30
,		eng_header.DOC_INTERNALID	--31
,		eng_header.DOC_POREF		--32
,		eng_header.DOC_PODESC		--33
,		eng_header.DOC_SOREF		--34
,		eng_header.DOC_SODESC		--35
,		eng_header.PROFORMAINVOICENUMBER	--36
,		eng_header.PAY_SWIFTCODE		--37
,		eng_header.PAY_TERMS		--38
,		eng_header.DEL_APPROACH		--39
,		eng_header.DEL_PACK		--40
,		eng_header.DEL_DTVALID		--41
,		eng_header.DEL_EXP		--42
,		eng_header.DEL_COUNTRY		--43
,		eng_header.DEL_GROSSWGHT		--44
,		eng_header.DEL_NETWGHT	--45
,		eng_header.DEL_TERMS	--46
,		eng_header.TOTALSALES	--47
,		eng_header.TOTALSALESAMT	--48
,		eng_header.TOTALDISCAMT		--49
,		eng_header.NETAMT		--50
,		eng_header.EXTRADISC	--51
,		eng_header.TOT_ITEMSDISCAMT	--52
,		eng_header.TAXAMOUNT	--53
,		eng_header.TOTAMT		--54
,		eng_header.SIGNTYP		--55
,		eng_header.SIGNVAL		--56
,		eng_header.SOPNUMBER		--57
,		eng_header.SOPTYPE		--58
,		eng_header.CANCELLEDYN		--59
,		eng_header.TRX_DATE		--60
,		eng_header.CT_REFERENCE		--61
,		eng_header.DOCCURRENCYCODE	--62
,		eng_header.DOCCURRENCYXCHANGERATE	--63
from xxcite_tax_headers_eng_v eng_header	
union all
select  ''0'' as ISS_BRANCHID
 ,		eng_header.ISS_COUNTRY
 ,		eng_header.ISS_GOVERNATE
 ,		eng_header.ISS_REGIONCITY
 ,		eng_header.ISS_STREET
 ,		eng_header.ISS_BLDGNO
 ,		eng_header.ISS_POSTAL
 ,		eng_header.ISS_FLOOR
 ,		eng_header.ISS_ROOM
 ,		eng_header.ISS_LANDMARK
 ,		eng_header.ISS_ADDINFO
 ,		eng_header.ISS_TYPE
 ,		eng_header.ISS_ID
 ,		eng_header.ISS_NAME
,		eng_header.RECV_COUNTRY
,		eng_header.RECV_GOVERNATE
,		eng_header.RECV_REGIONCITY
,		eng_header.RECV_STREET
,		eng_header.RECV_BLDGNO
,		eng_header.RECV_POSTAL
,		eng_header.RECV_FLOOR
,		eng_header.RECV_ROOM
,		eng_header.RECV_LANDMARK
,		eng_header.RECV_ADDINFO
,		eng_header.RECV_TYPE
,		eng_header.RECV_ID
,		eng_header.RECV_NAME
,		eng_header.DOC_TYPE
,		eng_header.DOC_TYPVER
,		eng_header.DOC_DATETIMEISS
,		eng_header.DOC_TAXPAYERACT
,		eng_header.DOC_INTERNALID
,		eng_header.DOC_POREF
,		eng_header.DOC_PODESC
,		eng_header.DOC_SOREF
,		eng_header.DOC_SODESC
,		eng_header.PROFORMAINVOICENUMBER
,		eng_header.PAY_SWIFTCODE
,		eng_header.PAY_TERMS
,		eng_header.DEL_APPROACH
,		eng_header.DEL_PACK
,		eng_header.DEL_DTVALID
,		eng_header.DEL_EXP
,		eng_header.DEL_COUNTRY
,		eng_header.DEL_GROSSWGHT
,		eng_header.DEL_NETWGHT
,		eng_header.DEL_TERMS
,		eng_header.TOTALSALES
,		eng_header.TOTALSALESAMT
,		eng_header.TOTALDISCAMT
,		eng_header.NETAMT
,		eng_header.EXTRADISC
,		eng_header.TOT_ITEMSDISCAMT
,		eng_header.TAXAMOUNT
,		eng_header.TOTAMT
,		eng_header.SIGNTYP
,		eng_header.SIGNVAL
,		eng_header.SOPNUMBER
,		eng_header.SOPTYPE
,		eng_header.CANCELLEDYN
,		eng_header.TRX_DATE
,		eng_header.CT_REFERENCE
,		eng_header.DOCCURRENCYCODE
,		eng_header.DOCCURRENCYXCHANGERATE
from xxcite_tax_headers_home_v eng_header
union all
select  ''0'' as ISS_BRANCHID
 ,		eng_header.ISS_COUNTRY
 ,		eng_header.ISS_GOVERNATE
 ,		eng_header.ISS_REGIONCITY
 ,		eng_header.ISS_STREET
 ,		eng_header.ISS_BLDGNO
 ,		eng_header.ISS_POSTAL
 ,		eng_header.ISS_FLOOR
 ,		eng_header.ISS_ROOM
 ,		eng_header.ISS_LANDMARK
 ,		eng_header.ISS_ADDINFO
 ,		eng_header.ISS_TYPE
 ,		eng_header.ISS_ID
 ,		eng_header.ISS_NAME
,		eng_header.RECV_COUNTRY
,		eng_header.RECV_GOVERNATE
,		eng_header.RECV_REGIONCITY
,		eng_header.RECV_STREET
,		eng_header.RECV_BLDGNO
,		eng_header.RECV_POSTAL
,		eng_header.RECV_FLOOR
,		eng_header.RECV_ROOM
,		eng_header.RECV_LANDMARK
,		eng_header.RECV_ADDINFO
,		eng_header.RECV_TYPE
,		eng_header.RECV_ID
,		eng_header.RECV_NAME
,		eng_header.DOC_TYPE
,		eng_header.DOC_TYPVER
,		eng_header.DOC_DATETIMEISS
,		eng_header.DOC_TAXPAYERACT
,		eng_header.DOC_INTERNALID
,		eng_header.DOC_POREF
,		eng_header.DOC_PODESC
,		eng_header.DOC_SOREF
,		eng_header.DOC_SODESC
,		eng_header.PROFORMAINVOICENUMBER
,		eng_header.PAY_SWIFTCODE
,		eng_header.PAY_TERMS
,		eng_header.DEL_APPROACH
,		eng_header.DEL_PACK
,		eng_header.DEL_DTVALID
,		eng_header.DEL_EXP
,		eng_header.DEL_COUNTRY
,		eng_header.DEL_GROSSWGHT
,		eng_header.DEL_NETWGHT
,		eng_header.DEL_TERMS
,		eng_header.TOTALSALES
,		eng_header.TOTALSALESAMT
,		eng_header.TOTALDISCAMT
,		eng_header.NETAMT
,		eng_header.EXTRADISC
,		eng_header.TOT_ITEMSDISCAMT
,		eng_header.TAXAMOUNT
,		eng_header.TOTAMT
,		eng_header.SIGNTYP
,		eng_header.SIGNVAL
,		eng_header.SOPNUMBER
,		eng_header.SOPTYPE
,		eng_header.CANCELLEDYN
,		eng_header.TRX_DATE
,		eng_header.CT_REFERENCE
,		eng_header.DOCCURRENCYCODE
,		eng_header.DOCCURRENCYXCHANGERATE
from xxcite_tax_headers_v eng_header') as ora_header
where cast(ora_header.doc_internalid as nvarchar(50)) = @DOC_InternalId
and   ISS_ID = @ISS_ID
order by DOC_INTERNALID;

select cast(LIN_INTERNALCODE as nvarchar(50)) as LIN_INTERNALCODE
,		cast(LIN_UNTTYP as nvarchar(30)) as LIN_UNTTYP
,		cast(isnull(LIN_QTY,'0') as decimal(28,5)) as LIN_QTY
,		cast(isnull(LIN_CURSOLD,'0') as nvarchar(3)) as LIN_CURSOLD
,		cast(isnull(LIN_AMTEGP,'0') as decimal(28,5)) as LIN_AMTEGP
,		cast(isnull(LIN_AMTSOLD,'0') as decimal(28,5)) as LIN_AMTSOLD
,		cast(isnull(LIN_CURREXCH,'0') as decimal(28,5)) as LIN_CURREXCH
,		cast(isnull(SAL_TOT,'0') as decimal(28,5)) as SAL_TOT
,		cast(isnull(VAL_DIFF,'0') as decimal(28,5)) as VAL_DIFF
,		cast(isnull(TAXABLE_FEE,'0') as decimal(28,5)) as TAXABLE_FEE
,		cast(isnull(DISC_RATE,'0') as decimal(28,5)) as DISC_RATE
,		cast(isnull(DISC_AMT,'0') as decimal(28,5)) as DISC_AMT
,		cast(isnull(NET_TOT,'0') as decimal(28,5)) as NET_TOT
,		cast(isnull(LIN_TAXAMOUNT,'0') as decimal(28,5)) as LIN_TAXAMOUNT
,		cast(isnull(ITM_DISC,'0') as decimal(28,5)) as ITM_DISC
,		cast(LIN_ERPUNTTYP as nvarchar(30)) as LIN_ERPUNTTYP
,		cast(isnull(TOTAL,'0') as decimal(28,5)) as TOTAL
,		cast(LNITMSEQNM as nvarchar(50)) as LINE_ITEM_SEQ
,		LIN_ITEMCODE
,		cast(SOPNUMBER as nvarchar(50)) as SOPNUMBER
,		LIN_ITEMTYPE
,		cast(LIN_DESC as nvarchar(500)) as LIN_DESC
from openquery([PROD],'
select	eng_line.LIN_INTERNALCODE	--0
,		eng_line.LIN_UNTTYP		--1
,		eng_line.LIN_QTY		--2
,		eng_line.LIN_CURSOLD	--3
,		eng_line.LIN_AMTEGP		--4
,		eng_line.LIN_AMTSOLD	--5
,		eng_line.LIN_CURREXCH	--6
,		eng_line.SAL_TOT		--7
,		eng_line.VAL_DIFF		--8
,		eng_line.TAXABLE_FEE	--9
,		eng_line.DISC_RATE		--10
,		eng_line.DISC_AMT		--11
,		eng_line.NET_TOT		--12
,		eng_line.LIN_TAXAMOUNT	--13
,		eng_line.ITM_DISC		--14
,		eng_line.LIN_ERPUNTTYP	--15
,		eng_line.TOTAL			--16
,		cast(eng_line.LNITMSEQNM as varchar2(40))	as LNITMSEQNM	--17
,		cast(eng_line.LIN_ITEMCODE as varchar2(100)) as LIN_ITEMCODE	--18
,		eng_line.SOPNUMBER	--19
,		eng_line.LIN_ITEMTYPE	--20
,		eng_header.iss_id		--21
,		eng_line.LIN_DESC
from xxcite_tax_lines_eng_v eng_line
join xxcite_tax_headers_eng_v eng_header
	on eng_line.sopnumber = eng_header.sopnumber
union all
select	eng_line.LIN_INTERNALCODE
,		eng_line.LIN_UNTTYP
,		eng_line.LIN_QTY
,		eng_line.LIN_CURSOLD
,		eng_line.LIN_AMTEGP
,		eng_line.LIN_AMTSOLD
,		eng_line.LIN_CURREXCH
,		eng_line.SAL_TOT
,		eng_line.VAL_DIFF
,		eng_line.TAXABLE_FEE
,		eng_line.DISC_RATE
,		eng_line.DISC_AMT
,		eng_line.NET_TOT
,		eng_line.LIN_TAXAMOUNT
,		eng_line.ITM_DISC
,		eng_line.LIN_ERPUNTTYP
,		eng_line.TOTAL
,		cast(eng_line.LNITMSEQNM as varchar2(40))	as LNITMSEQNM
,		cast(eng_line.LIN_ITEMCODE as varchar2(100)) as LIN_ITEMCODE
,		eng_line.SOPNUMBER
,		eng_line.LIN_ITEMTYPE
,		eng_header.iss_id
,		eng_line.LIN_DESC
from xxcite_tax_lines_home_v eng_line
join xxcite_tax_headers_home_v eng_header
	on eng_line.sopnumber = eng_header.sopnumber
union all
select	eng_line.LIN_INTERNALCODE
,		eng_line.LIN_UNTTYP
,		eng_line.LIN_QTY
,		eng_line.LIN_CURSOLD
,		eng_line.LIN_AMTEGP
,		eng_line.LIN_AMTSOLD
,		eng_line.LIN_CURREXCH
,		eng_line.SAL_TOT
,		eng_line.VAL_DIFF
,		eng_line.TAXABLE_FEE
,		eng_line.DISC_RATE
,		eng_line.DISC_AMT
,		eng_line.NET_TOT
,		eng_line.LIN_TAXAMOUNT
,		eng_line.ITM_DISC
,		eng_line.LIN_ERPUNTTYP
,		eng_line.TOTAL
,		cast(eng_line.LNITMSEQNM as varchar2(40))	as LNITMSEQNM
,		cast(eng_line.LIN_ITEMCODE as varchar2(100)) as LIN_ITEMCODE
,		eng_line.SOPNUMBER
,		eng_line.LIN_ITEMTYPE
,		eng_header.iss_id
,		eng_line.LIN_DESC
from xxcite_tax_lines_v eng_line
join xxcite_tax_headers_v eng_header
	on eng_line.sopnumber = eng_header.sopnumber
') ora_lines
where cast(ora_lines.SOPNUMBER as nvarchar(50)) = @DOC_InternalId
and   ISS_ID = @ISS_ID
order by SOPNUMBER,LINE_ITEM_SEQ;

select cast(TAXPERCENT as decimal(28,5)) as TAXPERCENT
,		cast(TAXTYPE as nvarchar(30)) as TAXTYPE
,		cast(TAXSUBTYPE as nvarchar(50)) as TAXSUBTYPE
,		cast(AMT as decimal(28,5)) as AMT
,		cast(LINEITMSEQ as nvarchar(50)) as LINE_ITEM_SEQ
,		cast(SOPNUMBER as nvarchar(50)) as SOPNUMBER
from openquery([PROD],'
select  eng_details.TAXPERCENT	--0
,		eng_details.TAXTYPE	--1
,		eng_details.TAXSUBTYPE	--2
,		eng_details.AMT	--3
,		eng_details.LINEITMSEQ	--4
,		eng_details.sopnumber	--5
,		eng_header.ISS_ID		--6
from xxcite_tax_details_eng_v eng_details
join xxcite_tax_headers_eng_v eng_header
	on eng_header.sopnumber = eng_details.sopnumber
union all
select  eng_details.TAXPERCENT
,		eng_details.TAXTYPE
,		eng_details.TAXSUBTYPE
,		eng_details.AMT
,		eng_details.LINEITMSEQ
,		eng_details.sopnumber
,		eng_header.ISS_ID
from xxcite_tax_details_home_v eng_details
join xxcite_tax_headers_home_v eng_header
	on eng_details.sopnumber = eng_header.sopnumber
union all
select  eng_details.TAXPERCENT
,		eng_details.TAXTYPE
,		eng_details.TAXSUBTYPE
,		eng_details.AMT
,		eng_details.LINEITMSEQ
,		eng_details.sopnumber
,		eng_header.ISS_ID
from xxcite_tax_details_v eng_details
join xxcite_tax_headers_v eng_header
	on eng_details.sopnumber = eng_header.sopnumber
') ora_details
where cast(ora_details.sopnumber as nvarchar(50)) = @DOC_InternalId
and   ISS_ID = @ISS_ID
order by SOPNUMBER,LINE_ITEM_SEQ;
end
go
use EInvoice_Test
go
if object_id(N'[dbo].[GetUnsubmittedDocumentsFromOracle]','P') is not null
	drop procedure [dbo].[GetUnsubmittedDocumentsFromOracle];
go
create procedure [dbo].[GetUnsubmittedDocumentsFromOracle]
@TaxpayerId nvarchar(50),
@APIEnvId int
as
begin
	declare @documentId as int,@InternalId as nvarchar(50),@lineId as int;
	declare @DOC_DATETIMEISS as datetime,@PROFORMAINVOICENUMBER as nvarchar(50),@TOTALSALES as decimal(28,5),@TOTALDISCAMT as decimal(28,5),@NETAMT as decimal(28,5);
	declare @EXTRADISC as decimal(28,5),@TOT_ITEMSDISCAMT as decimal(28,5),@TOTAMT as decimal(28,5);
	declare @LIN_INTERNALCODE as nvarchar(50),
			@LIN_UNTTYP as nvarchar(30),
			@LIN_QTY as decimal(28,5),
			@LIN_CURSOLD nvarchar(3),
			@LIN_AMTEGP decimal(28,5),
			@LIN_AMTSOLD decimal(28,5),
			@LIN_CURREXCH decimal(28,5),
			@SAL_TOT decimal(28,5),
			@VAL_DIFF decimal(28,5),
			@TAXABLE_FEE decimal(28,5),
			@DISC_RATE decimal(28,5),
			@DISC_AMT decimal(28,5),
			@NET_TOT decimal(28,5),
			@LIN_TAXAMOUNT decimal(28,5),
			@ITM_DISC decimal(28,5),
			@LIN_ERPUNTTYP nvarchar(30),
			@TOTAL decimal(28,5),
			@LINE_ITEM_SEQ nvarchar(50),
			@LIN_ITEMCODE nvarchar(200),
			@SOPNUMBER nvarchar(50),
			@LIN_ITEMTYPE nvarchar(50),
			@LIN_DESC nvarchar(500);
	declare doc_head_cur cursor fast_forward for
	select	dbo.Document.Id
	,		dbo.Document.InternalId
	from dbo.Document
	left join dbo.DocumentSubmission s1
		on dbo.Document.Id = s1.DocumentId
	where dbo.Document.TaxpayerId = @TaxpayerId
	and s1.APIEnvironmentId = @APIEnvId
	and (s1.[UUID] is null
	or s1.[Status] in ('Invalid','Rejected','Cancelled'))
	and not exists (select 1 from dbo.DocumentSubmission where dbo.DocumentSubmission.DocumentId = dbo.Document.Id and dbo.DocumentSubmission.APIEnvironmentId = @APIEnvId and dbo.DocumentSubmission.[Status] in ('Valid','Submitted'));
	begin try
		begin transaction t1;
		open doc_head_cur;
		fetch next from doc_head_cur into @documentId,@internalId;
		while @@FETCH_STATUS = 0
		begin
			--update header....
			select  @DOC_DATETIMEISS = cast(DOC_DATETIMEISS as datetime)
			,		@PROFORMAINVOICENUMBER = cast(PROFORMAINVOICENUMBER as nvarchar(50))
			,		@TOTALSALES = cast(TOTALSALES as decimal(28,5))
			,		@TOTALDISCAMT = cast(TOTALDISCAMT as decimal(28,5))
			,		@NETAMT = cast(NETAMT as decimal(28,5))
			,		@EXTRADISC = cast(EXTRADISC as decimal(28,5))
			,		@TOT_ITEMSDISCAMT = cast(TOT_ITEMSDISCAMT as decimal(28,5))
			,		@TOTAMT = cast(TOTAMT as decimal(28,5))
			from openquery([PROD],'
				select  ''0'' as ISS_BRANCHID --0
				,		eng_header.ISS_COUNTRY --1
				,		eng_header.ISS_GOVERNATE --2
				,		eng_header.ISS_REGIONCITY --3
				,		eng_header.ISS_STREET --4
				,		eng_header.ISS_BLDGNO --5
				,		eng_header.ISS_POSTAL	--6
				,		eng_header.ISS_FLOOR	--7
				,		eng_header.ISS_ROOM		--8
				,		eng_header.ISS_LANDMARK		--9
				,		eng_header.ISS_ADDINFO		--10
				,		eng_header.ISS_TYPE		--11
				,		eng_header.ISS_ID		--12
				,		eng_header.ISS_NAME		--13
				,		eng_header.RECV_COUNTRY	--14
				,		eng_header.RECV_GOVERNATE	--15
				,		eng_header.RECV_REGIONCITY	--16
				,		eng_header.RECV_STREET	--17
				,		eng_header.RECV_BLDGNO	--18
				,		eng_header.RECV_POSTAL		--19
				,		eng_header.RECV_FLOOR		--20
				,		eng_header.RECV_ROOM		--21
				,		eng_header.RECV_LANDMARK	--22
				,		eng_header.RECV_ADDINFO		--23
				,		eng_header.RECV_TYPE		--24
				,		eng_header.RECV_ID		--25
				,		eng_header.RECV_NAME	--26
				,		eng_header.DOC_TYPE		--27
				,		eng_header.DOC_TYPVER	--28
				,		eng_header.DOC_DATETIMEISS	--29
				,		eng_header.DOC_TAXPAYERACT	--30
				,		eng_header.DOC_INTERNALID	--31
				,		eng_header.DOC_POREF		--32
				,		eng_header.DOC_PODESC		--33
				,		eng_header.DOC_SOREF		--34
				,		eng_header.DOC_SODESC		--35
				,		eng_header.PROFORMAINVOICENUMBER	--36
				,		eng_header.PAY_SWIFTCODE		--37
				,		eng_header.PAY_TERMS		--38
				,		eng_header.DEL_APPROACH		--39
				,		eng_header.DEL_PACK		--40
				,		eng_header.DEL_DTVALID		--41
				,		eng_header.DEL_EXP		--42
				,		eng_header.DEL_COUNTRY		--43
				,		eng_header.DEL_GROSSWGHT		--44
				,		eng_header.DEL_NETWGHT	--45
				,		eng_header.DEL_TERMS	--46
				,		eng_header.TOTALSALES	--47
				,		eng_header.TOTALSALESAMT	--48
				,		eng_header.TOTALDISCAMT		--49
				,		eng_header.NETAMT		--50
				,		eng_header.EXTRADISC	--51
				,		eng_header.TOT_ITEMSDISCAMT	--52
				,		eng_header.TAXAMOUNT	--53
				,		eng_header.TOTAMT		--54
				,		eng_header.SIGNTYP		--55
				,		eng_header.SIGNVAL		--56
				,		eng_header.SOPNUMBER		--57
				,		eng_header.SOPTYPE		--58
				,		eng_header.CANCELLEDYN		--59
				,		eng_header.TRX_DATE		--60
				,		eng_header.CT_REFERENCE		--61
				,		eng_header.DOCCURRENCYCODE	--62
				,		eng_header.DOCCURRENCYXCHANGERATE	--63
			from xxcite_tax_headers_eng_v eng_header	
			union all
			select  ''0'' as ISS_BRANCHID
			,		eng_header.ISS_COUNTRY
			,		eng_header.ISS_GOVERNATE
			,		eng_header.ISS_REGIONCITY
			,		eng_header.ISS_STREET
			,		eng_header.ISS_BLDGNO
			,		eng_header.ISS_POSTAL
			,		eng_header.ISS_FLOOR
			,		eng_header.ISS_ROOM
			,		eng_header.ISS_LANDMARK
			,		eng_header.ISS_ADDINFO
			,		eng_header.ISS_TYPE
			,		eng_header.ISS_ID
			,		eng_header.ISS_NAME
			,		eng_header.RECV_COUNTRY
			,		eng_header.RECV_GOVERNATE
			,		eng_header.RECV_REGIONCITY
			,		eng_header.RECV_STREET
			,		eng_header.RECV_BLDGNO
			,		eng_header.RECV_POSTAL
			,		eng_header.RECV_FLOOR
			,		eng_header.RECV_ROOM
			,		eng_header.RECV_LANDMARK
			,		eng_header.RECV_ADDINFO
			,		eng_header.RECV_TYPE
			,		eng_header.RECV_ID
			,		eng_header.RECV_NAME
			,		eng_header.DOC_TYPE
			,		eng_header.DOC_TYPVER
			,		eng_header.DOC_DATETIMEISS
			,		eng_header.DOC_TAXPAYERACT
			,		eng_header.DOC_INTERNALID
			,		eng_header.DOC_POREF
			,		eng_header.DOC_PODESC
			,		eng_header.DOC_SOREF
			,		eng_header.DOC_SODESC
			,		eng_header.PROFORMAINVOICENUMBER
			,		eng_header.PAY_SWIFTCODE
			,		eng_header.PAY_TERMS
			,		eng_header.DEL_APPROACH
			,		eng_header.DEL_PACK
			,		eng_header.DEL_DTVALID
			,		eng_header.DEL_EXP
			,		eng_header.DEL_COUNTRY
			,		eng_header.DEL_GROSSWGHT
			,		eng_header.DEL_NETWGHT
			,		eng_header.DEL_TERMS
			,		eng_header.TOTALSALES
			,		eng_header.TOTALSALESAMT
			,		eng_header.TOTALDISCAMT
			,		eng_header.NETAMT
			,		eng_header.EXTRADISC
			,		eng_header.TOT_ITEMSDISCAMT
			,		eng_header.TAXAMOUNT
			,		eng_header.TOTAMT
			,		eng_header.SIGNTYP
			,		eng_header.SIGNVAL
			,		eng_header.SOPNUMBER
			,		eng_header.SOPTYPE
			,		eng_header.CANCELLEDYN
			,		eng_header.TRX_DATE
			,		eng_header.CT_REFERENCE
			,		eng_header.DOCCURRENCYCODE
			,		eng_header.DOCCURRENCYXCHANGERATE
			from xxcite_tax_headers_home_v eng_header
			union all
			select  ''0'' as ISS_BRANCHID
			,		eng_header.ISS_COUNTRY
			,		eng_header.ISS_GOVERNATE
			,		eng_header.ISS_REGIONCITY
			,		eng_header.ISS_STREET
			,		eng_header.ISS_BLDGNO
			,		eng_header.ISS_POSTAL
			,		eng_header.ISS_FLOOR
			,		eng_header.ISS_ROOM
			,		eng_header.ISS_LANDMARK
			,		eng_header.ISS_ADDINFO
			,		eng_header.ISS_TYPE
			,		eng_header.ISS_ID
			,		eng_header.ISS_NAME
			,		eng_header.RECV_COUNTRY
			,		eng_header.RECV_GOVERNATE
			,		eng_header.RECV_REGIONCITY
			,		eng_header.RECV_STREET
			,		eng_header.RECV_BLDGNO
			,		eng_header.RECV_POSTAL
			,		eng_header.RECV_FLOOR
			,		eng_header.RECV_ROOM
			,		eng_header.RECV_LANDMARK
			,		eng_header.RECV_ADDINFO
			,		eng_header.RECV_TYPE
			,		eng_header.RECV_ID
			,		eng_header.RECV_NAME
			,		eng_header.DOC_TYPE
			,		eng_header.DOC_TYPVER
			,		eng_header.DOC_DATETIMEISS
			,		eng_header.DOC_TAXPAYERACT
			,		eng_header.DOC_INTERNALID
			,		eng_header.DOC_POREF
			,		eng_header.DOC_PODESC
			,		eng_header.DOC_SOREF
			,		eng_header.DOC_SODESC
			,		eng_header.PROFORMAINVOICENUMBER
			,		eng_header.PAY_SWIFTCODE
			,		eng_header.PAY_TERMS
			,		eng_header.DEL_APPROACH
			,		eng_header.DEL_PACK
			,		eng_header.DEL_DTVALID
			,		eng_header.DEL_EXP
			,		eng_header.DEL_COUNTRY
			,		eng_header.DEL_GROSSWGHT
			,		eng_header.DEL_NETWGHT
			,		eng_header.DEL_TERMS
			,		eng_header.TOTALSALES
			,		eng_header.TOTALSALESAMT
			,		eng_header.TOTALDISCAMT
			,		eng_header.NETAMT
			,		eng_header.EXTRADISC
			,		eng_header.TOT_ITEMSDISCAMT
			,		eng_header.TAXAMOUNT
			,		eng_header.TOTAMT
			,		eng_header.SIGNTYP
			,		eng_header.SIGNVAL
			,		eng_header.SOPNUMBER
			,		eng_header.SOPTYPE
			,		eng_header.CANCELLEDYN
			,		eng_header.TRX_DATE
			,		eng_header.CT_REFERENCE
			,		eng_header.DOCCURRENCYCODE
			,		eng_header.DOCCURRENCYXCHANGERATE
			from xxcite_tax_headers_v eng_header') as ora_header
		where ora_header.doc_internalid = @InternalId;
		update dbo.Document
		set dbo.document.DateTimeIssued = @DOC_DATETIMEISS,
			dbo.document.ExtraDiscountAmount = @EXTRADISC,
			dbo.document.NetAmount = @NETAMT,
			dbo.document.ProformaInvoiceNumber = @PROFORMAINVOICENUMBER,
			dbo.document.TotalAmount = @TOTAMT,
			dbo.document.TotalDiscountAmount = @TOTALDISCAMT,
			dbo.document.TotalItemsDiscountAmount = @TOT_ITEMSDISCAMT,
			dbo.document.TotalSalesAmount = @TOTALSALES
		where dbo.Document.Id = @documentId;
		--remove lines and taxable items....
		delete from dbo.TaxableItem where InvoiceLineId in (select Id from InvoiceLine where DocumentId = @documentId);
		delete from dbo.InvoiceLine where DocumentId = @documentId;
		--reload lines and taxable items.....
			declare doc_line_cur cursor fast_forward for
			select  cast(LIN_INTERNALCODE as nvarchar(50)) as LIN_INTERNALCODE
			,		cast(LIN_UNTTYP as nvarchar(30)) as LIN_UNTTYP
			,		cast(isnull(LIN_QTY,'0') as decimal(28,5)) as LIN_QTY
			,		cast(isnull(LIN_CURSOLD,'0') as nvarchar(3)) as LIN_CURSOLD
			,		cast(isnull(LIN_AMTEGP,'0') as decimal(28,5)) as LIN_AMTEGP
			,		cast(isnull(LIN_AMTSOLD,'0') as decimal(28,5)) as LIN_AMTSOLD
			,		cast(isnull(LIN_CURREXCH,'0') as decimal(28,5)) as LIN_CURREXCH
			,		cast(isnull(SAL_TOT,'0') as decimal(28,5)) as SAL_TOT
			,		cast(isnull(VAL_DIFF,'0') as decimal(28,5)) as VAL_DIFF
			,		cast(isnull(TAXABLE_FEE,'0') as decimal(28,5)) as TAXABLE_FEE
			,		cast(isnull(DISC_RATE,'0') as decimal(28,5)) as DISC_RATE
			,		cast(isnull(DISC_AMT,'0') as decimal(28,5)) as DISC_AMT
			,		cast(isnull(NET_TOT,'0') as decimal(28,5)) as NET_TOT
			,		cast(isnull(LIN_TAXAMOUNT,'0') as decimal(28,5)) as LIN_TAXAMOUNT
			,		cast(isnull(ITM_DISC,'0') as decimal(28,5)) as ITM_DISC
			,		cast(LIN_ERPUNTTYP as nvarchar(30)) as LIN_ERPUNTTYP
			,		cast(isnull(TOTAL,'0') as decimal(28,5)) as TOTAL
			,		cast(LNITMSEQNM as nvarchar(50)) as LINE_ITEM_SEQ
			,		LIN_ITEMCODE
			,		cast(SOPNUMBER as nvarchar(50)) as SOPNUMBER
			,		LIN_ITEMTYPE
			,		cast(LIN_DESC as nvarchar(500)) as LIN_DESC
			from openquery([PROD],'
			select	eng_line.LIN_INTERNALCODE	--0
			,		eng_line.LIN_UNTTYP		--1
			,		eng_line.LIN_QTY		--2
			,		eng_line.LIN_CURSOLD	--3
			,		eng_line.LIN_AMTEGP		--4
			,		eng_line.LIN_AMTSOLD	--5
			,		eng_line.LIN_CURREXCH	--6
			,		eng_line.SAL_TOT		--7
			,		eng_line.VAL_DIFF		--8
			,		eng_line.TAXABLE_FEE	--9
			,		eng_line.DISC_RATE		--10
			,		eng_line.DISC_AMT		--11
			,		eng_line.NET_TOT		--12
			,		eng_line.LIN_TAXAMOUNT	--13
			,		eng_line.ITM_DISC		--14
			,		eng_line.LIN_ERPUNTTYP	--15
			,		eng_line.TOTAL			--16
			,		cast(eng_line.LNITMSEQNM as varchar2(40))	as LNITMSEQNM	--17
			,		cast(eng_line.LIN_ITEMCODE as varchar2(100)) as LIN_ITEMCODE	--18
			,		eng_line.SOPNUMBER	--19
			,		eng_line.LIN_ITEMTYPE	--20
			,		eng_header.iss_id		--21
			,		eng_line.LIN_DESC
			from xxcite_tax_lines_eng_v eng_line
			join xxcite_tax_headers_eng_v eng_header
				on eng_line.sopnumber = eng_header.sopnumber
			union all
			select	eng_line.LIN_INTERNALCODE
			,		eng_line.LIN_UNTTYP
			,		eng_line.LIN_QTY
			,		eng_line.LIN_CURSOLD
			,		eng_line.LIN_AMTEGP
			,		eng_line.LIN_AMTSOLD
			,		eng_line.LIN_CURREXCH
			,		eng_line.SAL_TOT
			,		eng_line.VAL_DIFF
			,		eng_line.TAXABLE_FEE
			,		eng_line.DISC_RATE
			,		eng_line.DISC_AMT
			,		eng_line.NET_TOT
			,		eng_line.LIN_TAXAMOUNT
			,		eng_line.ITM_DISC
			,		eng_line.LIN_ERPUNTTYP
			,		eng_line.TOTAL
			,		cast(eng_line.LNITMSEQNM as varchar2(40))	as LNITMSEQNM
			,		cast(eng_line.LIN_ITEMCODE as varchar2(100)) as LIN_ITEMCODE
			,		eng_line.SOPNUMBER
			,		eng_line.LIN_ITEMTYPE
			,		eng_header.iss_id
			,		eng_line.LIN_DESC
			from xxcite_tax_lines_home_v eng_line
			join xxcite_tax_headers_home_v eng_header
				on eng_line.sopnumber = eng_header.sopnumber
			union all
			select	eng_line.LIN_INTERNALCODE
			,		eng_line.LIN_UNTTYP
			,		eng_line.LIN_QTY
			,		eng_line.LIN_CURSOLD
			,		eng_line.LIN_AMTEGP
			,		eng_line.LIN_AMTSOLD
			,		eng_line.LIN_CURREXCH
			,		eng_line.SAL_TOT
			,		eng_line.VAL_DIFF
			,		eng_line.TAXABLE_FEE
			,		eng_line.DISC_RATE
			,		eng_line.DISC_AMT
			,		eng_line.NET_TOT
			,		eng_line.LIN_TAXAMOUNT
			,		eng_line.ITM_DISC
			,		eng_line.LIN_ERPUNTTYP
			,		eng_line.TOTAL
			,		cast(eng_line.LNITMSEQNM as varchar2(40))	as LNITMSEQNM
			,		cast(eng_line.LIN_ITEMCODE as varchar2(100)) as LIN_ITEMCODE
			,		eng_line.SOPNUMBER
			,		eng_line.LIN_ITEMTYPE
			,		eng_header.iss_id
			,		eng_line.LIN_DESC
			from xxcite_tax_lines_v eng_line
			join xxcite_tax_headers_v eng_header
				on eng_line.sopnumber = eng_header.sopnumber
			') ora_lines
			where ora_lines.SOPNUMBER = @internalId;
			open doc_line_cur;
			fetch next from doc_line_cur into @LIN_INTERNALCODE,@LIN_UNTTYP,@LIN_QTY,@LIN_CURSOLD,@LIN_AMTEGP,@LIN_AMTSOLD ,@LIN_CURREXCH,@SAL_TOT,@VAL_DIFF,@TAXABLE_FEE,
			@DISC_RATE,@DISC_AMT,@NET_TOT,@LIN_TAXAMOUNT,@ITM_DISC,@LIN_ERPUNTTYP,@TOTAL,@LINE_ITEM_SEQ,@LIN_ITEMCODE,@SOPNUMBER,@LIN_ITEMTYPE,@LIN_DESC;
			while @@FETCH_STATUS = 0
			begin
				insert into dbo.InvoiceLine([InternalCode],[UnitType],[Quantity],[CurrencySold],[AmountEGP],[AmountSold],[CurrencyExchangeRate],[SalesTotal],[ValueDifference],[TotalTaxableFees],[DiscountRate],[DiscountAmount],[NetTotal],[ItemsDiscount],[Total],[ItemCode],[ItemType],[Description]) 
				values(@LIN_INTERNALCODE,@LIN_UNTTYP,@LIN_QTY,@LIN_CURSOLD,@LIN_AMTEGP,@LIN_AMTSOLD,@LIN_CURREXCH,@SAL_TOT,@VAL_DIFF,@TAXABLE_FEE,@DISC_RATE,@DISC_AMT,@NET_TOT,@ITM_DISC,@TOTAL,@LIN_ITEMCODE,@LIN_ITEMTYPE,@LIN_DESC);
				set @lineId = IDENT_CURRENT(N'dbo.InvoiceLine');
				insert into dbo.TaxableItem ([TaxType],[SubType],[Amount],[Rate],[InvoiceLineId])
				select  cast(TAXTYPE as nvarchar(30)) as TAXTYPE
				,		cast(TAXSUBTYPE as nvarchar(50)) as TAXSUBTYPE
				,		cast(AMT as decimal(28,5)) as AMT
				,		cast(TAXPERCENT as decimal(28,5)) as TAXPERCENT
				,		@lineId
				from openquery([PROD],'
				select  eng_details.TAXPERCENT	--0
				,		eng_details.TAXTYPE	--1
				,		eng_details.TAXSUBTYPE	--2
				,		eng_details.AMT	--3
				,		eng_details.LINEITMSEQ	--4
				,		eng_details.sopnumber	--5
				,		eng_header.ISS_ID		--6
				from xxcite_tax_details_eng_v eng_details
				join xxcite_tax_headers_eng_v eng_header
					on eng_header.sopnumber = eng_details.sopnumber
				union all
				select  eng_details.TAXPERCENT
				,		eng_details.TAXTYPE
				,		eng_details.TAXSUBTYPE
				,		eng_details.AMT
				,		eng_details.LINEITMSEQ
				,		eng_details.sopnumber
				,		eng_header.ISS_ID
				from xxcite_tax_details_home_v eng_details
				join xxcite_tax_headers_home_v eng_header
					on eng_details.sopnumber = eng_header.sopnumber
				union all
				select  eng_details.TAXPERCENT
				,		eng_details.TAXTYPE
				,		eng_details.TAXSUBTYPE
				,		eng_details.AMT
				,		eng_details.LINEITMSEQ
				,		eng_details.sopnumber
				,		eng_header.ISS_ID
				from xxcite_tax_details_v eng_details
				join xxcite_tax_headers_v eng_header
					on eng_details.sopnumber = eng_header.sopnumber
				') ora_details
				where cast(SOPNUMBER as nvarchar(50)) = @InternalId
				and cast(LINEITMSEQ as nvarchar(50)) = @LINE_ITEM_SEQ;
				fetch next from doc_line_cur into @LIN_INTERNALCODE,@LIN_UNTTYP,@LIN_QTY,@LIN_CURSOLD,@LIN_AMTEGP,@LIN_AMTSOLD ,@LIN_CURREXCH,@SAL_TOT,@VAL_DIFF,@TAXABLE_FEE,
				@DISC_RATE,@DISC_AMT,@NET_TOT,@LIN_TAXAMOUNT,@ITM_DISC,@LIN_ERPUNTTYP,@TOTAL,@LINE_ITEM_SEQ,@LIN_ITEMCODE,@SOPNUMBER,@LIN_ITEMTYPE,@LIN_DESC;
			end
			close doc_line_cur;
			deallocate doc_line_cur;
		fetch next from doc_head_cur into @documentId,@internalId;
		end
		close doc_head_cur;
		deallocate doc_head_cur;
		commit transaction t1;
		--finally Return Data
		select  [dbo].[Document].Approach
		,		[dbo].[Document].BankAccountIBAN
		,		[dbo].[Document].BankAccountNo
		,		[dbo].[Document].BankAddress
		,		[dbo].[Document].BankName
		,		[dbo].[Document].CountryOfOrigin
		,		[dbo].[Document].CustomerId
		,		[dbo].[Customer].Name as RECV_Name
		,		[dbo].[Customer].[Type] as RECV_Type
		,		[dbo].[Customer].VerCol as RECV_VerCol
		,		[dbo].[Customer].AdditionalInformation as RECV_AdditionalInformation
		,		[dbo].[Customer].BuildingNumber as RECV_BuildingNumber
		,		[dbo].[Customer].Country as RECV_Country
		,		[dbo].[Customer].[Floor] as RECV_Floor
		,		[dbo].[Customer].Governate as RECV_Governate
		,		[dbo].[Customer].Id as RECV_Id
		,		[dbo].[Customer].Landmark as RECV_Landmark
		,		[dbo].[Customer].PostalCode as RECV_PostalCode
		,		[dbo].[Customer].RegionCity as RECV_RegionCity
		,		[dbo].[Customer].Room as RECV_Room
		,		[dbo].[Customer].Street as RECV_Street
		,		[dbo].[Document].DateTimeIssued
		,		[dbo].[Document].DateValidity
		,		[dbo].[Document].DeliveryTerms
		,		[dbo].[Document].DocumentType
		,		[dbo].[Document].DocumentTypeVersion
		,		[dbo].[Document].ExportPort
		,		[dbo].[Document].ExtraDiscountAmount
		,		[dbo].[Document].GrossWeight
		,		[dbo].[Document].NetWeight
		,		[dbo].[Document].Id
		,		[dbo].[Document].InternalId
		,		[dbo].[Document].NetAmount
		,		[dbo].[Document].Packaging
		,		[dbo].[Document].PaymentTerms
		,		[dbo].[Document].ProformaInvoiceNumber
		,		[dbo].[Document].PurchaseOrderDescription
		,		[dbo].[Document].PurchaseOrderReference
		,		[dbo].[Document].SalesOrderDescription
		,		[dbo].[Document].SalesOrderReference
		,		[dbo].[Document].SwiftCode
		,		[dbo].[Document].TaxpayerActivityCode
		,		[dbo].[Document].TaxpayerId
		,		[dbo].TaxPayer.Name as ISS_Name
		,		[dbo].TaxPayer.[Type] as ISS_Type
		,		[dbo].TaxPayer.AdditionalInformation as ISS_AdditionalInformation
		,		[dbo].TaxPayer.BranchId as ISS_BranchId
		,		[dbo].TaxPayer.BuildingNumber as ISS_BuildingNumber
		,		[dbo].TaxPayer.Country as ISS_Country
		,		[dbo].TaxPayer.[Floor] as ISS_Floor
		,		[dbo].TaxPayer.Governate as ISS_Governate
		,		[dbo].TaxPayer.Landmark as ISS_Landmark
		,		[dbo].TaxPayer.PostalCode as ISS_PostalCode
		,		[dbo].TaxPayer.RegionCity as ISS_RegionCity
		,		[dbo].TaxPayer.Room as ISS_Room
		,		[dbo].TaxPayer.Street	as ISS_Street
		,		[dbo].[TaxPayer].[VerCol] as ISS_VerCol
		,		[dbo].[Document].TotalAmount
		,		[dbo].[Document].TotalDiscountAmount
		,		[dbo].[Document].TotalItemsDiscountAmount
		,		[dbo].[Document].TotalSalesAmount
		,		[dbo].[Document].VerCol
		from dbo.Document
		join  dbo.Customer
			on dbo.Document.CustomerId = dbo.Customer.CustomerId
		join dbo.TaxPayer
			on dbo.Document.TaxpayerId = dbo.TaxPayer.Id
		left join dbo.DocumentSubmission s1
			on dbo.Document.Id = s1.DocumentId
		where dbo.Document.TaxpayerId = @TaxpayerId
		and s1.APIEnvironmentId = @APIEnvId
		and (s1.[UUID] is null
		or s1.[Status] in ('Invalid','Rejected','Cancelled'))
		and not exists (select 1 from dbo.DocumentSubmission where dbo.DocumentSubmission.DocumentId = dbo.Document.Id and dbo.DocumentSubmission.APIEnvironmentId = @APIEnvId and dbo.DocumentSubmission.[Status] in ('Valid','Submitted'));
	end try
	begin catch
		rollback transaction t1;
		close doc_head_cur;
		deallocate doc_line_cur;
		throw;
	end catch
end
go
if object_id(N'[dbo].[FindDocumentSubmissionsByState]','P') is not null
	drop procedure [dbo].[FindDocumentSubmissionsByState]
go
create procedure [dbo].[FindDocumentSubmissionsByState]
@State as nvarchar(50),
@IssuerId as nvarchar(30),
@APIEnvId as int
as
begin
	select dbo.DocumentSubmission.APIEnvironmentId
	,		dbo.DocumentSubmission.DocumentId
	,		dbo.DocumentSubmission.[Status]
	,		dbo.DocumentSubmission.SubmissionDate
	,		dbo.DocumentSubmission.SubmissionUUID
	,		dbo.DocumentSubmission.UUID
	,		dbo.DocumentSubmission.VerCol
	,		dbo.Document.Approach
	,		dbo.Document.BankAccountIBAN
	,		dbo.Document.BankAccountNo
	,		dbo.Document.BankAddress
	,		dbo.Document.BankName
	,		dbo.Document.CountryOfOrigin
	,		dbo.Document.CustomerId
	,		dbo.Document.DateTimeIssued
	,		dbo.Document.DateValidity
	,		dbo.Document.DeliveryTerms
	,		dbo.Document.DocumentType
	,		dbo.Document.DocumentTypeVersion
	,		dbo.Document.ExportPort
	,		dbo.Document.ExtraDiscountAmount
	,		dbo.Document.GrossWeight
	,		dbo.Document.InternalId
	,		dbo.Document.NetAmount
	,		dbo.Document.NetWeight
	,		dbo.Document.Packaging
	,		dbo.Document.PaymentTerms
	,		dbo.Document.ProformaInvoiceNumber
	,		dbo.Document.PurchaseOrderDescription
	,		dbo.Document.PurchaseOrderReference
	,		dbo.Document.SalesOrderDescription
	,		dbo.Document.SalesOrderReference
	,		dbo.Document.SwiftCode
	,		dbo.Document.TaxpayerActivityCode
	,		dbo.Document.TaxpayerId
	,		dbo.Document.TotalAmount
	,		dbo.Document.TotalDiscountAmount
	,		dbo.Document.TotalItemsDiscountAmount
	,		dbo.Document.TotalSalesAmount
	,		dbo.Document.VerCol as DOC_VerCol
	,		dbo.TaxPayer.AdditionalInformation as ISS_AdditionalInformation
	,		dbo.TaxPayer.BranchId as ISS_BranchId
	,		dbo.TaxPayer.BuildingNumber as ISS_BuildingNumber
	,		dbo.TaxPayer.Country as ISS_Country
	,		dbo.TaxPayer.[Floor] as ISS_Floor
	,		dbo.TaxPayer.Governate as ISS_Governate
	,		dbo.TaxPayer.Landmark as ISS_Landmark
	,		dbo.TaxPayer.Name as ISS_Name
	,		dbo.TaxPayer.PostalCode as ISS_PostalCode
	,		dbo.TaxPayer.RegionCity as ISS_RegionCity
	,		dbo.TaxPayer.Room as ISS_Room
	,		dbo.TaxPayer.Street as ISS_Street
	,		dbo.TaxPayer.[Type] as ISS_Type
	,		dbo.TaxPayer.VerCol as ISS_VerCol
	,		dbo.Customer.AdditionalInformation as REC_AdditionalInformation
	,		dbo.Customer.BuildingNumber as REC_BuildingNumber
	,		dbo.Customer.Country as REC_Country
	,		dbo.Customer.[Floor] as REC_Floor
	,		dbo.Customer.Governate as REC_Governate
	,		dbo.Customer.Id as RECV_Id
	,		dbo.Customer.Landmark as RECV_Landmark
	,		dbo.Customer.Name as RECV_Name
	,		dbo.Customer.PostalCode as RECV_PostalCode
	,		dbo.Customer.RegionCity as RECV_RegionCity
	,		dbo.Customer.Room as RECV_Room
	,		dbo.Customer.Street as RECV_Street
	,		dbo.Customer.[Type] as RECV_Type
	,		dbo.Customer.VerCol as RECV_VerCol
	,		dbo.APIEnvironment.Name as APIENV_Name
	,		dbo.APIEnvironment.BaseUri as APIENV_BaseUri
	,		dbo.APIEnvironment.LogInUri as APIENV_LogInUri
	,		dbo.APIEnvironment.VerCol as APIENV_VerCol
	from dbo.DocumentSubmission
	join dbo.Document
		on dbo.Document.Id = dbo.DocumentSubmission.DocumentId
	join dbo.TaxPayer
		on dbo.TaxPayer.Id = dbo.Document.TaxpayerId
	join dbo.Customer
		on dbo.Customer.CustomerId = dbo.Document.CustomerId
	join dbo.APIEnvironment
		on dbo.APIEnvironment.Id = dbo.DocumentSubmission.APIEnvironmentId
	where dbo.DocumentSubmission.APIEnvironmentId = @APIEnvId
	and dbo.Document.TaxpayerId = @IssuerId
	and dbo.DocumentSubmission.[Status] = @State;
end
go
if object_id(N'[dbo].[FindInvoiceSummary]','P') is not null
	drop procedure [dbo].[FindInvoiceSummary];
go
create procedure [dbo].[FindInvoiceSummary]
@APIEnvironmentId int,
@TaxpayerId nvarchar(30),
@CustomerName nvarchar(200) = '%',
@IssueDateFrom DateTime = null,
@IssueDateTo DateTime = null
as
begin
	if @IssueDateFrom is null
		select @IssueDateFrom = min(DateTimeIssued) from dbo.Document;
	if @IssueDateTo is null
		select @IssueDateTo = max(DateTimeIssued) from dbo.Document;
	select *
	from [dbo].[InvoiceSummaryView]
	where dbo.InvoiceSummaryView.APIEnvironmentId = @APIEnvironmentId
	and dbo.InvoiceSummaryView.ISS_Id = @TaxpayerId
	and dbo.InvoiceSummaryView.RECV_Name like @CustomerName
	and dbo.InvoiceSummaryView.DateTimeIssued between @IssueDateFrom and @IssueDateTo
	order by dbo.InvoiceSummaryView.ProformaInvoiceNumber;
end
go
if object_id(N'[dbo].[InvoiceSummaryView]','V') is not null
	drop view [dbo].[InvoiceSummaryView]
go
create view [dbo].[InvoiceSummaryView]
as
	select  dbo.Document.ProformaInvoiceNumber
	,		dbo.Document.DateTimeIssued
	,		dbo.TaxPayer.Id as ISS_Id
	,		dbo.TaxPayer.Name as ISS_Name
	,		dbo.Customer.Id as RECV_Id
	,		dbo.Customer.Name as RECV_Name
	,		dbo.Document.TotalSalesAmount
	,		dbo.Document.TotalDiscountAmount
	,		dbo.Document.NetAmount
	,		sum(case when dbo.TaxableItem.TaxType='T1' then dbo.TaxableItem.Amount else 0 end) as ValueAddedTax
	,		sum(case when dbo.TaxableItem.TaxType='T3' then dbo.TaxableItem.Amount else 0 end) as TableTax
	,		sum(case when dbo.TaxableItem.TaxType='T4' then dbo.TaxableItem.Amount else 0 end) as WithHoldingTax
	,		dbo.Document.TotalAmount
	,		dbo.DocumentSubmission.UUID
	,		dbo.DocumentSubmission.[Status]
	,		dbo.DocumentSubmission.APIEnvironmentId
	,		dbo.APIEnvironment.Name as API_Name
	from dbo.Document
	join dbo.InvoiceLine
		on dbo.Document.Id = dbo.InvoiceLine.DocumentId
	join dbo.TaxableItem
		on dbo.TaxableItem.InvoiceLineId = dbo.InvoiceLine.Id
	join dbo.Customer
		on dbo.Document.CustomerId = dbo.Customer.CustomerId
	join dbo.TaxPayer
		on dbo.TaxPayer.Id = dbo.Document.TaxpayerId
	left join dbo.DocumentSubmission
		on dbo.DocumentSubmission.DocumentId = dbo.Document.Id
	left join dbo.APIEnvironment
		on dbo.DocumentSubmission.APIEnvironmentId = dbo.APIEnvironment.Id
	group by dbo.Document.ProformaInvoiceNumber
	,		dbo.Document.DateTimeIssued
	,		dbo.TaxPayer.Id
	,		dbo.TaxPayer.Name
	,		dbo.Customer.Id
	,		dbo.Customer.Name
	,		dbo.Document.TotalSalesAmount
	,		dbo.Document.TotalDiscountAmount
	,		dbo.Document.NetAmount
	,		dbo.Document.TotalAmount
	,		dbo.DocumentSubmission.UUID
	,		dbo.DocumentSubmission.[Status]
	,		dbo.DocumentSubmission.APIEnvironmentId
	,		dbo.APIEnvironment.Name
go
------------------------------------------------------ Create login -----------------------------------------------
USE [master]
GO
if SUSER_ID(N'kiriazi') is null
	CREATE LOGIN [kiriazi] WITH PASSWORD=N'kiriazi' , DEFAULT_DATABASE=[master]
GO
USE [EInvoice_Test]
GO
if USER_ID('kiriazi') is null
	CREATE USER [kiriazi] FOR LOGIN [kiriazi] WITH DEFAULT_SCHEMA = [dbo]
GO
USE [EInvoice_Test]
GO
ALTER ROLE [db_owner] ADD MEMBER [kiriazi]
GO
