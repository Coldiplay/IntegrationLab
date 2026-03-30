using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using BaseLibrary.Model;
using BaseLibrary.Tools;

namespace IntegrationLab.Model;

public partial class HubData : ObservableObject
{
    public HubData()
    {
        TestData();
    }
    
    private void TestData()
    {
        var faker = GlobalOptions.Faker;

        //Заполнение рейсов (включая грузы и заказы)
        for (var i = 0; i < 10; i++)
        {
            var shippingOrder = new ShippingOrder()
            {
                OrderDate = DateTime.Now.AddDays(2),
                Address = faker.Address.FullAddress(),
                Id = i + 1, 
                ReceiverFio = faker.Name.FullName(), 
                Status = OrderStatus.InProcessing
            };
            var cargos = new List<Cargo>();
            
            for (var j = 0; j < faker.Random.Byte(1, 10); j++)
            {
                var cargoType = new CargoType()
                {
                    Id = faker.Random.Int(1),
                    Title = faker.Commerce.ProductMaterial(),
                };
                
                cargos.Add(new Cargo
                {
                    CargoType = cargoType,
                    DangerLevel = faker.Random.Bool() 
                        ? (DangerLevel)faker.Random.Int(0, Enum.GetValues<DangerLevel>().Length) 
                        : null,
                    Dimensions = new Dimensions()
                    {
                        Height = faker.Random.Double(1, 100),
                        Length = faker.Random.Double(1, 100),
                        Width = faker.Random.Double(1, 100)
                    },
                    Id = Guid.NewGuid(),
                    Name = faker.Commerce.ProductName(), 
                    Weight = double.Round(faker.Random.Double(1, 100), 2), 
                });  
            }
            
            shippingOrder.AddRangeCargo(cargos);
            
            
            var shipping = new Shipping()
            {
                Cargos = shippingOrder.Cargos,
                DesignatedDriverId = App.CurrentDriver.UserId,
                EstimatedDeliveryDate = DateTime.Now.AddDays(7 + i),
                Id = Guid.NewGuid(), 
                Vehicle = new Vehicle()
                {
                    BodySize = new Dimensions()
                    {
                        Height = faker.Random.Double(200, 300),
                        Length = faker.Random.Double(400, 800),
                        Width = faker.Random.Double(200, 300)
                    }, 
                    Id = faker.Random.Int(1), 
                    BodyType = VehicleBodyType.Контейнеровоз, 
                    BrandName = faker.Vehicle.Manufacturer(), 
                    FuelType = VehicleFuelType.Дизель, 
                    LiftingCapacity = faker.Random.Float(3000, 10000), 
                    MaxCargoVolume = faker.Random.Float(10, 50), 
                    MaxFuelVolume = faker.Random.Float(4, 10), 
                    Model = faker.Vehicle.Model(),
                    NeededRights = Rights.A, 
                    Number = faker.Vehicle.Vin(), 
                    NumberOfAxes = (byte)(faker.Random.Byte(1, 3) * 2), 
                    VehicleSize = new Dimensions()
                    {
                        Height = faker.Random.Double(200, 350),
                        Length = faker.Random.Double(400, 800),
                        Width = faker.Random.Double(200, 300)
                    },
                    VehicleWeight = faker.Random.Float(2000, 8500),
                }, 
                DeliveryPoint = $"{faker.Address.City()}, {faker.Address.StreetAddress()}", 
                ShippingStatus = ShippingStatus.GettingReadyToShip, 
                DesignatedDriver = App.CurrentDriver.User, 
                DeliveryDate = DateTime.Now.AddDays(7 + i),
            };
            Shippings.Add(shipping);
        }
        
        // Заполнение чатов
        for (var i = 0; i < faker.Random.Byte(1, 6); i++)
        {
            var fName = faker.Person.FirstName;
            var lName = faker.Person.LastName;
            var sender = new User()
            {
                Id = faker.Random.Int(1),
                Login = faker.Internet.UserName(fName, lName),
                Name = fName,
                LastName = lName,
            };
            var chat = new Chat()
            {
                Id = faker.Random.Int(1),
                Name = sender.Login, //faker.Internet.DomainName(), //TODO: потом сделать нормальное создание чатов
                IsPrivateChat = true
                // Receiver = App.CurrentDriver.User,
                // ReceiverId = App.CurrentDriver.UserId,
                // Sender = sender,
                // SenderId = sender.Id
            };
            var messages = new ObservableCollection<Message>();
            
            for (var j = 0; j < faker.Random.Byte(1, 6); j++)
            {
                messages.Add( 
                    new Message()
                    {
                        Id = Guid.NewGuid(),
                        Chat = chat,
                        ChatId = chat.Id,
                        Content = faker.Lorem.Lines(faker.Random.Byte(1, 3)),
                        Sender = sender,
                        SenderId = sender.Id, 
                        Date = DateTime.Now.AddMinutes(-10) 
                    });
            }
            Chats.TryAdd(chat, ([sender, App.CurrentDriver.User], messages));
        }
        
        //Заполнение инцидентов
        for (var i = 0; i < Shippings.Count - faker.Random.Int(0, Shippings.Count - 1); i++)
        {
            Incidents.Add(new Incident()
            {
                DriverId = App.CurrentDriver.UserId,
                Description = faker.Lorem.Paragraphs(2),
                Driver = App.CurrentDriver.User,
                Id = i,
                IncidentDate = DateTime.Now.AddDays(-i),
                Shipping = Shippings[i],
                ShippingId = Shippings[i].Id,
                Status = IncidentStatus.InProgress
            });
        }
    }
    
    // [ObservableProperty]
    // private ObservableCollection<Message> _messages = [];
    [ObservableProperty]
    private ConcurrentDictionary<Chat, (ObservableCollection<User> members, ObservableCollection<Message> messages)> _chats = [];
    [ObservableProperty]
    private ObservableCollection<Shipping> _shippings = [];
    [ObservableProperty]
    private ObservableCollection<Incident> _incidents = [];
}