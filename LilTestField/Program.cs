using System.Collections;
using BaseLibrary.Auth;
using BaseLibrary.Model.Classes;
using BaseLibrary.Tools;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LilTestField;

internal class Program
{
    private static JsonSerializerSettings _options = new JsonSerializerSettings()
    {
        ContractResolver = new DefaultContractResolver()
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        }
    };

    private static HubConnection _hub = new HubConnectionBuilder()
        .WithUrl(GlobalOptions.HUB_URI)
        .WithAutomaticReconnect()
        .Build();
    
    private static async Task Main(string[] args)
    {
        /*
        var json = """
                   {
                       "type": "Гдетотам\\PostResource",
                       "data": {
                           "id": 1,
                           "title": "Заголовок",
                           "content": "Содержимое",
                           "created_at": "2025-01-15T10:30:00+00:00",
                           "updated_at": "2025-01-15T10:30:00+00:00"
                       },
                        "relationships": {
                            "author": {
                                "id": 1,
                                "name": "Иван Петров"
                            }
                        },
                        "links": {
                            "self": "http://api.example.com/posts/1"
                        },
                       "status": 201,
                       "message": "message"
                   }
                   """;


        var sw = new Stopwatch();
        sw.Start();
        var test = LaravelRequestHandler.ParseResponse<Post>(json);
        sw.Stop();
        var tess = sw.ElapsedMilliseconds;
        ;
        sw.Reset();
        sw.Start();
        var test2 = LaravelRequestHandler.ParseResponse<Post>(json);
        sw.Stop();
        var tesss = sw.ElapsedMilliseconds;
        ;
        */
        await TestConnection();
        ;
    }


    private class Test5(int something)
    {
        public int Some { get; set; } = something;
    };
    private static async Task<T> TestArrayListCast<T>()
    {
        var list = new ArrayList();
        for (int i = 0; i < 5; i++)
        {
            list.Add(new Test5(i));
        }

        var tType = typeof(T);
        var enumType = typeof(Enumerable);
        if (!typeof(IEnumerable).IsAssignableFrom(tType)) throw new Exception();
        
        var u = tType.GetGenericArguments()[0];
        var cast = enumType
            .GetMethod(nameof(Enumerable.Cast))!
            .MakeGenericMethod(u);
        var result = cast.Invoke(null, [list]);
        return (T)result!;

    }
    
    
    private static async Task<bool> Connect()
    {
        for (var i = 0; i < 3; i++)
        {
            try
            {
                await _hub.StartAsync();
                break;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Thread.Sleep(6500);
            }
        }
            

        var connected = _hub.State == HubConnectionState.Connected;
        Console.WriteLine(connected ? "Успешное подключение" : "Мда");
        return connected;
    }

    private static async Task TestConnection()
    {
        while (true)
        {
            if (_hub.State == HubConnectionState.Connected) break;
            try
            {
                if (!await Connect()) continue;
            
                Console.Clear();
                var response = await _hub.InvokeAsync<Response>("Authorize", "admin", "password");
                var authUser = (await HandleResponse<UserAuth>(response))!;
                Console.WriteLine(
                    $"login {authUser.User.Login}\nrole {Enum.GetName(authUser.User.Role)}\ntoken {authUser.Token}");
                _hub = new HubConnectionBuilder()
                    .WithUrl(GlobalOptions.HUB_URI,
                        options => { options.Headers.Add("Authorization", "Bearer " + authUser.Token); })
                    .WithAutomaticReconnect()
                    .Build();
                if (await Connect()) break;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        Thread.Sleep(10000);

        List<Shipping>? shippings;
        List<Message>? messages;
        List<User>? members;
        List<Incident>? incidents;
        List<Chat>? chats;
        while (true)
        {
            while (true)
            {
                if (_hub.State == HubConnectionState.Connected) break;
                try
                {
                    if (!await Connect()) continue;
            
                    Console.Clear();
                    var response = await _hub.InvokeAsync<Response>("Authorize", "admin", "password");
                    var authUser = (await HandleResponse<UserAuth>(response))!;
                    Console.WriteLine(
                        $"login {authUser.User.Login}\nrole {Enum.GetName(authUser.User.Role)}\ntoken {authUser.Token}");
                    _hub = new HubConnectionBuilder()
                        .WithUrl(GlobalOptions.HUB_URI,
                            options => { options.Headers.Add("Authorization", "Bearer " + authUser.Token); })
                        .WithAutomaticReconnect()
                        .Build();
                    if (await Connect()) break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            
            try
            {
                shippings = (await GetSomething<IEnumerable<Shipping>>("GetShippings"))?.ToList();
                incidents = (await GetSomething<IEnumerable<Incident>>("GetIncidents"))?.ToList();
                chats = (await GetSomething<IEnumerable<Chat>>("GetChats"))?.ToList();
                
                if (chats?.Count > 0)
                {
                    messages = (await GetSomething<IEnumerable<Message>>("GetChatMessages", chats.First().Id))?.ToList();
                    members =(await GetSomething<IEnumerable<User>>("GetChatMembers", chats.First().Id))?.ToList();
                }
                    
                break;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Thread.Sleep(5000);
        }



        Thread.Sleep(60000);
    }

    private static async Task<T?> GetSomething<T>(string methodName, params object?[]? parameters)
    {
        var response = await (parameters?.Length switch
        {
            1 => _hub.InvokeAsync<Response>(methodName, parameters[0]),
            2 => _hub.InvokeAsync<Response>(methodName, parameters[0], parameters[1]),
            3 => _hub.InvokeAsync<Response>(methodName, parameters[0], parameters[1], parameters[2]),
            _ => _hub.InvokeAsync<Response>(methodName)
        });
        var smth = await HandleResponse<T>(response);
        return smth ?? default;
    }
    
    private static async Task<T?> HandleResponse<T>(Response response)
    {
        if ((int)response.StatusCode < 400)
            Console.WriteLine("Trying deserialize...");
        
        try
        {
            return JsonConvert.DeserializeObject<T>(response.Data?.ToString(), _options);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            return default;
        }
    }
    

    /*
    private static async Task SaveRSAKeyPair()
    {
        var rsa = new RSACryptoServiceProvider();
        var keys = GenerateRSAKeyPair();
        await File.WriteAllTextAsync("private.xml", keys.privateKey);
        await File.WriteAllTextAsync("public.xml", keys.publicKey);
    }

    private static (string publicKey, string privateKey) GenerateRSAKeyPair()
    {
        using var rsa = new RSACryptoServiceProvider();
        var publicKey = rsa.ToXmlString(false); //Открытый
        var privateKey = rsa.ToXmlString(true); //И открытый и закрытый ключи
        return (publicKey, privateKey);
    }

    private static void Test()
    {
        var rsaKey = RSA.Create(2048);
        var test = rsaKey.ExportRSAPublicKey();
        const string subjectCa = "CN=myauthority.ru";
        var certReq = new CertificateRequest(subjectCa, rsaKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        certReq.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
        certReq.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(certReq.PublicKey, false));
        var expiration = DateTimeOffset.Now.AddYears(5);
        var caCert = certReq.CreateSelfSigned(DateTimeOffset.Now, expiration);

        var clientKey = RSA.Create(2048);
        const string subject = "CN=10.10.10.*";
        var clientReq = new CertificateRequest(subject, clientKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        clientReq.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
        clientReq.CertificateExtensions.Add(
            new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.NonRepudiation, false));
        clientReq.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(clientReq.PublicKey, false));
        var serialNumber = BitConverter.GetBytes(DateTime.Now.ToBinary());
        var clientCert = clientReq.Create(caCert, DateTimeOffset.Now, expiration, serialNumber);

        //Save cert public key
        var builder = new StringBuilder();
        builder.AppendLine("-----BEGIN CERTIFICATE-----");
        builder.AppendLine(Convert.ToBase64String(clientCert.RawData, Base64FormattingOptions.InsertLineBreaks));
        builder.AppendLine("-----END CERTIFICATE-----");
        File.WriteAllText("public.crt", builder.ToString());

        //Save cert private key
        var name = clientKey.SignatureAlgorithm.ToUpper();
        builder.Clear();
        builder.AppendLine($"-----BEGIN {name} PRIVATE KEY-----");
        builder.AppendLine(Convert.ToBase64String(clientKey.ExportRSAPrivateKey(),
            Base64FormattingOptions.InsertLineBreaks));
        builder.AppendLine($"-----END {name} PRIVATE KEY-----");
        File.WriteAllText("private.key", builder.ToString());


        var textPrivate = File.ReadAllText("private.key");
        var textCert = File.ReadAllText("public.crt");
        var fullPath = Path.GetFullPath("private.key");
        ;

        /*
        //origin loader
        //var exportCert = new X509Certificate2(clientCert.Export(X509ContentType.Cert), (string)null, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet).CopyWithPrivateKey(clientKey);

        var exportCert = X509CertificateLoader.LoadPkcs12(clientCert.Export(X509ContentType.Cert), (string)null, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet).CopyWithPrivateKey(clientKey);
        //File.WriteAllBytes("client.pfx", exportCert.Export(X509ContentType.Pfx));
        File.WriteAllBytes("client.p12", exportCert.Export(X509ContentType.Pkcs12));
        
    }
    */
}