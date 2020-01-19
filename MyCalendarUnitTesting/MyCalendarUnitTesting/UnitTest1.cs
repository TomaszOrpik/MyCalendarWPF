using NUnit.Framework;
using CustomCalendar;

namespace MyCalendarUnitTesting
{
    public class NoteTests
    {
        public Note note1;
        public Note note2;
        [SetUp]
        public void Setup()
        {
            note1 = new Note("1", "test1", "02-02-2020", "testing", "testing Environment", true);
        }

        [Test]
        public void SaveNote()
        {
            note1.Save(); //save data to database
            note2 = new Note("test1"); //get data from database
            Assert.True(note1.GetValue("title") == "test1"); //compare data
        }
    }
}