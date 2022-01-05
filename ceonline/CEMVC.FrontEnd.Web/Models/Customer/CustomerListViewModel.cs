using System;
using System.Linq;

namespace CEMVC.FrontEnd.Web.Models
{
    public class CustomerListViewModel
    {
        public int Id { get; set; }
        public string PrimaryFirstName { get; set; }
        public string PrimaryLastName { get; set; }
        public string SecondaryFirstName { get; set; }
        public string SecondaryLastName { get; set; }
        public string Address { get; set; }
        public bool IsArch { get; set; }
        public DateTime? ChangedAt { get; set; }
        public bool IsCurrent { get; set; }

        public string Name
        {
            get
            {
                return (PrimaryFirstName + " " + PrimaryLastName).Trim() + (string.IsNullOrWhiteSpace(SecondaryFirstName) && string.IsNullOrWhiteSpace(SecondaryLastName) ? "" : " & ") + 
                    (SecondaryFirstName + " " + SecondaryLastName).Trim();
            }
        }        
        public static IQueryable<CustomerListViewModel> FromBO(IQueryable<CEMVC.Core.DAL.Customer> query, int? currentId = null)
        {
            return query.Select(c => new CustomerListViewModel {
                Id = c.id,
                PrimaryFirstName = c.first_name == null ? "" : c.first_name,
                PrimaryLastName = c.last_name == null ? "" : c.last_name,
                SecondaryFirstName = c.s_first_name == null ? "" : c.s_first_name,
                SecondaryLastName = c.s_last_name ==  null ? "" : c.s_last_name,
                Address = c.address,
                IsArch = c.is_archived,
                IsCurrent = currentId != null && c.id == currentId,
                ChangedAt = c.updated_at == null || c.Projects.Max(d => d.last_change_date) > c.updated_at ? c.Projects.Max(d => d.last_change_date) : c.updated_at
            });
        }

        public static CustomerListViewModel FromBO(CEMVC.Core.DAL.Customer c)
        {
            return new CustomerListViewModel
            {
                Id = c.id,
                PrimaryFirstName = c.first_name ?? "",
                PrimaryLastName = c.last_name ?? "",
                SecondaryFirstName = c.s_first_name ?? "",
                SecondaryLastName = c.s_last_name ?? "",
                Address = c.address,
                IsArch = c.is_archived,
                ChangedAt = c.updated_at,
            };
        }
    }

    public class ProjectListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Cost { get; set; }
        public bool IsArch { get; set; }
        public DateTime? ChangedAt { get; set; }

        public static IQueryable<ProjectListViewModel> FromBO(IQueryable<CEMVC.Core.DAL.Project> query)
        {
            return query.Select(p => new ProjectListViewModel { Id = p.id, Title = p.title, Cost = 0, IsArch = p.is_archived, ChangedAt = p.last_change_date })
                .OrderBy(q => q.IsArch)
                .ThenByDescending(q => q.ChangedAt);
        }
    }
}