using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassComparer;
public class MyClass
{
    public string Version { get; set; }
    public Guid Guid { get; set; }
    public List<MyOtherClass> OtherClasses { get; set; }

}
