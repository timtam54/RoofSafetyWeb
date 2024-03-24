using System;
using System.ComponentModel.DataAnnotations;
namespace RoofSafety.Models
{

        public class InspPhoto
        {
        [Key]
            public int id { get; set; }
            public int InspEquipID { get; set; }
            public string? photoname { get; set; }
            public string? Description { get; set; }

        public string? SourceTable { get; set; }
        }
    
}

