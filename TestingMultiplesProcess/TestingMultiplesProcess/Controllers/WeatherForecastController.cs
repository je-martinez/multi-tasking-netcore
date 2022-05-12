using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace TestingMultiplesProcess.Controllers
{

    public static class ListHelper
    {
        public static List<List<T>> ChunkList<T>(IEnumerable<T> data, int size)
        {
            return data
              .Select((x, i) => new { Index = i, Value = x })
              .GroupBy(x => x.Index / size)
              .Select(x => x.Select(v => v.Value).ToList())
              .ToList();
        }
    }

    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private List<string> Entities()
        {
            List<string> items = new List<string>();
            for(int i = 0; i <= 5; i++)
            {
                items.Add($"Item {i}");
            }
            return items;
        }

        private async Task Process(string item)
        {
            //A: Setup and stuff you don't want timed
            var timer = new Stopwatch();
            timer.Start();

            //Paso #1
            await Task.Delay(5000);

            //Paso #2
            await Task.Delay(7000);

            //Paso #3
            await Task.Delay(3000);

            timer.Stop();
            TimeSpan timeTaken = timer.Elapsed;
            string foo = $"< {item} >Time taken: " + timeTaken.ToString(@"m\:ss\.fff");
            Console.WriteLine(foo);
        }

        private async Task Process2(string item)
        {
            //A: Setup and stuff you don't want timed
            var timer = new Stopwatch();
            timer.Start();
            var client = new RestClient("https://jsonplaceholder.typicode.com");
            var request = new RestRequest("posts", Method.Get);
            var timeline = await client.GetAsync<List<ResponseApiPlaceHolderDTO>>(request);
            timer.Stop();
            TimeSpan timeTaken = timer.Elapsed;
            string foo = $"< {item} >Time taken: " + timeTaken.ToString(@"m\:ss\.fff");
            Console.WriteLine(foo);
        }

        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {

            List<string> items = Entities();

            //A: Setup and stuff you don't want timed
            var timer = new Stopwatch();
            timer.Start();

            //var tasks = items.Select(q => 0{
            //    Console.WriteLine($"Tarea {q}");
            //    return Process(q);
            //}).ToList();


            List<List<string>> listOfList = ListHelper.ChunkList<string>(items.AsEnumerable(), 300);


            //#Forma #1
            //listOfList.ForEach(async item =>
            //{
            //    var timer = new Stopwatch();
            //    timer.Start();

            //    var tasks = item.Select(q => Process(q)).ToList();
            //    await Task.WhenAll(tasks.ToArray());

            //    //var tasks = items.Select(q => 0{
            //    //    Console.WriteLine($"Tarea {q}");
            //    //    return Process(q);
            //    //}).ToList();

            //    timer.Stop();
            //    TimeSpan timeTaken = timer.Elapsed;
            //    string foo = $"<All Finish List N>Time taken: " + timeTaken.ToString(@"m\:ss\.fff");
            //    Console.WriteLine(foo);
            //});

            //Forma #2
            var a = listOfList.Select(q => q.Select(i => Process2(i))).SelectMany(q => q).ToList();
            await Task.WhenAll(a.ToArray());

            //foreach (var item in listOfList.Select((value, i) => new { i, value }))
            //{
            //    var value = item.value;
            //    var index = item.i;
            //    Console.WriteLine($"Lista #{index} - {value.Count()}");
            //}

            //await Task.WhenAll(tasks.ToArray());

            timer.Stop();
            TimeSpan timeTaken = timer.Elapsed;
            string foo = "<All Finish >Time taken: " + timeTaken.ToString(@"m\:ss\.fff");
            Console.WriteLine(foo);

            //items.ForEach(async (item) =>
            //{
            //    //A: Setup and stuff you don't want timed
            //    var timer = new Stopwatch();
            //    timer.Start();

            //    await Process(item);

            //    //B: Run stuff you want timed
            //    timer.Stop();

            //    TimeSpan timeTaken = timer.Elapsed;
            //    string foo = "Time taken: " + timeTaken.ToString(@"m\:ss\.fff");
            //    Console.WriteLine(foo);
            //});




            return new List<string>().AsEnumerable();
        }
    }
}
