using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ClassComparer;

public class CompareResponse
{
    public bool IsEqual => !Differences.Any();
    public List<Difference> Differences { get; set; } = new List<Difference>();
}
public class Difference
{
    public string Property { get; set; }
    public object Expected { get; set; }
    public object Actual { get; set; }
}

public class Comparer
{
    private CompareResponse response;

    private readonly List<string> classTypes = new List<string> { "MyClass", "MyOtherClass", "SomeClass" };

    public CompareResponse IsEqual(MyClass expected, MyClass actual)
    {
        response = new CompareResponse();

        Compare(nameof(MyClass), typeof(MyClass), expected, actual);

        return response;
    }

    private void Compare<T>(string propName, Type type, T expected, T actual)
    {
        if (classTypes.Contains(type.Name))
        {
            foreach (var prop in type.GetProperties())
            {
                Compare(propName + "." + prop.Name, prop.PropertyType, prop.GetValue(expected), prop.GetValue(actual));
            }
        }
        else if (type.Name == typeof(List<>).Name)
        {
            CompareList(propName, expected.GetType().GenericTypeArguments[0], expected as IList, actual as IList);
        }
        else
        {
            if (!expected.Equals(actual))
                response.Differences.Add(new Difference() { Property = propName, Actual = actual, Expected = expected });
        }
    }

    private void CompareList(string propName, Type type, IList? expected, IList? actual)
    {
        if(classTypes.Contains(type.Name))
        {
            if(expected != null && actual != null && expected.Count == actual.Count)
            {
                for (var i = 0; i < expected.Count; i++)
                {
                    foreach (var prop in type.GetProperties())
                    {
                        Compare(propName + "." + prop.Name, prop.PropertyType, prop.GetValue(expected[i]), prop.GetValue(actual[i]));
                    }
                }
            }
            else
            {
                response.Differences.Add(new Difference() { Property = propName, Actual = JsonConvert.SerializeObject(actual), Expected = JsonConvert.SerializeObject(expected) });
            }
        }
        else
        {
            var actualContainsAll = true;
            var expectedContainsAll = true;

            foreach (var expectedItem in expected)
            {
                if (!actual.Contains(expectedItem))
                {
                    actualContainsAll = false;
                    continue;
                }
            }

            foreach (var actualItem in actual)
            {
                if (!expected.Contains(actualItem))
                {
                    expectedContainsAll = false;
                    continue;
                }
            }

            if(!actualContainsAll || !expectedContainsAll)
            {
                response.Differences.Add(new Difference() { Property = propName, Actual = JsonConvert.SerializeObject(actual), Expected = JsonConvert.SerializeObject(expected) });
            }
        }
    }
}
