using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using NUnit.Framework;

namespace WrapperAPI
{
    public static class Browser
    {
        static FirefoxDriver firefox = null;
        
        public static void InitOpenFirefox()
        {
            firefox = new FirefoxDriver();
            firefox.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, 15);
            firefox.Manage().Timeouts().PageLoad = new TimeSpan(0, 0, 25);
            //return firefox;
        }

        //Open URL in the test browser
        public static void OpenUrl(string web_addr)
        {   
            firefox.Navigate().GoToUrl(web_addr);
        }

        //quit the browser
        public static void QuitBrowser()
        {
            //Close all instances of the browser
            firefox.Quit();
        }

        //find a web element
        //TODO: rewrite using "Boolean ElementExists(string id)"
        public static IWebElement FindWebElement(string elem_id, string search_by="el_id")
        {
            IWebElement elem;
            try
            {
                elem = null;// firefox.FindElement(By.Id(elem_id));
                if (search_by == "el_id") {
                    elem = firefox.FindElement(By.Id(elem_id));
                }
                if (search_by == "class_name")
                {
                    elem = firefox.FindElement(By.ClassName(elem_id));
                }

            }
            catch (NoSuchElementException e)
            {
                elem = null;
                Assert.Fail(e.Message);
            }
            
            return elem;
        }

        //Check if a web element exist (return True if it exits)
        public static Boolean ElementExists(string elem_attr, string search_by="elem_id")
        {
            Boolean isPresent = false;
            if (search_by == "elem_id")
            {
                isPresent = firefox.FindElements(By.Id(elem_attr)).Count > 0;
            }
            if (search_by == "link_text")
            {
                isPresent = firefox.FindElements(By.LinkText(elem_attr)).Count > 0;
            }
            //Boolean isPresent = firefox.FindElements(By.Id(elem_id)).Count > 0;
            return isPresent;
        }

        //Enter text into e.g. input field 
        public static void SetText(string el_id, string txt, string elem_name)
        {
            string err = ""; //contains error message, "" - means no error occurred
            IWebElement el = FindWebElement(el_id);
            if (el != null)
            {
                el.Clear();
                el.SendKeys(txt); }
            else
            {
                err = String.Format("web element ({0}) not found", elem_name);
                Assert.Fail(err);
            }
        }

        //click element (e.g. a Button)
        public static void WebClick(string elem_id, string elem_name)
        {
            string err = ""; //contains error message, "" - means no error occurred
            
            try
            {
                IWebElement elem = firefox.FindElement(By.Id(elem_id));
                elem.Click();
            }
            catch (NoSuchElementException e)
            {
                err = String.Format("Fail: web element (id = '{0}', {1}) not found, ", elem_id, elem_name);
                err += e.Message;
                //Assert.Fail(err + e.Message);
            }
            
            Assert.IsEmpty(err, err);
        }

        //TODO: add check if elem/link exists
        //public static void ClickLink_test(string href)
        //{
            //firefox.FindElement(By.XPath(String.Format("//a[@href='{0}']", href))).Click();
            //firefox.FindElement(By.LinkText(href)).Click();
            //string xpath = String.Format("//a[contains(@href, '{0}')]", href);
            //IWebElement el = firefox.FindElement(By.XPath(xpath));
            //IWebElement el = firefox.FindElement(By.LinkText(href));
            //WebDriverWait wait = new WebDriverWait(firefox, new TimeSpan(0,0,10));
            //wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText(href))); //Locate element by partial linkText.
        
            //el.Click();
            //Browser.OpenUrl(href);
            //link.Submit();
        //}

        //TODO: add check if elem/link exists
        public static void ClickLink(string locator, string search_by="link_text")
        {
            if (search_by == "link_text") {
                firefox.FindElement(By.LinkText(locator)).Click();
            }
        }

        public static void SelectDayInDatePicker(string day)
        {
            Browser.ClickLink(day);
        }

        public static void SelectMonth(string mon) {
            // select the timezone from the drop down list
            IWebElement monthDropDown = Browser.FindWebElement("ui-datepicker-month", "class_name");
            SelectElement selectMonth = new SelectElement(monthDropDown);

            //select by value
            selectMonth.SelectByValue(mon);
        }

        public static List<string> ParseAttendTable()
        {
            //Parse the attendance table and return the list of the rows
            //the format of a row: 'cell1,cell2,..,celln,'
            //Boolean isFound = false;
            ReadOnlyCollection<IWebElement> rows = firefox.FindElements(By.CssSelector("tbody > tr"));
            int num_row = rows.Count();
            List<string> row_list = new List<string>();
           
            foreach (IWebElement row in rows)
            {
                string row_txt = "";
                ReadOnlyCollection<IWebElement> cells = row.FindElements(By.TagName("td"));
                foreach (IWebElement cell in cells){
                    row_txt += cell.Text + ",";
                }
                //add row to the list
                row_list.Add(row_txt);
            }
            //Assert.Fail(txt);
            return row_list;
        }

