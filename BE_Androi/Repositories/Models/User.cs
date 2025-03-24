﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; }

    public string PasswordHash { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public string Address { get; set; }

    public string Role { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}