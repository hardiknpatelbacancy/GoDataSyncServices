using System;
using System.Xml.Linq;

namespace GoDataSyncServices.Models
{
    public class Companies
    {
        public Guid id { get; set; }
        public Guid? tenant_id { get; set; }
        public string name { get; set; }
        public bool? enabled { get; set; }
        public bool? is_deleted { get; set; }
        public bool? attach_invoices { get; set; }
        public string general_labor_id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}

/*
 CREATE TABLE [dbo].[Companies](
	[id] [uniqueidentifier] NOT NULL,
	[tenant_id] [uniqueidentifier] NULL,
	[name] [nvarchar](255) NOT NULL,
	[enabled] [bit] NULL,
	[is_deleted] [bit] NULL,
	[attach_invoices] [bit] NULL,
	[general_labor_id] [nvarchar](255) NULL,
	[created_at] [datetime2](7) NULL,
	[updated_at] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
*/