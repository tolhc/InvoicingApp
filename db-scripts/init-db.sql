create database InvoicingDb collate SQL_Latin1_General_CP1_CI_AS
go

use InvoicingDb
go

grant connect on database :: InvoicingDb to dbo
go

grant view any column encryption key definition, view any column master key definition on database :: InvoicingDb to [public]
go

create table dbo.Companies
(
    Id   uniqueidentifier not null
        primary key,
    Name nvarchar(100)    not null
)
go

create table dbo.Invoices
(
    Id             uniqueidentifier not null
        primary key,
    DateIssued            datetime         not null,
    NetAmount             float            not null,
    VatAmount             float            not null,
    TotalAmount           float            not null,
    Description           nvarchar(255)    not null,
    CompanyId             uniqueidentifier not null
        references dbo.Companies,
    CounterPartyCompanyId uniqueidentifier not null
        references dbo.Companies
)
go

create table dbo.Users
(
    Id        uniqueidentifier not null
        primary key,
    CompanyId uniqueidentifier not null
        references dbo.Companies,
    Name      nvarchar(100)    not null
)

go


INSERT [dbo].[Companies] ([Id], [Name]) VALUES (N'0cdf57e7-04b0-4804-926f-0bc851270653', N'Company Name 4')
GO
INSERT [dbo].[Companies] ([Id], [Name]) VALUES (N'91b5d61b-31e4-446c-bc86-0e2bb0849916', N'Company Name 3')
GO
INSERT [dbo].[Companies] ([Id], [Name]) VALUES (N'bd2717f2-bae0-43ca-ac43-43f48e6a1397', N'Company Name 6')
GO
INSERT [dbo].[Companies] ([Id], [Name]) VALUES (N'79e45fc1-902e-4f7c-9a3a-6590ea047710', N'Company Name 1')
GO
INSERT [dbo].[Companies] ([Id], [Name]) VALUES (N'39f87698-e181-4c5f-b2ba-b45b551e9e50', N'Company Name 2')
GO
INSERT [dbo].[Companies] ([Id], [Name]) VALUES (N'0a22cece-6c1a-4195-a872-c8bc35769461', N'Company Name 5')
GO
INSERT [dbo].[Users] ([Id], [CompanyId], [Name]) VALUES (N'099abb0b-33ba-4374-940e-1d4f75c3ed99', N'91b5d61b-31e4-446c-bc86-0e2bb0849916', N'UserName 3')
GO
INSERT [dbo].[Users] ([Id], [CompanyId], [Name]) VALUES (N'a488b697-56cb-43cb-8ff1-23dac263da80', N'0cdf57e7-04b0-4804-926f-0bc851270653', N'UserName 1')
GO
INSERT [dbo].[Users] ([Id], [CompanyId], [Name]) VALUES (N'6ad3664c-0a7c-412d-ad54-8096f784e991', N'0cdf57e7-04b0-4804-926f-0bc851270653', N'UserName 2')
GO
INSERT [dbo].[Users] ([Id], [CompanyId], [Name]) VALUES (N'2435bee5-abe9-4fd8-a99b-90e6f19d3be5', N'79e45fc1-902e-4f7c-9a3a-6590ea047710', N'UserName 4')
GO
INSERT [dbo].[Users] ([Id], [CompanyId], [Name]) VALUES (N'04327578-0eb4-4fd3-9282-fe95a7035d08', N'91b5d61b-31e4-446c-bc86-0e2bb0849916', N'UserName 5')
GO
INSERT [dbo].[Invoices] ([Id], [DateIssued], [NetAmount], [VatAmount], [TotalAmount], [Description], [CompanyId], [CounterPartyCompanyId]) VALUES (N'a6f431b7-82cf-40f9-bed5-10d7aca30029', CAST(N'2024-11-20T00:00:00.000' AS DateTime), CAST(5.300 AS Decimal(18, 3)), CAST(1.200 AS Decimal(18, 3)), CAST(6.500 AS Decimal(18, 3)), N'Invoice Description 2 from Company 6 to Company 2', N'bd2717f2-bae0-43ca-ac43-43f48e6a1397', N'39f87698-e181-4c5f-b2ba-b45b551e9e50')
GO
INSERT [dbo].[Invoices] ([Id], [DateIssued], [NetAmount], [VatAmount], [TotalAmount], [Description], [CompanyId], [CounterPartyCompanyId]) VALUES (N'e1ebcd63-9276-425f-a7f9-239dbb8c596c', CAST(N'2024-11-18T00:00:00.000' AS DateTime), CAST(5.100 AS Decimal(18, 3)), CAST(1.100 AS Decimal(18, 3)), CAST(6.200 AS Decimal(18, 3)), N'Invoice Description from Company 1 to Company 2', N'79e45fc1-902e-4f7c-9a3a-6590ea047710', N'39f87698-e181-4c5f-b2ba-b45b551e9e50')
GO
INSERT [dbo].[Invoices] ([Id], [DateIssued], [NetAmount], [VatAmount], [TotalAmount], [Description], [CompanyId], [CounterPartyCompanyId]) VALUES (N'cd25c2eb-127c-4153-954e-3ccc78557de3', CAST(N'2024-11-16T00:00:00.000' AS DateTime), CAST(7.100 AS Decimal(18, 3)), CAST(2.000 AS Decimal(18, 3)), CAST(9.100 AS Decimal(18, 3)), N'Invoice Description 4 from Company 6 to Company 2', N'bd2717f2-bae0-43ca-ac43-43f48e6a1397', N'39f87698-e181-4c5f-b2ba-b45b551e9e50')
GO
INSERT [dbo].[Invoices] ([Id], [DateIssued], [NetAmount], [VatAmount], [TotalAmount], [Description], [CompanyId], [CounterPartyCompanyId]) VALUES (N'11c6d044-81e4-4bff-b83c-60b2d3f17d3b', CAST(N'2024-11-21T00:00:00.000' AS DateTime), CAST(4.200 AS Decimal(18, 3)), CAST(0.500 AS Decimal(18, 3)), CAST(4.700 AS Decimal(18, 3)), N'Invoice Description from Company 2 to Company 1', N'39f87698-e181-4c5f-b2ba-b45b551e9e50', N'79e45fc1-902e-4f7c-9a3a-6590ea047710')
GO
INSERT [dbo].[Invoices] ([Id], [DateIssued], [NetAmount], [VatAmount], [TotalAmount], [Description], [CompanyId], [CounterPartyCompanyId]) VALUES (N'9b56e2e3-b51b-4a26-af14-82b3f1e68e94', CAST(N'2024-11-20T00:00:00.000' AS DateTime), CAST(100.340 AS Decimal(18, 3)), CAST(14.000 AS Decimal(18, 3)), CAST(114.340 AS Decimal(18, 3)), N'Invoice Description from Company 1 to Company 5', N'79e45fc1-902e-4f7c-9a3a-6590ea047710', N'0a22cece-6c1a-4195-a872-c8bc35769461')
GO
INSERT [dbo].[Invoices] ([Id], [DateIssued], [NetAmount], [VatAmount], [TotalAmount], [Description], [CompanyId], [CounterPartyCompanyId]) VALUES (N'76ccabb8-2fde-45fd-b6cd-8e21ec5cc5c4', CAST(N'2024-11-20T00:00:00.000' AS DateTime), CAST(12.500 AS Decimal(18, 3)), CAST(5.850 AS Decimal(18, 3)), CAST(18.350 AS Decimal(18, 3)), N'Invoice Description from Company 1 to Company 3', N'79e45fc1-902e-4f7c-9a3a-6590ea047710', N'91b5d61b-31e4-446c-bc86-0e2bb0849916')
GO
INSERT [dbo].[Invoices] ([Id], [DateIssued], [NetAmount], [VatAmount], [TotalAmount], [Description], [CompanyId], [CounterPartyCompanyId]) VALUES (N'142b42a2-7832-46a7-9ddc-c0cc805816cb', CAST(N'2024-11-19T00:00:00.000' AS DateTime), CAST(1.600 AS Decimal(18, 3)), CAST(0.140 AS Decimal(18, 3)), CAST(1.740 AS Decimal(18, 3)), N'Invoice Description from Company 6 to Company 2', N'bd2717f2-bae0-43ca-ac43-43f48e6a1397', N'39f87698-e181-4c5f-b2ba-b45b551e9e50')
GO
INSERT [dbo].[Invoices] ([Id], [DateIssued], [NetAmount], [VatAmount], [TotalAmount], [Description], [CompanyId], [CounterPartyCompanyId]) VALUES (N'558f73de-15b1-4614-ab65-cdd8cbd10784', CAST(N'2024-11-30T00:00:00.000' AS DateTime), CAST(133.000 AS Decimal(18, 3)), CAST(12.000 AS Decimal(18, 3)), CAST(145.000 AS Decimal(18, 3)), N'Invoice Description 3 from Company 6 to Company 2', N'bd2717f2-bae0-43ca-ac43-43f48e6a1397', N'39f87698-e181-4c5f-b2ba-b45b551e9e50')
GO
INSERT [dbo].[Invoices] ([Id], [DateIssued], [NetAmount], [VatAmount], [TotalAmount], [Description], [CompanyId], [CounterPartyCompanyId]) VALUES (N'e3d4ef3a-8361-45c2-a78f-dab1fa8db6f6', CAST(N'2024-11-15T00:00:00.000' AS DateTime), CAST(1002.320 AS Decimal(18, 3)), CAST(824.100 AS Decimal(18, 3)), CAST(1826.420 AS Decimal(18, 3)), N'Invoice Description from Company 2 to Company 3', N'39f87698-e181-4c5f-b2ba-b45b551e9e50', N'91b5d61b-31e4-446c-bc86-0e2bb0849916')
GO

