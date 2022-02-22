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

    public CompareResponse IsEqual(MyClass expected, MyClass actual)
    {
        response = new CompareResponse();

        Compare(nameof(MyClass), typeof(MyClass), expected, actual);

        return response;
    }

    private void Compare<T>(string propName, Type type, T expected, T actual)
    {
        if (type.IsClass && !type.FullName.StartsWith("System."))
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
        if(type.IsClass && !type.FullName.StartsWith("System."))
        {
            if(expected != null && actual != null && expected.Count == actual.Count)
            {
                List<object> expectedList = new();
                List<object> actualList = new();

                foreach (var expectedItem in expected)
                {
                    expectedList.Add(Convert.ChangeType(expectedItem, type));
                }
                foreach (var actualItem in actual)
                {
                    actualList.Add(Convert.ChangeType(actualItem, type));
                }

                var orderByProperty = type.GetProperties().First(p => p.GetType().FullName.StartsWith("System."));
                expectedList = expectedList.OrderBy(o => orderByProperty.GetValue(o)).ToList();
                actualList = actualList.OrderBy(o => orderByProperty.GetValue(o)).ToList();

                for (var i = 0; i < expected.Count; i++)
                {
                    foreach (var prop in type.GetProperties())
                    {
                        Compare(propName + "." + prop.Name, prop.PropertyType, prop.GetValue(expectedList[i]), prop.GetValue(actualList[i]));
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
