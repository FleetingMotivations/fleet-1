using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetServer;

namespace TestClient
{
    class Program
    {

        private static FleetClientToken GetMyself()
        {
            return new FleetClientToken
            {
                Identifier = "1876786972",
                Token = "1454392689"
            };
        }

        private static FleetClientIdentifier GetTarget()
        {
            return new FleetClientIdentifier
            {
                Identifier = "123456",
                WorkstationName = "Target"
            };
        }

        private static List<FleetClientIdentifier> GetMultipleTargets()
        {
            return new List<FleetClientIdentifier>
            {
                new FleetClientIdentifier
                {
                    Identifier = "123456",
                    WorkstationName = "Target"
                },
                new FleetClientIdentifier
                {
                    Identifier = "1454392689",
                    WorkstationName = "Second_Target"
                }
            };
        }

        private static FleetFileIdentifier GetFileIdentifier()
        {
            return new FleetFileIdentifier
            {
                FileName = "ITBad.jpg",
                Identifier = "1",
                FileSize = 0
            };
        }

        static void Main(string[] args)
        {
           
            var fileBytes = File.ReadAllBytes(args[0]);
            Console.WriteLine($"Reading file from {args[0]}");

            // Manually specify binding and addressing
            var remoteAddress = new System.ServiceModel.EndpointAddress("http://localhost:8733/Design_Time_Addresses/FleetServer/FleetService");
            var binding = new System.ServiceModel.BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;

            var server = new FleetServiceClient(binding, remoteAddress);
            server.Endpoint.Binding.SendTimeout = new TimeSpan(0,0,20,0);
                    
            var me = GetMyself();
            var target = GetTarget();
            var targets = GetMultipleTargets();

            try
            {
                var trySingleTarget = server.SendFileSingleRecipient(me, target, new FleetFile
                {
                    FileName = args[1],
                    FileContents = fileBytes
                });

                Console.WriteLine($"Test sending a file to to a single recipient:{trySingleTarget}");

                var tryMultiTarget = server.SendFileMultipleRecipient(me, targets.ToArray(), new FleetFile
                {
                    FileName = args[1],
                    FileContents = fileBytes
                });

                Console.WriteLine($"Test sending a file to to a multiple recipients:{trySingleTarget}");

                var receivedFile = server.GetFile(me, GetFileIdentifier());

                var path = $"{Directory.GetCurrentDirectory()}/temp";
                var filePath = $"{path}/{receivedFile.FileName}";
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                using (var fileStream = new BinaryWriter(new FileStream(filePath, FileMode.CreateNew)))
                {
                    foreach (var chunk in receivedFile.FileContents)
                    {
                        fileStream.Write(chunk);
                    }
                }

            }
            catch (Exception e)
            {
                Console.Write(e);
            }
            

            Console.Read();
        }
    }
}
