using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Cars
{
    class Program
    {
        static void Main(string[] args)
        {
            var cars = ProcessCars("fuel.csv");
            var manufacturers = ProcessManufacturers("manufacturers.csv");

            var query =
                from car in cars
                group car by car.Manufacturer.ToUpper()
                into maufacturer
                orderby maufacturer.Key
                select maufacturer;

            //foreach (var car in query)
            //{
            //    Console.WriteLine($"{car.Key} has {car.Count()} cars.");
            //}

            //foreach (var group in query)
            //{
            //    Console.WriteLine(group.Key);
            //    foreach (var car in group.OrderByDescending(c => c.Combined).Take(2))
            //    {
            //        Console.WriteLine($"\t{car.Name}:{car.Combined}");
            //    }
            //}

            var query2 =
                cars.GroupBy(c => c.Manufacturer.ToUpper())
                    .OrderBy(g => g.Key);

            var query3 =
                from manufacturer in manufacturers
                join car in cars on manufacturer.Name equals car.Manufacturer
                    into carGroup
                orderby manufacturer.Name
                select new
                {
                    Manufacturer = manufacturer,
                    cars = carGroup
                };

            //foreach (var group in query3)
            //{
            //    Console.WriteLine($"{group.Manufacturer.Name}:{group.Manufacturer.Headquarters}");
            //    foreach (var car in group.cars.OrderByDescending(c => c.Combined).Take(2))
            //    {
            //        Console.WriteLine($"\t{car.Name}:{car.Combined}");
            //    }
            //}


            var query4 =
                manufacturers.GroupJoin(cars, m => m.Name, c => c.Manufacturer,
                    (m, g) =>
                        new
                        {
                            Manufacture = m,
                            carGroup = g
                        })
                    .OrderBy(m=>m.Manufacture.Name);

            var query5 =
                from manufacturer in manufacturers
                join car in cars on manufacturer.Name equals car.Manufacturer into cargroup
                select new
                {
                    Manufacturer = manufacturer,
                    Car = cargroup
                }
                into result
                group result by result.Manufacturer.Headquarters;

            //foreach (var group in query5)
            //{
            //    Console.WriteLine($"{group.Key}");

            //    foreach (var car in group.SelectMany(g => g.Car)  //under each headquarter, you have many manufacter group, the selectmany will select cars from all of the group under the same country
            //                               .OrderByDescending(c => c.Combined).Take(3))
            //    {
            //        Console.WriteLine($"\t{car.Name}:{car.Combined}");
            //    }
            //}

            // dose the same job as query 5
            var query6 =
                from manufacturer in manufacturers
                join car in cars on manufacturer.Name equals car.Manufacturer
                select new
                {
                    Manufacturer = manufacturer,
                    Car = car
                }
                into result
                group result by result.Manufacturer.Headquarters;

            //dose the same job as query 6
            var query7 =
                manufacturers.Join(cars, m => m.Name, c => c.Manufacturer,
                    (m, c) => new
                    {
                        Manufacturer = m,
                        Car = c
                    })
                    .GroupBy(m => m.Manufacturer.Headquarters);

            //foreach (var group in query7)
            //{
            //    Console.WriteLine($"{group.Key}");

            //    foreach (var car in group.Select(g => g.Car)  // under each headquarter, you have many cars which are belong to the same country, so here we can use select
            //                               .OrderByDescending(c => c.Combined).Take(3))
            //    {
            //        Console.WriteLine($"\t{car.Name}:{car.Combined}");
            //    }
            //}

            var query8 =
                from car in cars
                group car by car.Manufacturer
                into carGroup
                select new
                {
                    Name = carGroup.Key,
                    Max = carGroup.Max(c => c.Combined),
                    Min = carGroup.Min(c => c.Combined),
                    Avg = carGroup.Average(c => c.Combined),
                    Cars = carGroup
                }
                into aggresult
                orderby aggresult.Max
                select aggresult;

            var query9 =
                cars.GroupBy(c => c.Manufacturer)
                    .Select(g =>
                        new
                        {
                            Name = g.Key,
                            Max = g.Max(c => c.Combined),
                            Min = g.Min(c => c.Combined),
                            Avg = g.Average(c => c.Combined),
                            Cars = g
                        })
                    .OrderBy(s => s.Max);

            foreach (var result in query9)
            {
                Console.WriteLine($"Name: {result.Name}");
                Console.WriteLine($"\tMax: {result.Max}");
                Console.WriteLine($"\tMin: {result.Min}");
                Console.WriteLine($"\tAvg: {result.Avg}");
                
                //foreach (var car in result.Cars.OrderByDescending(c => c.Combined).Take(3))
                //{
                //    Console.WriteLine($"\t{car.Name}:{car.Combined}");
                //}
            }



        }





        private static List<Car> ProcessCars(string path)
        {
            var query =

                File.ReadAllLines(path)
                    .Skip(1)
                    .Where(l => l.Length > 1)
                    .ToCar();

            return query.ToList();
        }

        private static List<Manufacturer> ProcessManufacturers(string path)
        {
            var query =
                   File.ReadAllLines(path)
                       .Where(l => l.Length > 1)
                       .Select(l =>
                       {
                           var columns = l.Split(',');
                           return new Manufacturer
                           {
                               Name = columns[0],
                               Headquarters = columns[1],
                               Year = int.Parse(columns[2])
                           };
                       });
            return query.ToList();
        }
    }

    public static class CarExtensions
    {        
        public static IEnumerable<Car> ToCar(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var columns = line.Split(',');

                yield return new Car
                {
                    Year = int.Parse(columns[0]),
                    Manufacturer = columns[1],
                    Name = columns[2],
                    Displacement = double.Parse(columns[3]),
                    Cylinders = int.Parse(columns[4]),
                    City = int.Parse(columns[5]),
                    Highway = int.Parse(columns[6]),
                    Combined = int.Parse(columns[7])
                };
            }
        }
    }
}
