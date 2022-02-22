// See https://aka.ms/new-console-template for more information
using ClassComparer;

Console.WriteLine("Hello, World!");

var expected = new MyClass() {
    Guid = Guid.NewGuid(),
    OtherClasses = new List<MyOtherClass> {
        new MyOtherClass() { 
            Description = "JKL", 
            Name = "YUI", 
            SomeClass = new SomeClass() { 
                List = new List<string> { "A", "B", "C" },
                List2 = new List<SomeClassEnum> { SomeClassEnum.BBB, SomeClassEnum.AAA}
            },
        },
        new MyOtherClass() {
            Description = "ASD",
            Name = "ZXC",
            SomeClass = new SomeClass() {
                List = new List<string> { "A", "D", "Z" },
                List2 = new List<SomeClassEnum> { SomeClassEnum.BBB, SomeClassEnum.AAA}
            },
        },
    },
    Version = "1"
};

var actual = new MyClass()
{
    Guid = Guid.NewGuid(),
    OtherClasses = new List<MyOtherClass> {
        new MyOtherClass() {
            Description = "ASD",
            Name = "ZXC",
            SomeClass = new SomeClass() {
                List = new List<string> { "A", "B", "C" },
                List2 = new List<SomeClassEnum> { SomeClassEnum.AAA, SomeClassEnum.BBB }
            },
        },
        new MyOtherClass() {
            Description = "JKL",
            Name = "YUI",
            SomeClass = new SomeClass() {
                List = new List<string> { "A", "D", "Z" },
                List2 = new List<SomeClassEnum> { SomeClassEnum.BBB, SomeClassEnum.AAA}
            },
        }
    },
    Version = "2"
};

var comparer = new Comparer();

var resp = comparer.IsEqual(expected, actual);

Console.ReadLine();
