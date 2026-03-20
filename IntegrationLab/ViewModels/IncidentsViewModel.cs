using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using IntegrationLab.Views;
using Models.Model;
using Models.Tools;

namespace IntegrationLab.ViewModels;

public partial class IncidentsViewModel : ViewModelControlBase<IncidentsView>
{
    [ObservableProperty] private ObservableCollection<Incident> _incidents = [];
    
    private void TestData()
    {
        var faker = GlobalOptions.Faker;
        for (int i = 0; i < 5; i++)
        {
            var shippingOrder = new ShippingOrder()
            {
                OrderDate = DateTime.Now.AddDays(-14 + i),
                Address = faker.Address.FullAddress(),
                Id = i + 1
            };
            var cargos = new List<Cargo>();
            
            for (int j = 0; j < faker.Random.Int(1, 10); j++)
            {
                var cargoType = new CargoType()
                {
                    Id = faker.Random.Int(1),
                    Title = faker.Commerce.ProductMaterial()
                };
                
                cargos.Add(new Cargo
                {
                    CargoType = cargoType,
                    DangerLevel = DangerLevel.None,
                    Dimensions = new Dimensions()
                    {
                        Height = faker.Random.Double(1, 100),
                        Length = faker.Random.Double(1, 100),
                        Width = faker.Random.Double(1, 100)
                    },
                    Id = Guid.NewGuid(),
                    Name = faker.Commerce.ProductName(),
                });  
            }
            
            shippingOrder.AddRangeCargo(cargos);
            
            
            var shipping = new Shipping()
            {
                Cargos = shippingOrder.Cargos,
                DesignatedDriverId = App.CurrentDriver.UserId,
                EstimatedDeliveryDate = DateTime.Now.AddDays(-7 + i),
                Id = Guid.NewGuid()
            };
            
            Incidents.Add(new Incident()
            {
                DriverId = App.CurrentDriver.UserId,
                Description = faker.Lorem.Paragraph(),
                Driver = App.CurrentDriver.User,
                Id = i,
                IncidentDate = DateTime.Now.AddDays(-i),
                Shipping = shipping,
                ShippingId = shipping.Id,
                Status = IncidentStatus.InProgress
            });
        }
    }
    
    public override void OnCreating()
    {
        TestData();
        
    }
}