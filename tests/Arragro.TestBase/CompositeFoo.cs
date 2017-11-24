using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Arragro.TestBase
{
    public class CompositeFoo
    {
        [Key]
        public int Id { get; set; }
        [Key]
        public int SecondId { get; set; }
        [MaxLength(6)]
        [Required]
        public string Name { get; set; }
    }

    public static class ModelFooExtentions1
    {
        public static List<CompositeFoo> InitialiseAndLoadModelFoos(this List<CompositeFoo> compositeFoos)
        {
            compositeFoos = new List<CompositeFoo>();
            compositeFoos.Add(new CompositeFoo { Id = 1, SecondId = 1, Name = "Test 1" });
            compositeFoos.Add(new CompositeFoo { Id = 1, SecondId = 2, Name = "Test 2" });

            return compositeFoos;
        }
    }
}