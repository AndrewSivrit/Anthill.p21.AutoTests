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
    [TestFixture]
    class MobileTests
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

            //login = "sergeykorolevsky2015@gmail.com";
            //password = "111";

            login = "artvbashuk@gmail.com";
            password = "9999";

            //login = "artvbashuk@gmail.com";
            //password = "123";

            helperTest = new HelperTest();
            ChromeOptions options = new ChromeOptions();

            options.AddArguments("--no-sandbox");
            options.AddArguments("--headless");

            options.AddUserProfilePreference("download.default_directory", "C:/Work/Anthill/Anthill.p21.AutoTests/logs_img");
            options.AddUserProfilePreference("intl.accept_languages", "nl");
            options.AddUserProfilePreference("disable-popup-blocking", "true");

            options.EnableMobileEmulation("iPhone X");

            driver = new ChromeDriver(pathDrivers, options);
            //driver = new InternetExplorerDriver(pathDrivers);

            driver.Manage().Cookies.DeleteAllCookies();
            driver.Manage().Window.Maximize();

            //driver.Url = mainURLs + "auth/login";
            //driver.Manage().Window.Maximize();
            //_edge = new EdgeDriver(pathDrivers);
            //_firefox = new FirefoxDriver(pathDrivers);

            //((ITakesScreenshot)driver).GetScreenshot().SaveAsFile("Test3.png", ScreenshotImageFormat.Png); - screenshots
        }

        [Test]
        public void MobileLogin()
        {
            helperTest.LoginToSiteMobile(driver, authUrl, homeUrl, login, password, mainURL);

            Thread.Sleep(4000);
            Assert.AreEqual(homeUrl, driver.Url);
            IWebElement ClickMenu = driver.FindElement(By.Id("open-category-panel-button"));
            ClickMenu.Click();

            Thread.Sleep(1000);

            IWebElement LogOut = driver.FindElement(By.Id("log-out"));
            LogOut.Click();

            Assert.AreEqual(authUrl, driver.Url);

            Thread.Sleep(4000);
        }

        [Test]
        public void MobileLoginWrongCreds()
        {
            helperTest.LoginToSiteMobile(driver, authUrl, homeUrl, login, "fgdgf", mainURL);

            Thread.Sleep(4000);

            Assert.AreEqual(authUrl, driver.Url);
        }

        [Test]
        public void MobileSearchPopProduct()
        {            
            String bodyTextProduct;

            helperTest.LoginToSiteMobile(driver, authUrl, homeUrl, login, password, mainURL);

            Thread.Sleep(4000);

            IWebElement SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.SendKeys("Knee");

            SearchBox.SendKeys(Keys.Enter);

            Thread.Sleep(4000);

            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;

            Assert.IsTrue(bodyTextProduct.Contains("Balance™ Knee"));            
        }

        [Test]
        public void MobileSearchByPartName()
        {
            Actions actions = new Actions(driver);
            IWebElement OpenItem;
            String bodyTextProduct;

            helperTest.LoginToSiteMobile(driver, authUrl, homeUrl, login, password, mainURL);

            helperTest.waitElementId(driver, 60, "open-category-panel-button");

            Assert.AreEqual(homeUrl, driver.Url);

            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));

            SearchBox.Clear();
            SearchBox.SendKeys("ALC-9460-E");

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/app-header/nav/div[2]/app-search-panel/div/div/div/app-search-panel-dropdown/div[1]/div/div/div[2]/div/p[2]/span[1]");
            OpenItem = driver.FindElement((By.XPath("/html/body/app-root/app-header/nav/div[2]/app-search-panel/div/div/div/app-search-panel-dropdown/div[1]/div/div/div[2]/div/p[2]/span[1]")));
            OpenItem.Click();

            Thread.Sleep(3000);

            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div/app-product/div[1]/div/p[1]")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Alpha Classic® Liner"));

            helperTest.UseDropDownByName(driver, "/html/body/app-root/div/app-product/div[1]/div/div[2]/app-attributes/form/div/div[1]/select", "Cushion");
            helperTest.UseDropDownByName(driver, "/html/body/app-root/div/app-product/div[1]/div/div[2]/app-attributes/form/div/div[2]/select", "Medium");
            helperTest.UseDropDownByName(driver, "/html/body/app-root/div/app-product/div[1]/div/div[2]/app-attributes/form/div/div[3]/select", "Contoured");

            helperTest.UseDropDownByName(driver, "/html/body/app-root/div/app-product/div[1]/div/div[2]/app-attributes/form/div/div[4]/select", "6 mm");

            helperTest.UseDropDownByName(driver, "/html/body/app-root/div/app-product/div[1]/div/div[2]/app-attributes/form/div/div[5]/select", "Buff");

            helperTest.UseDropDownByName(driver, "/html/body/app-root/div/app-product/div[1]/div/div[2]/app-attributes/form/div/div[6]/select", "Spirit");
            helperTest.UseDropDownByName(driver, "/html/body/app-root/div/app-product/div[1]/div/div[2]/app-attributes/form/div/div[7]/select", "Without Umbrella");

            Thread.Sleep(5000);

            helperTest.waitElementId(driver, 60, "mobile-add-to-cart");
            OpenItem = driver.FindElement((By.Id("mobile-add-to-cart")));
            OpenItem.Click();

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-approver/section/section/article/div[2]/app-cart-product-order-for-approver[1]/section/article[1]/app-product-card/article/div[2]/p[2]");
            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-approver/section/section/article/div[2]/app-cart-product-order-for-approver[1]/section/article[1]/app-product-card/article/div[2]/p[2]")).Text;

            Assert.IsTrue(bodyTextProduct.Contains("ALC-9460-E"));

            Thread.Sleep(3000);

            helperTest.waitElementId(driver, 60, "search");
            SearchBox = driver.FindElement(By.Id("search"));

            SearchBox.Clear();
            SearchBox.SendKeys("ALC-5067-E");

            Thread.Sleep(3000);

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/app-header/nav/div[2]/app-search-panel/div/div/div/app-search-panel-dropdown/div[1]/div/div/div[2]/div/p[2]/span[1]");
            OpenItem = driver.FindElement((By.XPath("/html/body/app-root/app-header/nav/div[2]/app-search-panel/div/div/div/app-search-panel-dropdown/div[1]/div/div/div[2]/div/p[2]/span[1]")));
            OpenItem.Click();

            Thread.Sleep(3000);

            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div/app-product/div[1]/div/p[1]")).Text;
            Assert.IsTrue(bodyTextProduct.Contains("Alpha Classic® Liner"));

            helperTest.UseDropDownByName(driver, "/html/body/app-root/div/app-product/div[1]/div/div[2]/app-attributes/form/div/div[1]/select", "Cushion");
            helperTest.UseDropDownByName(driver, "/html/body/app-root/div/app-product/div[1]/div/div[2]/app-attributes/form/div/div[2]/select", "Extra Large");
            helperTest.UseDropDownByName(driver, "/html/body/app-root/div/app-product/div[1]/div/div[2]/app-attributes/form/div/div[3]/select", "Uniform");

            helperTest.UseDropDownByName(driver, "/html/body/app-root/div/app-product/div[1]/div/div[2]/app-attributes/form/div/div[4]/select", "6 mm");

            helperTest.UseDropDownByName(driver, "/html/body/app-root/div/app-product/div[1]/div/div[2]/app-attributes/form/div/div[5]/select", "Green/Grey");

            helperTest.UseDropDownByName(driver, "/html/body/app-root/div/app-product/div[1]/div/div[2]/app-attributes/form/div/div[6]/select", "Original");
            helperTest.UseDropDownByName(driver, "/html/body/app-root/div/app-product/div[1]/div/div[2]/app-attributes/form/div/div[7]/select", "Without Umbrella");

            Thread.Sleep(3000);

            helperTest.waitElementId(driver, 60, "mobile-add-to-cart");
            OpenItem = driver.FindElement((By.Id("mobile-add-to-cart")));
            OpenItem.Click();

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-approver/section/section/article/div[2]/app-cart-product-order-for-approver[1]/section/article[1]/app-product-card/article/div[2]/p[2]");
            bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-approver/section/section/article/div[2]/app-cart-product-order-for-approver[1]/section/article[1]/app-product-card/article/div[2]/p[2]")).Text;

            Assert.IsTrue(bodyTextProduct.Contains("ALC-5067-E"));

            Thread.Sleep(3000);
        }

        [Test]
        public void MobileSearchByHCPCS()
        {
            Actions actions = new Actions(driver);
            String bodyTextProduct;

            helperTest.LoginToSiteMobile(driver, authUrl, homeUrl, login, password, mainURL);

            helperTest.waitElementId(driver, 60, "open-category-panel-button");

            Assert.AreEqual(homeUrl, driver.Url);

            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));

            SearchBox.Clear();
            SearchBox.SendKeys("L5450");

            SearchBox.SendKeys(Keys.Enter);

            Thread.Sleep(5000);

            bodyTextProduct = driver.FindElement(By.TagName("body")).Text;

            Assert.IsTrue(bodyTextProduct.Contains("Respond ROM BKA Rigid Dressing"));
            Assert.IsTrue(bodyTextProduct.Contains("SafeLimb"));
            Assert.IsTrue(bodyTextProduct.Contains("Limbguard™"));
            Assert.IsTrue(bodyTextProduct.Contains("ProtectOR"));

            //SearchBox.Clear();
            //SearchBox.SendKeys("L8417");

            //SearchBox.SendKeys(Keys.Enter);

            //Thread.Sleep(3000);

            //driver.Navigate().Refresh();

            //Thread.Sleep(8000);

            //bodyTextProduct = driver.FindElement(By.TagName("body")).Text;

            //Assert.IsTrue(bodyTextProduct.Contains("One Sleeve"));
            //Assert.IsTrue(bodyTextProduct.Contains("One® Gel Sock"));
            //Assert.IsTrue(bodyTextProduct.Contains("Skin Reliever Gel Sheath"));
        }

        [Test]
        public void MobileCheckImagesOnPages()
        {
            IWebElement Img;
            Boolean ImagePresent;

            helperTest.LoginToSiteMobile(driver, authUrl, homeUrl, login, password, mainURL);

            Thread.Sleep(4000);

            Assert.AreEqual(homeUrl, driver.Url);

            helperTest.waitElementId(driver, 60, "search");

            for (int i = 1; i <= 5; i++)
            {
                string path = "/html/body/app-root/div/app-home/div/div[2]/div[2]/div[" + i.ToString() + "]/ app-product-card/mdb-card/div/mdb-card-img/img";
                Img = driver.FindElement(By.XPath(path));

                ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);

                Assert.IsTrue(ImagePresent);
            }

            driver.Url = mainURLs + "product?productID=255";

            helperTest.waitElementId(driver, 60, "search");

            Thread.Sleep(4000);

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/div[1]/div/div[1]/swiper/div/div[1]/div[1]/img");

            Img = driver.FindElement(By.XPath("/html/body/app-root/div/app-product/div[1]/div/div[1]/swiper/div/div[1]/div[1]/img"));

            ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);

            Assert.IsTrue(ImagePresent);

            IWebElement SearchBox = driver.FindElement(By.Id("search"));
            SearchBox.SendKeys("knee");

            SearchBox.SendKeys(Keys.Enter);

            Thread.Sleep(12000);

            for (int i = 1; i < 25; i++)
            {
                string path = "/html/body/app-root/div/app-category/div/div/div/app-configurable/div/div[2]/div/div[" + i.ToString() + "]/app-product-card-configurable/section/div[1]/img";
                Img = driver.FindElement(By.XPath(path));

                ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);

                Assert.IsTrue(ImagePresent);
            }
        }

        [Test]
        public void MobilePagenation()
        {
            IWebElement Img;
            Boolean ImagePresent;

            helperTest.LoginToSiteMobile(driver, authUrl, homeUrl, login, password, mainURL);

            Thread.Sleep(4000);

            Assert.AreEqual(homeUrl, driver.Url);

            helperTest.waitElementId(driver, 60, "search");
            IWebElement SearchBox = driver.FindElement(By.Id("search"));

            SearchBox.Clear();
            SearchBox.SendKeys("Liners");
            SearchBox.SendKeys(Keys.Enter);

            Thread.Sleep(5000);

            for (int j = 1; j < 4; j++)
            {
                for (int i = 1; i < 25; i++)
                {

                    string path = "/html/body/app-root/div/app-category/div/div/div/app-configurable/div/div[2]/div/div[" + i.ToString() + "]/app-product-card-configurable/section/div[1]/img";
                    Img = driver.FindElement(By.XPath(path));


                    ImagePresent = (Boolean)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].complete && typeof arguments[0].naturalWidth != \"undefined\" && arguments[0].naturalWidth > 0", Img);
                    Assert.IsTrue(ImagePresent);
                }

                helperTest.JsClickElement(driver, "/html/body/app-root/div/app-category/div/div/div/div/ngb-pagination/ul/li[8]/a/span");

                Thread.Sleep(5000);

                driver.Navigate().Refresh();

                Thread.Sleep(9000);
            }
        }

        public void InputAndCheckAdd(string productId, string nameCheck)
        {
            IWebElement InpBox = driver.FindElement(By.XPath("/html/body/app-root/div/app-quick-order-pad/div[1]/div[2]/input"));
            InpBox.Clear();
            InpBox.SendKeys(productId);

            Thread.Sleep(1000);

            IWebElement AddItem = driver.FindElement(By.Id("mobile-add-item"));
            AddItem.Click();

            Thread.Sleep(4000);

            String bodyText = driver.FindElement(By.TagName("body")).Text;
            Assert.IsTrue(bodyText.Contains(nameCheck));
        }

        [Test]
        public void MobileQuickOrderAndDeleteFromCart()
        {
            helperTest.LoginToSiteMobile(driver, authUrl, homeUrl, login, password, mainURL);

            Thread.Sleep(4000);

            helperTest.waitElementId(driver, 60, "open-category-panel-button");
            IWebElement ClickMenu = driver.FindElement(By.Id("open-category-panel-button"));
            ClickMenu.Click();

            Thread.Sleep(4000);

            IWebElement QuickOrderBtn = driver.FindElement(By.Id("toggleQuickOrder"));
            QuickOrderBtn.Click();
            Thread.Sleep(3000);

            Assert.AreEqual(mainURLs + "quick-order", driver.Url);

            InputAndCheckAdd("3244", "Profile Orthosis 2XL");
            InputAndCheckAdd("3245", "Profile Orthosis 3XL");
            InputAndCheckAdd("1211", "Splint SM Left");
            InputAndCheckAdd("1212", "Splint MD Left");
            InputAndCheckAdd("3243", "Profile Orthosis XL");
            InputAndCheckAdd("1213", "Splint LG Left");
            InputAndCheckAdd("3241", "Orthosis MD");
            InputAndCheckAdd("3239", "Orthosis XS");
            InputAndCheckAdd("3231", "Orthosis 10");

            Thread.Sleep(3000);

            helperTest.JsClickElement(driver, "//*[text()='" + " Add to Cart " + "']");

            Thread.Sleep(5000);

            Assert.AreEqual(mainURLs + "cart/index", driver.Url);

            helperTest.waitElementId(driver, 60, "submit_order");

            String bodyTextCart2 = driver.FindElement(By.TagName("body")).Text;

            Thread.Sleep(3000);

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

            Thread.Sleep(1000);

            for (int j = 1; j < 10; j++)
            {
                helperTest.JsClickElement(driver, "/html/body/app-root/div/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-approver/section/section/article/div[2]/app-cart-product-order-for-approver[1]/section/article[4]/div[3]/div/app-tag-button[1]/span/span");

                Thread.Sleep(1000);
            }
        }

        [Test]
        public void MobileShoppingList()
        {
            helperTest.LoginToSiteMobile(driver, authUrl, homeUrl, login, password, mainURL);

            Thread.Sleep(4000);
            helperTest.waitElementId(driver, 60, "open-category-panel-button");
            IWebElement ClickMenu = driver.FindElement(By.Id("open-category-panel-button"));
            ClickMenu.Click();

            Thread.Sleep(2000);

            helperTest.JsClickElementID(driver, "shopping-lists");

            Thread.Sleep(4000);

            Assert.AreEqual(mainURLs + "shopping/list", driver.Url);

            helperTest.JsClickElement(driver, "//*[text()='" + "Create new list" + "']");

            helperTest.InputStringXpath(driver, "list1", "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/app-create-list-modal/div/div/div[2]/div[1]/div[2]/input");
            helperTest.InputStringXpath(driver, "description if list 1", "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/app-create-list-modal/div/div/div[2]/div[2]/div[2]/textarea");

            //helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/app-create-list-modal/div/div/div[2]/div[3]/mdb-checkbox/div/label");
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/app-create-list-modal/div/div/div[3]/button");

            Thread.Sleep(4000);

            helperTest.JsClickElement(driver, "//*[text()='" + "Create new list" + "']");

            helperTest.InputStringXpath(driver, "list2", "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/app-create-list-modal/div/div/div[2]/div[1]/div[2]/input");
            helperTest.InputStringXpath(driver, "description if list 2", "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/app-create-list-modal/div/div/div[2]/div[2]/div[2]/textarea");

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/app-create-list-modal/div/div/div[3]/button");

            driver.Url = mainURLs + "product?productID=7254";

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[1]/div/div[4]/div/span/div/div/span");

            helperTest.JsClickElement(driver, "//*[text()='" + "list1" + "']");

            Thread.Sleep(4000);

            driver.Url = mainURLs + "product?productID=15048";

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[1]/div/div[4]/div/span/div/div/span");

            helperTest.JsClickElement(driver, "//*[text()='" + "list1" + "']");

            Thread.Sleep(4000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/app-added-to-list-modal/div/div/div[3]/button[1]");

            Thread.Sleep(8000);

            String bodyTextCart2 = driver.FindElement(By.TagName("body")).Text;

            Assert.IsTrue(bodyTextCart2.Contains("112-10"));

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[2]/div/div/app-my-current-list[2]/div[1]/section/div[4]/span/div/span/i");
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[2]/div/div/app-my-current-list[2]/div[1]/section/div[4]/span/div/div/span/app-tag-button/span");

            bodyTextCart2 = driver.FindElement(By.TagName("body")).Text;

            Assert.IsFalse(bodyTextCart2.Contains("62471-AM"));

            //helperTest.InputStringXpath(driver, "10", "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[2]/div/div[2]/app-my-current-list[1]/section/div[3]/div[1]/app-qty/input");

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[2]/div/div/app-my-current-list/div[1]/section/div[4]/app-button/div/button");

            Thread.Sleep(8000);

            driver.Url = mainURLs + "product?productID=15048";

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[1]/div/div[4]/div/span/div/div/span");

            helperTest.JsClickElement(driver, "//*[text()='" + "list2" + "']");

            Thread.Sleep(4000);

            driver.Url = mainURLs + "product?productID=7254";

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/div[1]/div/div[4]/div/span/div/div/span");

            helperTest.JsClickElement(driver, "//*[text()='" + "list2" + "']");

            Thread.Sleep(4000);

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-product/app-added-to-list-modal/div/div/div[3]/button[1]");

            Thread.Sleep(8000);

            bodyTextCart2 = driver.FindElement(By.TagName("body")).Text;

            Assert.IsTrue(bodyTextCart2.Contains("62471-AM"));

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[2]/div/div/app-my-current-list[2]/div[1]/section/div[4]/span/div/span/i");
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[2]/div/div/app-my-current-list[2]/div[1]/section/div[4]/span/div/div/span/app-tag-button/span");

            bodyTextCart2 = driver.FindElement(By.TagName("body")).Text;

            Assert.IsFalse(bodyTextCart2.Contains("112-10"));

            //helperTest.InputStringXpath(driver, "10", "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[2]/div/div[2]/app-my-current-list[1]/section/div[3]/div[1]/app-qty/input");

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[2]/div/div/app-my-current-list/div[1]/section/div[4]/app-button/div/button");

            Thread.Sleep(8000);

            bodyTextCart2 = driver.FindElement(By.XPath("/html/body/app-root/div/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-approver/section/section/article/div[2]/app-cart-product-order-for-approver[1]/section/article[1]/app-product-card/article/div[2]/p[2]")).Text;
            Assert.IsTrue(bodyTextCart2.Contains("62471-AM"));

            Thread.Sleep(1000);

            bodyTextCart2 = driver.FindElement(By.XPath("/html/body/app-root/div/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-approver/section/section/article/div[2]/app-cart-product-order-for-approver[2]/section/article[1]/app-product-card/article/div[2]/p[2]")).Text;
            Assert.IsTrue(bodyTextCart2.Contains("112-10"));

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-approver/section/section/article/div[2]/app-cart-product-order-for-approver[1]/section/article[4]/div[3]/div/app-tag-button[1]/span");
            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-cart-root/div/div/app-shopping-cart/app-shopping-cart-approver/section/section/article/div[2]/app-cart-product-order-for-approver[1]/section/article[4]/div[3]/div/app-tag-button[1]/span");

            //IWebElement ClickUser2 = driver.FindElement(By.Id("username_button"));
            //Actions acti = new Actions(driver);
            //acti.MoveToElement(ClickUser2).Build().Perform();

            //helperTest.JsClickElement(driver, "//*[text()='" + "Shopping Lists" + "']");

            //Thread.Sleep(4000);

            //IWebElement RemoveList1 = driver.FindElement(By.XPath("/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/mdb-card/div/mdb-card-body/mdb-card-text/p/div/div/span[2]/i"));
            //Actions RemoveTheList1 = new Actions(driver);
            //RemoveTheList1.MoveToElement(RemoveList1).Build().Perform();
            //helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/mdb-card/div/mdb-card-body/mdb-card-text/p/div/div/span[2]/i");

            //IWebElement RemoveList2 = driver.FindElement(By.XPath("/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/mdb-card/div/mdb-card-body/mdb-card-text/p/div/div/span[1]/i"));
            //Actions RemoveTheList2 = new Actions(driver);
            //RemoveTheList2.MoveToElement(RemoveList2).Build().Perform();
            //helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-shopping-list/div/div/div[1]/app-my-shopping-list/mdb-card/div/mdb-card-body/mdb-card-text/p/div/div/span[1]/i");

            //Thread.Sleep(4000);
        }

        [Test]
        public void MobileSubmitOrder()
        {
            helperTest.LoginToSiteMobile(driver, authUrl, homeUrl, login, password, mainURL);

            driver.Url = mainURLs + "product?productID=1528";

            Thread.Sleep(3000);

            Assert.AreEqual(mainURLs + "product?productID=1528", driver.Url);

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-product/div[1]/div/p[1]");

            String bodyTextProduct = driver.FindElement(By.XPath("/html/body/app-root/div/app-product/div[1]/div/p[1]")).Text;

            Assert.IsTrue(bodyTextProduct.Contains("Attachment Kits 650 AK"));

            helperTest.JsClickElementId(driver, "mobile-add-to-cart");

            Thread.Sleep(5000);

            helperTest.waitElementId(driver, 60, "submit_order");

            Assert.AreEqual(mainURLs + "cart/index", driver.Url);

            IWebElement InpBox = driver.FindElement(By.Id("poNumber"));

            InpBox.Clear();
            InpBox.SendKeys("TESTPO " + DateTime.Now.ToString("yyyyMMdd"));

            IWebElement InpBox2 = driver.FindElement(By.Id("notesInput"));
            InpBox2.Clear();
            InpBox2.SendKeys("TEST ORDER PLEASE DO NOT PROCESS " + DateTime.Now.ToString("yyyyMMdd"));

            Thread.Sleep(1000);

            helperTest.JsClickElementId(driver, "submit_order");

            helperTest.waitElementXpath(driver, 180, "/html/body/app-root/div/app-cart-root/div/div/app-review-cart/section/section/article/div[2]/app-cart-product-order/section/article[1]/app-product-card/article/div[2]/p[1]");

            Assert.AreEqual(mainURLs + "cart/review", driver.Url);

            Thread.Sleep(3000);

            helperTest.JsClickElementId(driver, "submit_order");

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-cart-root/div/div/app-review-cart/div[1]/div/div/div[2]/div/span[1]");

            helperTest.FindTextInBody(driver, "Thank you for your order. Your order number is");

            Thread.Sleep(4000);

            helperTest.JsClickElement(driver, "//*[text()='" + " OK " + "']");

            Thread.Sleep(4000);
        }

        [Test]
        public void MobileSubmitRMAs()
        {
            helperTest.LoginToSiteMobile(driver, authUrl, homeUrl, login, password, mainURL);

            helperTest.waitElementId(driver, 60, "search");

            Assert.AreEqual(homeUrl, driver.Url);

            driver.Url = mainURLs + "shopping/order-history?tab=orders&page=1";

            Thread.Sleep(1000);

            Assert.AreEqual(mainURLs + "shopping/order-history?tab=orders&page=1", driver.Url);

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-order-history/section/section/app-history-order-item[1]/article/article/section/div[2]/section/div[2]/div[2]/app-button[2]/div/button");

            helperTest.InputStringXpath(driver, "123456", "/html/body/app-root/div/app-main/div/app-order-history/app-rma-modal/section/div/div[2]/div[1]/section/form/div[3]/div[2]/input");
            helperTest.InputStringXpath(driver, "1234567890", "/html/body/app-root/div/app-main/div/app-order-history/app-rma-modal/section/div/div[2]/div[1]/section/form/div[4]/div[2]/input");

            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-main/div/app-order-history/app-rma-modal/section/div/div[2]/div[1]/section/form/div[5]/div[2]/select", 2);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-main/div/app-order-history/app-rma-modal/section/div/div[2]/div[1]/section/form/div[6]/div[2]/select", 3);
            helperTest.UseDropDown(driver, "/html/body/app-root/div/app-main/div/app-order-history/app-rma-modal/section/div/div[2]/div[1]/section/form/div[7]/div[2]/select", 5);

            helperTest.InputStringXpath(driver, "Broken", "/html/body/app-root/div/app-main/div/app-order-history/app-rma-modal/section/div/div[2]/div[1]/section/form/div[8]/div[2]/textarea");

            IWebElement InpBox2 = driver.FindElement(By.XPath("/html/body/app-root/div/app-main/div/app-order-history/app-rma-modal/section/div/div[2]/div[1]/section/form/div[10]/div[2]/textarea"));
            InpBox2.Clear();
            InpBox2.SendKeys("TEST RMA PLEASE DO NOT PROCESS " + DateTime.Now.ToString("yyyyMMdd"));

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-order-history/app-rma-modal/section/div/div[2]/div[2]/div/button");

            Thread.Sleep(5000);

            helperTest.FindTextInBody(driver, "Success");

            helperTest.JsClickElement(driver, "//*[text()='" + " Submit for Return " + "']");

            helperTest.waitElementXpath(driver, 60, "/html/body/app-root/div/app-main/div/app-order-history/div[4]/div/div/div[3]/app-button/div/button");

            Thread.Sleep(3000);

            helperTest.FindTextInBody(driver, "Thank you for your submission");

            helperTest.JsClickElement(driver, "/html/body/app-root/div/app-main/div/app-order-history/div[4]/div/div/div[3]/app-button/div/button");
        }

        [TearDown]
        public void Cleanup()
        {
            //driver.Quit();
            driver?.Dispose();

        }
    }
}
