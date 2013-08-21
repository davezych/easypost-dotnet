﻿using System.Collections.Generic;
using System.Linq;
using Easypost;
using NUnit.Framework;

namespace EasyPost.Api.Tests
{
    [TestFixture]
    public class EasyPostClientTests
    {
        private IEasyPostClient _client;

        [SetUp]
        public void Setup()
        {
            _client = new EasyPostClient("YOUR_TEST_API_KEY");
        }

        [Test]
        public void TestAddresses()
        {
            var fromAddress = _client.CreateAddress(new Address
            {
                Name = "EasyPost",
                Street1 = "2135 Sacramento St",
                City = "San Francisco",
                State = "CA",
                Zip = "94109",
                Email = "support@easypost.com",
            });
            Assert.IsNotNull(fromAddress.Id);

            var verified = _client.VerifyAddress(fromAddress.Id);
            Assert.IsNotNull(verified.Address.Id);

            var sameAsFrom = _client.GetAddress(fromAddress.Id);
            Assert.AreEqual(fromAddress.Id, sameAsFrom.Id);

            var allAddresses = _client.GetAddresses();
            var shouldExist = allAddresses.SingleOrDefault(x => string.Equals(x.Id, fromAddress.Id));
            Assert.IsNotNull(shouldExist);
        }

        [Test]
        public void TestParcels()
        {
            var customParcel = _client.CreateParcel(new Parcel
            {
                LengthInches = 6,
                WidthInches = 6,
                HeightInches = 4,
                WeightOunces = 13,
            });
            Assert.IsNotNull(customParcel.Id);

            var parcel = _client.CreateParcel(new Parcel
            {
                PredefinedPackage = ParcelType.FedExEnvelope,
                WeightOunces = 2,
            });
            Assert.IsNotNull(parcel.Id);

            var sameAsCustomParcel = _client.GetParcel(customParcel.Id);
            Assert.AreEqual(customParcel.Id, sameAsCustomParcel.Id);

            var allParcels = _client.GetParcels();
            var shouldExist = allParcels.SingleOrDefault(x => string.Equals(x.Id, customParcel.Id));
            Assert.IsNotNull(shouldExist);
        }

        [Test]
        public void TestShipments()
        {
            var addresses = _client.GetAddresses();
            var parcels = _client.GetParcels();

            var shipment = _client.CreateShipment(new Shipment
            {
                Parcel = parcels[0],
                FromAddress = addresses[0],
                ToAddress = addresses[1],
            });
            Assert.IsNotNull(shipment.Id);
            
            var sameAsShipment = _client.GetShipment(shipment.Id);
            Assert.AreEqual(shipment.Id, sameAsShipment.Id);

            var rates = _client.GetShipmentRates(shipment.Id);
            Assert.AreEqual(rates.Count, shipment.Rates.Count);
        }

        [Test]
        public void TestBuyingLabel()
        {
            // TODO test this without actually buying...
        }

        [Test]
        public void TestCustoms()
        {
            var customsItem = _client.CreateCustomsItem(new CustomsItem
            {
                Description = "testing",
                Quantity = 1,
                Value = 100,
                WeightOunces = 16,
                HsTariffNumber = "123456",
                OriginCountry = "US",
            });
            Assert.IsNotNull(customsItem.Id);

            var sameAsCustomsItem = _client.GetCustomsItem(customsItem.Id);
            Assert.AreEqual(sameAsCustomsItem.Id, customsItem.Id);

            var allCustomsItems = _client.GetCustomsItems();
            var shouldExistItem = allCustomsItems.SingleOrDefault(x => string.Equals(x.Id, customsItem.Id));
            Assert.IsNotNull(shouldExistItem);

            var customsInfo = _client.CreateCustomsInfo(new CustomsInfo
            {
                CustomsCertify = true,
                CustomsSigner = "Jonathan Calhoun",
                ContentsType = "merchandise",
                ContentsExplanation = " ",
                RestrictionType = "none",
                EelPfc = "NOEEI 30.37(a)",
                CustomsItems = new List<CustomsItem> {customsItem},
            });
            Assert.IsNotNull(customsInfo.Id);

            var sameAsCustomsInfo = _client.GetCustomsInfo(customsInfo.Id);
            Assert.AreEqual(sameAsCustomsInfo.Id, customsInfo.Id);

            var allCustomsInfos = _client.GetCustomsInfos();
            var shouldExistInfo = allCustomsInfos.SingleOrDefault(x => string.Equals(x.Id, customsInfo.Id));
            Assert.IsNotNull(shouldExistInfo);
        }

        [Test]
        public void TestScanForms()
        {
            var scanForm = _client.CreateScanForm(new ScanForm
            {
                TrackingCodes = new List<string> {"123456", "123455", "123454"},
                Address = new Address
                {
                    Name = "EasyPost",
                    Street1 = "2135 Sacramento St",
                    City = "San Francisco",
                    State = "CA",
                    Zip = "94109",
                    Email = "support@easypost.com",
                },
            });
            Assert.IsNotNull(scanForm.Id);

            var sameAsScanForm = _client.GetScanForm(scanForm.Id);
            Assert.AreEqual(scanForm.Id, sameAsScanForm.Id);

            var allScanForms = _client.GetScanForms();
            var shouldExist = allScanForms.SingleOrDefault(x => string.Equals(x.Id, scanForm.Id));
            Assert.IsNotNull(shouldExist);
        }

        [Test]
        public void TestRefunds()
        {
            var refunds = _client.CreateRefund(new RefundRequest
            {
                Carrier = "USPS",
                TrackingCodes = new List<string> { "CJ123456789US", "LN123456789US" }
            });
            Assert.IsTrue(refunds.Count == 2);
            Assert.IsNotNull(refunds[0].Id);

            var sameAsRefund = _client.GetRefund(refunds[0].Id);
            Assert.AreEqual(refunds[0].Id, sameAsRefund.Id);

            var allRefunds = _client.GetRefunds();
            var shouldExist = allRefunds.SingleOrDefault(x => string.Equals(x.Id, refunds[0].Id));
            Assert.IsNotNull(shouldExist);
        }
    }
}