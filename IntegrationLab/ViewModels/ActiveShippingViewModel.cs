using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using IntegrationLab.Views;
using Models.Model;
using Models.Tools;

namespace IntegrationLab.ViewModels;

public partial class ActiveShippingViewModel : ViewModelControlBase<ActiveShippingView>
{
    [ObservableProperty] private Shipping _activeShipping;
    
    public ActiveShippingViewModel()
    {
        //TODO: Убрать потом и раскомментить в OnCreating
        TestData();
    }
    
    
    private void TestData()
    {
        var faker = GlobalOptions.Faker;
        var shippingOrder = new ShippingOrder()
        {
            OrderDate = DateTime.Now.AddDays(2),
            Address = faker.Address.FullAddress(),
            Id = faker.Random.Int(1)
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
            
            
        ActiveShipping = new Shipping()
        {
            Cargos = shippingOrder.Cargos,
            DesignatedDriverId = App.CurrentDriver.UserId,
            EstimatedDeliveryDate = DateTime.Now.AddDays(faker.Random.Byte(2, 10)),
            Id = Guid.NewGuid()
        };
    }
    
    public override void OnCreating()
    {
        //TestData();    
    }
}