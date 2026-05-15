using BaseLibrary.Model;
using BaseLibrary.Model.Classes;

namespace BaseLibrary.Auth;

public class UserAuth
{
    public User User { get; set; }
    public string Token { get; set; }
}