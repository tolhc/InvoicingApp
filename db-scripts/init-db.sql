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
    InvoiceId             uniqueidentifier not null
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

