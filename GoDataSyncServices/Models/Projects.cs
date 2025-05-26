using System;

namespace GoDataSyncServices.Models
{
    public class Projects
    {
        public Guid Id { get; set; }
        public Guid TenantsId { get; set; }
        public Guid CompaniesId { get; set; }
        public string ExternalProjectId { get; set; }
        public string ExternalDivisionId { get; set; }
        public string ProjectName { get; set; }
        public Guid? LocationsId { get; set; }
        public string SummaryDescription { get; set; }
        public string DetailDescription { get; set; }
        public Guid WorkflowsId { get; set; }
        public int WorkflowState { get; set; }
        public Guid ClientsId { get; set; }
        public string ProjectManagerId { get; set; }
        public Guid? ParentProjectsId { get; set; }
        public DateTime? StartDateTarget { get; set; }
        public DateTime? EndDateTarget { get; set; }
        public string AccountsId { get; set; }
        public bool? Deleted { get; set; }
        public Guid? ResourceProjectsId { get; set; }
        public Guid? ResourceTasksId { get; set; }
        public Guid? TravelProjectsId { get; set; }
        public Guid? TravelTasksId { get; set; }
        public int? DivisionsId { get; set; }
        public int? BranchId { get; set; }
        public int? DefaultTaxAuthorityId { get; set; }
        public string ClientsName { get; set; }
        public bool? TransactionMade { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

/*
 CREATE TABLE [dbo].[Projects](
	[id] [uniqueidentifier] NOT NULL,
	[tenants_id] [uniqueidentifier] NOT NULL,
	[companies_id] [uniqueidentifier] NOT NULL,
	[external_project_id] [nvarchar](100) NULL,
	[external_division_id] [nvarchar](100) NULL,
	[project_name] [nvarchar](255) NOT NULL,
	[locations_id] [uniqueidentifier] NULL,
	[summary_description] [nvarchar](max) NULL,
	[detail_description] [nvarchar](max) NULL,
	[workflows_id] [uniqueidentifier] NOT NULL,
	[workflow_state] [int] NOT NULL,
	[clients_id] [uniqueidentifier] NOT NULL,
	[project_manager_id] [nvarchar](100) NOT NULL,
	[parent_projects_id] [uniqueidentifier] NULL,
	[start_date_target] [datetime2](7) NULL,
	[end_date_target] [datetime2](7) NULL,
	[accounts_id] [nvarchar](100) NULL,
	[deleted] [bit] NULL,
	[resource_projects_id] [uniqueidentifier] NULL,
	[resource_tasks_id] [uniqueidentifier] NULL,
	[travel_projects_id] [uniqueidentifier] NULL,
	[travel_tasks_id] [uniqueidentifier] NULL,
	[divisions_id] [int] NULL,
	[branch_id] [int] NULL,
	[default_tax_authority_id] [int] NULL,
	[clients_name] [nvarchar](255) NULL,
	[transaction_made] [bit] NULL,
	[created_at] [datetime2](7) NULL,
	[updated_at] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
 */