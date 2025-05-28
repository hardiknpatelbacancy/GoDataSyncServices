using System;

namespace GoDataSyncServices.Models
{
    public class Locations
    {
        public Guid Id { get; set; }
        public string Address { get; set; }
        public Guid TenantsId { get; set; }
        public Guid CompaniesId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string AccountsId { get; set; }
        public bool? Deleted { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

/*
 CREATE TABLE [dbo].[Locations](
	[id] [uniqueidentifier] NOT NULL,
	[address] [nvarchar](max) NOT NULL,
	[tenants_id] [uniqueidentifier] NOT NULL,
	[companies_id] [uniqueidentifier] NOT NULL,
	[latitude] [float] NULL,
	[longitude] [float] NULL,
	[accounts_id] [nvarchar](100) NULL,
	[deleted] [bit] NULL,
	[created_at] [datetime2](7) NULL,
	[updated_at] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
 */