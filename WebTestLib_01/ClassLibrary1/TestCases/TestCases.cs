using NUnit.Framework;
using WrapperAPI;

namespace TestCases
{
    [TestFixture]
    public class TestSuite
    {
        [OneTimeSetUp]
        public void InitBrowser()
        {
            Browser.InitOpenFirefox();
        }

        [Test]
        public void Scenario_1()
        {
            Browser.OpenUrl("http://opensource.demo.orangehrmlive.com");
            Users.LogInAs("Admin", "admin");
            Users.AddUser("test_1234", "Te$t_12_@#");
            Users.LogOut();
            Users.LogInAs("test_1234", "Te$t_12_@#");
            Users.LogOut();
        }

        [Test]
        public void Scenario_2()
        {
            Browser.OpenUrl("http://opensource.demo.orangehrmlive.com");
            Users.LogInAs("Admin", "admin");
            Users.ViewAttendanceRecords("John Smith", "18");
            if (!Users.isAddendTableEmpty()) {
                Assert.Fail("Fail: The attendance table is not empty!");
            }
            Users.AddNewAttendRecord("18", "13:00", "2", "Don't be late!", "John Smith");
            string err = Users.CheckAddedRecord("John Smith", "18", "13:00", "2", "Don't be late!");
            Users.RemoveAllRecords("John Smith", "18");
            Assert.IsEmpty(err, err); //assert to Users.CheckAddedRecord (two lines higher)
            Users.LogOut();
        }

        [OneTimeTearDown]
        public void Dispose()
        {
            Browser.QuitBrowser();
        }
    }
}
