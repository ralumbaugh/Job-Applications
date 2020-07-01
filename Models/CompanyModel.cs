using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobApplications.Models
{
    public class Company
    {
    [Key]
    public int CompanyId {get; set;}
    [Required (ErrorMessage = "Please enter a company name.")]
    public string Name {get; set;}
    [Required (ErrorMessage = "Please enter a description of the company.")]
    public string Description {get; set;}
    public int UserId {get; set;}
    public User User {get; set;}
    public string Notes {get; set;}
    public List<Interview> Interviews {get; set;}
    public List<Contact> Contacts {get; set;}
    public DateTime CreatedAt {get; set;} = DateTime.Now;
    public DateTime UpdatedAt {get; set;} = DateTime.Now;
    }
}