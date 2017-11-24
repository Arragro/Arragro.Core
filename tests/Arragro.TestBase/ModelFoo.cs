using Arragro.Core.Common.BusinessRules;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Arragro.TestBase
{
    public class ModelFoo
    {
        public int Id { get; set; }
        [MaxLength(6)]
        [Required]
        public string Name { get; set; }
    }

    public class ModelFooInt : Auditable<int>
    {
        public int Id { get; set; }
    }

    public class ModelFooGuid : Auditable<Guid>
    {
        public int Id { get; set; }
    }

    public static class ModelFooExtentions
    {
        public static List<ModelFoo> InitialiseAndLoadModelFoos(this List<ModelFoo> modelFoos)
        {
            modelFoos = new List<ModelFoo>();
            modelFoos.Add(new ModelFoo { Id = 1, Name = "Test 1" });
            modelFoos.Add(new ModelFoo { Id = 2, Name = "Test 2" });

            return modelFoos;
        }
    }
}