        public static void RefreshPage()
        {
            firefox.Navigate().Refresh();
        }
    }

    public static class Users
    {
        public static void LogInAs(string login, string password)
        {
            //Browser.OpenUrl("http://www.google.com.ua");
            Browser.OpenUrl("http://opensource.demo.orangehrmlive.com");
            Browser.SetText("txtUsername", login, "User name field");
            Browser.SetText("txtPassword", password, "Password field");
            Browser.WebClick("btnLogin", "Login button");
            // check if login is succeessful
            if (!Browser.ElementExists("welcome"))
            {
                //login failed!
                Assert.Fail("Login failed: check credentials!");
            }
        }

        public static void LogOut()
        {
            //Browser.OpenUrl("http://www.google.com.ua");
            //Browser.OpenUrl("http://opensource.demo.orangehrmlive.com");
            string href = "http://opensource.demo.orangehrmlive.com/index.php/auth/logout";
            Browser.OpenUrl(href);
            // check if logout is succeessful
            if (!Browser.ElementExists("logInPanelHeading"))
            {
                //login failed!
                Assert.Fail("Logout failed!");
            }
        }

        public static void AddUser(string userName, string passw)
        {
            //go to the user management page
            Browser.OpenUrl("http://opensource.demo.orangehrmlive.com/index.php/admin/viewSystemUsers");
            //click 'Add' button on 'User management' page
            Browser.WebClick("btnAdd", "Add a new user button");
            //Set values to the text fields (user name, password etc)
            Browser.SetText("systemUser_employeeName_empName", "John Smith", "Employee name");
            Browser.SetText("systemUser_userName", userName, "User name");
            Browser.SetText("systemUser_password", passw, "Password");
            Browser.SetText("systemUser_confirmPassword", passw, "Confirm Password");
            //Save the new user data
            Browser.WebClick("btnSave", "Save a new user button");
            //check if a new user is created
            if (!Browser.ElementExists(userName, "link_text"))
            {
                string err = String.Format("Fail: user '{0}' not created!", userName); 
                Assert.Fail(err);
            }
        }

        //display the attendace table for a specific user
        public static void ViewAttendanceRecords(string userName, string day)
        {
            Browser.OpenUrl("http://opensource.demo.orangehrmlive.com/index.php/attendance/viewAttendanceRecord");
            Browser.RefreshPage();
            Browser.SetText("attendance_employeeName_empName", userName, "employee name field");
            Browser.WebClick("attendance_date", "date text box");
            //Browser.SelectMonth("2");
            Browser.SelectDayInDatePicker(day);

            Browser.WebClick("btView", "View button");
        }

        public static bool isAddendTableEmpty()
        {
            bool isEmpty = false;
            string emptiness_indicator = "No attendance records to display";
            List<string> attendTable = new List<string>();
            attendTable = Browser.ParseAttendTable();
            if (attendTable.Count() == 1)
            {
                foreach (string row in attendTable) {
                    if (row.Contains(emptiness_indicator)) {
                        isEmpty = true;
                    }
                }
            }

            return isEmpty;
        }

        public static void AddNewAttendRecord(string day, string time, string timezone_id, string note, string user) {
            Browser.WebClick("btnPunchOut", "Add attendance record button");
            
            //string record = String.Format("{0},2018-06-{1} {2}:00 GMT {3}.0,{4}", user, day, time, timezone_id, note);
            //Browser.SetText("attendance_date", date, "attend date text box");
            Browser.WebClick("attendance_date", "attend date text box");
            Browser.SelectDayInDatePicker(day);
            Browser.SetText("attendance_time", time, "attend time text box");
            Browser.SetText("attendance_note", note, "attend time text box");

            // select the timezone from the drop down list
            IWebElement timezone = Browser.FindWebElement("attendance_timezone");
            SelectElement selectTimezone = new SelectElement(timezone);

            //select by value
            selectTimezone.SelectByValue(timezone_id);
            Browser.WebClick("btnPunch", "Add record button");
            //return record;
        }

        public static string CheckAddedRecord(string user, string day, string time, string timezone_id, string note) {
            Users.ViewAttendanceRecords(user, day);

            string record = String.Format("{0},2018-06-{1} {2}:00 GMT {3}.0,{4}", user, day, time, timezone_id, note);

            List<string> attendTab = Browser.ParseAttendTable();
            int found = 0;
            if (attendTab.Count() > 0) {
                foreach (string row in attendTab) {
                    if (row.Contains(record)) {
                        found++;
                    }
                }
            }
            string err = "";
            if (found == 0) {
                err = "Fail: the record not found";
            }
            if (found > 1)
            {
                err = "Fail: multiple records found in the table";
            }
            return err;
        }

        public static void RemoveAllRecords(string user, string day)
        {
            //bool isEmpty = Users.isAddendTableEmpty();
            Users.ViewAttendanceRecords(user, day);
            

            if (!Users.isAddendTableEmpty())
            {
                IWebElement checkAll = Browser.FindWebElement("ohrmList_chkSelectAll");
                if (checkAll.GetAttribute("value") == "")
                {
                    checkAll.Click();
                }
            }
            Browser.WebClick("btnDelete", "Delete button");
            Browser.WebClick("okBtn", "OK button");
        }   

    }
}
