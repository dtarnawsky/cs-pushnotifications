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
            var deviceToken = "D6A38FA026DC62EA590CA1F75302988F23757A0A325804A71AC7DCFC51CE284B";

            if (args.Length > 0)
            {
                deviceToken = args[0];
            }
            var client = NotificationHubClient.CreateClientFromConnectionString(primaryConnectionString, hubName);


            await RegisterAppleDevice(client, deviceToken);
            //await CreateAndDeleteInstallationAsync(nhClient);
            //await CreateAndDeleteRegistrationAsync(nhClient);
        }

        private static async Task RegisterAppleDevice(NotificationHubClient client, string token)
        {
            Console.WriteLine($"Register Apple Device {token}");
            var desc = await client.CreateAppleNativeRegistrationAsync(token);
            Console.WriteLine($"Register Apple Device Result:  {desc.Serialize()}");
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
