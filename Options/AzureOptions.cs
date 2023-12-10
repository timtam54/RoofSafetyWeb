using System;
namespace RoofSafety.Options
{
	public class AzureOptions
	{
		/*
		 *    "ResourceGroup": "PerthCityCranesResource",
    "Account": "rssblob",
    "Container": "rssimage"*/
		public string? ResourceGroup { get; set; }

        public string? Account { get; set; }
        public string? Container { get; set; }
        public string? ConnectionString { get; set; }
    }
}

