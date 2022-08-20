using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpTutorial
{
    class Program
    {
        private static object graphServiceClient;

        static void Main(string[] args)
        {
            try
            {
                getUsersAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
        }

        public async static Task getUsersAsync()
        {
            var clientId = "283a21c5-07a8-4f49-b39e-a20ab0398e89";
            var tenantId = " 251c4782-827b-4033-b7b5-12500263871f";
            var clientSecret = "25JATiR6I62xjinUasgY7Lad9S9q4EoXDhBzDkWe3hQ=";
            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithTenantId(tenantId)
                .WithClientSecret(clientSecret)
                .Build();

            ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);
            GraphServiceClient graphClient = new GraphServiceClient(authProvider);

            var queryOptions = new List<QueryOption>()
            {
                new QueryOption("$count", "true")
            };

            // GET  reportee
            var directReports = await graphClient.Users["kaushal.raval@euromonitor.com"].DirectReports.Request().GetAsync();
            List<string> testuser = new List<string>();
            bool isReportee = false;
            foreach (var directReport in directReports.CurrentPage)
            {
                var users = (User)directReport;
                testuser.Add(users.Id.ToString());
                isReportee = testuser.Contains("d7afabc4-220c-4557-aeac-8a1e2b95f380");
                Console.WriteLine($"Reportess under kaushal:{users.DisplayName}");
            }


            //GET  manager
            var directoryManagerObject = await graphClient.Users["ashutosh.chaturvedi@euromonitor.com"].Manager
                                        .Request()
                                        .GetAsync();
            var manager = (User)directoryManagerObject;
            Console.WriteLine($"Manager Name:{manager.DisplayName}");

            //Get all the managers
            var user = await graphClient.Users["ashutosh.chaturvedi@euromonitor.com"]
                        .Request(queryOptions)
                        .Header("ConsistencyLevel", "eventual")
                        .Expand("manager($levels=max;$select=id,displayName)")
                        .Select("id,displayName")
                        .GetAsync();
            //  var managers1 = (User)user;
            //foreach (var item in user.)
            //{

            //}
            //GET Groups in my organization
            var groups = await graphClient.Groups
                       .Request()
                       .GetAsync();
            foreach (var group in groups.CurrentPage)
            {
                Console.WriteLine($"Lis of Gropus in organization:{group.DisplayName}");
            }

            //GET all the groups to which I belong
            var compoundGroups = await graphClient.Users["kaushal.raval@euromonitor.com"].TransitiveMemberOf
                        .Request(queryOptions)
                        .Header("ConsistencyLevel", "eventual")
                        .GetAsync();
            foreach (var group in compoundGroups.CurrentPage)
            {
                var DlsIBelongs = (Group)group;
                Console.WriteLine($"Groups I belong to:{DlsIBelongs.DisplayName}");
            }


            //Get identities
            var users1 = await graphClient.Users["ashutosh.chaturvedi@euromonitor.com"]
             .Request()
             .Select("Identities")
             .GetAsync();

            var identities = users1.Identities;
            foreach (var item in identities)
            {
                Console.WriteLine($"Is user of type:{item.Issuer}");
                Console.WriteLine($"Assigned Id:{item.IssuerAssignedId}");
                Console.WriteLine($"Sign in Type:{item.SignInType}");
            }


            //////Trach my changes
            ////var deltaInformation = await graphClient.Users
            ////              .Delta()
            ////              .Request()
            ////              .Select("displayName,givenName,surname")
            ////              .GetAsync();

            ////foreach (var item in deltaInformation)
            ////{
            ////    Console.WriteLine($"Is user of type:{item.DisplayName}");
            ////    Console.WriteLine($"Assigned Id:{item.Id}");
            ////    Console.WriteLine($"Sign in Type:{item.Mail}");
            ////    Console.WriteLine($"Sign in Type:{item.UserPrincipalName}");
            ////    Console.WriteLine($"Sign in Type:{item.GivenName}");
            ////}

            //Get User's job title
            var userDetails = await graphClient.Users["152441c5-1982-418a-9613-d99f36975b02"]
                    .Request()
                    .GetAsync();
            Console.WriteLine($"User Name:{userDetails.DisplayName}");
            Console.WriteLine($"Role:{userDetails.JobTitle}");
            Console.ReadLine();
        }
    }
}
