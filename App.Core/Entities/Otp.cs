﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace App.Core.Entities;

public partial class Otp
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? Code { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; }
}