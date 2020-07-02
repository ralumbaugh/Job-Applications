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
    public List<Company> AllCompanies {get; set;}
    }
}