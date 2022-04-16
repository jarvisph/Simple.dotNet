using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.Core.Helper
{
    public static class CDNHelper
    {
        private static IConfiguration Configuration
        {
            get
            {
                return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            }
        }
        public static string GetImage(string image)
        {
            if (string.IsNullOrWhiteSpace(image)) return string.Empty;
            return Configuration["Tool:ImageCDN"] + image;
        }
    }
}
