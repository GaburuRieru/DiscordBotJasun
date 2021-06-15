using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace CuteNoisesBot
{
    public static class ContentDelivery
    {
        //private const string NoiseFolder = "Noises";

        public static Uri GetUri(string contentDomain, string playsoundPath)
        {
            //var fullPath = Path.Combine("http:\\\\" ,contentDomain, playsoundPath);
            //var fullPath = Path.Combine(contentDomain, playsoundPath);
            //var fullPath = "http://d2x0wgjz0bg9e5.cloudfront.net/Noises/koroneBonk.ogg";
            //Console.WriteLine($"FullPath is: {fullPath}");
            return new Uri($"https://{contentDomain}/{playsoundPath}");
        }
    }
}