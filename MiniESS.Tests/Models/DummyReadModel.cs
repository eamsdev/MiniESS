using System;
using System.ComponentModel.DataAnnotations;

namespace MiniESS.Subscription.Tests.Models;

public class DummyReadModel
{
    [Key]
    public Guid StreamId { get; set; }
    public int Count { get; set; }
}