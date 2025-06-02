using System;

namespace GoDataSyncServices.Models
{
    public class Clients
    {
        public Guid Id { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? CompanyId { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

/*
 CREATE TABLE [dbo].[Clients](
	[id] [uniqueidentifier] NOT NULL,
	[tenant_id] [uniqueidentifier] NULL,
	[company_id] [uniqueidentifier] NULL,
	[name] [nvarchar](255) NOT NULL,
	[created_at] [datetime2](7) NULL,
	[updated_at] [datetime2](7) NULL,
	[client_external_id] [nvarchar](255) NULL,
	[bill_email] [nvarchar](255) NULL,
	[address_line1] [nvarchar](255) NULL,
	[address_line2] [nvarchar](255) NULL,
	[bill_address_line_1] [nvarchar](255) NULL,
	[bill_address_line_2] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
 */