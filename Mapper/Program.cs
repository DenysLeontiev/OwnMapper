using System;
using System.Linq;
using System.Reflection;

namespace Mapper
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Mapper mapper = new Mapper();
            
            var person = new Person()
            {
                FirstName = "Den",
                LastName = "Leon"
            };

            var dto = mapper.Map<PersonDto>(person);

            Console.WriteLine($"Person name: {person.FirstName} | Dto name: {dto.FirstName}");
        }
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    
    public class PersonDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // public override string ToString()
        // {
        //     return $"My FirstName is {FirstName} and my LastName is {LastName}";
        // }
    }

    public interface IMapper
    {
        TDest Map<TSource, TDest>(TSource source) where TDest : new();
        TDest Map<TDest>(object source) where TDest : new();
    }

    public class Mapper : IMapper
    {
        public TDest Map<TSource, TDest>(TSource source) where TDest : new()
        {
            return Map<TDest>(source);
        }

        public TDest Map<TDest>(object source) where TDest : new()
        {
            var sourceProps = source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var destProps = typeof(TDest).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var mappedResult = new TDest();

            foreach (var destProp in destProps)
            {
                var sourceProp = sourceProps.FirstOrDefault(x => x.Name.Equals(destProp.Name));

                if (sourceProp != null && sourceProp.CanRead && destProp.CanWrite)
                {
                    destProp.SetValue(mappedResult, sourceProp.GetValue((source)));
                }
            }

            return mappedResult;
        }
    }
}