using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLibrary.Auth;
using BaseLibrary.Model.Classes;

namespace IntegrationLab.Model;

public interface IHubHandler
{
    Task Start();
    Task Load();

    Task<IEnumerable<User>?> GetChatMembers(ulong chatId);
    Task<IEnumerable<Chat>?> GetChats();
    Task<IEnumerable<Message>?> GetChatMessages(ulong chatId);
    Task<Chat?> CreateChat(Chat? chat);
    Task<User?> AddChatMember(Chat? chat, User? user);
    Task<Message?> SendMessage(Message? message);
    
    Task<IEnumerable<Incident>?> GetIncidents();
    Task<Incident?> CreateIncident(Incident? incident);
    
    Task<IEnumerable<Shipping>?> GetShippings();
    
    Task<IEnumerable<DriversShift>?> GetDriversShifts();
    Task<DriversShift?> StartShift();
    Task<DriversShift?> EndShift(DriversShift? shift);

    Task<ShiftBreak?> StartBreak(DriversShift? shift);
    Task<ShiftBreak?> EndBreak(ShiftBreak? shiftBreak);

    Task<UserAuth?> Authorize(string login, string password);
}