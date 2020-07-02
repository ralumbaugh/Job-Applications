using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobApplications.Models
{
    public class Position
    {
    [Key]
    public int PositionId {get; set;}
    [Required (ErrorMessage = "Please put a position name")]
    public string Name {get; set;}
    [Required (ErrorMessage = "Please put a description of the position")]
    public string Description {get; set;}
    public string Requirements {get; set;}
    [DataType (DataType.Url, ErrorMessage="Please put a URL to the job posting")]
    public string Link {get; set;}
    public string Notes {get; set;}
    public List<Contact> Contacts {get; set;}
    public List<Interview> Interviews {get; set;}
    public int CompanyId {get; set;}
    public Company Company {get; set;}

    public DateTime CreatedAt {get; set;} = DateTime.Now;
    public DateTime UpdatedAt {get; set;} = DateTime.Now;
    }
}