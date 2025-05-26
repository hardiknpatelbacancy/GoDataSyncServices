using System;

namespace GoDataSyncServices.Models
{
    public class Tasks
    {
        public Guid Id { get; set; }
        public Guid TenantsId { get; set; }
        public Guid CompaniesId { get; set; }
        public Guid ProjectsId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Priority { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Status { get; set; }
        public Guid? AssignedToId { get; set; }
        public Guid? CreatedById { get; set; }
        public decimal? EstimatedHours { get; set; }
        public decimal? ActualHours { get; set; }
        public bool? IsBillable { get; set; }
        public decimal? BillableRate { get; set; }
        public string Notes { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

/*
 CREATE TABLE dbo.Tasks(
	id uniqueidentifier NOT NULL,
	name nvarchar(255) NOT NULL,
	tenants_id uniqueidentifier NOT NULL,
	companies_id uniqueidentifier NOT NULL,
	parent_tasks_id uniqueidentifier NULL,
	locations_id uniqueidentifier NULL,
	projects_id uniqueidentifier NOT NULL,
	workflows_id uniqueidentifier NOT NULL,
	summary_contract_description nvarchar(max) NULL,
	summary_production_description nvarchar(max) NULL,
	detail_contract_description nvarchar(max) NULL,
	detail_production_description nvarchar(max) NULL,
	workflow_state int NOT NULL,
	priority float NULL,
	custom_sort nvarchar(100) NULL,
	sequence float NULL,
	promised_date datetime2(7) NULL,
	target_start_date datetime2(7) NULL,
	target_end_date datetime2(7) NULL,
	actual_end_date datetime2(7) NULL,
	production_clock_hours_allowed_per_day float NULL,
	story_points float NULL,
	budget_hours_total float NULL,
	billing_type int NULL,
	billing_rate_in_cents decimal(18, 2) NULL,
	billing_amount_in_cents decimal(18, 2) NULL,
	accounts_id nvarchar(100) NULL,
	deleted bit NULL,
	recurring_options_id uniqueidentifier NULL,
	recurring_state int NULL,
	project_manager_member_id uniqueidentifier NULL,
	sales_person_member_id uniqueidentifier NULL,
	estimator_member_id uniqueidentifier NULL,
	purchaser_member_id uniqueidentifier NULL,
	profit_center_id int NULL,
	branch_id int NULL,
	billing_batch_id int NULL,
	budget_occurrences int NULL,
	external_id nvarchar(100) NULL,
	created_at datetime2(7) NULL,
	updated_at datetime2(7) NULL,
PRIMARY KEY CLUSTERED 
(
	id ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON PRIMARY
) ON PRIMARY TEXTIMAGE_ON PRIMARY
GO
*/