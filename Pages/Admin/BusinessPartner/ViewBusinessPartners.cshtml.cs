using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Lab2_Johnson_Imlay_Freeman.Pages.Admin.BusinessPartners
{
    public class ViewBusinessPartnersModel : PageModel
    {
        private readonly string _connectionString = "Server=localhost;Database=Lab1;Trusted_Connection=True;";

        public List<BusinessPartnerModel> BusinessPartners { get; set; } = new();

        public void OnGet()
        {
            LoadBusinessPartners();
        }

        private void LoadBusinessPartners()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT BusinessPartnerID, Name, OrgType, PrimaryContact, ActiveStatus 
                    FROM BusinessPartner", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        BusinessPartners.Add(new BusinessPartnerModel
                        {
                            BusinessPartnerID = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            OrgType = reader.GetString(2),
                            PrimaryContact = reader.IsDBNull(3) ? "N/A" : reader.GetString(3),
                            ActiveStatus = reader.GetBoolean(4)
                        });
                    }
                }
            }
        }

        public class BusinessPartnerModel
        {
            public int BusinessPartnerID { get; set; }
            public string Name { get; set; } = "";
            public string OrgType { get; set; } = "";
            public string PrimaryContact { get; set; } = "";
            public bool ActiveStatus { get; set; }
        }
    }
}
