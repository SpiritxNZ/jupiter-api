using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace jupiterCore
{
    static class Custom
    {
        public static IConfiguration AppSetting { get; }
        static Custom()
        {
            AppSetting = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
        }
    }
}
