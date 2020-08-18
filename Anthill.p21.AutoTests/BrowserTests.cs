﻿using System;
using System.IO;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System.Linq;
using OpenQA.Selenium.Support.Extensions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Text;
using System.ComponentModel.DataAnnotations;
using OpenQA.Selenium.Support.UI;

namespace Selenium.Test
{

    public class PageTestBase
    {
        protected IWebDriver Driver;

        protected void UITest(Action action, IWebDriver driver, string nameTest, int retriesRemaining = 1)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {

                var screenshot = driver.TakeScreenshot();

                var filePath = "../logs_img/test_" + nameTest + ".jpg";

                screenshot.SaveAsFile(filePath, ScreenshotImageFormat.Jpeg);

                throw;

                //((ITakesScreenshot)driver).GetScreenshot().SaveAsFile("Test3.png", ScreenshotImageFormat.Png);
            }
        }
    }

    [TestFixture]
    public class BrowserTests : PageTestBase
    {
        private static IWebDriver driver;
        private static HelperTest helperTest;
        string mainURL;
        string homeUrl;
        string authUrl;

        private string password;
        private string login;

        string currentFile = string.Empty;
        string mainURLs = "https://v2dev.cascade-usa.com/";

        [SetUp]
        public void SetUp()
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var pathDrivers = directory + "/../../../../drivers/";
            mainURL = mainURLs + "";
            homeUrl = mainURLs + "home";
            authUrl = mainURLs + "auth/login";

            //login = "Anthony.Kosenko@gmail.com";
            //password = "12345";

            login = "artvbashuk@gmail.com";
            password = "9999";

            //login = "sergeykorolevsky2015@gmail.com";
            //password = "5555";

            //login = "artvbashuk@gmail.com";
            //password = "123";

            //FirstName = "Sergey";
            //LastName = "Korolevsky";
            //Email = "sergeykorolevsky2015@gmail.com";
            //PhoneNumber = "8888888888";
            //RegPassword = "54321";
            //ConfirmPassword = "54321";
            //CascadeAccountNumber = "102978";

            helperTest = new HelperTest();

            ChromeOptions options = new ChromeOptions();

            options.AddArguments("--no-sandbox");
            options.AddArguments("--headless");

            options.AddUserProfilePreference("download.default_directory", "C:/Work/Anthill/Anthill.p21.AutoTests/logs_img");
            options.AddUserProfilePreference("intl.accept_languages", "nl");
            options.AddUserProfilePreference("disable-popup-blocking", "true");

            driver = new ChromeDriver(pathDrivers, options);
            //driver = new InternetExplorerDriver(pathDrivers);            

            driver.Manage().Cookies.DeleteAllCookies();
            driver.Manage().Window.Size = new System.Drawing.Size(1920, 1024);
            //driver.Manage().Window.Maximize();

            //driver.Url = mainURLs + "auth/login";
            //driver.Manage().Window.Maximize();
            //_edge = new EdgeDriver(pathDrivers);
            //_firefox = new FirefoxDriver(pathDrivers);

            //((ITakesScreenshot)driver).GetScreenshot().SaveAsFile("Test3.png", ScreenshotImageFormat.Png); - screenshots
        }

        //[Test]
        public void CheckFilesByUrls()
        {
            string pathCsv = "C:\\Work\\Anthill\\Anthill.p21.AutoTests\\Anthill.p21.AutoTests\\bin\\Debug\\netcoreapp2.1\\validate_files_done_false.csv";
            string outPathCsv = "C:\\Work\\Anthill\\Anthill.p21.AutoTests\\Anthill.p21.AutoTests\\bin\\Debug\\netcoreapp2.1\\validate_files_out4.csv";

            File.Delete(outPathCsv);

            List<Values> values = File.ReadAllLines(pathCsv)
                                            .Skip(1)
                                            .Select(v => Values.FromCsv(v))
                                            .ToList();

            string name = "3D-LITELSOXTRA_SIZECHART.png";
            string folder = "C:\\Work\\Anthill\\Anthill.p21.AutoTests\\Anthill.p21.AutoTests\\bin\\Debug\\netcoreapp2.1\\files\\";

            string bufff = "item_category_link_uid,item_category_uid,sequence_no,link_name,full_link_path, isLoading";
            List<String> bufffs = new List<String>();
            bufffs.Add(bufff);
            File.WriteAllLines(outPathCsv, bufffs);

            for (int i = 0; i < values.Count(); i++)
            {
                List<String> buffs = new List<String>();

                try
                {
                    downloadFile(values[i].full_link_path, name, folder);
                    values[i].isFound = "True";
                }
                catch (Exception ex)
                {
                    values[i].isFound = "false";

                }

                string buff = values[i].item_category_link_uid + "," + values[i].item_category_uid + "," + values[i].sequence_no + "," + values[i].link_name + "," + values[i].full_link_path + "," + values[i].isFound;

                buffs.Add(Encoding.UTF8.GetString(Encoding.Default.GetBytes(buff)));

                File.AppendAllLines(outPathCsv, buffs);

            }

        }

        public void downloadFile(string remoteUri, string name, string folder)
        {

            string myStringWebResource = null;

            // Create a new WebClient instance.
            using (WebClient myWebClient = new WebClient())
            {
                myStringWebResource = remoteUri;
                // Download the Web resource and save it into the current filesystem folder.
                myWebClient.DownloadFile(myStringWebResource, folder + name);
                //myWebClient.DownloadFileCompleted();
            }
            Thread.Sleep(10000);
            //Task.Delay(10000).Wait();//wait for sometime till download is completed
            //string path = "C:\\Users\\abc\\Downloads";//the path of the folder where the zip file will be downloaded
            string path = folder + name;

            if (Directory.Exists(folder)) //we check if the directory or folder exists
            {
                bool result = CheckFile(name, folder); // boolean result true or false is stored after checking the zip file name
                if (result == true)
                {
                    //ExtractFiles();// if the zip file is present , this method is called to extract files within the zip file
                    File.Delete(path);
                }

                else
                {
                    Assert.Fail("The file does not exist.");// if the zip file is not present, then the  test fails
                }
            }
            else
            {
                Assert.Fail("The directory or folder does not exist.");//if the directory or folder does not exist, then the test fails
            }
        }


        public bool CheckFile(string name, string folder) // the name of the zip file which is obtained, is passed in this method
        {
            currentFile = folder + name + ""; // the zip filename is stored in a variable
            if (File.Exists(currentFile)) //helps to check if the zip file is present
            {
                return true; //if the zip file exists return boolean true
            }
            else
            {
                return false; // if the zip file does not exist return boolean false
            }
        }

        [Test]
        public void Login()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            Thread.Sleep(4000);
            Assert.AreEqual(homeUrl, driver.Url);
            IWebElement ClickUser = driver.FindElement(By.Id("username_button"));

            Actions actions = new Actions(driver);
            actions.MoveToElement(ClickUser).Build().Perform();

            helperTest.waitElementId(driver, 60, "logout_button");
            var LogOut = driver.FindElement(By.Id("logout_button"));

            LogOut.Click();

            Assert.AreEqual(authUrl, driver.Url);

            Thread.Sleep(4000);
        }

        //[Test]
        public void SubmitterApproval()
        {
            login = "Anthony.Kosenko@gmail.com";
            password = "12345";

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            Thread.Sleep(4000);
            Assert.AreEqual(homeUrl, driver.Url);

            driver.Url = mainURLs + "product?productID=1282";

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            Thread.Sleep(5000);

            helperTest.JsClickElementId(driver, "header_cart_icon");

            Thread.Sleep(2000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-submitter/section/section/article/div[2]/app-cart-product-order-for-submitter/section/article[2]/div/input[1]");

            helperTest.JsClickElement(driver, "//*[text()='" + " Submit for Approval " + "']");

            Thread.Sleep(5000);

            string bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("submitted to Andrew"));

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-submitter/div/div/div/div/div");

            IWebElement ClickUser = driver.FindElement(By.Id("username_button"));

            Actions actions = new Actions(driver);
            actions.MoveToElement(ClickUser).Build().Perform();

            helperTest.waitElementId(driver, 60, "logout_button");
            var LogOut = driver.FindElement(By.Id("logout_button"));

            LogOut.Click();

            login = "nikitin_andrew@bk.ru";
            password = "12345";

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            Thread.Sleep(5000);

            helperTest.JsClickElementId(driver, "header_cart_icon");

            helperTest.waitElementId(driver, 60, "submit_order");

            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Original Grace Plate"));

            Thread.Sleep(1000);

            helperTest.JsClickElement(driver, "//*[text()='" + "Move to Cart" + "']");

            Thread.Sleep(1000);

            helperTest.JsClickElement(driver, "//*[text()='" + "Keep Current" + "']");

            IWebElement InpBox = driver.FindElement(By.Id("poNumber"));
            InpBox.SendKeys("TESTPO " + DateTime.Now.ToString("yyyyMMdd"));

            IWebElement InpBox2 = driver.FindElement(By.Id("notesInput"));
            InpBox2.Clear();
            InpBox2.SendKeys("TEST ORDER PLEASE DO NOT PROCESS " + DateTime.Now.ToString("yyyyMMdd"));

            Thread.Sleep(1000);

        }
        [Test]
        public void AddToCartFromPreview()
        {
            Actions actions = new Actions(driver);
            String bodyTextProduct;
            IWebElement NavigateCusror;

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            Thread.Sleep(4000);

            Assert.AreEqual(homeUrl, driver.Url);

            helperTest.waitElementId(driver, 60, "search");

            IWebElement SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.SendKeys("liners AK");

            SearchBox.SendKeys(Keys.Enter);

            Thread.Sleep(3000);
            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/div/div[2]/div/div[1]/app-product-card-configurable/section/div[1]/a/img");

            NavigateCusror = driver.FindElement(By.XPath("/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/div/div[2]/div/div[1]/app-product-card-configurable/section/div[1]/a/img"));
            actions.MoveToElement(NavigateCusror).Build().Perform();

            helperTest.JsClickElement(driver, "//*[text()='" + " Preview " + "']");

            Thread.Sleep(3000);

            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/app-preview-details-panel/section/div/div[2]/mdb-card/div/mdb-card-body/mdb-card-text/p/div/span[1]")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Alpha Classic® AK Liner"));

            helperTest.UseDropDownByName(driver, "/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/app-preview-details-panel/section/div/div[2]/mdb-card/div/mdb-card-body/mdb-card-text/p/div/app-attributes/form/div/div[1]/select", "Locking");
            helperTest.UseDropDownByName(driver, "/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/app-preview-details-panel/section/div/div[2]/mdb-card/div/mdb-card-body/mdb-card-text/p/div/app-attributes/form/div/div[2]/select", "Large");
            helperTest.UseDropDownByName(driver, "/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/app-preview-details-panel/section/div/div[2]/mdb-card/div/mdb-card-body/mdb-card-text/p/div/app-attributes/form/div/div[3]/select", "MAX");

            helperTest.UseDropDownByName(driver, "/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/app-preview-details-panel/section/div/div[2]/mdb-card/div/mdb-card-body/mdb-card-text/p/div/app-attributes/form/div/div[4]/select", "Standard Umbrella");

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            Thread.Sleep(3000);

            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/app-preview-cart-panel/section/div/div[2]/article/div[1]/div[1]/app-product-card[1]/article/div[2]/div[1]/p[2]")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("AKL-2636-X"));

            helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

            Thread.Sleep(3000);

            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("AKL-2636-X"));

            Thread.Sleep(2000);

            helperTest.JsClickElementID(driver, "remove-button-0");
        }

        [Test]
        public void Pagenation()
        {
            IWebElement Img;
            Boolean ImagePresent;

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            Thread.Sleep(4000);

            Assert.AreEqual(homeUrl, driver.Url);

            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));

            SearchBox.Clear();
            SearchBox.SendKeys("Liners");
            SearchBox.SendKeys(Keys.Enter);

            Thread.Sleep(5000);

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

                Thread.Sleep(5000);
            }

        }

        [Test]
        public void CheckImagesOnPages()
        {
            IWebElement Img;
            Boolean ImagePresent;

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            Thread.Sleep(4000);

            Assert.AreEqual(homeUrl, driver.Url);

            helperTest.waitElementId(driver, 60, "search");

            for (int i = 1; i <= 5; i++)
            {
                string path = "/html/body/app-root/div/app-home/div/div[2]/div[2]/div[" + i.ToString() + "]/ app-product-card/mdb-card/div/a/mdb-card-img/img";
                Img = driver.FindElement(By.XPath(path));

                ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);
                Assert.IsTrue(ImagePresent);
            }

            driver.Url = mainURLs + "product?productID=255";

            helperTest.waitElementId(driver, 60, "search");

            Thread.Sleep(5000);

            helperTest.waitElementId(driver, 60, "myimage");

            Img = driver.FindElement(By.Id("myimage"));

            ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);

            Assert.IsTrue(ImagePresent);

            IWebElement SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.SendKeys("orthotics");

            SearchBox.SendKeys(Keys.Enter);

            Thread.Sleep(12000);

            for (int i = 0; i < 25; i++)
            {

                if (i == 2) i += 1;
                string ids = "configurable_img_" + i.ToString();

                Img = driver.FindElement(By.Id(ids));


                ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);

                Assert.IsTrue(ImagePresent);

            }

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

            Thread.Sleep(8000);

            helperTest.waitElementId(driver, 60, "header_cart_icon");
            IWebElement CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();

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

            IWebElement RemoveList2 = driver.FindElement(By.XPath("/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/mdb-card/div/mdb-card-body/mdb-card-text/p/div/div/span[1]/i"));
            Actions RemoveTheList2 = new Actions(driver);
            RemoveTheList2.MoveToElement(RemoveList2).Build().Perform();
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/mdb-card/div/mdb-card-body/mdb-card-text/p/div/div/span[1]/i");

            Thread.Sleep(4000);
        }

        [Test]
        public void Comparision()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Actions actions = new Actions(driver);
            String bodyTextProduct;
            IWebElement NavigateCusror;

            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));

            SearchBox.Clear();
            SearchBox.SendKeys("RevoFit");

            SearchBox.SendKeys(Keys.Enter);

            Thread.Sleep(3000);
            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/div/div[2]/div/div[1]/app-product-card-configurable/section/div[1]/a/img");

            NavigateCusror = driver.FindElement(By.XPath("/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/div/div[2]/div/div[1]/app-product-card-configurable/section/div[1]/a/img"));
            actions.MoveToElement(NavigateCusror).Build().Perform();

            helperTest.JsClickElement(driver, "//*[text()='" + " + Compare " + "']");

            NavigateCusror = driver.FindElement(By.XPath("/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/div/div[2]/div/div[2]/app-product-card-configurable/section/div[1]/a/img"));
            actions.MoveToElement(NavigateCusror).Build().Perform();

            helperTest.JsClickElement(driver, "//*[text()='" + " + Compare " + "']");

            NavigateCusror = driver.FindElement(By.XPath("/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/div/div[2]/div/div[3]/app-product-card-configurable/section/div[1]/a/img"));
            actions.MoveToElement(NavigateCusror).Build().Perform();

            helperTest.JsClickElement(driver, "//*[text()='" + " + Compare " + "']");

            NavigateCusror = driver.FindElement(By.XPath("/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/div/div[2]/div/div[4]/app-product-card-configurable/section/div[1]/a/img"));
            actions.MoveToElement(NavigateCusror).Build().Perform();

            helperTest.JsClickElement(driver, "//*[text()='" + " + Compare " + "']");

            helperTest.JsClickElement(driver, "//*[text()='" + " Compare products " + "']");

            Thread.Sleep(4000);

            Assert.AreEqual(mainURLs + "comparison", driver.Url);

            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;

            Assert.IsTrue(bodyTextProduct.Contains("RevoFit2 Kit"));
            Assert.IsTrue(bodyTextProduct.Contains("Medical RevoLock Lanyard Kit"));
            Assert.IsTrue(bodyTextProduct.Contains("Medical RevoLock Upper Extremity"));
            Assert.IsTrue(bodyTextProduct.Contains("Medical Replacement Parts"));

            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-compare/section/section/div[1]/div/app-compare-item[3]/section/article/div[1]/i");

            Thread.Sleep(1000);

            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;

            Assert.IsFalse(bodyTextProduct.Contains("Medical RevoLock Upper Extremity"));

            Thread.Sleep(1000);

        }

        [Test]
        public void ShipAddress()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            Thread.Sleep(4000);

            driver.Url = mainURLs + "account-info";

            Thread.Sleep(4000);

            String[] s1 = new String[4];

            s1[0] = driver.FindElement(By.XPath("/html/body/app-root/div/app-account-info/div[1]/div[4]/div/div/div[2]/div/div[1]/span[1]")).Text;
            s1[1] = driver.FindElement(By.XPath("/html/body/app-root/div/app-account-info/div[1]/div[4]/div/div/div[3]/div/div[1]/span[1]")).Text;
            s1[2] = driver.FindElement(By.XPath("/html/body/app-root/div/app-account-info/div[1]/div[4]/div/div/div[4]/div/div[1]/span[1]")).Text;
            s1[3] = driver.FindElement(By.XPath("/html/body/app-root/div/app-account-info/div[1]/div[4]/div/div/div[5]/div/div[1]/span[1]")).Text;

            driver.Url = mainURLs + "cart/index";

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-approver/section/section/article[2]/div[1]/div/div/div/input");

            Thread.Sleep(4000);

            helperTest.JsClickElement(driver, "//*[text()='" + "Edit" + "']");

            driver.FindElement(By.XPath("/html/body/app-root/div/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-approver/section/aside/app-order-info-aside-for-approver/aside/div[1]/div/select")).Click();

            String bodyText = driver.FindElement(By.TagName("body")).Text;
            for (int i = 0; i < 4; i++)
            {
                Assert.IsTrue(bodyText.Contains(s1[i]));
            }

            driver.Url = mainURLs + "account-info";

            Thread.Sleep(4000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-account-info/div[1]/div[4]/div/div/div[3]/div/div[2]/div[2]/label");
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-account-info/div[1]/div[4]/div/div/div[4]/div/div[2]/div[2]/label");
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-account-info/div[1]/div[4]/div/div/div[5]/div/div[2]/div[2]/label");

            driver.Url = mainURLs + "cart/index";

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-approver/section/section/article[2]/div[1]/div/div/div/input");

            Thread.Sleep(2000);

            helperTest.JsClickElement(driver, "//*[text()='" + "Edit" + "']");

            driver.FindElement(By.XPath("/html/body/app-root/div/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-approver/section/aside/app-order-info-aside-for-approver/aside/div[1]/div/select")).Click();

            bodyText = driver.FindElement(By.TagName("body")).Text;

            Assert.IsFalse(bodyText.Contains(s1[1]));
            Assert.IsFalse(bodyText.Contains(s1[2]));
            Assert.IsFalse(bodyText.Contains(s1[3]));

            driver.Url = mainURLs + "account-info";

            Thread.Sleep(4000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-account-info/div[1]/div[4]/div/div/div[3]/div/div[2]/div[2]/label");
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-account-info/div[1]/div[4]/div/div/div[4]/div/div[2]/div[2]/label");
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-account-info/div[1]/div[4]/div/div/div[5]/div/div[2]/div[2]/label");

            Thread.Sleep(4000);
        }

        [Test]
        public void NewShipAddress()
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
                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-approver/section/aside/app-order-info-aside-for-approver/div/div/div/div[2]/div/form/select[1]", 2);
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
        public void FilterAndClearAll()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            Thread.Sleep(4000);

            Assert.AreEqual(homeUrl, driver.Url);

            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));

            SearchBox.Clear();
            SearchBox.SendKeys("Liners");
            SearchBox.SendKeys(Keys.Enter);

            Thread.Sleep(5000);

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
        public void LoginWrongCreds()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, "fgdgf", mainURL);

            Thread.Sleep(4000);

            Assert.AreEqual(authUrl, driver.Url);
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

            Thread.Sleep(4000);

            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;

            Assert.IsTrue(bodyTextProduct.Contains("Balance™ Knee"));
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

        [Test]
        public void QuickOrderAndDeleteFromCart()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            Thread.Sleep(4000);

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

            IWebElement ele = driver.FindElement(By.XPath("/html/body/app-root/div/app-quick-order-pad/div[1]/div[3]/div/app-button/div/button"));
            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            executor.ExecuteScript("arguments[0].click();", ele);

            Thread.Sleep(1000);

            IWebElement CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();

            Thread.Sleep(5000);

            Assert.AreEqual(mainURLs + "cart/index", driver.Url);

            helperTest.waitElementId(driver, 60, "product-name-in-cart0");

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

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        [Test]
        public void SubmitOrder()
        {
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

            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-cart-root/div/div/app-review-cart/section/aside/app-order-info-aside/aside/div[3]/div[3]/app-button/div/button");

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-cart-root/div/div/app-review-cart/div[1]/div/div/div[3]/app-button/div/button");

            helperTest.FindTextInBody(driver, "Thank you for your order. Your order number is");

            Thread.Sleep(4000);

            helperTest.JsClickElement(driver, "//*[text()='" + " OK " + "']");

            Thread.Sleep(2000);
        }

        [Test]
        public void submitRMAs()
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

        [Test]
        public void searchByHCPCS()
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
        public void searchByPartName()
        {
            IWebElement CartBtn;
            String bodyTextProduct1;
            String bodyTextProduct2;

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

            Thread.Sleep(5000);

            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();

            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct1 = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct1.Contains("ALC-9460-E"));

            helperTest.UseDropDownIdByName(driver, "basic-addon1", "Part Number");

            helperTest.waitElementId(driver, 60, "search");
            SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.Clear();
            SearchBox.SendKeys("ALC-5067-E");

            helperTest.waitElementId(driver, 60, "add_to_cart_in_dropdown");
            helperTest.JsClickElementId(driver, "add_to_cart_in_dropdown");

            Thread.Sleep(4000);

            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct2 = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct2.Contains("ALC-5067-E"));

            for (int j = 0; j < 2; j++)
            {
                helperTest.JsClickElementID(driver, "remove-button-0");

                Thread.Sleep(1000);
            }
        }

        [Test]
        public void MostFrequentlyPurchased()
        {
            string bodyTextProduct;

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

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

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span");
            driver.FindElement(By.XPath("/html/body/app-root/div/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span")).Click();

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/div[5]/div/div/div[2]/div/div/div/article[1]/div[5]/p/app-qty/input");

            string item1 = driver.FindElement(By.XPath("/html/body/app-root/div/app-product/div[5]/div/div/div[2]/div/div/div/article[1]/div[2]/p")).Text;
            string item2 = driver.FindElement(By.XPath("/html/body/app-root/div/app-product/div[5]/div/div/div[2]/div/div/div/article[4]/div[2]/p")).Text;
            helperTest.InputStringXpath(driver, "5", "/html/body/app-root/div/app-product/div[5]/div/div/div[2]/div/div/div/article[1]/div[5]/p/app-qty/input");
            driver.FindElement(By.XPath("/html/body/app-root/div/app-product/div[5]/div/div/div[2]/div/div/div/article[1]/div[6]/app-button/div/button")).Click();

            Thread.Sleep(5000);

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span");
            driver.FindElement(By.XPath("/html/body/app-root/div/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span")).Click();

            driver.FindElement(By.XPath("/html/body/app-root/div/app-product/div[5]/div/div/div[2]/div/div/div/article[4]/div[6]/app-button/div/button")).Click();

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
        public void ReplacementParts()
        {
            string bodyTextProduct;

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

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

            driver.FindElement(By.XPath("/html/body/app-root/div/app-product/div[3]/div/div/div[1]/app-close-button/p/span")).Click();

            Thread.Sleep(1000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            Thread.Sleep(3000);

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
        public void AccessoryParts()
        {
            string bodyTextProduct;

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Assert.AreEqual(homeUrl, driver.Url);

            helperTest.waitElementId(driver, 60, "search");
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

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/div[4]/div/div/div[2]/div/div/section/div/h6[1]/input");
            IWebElement PartNumber = driver.FindElement(By.XPath("/html/body/app-root/div/app-product/div[4]/div/div/div[2]/div/div/section/div/h6[1]/input"));
            PartNumber.Clear();
            PartNumber.SendKeys("740-L-BLK-5PK");

            Thread.Sleep(1000);
                        
            string item1 = driver.FindElement(By.XPath("/html/body/app-root/div/app-product/div[4]/div/div/div[2]/div/div/div/article[3]/div[4]/p")).Text;
            helperTest.InputStringXpath(driver, "2", "/html/body/app-root/div/app-product/div[4]/div/div/div[2]/div/div/div/article[3]/div[7]/p/app-qty/input");
            driver.FindElement(By.XPath("/html/body/app-root/div/app-product/div[4]/div/div/div[2]/div/div/div/article[3]/div[8]/app-button/div/button")).Click();

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span");
            driver.FindElement(By.XPath("/html/body/app-root/div/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span")).Click();

            string item2 = driver.FindElement(By.XPath("/html/body/app-root/div/app-product/div[4]/div/div/div[2]/div/div/div/article[7]/div[4]/p")).Text;
            helperTest.InputStringXpath(driver, "4", "/html/body/app-root/div/app-product/div[4]/div/div/div[2]/div/div/div/article[7]/div[7]/p/app-qty/input");
            driver.FindElement(By.XPath("/html/body/app-root/div/app-product/div[4]/div/div/div[2]/div/div/div/article[7]/div[8]/app-button/div/button")).Click();

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span");
            driver.FindElement(By.XPath("/html/body/app-root/div/app-product/app-preview-cart-panel/section/div/div[1]/app-close-button/p/span")).Click();
            driver.FindElement(By.XPath("/html/body/app-root/div/app-product/div[4]/div/div/div[1]/app-close-button/p/span")).Click();
            Thread.Sleep(1000);

            helperTest.InputStringXpath(driver, "740-L-BLK-5PK", "/html/body/app-root/div/app-product/div[1]/div[3]/section/div/h6[1]/input");
            helperTest.waitElementId(driver, 60, "add_product_to_cart0");
            helperTest.JsClickElementID(driver, "add_product_to_cart0");

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

        //[Test]
        public void ExtendedOrder()
        {
            string bodyTextProduct;
            IWebElement SearchBox;

            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");
            Assert.AreEqual(homeUrl, driver.Url);

            var useTheCode = false;
            if (useTheCode)
            {
                //Patient 1

                SearchBox = driver.FindElement(By.Id("search"));
                SearchBox.SendKeys("tiso 464");
                SearchBox.SendKeys(Keys.Enter);

                helperTest.waitElementId(driver, 60, "configurable_img_0");
                helperTest.JsClickElementID(driver, "configurable_img_0");

                helperTest.waitElementId(driver, 60, "product_name_in_product_page");
                bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("Vista® 464 TLSO"));

                helperTest.InputStringId(driver, "2", "qty_product_page");

                helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

                helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/div[5]/div/div/div[2]/div/div/div/article[1]/div[5]/p/app-qty/input");

                helperTest.InputStringXpath(driver, "5", "/html/body/app-root/div/app-product/div[5]/div/div/div[2]/div/div/div/article[2]/div[5]/p/app-qty/input");
                driver.FindElement(By.XPath("/html/body/app-root/div/app-product/div[5]/div/div/div[2]/div/div/div/article[2]/div[6]/app-button/div/button")).Click();

                Thread.Sleep(3000);

                helperTest.JsClickElement(driver, "//*[text()='" + " Review Cart " + "']");

                helperTest.waitElementId(driver, 60, "product-name-in-cart0");

                bodyTextProduct = driver.FindElement(By.TagName("body")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("Vista 464 TLSO Upgrade Kit"));
                Assert.IsTrue(bodyTextProduct.Contains("Vista 464 TLSO Adjustable"));

                helperTest.InputStringId(driver, "Patient 1", "patient_id_in_cart0");
                helperTest.InputStringId(driver, "test notes for item 993644", "notes_in_cart0");
                helperTest.InputStringId(driver, "Patient 1", "patient_id_in_cart1");
                helperTest.InputStringId(driver, "test notes for item 993640", "notes_in_cart1");

                Thread.Sleep(1000);

                //Patient 2

                SearchBox = driver.FindElement(By.Id("search"));
                SearchBox.SendKeys("alpha classic");
                SearchBox.SendKeys(Keys.Enter);

                helperTest.waitElementId(driver, 60, "configurable_img_2");
                helperTest.JsClickElementID(driver, "configurable_img_2");

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
                Assert.IsTrue(bodyTextProduct.Contains("Alpha Classic Lock Uni Orig G/G 6mm LG"));

                helperTest.InputStringId(driver, "Patient 2", "patient_id_in_cart0");
                helperTest.InputStringId(driver, "test notes for item ALL-5066-E", "notes_in_cart0");

                Thread.Sleep(1000);

                //Patient 3

                helperTest.JsClickElementID(driver, "product-name-in-cart0");

                Assert.AreEqual(mainURLs + "product?productID=1483", driver.Url);

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
                Assert.IsTrue(bodyTextProduct.Contains("Alpha Classic Cush Uni Orig G/G 6mm LG"));

                helperTest.InputStringId(driver, "Patient 3", "patient_id_in_cart0");
                helperTest.InputStringId(driver, "test notes for item ALC-5066-E", "notes_in_cart0");

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









        [Test]
        public void AddToCartStep13()
        {
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Thread.Sleep(2000);

            IWebElement CartBtn;
            String bodyTextProduct;

            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));

            SearchBox.Clear();
            SearchBox.SendKeys("60SL");
            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "/html/body/app-root/app-header/nav/div[1]/div[2]/app-search-panel/div/div[2]/div/form/div[1]/app-search-panel-dropdown/div[1]/div/div[2]/p[1]");

            Thread.Sleep(5000);

            driver.Url.Contains("product?productID=6646");            

            helperTest.InputStringId(driver, "5", "qty_product_page1");
            helperTest.JsClickElementId(driver, "add_product_to_cart1");

            // go to cart
            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();

            // check sku
            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("60SL"));

            Thread.Sleep(2000);

            helperTest.JsClickElementID(driver, "remove-button-0");
        }

        ////[Test]
        public void Step14()
        {
            UITest(() =>
            {

                helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

                helperTest.waitElementId(driver, 60, "toggleQuickOrder");

                Actions actions = new Actions(driver);
                IWebElement CartBtn;
                String bodyTextProduct;
                IWebElement NavigateCusror;

                helperTest.JsClickElement(driver, "/html/body/app-root/app-header/nav/div/a/span/i");

                helperTest.JsClickElement(driver, "//*[text()='" + " Orthotics " + "']");
                helperTest.JsClickElement(driver, "//*[text()='" + " Spinal (721) " + "']");
                helperTest.JsClickElement(driver, "//*[text()='" + "Prefabricated Orthoses" + "']");

                helperTest.JsClickElement(driver, "//*[text()='" + " Aspen Medical Products (28) " + "']");
                helperTest.JsClickElement(driver, "//*[text()='" + "Aspen® Summit™ 456" + "']");

                Thread.Sleep(5000);
                Assert.AreEqual(mainURLs + "product?productID=8080", driver.Url);

                helperTest.InputStringId(driver, "2", "qty_product_page6");
                helperTest.JsClickElementId(driver, "add_product_to_cart6");

                // go to cart
                helperTest.waitElementId(driver, 60, "header_cart_icon");
                CartBtn = driver.FindElement((By.Id("header_cart_icon")));
                CartBtn.Click();
                // check sku
                helperTest.waitElementId(driver, 60, "item-name-in-cart0");
                bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("992710"));
                // wtite descr
                helperTest.InputStringId(driver, "patient 13", "patient_id_in_cart0");
                helperTest.InputStringId(driver, "test notes for item 992710", "notes_in_cart0");
            }, driver, MethodBase.GetCurrentMethod().ToString() + DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        ////[Test]
        public void Step15()
        {
            UITest(() =>
            {
                helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

                helperTest.waitElementId(driver, 60, "toggleQuickOrder");

                Actions actions = new Actions(driver);
                IWebElement CartBtn;
                String bodyTextProduct;
                IWebElement NavigateCusror;

                helperTest.waitElementId(driver, 60, "search");
                IWebElement SearchBox = driver.FindElement(By.Id("search"));

                SearchBox.Clear();
                SearchBox.SendKeys("acrylic resin");
                Thread.Sleep(3000);

                Thread.Sleep(3000);
                helperTest.waitElementXpath(driver, 60, "/html/body/app-root/app-header/nav/div/div[1]/app-search-panel/div/div[2]/div[2]/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div[1]/div/p[2]/span[2]");

                NavigateCusror = driver.FindElement(By.XPath("/html/body/app-root/app-header/nav/div/div[1]/app-search-panel/div/div[2]/div[2]/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div[1]/div/p[2]/span[2]"));

                actions = new Actions(driver);
                actions.MoveToElement(NavigateCusror).Build().Perform();

                helperTest.JsClickElementId(driver, "product-card-img");

                Thread.Sleep(5000);

                Assert.AreEqual(mainURLs + "product?productID=8580", driver.Url);

                helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

                // go to cart
                helperTest.waitElementId(driver, 60, "header_cart_icon");
                CartBtn = driver.FindElement((By.Id("header_cart_icon")));
                CartBtn.Click();
                // check sku
                helperTest.waitElementId(driver, 60, "item-name-in-cart0");
                bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("EAR1"));
                // wtite descr
                helperTest.InputStringId(driver, "patient 13", "patient_id_in_cart0");
                helperTest.InputStringId(driver, "test notes for item EAR1", "notes_in_cart0");
            }, driver, MethodBase.GetCurrentMethod().ToString() + DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        //[Test]
        public void AddToCartStep16()
        {
            //UITest(() =>
            //{
            helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            Assert.AreEqual(homeUrl, driver.Url);
            Actions actions = new Actions(driver);
            IWebElement CartBtn;
            String bodyTextProduct;
            IWebElement NavigateCusror;

            //search text
            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.Clear();
            SearchBox.SendKeys("Ossur");

            Thread.Sleep(3000);
            helperTest.JsClickElement(driver, "//*[text()='" + " Ossur " + "']");
            helperTest.JsClickElement(driver, "//*[text()='" + "Prosthetics" + "']");
            helperTest.JsClickElement(driver, "//*[text()='" + "Lower Extremity" + "']");
            helperTest.JsClickElement(driver, "//*[text()='" + "Knee Prosthetics" + "']");
            helperTest.JsClickElement(driver, "//*[text()='" + "Balance™ Knee OFM2" + "']");

            Thread.Sleep(3000);
            Assert.AreEqual(mainURLs + "product?productID=15676", driver.Url);

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[5]/div/div/div[1]/app-close-button/p/span");

            // go to cart
            helperTest.waitElementId(driver, 60, "header_cart_icon");
            CartBtn = driver.FindElement((By.Id("header_cart_icon")));
            CartBtn.Click();
            // check sku
            helperTest.waitElementId(driver, 60, "item-name-in-cart0");
            bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("1721120"));
            // wtite descr
            helperTest.InputStringId(driver, "patient 15", "patient_id_in_cart0");
            helperTest.InputStringId(driver, "test notes for item 1721120", "notes_in_cart0");
            helperTest.InputStringId(driver, "2", "qty-in-cart0");

            Thread.Sleep(5000);
            // }, driver, MethodBase.GetCurrentMethod().ToString() + DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        [Test]
        public void AddToCartStep17()
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

        ////[Test]
        public void Step19()
        {
            UITest(() =>
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
                SearchBox.SendKeys("tamarack");

                Thread.Sleep(3000);

                SearchBox.SendKeys(Keys.Enter);

                Thread.Sleep(3000);
                helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/div/div[2]/div/div[1]/app-product-card-configurable/section/div[1]/a/img");

                NavigateCusror = driver.FindElement(By.XPath("/html/body/app-root/div/app-category/div/div/div[2]/app-configurable/div/div[2]/div/div[1]/app-product-card-configurable/section/div[1]/a/img"));
                actions.MoveToElement(NavigateCusror).Build().Perform();

                helperTest.JsClickElement(driver, "//*[text()='" + " Preview " + "']");
                helperTest.JsClickElement(driver, "//*[text()='" + "View Product Details" + "']");

                Thread.Sleep(3000);
                Assert.AreEqual(mainURLs + "product?productID=502", driver.Url);

                helperTest.InputStringXpath(driver, "740-M", "/html/body/app-root/div/app-product/div[1]/div[3]/section/div/h6[1]/input");
                Thread.Sleep(2000);
                helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[1]/div[3]/div[1]/article[2]/div[6]/app-button/div/button");

                //go to cart
                helperTest.waitElementId(driver, 60, "header_cart_icon");
                CartBtn = driver.FindElement((By.Id("header_cart_icon")));
                CartBtn.Click();
                // check sku
                helperTest.waitElementId(driver, 60, "item-name-in-cart0");
                bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("740-M"));
                // wtite descr
                helperTest.InputStringId(driver, "patient 17", "patient_id_in_cart0");
                helperTest.InputStringId(driver, "test notes for item 740-M", "notes_in_cart0");

                Thread.Sleep(5000);
            }, driver, MethodBase.GetCurrentMethod().ToString() + DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        ////[Test]
        public void Step20()
        {
            UITest(() =>
            {
                helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

                helperTest.waitElementId(driver, 60, "toggleQuickOrder");

                Actions actions = new Actions(driver);
                IWebElement CartBtn;
                String bodyTextProduct;
                IWebElement NavigateCusror;

                Thread.Sleep(3000);
                driver.Url = mainURLs + "product?productID=9975";

                helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[1]/div[3]/div[1]/article[1]/div[6]/app-button/div/button");

                //go to cart
                helperTest.waitElementId(driver, 60, "header_cart_icon");
                CartBtn = driver.FindElement((By.Id("header_cart_icon")));
                CartBtn.Click();
                // check sku
                helperTest.waitElementId(driver, 60, "item-name-in-cart0");
                bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("08814"));
                // wtite descr
                helperTest.InputStringId(driver, "patient 19", "patient_id_in_cart0");
                helperTest.InputStringId(driver, "test notes for item 08814", "notes_in_cart0");

                Thread.Sleep(5000);
            }, driver, MethodBase.GetCurrentMethod().ToString() + DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        ////[Test]
        public void Step21()
        {
            UITest(() =>
            {
                helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

                helperTest.waitElementId(driver, 60, "toggleQuickOrder");

                Actions actions = new Actions(driver);
                IWebElement CartBtn;
                String bodyTextProduct;
                IWebElement NavigateCusror;

                Thread.Sleep(3000);
                driver.Url = mainURLs + "product?productID=14609";

                helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[1]/div[1]/div/mdb-breadcrumb/ol/mdb-breadcrumb-item[2]/li");

                Thread.Sleep(3000);
                Assert.AreEqual(mainURLs + "category/catalogsearch/configurable?searchBy=category&queryStr=14&categoryName=Cervical&viewMode=configurable&categories=%7B%22ORTHOTICS%22:0,%22CERVICAL%22:0%7D", driver.Url);


                helperTest.JsClickElement(driver, "//*[text()='" + " Aspen Medical Products (9) " + "']");
                helperTest.JsClickElement(driver, "//*[text()='" + "Aspen® Vista® CTO4" + "']");

                Assert.AreEqual(mainURLs + "product?productID=14609", driver.Url);

                helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

                //go to cart
                helperTest.waitElementId(driver, 60, "header_cart_icon");
                CartBtn = driver.FindElement((By.Id("header_cart_icon")));
                CartBtn.Click();
                // check sku
                helperTest.waitElementId(driver, 60, "item-name-in-cart0");
                bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("984550"));
                // wtite descr
                helperTest.InputStringId(driver, "patient 20", "patient_id_in_cart0");
                helperTest.InputStringId(driver, "test notes for item 984550", "notes_in_cart0");

                Thread.Sleep(5000);
            }, driver, MethodBase.GetCurrentMethod().ToString() + DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        ////[Test]
        public void Step22()
        {
            UITest(() =>
            {
                helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

                helperTest.waitElementId(driver, 60, "toggleQuickOrder");

                Actions actions = new Actions(driver);
                IWebElement CartBtn;
                String bodyTextProduct;
                IWebElement NavigateCusror;

                helperTest.JsClickElement(driver, "/html/body/app-root/app-header/nav/div/a/span/i");
                helperTest.JsClickElement(driver, "//*[text()='" + " Prosthetics " + "']");
                helperTest.JsClickElement(driver, "//*[text()='" + " Liners / Suspension (837) " + "']");
                helperTest.JsClickElement(driver, "//*[text()='" + "L5679" + "']");

                helperTest.JsClickElement(driver, "//*[text()='" + "Alpha Hybrid® Liners" + "']");

                Thread.Sleep(3000);
                Assert.AreEqual(mainURLs + "product?productID=7158", driver.Url);



                helperTest.UseDropDownByName(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[1]/select", "Standard Umbrella");
                helperTest.UseDropDownByName(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[2]/select", "Uniform");
                helperTest.UseDropDownByName(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[3]/select", "Medium Plus");
                helperTest.UseDropDownByName(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[4]/select", "Locking");

                //helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[1]/select", 2);
                //helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[2]/select", 3);
                //helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[3]/select", 6);
                //helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[4]/select", 3);

                helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

                //go to cart
                helperTest.waitElementId(driver, 60, "header_cart_icon");
                CartBtn = driver.FindElement((By.Id("header_cart_icon")));
                CartBtn.Click();
                // check sku
                helperTest.waitElementId(driver, 60, "item-name-in-cart0");
                bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("H351-5364"));
                // wtite descr
                helperTest.InputStringId(driver, "patient 21", "patient_id_in_cart0");
                helperTest.InputStringId(driver, "test notes for item H351-5364", "notes_in_cart0");

                Thread.Sleep(5000);
            }, driver, MethodBase.GetCurrentMethod().ToString() + DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        ////[Test]
        public void Step23()
        {
            UITest(() =>
            {
                helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

                helperTest.waitElementId(driver, 60, "toggleQuickOrder");

                Actions actions = new Actions(driver);
                IWebElement CartBtn;
                String bodyTextProduct;
                IWebElement NavigateCusror;

                helperTest.waitElementId(driver, 60, "search");
                IWebElement SearchBox = driver.FindElement(By.Id("search"));

                SearchBox.SendKeys("vista collar w/pads set");
                SearchBox.SendKeys(Keys.Enter);

                helperTest.JsClickElement(driver, "//*[text()='" + "Aspen® Vista® Collar" + "']");

                Thread.Sleep(3000);
                Assert.AreEqual(mainURLs + "product?productID=3337", driver.Url);

                //helperTest.InputStringId(driver, "5", "qty_in_product_page1");
                //helperTest.JsClickElementId(driver, "add_to_cart_product_page1");

                helperTest.InputStringId(driver, "5", "qty_product_page1");
                helperTest.JsClickElementId(driver, "add_product_to_cart1");

                helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[5]/div/div/div[1]/app-close-button/p/span");

                //go to cart
                helperTest.waitElementId(driver, 60, "header_cart_icon");
                CartBtn = driver.FindElement((By.Id("header_cart_icon")));
                CartBtn.Click();
                // check sku
                helperTest.waitElementId(driver, 60, "item-name-in-cart0");
                bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("984000"));
                // wtite descr
                helperTest.InputStringId(driver, "patient 22", "patient_id_in_cart0");
                helperTest.InputStringId(driver, "test notes for item 984000", "notes_in_cart0");

                Thread.Sleep(5000);
            }, driver, MethodBase.GetCurrentMethod().ToString() + DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        ////[Test]
        public void Step24()
        {
            UITest(() =>
            {
                helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

                helperTest.waitElementId(driver, 60, "toggleQuickOrder");

                Thread.Sleep(3000);

                Actions actions = new Actions(driver);
                IWebElement CartBtn;
                String bodyTextProduct;
                IWebElement NavigateCusror;

                Thread.Sleep(3000);
                driver.Url = mainURLs + "product?productID=8706";

                helperTest.InputStringId(driver, "5", "qty_product_page");

                helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

                helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[5]/div/div/div[1]/app-close-button/p/span");

                //go to cart
                helperTest.waitElementId(driver, 60, "header_cart_icon");
                CartBtn = driver.FindElement((By.Id("header_cart_icon")));
                CartBtn.Click();
                // check sku
                helperTest.waitElementId(driver, 60, "item-name-in-cart0");
                bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("993540"));
                // wtite descr
                helperTest.InputStringId(driver, "patient 23", "patient_id_in_cart0");
                helperTest.InputStringId(driver, "test notes for item 993540", "notes_in_cart0");

                Thread.Sleep(5000);
            }, driver, MethodBase.GetCurrentMethod().ToString() + DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        ////[Test]
        public void Step25()
        {
            UITest(() =>
            {
                helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

                helperTest.waitElementId(driver, 60, "toggleQuickOrder");

                Actions actions = new Actions(driver);
                IWebElement CartBtn;
                String bodyTextProduct;
                IWebElement NavigateCusror;

                helperTest.waitElementId(driver, 60, "search");
                IWebElement SearchBox = driver.FindElement(By.Id("search"));

                SearchBox.SendKeys("c1l");

                Thread.Sleep(3000);
                helperTest.waitElementXpath(driver, 60, "/html/body/app-root/app-header/nav/div/div[1]/app-search-panel/div/div[2]/div[2]/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div[2]/div/p[2]/span[1]");

                NavigateCusror = driver.FindElement(By.XPath("/html/body/app-root/app-header/nav/div/div[1]/app-search-panel/div/div[2]/div[2]/form/div[1]/app-search-panel-dropdown/div[1]/div/div[1]/div[2]/div/p[2]/span[1]"));
                actions = new Actions(driver);
                actions.MoveToElement(NavigateCusror).Build().Perform();

                helperTest.JsClickElement(driver, "/html/body/app-root/app-header/nav/div/div[1]/app-search-panel/div/div[2]/div[2]/form/div[1]/app-search-panel-dropdown/div[1]/div/div[2]/div[2]/div[2]/span/app-button/div/button");

                Thread.Sleep(3000);

                //go to cart
                helperTest.waitElementId(driver, 60, "header_cart_icon");
                CartBtn = driver.FindElement((By.Id("header_cart_icon")));
                CartBtn.Click();
                // check sku
                helperTest.waitElementId(driver, 60, "item-name-in-cart0");
                bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("C1L"));
                // wtite descr
                helperTest.InputStringId(driver, "patient 24", "patient_id_in_cart0");
                helperTest.InputStringId(driver, "test notes for item C1L", "notes_in_cart0");

                Thread.Sleep(5000);
            }, driver, MethodBase.GetCurrentMethod().ToString() + DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        ////[Test]
        public void Step26()
        {
            UITest(() =>
            {
                helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

                helperTest.waitElementId(driver, 60, "toggleQuickOrder");

                Thread.Sleep(3000);

                Actions actions = new Actions(driver);
                IWebElement CartBtn;
                String bodyTextProduct;
                IWebElement NavigateCusror;

                Thread.Sleep(3000);
                driver.Url = mainURLs + "product?productID=1274";

                helperTest.UseDropDown(driver, "/html/body/app-root/div/app-product/div[1]/div[2]/div[3]/div/app-attributes/form/div/div[1]/select", 2);

                helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

                //go to cart
                helperTest.waitElementId(driver, 60, "header_cart_icon");
                CartBtn = driver.FindElement((By.Id("header_cart_icon")));
                CartBtn.Click();
                // check sku
                helperTest.waitElementId(driver, 60, "item-name-in-cart0");
                bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("700-250"));
                // wtite descr
                helperTest.InputStringId(driver, "patient 25", "patient_id_in_cart0");
                helperTest.InputStringId(driver, "test notes for item 700-250", "notes_in_cart0");

                Thread.Sleep(5000);
            }, driver, MethodBase.GetCurrentMethod().ToString() + DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        ////[Test]
        public void Step27()
        {
            UITest(() =>
            {
                helperTest.LoginToSite(driver, authUrl, homeUrl, login, password, mainURL);

                helperTest.waitElementId(driver, 60, "toggleQuickOrder");

                Actions actions = new Actions(driver);
                IWebElement CartBtn;
                String bodyTextProduct;
                IWebElement NavigateCusror;

                helperTest.waitElementId(driver, 60, "search");
                IWebElement SearchBox = driver.FindElement(By.Id("search"));

                SearchBox.SendKeys("sprystep");
                SearchBox.SendKeys(Keys.Enter);

                helperTest.JsClickElement(driver, "//*[text()='" + "Townsend SpryStep®" + "']");

                Thread.Sleep(3000);
                driver.Url = mainURLs + "product?productID=15363";

                helperTest.InputStringXpath(driver, "1001", "/html/body/app-root/div/app-product/div[1]/div[3]/section/div/h6[1]/input");

                helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[1]/div[3]/div[1]/article/div[6]/app-button/div/button");

                helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[5]/div/div/div[1]/app-close-button/p/span");
                Thread.Sleep(3000);
                //go to cart
                helperTest.waitElementId(driver, 60, "header_cart_icon");
                CartBtn = driver.FindElement((By.Id("header_cart_icon")));
                CartBtn.Click();
                // check sku
                helperTest.waitElementId(driver, 60, "item-name-in-cart0");
                bodyTextProduct = driver.FindElement(By.Id("item-name-in-cart0")).Text;
                Assert.IsTrue(bodyTextProduct.Contains("17H1001"));
                // wtite descr
                helperTest.InputStringId(driver, "patient 26", "patient_id_in_cart0");
                helperTest.InputStringId(driver, "test notes for item 17H1001", "notes_in_cart0");

                Thread.Sleep(5000);
            }, driver, MethodBase.GetCurrentMethod().ToString() + DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        ////[Test]
        public void AllStepsExtendedShopCart()
        {
            UITest(() =>
            {
                //Step02();
                //Step13();
                Step14();
                Step15();
                //Step16();
                //Step17();
                //Step18();
                Step19();
                Step20();
                Step21();
                Step22();
                Step23();
                Step24();
                Step25();
                Step26();
                Step27();                
            }, driver, MethodBase.GetCurrentMethod().ToString() + DateTime.Now.ToString("yyyyMMddHHmmss"));

        }

        public void PerfomanceTest()
        {

            driver.Url = authUrl;

            IWebElement InpBox = driver.FindElement(By.Id("input-mail"));
            InpBox.SendKeys(login);

            IWebElement PassBox = driver.FindElement(By.Id("input-pass"));
            PassBox.SendKeys(password);

            //IWebElement OkButton = driver.FindElement(By.Id("login_button"));

            //OkButton.Click();
            var start = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            helperTest.JsClickElement(driver, "//*[text()='" + " Login " + "']");

            helperTest.waitElementId(driver, 60, "toggleQuickOrder");

            var end = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            var times = (end - start);

            Console.WriteLine(times);
            Console.ReadKey();
        }

        [TearDown]
        public void Cleanup()
        {
            //driver.Quit();
            driver?.Dispose();
        }
    }
}
