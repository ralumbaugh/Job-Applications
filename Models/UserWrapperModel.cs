using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobApplications.Models
{
    public class UserWrapper
    {
    [Key]
    public int UserWrapperId {get; set;}
    public User CurrentUser {get; set;}
    public Company CurrentCompany {get; set;}
    public Position CurrentPosition {get; set;}
    public Interview CurrentInterview {get; set;}
    public Contact CurrentContact {get; set;}
    public List<Contact> AllContacts {get; set;}
    public List<Company> AllCompanies {get; set;}
    public List<Interview> AllInterviews {get; set;}
    }
}