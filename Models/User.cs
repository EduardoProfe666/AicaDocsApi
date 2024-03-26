using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AicaDocsApi.Models;

public class User : IdentityUser
{
    [PersonalData]
    public string? FullName { get; set; }
}