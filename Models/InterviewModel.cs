using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobApplications.Models
{
    public class Interview
    {
    [Key]
    public int InterviewId {get; set;}
    [Required (ErrorMessage="A date is required")]
    public DateTime? Date {get; set;}
    [Required (ErrorMessage= "Which company are you interviewing with?")]
    public int PositionId {get; set;}
    public Position Position {get; set;}
    public string Notes {get; set;}
    public DateTime CreatedAt {get; set;} = DateTime.Now;
    public DateTime UpdatedAt {get; set;} = DateTime.Now;
    }
}