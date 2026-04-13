using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace LilTestField;

class Program
{
    static async Task Main(string[] args)
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
        var test = LaravelParser.ParseResponse<Post>(json);
        sw.Stop();
        var tess = sw.ElapsedMilliseconds;
        ;
        sw.Reset();
        sw.Start();
        var test2 = LaravelParser.ParseResponse<Post>(json);
        sw.Stop();
        var tesss = sw.ElapsedMilliseconds;
        ;
        */
        
        //TODO: check later
        /*
        var connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7045/hub")
            .WithAutomaticReconnect()
            .Build();
        //connection.SendAsync("Authorize", "test", "test2").Wait();
        for (int i = 0; i < 3; i++)
        {
            try
            {
                await connection.StartAsync();
                break;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Thread.Sleep(6000);
            }
        }

        Console.Clear();
        Console.WriteLine("Успешное подключение");
        ;
        
        var response = await connection.InvokeAsync<object>("Authorize", "test", "test2");
        ;
        */
        Test();

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
        var caCert  = certReq.CreateSelfSigned(DateTimeOffset.Now, expiration);
        
        var clientKey = RSA.Create(2048);
        const string subject = "CN=10.10.10.*";
        var clientReq = new CertificateRequest(subject, clientKey,HashAlgorithmName.SHA256,RSASignaturePadding.Pkcs1);
        clientReq.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
        clientReq.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.NonRepudiation, false));
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
        builder.AppendLine(Convert.ToBase64String(clientKey.ExportRSAPrivateKey(), Base64FormattingOptions.InsertLineBreaks));
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
        */
    }

    
    
}