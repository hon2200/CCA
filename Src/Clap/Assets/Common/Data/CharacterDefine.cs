using JetBrains.Annotations;
using System;
using System.Linq;

namespace Common.Data
{
    public class CharacterDefine
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public override string ToString()
        {

            return $"ID: {ID}, " +
                   $"Name: {Name}, " +
                   $"Description: {Description} ";
        }
    }
}