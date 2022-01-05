using CEMVC.Core.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEMVC.FrontEnd.Web.Models
{
    public struct Contact
    {
        public string Name { get; set; }
        public string Value { get; set; }
        
        public Contact(string val, string name)
        {
            Name = name;
            Value = val;
        }
    }

    public struct Address
    {
        public string FirstLine { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public Address(string addr, string city, string state, string zip)
        {
            FirstLine = addr;
            City = city;
            State = state;
            Zip = zip;
        }

        public override string ToString()
        {
            return string.Join(" ", string.Join(", ", new string[] { FirstLine, City, State }.Where(s => !string.IsNullOrEmpty(s))), Zip ?? "");
        }
    }

    public class CustomerViewModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                return string.Join(" ", FirstName, LastName);
            }
        }

        public string FirstNameAlt { get; set; }

        public string LastNameAlt { get; set; }

        public string FullNameAlt
        {
            get
            {
                return string.Join(" ", FirstNameAlt, LastNameAlt);
            }
        }

        public Address Address { get; set; }
        public IList<KeyValuePair<string, string>> Phones { get; set; }
        public IList<KeyValuePair<string, string>> Emails { get; set; }

        public string Notes { get; set; }

        //public IEnumerable<ProjectListViewModel> Projects { get; set; }

        public bool IsArchived { get; set; }

        public DateTime? ZapierSentDate { get; set; }

        public static CustomerViewModel FromBO(Customer d)
        {
            return new CustomerViewModel
            {
                Id = d.id,
                FirstName = d.first_name,
                LastName = d.last_name,
                FirstNameAlt = d.s_first_name,
                LastNameAlt = d.s_last_name,

                Address = new Address(d.address, d.city, d.state, d.zip),

                Phones = new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("Home", d.phone_home),
                    new KeyValuePair<string, string>("Business", d.phone_business),
                    new KeyValuePair<string, string>("Fax", d.phone_fax),
                    new KeyValuePair<string, string>("Mobile", d.phone_mobile),
                    new KeyValuePair<string, string>("Other", d.phone_other)
                }.Where(c => !string.IsNullOrEmpty(c.Value)).ToArray(),

                Emails = new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("Personal", d.email_personal),
                    new KeyValuePair<string, string>("Business", d.email_business),
                    new KeyValuePair<string, string>("Instant Messenger", d.email_instant_messenger),
                    new KeyValuePair<string, string>("Other", d.email_other)
                }.Where(c => !string.IsNullOrEmpty(c.Value)).ToArray(),

                Notes = d.notes,
                IsArchived = d.is_archived
            };
        }
    }
}