using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Selenium.Test
{
    [TestFixture]
    public class BrowserTests
    {
        private static IWebDriver driver;
        private static HelperTest helperTest;
        string mainURL;
        string homeUrl;
        string authUrl;

        private string password;
        private string login;
        
        string mainURLs = "https://cascade-usa.com/";

        [SetUp]
        public void SetUp()
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var pathDrivers = directory + "/../../../../drivers/";
            mainURL = mainURLs + "";
            homeUrl = mainURLs + "home";
            authUrl = mainURLs + "auth/login";

            login = "sergeykorolevsky2015@gmail.com";
            password = "StevenGerrard_2015";

            //login = "sergeycascade01@yandex.com";
            //password = "StevenGerrard_01";

            helperTest = new HelperTest();

            //Chrome
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--headless");
            driver = new ChromeDriver(pathDrivers, options);

            //Firefox
            //FirefoxOptions options = new FirefoxOptions();
            //FirefoxDriverService geckoService = FirefoxDriverService.CreateDefaultService(pathDrivers);
            //geckoService.Host = "::1";
            //driver = new FirefoxDriver(geckoService, options);

            //Edge
            //EdgeOptions edgeOptions = new EdgeOptions();
            //string msedgedriverExe = @"msedgedriver.exe";
            //EdgeDriverService service = EdgeDriverService.CreateDefaultService(pathDrivers, msedgedriverExe);
            //driver = new EdgeDriver(service, edgeOptions);

            driver.Manage().Cookies.DeleteAllCookies();
            driver.Manage().Window.Size = new System.Drawing.Size(1910, 1024);

            //((ITakesScreenshot)driver).GetScreenshot().SaveAsFile("Test3.png", ScreenshotImageFormat.Png); - screenshots
        }

        [Test]
        public void Login()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            Assert.AreEqual(homeUrl, driver.Url);            

            helperTest.LogOut(driver, authUrl);
        }

        [Test]
        public void LoginWrongCreds()
        {
            password = "fgdgf";

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            Assert.AreEqual(authUrl, driver.Url);
        }              

        [Test]
        public void AccessoryParts()
        {
            string bodyTextProduct;

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            Assert.AreEqual(homeUrl, driver.Url);
            
            IWebElement SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.Clear();
            SearchBox.SendKeys("tamarack");
            SearchBox.SendKeys(Keys.Enter);

            helperTest.waitElementId(driver, 60, "configurable_img_0");
            helperTest.JsClickElement(driver, "//*[text()='" + "Tamarack® Flexure Joint™" + "']");

            helperTest.waitElementId(driver, 60, "product_name_in_product_page");
            Assert.AreEqual(mainURLs + "product?productID=502&Name=tamarack%C2%AE-flexure-joint%E2%84%A2", driver.Url);

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/div[1]/div[2]/div[4]/div[2]/span[1]/span");
            helperTest.JsClickElement(driver, "//*[text()='" + "Accessories" + "']");

            Thread.Sleep(3000);

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div[1]/app-product/div[4]/div/div[1]/div[2]/div/div[1]/section/div/h6[1]/input");
            IWebElement PartNumber = driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-product/div[4]/div/div[1]/div[2]/div/div[1]/section/div/h6[1]/input"));
            PartNumber.Clear();
            PartNumber.SendKeys("740-L-BLK-5PK");

            Thread.Sleep(3000);

            string item1 = driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-product/div[4]/div/div[1]/div[2]/div/div[1]/div/article[2]/div[3]/p")).Text;
            helperTest.InputStringXpath(driver, "2", "/html/body/app-root/div[1]/app-product/div[4]/div/div[1]/div[2]/div/div[1]/div/article[2]/div[7]/p/app-qty/input");
            helperTest.JsClickElementID(driver, "accessory_add_to_cart_1");
            Thread.Sleep(6000);

            string item2 = driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-product/div[4]/div/div[1]/div[2]/div/div[1]/div/article[6]/div[3]/p")).Text;
            helperTest.InputStringXpath(driver, "4", "/html/body/app-root/div[1]/app-product/div[4]/div/div[1]/div[2]/div/div[1]/div/article[6]/div[7]/p/app-qty/input");
            helperTest.JsClickElementID(driver, "accessory_add_to_cart_5");
            Thread.Sleep(6000);

            driver.FindElement(By.XPath("//html/body/app-root/div[1]/app-product/div[4]/div/div[1]/div[1]/app-close-button/p/span")).Click();
            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div[1]/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span");
            driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span")).Click();
            Thread.Sleep(1000);

            helperTest.InputStringXpath(driver, "740-L-BLK-5PK", "/html/body/app-root/div[1]/app-product/div[1]/div[3]/section/div/h6[1]/input");
            helperTest.waitElementId(driver, 60, "add_product_to_cart0");
            helperTest.JsClickElementID(driver, "add_product_to_cart0");

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div[1]/app-product/div[5]/div/div/div[1]/app-close-button/p/span");
            driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-product/div[5]/div/div/div[1]/app-close-button/p/span")).Click();

            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

            helperTest.waitElementId(driver, 60, "product-name-in-cart0");

            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;

            Assert.IsTrue(bodyTextProduct.Contains("TAMARACK FLEXURE JOINT BLACK LG 5PK"));
            Assert.IsTrue(bodyTextProduct.Contains(item1));
            Assert.IsTrue(bodyTextProduct.Contains(item2));

            Thread.Sleep(1000);

            for (int j = 0; j < 3; j++)
            {
                helperTest.JsClickElementID(driver, "remove-button-0");

                Thread.Sleep(1000);
            }
        }

        [Test]
        public void AddToCartFromPreview()
        {
            Actions actions = new Actions(driver);
            String bodyTextProduct;
            IWebElement NavigateCursor;

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);            

            Assert.AreEqual(homeUrl, driver.Url);            

            IWebElement SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.SendKeys("liners AK");
            SearchBox.SendKeys(Keys.Enter);

            helperTest.waitElementId(driver, 60, "configurable_img_0");

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/div/div[2]/div/div[1]/app-product-card-configurable/section/div[1]/a/img");

            NavigateCursor = driver.FindElement(By.XPath("/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/div/div[2]/div/div[1]/app-product-card-configurable/section/div[1]/a/img"));
            actions.MoveToElement(NavigateCursor).Build().Perform();

            helperTest.JsClickElement(driver, "//*[text()='" + " Preview " + "']");

            Thread.Sleep(3000);

            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/app-preview-details-panel/section/div/div[2]/mdb-card/div/mdb-card-body/mdb-card-text/p/div/span[1]")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Alpha Classic® AK Liner"));

            helperTest.UseDropDownByName(driver, "/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/app-preview-details-panel/section/div/div[2]/mdb-card/div/mdb-card-body/mdb-card-text/p/div/app-attributes/form/div/div[1]/select", "Locking");
            helperTest.UseDropDownByName(driver, "/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/app-preview-details-panel/section/div/div[2]/mdb-card/div/mdb-card-body/mdb-card-text/p/div/app-attributes/form/div/div[2]/select", "Large");
            helperTest.UseDropDownByName(driver, "/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/app-preview-details-panel/section/div/div[2]/mdb-card/div/mdb-card-body/mdb-card-text/p/div/app-attributes/form/div/div[3]/select", "MAX");

            helperTest.UseDropDownByName(driver, "/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/app-preview-details-panel/section/div/div[2]/mdb-card/div/mdb-card-body/mdb-card-text/p/div/app-attributes/form/div/div[4]/select", "Standard Umbrella");

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            Thread.Sleep(1000);

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/app-preview-cart-panel/section/div/div[2]/article/div[1]/div[1]/app-product-card[1]/article/div[2]/div[1]/p[2]");

            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/app-preview-cart-panel/section/div/div[2]/article/div[1]/div[1]/app-product-card[1]/article/div[2]/div[1]/p[2]")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("AKL-2636-X"));            

            helperTest.JsClickElement(driver, "/html/body/app-root/div[1]/app-category/div/div/div[2]/app-configurable/app-preview-cart-panel/section/div/div[2]/article/div[2]/app-button/div/button");

            Thread.Sleep(3000);

            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("AKL-2636-X"));

            Thread.Sleep(2000);

            helperTest.JsClickElementID(driver, "remove-button-0");
        }

        [Test]
        public void CheckImagesOnPages()
        {
            IWebElement Img;
            Boolean ImagePresent;

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL); ;

            Assert.AreEqual(homeUrl, driver.Url);

            helperTest.waitElementId(driver, 60, "search");

            for (int i = 1; i <= 5; i++)
            {
                string path = "/html/body/app-root/div/app-home/div/div[2]/div[2]/div[" + i.ToString() + "]/ app-product-card/mdb-card/div/a/mdb-card-img/img";
                Img = driver.FindElement(By.XPath(path));

                ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);
                Assert.IsTrue(ImagePresent);
            }

            driver.Url = mainURLs + "product?productID=255&Name=ossur®-miami-jto®-thoracic-extension";

            helperTest.waitElementId(driver, 60, "myimage");

            Thread.Sleep(1000);

            Img = driver.FindElement(By.Id("myimage"));

            ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);

            Assert.IsTrue(ImagePresent);

            IWebElement SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.SendKeys("orthotics");

            SearchBox.SendKeys(Keys.Enter);

            helperTest.waitElementId(driver, 60, "configurable_img_0");            

            for (int i = 0; i < 25; i++)
            {
                if (i == 2) i += 1;
                string ids = "configurable_img_" + i.ToString();

                Img = driver.FindElement(By.Id(ids));


                ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);

                Assert.IsTrue(ImagePresent);
            }
        }

        //[Test]
        public void CMSpages()
        {
            IWebElement Img;
            Boolean ImagePresent;
            String bodyTextProduct;
            string path;

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            //About Cascade

            helperTest.JsClickElement(driver, "//*[text()='" + "About Cascade" + "']");            

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div[1]/app-page/div/div/div/div/div[1]/h1");

            Assert.IsTrue(driver.Url.Contains("pages/aboutus"));

            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-page/div/div/div/div/div[1]/h1")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("About Us"));

            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-page/div/div/div/div/div[2]/div")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Cascade is a wholesale distributor of orthotics, prosthetics, materials, and equipment."));

            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-page/div/div/div/div/div[3]/div")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Provide solutions that improve the quality of life."));

            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-page/div/div/div/div/div[4]/div")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Continually innovate and expand our solutions through a culture of partnership, excellence in operations and service, and technological efficiency."));

            Thread.Sleep(2000);

            for (int i = 1; i <= 9; i++)
            {
                path = "/html/body/app-root/div[1]/app-page/div/div/div/div/div[6]/div[" + i.ToString() + "]/a/img";
                Img = driver.FindElement(By.XPath(path));
                ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);
                Assert.IsTrue(ImagePresent);
            }

            helperTest.JsClickElement(driver, "/html/body/app-root/div[1]/app-page/div/div/div/div/div[6]/div[1]/a/img");            
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            Thread.Sleep(3000);
            Assert.IsTrue(driver.Url.Contains("v2prodbackend.azurewebsites.net/api/digital/asset/get?fileName=2020+Cascade+Major+Landmarks.pdf"));
            driver.Close();
            driver.SwitchTo().Window(driver.WindowHandles[0]);

            Thread.Sleep(1000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div[1]/app-page/div/div/div/div/div[6]/div[2]/a/img");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            Thread.Sleep(8000);
            Assert.IsTrue(driver.Url.Contains("v2prodbackend.azurewebsites.net/api/digital/asset/get?fileName=Cascade_DC+Map_FINAL_2020.pdf"));
            driver.Close();
            driver.SwitchTo().Window(driver.WindowHandles[0]);

            Thread.Sleep(1000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div[1]/app-page/div/div/div/div/div[6]/div[5]/a/img");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            Thread.Sleep(8000);
            Assert.IsTrue(driver.Url.Contains("v2prodbackend.azurewebsites.net/api/digital/asset/get?fileName=transit_timeCOS.pdf"));
            driver.Close();
            driver.SwitchTo().Window(driver.WindowHandles[0]);

            Thread.Sleep(1000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div[1]/app-page/div/div/div/div/div[6]/div[8]/a/img");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            Thread.Sleep(3000);
            Assert.IsTrue(driver.Url.Contains("v2prodbackend.azurewebsites.net/api/digital/asset/get?fileName=2020+Integrations+Logo+Sheet.pdf"));
            driver.Close();
            driver.SwitchTo().Window(driver.WindowHandles[0]);

            //Resources

            helperTest.JsClickElement(driver, "//*[text()='" + "Resources" + "']");            

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div[1]/app-page/div/div/div/div/div[1]/h1");

            Assert.IsTrue(driver.Url.Contains("/pages/resources"));

            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-page/div/div/div/div/div[1]/h1")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Resources"));

            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-page/div/div/div/div/div[2]/div")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("For over 47 years, Cascade Orthopedic Supply has been a leader provider of orthotic and prosthetic products across North America."));

            Thread.Sleep(2000);

            for (int i = 1; i <= 6; i++)
            {
                path = "/html/body/app-root/div[1]/app-page/div/div/div/div/div[3]/div[" + i.ToString() + "]/a/img";
                Img = driver.FindElement(By.XPath(path));
                ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);
                Assert.IsTrue(ImagePresent);
            }

            helperTest.JsClickElement(driver, "/html/body/app-root/div[1]/app-page/div/div/div/div/div[3]/div[1]/a/img");            
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            Thread.Sleep(3000);
            Assert.IsTrue(driver.Url.Contains("v2prodbackend.azurewebsites.net/api/digital/asset/get?fileName=credit_app.pdf"));
            driver.Close();
            driver.SwitchTo().Window(driver.WindowHandles[0]);

            //Industry Partners

            helperTest.JsClickElement(driver, "/html/body/app-root/div[1]/app-page/div/div/div/div/div[3]/div[5]/a/img");

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div[1]/app-page/div/div/div/div/div[1]/h1");

            Assert.IsTrue(driver.Url.Contains("/pages/partners"));

            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-page/div/div/div/div/div[1]/h1")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Industry Partners"));

            Thread.Sleep(4000);

            path = "/html/body/app-root/div[1]/app-page/div/div/div/div/div[2]/div[1]/a/img";
            Img = driver.FindElement(By.XPath(path));
            ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);
            Assert.IsTrue(ImagePresent);

            path = "/html/body/app-root/div[1]/app-page/div/div/div/div/div[2]/div[2]/img";
            Img = driver.FindElement(By.XPath(path));
            ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);
            Assert.IsTrue(ImagePresent);

            path = "/html/body/app-root/div[1]/app-page/div/div/div/div/div[2]/div[3]/a/img";
            Img = driver.FindElement(By.XPath(path));
            ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);
            Assert.IsTrue(ImagePresent);

            for (int i = 1; i <= 7; i++)
            {
                path = "/html/body/app-root/div[1]/app-page/div/div/div/div/div[3]/div[" + i.ToString() + "]/div[1]/a/img";
                Img = driver.FindElement(By.XPath(path));
                ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);
                Assert.IsTrue(ImagePresent);
            }

            for (int i = 1; i <= 7; i++)
            {
                path = "/html/body/app-root/div[1]/app-page/div/div/div/div/div[3]/div[" + i.ToString() + "]/div[2]/a/img";
                Img = driver.FindElement(By.XPath(path));
                ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);
                Assert.IsTrue(ImagePresent);
            }

            path = "/html/body/app-root/div[1]/app-page/div/div/div/div/div[3]/div[8]/div/a/img";
            Img = driver.FindElement(By.XPath(path));
            ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);
            Assert.IsTrue(ImagePresent);

            path = "/html/body/app-root/div[1]/app-page/div/div/div/div/div[3]/div[9]/div[1]/a/img";
            Img = driver.FindElement(By.XPath(path));
            ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);
            Assert.IsTrue(ImagePresent);

            path = "/html/body/app-root/div[1]/app-page/div/div/div/div/div[3]/div[9]/div[2]/a/img";
            Img = driver.FindElement(By.XPath(path));
            ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);
            Assert.IsTrue(ImagePresent);

            //Industry Pressroom

            helperTest.JsClickElement(driver, "//*[text()='" + "Resources" + "']");

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div[1]/app-page/div/div/div/div/div[3]/div[6]/a/img");
            helperTest.JsClickElement(driver, "/html/body/app-root/div[1]/app-page/div/div/div/div/div[3]/div[6]/a/img");

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div[1]/app-page/div/div/div/div/div[1]/h1");

            Assert.IsTrue(driver.Url.Contains("/pages/pressroom"));

            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-page/div/div/div/div/div[1]/h1")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Industry Pressroom"));

            Thread.Sleep(4000);

            for (int i = 1; i <= 3; i++)
            {
                path = "/html/body/app-root/div[1]/app-page/div/div/div/div/div[3]/div[1]/div[" + i.ToString() + "]/a/img";
                Img = driver.FindElement(By.XPath(path));
                ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);
                Assert.IsTrue(ImagePresent);
            }

            for (int i = 1; i <= 3; i++)
            {
                path = "/html/body/app-root/div[1]/app-page/div/div/div/div/div[3]/div[2]/div[" + i.ToString() + "]/a/img";
                Img = driver.FindElement(By.XPath(path));
                ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);
                Assert.IsTrue(ImagePresent);
            }

            for (int i = 1; i <= 2; i++)
            {
                path = "/html/body/app-root/div[1]/app-page/div/div/div/div/div[3]/div[3]/div[" + i.ToString() + "]/a/img";
                Img = driver.FindElement(By.XPath(path));
                ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);
                Assert.IsTrue(ImagePresent);
            }

            //Suppliers

            helperTest.JsClickElement(driver, "//*[text()='" + "Suppliers" + "']");

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div[1]/app-page/div/div[1]/div/div[1]/h1");

            Assert.IsTrue(driver.Url.Contains("/pages/suppliers"));

            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-page/div/div[1]/div/div[1]/h1")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Supplier Partners"));

            Thread.Sleep(4000);

            path = "/html/body/app-root/div[1]/app-page/div/div[1]/div/div[2]/div/img";
            Img = driver.FindElement(By.XPath(path));
            ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);
            Assert.IsTrue(ImagePresent);

            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-page/div/div[2]/div/span[2]/a")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("3M Company"));

            helperTest.JsClickElement(driver, "/html/body/app-root/div[1]/app-page/div/div[2]/div/span[2]/a");

            helperTest.waitElementId(driver, 60, "configurable_img_0");

            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/div/app-search-result-info-panel[1]/div/div[2]/span[1]/span[2]/span")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Supplier: \"3M Company\""));

            //COVID19 update

            helperTest.JsClickElement(driver, "//*[text()='" + "COVID19 Update" + "']");

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div[1]/app-page/div/div/h3[5]");

            Assert.IsTrue(driver.Url.Contains("/pages/covid19-update"));

            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-page/div/div/h3[5]")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("COVID-19 Workplace Safety and Operational Resources"));

            Thread.Sleep(4000);

            path = "/html/body/app-root/div[1]/app-page/div/div/p[1]/strong/img";
            Img = driver.FindElement(By.XPath(path));
            ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);
            Assert.IsTrue(ImagePresent);

            //Privacy Police

            helperTest.JsClickElement(driver, "//*[text()='" + "Privacy Policy" + "']");

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div[1]/app-page/div/div/div[2]/p[2]/span/b");

            Assert.IsTrue(driver.Url.Contains("/pages/privacy-policy"));

            Thread.Sleep(2000);

            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-page/div/div/div[2]/p[2]/span/b")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Privacy Policy"));

            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Please read this Policy carefully to understand our policies and practices regarding your personal information and how we will treat it"));

            //Terms of Sale

            helperTest.JsClickElement(driver, "//*[text()='" + "Terms of Sale" + "']");

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div[1]/app-page/div/div/h1/span/span/span");

            Assert.IsTrue(driver.Url.Contains("/pages/terms-of-sale"));

            Thread.Sleep(2000);

            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-page/div/div/h1/span/span/span")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Terms of Sale"));

            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Shipping charges are assessed according to the carrier service rate at the time of service."));

            //Continuing Education

            helperTest.JsClickElement(driver, "//*[text()='" + "Continuing Education" + "']");

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div[1]/app-page/div/div/div/div/div[1]/h1");

            Assert.IsTrue(driver.Url.Contains("/pages/continuing-education"));
            driver.Url.Contains("/pages/continuing-education");

            Thread.Sleep(2000);

            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-page/div/div/div/div/div[1]/h1")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Continuing Education"));

            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("All courses will be offered remotely until further notice. Please fill out the form below and a Cascade Team member will be happy to help you!"));

            Thread.Sleep(2000);

            for (int i = 1; i <= 2; i++)
            {
                path = "/html/body/app-root/div[1]/app-page/div/div/div/div/div[2]/div/div[" + i.ToString() + "]/a/img";
                Img = driver.FindElement(By.XPath(path));
                ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);
                Assert.IsTrue(ImagePresent);
            }

            //Contact Us

            helperTest.JsClickElement(driver, "//*[text()='" + "Contact Us" + "']");

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div[1]/app-page/div/div/div/div[1]/div[1]/h1");

            Assert.IsTrue(driver.Url.Contains("/pages/contact-us"));

            Thread.Sleep(2000);

            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-page/div/div/div/div[1]/div[1]/h1")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Contact Us"));

            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Have questions on the products or services Cascade offers? Fill out the form below and someone will be happy to help you!"));

            Thread.Sleep(2000);

            for (int i = 1; i <= 4; i++)
            {
                path = "/html/body/app-root/div[1]/app-page/div/div/div/div[1]/div[2]/div[1]/div[" + i.ToString() + "]/a/img";
                Img = driver.FindElement(By.XPath(path));
                ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);
                Assert.IsTrue(ImagePresent);
            }

            for (int i = 1; i <= 2; i++)
            {
                path = "/html/body/app-root/div[1]/app-page/div/div/div/div[1]/div[2]/div[2]/div[" + i.ToString() + "]/img";
                Img = driver.FindElement(By.XPath(path));
                ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);
                Assert.IsTrue(ImagePresent);
            }

            Thread.Sleep(1000);
        }

        [Test]
        public void Comparision()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            Actions actions = new Actions(driver);
            String bodyTextProduct;
            IWebElement NavigateCursor;

            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));

            SearchBox.Clear();
            SearchBox.SendKeys("RevoFit");
            SearchBox.SendKeys(Keys.Enter);

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/div/div[2]/div/div[1]/app-product-card-configurable/section/div[1]/a/img");
            NavigateCursor = driver.FindElement(By.XPath("/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/div/div[2]/div/div[1]/app-product-card-configurable/section/div[1]/a/img"));
            actions.MoveToElement(NavigateCursor).Build().Perform();

            helperTest.JsClickElement(driver, "//*[text()='" + " + Compare " + "']");

            NavigateCursor = driver.FindElement(By.XPath("/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/div/div[2]/div/div[2]/app-product-card-configurable/section/div[1]/a/img"));
            actions.MoveToElement(NavigateCursor).Build().Perform();

            helperTest.JsClickElement(driver, "//*[text()='" + " + Compare " + "']");

            NavigateCursor = driver.FindElement(By.XPath("/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/div/div[2]/div/div[3]/app-product-card-configurable/section/div[1]/a/img"));
            actions.MoveToElement(NavigateCursor).Build().Perform();

            helperTest.JsClickElement(driver, "//*[text()='" + " + Compare " + "']");

            NavigateCursor = driver.FindElement(By.XPath("/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/div/div[2]/div/div[4]/app-product-card-configurable/section/div[1]/a/img"));
            actions.MoveToElement(NavigateCursor).Build().Perform();

            helperTest.JsClickElement(driver, "//*[text()='" + " + Compare " + "']");

            helperTest.JsClickElement(driver, "//*[text()='" + " Compare products " + "']");

            Thread.Sleep(4000);

            Assert.AreEqual(mainURLs + "comparison", driver.Url);

            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;

            Assert.IsTrue(bodyTextProduct.Contains("RevoFit2 Kit"));
            Assert.IsTrue(bodyTextProduct.Contains("Medical RevoLock Lanyard Kit"));
            Assert.IsTrue(bodyTextProduct.Contains("Medical RevoLock Upper Extremity"));
            Assert.IsTrue(bodyTextProduct.Contains("Medical Replacement Parts"));

            Thread.Sleep(1000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-compare/section/section/div[1]/div/app-compare-item[3]/section/article/div[1]/i");

            Thread.Sleep(1000);

            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;

            Assert.IsFalse(bodyTextProduct.Contains("Medical RevoLock Upper Extremity"));

            Thread.Sleep(1000);
        }

        [Test]
        public void FilterAndClearAll()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            Assert.AreEqual(homeUrl, driver.Url);

            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));

            SearchBox.Clear();
            SearchBox.SendKeys("Liners");
            SearchBox.SendKeys(Keys.Enter);

            helperTest.waitElementId(driver, 60, "configurable_img_0");

            helperTest.JsClickElement(driver, "//*[text()='" + "L5679" + "']");

            Thread.Sleep(3000);

            helperTest.waitElementId(driver, 60, "label_filter_lcode_0");
            var isChecked = driver.FindElement(By.Id("filter_lcode_0")).Selected;

            Assert.IsTrue(isChecked);

            Thread.Sleep(5000);

            helperTest.JsClickElementId(driver, "clear-all");

            Thread.Sleep(2000);

            isChecked = driver.FindElement(By.Id("filter_lcode_0")).Selected;

            Assert.IsFalse(isChecked);

            Thread.Sleep(2000);
        }

        [Test]
        public void MostFrequentlyPurchased()
        {
            string bodyTextProduct;

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            Assert.AreEqual(homeUrl, driver.Url);

            driver.Url = mainURLs + "product?productID=15678";

            Thread.Sleep(1000);

            driver.Url.Contains("product?productID=15678");

            helperTest.waitElementId(driver, 60, "product_name_in_product_page");

            bodyTextProduct = driver.FindElement(By.Id("product_name_in_product_page")).Text;

            Assert.IsTrue(bodyTextProduct.Contains("OH5 Knee™"));

            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div/select", 2);

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/div[1]/div[2]/div[4]/div[1]/mdb-card/div/mdb-card-body/mdb-card-text/p/div/app-button/div/button");

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            Thread.Sleep(5000);

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/div[5]/div/div/div[2]/div/div/div/article[1]/div[5]/p/app-qty/input");

            string item1 = driver.FindElement(By.XPath("/html/body/app-root/div/app-product/div[5]/div/div/div[2]/div/div/div/article[1]/div[2]/p")).Text;
            string item2 = driver.FindElement(By.XPath("/html/body/app-root/div/app-product/div[5]/div/div/div[2]/div/div/div/article[4]/div[2]/p")).Text;
            helperTest.InputStringXpath(driver, "5", "/html/body/app-root/div/app-product/div[5]/div/div/div[2]/div/div/div/article[1]/div[5]/p/app-qty/input");
            helperTest.JsClickElementID(driver, "accessory_add_to_cart_0");

            Thread.Sleep(5000);

            helperTest.JsClickElementID(driver, "accessory_add_to_cart_3");

            Thread.Sleep(5000);

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div[1]/app-product/div[5]/div/div/div[1]/app-close-button/p/span");
            driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-product/div[5]/div/div/div[1]/app-close-button/p/span")).Click();                       

            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

            helperTest.waitElementId(driver, 60, "product-name-in-cart0");

            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;

            Assert.IsTrue(bodyTextProduct.Contains(item1));
            Assert.IsTrue(bodyTextProduct.Contains(item2));
            Assert.IsTrue(bodyTextProduct.Contains("OH5 Knee with Loop Adapter"));

            Thread.Sleep(1000);

            for (int j = 0; j < 3; j++)
            {
                helperTest.JsClickElementID(driver, "remove-button-0");

                Thread.Sleep(1000);
            }
        }

        [Test]
        public void Pagenation()
        {
            IWebElement Img;
            Boolean ImagePresent;

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);            

            Assert.AreEqual(homeUrl, driver.Url);

            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));

            SearchBox.Clear();
            SearchBox.SendKeys("Liners");
            SearchBox.SendKeys(Keys.Enter);

            helperTest.waitElementId(driver, 60, "configurable_img_0");

            for (int j = 1; j < 3; j++)
            {
                for (int i = 0; i < 25; i++)
                {

                    string ids = "configurable_img_" + i.ToString();

                    Img = driver.FindElement(By.Id(ids));


                    ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);
                    Assert.IsTrue(ImagePresent);
                }

                helperTest.JsClickElement(driver, "/html/body/app-root/div/app-category/div/div/div[2]/div/ngb-pagination/ul/li[10]/a");

                helperTest.waitElementId(driver, 60, "configurable_img_0");
            }
        }

        [Test]
        public void QuickOrderAndDeleteFromCart()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            IWebElement QuickOrderBtn = driver.FindElement(By.Id("toggleQuickOrder"));
            QuickOrderBtn.Click();
            Thread.Sleep(1000);

            IWebElement ele2 = driver.FindElement(By.XPath("/html/body/app-root/div/app-quick-order-pad/div[1]/div[3]/app-tag-button/span/span"));
            IJavaScriptExecutor executor2 = (IJavaScriptExecutor)driver;
            executor2.ExecuteScript("arguments[0].click();", ele2);
            executor2.ExecuteScript("arguments[0].click();", ele2);

            InputAndCheckAdd("3244", 0, "Profile Orthosis 2XL");
            InputAndCheckAdd("3245", 1, "Profile Orthosis 3XL");
            InputAndCheckAdd("1211", 2, "Splint SM Left");
            InputAndCheckAdd("1212", 3, "Splint MD Left");
            InputAndCheckAdd("3243", 4, "Profile Orthosis XL");
            InputAndCheckAdd("1213", 5, "Splint LG Left");
            InputAndCheckAdd("1213", 6, "Splint LG Left");
            InputAndCheckAdd("3241", 7, "Orthosis MD");
            InputAndCheckAdd("3239", 8, "Orthosis XS");
            InputAndCheckAdd("3231", 9, "Orthosis 10");

            Thread.Sleep(1000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

            helperTest.waitElementId(driver, 60, "product-name-in-cart0");

            Assert.AreEqual(mainURLs + "cart/index", driver.Url);

            String bodyTextCart2 = driver.FindElement(By.TagName("body")).Text;

            Assert.IsTrue(bodyTextCart2.Contains("Profile Orthosis 2XL"));
            Assert.IsTrue(bodyTextCart2.Contains("Profile Orthosis 3XL"));
            Assert.IsTrue(bodyTextCart2.Contains("Splint SM Left"));
            Assert.IsTrue(bodyTextCart2.Contains("Splint MD Left"));
            Assert.IsTrue(bodyTextCart2.Contains("Profile Orthosis XL"));
            Assert.IsTrue(bodyTextCart2.Contains("Splint LG Left"));
            Assert.IsTrue(bodyTextCart2.Contains("Splint LG Left"));
            Assert.IsTrue(bodyTextCart2.Contains("Orthosis MD"));
            Assert.IsTrue(bodyTextCart2.Contains("Orthosis XS"));
            Assert.IsTrue(bodyTextCart2.Contains("Orthosis 10"));

            for (int j = 0; j < 10; j++)
            {
                helperTest.JsClickElementID(driver, "remove-button-0");

                Thread.Sleep(1000);
            }
        }

        [Test]
        public void ReplacementParts()
        {
            string bodyTextProduct;

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);            

            Assert.AreEqual(homeUrl, driver.Url);

            driver.Url = mainURLs + "product?productID=11300";

            helperTest.waitElementId(driver, 60, "product_name_in_product_page");
            Assert.AreEqual(mainURLs + "product?productID=11300", driver.Url);

            bodyTextProduct = driver.FindElement(By.Id("product_name_in_product_page")).Text;

            Assert.IsTrue(bodyTextProduct.Contains("Alpha Basic Liner"));

            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div[1]/select", 2);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div[2]/select", 7);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div[3]/select", 3);

            Thread.Sleep(2000);

            helperTest.JsClickElement(driver, "//*[text()='" + "Replacement Parts" + "']");

            Thread.Sleep(3000);

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/div[3]/div/div/div[2]/div/div/section/div/h6[1]/input");
            IWebElement BasicPart = driver.FindElement(By.XPath("/html/body/app-root/div/app-product/div[3]/div/div/div[2]/div/div/section/div/h6[1]/input"));
            var PartNumber = BasicPart.GetAttribute("value");
            Assert.AreEqual(PartNumber, "ABKL-32-6");

            string item = driver.FindElement(By.XPath("/html/body/app-root/div/app-product/div[3]/div/div/div[2]/div/div/div/article[1]/div[2]/p")).Text;
            helperTest.InputStringXpath(driver, "5", "/html/body/app-root/div/app-product/div[3]/div/div/div[2]/div/div/div/article[1]/div[7]/p/app-qty/input");
            driver.FindElement(By.XPath("/html/body/app-root/div/app-product/div[3]/div/div/div[2]/div/div/div/article[1]/div[8]/app-button/div/button")).Click();

            Thread.Sleep(5000);

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div[1]/app-product/div[3]/div/div/div[1]/app-close-button/p/span");
            driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-product/div[3]/div/div/div[1]/app-close-button/p/span")).Click();

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div[1]/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span");
            driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span")).Click();

            Thread.Sleep(1000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div[1]/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span");

            Thread.Sleep(1000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

            helperTest.waitElementId(driver, 60, "product-name-in-cart0");

            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;

            Assert.IsTrue(bodyTextProduct.Contains(item));
            Assert.IsTrue(bodyTextProduct.Contains("Alpha Basic Large Lck Umbrella 32"));

            Thread.Sleep(1000);

            for (int j = 0; j < 2; j++)
            {
                helperTest.JsClickElementID(driver, "remove-button-0");

                Thread.Sleep(1000);
            }
        }

        [Test]
        public void SearchByHCPCS()
        {
            String bodyTextProduct;

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Assert.AreEqual(homeUrl, driver.Url);

            helperTest.UseDropDownIdByName(driver, "basic-addon1", "HCPCS");

            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));

            SearchBox.Clear();
            SearchBox.SendKeys("L5647");

            SearchBox.SendKeys(Keys.Enter);

            Thread.Sleep(5000);

            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;

            Assert.IsTrue(bodyTextProduct.Contains("Medium Air-Lock"));
            Assert.IsTrue(bodyTextProduct.Contains("Opti-Seal®"));
            Assert.IsTrue(bodyTextProduct.Contains("Alpha® Suction Pro"));
            Assert.IsTrue(bodyTextProduct.Contains("Air-Lock Lanyard"));

            SearchBox.Clear();
            SearchBox.SendKeys("L8417");

            SearchBox.SendKeys(Keys.Enter);

            Thread.Sleep(5000);

            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;

            Assert.IsTrue(bodyTextProduct.Contains("One Sleeve"));
            Assert.IsTrue(bodyTextProduct.Contains("One® Gel Sock"));
            Assert.IsTrue(bodyTextProduct.Contains("Skin Reliever Gel Sheath"));
        }

        [Test]
        public void SearchByPartName()
        {
            string bodyTextProduct;

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");
            Assert.AreEqual(homeUrl, driver.Url);

            helperTest.UseDropDownIdByName(driver, "basic-addon1", "Part Number");

            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.Clear();
            SearchBox.SendKeys("ALC-9460-E");

            helperTest.waitElementId(driver, 60, "add_to_cart_in_dropdown");
            helperTest.JsClickElementId(driver, "add_to_cart_in_dropdown");

            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("ALC-9460-E"));

            helperTest.UseDropDownIdByName(driver, "basic-addon1", "Part Number");

            helperTest.waitElementId(driver, 60, "search");
            SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.Clear();
            SearchBox.SendKeys("ALC-5067-E");

            helperTest.waitElementId(driver, 60, "add_to_cart_in_dropdown");

            Thread.Sleep(1000);

            helperTest.JsClickElementId(driver, "add_to_cart_in_dropdown");

            Thread.Sleep(4000);

            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("ALC-5067-E"));

            for (int j = 0; j < 2; j++)
            {
                helperTest.JsClickElementID(driver, "remove-button-0");

                Thread.Sleep(1000);
            }
        }

        [Test]
        public void SearchBySkuAndDescription()
        {
            string bodyTextProduct;

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Assert.AreEqual(homeUrl, driver.Url);

            driver.Url = mainURLs + "product?productID=11602";

            Assert.AreEqual(mainURLs + "product?productID=11602", driver.Url);

            helperTest.waitElementId(driver, 60, "product_name_in_product_page");

            bodyTextProduct = driver.FindElement(By.Id("product_name_in_product_page")).Text;

            Assert.IsTrue(bodyTextProduct.Contains("Danmar Soft Shell Helmet"));

            helperTest.InputStringXpath(driver, "9820-BLACK-XS", "/html/body/app-root/div/app-product/div[1]/div[3]/section/div/h6[1]/input");
            helperTest.waitElementId(driver, 60, "add_product_to_cart0");
            helperTest.JsClickElementID(driver, "add_product_to_cart0");

            Thread.Sleep(3000);

            driver.Url = mainURLs + "product?productID=11602";

            Assert.AreEqual(mainURLs + "product?productID=11602", driver.Url);

            helperTest.waitElementId(driver, 60, "product_name_in_product_page");

            bodyTextProduct = driver.FindElement(By.Id("product_name_in_product_page")).Text;

            Assert.IsTrue(bodyTextProduct.Contains("Danmar Soft Shell Helmet"));

            helperTest.InputStringXpath(driver, "Soft Shell Helmet Black XL", "/html/body/app-root/div/app-product/div[1]/div[3]/section/div/h6[2]/input");
            helperTest.waitElementId(driver, 60, "add_product_to_cart0");
            helperTest.JsClickElementID(driver, "add_product_to_cart0");

            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

            helperTest.waitElementId(driver, 60, "product-name-in-cart0");

            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;

            Assert.IsTrue(bodyTextProduct.Contains("9820-BLACK-XL"));
            Assert.IsTrue(bodyTextProduct.Contains("Soft Shell Helmet Black XS"));

            Thread.Sleep(2000);

            for (int j = 0; j < 2; j++)
            {
                helperTest.JsClickElementID(driver, "remove-button-0");

                Thread.Sleep(1000);
            }
        }

        [Test]
        public void SearchPopProduct()
        {
            String bodyTextProduct;

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            Thread.Sleep(4000);

            IWebElement SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.SendKeys("Knee");
            SearchBox.SendKeys(Keys.Enter);

            helperTest.waitElementId(driver, 60, "configurable_img_0");

            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;

            Assert.IsTrue(bodyTextProduct.Contains("Balance™ Knee"));
        }

        [Test]
        public void SearchProductsExtended()
        {
            Actions NavigateAction;
            IWebElement NavigateCusror;
            IWebElement SearchBox;
            string bodyTextProduct;

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");
            Assert.AreEqual(homeUrl, driver.Url);

            helperTest.waitElementId(driver, 60, "search");
            SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.Clear();
            SearchBox.SendKeys("ottobock WalkOn");

            Thread.Sleep(2000);

            helperTest.JsClickElement(driver, "//*[text()='" + "View all products from ottobock." + "']");

            helperTest.waitElementId(driver, 60, "configurable_img_0");
            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/div/app-search-result-info-panel[1]/div/div[2]/span[1]/span[2]/span")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Supplier: \"ottobock\""));

            if (bodyTextProduct.Contains("EBS-PRO Modular Knee"))
            {
                helperTest.JsClickElement(driver, "//*[text()='" + "EBS-PRO Modular Knee" + "']");
            }
            else
            {
                helperTest.JsClickElement(driver, "/html/body/app-root/div[1]/app-category/div/div/div[2]/div/ngb-pagination/ul/li[10]/a");
                helperTest.waitElementId(driver, 60, "configurable_img_0");
                bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("EBS-PRO Modular Knee"));

                helperTest.JsClickElement(driver, "//*[text()='" + "EBS-PRO Modular Knee" + "']");
            }

            helperTest.waitElementId(driver, 60, "product_name_in_product_page");
            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("EBS-PRO Modular Knee"));

            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div[1]/select", 3);

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            Thread.Sleep(2000);

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span");
            driver.FindElement(By.XPath("/html/body/app-root/div/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span")).Click();

            helperTest.JsClickElementId(driver, "search");
            driver.FindElement(By.XPath("/html/body/app-root/app-header/nav/div[1]/div[2]/app-search-panel/div/div[2]/div/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div[2]/div/p[2]/span[1]")).Click();

            Thread.Sleep(2000);

            helperTest.waitElementId(driver, 60, "product_name_in_product_page");
            bodyTextProduct = driver.FindElement(By.Id("product_name_in_product_page")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("ottobock WalkOn®"));

            helperTest.InputStringXpath(driver, "28U11=L45-48", "/html/body/app-root/div/app-product/div[1]/div[3]/section/div/h6[1]/input");
            helperTest.waitElementId(driver, 60, "add_product_to_cart0");
            helperTest.JsClickElementID(driver, "add_product_to_cart0");

            Thread.Sleep(2000);

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/div[5]/div/div/div[1]/app-close-button/p/span");
            driver.FindElement(By.XPath("/html/body/app-root/div/app-product/div[5]/div/div/div[1]/app-close-button/p/span")).Click();

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span");
            driver.FindElement(By.XPath("/html/body/app-root/div/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span")).Click();

            helperTest.JsClickElementId(driver, "search");

            Thread.Sleep(2000);

            NavigateAction = new Actions(driver);

            NavigateCusror = driver.FindElement(By.XPath("/html/body/app-root/app-header/nav/div[1]/div[2]/app-search-panel/div/div[2]/div/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div[2]/div/p[2]/span[2]"));
            NavigateAction.MoveToElement(NavigateCusror).Build().Perform();
            helperTest.JsClickElementID(driver, "product_name");

            Thread.Sleep(2000);

            helperTest.waitElementId(driver, 60, "product_name_in_product_page");
            bodyTextProduct = driver.FindElement(By.Id("product_name_in_product_page")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("ottobock WalkOn® Reaction"));

            helperTest.InputStringXpath(driver, "28U24=L36-39", "/html/body/app-root/div/app-product/div[1]/div[3]/section/div/h6[1]/input");
            helperTest.waitElementId(driver, 60, "add_product_to_cart0");
            helperTest.JsClickElementID(driver, "add_product_to_cart0");

            Thread.Sleep(2000);

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/div[5]/div/div/div[1]/app-close-button/p/span");
            driver.FindElement(By.XPath("/html/body/app-root/div/app-product/div[5]/div/div/div[1]/app-close-button/p/span")).Click();

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span");
            driver.FindElement(By.XPath("/html/body/app-root/div/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span")).Click();

            helperTest.JsClickElementId(driver, "search");

            Thread.Sleep(2000);

            NavigateAction = new Actions(driver);

            NavigateCusror = driver.FindElement(By.XPath("/html/body/app-root/app-header/nav/div[1]/div[2]/app-search-panel/div/div[2]/div/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div[2]/div/p[2]/span[1]"));
            NavigateAction.MoveToElement(NavigateCusror).Build().Perform();
            helperTest.JsClickElement(driver, "//*[text()='" + "View Product Details" + "']");

            Thread.Sleep(2000);

            helperTest.waitElementId(driver, 60, "product_name_in_product_page");
            bodyTextProduct = driver.FindElement(By.Id("product_name_in_product_page")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("ottobock WalkOn®"));

            helperTest.InputStringXpath(driver, "28U11=L39-42", "/html/body/app-root/div/app-product/div[1]/div[3]/section/div/h6[1]/input");
            helperTest.waitElementId(driver, 60, "add_product_to_cart0");
            helperTest.JsClickElementID(driver, "add_product_to_cart0");

            Thread.Sleep(2000);

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/div[5]/div/div/div[1]/app-close-button/p/span");
            driver.FindElement(By.XPath("/html/body/app-root/div/app-product/div[5]/div/div/div[1]/app-close-button/p/span")).Click();

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span");
            driver.FindElement(By.XPath("/html/body/app-root/div/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span")).Click();

            helperTest.JsClickElementId(driver, "search");

            Thread.Sleep(2000);

            NavigateAction = new Actions(driver);

            NavigateCusror = driver.FindElement(By.XPath("/html/body/app-root/app-header/nav/div[1]/div[2]/app-search-panel/div/div[2]/div/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div[2]/div/p[2]/span[3]"));
            NavigateAction.MoveToElement(NavigateCusror).Build().Perform();
            helperTest.JsClickElement(driver, "//*[text()='" + "View Similar Products" + "']");

            helperTest.waitElementId(driver, 60, "configurable_img_0");
            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/div/app-search-result-info-panel[1]/div/div[2]/span[1]/span[2]/span")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Search All: \"ottobock WalkOn® Flex\""));

            Thread.Sleep(2000);

            helperTest.JsClickElement(driver, "//*[text()='" + "ottobock WalkOn®" + "']");

            helperTest.InputStringXpath(driver, "28U11=L36-39", "/html/body/app-root/div/app-product/div[1]/div[3]/section/div/h6[1]/input");
            helperTest.waitElementId(driver, 60, "add_product_to_cart0");
            helperTest.JsClickElementID(driver, "add_product_to_cart0");

            Thread.Sleep(2000);

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/div[5]/div/div/div[1]/app-close-button/p/span");
            driver.FindElement(By.XPath("/html/body/app-root/div/app-product/div[5]/div/div/div[1]/app-close-button/p/span")).Click();

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span");
            driver.FindElement(By.XPath("/html/body/app-root/div/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span")).Click();

            helperTest.UseDropDownIdByName(driver, "basic-addon1", "Part Number");

            helperTest.waitElementId(driver, 60, "search");
            SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.Clear();
            SearchBox.SendKeys("1210");
            SearchBox.SendKeys(Keys.Enter);

            Thread.Sleep(2000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-category/div/div/div[2]/app-details/div[1]/div/table/tbody/tr[1]/td[7]/div/div[1]/app-button/div/button");

            Thread.Sleep(2000);

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-category/div/div/div[2]/app-details/div[1]/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span");
            driver.FindElement(By.XPath("/html/body/app-root/div/app-category/div/div/div[2]/app-details/div[1]/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span")).Click();

            helperTest.JsClickElementId(driver, "search");

            Thread.Sleep(2000);

            NavigateAction = new Actions(driver);

            NavigateCusror = driver.FindElement(By.XPath("/html/body/app-root/app-header/nav/div[1]/div[2]/app-search-panel/div/div[2]/div/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div/p[3]"));
            NavigateAction.MoveToElement(NavigateCusror).Build().Perform();
            helperTest.JsClickElement(driver, "/html/body/app-root/app-header/nav/div[1]/div[2]/app-search-panel/div/div[2]/div/form/div[1]/app-search-panel-dropdown/div[1]/div/div[2]/p");

            Thread.Sleep(2000);

            helperTest.waitElementId(driver, 60, "product_name_in_product_page");

            Thread.Sleep(1000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            Thread.Sleep(2000);

            string item1 = driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-product/app-preview-cart-panel/section/div/div[2]/article/div[1]/div/app-product-card[1]/article/div[2]/div[1]/p[1]")).Text;

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span");
            driver.FindElement(By.XPath("/html/body/app-root/div/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span")).Click();

            helperTest.UseDropDownIdByName(driver, "basic-addon1", "Part Number");

            helperTest.JsClickElementId(driver, "search");
            SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.Clear();
            SearchBox.SendKeys("1210");

            Thread.Sleep(2000);

            NavigateAction = new Actions(driver);

            NavigateCusror = driver.FindElement(By.XPath("/html/body/app-root/app-header/nav/div[1]/div[2]/app-search-panel/div/div[2]/div/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div/p[4]"));
            NavigateAction.MoveToElement(NavigateCusror).Build().Perform();

            Thread.Sleep(1000);
            string item2 = driver.FindElement(By.XPath("/html/body/app-root/app-header/nav/div[1]/div[2]/app-search-panel/div/div[2]/div/form/div[1]/app-search-panel-dropdown/div[1]/div/div[2]/p")).Text;

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            Thread.Sleep(2000);

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span");
            driver.FindElement(By.XPath("/html/body/app-root/div/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span")).Click();

            helperTest.JsClickElementId(driver, "search");

            Thread.Sleep(2000);

            NavigateAction = new Actions(driver);

            NavigateCusror = driver.FindElement(By.XPath("/html/body/app-root/app-header/nav/div[1]/div[2]/app-search-panel/div/div[2]/div/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div/p[5]"));
            NavigateAction.MoveToElement(NavigateCusror).Build().Perform();
            helperTest.JsClickElement(driver, "//*[text()='" + "View Product Details" + "']");

            Thread.Sleep(2000);

            helperTest.waitElementId(driver, 60, "product_name_in_product_page");

            Thread.Sleep(1000);

            string item3 = driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-product/div[1]/div[2]/div[3]/div[1]/p[1]")).Text;

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            Thread.Sleep(2000);

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span");
            driver.FindElement(By.XPath("/html/body/app-root/div/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span")).Click();

            helperTest.UseDropDownIdByName(driver, "basic-addon1", "Part Number");

            helperTest.JsClickElementId(driver, "search");
            SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.Clear();
            SearchBox.SendKeys("1210");

            Thread.Sleep(2000);

            NavigateAction = new Actions(driver);

            NavigateCusror = driver.FindElement(By.XPath("/html/body/app-root/app-header/nav/div[1]/div[2]/app-search-panel/div/div[2]/div/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div/p[6]"));
            NavigateAction.MoveToElement(NavigateCusror).Build().Perform();
            helperTest.JsClickElement(driver, "//*[text()='" + "View Similar Products" + "']");

            Thread.Sleep(2000);

            string item4 = driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-category/div/div/div[2]/app-details/div[1]/div/table/tbody/tr[1]/td[4]/a")).Text;

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-category/div/div/div[2]/app-details/div[1]/div/table/tbody/tr[1]/td[7]/div/div[1]/app-button/div/button");

            Thread.Sleep(2000);

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-category/div/div/div[2]/app-details/div[1]/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span");
            driver.FindElement(By.XPath("/html/body/app-root/div/app-category/div/div/div[2]/app-details/div[1]/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span")).Click();

            helperTest.UseDropDownIdByName(driver, "basic-addon1", "Search All");

            helperTest.waitElementId(driver, 60, "search");
            SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.Clear();
            SearchBox.SendKeys("L4360");

            Thread.Sleep(2000);

            helperTest.JsClickElement(driver, "//*[text()='" + "View all products with manufacturer’s suggested HCPCS [L4360]" + "']");

            Thread.Sleep(2000);

            helperTest.waitElementId(driver, 60, "configurable_img_0");
            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/div/app-search-result-info-panel[1]/div/div[2]/span[1]/span[2]/span")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Search All: \"L4360\""));

            helperTest.JsClickElement(driver, "//*[text()='" + "Aircast® AirSelect Standard" + "']");

            Thread.Sleep(2000);

            helperTest.waitElementId(driver, 60, "product_name_in_product_page");
            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Aircast® AirSelect Standard"));

            helperTest.InputStringXpath(driver, "01EF-S", "/html/body/app-root/div/app-product/div[1]/div[3]/section/div/h6[1]/input");
            helperTest.waitElementId(driver, 60, "add_product_to_cart0");
            helperTest.JsClickElementID(driver, "add_product_to_cart0");            

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div[1]/app-product/div[5]/div/div/div[1]/app-close-button/p/span");
            driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-product/div[5]/div/div/div[1]/app-close-button/p/span")).Click();

            Thread.Sleep(2000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

            helperTest.waitElementId(driver, 60, "product-name-in-cart0");

            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;

            Assert.IsTrue(bodyTextProduct.Contains("AIRSELECT STANDARD SM"));
            Assert.IsTrue(bodyTextProduct.Contains(item4));
            Assert.IsTrue(bodyTextProduct.Contains(item3));
            Assert.IsTrue(bodyTextProduct.Contains(item2));
            Assert.IsTrue(bodyTextProduct.Contains(item1));
            Assert.IsTrue(bodyTextProduct.Contains("Original Grace Plate"));
            Assert.IsTrue(bodyTextProduct.Contains("WalkOn AFO LT SM"));
            Assert.IsTrue(bodyTextProduct.Contains("WalkOn AFO LT MD"));
            Assert.IsTrue(bodyTextProduct.Contains("WalkOn Reaction AFO LT SM"));
            Assert.IsTrue(bodyTextProduct.Contains("WalkOn AFO LT XL"));
            Assert.IsTrue(bodyTextProduct.Contains("EBS-PRO Knee KD Lamination Anchor"));

            Thread.Sleep(1000);

            for (int j = 0; j < 11; j++)
            {
                helperTest.JsClickElementID(driver, "remove-button-0");

                Thread.Sleep(1000);
            }
        }

        public void InputAndCheckAdd(string productId, int num, string nameCheck)
        {
            string numInput = "input-" + num.ToString();
            IWebElement InpBox = driver.FindElement(By.Id(numInput));
            InpBox.Clear();
            InpBox.SendKeys(productId);

            Thread.Sleep(3000);

            driver.FindElement(By.Id(numInput)).SendKeys(Keys.Enter);

            Thread.Sleep(7000);

            String bodyText = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyText.Contains(nameCheck));
        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        [Test]
        public void ShipAddress()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            Thread.Sleep(4000);

            driver.Url = mainURLs + "account-info";

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-account-info/div[1]/div[5]/div/div/div[2]/div/div[1]/span[1]");

            Thread.Sleep(2000);

            String[] s1 = new String[4];

            s1[0] = driver.FindElement(By.XPath("/html/body/app-root/div/app-account-info/div[1]/div[5]/div/div/div[2]/div/div[1]/span[1]")).Text;
            s1[1] = driver.FindElement(By.XPath("/html/body/app-root/div/app-account-info/div[1]/div[5]/div/div/div[3]/div/div[1]/span[1]")).Text;
            s1[2] = driver.FindElement(By.XPath("/html/body/app-root/div/app-account-info/div[1]/div[5]/div/div/div[4]/div/div[1]/span[1]")).Text;

            driver.Url = mainURLs + "cart/index";

            if (driver.Url.Contains("v2dev.cascade-usa"))
            {
                helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div[1]/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-common/section/aside/app-order-info-aside/aside/div[1]/div[2]/div[1]/app-tag-button/span/span");
            }
            else
            {
                helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-common/section/aside/app-order-info-aside/aside/div[1]/div[2]/div[1]/app-tag-button/span/span");
            }

            helperTest.JsClickElement(driver, "//*[text()='" + "Edit" + "']");

            if (driver.Url.Contains("v2dev.cascade-usa"))
            {
                driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-common/section/aside/app-order-info-aside/aside/div[1]/div/select")).Click();
            }
            else
            {
                driver.FindElement(By.XPath("/html/body/app-root/div/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-common/section/aside/app-order-info-aside/aside/div[1]/div/select")).Click();
            }

            String bodyText = driver.FindElement(By.TagName("body")).Text;
            for (int i = 0; i < 3; i++)
            {
                Assert.IsTrue(bodyText.Contains(s1[i]));
            }

            driver.Url = mainURLs + "account-info";

            Thread.Sleep(4000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-account-info/div[1]/div[5]/div/div/div[3]/div/div[2]/div[2]/label");
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-account-info/div[1]/div[5]/div/div/div[4]/div/div[2]/div[2]/label");

            driver.Url = mainURLs + "cart/index";

            if (driver.Url.Contains("v2dev.cascade-usa"))
            {
                helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div[1]/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-common/section/aside/app-order-info-aside/aside/div[1]/div[2]/div[1]/app-tag-button/span/span");
            }
            else
            {
                helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-common/section/aside/app-order-info-aside/aside/div[1]/div[2]/div[1]/app-tag-button/span/span");
            }

            helperTest.JsClickElement(driver, "//*[text()='" + "Edit" + "']");

            if (driver.Url.Contains("v2dev.cascade-usa"))
            {
                driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-common/section/aside/app-order-info-aside/aside/div[1]/div/select")).Click();
            }
            else
            {
                driver.FindElement(By.XPath("/html/body/app-root/div/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-common/section/aside/app-order-info-aside/aside/div[1]/div/select")).Click();
            }

            bodyText = driver.FindElement(By.TagName("body")).Text;

            Assert.IsFalse(bodyText.Contains(s1[2]));

            driver.Url = mainURLs + "account-info";

            Thread.Sleep(4000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-account-info/div[1]/div[5]/div/div/div[3]/div/div[2]/div[2]/label");
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-account-info/div[1]/div[5]/div/div/div[4]/div/div[2]/div[2]/label");

            Thread.Sleep(2000);
        }

        //[Test]
        public void ShipNewAddress()
        {
            String bodyTextProduct;

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");
            IWebElement QuickOrderBtn = driver.FindElement(By.Id("toggleQuickOrder"));
            QuickOrderBtn.Click();

            helperTest.waitElementId(driver, 60, "input-0");
            Assert.AreEqual(mainURLs + "quick-order", driver.Url);

            IWebElement AddItem = driver.FindElement(By.Id("input-0"));

            AddItem.Clear();
            AddItem.SendKeys("1210");
            AddItem.SendKeys(Keys.Enter);

            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("1210"));

            Thread.Sleep(1000);

            helperTest.JsClickElement(driver, "//*[text()='" + "Edit" + "']");

            Thread.Sleep(1000);

            helperTest.JsClickElement(driver, "//*[text()='" + "New Address" + "']");

            helperTest.waitElementId(driver, 60, "ShipToName");
            helperTest.InputStringId(driver, "Test", "ShipToName");
            helperTest.InputStringId(driver, "Test Address 1", "AddressOne");
            helperTest.InputStringId(driver, "Test Address 2", "AddressTwo");
            helperTest.InputStringId(driver, "Test City", "City");
            if (driver.Url.Contains("v2dev.cascade-usa"))
            {
                helperTest.UseDropDown(driver, "/html/body/app-root/div[1]/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-common/section/aside/app-order-info-aside/div/div/div/div[2]/div/form/select[1]", 2);
            }
            else
            {
                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-common/section/aside/app-order-info-aside/div/div/div/div[2]/div/form/select[1]", 2);
            }
            helperTest.InputStringId(driver, "11111", "ZipCode");
            helperTest.InputStringId(driver, "22222", "Phone");

            helperTest.JsClickElement(driver, "//*[text()='" + " Use This Address " + "']");

            Thread.Sleep(2000);

            Assert.AreEqual(mainURLs + "cart/index", driver.Url);

            IWebElement InpBox = driver.FindElement(By.Id("poNumber"));

            InpBox.Clear();
            InpBox.SendKeys("TESTPO " + DateTime.Now.ToString("yyyyMMdd"));

            IWebElement InpBox2 = driver.FindElement(By.Id("notesInput"));
            InpBox2.Clear();
            InpBox2.SendKeys("TEST ORDER PLEASE DO NOT PROCESS " + DateTime.Now.ToString("yyyyMMdd"));

            Thread.Sleep(1000);

            helperTest.JsClickElementId(driver, "submit_order");

            helperTest.waitElementId(driver, 180, "product-name-in-cartundefined");

            Assert.AreEqual(mainURLs + "cart/review", driver.Url);

            IWebElement ShipToIdBox = driver.FindElement(By.Id("shipToId"));
            var ShipToId = Convert.ToInt32(ShipToIdBox.GetAttribute("value"));
            Assert.AreNotEqual(ShipToId, 0);

            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-cart-root/div/div/app-review-cart/section/aside/app-order-info-aside/aside/div[3]/div[3]/app-button/div/button");

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-cart-root/div/div/app-review-cart/div[1]/div/div/div[3]/app-button/div/button");

            helperTest.FindTextInBody(driver, "Thank you for your order. Your order number is");

            Thread.Sleep(4000);

            helperTest.JsClickElement(driver, "//*[text()='" + " OK " + "']");

            Thread.Sleep(2000);
        }

        [Test]
        public void ShoppingList()
        {
            string bodyTextCart;

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            Thread.Sleep(4000);
            Assert.AreEqual(homeUrl, driver.Url);
            IWebElement ClickUser = driver.FindElement(By.Id("username_button"));


            Actions actions = new Actions(driver);
            actions.MoveToElement(ClickUser).Build().Perform();

            helperTest.JsClickElement(driver, "//*[text()='" + "Shopping Lists" + "']");

            Thread.Sleep(4000);

            Assert.AreEqual(mainURLs + "shopping/list", driver.Url);

            helperTest.JsClickElement(driver, "//*[text()='" + "Create new list" + "']");

            helperTest.InputStringXpath(driver, "list1", "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/app-create-list-modal/div/div/div[2]/div[1]/div[2]/input");
            helperTest.InputStringXpath(driver, "description of list 1", "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/app-create-list-modal/div/div/div[2]/div[2]/div[2]/textarea");

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/app-create-list-modal/div/div/div[3]/button");

            Thread.Sleep(4000);

            helperTest.JsClickElement(driver, "//*[text()='" + "Create new list" + "']");

            helperTest.InputStringXpath(driver, "list2", "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/app-create-list-modal/div/div/div[2]/div[1]/div[2]/input");
            helperTest.InputStringXpath(driver, "description of list 2", "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/app-create-list-modal/div/div/div[2]/div[2]/div[2]/textarea");

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/app-create-list-modal/div/div/div[3]/button");

            driver.Url = mainURLs + "product?productID=7254";

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[4]/div[2]/div/span[1]/div/div/span");

            helperTest.JsClickElement(driver, "//*[text()='" + "list1" + "']");

            Thread.Sleep(4000);

            driver.Url = mainURLs + "product?productID=15048";

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[4]/div[2]/div/span[1]/div/div/span");

            helperTest.JsClickElement(driver, "//*[text()='" + "list1" + "']");

            Thread.Sleep(4000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/app-added-to-list-modal/div/div/div[3]/button[1]");

            Thread.Sleep(8000);

            bodyTextCart = driver.FindElement(By.TagName("body")).Text;

            Assert.IsTrue(bodyTextCart.Contains("112-10"));

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[2]/div/div[2]/div[1]/app-my-current-list/section/div[3]/div[3]/app-tag-button/span/span");

            bodyTextCart = driver.FindElement(By.TagName("body")).Text;

            Assert.IsFalse(bodyTextCart.Contains("62471-AM"));

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[2]/div/div[2]/div[1]/app-my-current-list/section/div[3]/div[2]/app-button/div/button");

            Thread.Sleep(8000);

            driver.Url = mainURLs + "product?productID=15048";

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[4]/div[2]/div/span[1]/div/div/span");

            helperTest.JsClickElement(driver, "//*[text()='" + "list2" + "']");

            Thread.Sleep(4000);

            driver.Url = mainURLs + "product?productID=7254";

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[4]/div[2]/div/span[1]/div/div/span");

            helperTest.JsClickElement(driver, "//*[text()='" + "list2" + "']");

            Thread.Sleep(4000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/app-added-to-list-modal/div/div/div[3]/button[1]");

            Thread.Sleep(8000);

            bodyTextCart = driver.FindElement(By.TagName("body")).Text;

            Assert.IsTrue(bodyTextCart.Contains("62471-AM"));

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[2]/div/div[2]/div[1]/app-my-current-list/section/div[3]/div[3]/app-tag-button/span/span");

            bodyTextCart = driver.FindElement(By.TagName("body")).Text;

            Assert.IsFalse(bodyTextCart.Contains("112-10"));

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[2]/div/div[2]/div[1]/app-my-current-list/section/div[3]/div[2]/app-button/div/button");

            Thread.Sleep(2000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextCart = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextCart.Contains("62471-AM"));
            helperTest.waitElementId(driver, 60, "item-name-in-cart1");
            bodyTextCart = driver.FindElement(By.Id("item-name-in-cart1")).Text;
            Assert.IsTrue(bodyTextCart.Contains("112-10"));

            helperTest.waitElementId(driver, 60, "item-name-in-cart0");

            for (int j = 0; j < 2; j++)
            {
                helperTest.JsClickElementID(driver, "remove-button-0");

                Thread.Sleep(1000);
            }

            IWebElement ClickUser2 = driver.FindElement(By.Id("username_button"));
            Actions acti = new Actions(driver);
            acti.MoveToElement(ClickUser2).Build().Perform();

            helperTest.JsClickElement(driver, "//*[text()='" + "Shopping Lists" + "']");

            Thread.Sleep(4000);

            IWebElement RemoveList1 = driver.FindElement(By.XPath("/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/mdb-card/div/mdb-card-body/mdb-card-text/p/div/div/span[2]/i"));
            Actions RemoveTheList1 = new Actions(driver);
            RemoveTheList1.MoveToElement(RemoveList1).Build().Perform();
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/mdb-card/div/mdb-card-body/mdb-card-text/p/div/div/span[2]/i");
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/div/div/div/div[2]/app-button[2]/div/button");

            IWebElement RemoveList2 = driver.FindElement(By.XPath("/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/mdb-card/div/mdb-card-body/mdb-card-text/p/div/div/span[1]/i"));
            Actions RemoveTheList2 = new Actions(driver);
            RemoveTheList2.MoveToElement(RemoveList2).Build().Perform();
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/mdb-card/div/mdb-card-body/mdb-card-text/p/div/div/span[1]/i");
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/div/div/div/div[2]/app-button[2]/div/button");

            Thread.Sleep(2000);
        }

        [Test]
        public void ShoppingListExtended()
        {
            Actions NavigateAction;
            IWebElement NavigateCusror;
            IWebElement SearchBox;
            string bodyTextProduct;

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            Thread.Sleep(4000);
            Assert.AreEqual(homeUrl, driver.Url);
            IWebElement ClickUser = driver.FindElement(By.Id("username_button"));

            Actions actions = new Actions(driver);
            actions.MoveToElement(ClickUser).Build().Perform();

            helperTest.JsClickElement(driver, "//*[text()='" + "Shopping Lists" + "']");

            Thread.Sleep(4000);

            Assert.AreEqual(mainURLs + "shopping/list", driver.Url);

            helperTest.JsClickElement(driver, "//*[text()='" + "Create new list" + "']");

            helperTest.InputStringXpath(driver, "test list", "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/app-create-list-modal/div/div/div[2]/div[1]/div[2]/input");
            helperTest.InputStringXpath(driver, "test shopping list", "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/app-create-list-modal/div/div/div[2]/div[2]/div[2]/textarea");

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/app-create-list-modal/div/div/div[3]/button");

            Thread.Sleep(4000);

            helperTest.waitElementId(driver, 60, "search");
            SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.Clear();
            SearchBox.SendKeys("knee");

            Thread.Sleep(4000);

            NavigateAction = new Actions(driver);

            NavigateCusror = driver.FindElement(By.XPath("/html/body/app-root/app-header/nav/div[1]/div[2]/app-search-panel/div/div[2]/div/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div[2]/div/p[2]/span[4]"));
            NavigateAction.MoveToElement(NavigateCusror).Build().Perform();
            string item1 = driver.FindElement(By.Id("product_name")).Text;
            helperTest.JsClickElement(driver, "/html/body/app-root/app-header/nav/div[1]/div[2]/app-search-panel/div/div[2]/div/form/div[1]/app-search-panel-dropdown/div[1]/div/div[2]/div[1]/div/div/div/span");
            helperTest.JsClickElement(driver, "//*[text()='" + "test list" + "']");

            Thread.Sleep(2000);

            helperTest.JsClickElement(driver, "//*[text()='" + "Continue shopping" + "']");

            Thread.Sleep(2000);

            helperTest.waitElementId(driver, 60, "search");
            SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.Clear();
            SearchBox.SendKeys("sensor knee");
            SearchBox.SendKeys(Keys.Enter);

            NavigateAction = new Actions(driver);

            helperTest.waitElementId(driver, 60, "configurable_img_0");
            NavigateCusror = driver.FindElement(By.Id("configurable_img_0"));
            NavigateAction.MoveToElement(NavigateCusror).Build().Perform();
            helperTest.JsClickElement(driver, "//*[text()='" + " Add to List " + "']");
            helperTest.JsClickElement(driver, "//*[text()='" + "test list" + "']");

            Thread.Sleep(2000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/app-added-to-list-modal/div/div/div[3]/button[2]");

            Thread.Sleep(2000);

            driver.Navigate().Refresh();

            helperTest.waitElementId(driver, 60, "search");
            SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.Clear();
            SearchBox.SendKeys("ProCarve Knee");
            SearchBox.SendKeys(Keys.Enter);

            NavigateAction = new Actions(driver);

            helperTest.waitElementId(driver, 60, "configurable_img_0");
            NavigateCusror = driver.FindElement(By.Id("configurable_img_0"));
            NavigateAction.MoveToElement(NavigateCusror).Build().Perform();

            Thread.Sleep(2000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Preview " + "']");

            Thread.Sleep(2000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/app-preview-details-panel/section/div/div[2]/mdb-card/div/mdb-card-body/mdb-card-text/p/div/div[2]/div/span");
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/app-preview-details-panel/section/div/div[2]/mdb-card/div/mdb-card-body/mdb-card-text/p/div/div[2]/div/div/span");

            Thread.Sleep(2000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/app-preview-details-panel/app-added-to-list-modal/div/div/div[3]/button[2]");
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/app-preview-details-panel/section/div/div[1]/app-close-button/p/span");

            Thread.Sleep(2000);

            helperTest.waitElementId(driver, 60, "search");
            SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.Clear();
            SearchBox.SendKeys("OH5 Knee");
            SearchBox.SendKeys(Keys.Enter);

            helperTest.waitElementId(driver, 60, "configurable_img_0");
            helperTest.JsClickElementID(driver, "configurable_img_0");

            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div[1]/select", 2);
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[4]/div[2]/div/span[1]/div/div/span");
            helperTest.JsClickElement(driver, "//*[text()='" + "test list" + "']");
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/app-added-to-list-modal/div/div/div[3]/button[2]");

            Thread.Sleep(2000);

            helperTest.UseDropDownIdByName(driver, "basic-addon1", "Part Number");

            helperTest.waitElementId(driver, 60, "search");
            SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.Clear();
            SearchBox.SendKeys("1210");

            Thread.Sleep(2000);

            NavigateAction = new Actions(driver);

            NavigateCusror = driver.FindElement(By.XPath("/html/body/app-root/app-header/nav/div[1]/div[2]/app-search-panel/div/div[2]/div/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div/p[2]"));
            NavigateAction.MoveToElement(NavigateCusror).Build().Perform();
            string item2 = driver.FindElement(By.XPath("/html/body/app-root/app-header/nav/div[1]/div[2]/app-search-panel/div/div[2]/div/form/div[1]/app-search-panel-dropdown/div[1]/div/div[2]/p")).Text;
            helperTest.JsClickElement(driver, "/html/body/app-root/app-header/nav/div[1]/div[2]/app-search-panel/div/div[2]/div/form/div[1]/app-search-panel-dropdown/div[1]/div/div[2]/div[1]/div/div/span");
            helperTest.JsClickElement(driver, "//*[text()='" + "test list" + "']");
            helperTest.JsClickElement(driver, "/html/body/app-root/app-added-to-list-modal/div/div/div[3]/button[2]");

            Thread.Sleep(2000);

            helperTest.UseDropDownIdByName(driver, "basic-addon1", "Part Number");

            helperTest.waitElementId(driver, 60, "search");
            SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.Clear();
            SearchBox.SendKeys("1210");
            SearchBox.SendKeys(Keys.Enter);

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div[1]/app-category/div/div/div[2]/app-details/div[1]/div/table/tbody/tr[1]/td[1]/a/app-tooltip-image/div/img");

            Thread.Sleep(1000);

            string item3 = driver.FindElement(By.XPath("/html/body/app-root/div/app-category/div/div/div[2]/app-details/div[1]/div/table/tbody/tr[2]/td[3]/a")).Text;
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-category/div/div/div[2]/app-details/div[1]/div/table/tbody/tr[2]/td[7]/div/div[2]/div/span");
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-category/div/div/div[2]/app-details/div[1]/div/table/tbody/tr[2]/td[7]/div/div[2]/div/div/span");
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-category/div/div/div[2]/app-details/app-added-to-list-modal/div/div/div[3]/button[2]");

            Thread.Sleep(2000);

            helperTest.UseDropDownIdByName(driver, "basic-addon1", "Search All");

            helperTest.waitElementId(driver, 60, "search");
            SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.Clear();
            SearchBox.SendKeys("Aqua Knee");
            SearchBox.SendKeys(Keys.Enter);

            helperTest.waitElementId(driver, 60, "configurable_img_0");
            helperTest.JsClickElementID(driver, "configurable_img_0");

            Thread.Sleep(2000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            Thread.Sleep(2000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

            helperTest.waitElementId(driver, 60, "product-name-in-cart0");
            helperTest.JsClickElement(driver, "/html/body/app-root/div[1]/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-common/section/section/article/div[2]/app-cart-product-order/section/div/article[4]/div[3]/div/div/span");
            helperTest.JsClickElement(driver, "//*[text()='" + "test list" + "']");
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-common/app-added-to-list-modal/div/div/div[3]/button[2]");

            helperTest.JsClickElementID(driver, "remove-button-0");

            driver.Url = mainURLs + "shopping/order-history";

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div[1]/app-main/div/app-order-history/section/section/app-history-order-item[1]/article/article/section");

            string item4 = driver.FindElement(By.XPath("/html/body/app-root/div/app-main/div/app-order-history/section/section/app-history-order-item[1]/article/article/section/div[2]/section[1]/div[1]/div/div[2]/p[3]/span")).Text;

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-order-history/section/section/app-history-order-item[1]/article/article/section/div[2]/section[1]/div[2]/div[3]/div/span");
            helperTest.JsClickElement(driver, "//*[text()='" + "test list" + "']");
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-order-history/app-added-to-list-modal/div/div/div[3]/button[1]");

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div[1]/app-main/div/app-shopping-list/div/div/div[2]/div/div[2]");

            Thread.Sleep(2000);

            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;

            Assert.IsTrue(bodyTextProduct.Contains(item1));
            Assert.IsTrue(bodyTextProduct.Contains("Sensor Knee"));
            Assert.IsTrue(bodyTextProduct.Contains("ProCarve Knee"));
            Assert.IsTrue(bodyTextProduct.Contains("OH5 Knee"));
            Assert.IsTrue(bodyTextProduct.Contains(item2));
            Assert.IsTrue(bodyTextProduct.Contains(item3));
            Assert.IsTrue(bodyTextProduct.Contains("Aqua Knee"));
            Assert.IsTrue(bodyTextProduct.Contains(item4));

            IWebElement RemoveList1 = driver.FindElement(By.XPath("/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/mdb-card/div/mdb-card-body/mdb-card-text/p/div/div/span/i"));
            Actions RemoveTheList1 = new Actions(driver);
            RemoveTheList1.MoveToElement(RemoveList1).Build().Perform();
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/mdb-card/div/mdb-card-body/mdb-card-text/p/div/div/span/i");
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/div/div/div/div[2]/app-button[2]/div/button");

            Thread.Sleep(2000);
        }

        [Test]
        public void Step13AddToCart()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);
            
            String bodyTextProduct;

            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));

            SearchBox.Clear();
            SearchBox.SendKeys("60SL");
            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "/html/body/app-root/app-header/nav/div[1]/div[2]/app-search-panel/div/div[2]/div/form/div[1]/app-search-panel-dropdown/div[1]/div/div[2]/p[1]");

            Thread.Sleep(5000);

            driver.Url.Contains("product?productID=6646");

            helperTest.InputStringXpath(driver, "60SL", "/html/body/app-root/div/app-product/div[1]/div[3]/section/div/h6[1]/input");

            helperTest.InputStringXpath(driver, "5", "/html/body/app-root/div[1]/app-product/div[1]/div[3]/div[1]/article[1]/div[5]/p/input");
            helperTest.JsClickElementId(driver, "add_product_to_cart0");            

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div[1]/app-product/div[5]/div/div/div[1]/app-close-button/p/span");
            driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-product/div[5]/div/div/div[1]/app-close-button/p/span")).Click();

            Thread.Sleep(4000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("60SL"));

            Thread.Sleep(2000);

            helperTest.JsClickElementID(driver, "remove-button-0");
        }        

        [Test]
        public void Step17AddToCart()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Actions actions = new Actions(driver);
            IWebElement CartBtn;
            String bodyTextProduct;
            IWebElement NavigateCusror;

            //search text
            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.Clear();
            SearchBox.SendKeys("993740");

            Thread.Sleep(3000);

            Thread.Sleep(3000);
            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/app-header/nav/div[1]/div[2]/app-search-panel/div/div[2]/div/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div[2]/div/p[2]/span");

            NavigateCusror = driver.FindElement(By.XPath("/html/body/app-root/app-header/nav/div[1]/div[2]/app-search-panel/div/div[2]/div/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div[2]/div/p[2]/span"));
            actions.MoveToElement(NavigateCusror).Build().Perform();

            helperTest.InputStringId(driver, "2", "qty_in_dropdown");

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-home/div/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span");
            // go to cart
            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();
            // check sku
            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("993740"));            

            Thread.Sleep(2000);

            helperTest.JsClickElementID(driver, "remove-button-0");
        }


        //[Test]
        public void SubmitOrder()
        {
            IWebElement InpBox;

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Assert.AreEqual(homeUrl, driver.Url);

            driver.Url = mainURLs + "product?productID=1528";

            Assert.AreEqual(mainURLs + "product?productID=1528", driver.Url);

            helperTest.waitElementId(driver, 60, "product_name_in_product_page");

            String bodyTextProduct = driver.FindElement(By.Id("product_name_in_product_page")).Text;

            Assert.IsTrue(bodyTextProduct.Contains("Attachment Kits 650 AK"));

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            Thread.Sleep(5000);

            helperTest.JsClickElementId(driver, "header_cart_icon");

            helperTest.waitElementId(driver, 60, "item-name-in-cart0");

            Assert.AreEqual(mainURLs + "cart/index", driver.Url);

            InpBox = driver.FindElement(By.Id("poNumber"));
            InpBox.Clear();
            InpBox.SendKeys("TESTPO " + DateTime.Now.ToString("yyyyMMdd"));

            InpBox = driver.FindElement(By.Id("notesInput"));
            InpBox.Clear();
            InpBox.SendKeys("TEST ORDER PLEASE DO NOT PROCESS " + DateTime.Now.ToString("yyyyMMdd"));

            Thread.Sleep(1000);

            helperTest.JsClickElementId(driver, "submit_order");

            helperTest.waitElementId(driver, 180, "product-name-in-cartundefined");

            Assert.AreEqual(mainURLs + "cart/review", driver.Url);

            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-cart-root/div/div/app-review-cart/section/aside/app-order-info-aside/aside/div[3]/div[3]/app-button/div/button");

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-cart-root/div/div/app-review-cart/div[1]/div/div/div[3]/app-button/div/button");

            helperTest.FindTextInBody(driver, "Thank you for your order. Your order number is");

            Thread.Sleep(4000);

            helperTest.JsClickElement(driver, "//*[text()='" + " OK " + "']");

            Thread.Sleep(2000);
        }

        //[Test]
        public void SubmitRMAs()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Assert.AreEqual(homeUrl, driver.Url);

            driver.Url = mainURLs + "shopping/order-history?tab=orders&page=1";

            Thread.Sleep(1000);

            Assert.AreEqual(mainURLs + "shopping/order-history?tab=orders&page=1", driver.Url);

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-order-history/section/section/app-history-order-item[1]/article/article/section/div[2]/section[1]/div[2]/div[2]/div/app-button/div/button");

            helperTest.InputStringId(driver, "123456", "rma_patientID");
            helperTest.InputStringId(driver, "1234567890", "rma_serialNumbers");

            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-main/div/app-order-history/app-rma-modal/section/div/div[2]/div[1]/section/form/div[4]/div[2]/select", 2);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-main/div/app-order-history/app-rma-modal/section/div/div[2]/div[1]/section/form/div[5]/div[2]/select", 3);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-main/div/app-order-history/app-rma-modal/section/div/div[2]/div[1]/section/form/div[6]/div[2]/select", 5);

            helperTest.InputStringXpath(driver, "Broken", "/html/body/app-root/div/app-main/div/app-order-history/app-rma-modal/section/div/div[2]/div[1]/section/form/div[7]/div[2]/textarea");

            IWebElement InpBox2 = driver.FindElement(By.Id("rma_notes"));

            InpBox2.Clear();
            InpBox2.SendKeys("TEST RMA PLEASE DO NOT PROCESS " + DateTime.Now.ToString("yyyyMMdd"));

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-order-history/app-rma-modal/section/div/div[2]/div[2]/div[2]/button");

            Thread.Sleep(5000);

            helperTest.FindTextInBody(driver, "Success");

            helperTest.JsClickElement(driver, "//*[text()='" + " Submit for Return " + "']");

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-main/div/app-order-history/div[5]/div/div/div[3]/app-button/div/button");

            helperTest.FindTextInBody(driver, "Thank you for your submission");
        }

        //[Test]
        public void SubmitOrderExtended()
        {
            string bodyTextProduct;
            IWebElement SearchBox;

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");
            Assert.AreEqual(homeUrl, driver.Url);

            var useTheCode = true;
            if (useTheCode)
            {
                //Patient 1

                SearchBox = driver.FindElement(By.Id("search"));
                SearchBox.SendKeys("tiso 464");
                SearchBox.SendKeys(Keys.Enter);

                helperTest.waitElementId(driver, 60, "configurable_img_0");

                Thread.Sleep(2000);

                helperTest.JsClickElement(driver, "//*[text()='" + "Vista® 464 TLSO" + "']");

                helperTest.waitElementId(driver, 60, "product_name_in_product_page");
                bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("Vista® 464 TLSO"));

                helperTest.InputStringId(driver, "2", "qty_product_page");

                helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

                helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/div[5]/div/div/div[2]/div/div/div/article[1]/div[5]/p/app-qty/input");

                helperTest.InputStringXpath(driver, "5", "/html/body/app-root/div/app-product/div[5]/div/div/div[2]/div/div/div/article[2]/div[5]/p/app-qty/input");

                driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span")).Click();

                driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-product/div[5]/div/div/div[2]/div/div/div/article[2]/div[6]/app-button/div/button")).Click();

                Thread.Sleep(3000);

                helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

                helperTest.waitElementId(driver, 60, "product-name-in-cart0");

                bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("Therapy Pack Hot/Cold"));
                Assert.IsTrue(bodyTextProduct.Contains("Vista 464 TLSO Adjustable"));

                helperTest.InputStringId(driver, "Patient 1", "patient_id_in_cart0");
                helperTest.InputStringId(driver, "test notes for item 990005", "notes_in_cart0");
                helperTest.InputStringId(driver, "Patient 1", "patient_id_in_cart1");
                helperTest.InputStringId(driver, "test notes for item 993640", "notes_in_cart1");

                Thread.Sleep(1000);

                //Patient 2

                SearchBox = driver.FindElement(By.Id("search"));
                SearchBox.SendKeys("alpha classic");
                SearchBox.SendKeys(Keys.Enter);

                helperTest.waitElementId(driver, 60, "configurable_img_0");

                Thread.Sleep(2000);

                helperTest.JsClickElement(driver, "//*[text()='" + "Alpha Classic® Liners" + "']");

                helperTest.waitElementId(driver, 60, "product_name_in_product_page");
                bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("Alpha Classic® Liners"));

                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div[1]/select", 2);
                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div[2]/select", 5);
                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div[3]/select", 4);
                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div[4]/select", 3);
                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div[5]/select", 3);
                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div[6]/select", 3);
                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div[7]/select", 3);

                helperTest.InputStringId(driver, "2", "qty_product_page");

                helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

                Thread.Sleep(3000);

                helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

                helperTest.waitElementId(driver, 60, "product-name-in-cart0");

                bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("Alpha Classic Lock Uni Sprt G/G 6mm LG"));

                helperTest.InputStringId(driver, "Patient 2", "patient_id_in_cart0");
                helperTest.InputStringId(driver, "test notes for item ALL-5366-E", "notes_in_cart0");

                Thread.Sleep(1000);

                //Patient 3

                helperTest.JsClickElementID(driver, "product-name-in-cart0");

                driver.Url.Contains("product?productID=1483");

                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div[1]/select", 3);
                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div[2]/select", 5);
                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div[3]/select", 4);
                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div[4]/select", 3);
                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div[5]/select", 3);
                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div[6]/select", 3);
                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div[7]/select", 4);

                helperTest.InputStringId(driver, "2", "qty_product_page");

                helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

                Thread.Sleep(3000);

                helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

                helperTest.waitElementId(driver, 60, "product-name-in-cart0");

                bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("Alpha Classic Cush Uni Sprt G/G 6mm LG"));

                helperTest.InputStringId(driver, "Patient 3", "patient_id_in_cart0");
                helperTest.InputStringId(driver, "test notes for item ALC-5366-E", "notes_in_cart0");

                Thread.Sleep(1000);

                //Patient 4

                SearchBox = driver.FindElement(By.Id("search"));
                SearchBox.SendKeys("alpha hybrid");
                SearchBox.SendKeys(Keys.Enter);

                helperTest.waitElementId(driver, 60, "configurable_img_1");
                helperTest.JsClickElementID(driver, "configurable_img_1");

                helperTest.waitElementId(driver, 60, "product_name_in_product_page");
                bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("Alpha Hybrid® Liners"));

                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div[1]/select", 2);
                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div[2]/select", 3);
                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div[3]/select", 2);
                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div[4]/select", 2);

                helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

                Thread.Sleep(3000);

                helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

                helperTest.waitElementId(driver, 60, "product-name-in-cart0");

                bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("Alpha Hybrid Locking Prog MD Acc Umb"));

                helperTest.InputStringId(driver, "Patient 4", "patient_id_in_cart0");
                helperTest.InputStringId(driver, "test notes for item H352-6390", "notes_in_cart0");

                Thread.Sleep(1000);

                driver.Url = homeUrl;

                helperTest.waitElementId(driver, 60, "home_img_5");
                Assert.AreEqual(homeUrl, driver.Url);

                string item1 = driver.FindElement(By.XPath("/html/body/app-root/div/app-home/div/div[2]/div[2]/div[6]/app-product-card/mdb-card/div/mdb-card-body/mdb-card-title/a/p")).Text;

                helperTest.JsClickElementID(driver, "home_img_5");

                helperTest.waitElementId(driver, 60, "product_name_in_product_page");
                bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
                Assert.IsTrue(bodyTextProduct.Contains(item1));

                helperTest.InputStringId(driver, "5", "qty_product_page");

                helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

                helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/app-preview-cart-panel/section/div/div[2]/article/div[1]/div/app-product-card[1]/article/div[2]/div[1]/p[1]");

                string item2 = driver.FindElement(By.XPath("/html/body/app-root/div/app-product/app-preview-cart-panel/section/div/div[2]/article/div[1]/div/app-product-card[1]/article/div[2]/div[1]/p[1]")).Text;

                helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

                helperTest.waitElementId(driver, 60, "product-name-in-cart0");

                bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
                Assert.IsTrue(bodyTextProduct.Contains(item2));

                string PartNumber = driver.FindElement(By.Id("item-name-in-cart0")).Text;

                helperTest.InputStringId(driver, "Patient 4", "patient_id_in_cart0");
                helperTest.InputStringId(driver, "test notes for " + PartNumber, "notes_in_cart0");

                Thread.Sleep(1000);

                //Patient 5

                SearchBox = driver.FindElement(By.Id("search"));
                SearchBox.SendKeys("aspen summit");
                SearchBox.SendKeys(Keys.Enter);

                helperTest.waitElementId(driver, 60, "configurable_img_0");
                helperTest.JsClickElement(driver, "//*[text()='" + "Summit Lock" + "']");

                helperTest.waitElementId(driver, 60, "product_name_in_product_page");
                bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("Summit Lock"));

                helperTest.InputStringXpath(driver, "CD102", "/html/body/app-root/div/app-product/div[1]/div[3]/section/div/h6[1]/input");
                helperTest.waitElementId(driver, 60, "add_product_to_cart0");
                helperTest.JsClickElementID(driver, "add_product_to_cart0");

                Thread.Sleep(3000);

                helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

                helperTest.waitElementId(driver, 60, "product-name-in-cart0");

                bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("Summit Lock Low Profile Bowtie"));

                helperTest.InputStringId(driver, "Patient 5", "patient_id_in_cart0");
                helperTest.InputStringId(driver, "test notes for item CD102", "notes_in_cart0");

                Thread.Sleep(1000);

                //Patient 6

                SearchBox = driver.FindElement(By.Id("search"));
                SearchBox.SendKeys("lyn valve");
                SearchBox.SendKeys(Keys.Enter);

                helperTest.waitElementId(driver, 60, "configurable_img_0");
                helperTest.JsClickElement(driver, "//*[text()='" + "Lyn Valve® RV Auto-Expulsion Kit" + "']");

                helperTest.waitElementId(driver, 60, "product_name_in_product_page");
                Assert.AreEqual(mainURLs + "product?productID=19543", driver.Url);

                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div/select", 2);

                Thread.Sleep(3000);

                helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

                Thread.Sleep(4000);

                helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

                helperTest.waitElementId(driver, 60, "product-name-in-cart0");

                bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("Lyn Valve RV Auto-Expulsion Kit"));

                helperTest.InputStringId(driver, "Patient 6", "patient_id_in_cart0");
                helperTest.InputStringId(driver, "test notes for item PA0002", "notes_in_cart0");

                Thread.Sleep(1000);

                //Patient 7

                SearchBox = driver.FindElement(By.Id("search"));
                SearchBox.SendKeys("limblogic");
                SearchBox.SendKeys(Keys.Enter);

                helperTest.waitElementId(driver, 60, "configurable_img_0");
                helperTest.JsClickElement(driver, "//*[text()='" + "LimbLogic®" + "']");

                helperTest.waitElementId(driver, 60, "product_name_in_product_page");
                Assert.AreEqual(mainURLs + "product?productID=3723", driver.Url);

                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div[1]/select", 3);
                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div[2]/select", 4);

                Thread.Sleep(2000);

                helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

                helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/div[5]/div/div/div[2]/div/div/div/article[1]/div[5]/p/app-qty/input");

                helperTest.InputStringXpath(driver, "5", "/html/body/app-root/div/app-product/div[5]/div/div/div[2]/div/div/div/article[1]/div[5]/p/app-qty/input");
                driver.FindElement(By.XPath("/html/body/app-root/div/app-product/div[5]/div/div/div[2]/div/div/div/article[1]/div[6]/app-button/div/button")).Click();

                Thread.Sleep(3000);

                helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

                helperTest.waitElementId(driver, 60, "product-name-in-cart0");

                bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("LimbLogic Unilateral Kit Thermoplastic"));
                Assert.IsTrue(bodyTextProduct.Contains("LimbLogic Vacuum Pyramid Adaptor"));

                helperTest.InputStringId(driver, "Patient 7", "patient_id_in_cart0");
                helperTest.InputStringId(driver, "test notes for item LLV-01044", "notes_in_cart0");
                helperTest.InputStringId(driver, "Patient 7", "patient_id_in_cart1");
                helperTest.InputStringId(driver, "test notes for item LLV-2000-T", "notes_in_cart1");

                Thread.Sleep(1000);

                //Patient 8

                SearchBox = driver.FindElement(By.Id("search"));
                SearchBox.SendKeys("RevoFit2 Kit");
                SearchBox.SendKeys(Keys.Enter);

                helperTest.waitElementId(driver, 60, "configurable_img_0");
                helperTest.JsClickElementID(driver, "configurable_img_0");

                helperTest.waitElementId(driver, 60, "product_name_in_product_page");
                bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("RevoFit2 Kit"));

                helperTest.waitElementId(driver, 60, "add_product_to_cart0");
                helperTest.JsClickElementID(driver, "add_product_to_cart0");

                helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

                helperTest.waitElementId(driver, 60, "product-name-in-cart0");

                bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("RevoFit2 Lamination Kit"));

                helperTest.InputStringId(driver, "Patient 8", "patient_id_in_cart0");
                helperTest.InputStringId(driver, "test notes for item PK2000-320-05", "notes_in_cart0");

                Thread.Sleep(1000);

                //Patient 9

                SearchBox = driver.FindElement(By.Id("search"));
                SearchBox.SendKeys("tamarack");
                SearchBox.SendKeys(Keys.Enter);

                helperTest.waitElementId(driver, 60, "configurable_img_0");
                helperTest.JsClickElement(driver, "//*[text()='" + "GlideWear Hair & Scalp Protection Pillowcase" + "']");

                helperTest.waitElementId(driver, 60, "product_name_in_product_page");
                bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("GlideWear Hair & Scalp Protection Pillowcase"));

                helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

                Thread.Sleep(4000);

                helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

                helperTest.waitElementId(driver, 60, "product-name-in-cart0");
                bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("GlideWear Protection Pillowcase"));

                helperTest.InputStringId(driver, "Patient 9", "patient_id_in_cart0");
                helperTest.InputStringId(driver, "test notes for item GW-PCS", "notes_in_cart0");

                Thread.Sleep(1000);

                //Patient 10

                SearchBox = driver.FindElement(By.Id("search"));
                SearchBox.SendKeys("knee");
                SearchBox.SendKeys(Keys.Enter);

                helperTest.waitElementId(driver, 60, "configurable_img_0");

                helperTest.JsClickElement(driver, "//*[text()='" + "Prosthetics" + "']");

                Thread.Sleep(3000);

                helperTest.JsClickElement(driver, "//*[text()='" + "Ossur" + "']");

                helperTest.waitElementId(driver, 60, "configurable_img_0");
                helperTest.JsClickElement(driver, "//*[text()='" + "Balance™ Knee" + "']");

                helperTest.waitElementId(driver, 60, "product_name_in_product_page");
                bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("Balance™ Knee"));
                Assert.AreEqual(mainURLs + "product?productID=8911", driver.Url);

                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div/select", 2);

                Thread.Sleep(3000);

                helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

                Thread.Sleep(4000);

                helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

                helperTest.waitElementId(driver, 60, "product-name-in-cart0");
                bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("Balance Knee Kit"));

                helperTest.InputStringId(driver, "Patient 10", "patient_id_in_cart0");
                helperTest.InputStringId(driver, "test notes for item BKN12500", "notes_in_cart0");

                Thread.Sleep(1000);

                //Patient 11

                helperTest.JsClickElement(driver, "/html/body/app-root/app-header/nav/div[1]/a/span/i");

                Thread.Sleep(1000);

                helperTest.JsClickElement(driver, "//*[text()='" + " Foot Care " + "']");

                Thread.Sleep(1000);

                helperTest.JsClickElement(driver, "/html/body/app-root/app-header/nav/app-header-categories-panel/section/div/div[2]/collapsible-list/collapsible-list-item[9]/collapsible-body/span[5]");

                helperTest.waitElementId(driver, 60, "configurable_img_0");
                helperTest.JsClickElement(driver, "//*[text()='" + "APEX® Boss Runner - Men" + "']");

                helperTest.waitElementId(driver, 60, "product_name_in_product_page");
                bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("APEX® Boss Runner - Men"));
                Assert.AreEqual(mainURLs + "product?productID=567", driver.Url);

                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[3]/app-magento-attributes/form/div/div[1]/select", 2);
                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[3]/app-magento-attributes/form/div/div[2]/select", 2);
                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[3]/app-magento-attributes/form/div/div[3]/select", 9);
                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[3]/app-magento-attributes/form/div/div[4]/select", 3);

                helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

                Thread.Sleep(4000);

                helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

                helperTest.waitElementId(driver, 60, "product-name-in-cart0");
                bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("Boss Runner Black MW10"));

                helperTest.InputStringId(driver, "Patient 11", "patient_id_in_cart0");
                helperTest.InputStringId(driver, "test notes for item X520MM10", "notes_in_cart0");

                Thread.Sleep(1000);
            }

            //Patient 12

            SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.SendKeys("ossur");
            SearchBox.SendKeys(Keys.Enter);

            helperTest.waitElementId(driver, 60, "configurable_img_0");

            helperTest.JsClickElement(driver, "//*[text()='" + "Prosthetics" + "']");

            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "//*[text()='" + "Lower Extremity" + "']");

            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "//*[text()='" + "Knee" + "']");

            Thread.Sleep(3000);

            helperTest.waitElementId(driver, 60, "configurable_img_0");

            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;

            if (bodyTextProduct.Contains("Balance™ Knee OFM"))
            {
                helperTest.JsClickElement(driver, "//*[text()='" + "Balance™ Knee OFM" + "']");
            }
            else
            {
                helperTest.JsClickElement(driver, "/html/body/app-root/div/app-category/div/div/div[2]/div/ngb-pagination/ul/li[3]/a");
                helperTest.waitElementId(driver, 60, "configurable_img_0");
                bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("Balance™ Knee OFM2"));

                helperTest.JsClickElement(driver, "//*[text()='" + "Balance™ Knee OFM2" + "']");
            }

            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            Thread.Sleep(4000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

            helperTest.waitElementId(driver, 60, "product-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Balance Knee Kit"));

            helperTest.InputStringId(driver, "Patient 12", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item BKN12500", "notes_in_cart0");

            Thread.Sleep(1000);

            //Patient 13

            SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.SendKeys("993740");
            SearchBox.SendKeys(Keys.Enter);

            helperTest.waitElementId(driver, 60, "configurable_img_0");
            helperTest.JsClickElement(driver, "//*[text()='" + "Aspen® Horizon™" + "']");

            helperTest.waitElementId(driver, 60, "product_name_in_product_page");
            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Aspen® Horizon™"));
            Assert.AreEqual(mainURLs + "product?productID=17026", driver.Url);

            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div/select", 2);

            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            Thread.Sleep(4000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

            helperTest.waitElementId(driver, 60, "product-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Balance Knee Kit"));

            helperTest.InputStringId(driver, "Patient 13", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item BKN12500", "notes_in_cart0");

            Thread.Sleep(1000);

            //Patient 14

            SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.SendKeys("PRAFO® Ankle Foot Orthosis");
            SearchBox.SendKeys(Keys.Enter);

            helperTest.waitElementId(driver, 60, "configurable_img_0");
            helperTest.JsClickElement(driver, "//*[text()='" + "Anatomical Concepts PRAFO® Ankle Foot Orthosis" + "']");

            helperTest.waitElementId(driver, 60, "product_name_in_product_page");
            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Anatomical Concepts PRAFO® Ankle Foot Orthosis"));
            Assert.AreEqual(mainURLs + "product?productID=13411", driver.Url);

            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div/select", 2);

            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            Thread.Sleep(4000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

            helperTest.waitElementId(driver, 60, "product-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Balance Knee Kit"));

            helperTest.InputStringId(driver, "Patient 14", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item BKN12500", "notes_in_cart0");

            Thread.Sleep(1000);

            //Patient 15

            SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.SendKeys("Tamarack® Flexure Joint™");
            SearchBox.SendKeys(Keys.Enter);

            helperTest.waitElementId(driver, 60, "configurable_img_0");
            helperTest.JsClickElement(driver, "//*[text()='" + "Tamarack® Flexure Joint™" + "']");

            helperTest.waitElementId(driver, 60, "product_name_in_product_page");
            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Balance™ Knee"));
            Assert.AreEqual(mainURLs + "product?productID=8911", driver.Url);

            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div/select", 2);

            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            Thread.Sleep(4000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

            helperTest.waitElementId(driver, 60, "product-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Balance Knee Kit"));

            helperTest.InputStringId(driver, "Patient 15", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item BKN12500", "notes_in_cart0");

            Thread.Sleep(1000);

            //Patient 16

            SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.SendKeys("Premier Post-Op Brace");
            SearchBox.SendKeys(Keys.Enter);

            helperTest.waitElementId(driver, 60, "configurable_img_0");
            helperTest.JsClickElement(driver, "//*[text()='" + "Breg® T Scope® Premier Post-Op Brace" + "']");

            helperTest.waitElementId(driver, 60, "product_name_in_product_page");
            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Breg® T Scope® Premier Post-Op Brace"));
            Assert.AreEqual(mainURLs + "product?productID=9975", driver.Url);

            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div/select", 2);

            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            Thread.Sleep(4000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

            helperTest.waitElementId(driver, 60, "product-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Balance Knee Kit"));

            helperTest.InputStringId(driver, "Patient 16", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item BKN12500", "notes_in_cart0");

            Thread.Sleep(1000);

            //Patient 17

            SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.SendKeys("Aspen Vista");
            SearchBox.SendKeys(Keys.Enter);

            helperTest.waitElementId(driver, 60, "configurable_img_0");
            helperTest.JsClickElement(driver, "//*[text()='" + "Aspen® Vista® CTO4" + "']");

            helperTest.waitElementId(driver, 60, "product_name_in_product_page");
            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Aspen® Vista® CTO4"));
            Assert.AreEqual(mainURLs + "product?productID=14609", driver.Url);

            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div/select", 2);

            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            Thread.Sleep(4000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

            helperTest.waitElementId(driver, 60, "product-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Balance Knee Kit"));

            helperTest.InputStringId(driver, "Patient 17", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item BKN12500", "notes_in_cart0");

            Thread.Sleep(1000);

            //Patient 18

            SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.SendKeys("L5679");
            SearchBox.SendKeys(Keys.Enter);

            helperTest.waitElementId(driver, 60, "configurable_img_0");
            helperTest.JsClickElement(driver, "//*[text()='" + "Alpha Hybrid® Liners" + "']");

            helperTest.waitElementId(driver, 60, "product_name_in_product_page");
            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Alpha Hybrid® Liners"));
            Assert.AreEqual(mainURLs + "product?productID=7158", driver.Url);

            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div/select", 2);

            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            Thread.Sleep(4000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

            helperTest.waitElementId(driver, 60, "product-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Balance Knee Kit"));

            helperTest.InputStringId(driver, "Patient 18", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item BKN12500", "notes_in_cart0");

            Thread.Sleep(1000);

            //Patient 19

            SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.SendKeys("C1L");
            SearchBox.SendKeys(Keys.Enter);

            helperTest.waitElementId(driver, 60, "configurable_img_0");
            helperTest.JsClickElement(driver, "//*[text()='" + "Fabtech +PLUSeries® Composite Adhesive" + "']");

            helperTest.waitElementId(driver, 60, "product_name_in_product_page");
            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Fabtech +PLUSeries® Composite Adhesive"));
            Assert.AreEqual(mainURLs + "product?productID=6648", driver.Url);

            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div/select", 2);

            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            Thread.Sleep(4000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

            helperTest.waitElementId(driver, 60, "product-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Balance Knee Kit"));

            helperTest.InputStringId(driver, "Patient 19", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item BKN12500", "notes_in_cart0");

            Thread.Sleep(1000);

            //Patient 20

            SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.SendKeys("sprystep");
            SearchBox.SendKeys(Keys.Enter);

            helperTest.waitElementId(driver, 60, "configurable_img_0");
            helperTest.JsClickElement(driver, "//*[text()='" + "Townsend SpryStep®" + "']");

            helperTest.waitElementId(driver, 60, "product_name_in_product_page");
            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Townsend SpryStep®"));
            Assert.AreEqual(mainURLs + "product?productID=15363", driver.Url);

            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div[1]/app-attributes/form/div/div/select", 2);

            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            Thread.Sleep(4000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

            helperTest.waitElementId(driver, 60, "product-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Balance Knee Kit"));

            helperTest.InputStringId(driver, "Patient 20", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item BKN12500", "notes_in_cart0");

            Thread.Sleep(1000);
        }

        //[Test]
        public void SubmitterApprover()
        {
            Actions actions = new Actions(driver);
            IWebElement SearchBox;
            IWebElement InpBox;
            string bodyTextProduct;

            login = "sergeycascade11@yandex.com";
            password = "StevenGerrard_11";

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);                        

            Assert.AreEqual(homeUrl, driver.Url);

            helperTest.UseDropDownIdByName(driver, "basic-addon1", "Part Number");

            helperTest.waitElementId(driver, 60, "search");
            SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.Clear();
            SearchBox.SendKeys("1210");
            SearchBox.SendKeys(Keys.Enter);

            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div[1]/app-category/div/div/div[2]/app-details/div[1]/div/table/tbody/tr[1]/td[7]/div/div[1]/app-button/div/button");
            helperTest.CloseShoppingCartPreview(driver);

            helperTest.JsClickElement(driver, "/html/body/app-root/div[1]/app-category/div/div/div[2]/app-details/div[1]/div/table/tbody/tr[2]/td[7]/div/div[1]/app-button/div/button");
            helperTest.CloseShoppingCartPreview(driver);

            helperTest.JsClickElement(driver, "/html/body/app-root/div[1]/app-category/div/div/div[2]/app-details/div[1]/div/table/tbody/tr[3]/td[7]/div/div[1]/app-button/div/button");

            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("1210"));

            Thread.Sleep(1000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div[1]/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-submitter/section/section/article/div[2]/app-cart-product-order-for-submitter[1]/section/div/article[2]/div/div[1]/div/label");
            helperTest.JsClickElement(driver, "/html/body/app-root/div[1]/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-submitter/section/section/article/div[2]/app-cart-product-order-for-submitter[2]/section/div/article[2]/div/div[1]/div/label");
            helperTest.JsClickElement(driver, "/html/body/app-root/div[1]/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-submitter/section/section/article/div[2]/app-cart-product-order-for-submitter[3]/section/div/article[2]/div/div[1]/div/label");

            helperTest.InputStringId(driver, "Patient 1", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item 1", "notes_in_cart0");
            helperTest.InputStringId(driver, "Patient 2", "patient_id_in_cart1");
            helperTest.InputStringId(driver, "test notes for item 2", "notes_in_cart1");
            helperTest.InputStringId(driver, "Patient 3", "patient_id_in_cart2");
            helperTest.InputStringId(driver, "test notes for item 3", "notes_in_cart2");

            helperTest.UseDropDown(driver, "/html/body/app-root/div[1]/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-submitter/section/aside/app-order-info-aside-for-submitter/aside/div[3]/select", 1);

            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-submitter/section/aside/app-order-info-aside-for-submitter/aside/div[1]/p[3]")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("2638 Aztexc Drive"));

            helperTest.JsClickElement(driver, "//*[text()='" + " Submit for Approval " + "']");

            Thread.Sleep(3000);

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div[1]/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-submitter/div/div/div/div/div");

            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Your order has been submitted"));

            driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-submitter/div/div/div/div/div")).Click();

            Thread.Sleep(3000);

            IWebElement SwitchUser = driver.FindElement(By.Id("username_button"));
            actions.MoveToElement(SwitchUser).Build().Perform();

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/app-header/nav/div[1]/div[3]/links/ul/li[1]/div/div/a[4]");
            driver.FindElement(By.XPath("/html/body/app-root/app-header/nav/div[1]/div[3]/links/ul/li[1]/div/div/a[4]")).Click();

            Thread.Sleep(3000);

            helperTest.waitElementId(driver, 60, "home_img_0");

            helperTest.JsClickElementId(driver, "header_cart_icon");
            helperTest.waitElementId(driver, 60, "submit_order");

            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Modified Grace Plate"));
            Assert.IsTrue(bodyTextProduct.Contains("Ped 4-Hole Plate AL"));
            Assert.IsTrue(bodyTextProduct.Contains("Original Grace Plate"));

            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-approver/section/aside/app-order-info-aside-for-approver/aside/div[1]/p[3]")).Text;

            if (bodyTextProduct.Contains("1415 Kilborn Drive"))
            {
                helperTest.JsClickElement(driver, "/html/body/app-root/div[1]/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-approver/section/section/article[2]/div[2]/app-cart-product-order-for-approver[1]/section/div/article[4]/div[4]/app-tag-button[2]/span/span");
            }
            else
            {
                helperTest.JsClickElement(driver, "//*[text()='" + "Edit" + "']");

                helperTest.UseDropDown(driver, "/html/body/app-root/div[1]/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-approver/section/aside/app-order-info-aside-for-approver/aside/div[1]/div/select", 2);

                bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-approver/section/aside/app-order-info-aside-for-approver/aside/div[1]/p[3]")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("1415 Kilborn Drive"));                

                Thread.Sleep(1000);

                helperTest.JsClickElement(driver, "/html/body/app-root/div[1]/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-approver/section/section/article[2]/div[2]/app-cart-product-order-for-approver[1]/section/div/article[4]/div[4]/app-tag-button[2]/span/span");
            }                      

            Thread.Sleep(2000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Keep Current " + "']");

            helperTest.waitElementId(driver, 60, "product-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("product-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Ped 4-Hole Plate AL"));

            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-approver/section/aside/app-order-info-aside-for-approver/aside/div[1]/p[3]")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("1415 Kilborn Drive"));

            Thread.Sleep(1000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div[1]/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-approver/section/section/article[2]/div[2]/app-cart-product-order-for-approver[1]/section/div/article[4]/div[4]/app-tag-button[2]/span/span");

            Thread.Sleep(2000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Use Requested " + "']");

            helperTest.waitElementId(driver, 60, "product-name-in-cart1");
            bodyTextProduct = driver.FindElement(By.Id("product-name-in-cart1")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Modified Grace Plate"));

            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-approver/section/aside/app-order-info-aside-for-approver/aside/div[1]/p[3]")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("1225 West Front Street"));

            Thread.Sleep(1000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div[1]/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-approver/section/section/article[2]/div[2]/app-cart-product-order-for-approver/section/div/article[4]/div[4]/app-tag-button[1]/span/span");

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div[1]/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-approver/section/section/article[2]/div[2]/app-cart-product-order-for-approver/div/div/div/div[2]/div[2]/div/button[1]");
            helperTest.JsClickElement(driver, "/html/body/app-root/div[1]/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-approver/section/section/article[2]/div[2]/app-cart-product-order-for-approver/div/div/div/div[2]/div[2]/div/button[1]");

            Thread.Sleep(1000);

            InpBox = driver.FindElement(By.Id("poNumber"));
            InpBox.Clear();
            InpBox.SendKeys("TESTPO " + DateTime.Now.ToString("yyyyMMdd"));

            InpBox = driver.FindElement(By.Id("notesInput"));
            InpBox.Clear();
            InpBox.SendKeys("TEST ORDER PLEASE DO NOT PROCESS " + DateTime.Now.ToString("yyyyMMdd"));

            Thread.Sleep(1000);

            helperTest.JsClickElementId(driver, "submit_order");

            helperTest.waitElementId(driver, 180, "product-name-in-cartundefined");

            Assert.AreEqual(mainURLs + "cart/review", driver.Url);

            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div[1]/app-cart-root/div/div/app-review-cart/section/aside/app-order-info-aside/aside/div[1]/p[2]")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("1225 West Front Street"));

            Thread.Sleep(2000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-cart-root/div/div/app-review-cart/section/aside/app-order-info-aside/aside/div[3]/div[3]/app-button/div/button");

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-cart-root/div/div/app-review-cart/div[1]/div/div/div[3]/app-button/div/button");

            helperTest.FindTextInBody(driver, "Thank you for your order. Your order number is");

            Thread.Sleep(4000);

            helperTest.JsClickElement(driver, "//*[text()='" + " OK " + "']");

            Thread.Sleep(2000);
        }

        [TearDown]
        public void Cleanup()
        {
            driver.Quit();           
        }
    }
}