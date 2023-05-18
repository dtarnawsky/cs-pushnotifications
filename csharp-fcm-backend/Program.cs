using System;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;

namespace notify
{
  internal class Program
  {
    static async Task Main(string[] args)
    {
      FirebaseApp.Create();
      // See https://googleapis.dev/dotnet/Google.Apis.FirebaseCloudMessaging.v1/latest/api/Google.Apis.FirebaseCloudMessaging.v1.html

      if (args.Length == 0)
      {
        Console.WriteLine($"Please specify arguments. Example");
        Console.WriteLine($"dotnet run [ios|android] [device-token] - This will register a device");
        Console.WriteLine($"dotnet run send-ios \"message.json\" - Send a test notification message read from file");
        return;
      }

      switch (args[0])
      {   
        case "send": await SendFCMNotification(args[1]); break;
      }
    }

    private static async Task SendFCMNotification(string deviceToken)
    {      
      var message = new Message()
      {
        Notification = new Notification()
        {
          Title = "My Test Message",
          Body = "This is an example.",          
          ImageUrl = "https://ionic.io/blog/wp-content/uploads/2023/03/capacitor-5-beta-feature-image-2048x1024.png"
        },
        Token = deviceToken
      };
      var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);      
      Console.WriteLine($"SendFCMNotification Result: {response}");
    }

    private static async Task RegisterAndroidDevice(string token)
    {
      Console.WriteLine($"Register Android Device {token}");
      await Task.Delay(100);
      //var desc = await client.CreateFcmNativeRegistrationAsync(token);
      Console.WriteLine($"Register Android Device Result");
    }
  }
}
