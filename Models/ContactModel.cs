using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobApplications.Models
{
    public class Contact
    {
    [Key]
    public int ContactId {get; set;}

    [Required (ErrorMessage = "The contact has a name")]
    public string Name {get; set;}
    [Required (ErrorMessage="Which company is this a contact for?")]
    public int CompanyId {get; set;}
    public Company Company {get; set;}
    [DataType(DataType.PhoneNumber, ErrorMessage="Please input a valid phone number")]
    public string PhoneNumber {get; set;}
    [EmailAddress (ErrorMessage = "Please input a valid email address")]
    public string Email {get; set;}
    public string Notes {get; set;}
    public DateTime CreatedAt {get; set;} = DateTime.Now;
    public DateTime UpdatedAt {get; set;} = DateTime.Now;
    }
}