using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.PhantomJS;
using SimpleBrowser.WebDriver;
using Xunit;

namespace Sandbox.WebApi.FileUpload.Web.Tests
{
    public class FloodTest
    {
        [Fact]
        public async void Go()
        {
            var drivers = Enumerable
                .Range(0, 25)
                .Select(GetDriver)
                .ToArray();

            var tasks = drivers.Select(Upload);

            await Task.WhenAll(tasks);

            foreach (var driver in drivers) driver.Dispose();
        }

        static IWebDriver GetDriver(int i)
        {
            var driver = new FirefoxDriver();
            driver.Navigate().GoToUrl("http://localhost:55868/");

            Console.WriteLine(i);

            return driver;
        }

        static async Task Upload(IWebDriver driver)
        {
            driver.FindElement(By.CssSelector("#image1"))
                  .SendKeys(@"C:\Users\ant\SkyDrive\Pictures\Camera Roll\WP_20130909_003.mp4");
            driver.FindElement(By.CssSelector("#image2"))
                  .SendKeys(@"C:\Users\ant\SkyDrive\Pictures\Camera Roll\WP_20130908_003.mp4");
            driver.FindElement(By.CssSelector("#image3"))
                  .SendKeys(@"C:\Users\ant\SkyDrive\Pictures\Camera Roll\WP_20130908_002.mp4");

            driver.FindElement(By.CssSelector("[type='submit']")).Click();
        }
    }
}