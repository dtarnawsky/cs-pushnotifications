using System;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;


namespace notify
{
  internal class Program
  {
    static async Task Main(string[] args)
    {
      // Getting connection key from the new resource
      var hubName = "cs-ionic-notifications";
      //var primaryConnectionString = ""; // Get this from Azure Portal -> Manage -> Access Policies            
      var primaryConnectionString = "Endpoint=sb://cs-ionic-notifications.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=DYtF3x++/+hedJBa+Wiuf2X9X/mvOlS9zE+tW3BQAi8=";
      var client = NotificationHubClient.CreateClientFromConnectionString(primaryConnectionString, hubName);
      // Example deviceToken "D6A38FA026DC62EA590CA1F75302988F23757A0A325804A71AC7DCFC51CE284B"

      if (args.Length == 0)
      {
        Console.WriteLine($"Please specify arguments. Example");
        Console.WriteLine($"dotnet run [ios|android] [device-token] - This will register a device");
        Console.WriteLine($"dotnet run send-ios \"message.json\" - Send a test notification message read from file");
        return;
      }

      switch (args[0])
      {
        case "android": await RegisterAndroidDevice(client, args[1]); break;
        case "ios": await RegisterAppleDevice(client, args[1]); break;
        case "send-ios": await SendAppleNotification(client, args[1]); break;
        case "send-android": await SendFCMNotification(client, args[1]); break;
      }

      //await CreateAndDeleteInstallationAsync(nhClient);
      //await CreateAndDeleteRegistrationAsync(nhClient);
    }

    private static async Task RegisterAppleDevice(NotificationHubClient client, string token)
    {
      Console.WriteLine($"Register Apple Device {token}");
      var desc = await client.CreateAppleNativeRegistrationAsync(token);
      Console.WriteLine($"Register Apple Device Result:  {desc.Serialize()}");
    }

    private static async Task SendAppleNotification(NotificationHubClient client, string filename)
    {
      var message = await System.IO.File.ReadAllTextAsync(filename);
      Console.WriteLine($"SendNotification {message}");
      var desc = await client.SendAppleNativeNotificationAsync(message);
      Console.WriteLine($"SendNotification Result:  {desc.State}");
    }

    private static async Task SendFCMNotification(NotificationHubClient client, string filename)
    {
      var message = await System.IO.File.ReadAllTextAsync(filename);
      Console.WriteLine($"SendFCMNotification {message}");
      var desc = await client.SendFcmNativeNotificationAsync(message);
      Console.WriteLine($"SendFCMNotification Result:  {desc.State}");
    }

    private static async Task RegisterAndroidDevice(NotificationHubClient client, string token)
    {
      Console.WriteLine($"Register Android Device {token}");
      var desc = await client.CreateFcmNativeRegistrationAsync(token);
      Console.WriteLine($"Register Android Device Result:  {desc.Serialize()}");
    }

    private static async Task CreateAndDeleteRegistrationAsync(NotificationHubClient nhClient)
    {
      var registrationId = await nhClient.CreateRegistrationIdAsync();
      var registrationDescr = await nhClient.CreateFcmNativeRegistrationAsync(registrationId);
      Console.WriteLine($"Created FCM registration {registrationDescr.FcmRegistrationId}");

      var allRegistrations = await nhClient.GetAllRegistrationsAsync(1000);
      foreach (var regFromServer in allRegistrations)
      {
        if (regFromServer.RegistrationId == registrationDescr.RegistrationId)
        {
          Console.WriteLine($"Found FCM registration {registrationDescr.FcmRegistrationId}");
          break;
        }
      }

      //registrationDescr = await nhClient.GetRegistrationAsync<FcmRegistrationDescription>(registrationId);
      //Console.WriteLine($"Retrieved FCM registration {registrationDescr.FcmRegistrationId}");

      await nhClient.DeleteRegistrationAsync(registrationDescr);
      Console.WriteLine($"Deleted FCM registration {registrationDescr.FcmRegistrationId}");
    }

    private static async Task CreateAndDeleteInstallationAsync(NotificationHubClient nhClient)
    {
      // Register some fake devices
      var fcmDeviceId = Guid.NewGuid().ToString();
      var fcmInstallation = new Installation
      {
        InstallationId = fcmDeviceId,
        Platform = NotificationPlatform.Fcm,
        PushChannel = fcmDeviceId,
        PushChannelExpired = false,
        Tags = new[] { "fcm" }
      };
      await nhClient.CreateOrUpdateInstallationAsync(fcmInstallation);

      while (true)
      {
        try
        {
          var installationFromServer = await nhClient.GetInstallationAsync(fcmInstallation.InstallationId);
          break;
        }
        catch (MessagingEntityNotFoundException)
        {
          // Wait for installation to be created
          await Task.Delay(1000);
        }
      }
      Console.WriteLine($"Created FCM installation {fcmInstallation.InstallationId}");
      await nhClient.DeleteInstallationAsync(fcmInstallation.InstallationId);
      while (true)
      {
        try
        {
          var installationFromServer = await nhClient.GetInstallationAsync(fcmInstallation.InstallationId);
          await Task.Delay(1000);
        }
        catch (MessagingEntityNotFoundException)
        {
          Console.WriteLine($"Deleted FCM installation {fcmInstallation.InstallationId}");
          break;
        }
      }
    }
  }
}
