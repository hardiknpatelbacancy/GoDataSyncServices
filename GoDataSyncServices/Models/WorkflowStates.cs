using System;

namespace GoDataSyncServices.Models
{
    public class WorkflowStates
    {
        public Guid Id { get; set; }
        public Guid TenantsId { get; set; }
        public Guid CompaniesId { get; set; }
        public Guid WorkflowsId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Order { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

/*
 CREATE TABLE [dbo].[WorkflowStates](
	[id] [uniqueidentifier] NOT NULL,
	[workflows_id] [uniqueidentifier] NOT NULL,
	[state_name] [nvarchar](100) NOT NULL,
	[state_order] [int] NOT NULL,
	[created_at] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
 */