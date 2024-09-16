using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Sample07.Basic.ConsoleApp.ModelMapping.Models;
using Sample07.Basic.ConsoleApp.ModelMapping.ViewModels;
using System.Reflection;
using TTSS.Core;

// Register TTSS Core
var services = new ServiceCollection();
services.RegisterTTSSCore([Assembly.GetExecutingAssembly()]);

// Create service provider
var provider = services.BuildServiceProvider();

// Use AutoMapper to map models to view models
var mapper = provider.GetRequiredService<IMapper>();
var easy = new Easy
{
    Id = 1,
    Name = "Alice",
    BirthDate = new DateTime(2000, 1, 1)
};
var easyVm = mapper.Map<EasyVm>(easy);
Console.WriteLine(easyVm);

var normal = new Normal
{
    Id = 2,
    FirstName = "Bob",
    LastName = "Smith",
    BirthDate = new DateTime(2001, 2, 2),
};
var normalVm = mapper.Map<NormalVm>(normal);
Console.WriteLine(normalVm);

// Key takeaways from this example:
// 1. AutoMapper is registered in TTSS Core.
// 2. Use mapping profiles to map models to view models.