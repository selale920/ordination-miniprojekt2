using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;
using Data;
using shared.Model;
using System;
using System.Linq;
using System.Collections.Generic;

namespace ordination_test
{
    [TestClass]
    public class ServiceTest
    {
        private DataService service;

        [TestInitialize]
        public void SetupBeforeEachTest()
        {
            DbContextOptionsBuilder<OrdinationContext> optionsBuilder =
                new DbContextOptionsBuilder<OrdinationContext>();

            optionsBuilder.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

            OrdinationContext context = new OrdinationContext(optionsBuilder.Options);

            service = new DataService(context);
            service.SeedData();

            Assert.IsNotNull(service);
        }

        [TestMethod]
        public void PatientsExist()
        {
            List<Patient> patienter = service.GetPatienter();

            Assert.IsNotNull(patienter);
            Assert.IsTrue(patienter.Count > 0);
        }

        [TestMethod]
        public void OpretDagligFast()
        {
            List<Patient> patienter = service.GetPatienter();
            List<Laegemiddel> laegemidler = service.GetLaegemidler();

            Assert.IsNotNull(patienter);
            Assert.IsNotNull(laegemidler);
            Assert.IsTrue(patienter.Count > 0);
            Assert.IsTrue(laegemidler.Count > 0);

            Patient patient = patienter.First();
            Laegemiddel laegemiddel = laegemidler.First();

            int initialCount = service.GetDagligFaste().Count();

            DagligFast dagligFast = service.OpretDagligFast(
                patient.PatientId,
                laegemiddel.LaegemiddelId,
                2,
                2,
                1,
                0,
                DateTime.Now,
                DateTime.Now.AddDays(3)
            );

            Assert.IsNotNull(dagligFast);
            Assert.IsTrue(dagligFast.OrdinationId > 0);
            Assert.AreEqual(initialCount + 1, service.GetDagligFaste().Count());
        }

        [TestMethod]
        public void OpretDagligSkaev()
        {
            List<Patient> patienter = service.GetPatienter();
            List<Laegemiddel> laegemidler = service.GetLaegemidler();

            Assert.IsNotNull(patienter);
            Assert.IsNotNull(laegemidler);
            Assert.IsTrue(patienter.Count > 0);
            Assert.IsTrue(laegemidler.Count > 0);

            Patient patient = patienter.First();
            Laegemiddel laegemiddel = laegemidler.First();

            int initialCount = service.GetDagligSkæve().Count();

            Dosis[] doser = new Dosis[]
            {
                new Dosis(DateTime.Now, 1),
                new Dosis(DateTime.Now.AddHours(1), 2),
                new Dosis(DateTime.Now.AddHours(2), 3)
            };

            DagligSkæv dagligSkaev = service.OpretDagligSkaev(
                patient.PatientId,
                laegemiddel.LaegemiddelId,
                doser,
                DateTime.Now,
                DateTime.Now.AddDays(3)
            );

            Assert.IsNotNull(dagligSkaev);
            Assert.IsTrue(dagligSkaev.OrdinationId > 0);
            Assert.AreEqual(initialCount + 1, service.GetDagligSkæve().Count());
        }

        [TestMethod]
        public void OpretPN()
        {
            List<Patient> patienter = service.GetPatienter();
            List<Laegemiddel> laegemidler = service.GetLaegemidler();

            Assert.IsNotNull(patienter);
            Assert.IsNotNull(laegemidler);
            Assert.IsTrue(patienter.Count > 0);
            Assert.IsTrue(laegemidler.Count > 0);

            Patient patient = patienter.First();
            Laegemiddel laegemiddel = laegemidler.First();

            int initialCount = service.GetPNs().Count();

            PN pn = service.OpretPN(
                patient.PatientId,
                laegemiddel.LaegemiddelId,
                2,
                DateTime.Now,
                DateTime.Now.AddDays(3)
            );

            Assert.IsNotNull(pn);
            Assert.IsTrue(pn.OrdinationId > 0);
            Assert.AreEqual(initialCount + 1, service.GetPNs().Count());
        }

        [TestMethod]
        public void AnvendPN()
        {
            List<Patient> patienter = service.GetPatienter();
            List<Laegemiddel> laegemidler = service.GetLaegemidler();

            Assert.IsNotNull(patienter);
            Assert.IsNotNull(laegemidler);
            Assert.IsTrue(patienter.Count > 0);
            Assert.IsTrue(laegemidler.Count > 0);

            Patient patient = patienter.First();
            Laegemiddel laegemiddel = laegemidler.First();

            PN pn = service.OpretPN(
                patient.PatientId,
                laegemiddel.LaegemiddelId,
                2,
                DateTime.Now.AddDays(-1),
                DateTime.Now.AddDays(3)
            );

            Assert.IsNotNull(pn);
            Assert.AreEqual(0, pn.getAntalGangeGivet());

            string response = service.AnvendOrdination(
                pn.OrdinationId,
                new Dato { dato = DateTime.Now }
            );

            Assert.AreEqual("dosis have been given", response);

            PN updatedPn = service.GetPNs().FirstOrDefault(x => x.OrdinationId == pn.OrdinationId);

            Assert.IsNotNull(updatedPn);
            Assert.AreEqual(1, updatedPn.getAntalGangeGivet());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OpretDagligFast_MedUgyldigeDatoer_KasterException()
        {
            List<Patient> patienter = service.GetPatienter();
            List<Laegemiddel> laegemidler = service.GetLaegemidler();

            Assert.IsNotNull(patienter);
            Assert.IsNotNull(laegemidler);
            Assert.IsTrue(patienter.Count > 0);
            Assert.IsTrue(laegemidler.Count > 0);

            Patient patient = patienter.First();
            Laegemiddel laegemiddel = laegemidler.First();

            service.OpretDagligFast(
                patient.PatientId,
                laegemiddel.LaegemiddelId,
                2,
                2,
                1,
                0,
                DateTime.Now,
                DateTime.Now.AddDays(-1)
            );
        }
    }
}