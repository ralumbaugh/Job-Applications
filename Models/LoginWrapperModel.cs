using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobApplications.Models
{
    public class LoginWrapper
    {
        public User NewUser {get; set;}
        public LoginUser LoginUser {get; set;}
    }
}