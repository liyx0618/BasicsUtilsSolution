using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Util.Basics
{
    public static class AttributeExtensions
    {
        public static TValue GetAttribute<TAttribute, TValue>(
            this Type type
            , string MemberName
            , Func<TAttribute, TValue> valueSelector
            , bool inherit = false)
            where TAttribute : Attribute
        {
            var att = type.GetMember(MemberName).FirstOrDefault()
                .GetCustomAttributes(typeof(TAttribute), inherit)
                .FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return valueSelector(att);
            }
            return default;
        }


        class Program
        {
            static void Main()
            {
                // 读取 MyClass 类的 MyMethod 方法的 Description 特性的值
                var description = typeof(MyClass)
                    .GetAttribute("MyMethod", (DescriptionAttribute d) => d.Description);
                Console.WriteLine(description); // 输出：Hello
            }
        }
        public class MyClass
        {
            [Description("Hello")]
            public void MyMethod() { }
        }
    }
}
