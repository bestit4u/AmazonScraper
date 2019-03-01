using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Drawing;
using Excel;
using PCMSnotification.Properties;

using HtmlAgilityPack;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

using Selenium;
using Selenium.Internal.SeleniumEmulation;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PCMSnotification
{
    public class FileWriter
    {
        private string filepath = string.Empty;
        public FileWriter(string _filepath)
        {
            filepath = _filepath;
        }
        private ReaderWriterLockSlim lock_ = new ReaderWriterLockSlim();
        public bool WriteData(string writestr)
        {
            bool result = true;
            lock_.EnterWriteLock();
            try
            {
                StreamWriter sw = new StreamWriter(filepath, true, Encoding.Unicode);
                sw.WriteLine(writestr);
                sw.Close();
            }
            catch
            {
                result = false;
            }
            finally
            {
                lock_.ExitWriteLock();
            }
            return result;
        }

    } // eo 
    public class Scrape
    {
        private onWriteStatusEvent onWriteStatus = null;
        private onSetProgressEvent onSetProgress = null;
        private List<string> dataSet = null;
        private bool useProxy;
        private List<string> proxylist = null;
        public List<PcmDataSet> saveDataSet = null;
        private List<string> prices = null;
        private List<string> sellers = null;
        private string curproxy = string.Empty;
        public HttpClient httpClient;
        private HttpClientHandler handler = null;
        private CookieContainer container = null;
        private int ThreadIndex = 0;
        ProgressInfo proinfo;
        
        private int loopcount = 20;
        private int delay = 0;
        private IWebDriver driver = null;

        public Scrape(int _threadindex, onWriteStatusEvent _onWriteStatus, onSetProgressEvent _onSetProgress, List<string> _dataSet,int _delay,bool _useProxy,List<string>_proxylist)
        {
            onWriteStatus = _onWriteStatus;
            onSetProgress = _onSetProgress;
            dataSet = _dataSet;
            useProxy = _useProxy;
            proxylist = _proxylist;
            delay = _delay;
            ThreadIndex = _threadindex;
            saveDataSet = new List<PcmDataSet>();
            proinfo.ThreadIndex = _threadindex;
            handler = new HttpClientHandler();
            container = new CookieContainer();
            handler.CookieContainer = container;
            httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2272.101 Safari/537.36");
            
            //Init();
            //InitPhantomJS();
        }

        #region PreInit
        private bool testproxy(string proxyinfo)
        {
            bool ret = false;
            if (proxyinfo == "localhost")
                return true;
            HttpClientHandler handler = new HttpClientHandler()
            {
                Proxy = new WebProxy("http://" + proxyinfo),
                UseProxy = true,
            };
            HttpClient client = new HttpClient(handler);
            try
            {
                //var byteArray = Encoding.ASCII.GetBytes("username:password1234");
                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                HttpResponseMessage response = client.GetAsync("https://google.com").Result;
                // ... Check Status Code                                
                if ((int)response.StatusCode == 200)
                    ret = true;
            }
            catch (Exception e)
            {
                string error = e.Message;
            }
            return ret;
        }
        private int SetProxy(bool refresh,string redircturl)
        {
            try
            {
                ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
                chromeDriverService.HideCommandPromptWindow = true;
                ChromeOptions option = new ChromeOptions();
                option.AddArgument("--start-maximized");
                if (!useProxy)
                {
                    if (driver == null)
                    {
                        driver = new ChromeDriver(chromeDriverService, option);
                        driver.Manage().Window.Position = new Point(-2000, 0);
                    }
                    driver.Navigate().GoToUrl(redircturl);
                    if (refresh)
                        driver.Navigate().Refresh();
                    WaitForPageLoad(180);

                    return 1;
                }
                bool proxycheckresult = false;
                int totalcount = proxylist.Count;
                int proxycount = 0;
                while (!proxycheckresult)
                {
                    if (++proxycount > 100)
                        return 2;
                    Random rnd = new Random();
                    int proxyindex = rnd.Next(totalcount);
                    curproxy = proxylist[proxyindex];
                    proxycheckresult = testproxy(curproxy);
                }
                string message = string.Empty;
                if (driver != null)
                {
                    try
                    {
                        driver.Quit();
                        Thread.Sleep(1000);
                        message = "Quit";
                    }
                    catch (Exception e)
                    {
                        string error = e.Message;
                    }
                }
                if (curproxy != "localhost")
                {
                    var proxy = new Proxy();
                    proxy.HttpProxy = curproxy;
                    proxy.SslProxy = curproxy;

                    option.Proxy = proxy;
                }
                message += " and create new one with " + curproxy + " in thread " + ThreadIndex.ToString();
                logging("proxylog.txt",message);
                driver = new ChromeDriver(chromeDriverService, option);
                driver.Manage().Window.Position = new Point(-2000, 0);
                driver.Navigate().GoToUrl(redircturl);
                WaitForPageLoad(180);
                return 1;
            }
            catch (Exception e)
            {
                string error = e.Message;
            }
            return 1;
        }
        public bool RefreshBrowser()
        {
            try
            {
                driver.Navigate().Refresh();
                return true;
            }
            catch (Exception e)
            {
                string error = e.Message;
                return false;
            }
        }
        private static void logging(string filename, string str)
        {
            string confpath = System.Windows.Forms.Application.StartupPath + @"\" + filename;
            FileStream fs = new FileStream(confpath, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            sw.WriteLine(str);
            sw.Flush();
            sw.Close();
            fs.Close();
        }
        private void Init()
        {
            ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            ChromeOptions option = new ChromeOptions();
            option.AddArgument("--start-maximized");
            //option.AddArgument("--window-position=2000,3000");
            driver = new ChromeDriver(chromeDriverService, option);
            driver.Manage().Window.Position = new Point(-2000, 0);
        }

        private void InitPhantomJS()
        {
            PhantomJSDriverService phantomjsDriverService = PhantomJSDriverService.CreateDefaultService();
            phantomjsDriverService.HideCommandPromptWindow = true;
            ChromeOptions option = new ChromeOptions();
            //option.AddArgument("--start-maximized");
            driver = new PhantomJSDriver(phantomjsDriverService);
        }

        
        private void doClick(IWebElement element, int time = 500)
        {
            Actions action = new Actions(driver);
            action.MoveToElement(element).Click().Perform();
            Thread.Sleep(time);
        }

        private void ExitProcess(string procName)
        {
            try
            {
                Process[] liveProcess = Process.GetProcesses();
                if (liveProcess == null)
                    return;

                foreach (Process proc in liveProcess)
                {
                    if (proc.ProcessName == procName)
                        proc.Kill();
                }
            }
            catch (Exception)
            {

            }
        }

        public void WaitForPageLoad(int maxWaitTimeInSeconds)
        {
            string state = string.Empty;
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(maxWaitTimeInSeconds));

                //Checks every 500 ms whether predicate returns true if returns exit otherwise keep trying till it returns ture


                wait.Until(d =>
                {
                    try
                    {
                        state = ((IJavaScriptExecutor)driver).ExecuteScript(@"return document.readyState").ToString();
                    }
                    catch (InvalidOperationException)
                    {
                        //Ignore
                    }
                    catch (NoSuchWindowException)
                    {
                        //when popup is closed, switch to last windows
                        driver.SwitchTo().Window(driver.WindowHandles.Last());
                    }
                    //In IE7 there are chances we may get state as loaded instead of complete
                    return (state.Equals("complete", StringComparison.InvariantCultureIgnoreCase) || state.Equals("loaded", StringComparison.InvariantCultureIgnoreCase));
                });
            }
            catch (TimeoutException)
            {
                //sometimes Page remains in Interactive mode and never becomes Complete, then we can still try to access the controls 

                //if (!state.Equals("interactive", StringComparison.InvariantCultureIgnoreCase))
                //    throw;
            }
            catch (NullReferenceException)
            {
                //sometimes Page remains in Interactive mode and never becomes Complete, then we can still try to access the controls 

                //if (!state.Equals("interactive", StringComparison.InvariantCultureIgnoreCase))

                //    throw;
            }
            catch (WebDriverException)
            {
                if (driver.WindowHandles.Count == 1)
                {
                    driver.SwitchTo().Window(driver.WindowHandles[0]);
                }

                state = ((IJavaScriptExecutor)driver).ExecuteScript(@"return document.readyState").ToString();

                //if (!(state.Equals("complete", StringComparison.InvariantCultureIgnoreCase) || state.Equals("loaded", StringComparison.InvariantCultureIgnoreCase)))
                //    throw;
            }
        }

        private bool WaitForTagVisible(By by, int waitTime = 20)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(waitTime));
                wait.PollingInterval = TimeSpan.FromSeconds(1);
                wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(NoSuchFrameException));
                wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException), typeof(StaleElementReferenceException));
                wait.IgnoreExceptionTypes(typeof(ArgumentOutOfRangeException));
                wait.Until(d =>
                {
                    try
                    {
                        IWebElement element = driver.FindElement(by);
                        Thread.Sleep(2000);
                        return element;
                    }
                    catch (Exception)
                    {

                    }

                    IWebElement divElement = driver.FindElement(by);
                    if (divElement.Displayed)
                        return divElement;

                    throw new NoSuchElementException();
                });

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private IWebElement getElementBy(IWebDriver driver, By by)
        {
            try
            {
                IWebElement element = driver.FindElement(by);
                return element;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private IWebElement getElementBy(IWebElement ele, By by)
        {
            try
            {
                IWebElement element = ele.FindElement(by);
                return element;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void waitForTagDisplayed(By by)
        {
            try
            {
                int total = 0;
                while (true) 
                {
                    int count = 0;
                    IWebElement element = driver.FindElement(by);
                    while (element == null || !element.Displayed)
                    {
                        if (++count > 20)
                        {
                            count = 0;
                            if (++total > 3)
                                return;
                            driver.Navigate().Refresh();
                        }
                        Thread.Sleep(500);
                    }
                    WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 5));
                    wait.Until(ExpectedConditions.ElementToBeClickable(by));
                    break;
                }
            }
            catch (Exception e)
            {
                string error = e.Message;
                Thread.Sleep(3000);
            }
        }

        private void doClickEx(IWebElement element, By by)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 5));
                wait.Until(ExpectedConditions.ElementToBeClickable(by));

                element.Click();
            }
            catch (Exception e)
            {

            }
        }

        public void ScrollTo(int xPosition = 0, int yPosition = 0)
        {
            var js = String.Format("window.scrollTo({0}, {1})", xPosition, yPosition);
            ((IJavaScriptExecutor)driver).ExecuteScript(js);
        }

        public void doScrollClick(IWebElement element)
        {
            Actions action = new Actions(driver);
            action.MoveToElement(element).Perform();
            ScrollTo(element.Location.X, element.Location.Y + 100);
            Thread.Sleep(500);
            element.Click();
            Thread.Sleep(1000);
        }

        public void doRelease()
        {
            try
            {
                if (driver != null)
                    driver.Close();
            }
            catch (Exception e)
            {

            }
        }
        #endregion
        private bool doLogin()
        {
            try
            {
                driver.Navigate().GoToUrl("https://www.google.com/shopping?hl=en");
                WaitForPageLoad(180);
                IWebElement iteminput = getElementBy(driver, By.Id("gbqfq"));
                if (iteminput == null)
                    return false;
                //WaitForTagVisible(By.Id("txtLoginID"));

                //IWebElement eUsername = getElementBy(driver, By.Id("txtLoginID"));
                //if (eUsername == null)
                //    return false;

                //IWebElement ePassword = getElementBy(driver, By.Id("txtLoginPwd"));
                //if (ePassword == null)
                //    return false;

                //eUsername.SendKeys(Setting.Instance.username);
                //Thread.Sleep(500);

                //ePassword.SendKeys(Setting.Instance.password);
                //Thread.Sleep(500);

                //IList<IWebElement> lButtons = driver.FindElements(By.ClassName("tdBody"));
                //if (lButtons == null || lButtons.Count < 1)
                //    return false;

                //IWebElement eCorrectButton = null;
                //foreach(IWebElement eButton in lButtons)
                //{
                //    string value = eButton.GetAttribute("value");
                //    if (string.IsNullOrEmpty(value))
                //        continue;

                //    if(value == "Go")
                //    {
                //        eCorrectButton = eButton;
                //        break;
                //    }
                //}

                //if (eCorrectButton == null)
                //    return false;

                //doClick(eCorrectButton);
                //WaitForPageLoad(180);

                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        private bool doShowSearch()
        {
            try
            {
                IWebElement eFrame = getElementBy(driver, By.Id("tclitewin"));
                if (eFrame == null)
                    return false;

                driver.SwitchTo().Frame(eFrame);

                WaitForTagVisible(By.Id("quoteScreen-normal-body"));
                WaitForTagVisible(By.ClassName("x-grid-item-container"));

                IWebElement eGridContainer = getElementBy(driver, By.ClassName("x-grid-item-container"));
                if (eGridContainer == null)
                    return false;

                Thread.Sleep(20000);

                IWebElement eStockInfo = getElementBy(driver, By.Id("tbStkInfo"));
                if (eStockInfo == null)
                    return false;

                doClick(eStockInfo);

                IList<IWebElement> lMenus = driver.FindElements(By.CssSelector("span[class='x-menu-item-text x-menu-item-text-default x-menu-item-indent']"));
                if (lMenus == null || lMenus.Count < 1)
                    return false;

                IWebElement eCorrectMenu = null;
                foreach (IWebElement eMenu in lMenus)
                {
                    string caption = eMenu.Text.Trim();
                    if (caption == "Stock Tracker")
                    {
                        eCorrectMenu = eMenu;
                        break;
                    }
                }

                if (eCorrectMenu == null)
                    return false;

                doClick(eCorrectMenu);
                WaitForPageLoad(180);

                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        private bool doSearch(string col, ref string buyRate, ref string volume)
        {
            try
            {
                IList<IWebElement> lSearchInputs = driver.FindElements(By.CssSelector("input[class='x-form-field x-form-empty-field x-form-empty-field-default x-form-text x-form-text-toolbar  ']"));
                if (lSearchInputs == null || lSearchInputs.Count < 1)
                    return false;

                IWebElement eCorrectInput = null;

                foreach(IWebElement eInput in lSearchInputs)
                {
                    if(eInput.Displayed)
                    {
                        eCorrectInput = eInput;
                        break;
                    }
                }

                if (eCorrectInput == null)
                    return false;

                eCorrectInput.SendKeys(col);
                Thread.Sleep(500);

                eCorrectInput.SendKeys(Keys.Enter);

                //IList<IWebElement> lSearchBtns = driver.FindElements(By.CssSelector("a[class='x-btn x-unselectable x-box-item x-toolbar-item x-btn-default-toolbar-small']"));
                //if (lSearchBtns == null || lSearchBtns.Count < 1)
                //    return false;

                //IWebElement eCorrectSearchBtn = null;
                //foreach(IWebElement eSearchBtn in lSearchBtns)
                //{
                //    if (!eSearchBtn.Displayed)
                //        continue;

                //    IWebElement eSearchSpan = getElementBy(eSearchBtn, By.CssSelector("span[class='x-btn-inner x-btn-inner-default-toolbar-small']"));
                //    if (eSearchSpan == null)
                //        continue;

                //    string caption = eSearchSpan.Text.Trim();
                //    if (caption.Contains("Search"))
                //    {
                //        eCorrectSearchBtn = eSearchBtn;
                //        break;
                //    }
                //}

                //if (eCorrectSearchBtn == null)
                //    return false;

                //doClick(eCorrectSearchBtn);
                WaitForPageLoad(10);
                Thread.Sleep(1000);

                IWebElement eDataView = getElementBy(driver, By.CssSelector("div[class='x-component  x-fit-item x-component-default']"));
                if (eDataView == null)
                    return false;

                IWebElement eTable = getElementBy(eDataView, By.TagName("table"));
                if (eTable == null)
                    return false;

                doClick(eTable);
                WaitForPageLoad(10);
                Thread.Sleep(1000);

                IWebElement eCorrectSummary = null;
                IList<IWebElement> lTabs = driver.FindElements(By.CssSelector("a[class='x-tab  x-unselectable x-box-item x-tab-default x-top x-tab-top x-tab-default-top']"));
                foreach(IWebElement eTab in lTabs)
                {
                    string summary = eTab.Text.Trim();
                    if(summary == "Summary")
                    {
                        eCorrectSummary = eTab;
                        break;
                    }
                }

                if (eCorrectSummary == null)
                    return false;

                doClick(eCorrectSummary);
                WaitForPageLoad(10);
                Thread.Sleep(1000);

                IWebElement eDataTableDiv = getElementBy(driver, By.CssSelector("div[class='x-panel-body x-panel-body-default x-table-layout-ct x-panel-body-default x-docked-noborder-top x-docked-noborder-right x-docked-noborder-bottom x-docked-noborder-left']"));
                if (eDataTableDiv == null)
                    return false;

                IWebElement eDataTable = getElementBy(eDataTableDiv, By.TagName("table"));
                if (eDataTable == null)
                    return false;

                IList<IWebElement> lTds = eDataTable.FindElements(By.TagName("td"));
                if (lTds == null || lTds.Count < 2)
                    return false;

                for (int i = 0; i < lTds.Count - 1; i += 2)
                {
                    string title = lTds[i].Text.Trim();
                    string value = lTds[i + 1].Text.Trim();

                    if (title == "Buy Rate")
                        buyRate = value;

                    if (title == "Volume")
                        volume = value;
                }

                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
        private bool CompanyCheck(string company)
        {
            if (company == "" || company == " " || company == null)
                return false;
            int n;
            string first = company.Substring(0,1);
            if (int.TryParse(first, out n))
                return false;
            return true;
        }
        private void GoToShopping(string item)
        {
            bool result = doLogin();
            int count = 0;
            while (!result)
            {
                Thread.Sleep(1000);
                if (++count > 300)
                    return;
            }
            IWebElement iteminput = getElementBy(driver, By.Id("gbqfq"));
            count = 0;
            while (iteminput == null)
            {
                if (++count > loopcount)
                    break;
                Thread.Sleep(500);
                iteminput = getElementBy(driver, By.Id("gbqfq"));
            }
            if (iteminput == null)
                return;
            iteminput.Clear();
            iteminput.SendKeys(item);
            IWebElement btnfind = getElementBy(driver, By.Id("gbqfb"));
            count = 0;
            while (btnfind == null)
            {
                if (++count > loopcount)
                    break;
                Thread.Sleep(500);
                btnfind = getElementBy(driver, By.Id("gbqfq"));
            }
            if (btnfind == null)
                return;
            doClick(btnfind);
        }
        private bool CheckCaptcha()
        {
            try
            {
                bool result = true;
                IWebElement captchacharacters = getElementBy(driver, By.Id("captchacharacters"));
                if (captchacharacters == null)
                    return true;
                int repeatedcount = 0;
                while (captchacharacters != null)
                {
                    if (++repeatedcount > 10)
                    {
                        result = false;
                        break;
                    }
                    Thread.Sleep(3000);
                    IWebElement parent = captchacharacters.FindElement(By.XPath("../.."));
                    if (parent == null)
                        continue;
                    IWebElement imgtag = parent.FindElement(By.TagName("img"));
                    string imgurl = string.Empty;
                    string captchacode = string.Empty;
                    if (imgtag != null)
                        imgurl = imgtag.GetAttribute("src");
                    if (imgurl != "")
                        captchacode = trySolveCaptchaUpdate(imgurl);
                    captchacharacters.Clear();
                    captchacharacters.SendKeys(captchacode);
                    IList<IWebElement> buttons = driver.FindElements(By.TagName("button"));
                    foreach (IWebElement buttonone in buttons)
                    {
                        if (buttonone.Text == "Continue shopping")
                        {
                            doClick(buttonone);
                            break;
                        }

                    }
                    captchacharacters = driver.FindElement(By.Id("captchacharacters"));
                }
                string redircturl = string.Empty;
                if (captchacharacters == null)
                    redircturl = driver.Url;
                else
                    redircturl = "https://www.amazon.com";
                SetProxy(true,redircturl);
                return result;
            }
            catch (Exception e)
            {
                string error = e.Message;
                return false;
            }
        }
        private string trySolveCaptchaUpdate(string imgurl)
        {
            string response = string.Empty;
            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html, application/xhtml+xml, application/xml; q=0.9, image/webp, */*; q=0.8");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.111 Safari/537.36");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Upgrade-Insecure-Requests", "1");

                MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent("----WebKitFormBoundaryQRIA65tkuSBONMDE");
                multipartFormDataContent.Headers.Clear();
                multipartFormDataContent.Headers.Add("Content-Type", "multipart/form-data; boundary=----WebKitFormBoundaryQRIA65tkuSBONMDE");
                multipartFormDataContent.Add((HttpContent)new ByteArrayContent(Encoding.Default.GetBytes("post")), "method");
                multipartFormDataContent.Add((HttpContent)new ByteArrayContent(Encoding.Default.GetBytes(Setting.Instance.captchaKey)), "key");
                StreamContent streamContent;
                if (imgurl.IndexOf("http") == 0)
                {
                    WebRequest req = HttpWebRequest.Create(imgurl);
                    Stream stream = req.GetResponse().GetResponseStream();
                    streamContent = new StreamContent(stream);
                }
                else 
                {
                    streamContent = new StreamContent((Stream)File.OpenRead(imgurl));
                }
                
                streamContent.Headers.Clear();
                streamContent.Headers.Add("Content-Disposition", "form-data; name=\"file\"; filename=\"captcha.png\"");
                streamContent.Headers.Add("Content-Type", "image/png");
                multipartFormDataContent.Add(streamContent);


                HttpResponseMessage responseMessageCaptcha = httpClient.PostAsync("http://2captcha.com/in.php", (HttpContent)multipartFormDataContent).Result;
                responseMessageCaptcha.EnsureSuccessStatusCode();


                string captchaID = responseMessageCaptcha.Content.ReadAsStringAsync().Result;
                if (!captchaID.Contains("OK|"))
                    return response;


                captchaID = captchaID.Replace("OK|", "");
                if (string.IsNullOrEmpty(captchaID))
                    return response;


                int nCnt = 0;
                while (nCnt < 20)
                {
                    nCnt++;
                    Thread.Sleep(5000);


                    responseMessageCaptcha = httpClient.GetAsync(string.Format("http://2captcha.com/res.php?key={0}&action=get&id={1}", Setting.Instance.captchaKey, captchaID)).Result;
                    responseMessageCaptcha.EnsureSuccessStatusCode();


                    response = responseMessageCaptcha.Content.ReadAsStringAsync().Result;
                    if (string.IsNullOrEmpty(response))
                        continue;


                    if (!response.Contains("OK|"))
                        continue;


                    response = response.Replace("OK|", "");
                    break;
                }


                return response;
            }
            catch (Exception e)
            {
                string error = e.Message;
                return response;
            }
        }
        
        private void SetHttpClientCookies()
        {
            try
            {
                foreach (OpenQA.Selenium.Cookie cookie in driver.Manage().Cookies.AllCookies)
                    container.Add(new Uri("https://www.amazon.com/"), new System.Net.Cookie(cookie.Name, cookie.Value));
            }
            catch(Exception)
            {

            }
        }
        private string elematespace(string native)
        {
            string ret = string.Empty;
            string[] tmp = native.Split(' ');
            foreach(string tmpone in tmp)
            {
                if (tmpone != "")
                    ret += tmpone + " ";
            }
            if (ret.Length > 0)
                ret = ret.Substring(0,ret.Length-1);
            return ret;
        }
        private string GetBrandMark(string url)
        {
            string brand = string.Empty;
            try
            {
                driver.Navigate().GoToUrl(url);
                string content = driver.PageSource;
                string brandreg = @"=""brand-store-title[^\>]*>(?<VAL>[^\<]*)";
                Match brandmatch = Regex.Match(content, brandreg);
                string brandtmp = brandmatch.Groups["VAL"].Value.ToString();
                brand = elematespace(brandtmp);
            }
            catch (Exception e)
            {
                string error = e.Message;
            }
            return brand;
        }
        private PcmDataSet GetProductInfo(string url, string proxy)
        {
            PcmDataSet sellerInfo = new PcmDataSet();
            try
            {
                handler.CookieContainer = container;
                handler.Proxy = new WebProxy("http://" + curproxy);
                handler.UseProxy = true;
                HttpClient client = new HttpClient(handler);
                try
                {
                    //var byteArray = Encoding.ASCII.GetBytes("username:password1234");
                    //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    HttpResponseMessage response = client.GetAsync(url).Result;
                    response.EnsureSuccessStatusCode();
                    string retfirstJScode = response.Content.ReadAsStringAsync().Result;

                    string citynum = @" by <a[^\>]*>(?<VAL>[^\<]*)";
                    MatchCollection hrefarray = Regex.Matches(retfirstJScode, citynum);
                    foreach (Match href in hrefarray)
                    {
                        string sellertmp = href.Groups["VAL"].Value.ToString();
                        if (sellertmp != null || sellertmp != "" || sellertmp != " ")
                        {
                            sellerInfo.seller = sellertmp;
                            break;
                        }
                    }
                    citynum = @"span id=""priceblock_[^\>]*>\$(?<VAL>[^\<]*)";
                    Match pricematch = Regex.Match(retfirstJScode, citynum);
                    string pricetmp = pricematch.Groups["VAL"].Value.ToString();
                    if (pricetmp == "")
                    {
                        citynum = @"Other Sellers on Amazon:[^\$]*\$(?<VAL>[^\<]*)";
                        pricematch = Regex.Match(retfirstJScode, citynum);
                        pricetmp = pricematch.Groups["VAL"].Value.ToString();
                    }
                    if (pricetmp != null || pricetmp != "" || pricetmp != " ")
                        sellerInfo.price = elematespace(pricetmp.Replace("\t", "").Replace("\r", "").Replace("\n", ""));

                    string namereg = @"id=""productTitle[^\>]*>(?<VAL>[^\<]*)";
                    pricematch = Regex.Match(retfirstJScode, namereg);
                    string productname = pricematch.Groups["VAL"].Value.ToString().Replace("\n", "").Replace("\r", "");
                    productname = elematespace(productname);
                    if (productname != null && productname != "")
                        sellerInfo.productname = productname;
                    string brandreg = @"id=""brand""[^\>]*>(?<VAL>[^\<]*)";
                    pricematch = Regex.Match(retfirstJScode, brandreg);
                    string brand = pricematch.Groups["VAL"].Value.ToString();
                    if (brand == "")
                    {
                        brandreg = @"id=""brand[^\/]*/(?<VAL>[^\/]*)";
                        pricematch = Regex.Match(retfirstJScode, brandreg);
                        brand = pricematch.Groups["VAL"].Value.ToString().Replace("_", " ").Replace("-", " ");
                        if (brand == "s")
                        {
                            brandreg = @"id=""brand[^\/]*/(?<VAL>[^\""]*)";
                            pricematch = Regex.Match(retfirstJScode, brandreg);
                            string brandurl = "https://www.amazon.com/" + pricematch.Groups["VAL"].Value.ToString();
                            brand = GetBrandMark(brandurl);
                        }
                    }
                    brand = elematespace(brand.Replace("\n", "").Replace("\r", ""));
                    if (brand != null && brand != "")
                        sellerInfo.brand = brand;

                    if (sellerInfo.seller == "")
                    {
                        citynum = @"id=""olp_feature_div[^\/]*/(?<VAL>[^\'""]*)";
                        pricematch = Regex.Match(retfirstJScode, citynum);
                        string jumpurl = pricematch.Groups["VAL"].Value.ToString();
                        if (jumpurl != "")
                        {
                            PcmDataSet sellerdetail = GetSellerDetail("https://www.amazon.com/" + jumpurl);
                            sellerInfo.seller = sellerdetail.seller;
                            if (sellerdetail.price != "")
                                sellerInfo.price = sellerdetail.price;
                        }
                    }
                    string customerreg = @"id=""acrCustomerReviewText[^\>]*>(?<VAL>[^\ ]*)";
                    pricematch = Regex.Match(retfirstJScode, customerreg);
                    sellerInfo.customerreview = pricematch.Groups["VAL"].Value.ToString().Replace("\n", "").Replace("\t", "");
                    string answeredreg = @"id=""askATFLink[^\>]*>[^\>]*>(?<VAL>[^\<]*)";
                    pricematch = Regex.Match(retfirstJScode, answeredreg);
                    sellerInfo.answeredquestion = Regex.Replace(pricematch.Groups["VAL"].Value.ToString(), @"\D", "");
                }
                catch (Exception e)
                {
                    string error = e.Message;
                }
            }
            catch (Exception e)
            {
                string error = e.Message;
            }
            return sellerInfo;
        }
        private PcmDataSet GetSellerInfo(string url)
        {
            //SetHttpClientCookies();

            PcmDataSet sellerInfo = new PcmDataSet();
            try
            {
                driver.Navigate().GoToUrl(url);
                waitForTagDisplayed(By.Id("productTitle"));
                bool result = CheckCaptcha();
                string retfirstJScode = driver.PageSource;

                string citynum = @" by <a[^\>]*>(?<VAL>[^\<]*)";
                MatchCollection hrefarray = Regex.Matches(retfirstJScode, citynum);
                foreach (Match href in hrefarray)
                {
                    string sellertmp = href.Groups["VAL"].Value.ToString();
                    if (sellertmp != null || sellertmp != "" || sellertmp != " ")
                    {
                        sellerInfo.seller = sellertmp;
                        break;
                    }
                }
                citynum = @"span id=""priceblock_[^\>]*>\$(?<VAL>[^\<]*)";
                Match pricematch = Regex.Match(retfirstJScode, citynum);
                string pricetmp = pricematch.Groups["VAL"].Value.ToString();
                if (pricetmp == "")
                {
                    citynum = @"Other Sellers on Amazon:[^\$]*\$(?<VAL>[^\<]*)";
                    pricematch = Regex.Match(retfirstJScode, citynum);
                    pricetmp = pricematch.Groups["VAL"].Value.ToString();
                }
                if (pricetmp != null || pricetmp != "" || pricetmp != " ")
                    sellerInfo.price = elematespace(pricetmp.Replace("\t", "").Replace("\r", "").Replace("\n", ""));

                string namereg = @"id=""productTitle[^\>]*>(?<VAL>[^\<]*)";
                pricematch = Regex.Match(retfirstJScode, namereg);
                string productname = pricematch.Groups["VAL"].Value.ToString().Replace("\n", "").Replace("\r", "");
                productname = elematespace(productname);
                if (productname != null && productname != "")
                    sellerInfo.productname = productname;
                string brandreg = @"id=""brand""[^\>]*>(?<VAL>[^\<]*)";
                pricematch = Regex.Match(retfirstJScode, brandreg);
                string brand = pricematch.Groups["VAL"].Value.ToString();
                if (brand == "")
                {
                    brandreg = @"id=""brand[^\/]*/(?<VAL>[^\/]*)";
                    pricematch = Regex.Match(retfirstJScode, brandreg);
                    brand = pricematch.Groups["VAL"].Value.ToString().Replace("_", " ").Replace("-", " ");
                    if (brand == "s")
                    {
                        brandreg = @"id=""brand[^\/]*/(?<VAL>[^\""]*)";
                        pricematch = Regex.Match(retfirstJScode, brandreg);
                        string brandurl = "https://www.amazon.com/" + pricematch.Groups["VAL"].Value.ToString();
                        brand = GetBrandMark(brandurl);
                    }
                }
                brand = elematespace(brand.Replace("\n", "").Replace("\r", ""));
                if (brand != null && brand != "")
                    sellerInfo.brand = brand;

                if (sellerInfo.seller == "")
                {
                    citynum = @"id=""olp_feature_div[^\/]*/(?<VAL>[^\'""]*)";
                    pricematch = Regex.Match(retfirstJScode, citynum);
                    string jumpurl = pricematch.Groups["VAL"].Value.ToString();
                    if (jumpurl != "")
                    {
                        PcmDataSet sellerdetail = GetSellerDetail("https://www.amazon.com/" + jumpurl);
                        sellerInfo.seller = sellerdetail.seller;
                        if (sellerdetail.price != "")
                            sellerInfo.price = sellerdetail.price;
                    }
                }
                string customerreg = @"id=""acrCustomerReviewText[^\>]*>(?<VAL>[^\ ]*)";
                pricematch = Regex.Match(retfirstJScode,customerreg);
                sellerInfo.customerreview = pricematch.Groups["VAL"].Value.ToString().Replace("\n", "").Replace("\t", "");
                string answeredreg = @"id=""askATFLink[^\>]*>[^\>]*>(?<VAL>[^\<]*)";
                pricematch = Regex.Match(retfirstJScode,answeredreg);
                sellerInfo.answeredquestion = Regex.Replace(pricematch.Groups["VAL"].Value.ToString(), @"\D", "");
            }
            catch (Exception e)
            {
                string error = e.Message;
            }
            return sellerInfo;
        }
        private PcmDataSet GetSellerDetail(string url)
        {
            PcmDataSet sellerInfo = new PcmDataSet();
            try
            {
                if (prices != null)
                    prices.Clear();
                if (sellers != null)
                    sellers.Clear();
                driver.Navigate().GoToUrl(url);
                waitForTagDisplayed(By.Id("olpOfferList"));
                bool result = CheckCaptcha();
                string retfirstJScode = driver.PageSource;
                string pricereg = @"olpOfferPrice[^\$]*\$(?<VAL>[^\<]*)<";
                string sellerreg = @"olpSellerName""[^\/]*/[^\>*]*>(?<VAL>[^\<]*)";
                MatchCollection hrefarray = Regex.Matches(retfirstJScode, pricereg);
                prices = new List<string>();
                sellers = new List<string>();
                foreach (Match href in hrefarray)
                {
                    string pricetmp = href.Groups["VAL"].Value.ToString().Replace(" ", "");
                    if (pricetmp != null || pricetmp != "" || pricetmp != " ")
                    {
                        prices.Add(pricetmp);
                    }
                }
                hrefarray = Regex.Matches(retfirstJScode, sellerreg);
                foreach (Match href in hrefarray)
                {
                    string pricetmp = href.Groups["VAL"].Value.ToString().Replace(" ", "");
                    if (pricetmp != null || pricetmp != "" || pricetmp != " ")
                    {
                        sellers.Add(pricetmp);
                    }
                }
                if (prices.Count > 0)
                {
                    int index = 0;
                    float floattmp = 0;
                    float minprice = 0;
                    float.TryParse(prices[0], out minprice);
                    for (int i = 1; i < prices.Count; i++ )
                    {
                        bool parseresult = float.TryParse(prices[i], out floattmp);
                        if (parseresult && floattmp < minprice)
                        {
                            minprice = floattmp;
                            index = i;
                        }
                    }
                    if (sellers.Count > index)
                    {
                        sellerInfo.price = prices[index];
                        sellerInfo.seller = sellers[index];
                    }
                }
            }
            catch (Exception e) 
            {
                string error = e.Message;
            }
            return sellerInfo;
        }
        public bool doWorkByName(string savepath) 
        {
            FileWriter writer = new FileWriter(savepath);
            bool refresh = true;
            for (int index = 0; index < dataSet.Count; index++)
            {
                try
                {
                    string setone = dataSet[index];
                    int proxyresult = SetProxy(refresh,setone);
                    int count = 0;
                    while (proxyresult == 2)
                    {
                        if (++count > loopcount)
                            break;
                        Thread.Sleep(500);
                        proxyresult = SetProxy(refresh,setone);
                    }
                    if (proxyresult != 2)
                        refresh = false;
                    else
                        continue;
                    waitForTagDisplayed(By.Id("twotabsearchtextbox"));
                    bool captchasolve = CheckCaptcha();
                    List<string> detailurls = new List<string>();
                    IWebElement pagn = getElementBy(driver, By.Id("pagn"));
                    if (pagn == null)
                        continue;
                    string[] pagestrs = pagn.Text.Replace("Previous Page ", "").Replace(" Next Page", "").Split(' ');
                    string maxstring = string.Empty;
                    if (pagestrs.Length > 0)
                        maxstring = pagestrs[pagestrs.Length - 1];
                    IWebElement curpage = getElementBy(driver, By.ClassName("pagnCur"));
                    if (curpage == null)
                    {
                        try
                        {
                            waitForTagDisplayed(By.ClassName("s-item-container"));
                            IList<IWebElement> items = driver.FindElements(By.ClassName("s-item-container"));

                            foreach (IWebElement itemone in items)
                            {
                                try
                                {
                                    IWebElement detaillink = itemone.FindElement(By.ClassName("s-access-detail-page"));
                                    if (detaillink == null)
                                        continue;
                                    string strdetail = detaillink.GetAttribute("href");
                                    if (strdetail != "")
                                        detailurls.Add(strdetail);
                                }
                                catch (Exception e)
                                {
                                    string error = e.Message;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            string error = e.Message;
                        }
                    }
                    else 
                    {
                        string curstring = curpage.Text;
                        int nMaxPage = 0;
                        int nCurPage = 1;
                        bool parseresult = int.TryParse(maxstring, out nMaxPage);
                        if (!parseresult)
                            continue;
                        parseresult = int.TryParse(curstring, out nCurPage);

                        while (nCurPage <= nMaxPage)
                        {
                            try
                            {
                                waitForTagDisplayed(By.ClassName("s-item-container"));
                                IList<IWebElement> items = driver.FindElements(By.ClassName("s-item-container"));

                                foreach (IWebElement itemone in items)
                                {
                                    try
                                    {
                                        IWebElement detaillink = itemone.FindElement(By.ClassName("s-access-detail-page"));
                                        if (detaillink == null)
                                            continue;
                                        string strdetail = detaillink.GetAttribute("href");
                                        if (strdetail != "")
                                            detailurls.Add(strdetail);
                                    }
                                    catch (Exception e)
                                    {
                                        string error = e.Message;
                                    }
                                }
                                if (nCurPage == nMaxPage)
                                    break;
                                waitForTagDisplayed(By.Id("pagnNextLink"));
                                IWebElement nextlink = getElementBy(driver, By.Id("pagnNextLink"));

                                string nextlinkstr = nextlink.GetAttribute("href");
                                if (nextlinkstr != "" || nextlinkstr == " ")
                                {
                                    driver.Url = nextlinkstr;
                                    driver.Navigate();

                                }
                                waitForTagDisplayed(By.ClassName("pagnCur"));
                                curpage = getElementBy(driver, By.ClassName("pagnCur"));
                                captchasolve = CheckCaptcha();
                                curstring = curpage.Text;
                                parseresult = int.TryParse(curstring, out nCurPage);
                                if (!parseresult)
                                    continue;
                            }
                            catch (Exception e)
                            {
                                string error = e.Message;
                            }
                        }
                    }
                    if (detailurls.Count > 0)
                        SetProxy(refresh,detailurls[0]);
                    SetHttpClientCookies();
                    
                    for (int i = 0; i < detailurls.Count; i++)
                    {
                        try
                        {
                            if (i % 50 == 0)
                                SetProxy(refresh,detailurls[i]);
                            string detailone = detailurls[i];
                            PcmDataSet iteminfo = GetSellerInfo(detailone);
                            proinfo.Percentage = 100 * (float)index / (float)dataSet.Count + 100 * (float)i / (float)dataSet.Count / (float)detailurls.Count;
                            proinfo.CurrentProxy = curproxy;
                            onSetProgress(proinfo);
                            if (iteminfo.price == "" && iteminfo.seller == "")
                                continue;
                            onWriteStatus(string.Format("Finished product --> {0}", iteminfo.productname));
                            writer.WriteData(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", iteminfo.productname, iteminfo.brand, iteminfo.price, iteminfo.seller,iteminfo.customerreview,iteminfo.answeredquestion));
                        }
                        catch (Exception e)
                        {
                            string error = e.Message;
                            continue;
                        }
                    }
                    proinfo.Percentage = 100 * (float)(index + 1) / (float)dataSet.Count;
                    proinfo.CurrentProxy = "";
                    onSetProgress(proinfo);
                }
                catch (Exception e)
                {
                    string error = e.Message;
                    continue;
                }
                    
            }
            try
            {
                driver.Quit();
                Thread.Sleep(1000);
            }
            catch (Exception e)
            {
                string error = e.Message;
            }
            return true;
        }
        public bool doWorkBySeller(string savepath)
        {
            FileWriter writer = new FileWriter(savepath);
            bool refresh = true;
            for (int index = 0; index < dataSet.Count; index++)
            {
                try
                {
                    string setone = dataSet[index];
                    int proxyresult = SetProxy(refresh,setone);
                    int count = 0;
                    while (proxyresult == 2)
                    {
                        if (++count > loopcount)
                            break;
                        Thread.Sleep(500);
                        proxyresult = SetProxy(refresh,setone);
                    }
                    if (proxyresult != 2)
                        refresh = false;
                    else
                        continue;
                    driver.Url = setone;
                    driver.Navigate();
                    waitForTagDisplayed(By.Id("products-heading"));
                    bool captchasolve = CheckCaptcha();
                    IWebElement productheading = driver.FindElement(By.Id("products-heading"));
                    doClick(productheading);
                    waitForTagDisplayed(By.Id("products-results-data"));
                    IWebElement productsresults = driver.FindElement(By.Id("products-results-data"));
                    int exitcounter = 0;
                    int nMaxPage = 0;
                    string currenturl = driver.Url;
                    List<string> detailurls = new List<string>();
                    while(true)
                    {
                        try
                        {
                            Thread.Sleep(500);
                            if (++exitcounter > 30)
                                break;
                            IList<IWebElement> details = productsresults.FindElements(By.ClassName("product-details"));
                            if (details == null || details.Count == 0)
                                continue;
                            foreach (IWebElement detailone in details)
                            {
                                try
                                {
                                    IWebElement producttitle = detailone.FindElement(By.ClassName("product-title"));
                                    if (producttitle == null)
                                        continue;
                                    IWebElement alink = producttitle.FindElement(By.TagName("a"));
                                    string detailurl = alink.GetAttribute("href");
                                    if (detailurl != "")
                                        detailurls.Add(detailurl);
                                }
                                catch (Exception e)
                                {
                                    string error = e.Message;
                                    continue;
                                }
                            }
                            try
                            {
                                IWebElement navigation = productsresults.FindElement(By.ClassName("a-pagination"));
                                if (navigation == null)
                                {
                                    if (productheading == null)
                                    {
                                        driver.Navigate().Refresh();
                                        waitForTagDisplayed(By.Id("products-heading"));
                                        productheading = driver.FindElement(By.Id("products-heading"));
                                    }
                                    doClick(productheading);
                                    waitForTagDisplayed(By.ClassName("a-pagination"));
                                    navigation = productsresults.FindElement(By.ClassName("a-pagination"));
                                }

                                IList<IWebElement> navbuttons = navigation.FindElements(By.TagName("li"));
                                if (navbuttons == null || navbuttons.Count == 0)
                                    break;
                                IWebElement nextbtn = navbuttons[navbuttons.Count - 1];
                                IWebElement lastbtn = navbuttons[navbuttons.Count - 2];
                                string strmax = lastbtn.Text;
                                if (nMaxPage == 0)
                                {
                                    bool parseresult = int.TryParse(strmax, out nMaxPage);
                                    if (!parseresult)
                                        continue;
                                }
                                if (nextbtn == null)
                                    continue;

                                string nextbtnclass = nextbtn.GetAttribute("class");
                                if (nextbtnclass.IndexOf("a-disabled") > -1)
                                    break;
                                doClick(nextbtn);
                            }
                            catch (Exception e)
                            {
                                string error = e.Message;
                                break;
                            }
                        }
                        catch (Exception e)
                        {
                            string error = e.Message;
                            continue;
                        }
                    }
                    if (detailurls.Count > 0)
                        SetProxy(refresh,detailurls[0]);
                    for(int i=0;i<detailurls.Count;i++)
                    {
                        try
                        {
                            if (i % 100 == 0)
                                SetProxy(refresh,detailurls[i]);
                            string detailone = detailurls[i];
                            proinfo.Percentage = 100 * (float)index / (float)dataSet.Count + 100 * (float)i / (float)dataSet.Count / (float)detailurls.Count;
                            proinfo.CurrentProxy = curproxy;
                            onSetProgress(proinfo);
                            PcmDataSet iteminfo = GetSellerInfo(detailone);
                            if (iteminfo.seller == "" && iteminfo.price == "")
                                continue;
                            onWriteStatus(string.Format("Finished product --> {0}", iteminfo.productname));
                            writer.WriteData(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", iteminfo.productname, iteminfo.brand, iteminfo.price, iteminfo.seller, iteminfo.customerreview, iteminfo.answeredquestion));
                        }
                        catch (Exception e)
                        {
                            string error = e.Message;
                        }
                    }
                    proinfo.Percentage = 100 * (float)(index + 1) / (float)dataSet.Count;
                    proinfo.CurrentProxy = "";
                    onSetProgress(proinfo);
                }
                catch(Exception e)
                {
                    string error = e.Message;
                    continue;
                }
            }
            proinfo.Percentage = 100;
            onSetProgress(proinfo);
            if (driver != null)
                driver.Quit();
            return true;
        }
        public PcmDataSet doWork(string savepath)
        {
            //try
            //{
            //    FileWriter writer = new FileWriter(savepath);
            //    bool refresh = true;
            //    PcmDataSet todayData = new PcmDataSet();
            //    for (int index = 0; index < dataSet.Count; index++ )
            //    {
            //        PcmDataSet setone = dataSet[index];
            //        string curProductCode = setone.ProductCode;
            //        string curProductPrice = setone.ProductPrice.ToString();
            //        string CustomField = string.Empty;
            //        string ExcludeSite = string.Empty;
            //        foreach (string customone in setone.CustomField2)
            //        {
            //            CustomField += customone + "||";
            //        }
            //        foreach (string excludeone in setone.ExcludeSites)
            //        {
            //            ExcludeSite += excludeone + "||";
            //        }
            //        if (CustomField != "")
            //            CustomField = CustomField.Substring(0, CustomField.Length - 2);
            //        if (ExcludeSite != "")
            //            ExcludeSite = ExcludeSite.Substring(0, ExcludeSite.Length - 2);
            //        string printstr = curProductCode + "\t" + curProductPrice + "\t" + CustomField + "\t" + ExcludeSite + "\t";
            //        int proxyresult = SetProxy(refresh);
            //        int count = 0;
            //        while (proxyresult == 2)
            //        {
            //            if (++count > loopcount)
            //                break;
            //            Thread.Sleep(500);
            //            proxyresult = SetProxy(refresh);
            //        }
            //        if (proxyresult != 2)
            //            refresh = false;
            //        else
            //            continue;
            //        for (int i = 0; i < setone.CustomField2.Count; i++)
            //        {
            //            try
            //            {
            //                string col = setone.CustomField2[i];
            //                count = 0;
            //                IWebElement iteminput = getElementBy(driver, By.Id("gbqfq"));
            //                if (iteminput == null)
            //                    iteminput = getElementBy(driver, By.Id("lst-ib"));
            //                if (iteminput == null)
            //                    iteminput = getElementBy(driver, By.Id("sbhost"));
            //                while (iteminput == null)
            //                {
            //                    if (++count > loopcount)
            //                        break;
            //                    Thread.Sleep(500);
            //                    iteminput = getElementBy(driver, By.Id("gbqfq"));
            //                    if (iteminput == null)
            //                        iteminput = getElementBy(driver, By.Id("lst-ib"));
            //                    if (iteminput == null)
            //                        iteminput = getElementBy(driver, By.Id("sbhost"));
            //                }
            //                if (iteminput == null)
            //                    continue;
            //                iteminput.Clear();
            //                iteminput.SendKeys(col);
            //                count = 0;
            //                IWebElement btnfind = getElementBy(driver, By.Id("gbqfb"));
            //                if (btnfind == null)
            //                    btnfind = getElementBy(driver, By.Id("_fZl"));
            //                if (btnfind == null)
            //                    btnfind = getElementBy(driver, By.ClassName("lsb"));
            //                while (btnfind == null)
            //                {
            //                    if (++count > loopcount)
            //                        break;
            //                    Thread.Sleep(500);
            //                    btnfind = getElementBy(driver, By.Id("gbqfb"));
            //                    if (btnfind == null)
            //                        btnfind = getElementBy(driver, By.Id("_fZl"));
            //                    if (btnfind == null)
            //                        btnfind = getElementBy(driver, By.ClassName("lsb"));
            //                }
            //                if (btnfind == null)
            //                    continue;
            //                doClick(btnfind);
            //                count = 0;
            //                waitForTagDisplayed(By.Id("stt__ps-sort"));
            //                IWebElement sortbtn = getElementBy(driver, By.Id("stt__ps-sort"));
            //                count = 0;
            //                while (sortbtn == null)
            //                {
            //                    if (++count > 5)
            //                        break;
            //                    Thread.Sleep(500);
            //                    GoToShopping(col);
            //                    sortbtn = getElementBy(driver, By.Id("stt__ps-sort"));
            //                }
            //                if (sortbtn == null)
            //                    continue;
            //                doClick(sortbtn);
            //                WaitForPageLoad(180);
            //                IWebElement sortbox = getElementBy(driver, By.Id("stt__ps-sort-m"));
            //                count = 0;
            //                while (sortbox == null)
            //                {
            //                    if (++count > loopcount)
            //                        break;
            //                    sortbox = getElementBy(driver, By.Id("stt__ps-sort-m"));
            //                    Thread.Sleep(500);
            //                    doClick(sortbtn);
            //                }
            //                if (sortbox != null)
            //                {
            //                    count = 0;
            //                    waitForTagDisplayed(By.ClassName("goog-menuitem"));
            //                    IList<IWebElement> sortItems = sortbox.FindElements(By.ClassName("goog-menuitem"));
            //                    count = 0;
            //                    while (sortItems == null)
            //                    {
            //                        if (++count > loopcount)
            //                            break;
            //                        Thread.Sleep(500);
            //                        waitForTagDisplayed(By.ClassName("goog-menuitem"));
            //                        sortItems = sortbox.FindElements(By.ClassName("goog-menuitem"));
            //                    }

            //                    IWebElement sortItem = sortItems[1];
            //                    foreach (IWebElement sortItemone in sortItems)
            //                    {
            //                        IWebElement content = sortItemone.FindElement(By.ClassName("goog-menuitem-content"));
            //                        count = 0;
            //                        while (content == null)
            //                        {
            //                            if (++count > loopcount)
            //                                break;
            //                            Thread.Sleep(500);
            //                            content = sortItemone.FindElement(By.ClassName("goog-menuitem-content"));
            //                        }
            //                        if (content.Text == "Price - low to high")
            //                        {
            //                            sortItem = sortItemone;
            //                            break;
            //                        }
            //                    }
            //                    doClick(sortItem);
            //                }

            //                Thread.Sleep(1000 * delay);
            //                //string buyRate = string.Empty, volume = string.Empty;

            //                onWriteStatus(string.Format("Searching {0} keyword...", col));
            //                count = 0;
            //                WaitForTagVisible(By.ClassName("psli"));
                            
            //                IList<IWebElement> shopitems = driver.FindElements(By.ClassName("psli"));
            //                while (shopitems == null)
            //                {
            //                    if (++count > loopcount)
            //                        break;
            //                    Thread.Sleep(500);
            //                    shopitems = driver.FindElements(By.ClassName("psli"));
            //                    logging("logfile.txt", "psli is not found-" + count.ToString());
            //                }


            //                foreach (IWebElement itemone in shopitems)
            //                {
            //                    try
            //                    {
            //                        count = 0;
            //                        string info = itemone.FindElement(By.ClassName("shop__secondary")).Text;

            //                        if (info != "")
            //                        {
            //                            info = info.Replace("used ", "").Replace("from ", "").Replace("  ", " ");
            //                            if (info.Substring(0, 1) == "$")
            //                                info = info.Substring(1);
            //                            int idxsplit = info.IndexOf(" ");
            //                            if (index < 0)
            //                                continue;
            //                            string price = info.Substring(0,idxsplit);
            //                            string company = info.Substring(idxsplit + 1);
                                        
            //                            bool isexclude = true;
            //                            foreach (string excludeone in setone.ExcludeSites)
            //                            {
            //                                if (company.ToLower().IndexOf(excludeone.ToLower()) >= 0)
            //                                {
            //                                    isexclude = false;
            //                                    break;
            //                                }
            //                            }
            //                            if (!isexclude)
            //                                continue;
            //                            if (!CompanyCheck(company))
            //                            {
            //                                IWebElement atag = itemone.FindElement(By.TagName("a"));
            //                                if (atag == null)
            //                                    continue;
            //                                doClick(atag);
            //                                WaitForTagVisible(By.ClassName("pspo-popout"));
            //                                IWebElement popup = driver.FindElement(By.ClassName("pspo-popout"));
            //                                if (popup == null)
            //                                    continue;
            //                                IWebElement compare = popup.FindElement(By.ClassName(" _-bb"));
            //                                if (compare == null)
            //                                    continue;
            //                                doClick(compare);
            //                                waitForTagDisplayed(By.ClassName("os-seller-name"));
            //                                IList<IWebElement> companies = driver.FindElements(By.ClassName("os-seller-name"));
            //                                foreach (IWebElement companyone in companies)
            //                                {
            //                                    company = companyone.Text;
            //                                    isexclude = true;
            //                                    foreach (string excludeone in setone.ExcludeSites)
            //                                    {
            //                                        if (company.ToLower().IndexOf(excludeone.ToLower()) >= 0)
            //                                        {
            //                                            isexclude = false;
            //                                            break;
            //                                        }
            //                                    }
            //                                    if (!isexclude)
            //                                        continue;
            //                                    break;
            //                                }
            //                            }
            //                            if (!isexclude)
            //                                continue;
            //                            float distance = float.Parse(curProductPrice) - float.Parse(price);
            //                            string dist = distance.ToString("0.00");
            //                            printstr += company + "\t" + price + "\t" + dist + "\t";
            //                            break;
            //                        }
            //                    }
            //                    catch (Exception e)
            //                    {
            //                        continue;
            //                    }
            //                }
            //            }
            //            catch (Exception e)
            //            {
            //                continue;
            //            }

            //            //driver.Navigate().Refresh();
            //            //WaitForPageLoad(180);


            //            //doShowSearch();
            //            //doSearch(col, ref buyRate, ref volume);

            //            //todayData.buyRateCols.Add(buyRate);
            //            //todayData.volumeCols.Add(volume);

            //        }
            //        bool result = writer.WriteData(printstr);
            //        count = 0;
            //        while (!result) 
            //        {
            //            if (++count > 300) 
            //                break;
            //            Thread.Sleep(100);
            //            result = writer.WriteData(printstr);
            //        }
            //        onSetProgress(index+1);
            //    }
            //    if (driver != null)
            //        driver.Close();
            //    return todayData;
            //}
            //catch(Exception e)
            //{
            //    string errstr = e.Message;
            //    return null;
            //}
            return null;
        }
    }
}
