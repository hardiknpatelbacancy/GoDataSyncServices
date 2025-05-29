using System;

namespace GoDataSyncServices.Models
{
    public class Workflows
    {
        public Guid Id { get; set; }
        public Guid TenantsId { get; set; }
        public Guid CompaniesId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

/*
 CREATE TABLE [dbo].[Workflows](
	[id] [uniqueidentifier] NOT NULL,
	[tenants_id] [uniqueidentifier] NOT NULL,
	[companies_id] [uniqueidentifier] NOT NULL,
	[summary_description] [nvarchar](max) NULL,
	[detail_description] [nvarchar](max) NULL,
	[sort_order] [float] NULL,
	[accounts_id] [nvarchar](100) NULL,
	[deleted] [bit] NULL,
	[workflow_state_completed] [int] NULL,
	[created_at] [datetime2](7) NULL,
	[updated_at] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
 */