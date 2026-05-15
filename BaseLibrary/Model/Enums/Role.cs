namespace BaseLibrary.Model.Enums;

[Flags]
public enum Role
{
    User = 1 << 0,
    Admin =  1 << 1,
    Driver = 1 << 2,
    Logistician = 1 << 3,
}