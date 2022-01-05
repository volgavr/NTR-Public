using CEMVC.Core.DAL;
using CEMVC.Utility.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CEMVC.FrontEnd.Web.Models
{
    public struct CustomerContact
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public string Title { get; private set; }

        public CustomerContact(string val, PhoneContactEnum label) : this()
        {
            Value = val;
            Label = label.ToString();
            Title = Label;
            Id = (int)label;
        }
        public CustomerContact(string val, EmailContactEnum label) : this()
        {
            Value = val;
            Label = label.ToString();
            Title = label.GetDescription(Label);
            Id = (int)label;
        }
    }

    public class CustomerEditModel : IValidatableObject
    {
        [Required]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Primary First Name:")]
        [StringLength(50, ErrorMessage = "The {0} must be not more than {1} characters long.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Primary Last Name:")]
        [StringLength(50, ErrorMessage = "The {0} must be not more than {1} characters long.")]
        public string LastName { get; set; }

        [Display(Name = "Secondary First Name:")]
        [StringLength(50, ErrorMessage = "The {0} must be not more than {1} characters long.")]
        public string SecondaryFirstName { get; set; }

        [Display(Name = "Secondary Last Name:")]
        [StringLength(50, ErrorMessage = "The {0} must be not more than {1} characters long.")]
        public string SecondaryLastName { get; set; }

        [Display(Name = "Street Address:")]
        [StringLength(255, ErrorMessage = "The {0} must be not more than {1} characters long.")]
        public string Address { get; set; }

        [Display(Name = "City:")]
        [StringLength(50, ErrorMessage = "The {0} must be not more than {1} characters long.")]
        public string City { get; set; }

        [Display(Name = "State/Prov:")]
        [StringLength(50, ErrorMessage = "The {0} must be not more than {1} characters long.")]
        public string State { get; set; }

        [Display(Name = "Zip/Postal Code:")]
        [StringLength(50, ErrorMessage = "The {0} must be not more than {1} characters long.")]
        public string Zip { get; set; }

        //public string Phone { get; set; }

        public IList<CustomerContact> Phones { get; set; }

        public IList<CustomerContact> Emails { get; set; }

        [Display(Name = "Customer Notes:")]
        public string Notes { get; set; }

        [Display(Name = "Is Archived")]
        public bool IsArchived { get; set; }

        [UIHint("Hidden")]
        public DateTime? ZapierSentDate { get; set; }

        public event EventHandler AddressUpdated;

        public static CustomerEditModel FromBO(Customer customer)
        {
            var model = new CustomerEditModel();

            model.Id = customer.id;

            model.FirstName = customer.first_name;
            model.LastName = customer.last_name;
            model.SecondaryFirstName = customer.s_first_name;
            model.SecondaryLastName = customer.s_last_name;
            model.Address = customer.address;
            model.State = customer.state;
            model.City = customer.city;
            model.Zip = customer.zip;
            model.Notes = customer.notes;

            model.Emails = new[] {
                new CustomerContact(customer.email_personal ?? "", EmailContactEnum.Personal),
                new CustomerContact(customer.email_business ?? "", EmailContactEnum.Business),
                new CustomerContact(customer.email_instant_messenger ?? "", EmailContactEnum.InstantMessenger),
                new CustomerContact(customer.email_other ?? "", EmailContactEnum.Other)
            }.OrderBy(c => string.IsNullOrEmpty(c.Value)).ThenBy(c => c.Id).ToArray();

            model.Phones = new[] { new CustomerContact(customer.phone_home ?? "", PhoneContactEnum.Home),
                new CustomerContact(customer.phone_business ?? "", PhoneContactEnum.Business),
                new CustomerContact(customer.phone_mobile ?? "", PhoneContactEnum.Mobile),
                new CustomerContact(customer.phone_fax ?? "", PhoneContactEnum.Fax),
                new CustomerContact(customer.phone_other ?? "", PhoneContactEnum.Other)
            }.OrderBy(c => string.IsNullOrEmpty(c.Value)).ThenBy(c => c.Id).ToArray();

            return model;
        }

        internal static CustomerEditModel Default()
        {
            return new CustomerEditModel
            {
                Emails = new[] {
                new CustomerContact("", EmailContactEnum.Personal),
                new CustomerContact("", EmailContactEnum.Business),
                new CustomerContact("", EmailContactEnum.InstantMessenger),
                new CustomerContact("", EmailContactEnum.Other)
                },

                Phones = new[] { new CustomerContact("", PhoneContactEnum.Home),
                new CustomerContact("", PhoneContactEnum.Business),
                new CustomerContact("", PhoneContactEnum.Mobile),
                new CustomerContact("", PhoneContactEnum.Fax),
                new CustomerContact("", PhoneContactEnum.Other)
                }
            };
        }

        public void UpdateBO(Customer customer)
        {
            if (customer.address != Address || customer.state != State || customer.city != City || customer.zip != Zip)
                AddressUpdated?.Invoke(this, new EventArgs());

            customer.first_name = FirstName;
            customer.last_name = LastName;
            customer.s_first_name = SecondaryFirstName;
            customer.s_last_name = SecondaryLastName;
            customer.address = Address;
            customer.state = State;
            customer.city = City;
            customer.zip = Zip;
            customer.notes = Notes;
            customer.is_archived = IsArchived;

            foreach(var phone in Phones)
            {
                PhoneContactEnum label;
                if(!string.IsNullOrEmpty(phone.Label) && Enum.TryParse(phone.Label, out label))
                    switch (label)
                    {
                        case PhoneContactEnum.Home:
                            customer.phone_home = phone.Value;
                            break;
                        case PhoneContactEnum.Business:
                            customer.phone_business = phone.Value;
                            break;
                        case PhoneContactEnum.Mobile:
                            customer.phone_mobile = phone.Value;
                            break;
                        case PhoneContactEnum.Fax:
                            customer.phone_fax = phone.Value;
                            break;
                        case PhoneContactEnum.Other:
                            customer.phone_other = phone.Value;
                            break;
                    }
            }

            foreach(var email in Emails)
            {
                EmailContactEnum label;
                if (!string.IsNullOrEmpty(email.Label) && Enum.TryParse(email.Label, out label))
                    switch (label)
                    {
                        case EmailContactEnum.Personal:
                            customer.email_personal = email.Value;
                            break;
                        case EmailContactEnum.Business:
                            customer.email_business = email.Value;
                            break;
                        case EmailContactEnum.InstantMessenger:
                            customer.email_instant_messenger = email.Value;
                            break;
                        case EmailContactEnum.Other:
                            customer.email_other = email.Value;
                            break;
                    }
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
                errors.Add(new ValidationResult("Customer Name is empty"));

            return errors;
        }
    }
}