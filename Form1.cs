using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.IO;

namespace SBA
{
    public partial class Form1 : Form
    {
        IWebDriver driver;
        List<string> AllSymbols = new List<string>();
        List<string> ListsDirArr = new List<string>();
        List<string> PullListArr = new List<string>();
        string[] ExcelFilePaths;

        public Form1()
        {
            InitializeComponent();
            CreateDriverWithOptions();
            //PopulateSymbolTBAndArray();
        }


        public void CreateDriverWithOptions()
        {
            ChromeOptions co = new ChromeOptions();
            String CDDir = "C:\\Users\\Mike\\source\\repos\\SBA\\packages\\Selenium.WebDriver.ChromeDriver.106.0.5249.6100\\driver\\win32";
            co.AddArgument("user-data-dir=C:\\0Stocks\\ChromeProfile\\Profile 2");
            IWebDriver driver = new ChromeDriver(CDDir, co);
            SetCurrentDriver(driver);
        }

        public void WriteSymbols()
        {
            System.IO.File.WriteAllLines(@"C:\0Stocks\txts\Pull\AllSymbols.txt", AllSymbols);
        }

        private void ChromeGrabCharts()
        {
            TimeSpan ts = new TimeSpan(0, 0, 10);
            int i = 0;

            Actions actions = new Actions(driver);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
            foreach (string s in AllSymbols)
            {
                driver.Navigate().GoToUrl("https://www.nasdaq.com/market-activity/stocks/" + s + "/advanced-charting");
                DateTime now = DateTime.Now;
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.ClassName("ciq-dropdowns")));
                IWebElement SetTime = driver.FindElement(By.ClassName("ciq-dropdowns"));
                SetTime.Click();
                actions.Release();

                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath("//*[contains(text(), '1 Min')]")));
                IWebElement SetMin = driver.FindElement(By.XPath("//*[contains(text(), '1 Min')]"));
                SetMin.Click();
                actions.Release();

                IWebElement Selector = driver.FindElement(By.ClassName("ciq-footer full-screen-hide"));
                actions.MoveToElement(Selector);
                actions.Release();

                IWebElement SetDay = driver.FindElement(By.XPath("//*[contains(text(), '1D')]"));
                SetDay.Click();
                actions.Release();

                IWebElement TableView = driver.FindElement(By.ClassName("ciq - DT tableview - ui"));
                TableView.Click();
                actions.Release();
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.ClassName("ciq-data-table-download-csv")));
                IWebElement DownloadButton = driver.FindElement(By.ClassName("ciq-data-table-download-csv"));
                DownloadButton.Click(); actions.Release();

            }
        }

        private void btn_RefreshExcel_Click(object sender, EventArgs e)
        {
            string path = @"C:\0Stocks\excel\download\";
            string[] FilePaths = Directory.GetFiles(path);
            foreach (string s in FilePaths)
            {
                //TB_ExcelFiles.Items.Add(s);
            }

            SetExcelFilePaths(FilePaths);
        }

        public void SetExcelFilePaths(string[] paths)
        {
            ExcelFilePaths = paths;
        }
        public string[] GetExcelFilePaths()
        {
            return ExcelFilePaths;
        }

        private void btn_GetInfo_Click(object sender, EventArgs e)
        {
           // String symbol = txt_InfoBox.Text;

            driver.Navigate().GoToUrl("https://www.tradingview.com/chart/");
        }



        public void SetCurrentDriver(IWebDriver d)
        {
            driver = d;
        }

        public ChromeDriver GetCurrentDriver()
        {
            return (ChromeDriver)driver;
        }

        private void btn_Refresh_Click(object sender, EventArgs e)
        {
            foreach (string line in System.IO.Directory.GetFiles(@"C:\0Stocks\txts\Lists\"))
            {
                String[] SplitFileName = line.Split('\\');
                String FileName = SplitFileName.Last<String>();
                ListsDirArr.Add(line);
                LB_AvailableLists.Items.Add(FileName);
            }
        }

        private void btn_LoadList_Click(object sender, EventArgs e)
        {

            lb_CurrentListSymbols.Items.Clear();
            if(LB_AvailableLists.SelectedIndex != -1)
            {
                foreach (string line in System.IO.File.ReadLines(ListsDirArr[LB_AvailableLists.SelectedIndex]))
                {
                    lb_CurrentListSymbols.Items.Add(line);
                }
            }
        }

        private void btn_AddListToPull_Click(object sender, EventArgs e)
        {
            if (LB_AvailableLists.SelectedIndex != -1)
            {
                LB_PullList.Items.Add(LB_AvailableLists.Items[LB_AvailableLists.SelectedIndex].ToString());
                
                foreach (string items in LB_PullList.Items)
                {
                    //TODO
                }
            }
        }

        private void btn_UpdatePulls_Click(object sender, EventArgs e)
        {
            foreach(string line in LB_PullList.Items)
            {
                if (!LB_CurrentPull.Items.Contains(line))
                {
                    LB_CurrentPull.Items.Add(line);
                }
            }
        }

        private void btn_RmPullList_Click(object sender, EventArgs e)
        {
            if(LB_CurrentPull.SelectedIndex != -1)
            {
                LB_CurrentPull.Items.Remove(LB_CurrentPull.SelectedItems);
                ListsDirArr.Remove(@"C:\0Stocks\txts\Lists\" + LB_CurrentPull.SelectedItems);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach(string FileDir in ListsDirArr)
            {
                foreach(string line in System.IO.File.ReadLines(FileDir))
                {
                    AllSymbols.Add(line);
                }
            }
            WriteSymbols();
        }

    }
}
