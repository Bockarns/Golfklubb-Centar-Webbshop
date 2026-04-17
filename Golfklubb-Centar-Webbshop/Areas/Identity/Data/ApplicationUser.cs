using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Golfklubb_Centar_Webbshop.Models;
using NuGet.Protocol.Plugins;
using System.ComponentModel.DataAnnotations.Schema;

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
    public string? ProfileImageUrl { get; set; }
    public string? FullName { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }

    [InverseProperty("FkUser")]
    public virtual ICollection<Follow> Followers { get; set; } = new List<Follow>();
    [InverseProperty("FkFollowedUser")]
    public virtual ICollection<Follow> Following { get; set; } = new List<Follow>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}