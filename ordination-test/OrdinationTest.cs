using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using shared.Model;


namespace ordination_test.Ordination
{
    [TestClass]
    public class Ordination
    {
        [TestMethod]
        public void testOrdinationDurationPositive()
        {
            DateTime end = new DateTime(2025, 10, 10, 12, 0, 0);
            DateTime start = new DateTime(2025, 10, 12, 12, 0, 0);
            Laegemiddel laegemiddel = new Laegemiddel("Test", 1, 1, 1, "tablet");

            Assert.ThrowsException<ArgumentException>(() => new DagligFast(start, end, laegemiddel, 1, 1, 1, 1));
            Assert.ThrowsException<ArgumentException>(() => new DagligSkæv(start, end, laegemiddel));
            Assert.ThrowsException<ArgumentException>(() => new PN(start, end, 1, laegemiddel));
        }

        [TestMethod]
        public void antalDageTest()
        {
            DateTime start = new DateTime(2025, 10, 10, 12, 0, 0);
            DateTime end = new DateTime(2025, 10, 12, 11, 59, 59);
            Laegemiddel laegemiddel = new Laegemiddel("Test", 1, 1, 1, "tablet");

            DagligFast fast = new DagligFast(start, end, laegemiddel, 1, 1, 1, 1);

            Assert.AreEqual(2, fast.antalDage());
            
        }
    }
}