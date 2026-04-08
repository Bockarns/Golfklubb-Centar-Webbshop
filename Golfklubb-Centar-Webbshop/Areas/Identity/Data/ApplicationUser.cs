using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Golfklubb_Centar_Webbshop.Models;
using NuGet.Protocol.Plugins;

namespace Golfklubb_Centar_Webbshop.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<History> Histories { get; set; } = new List<History>();
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
    public bool IsForumBanned { get; set; } = false;
    public string? ProfileImagePath { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? FullName { get; set; }
}