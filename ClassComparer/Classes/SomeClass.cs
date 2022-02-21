using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassComparer;

public class SomeClass
{
    public List<string> List { get; set; }
    public List<SomeClassEnum> List2 { get; set; }
}

public enum SomeClassEnum
{
    AAA,
    BBB,
    CCC
}