using Appointments.Models;
using System;
using System.Linq;

namespace Appointments.Data
{
    public class DbInitializer
    {
        public static void Initialize(Datacontext context)
        {
            context.Database.EnsureCreated();

            if (context.Appointment.Any() && context.User.Any())
            {
                return;   // DB has been seeded
            }

            var appointments = new Appointment[]
            {
            new Appointment{Title="test",Description="test",BeginDate=DateTime.Parse("2005-09-01 12:30 PM"),EndDate=DateTime.Parse("2005-09-04 12:30 PM")},
            new Appointment{Title="test2212",Description="test2",BeginDate=DateTime.Parse("2002-09-01 12:30 PM"),EndDate=DateTime.Parse("2002-09-27 12:30 PM")}
            };

            var user = new User[]
            {
            new User{Username="Test1",Password="Test1",Email="test@gmail.com"},
            new User{Username="GroteJan33",Password="GroteJan33",Email="test1@gmail.com"}
            };

            foreach (var item in appointments)
            {
                context.Appointment.Add(item);
            }

            foreach (var items in user)
            {
                context.User.Add(items);
            }

            context.SaveChanges();
        }
    }
}
