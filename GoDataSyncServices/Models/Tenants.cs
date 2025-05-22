using Microsoft.AspNetCore.Http.HttpResults;
using System;

namespace GoDataSyncServices.Models
{
    public class Tenants
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string? owner_email { get; set; }
        public string? owner_name { get; set; }
        public bool? enabled { get; set; } = true;
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}

// tables script file
//    CREATE TABLE[dbo].[Tenants]
//        (
//    [id][uniqueidentifier] NOT NULL,
//    [name] [nvarchar] (255) NOT NULL,
//    [enabled] [bit] NULL,
//	[created_at][datetime2] (7) NULL,
//	[updated_at][datetime2] (7) NULL,
//	[owner_email][nvarchar] (255) NULL,
//	[owner_name][nvarchar] (255) NULL,
//PRIMARY KEY CLUSTERED
//(
//    [id] ASC
//)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON[PRIMARY]
//) ON[PRIMARY]
//GO