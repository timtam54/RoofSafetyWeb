using System.ComponentModel.DataAnnotations;


namespace RoofSafety.Models
{
   public class BuildingsSearch
    {
        public string Search { get; set; }
        public List<Building> Buildings { get; set; }
    }
    public class Building
    {
        public int id { get; set; }
        [Display(Name = "Building Name")]

        public string? BuildingName { get; set; }
       // public string? ClientName { get; set; }
        public Client? Client { get; set; }
        [Display(Name = "Client")]

        public int? InspFreqMonths { get; set; }

        public int ClientID { get; set; }

        public string? Address { get; set; }

        public string? ContactName { get; set; }
        public string? ContactNumber { get; set; }
        public string?  AccessDetails { get; set; }
    }

}
