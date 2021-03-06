﻿using System;
using System.Linq;
using AutoFixture;
using AutoFixture.Kernel;
using EMG.XML;
using NUnit.Framework;
using TestHelper;

namespace Tests.EMG30.Serialization
{
    [TestFixture]
    public class EventSerializationTests
    {
        private IFixture fixture;

        [SetUp]
        public void Initialize()
        {
            fixture = new Fixture();

            fixture.Customize<Import>(o => o.With(p => p.Version, 3.0m));

            fixture.Customize<ProviderNode>(o => o
                                                 .With(p => p.Locations, new LocationNode[0])
                                                 .With(p => p.ContentFields, new ProviderContentField[0]));

            fixture.Customize<CourseNode>(o => o
                                               .OmitAutoProperties()
                                               .With(p => p.UniqueIdentifier)
                                               .With(p => p.Name)
                                               .With(p => p.CourseType)
                                               .With(p => p.Events)
                                               .With(p => p.ContentFields, new CourseContentField[0])
            );

            fixture.Customize<LocationContacts>(o => o.With(p => p.Email, "test@testing.com"));

            fixture.Customizations.Add(new TypeRelay(typeof(EventNode), typeof(DistanceEvent)));
        }

        [Test]
        public void Event_uniqueIdentifier_must_be_unique()
        {
            var sharedUniqueIdentifier = fixture.Create<string>();

            fixture.Customize<DistanceEvent>(o => o
                                                  .OmitAutoProperties()
                                                  .With(p => p.UniqueIdentifier, sharedUniqueIdentifier)
                                                  .With(p => p.DeliveryMethod)
            );

            var import = fixture.Create<Import>();

            var serialized = import.ToXml();

            Assert.Throws<AggregateException>(() => XmlSchemas.XmlImport.ValidateDocument(serialized));

            Console.Write(serialized);
        }


        [Test]
        public void Minimal_area_event_can_be_serialized()
        {
            fixture.Customizations.Insert(0, new TypeRelay(typeof(EventNode), typeof(AreaEvent)));

            fixture.Customize<AreaEvent>(o => o.OmitAutoProperties().With(p => p.UniqueIdentifier).With(p => p.DeliveryMethod).With(p => p.Place));

            var import = fixture.Create<Import>();

            var serialized = import.ToXml();

            XmlSchemas.XmlImport.ValidateDocument(serialized);

            Console.Write(serialized);
        }

        [Test]
        public void Minimal_distance_event_can_be_serialized()
        {
            fixture.Customize<DistanceEvent>(o => o
                                                  .OmitAutoProperties()
                                                  .With(p => p.UniqueIdentifier)
                                                  .With(p => p.DeliveryMethod)
            );

            var import = fixture.Create<Import>();

            var serialized = import.ToXml();

            XmlSchemas.XmlImport.ValidateDocument(serialized);

            Console.Write(serialized);
        }

        [Test]
        public void Location_event_requires_referenced_location()
        {
            fixture.Customizations.Insert(0, new TypeRelay(typeof(EventNode), typeof(LocationEvent)));

            var locationIdenfier = fixture.Create<string>();

            fixture.Customize<LocationNode>(o => o.With(p => p.UniqueIdentifier, locationIdenfier));

            fixture.Customize<ProviderNode>(o => o
                                                 .With(p => p.Locations, fixture.CreateMany<LocationNode>(1).ToArray())
                                                 .With(p => p.ContentFields, new ProviderContentField[0]));

            fixture.Customize<LocationEvent>(o => o
                                                  .OmitAutoProperties()
                                                  .With(p => p.UniqueIdentifier)
                                                  .With(p => p.DeliveryMethod)
                                                  .With(p => p.LocationUniqueIdentifier, locationIdenfier)
            );

            var import = fixture.Create<Import>();

            var serialized = import.ToXml();

            XmlSchemas.XmlImport.ValidateDocument(serialized);

            Console.Write(serialized);
        }

        [Test]
        public void Event_with_additionalInfo_can_be_serialized()
        {
            fixture.Customize<DistanceEvent>(o => o
                                                  .OmitAutoProperties()
                                                  .With(p => p.UniqueIdentifier)
                                                  .With(p => p.DeliveryMethod)
                                                  .With(p => p.AdditionalInfo)
            );

            var import = fixture.Create<Import>();

            var serialized = import.ToXml();

            XmlSchemas.XmlImport.ValidateDocument(serialized);

            Console.Write(serialized);
        }

        [Test]
        public void Event_with_applicationInfo_can_be_serialized()
        {
            fixture.Customize<EventApplicationNode>(o => o.Without(p => p.UrlField));

            fixture.Customize<DistanceEvent>(o => o
                                                  .OmitAutoProperties()
                                                  .With(p => p.UniqueIdentifier)
                                                  .With(p => p.DeliveryMethod)
                                                  .With(p => p.Application)
            );

            var import = fixture.Create<Import>();

            var serialized = import.ToXml();

            XmlSchemas.XmlImport.ValidateDocument(serialized);

            Console.Write(serialized);
        }

        [Test]
        public void Event_with_applicationInfo_with_no_dates_can_be_serialized()
        {
            fixture.Customize<EventApplicationNode>(o => o
                .OmitAutoProperties()
                .With(p => p.Url)
                .With(p => p.ApplicationCode)
            );

            fixture.Customize<DistanceEvent>(o => o
                                                  .OmitAutoProperties()
                                                  .With(p => p.UniqueIdentifier)
                                                  .With(p => p.DeliveryMethod)
                                                  .With(p => p.Application)
            );

            var import = fixture.Create<Import>();

            var serialized = import.ToXml();

            XmlSchemas.XmlImport.ValidateDocument(serialized);

            Console.Write(serialized);
        }

        [Test]
        public void Event_with_applicationInfo_with_start_date_can_be_serialized()
        {
            fixture.Customize<EventApplicationNode>(o => o
                                                         .OmitAutoProperties()
                                                         .With(p => p.Url)
                                                         .With(p => p.ApplicationCode)
                                                         .With(p => p.StartDate)
            );

            fixture.Customize<DistanceEvent>(o => o
                                                  .OmitAutoProperties()
                                                  .With(p => p.UniqueIdentifier)
                                                  .With(p => p.DeliveryMethod)
                                                  .With(p => p.Application)
            );

            var import = fixture.Create<Import>();

            var serialized = import.ToXml();

            XmlSchemas.XmlImport.ValidateDocument(serialized);

            Console.Write(serialized);
        }

        [Test]
        public void Event_with_applicationInfo_with_end_date_can_be_serialized()
        {
            fixture.Customize<EventApplicationNode>(o => o
                                                         .OmitAutoProperties()
                                                         .With(p => p.Url)
                                                         .With(p => p.ApplicationCode)
                                                         .With(p => p.EndDate)
            );

            fixture.Customize<DistanceEvent>(o => o
                                                  .OmitAutoProperties()
                                                  .With(p => p.UniqueIdentifier)
                                                  .With(p => p.DeliveryMethod)
                                                  .With(p => p.Application)
            );

            var import = fixture.Create<Import>();

            var serialized = import.ToXml();

            XmlSchemas.XmlImport.ValidateDocument(serialized);

            Console.Write(serialized);
        }

        [Test]
        public void Event_with_flags_can_be_serialized()
        {
            fixture.Customize<DistanceEvent>(o => o
                                                  .OmitAutoProperties()
                                                  .With(p => p.UniqueIdentifier)
                                                  .With(p => p.DeliveryMethod)
                                                  .With(p => p.Flags)
            );

            var import = fixture.Create<Import>();

            var serialized = import.ToXml();

            XmlSchemas.XmlImport.ValidateDocument(serialized);

            Console.Write(serialized);
        }

        [Test]
        public void Event_with_flags_with_no_dates_can_be_serialized()
        {
            fixture.Customize<FlagNode>(o => o
                .OmitAutoProperties()
                .With(p => p.Name)
            );

            fixture.Customize<DistanceEvent>(o => o
                                                  .OmitAutoProperties()
                                                  .With(p => p.UniqueIdentifier)
                                                  .With(p => p.DeliveryMethod)
                                                  .With(p => p.Flags)
            );

            var import = fixture.Create<Import>();

            var serialized = import.ToXml();

            XmlSchemas.XmlImport.ValidateDocument(serialized);

            Console.Write(serialized);
        }

        [Test]
        public void Event_with_flags_with_start_dates_can_be_serialized()
        {
            fixture.Customize<FlagNode>(o => o
                                             .OmitAutoProperties()
                                             .With(p => p.Name)
                                             .With(p => p.StartDate)
            );

            fixture.Customize<DistanceEvent>(o => o
                                                  .OmitAutoProperties()
                                                  .With(p => p.UniqueIdentifier)
                                                  .With(p => p.DeliveryMethod)
                                                  .With(p => p.Flags)
            );

            var import = fixture.Create<Import>();

            var serialized = import.ToXml();

            XmlSchemas.XmlImport.ValidateDocument(serialized);

            Console.Write(serialized);
        }

        [Test]
        public void Event_with_flags_with_end_dates_can_be_serialized()
        {
            fixture.Customize<FlagNode>(o => o
                                             .OmitAutoProperties()
                                             .With(p => p.Name)
                                             .With(p => p.EndDate)
            );

            fixture.Customize<DistanceEvent>(o => o
                                                  .OmitAutoProperties()
                                                  .With(p => p.UniqueIdentifier)
                                                  .With(p => p.DeliveryMethod)
                                                  .With(p => p.Flags)
            );

            var import = fixture.Create<Import>();

            var serialized = import.ToXml();

            XmlSchemas.XmlImport.ValidateDocument(serialized);

            Console.Write(serialized);
        }

        [Test]
        public void Event_with_pace_can_be_serialized()
        {
            fixture.Customize<DistanceEvent>(o => o
                                                  .OmitAutoProperties()
                                                  .With(p => p.UniqueIdentifier)
                                                  .With(p => p.DeliveryMethod)
                                                  .With(p => p.Pace)
            );

            var import = fixture.Create<Import>();

            var serialized = import.ToXml();

            XmlSchemas.XmlImport.ValidateDocument(serialized);

            Console.Write(serialized);
        }

        [Test]
        public void Event_with_language_can_be_serialized()
        {
            fixture.Customize<DistanceEvent>(o => o
                                                  .OmitAutoProperties()
                                                  .With(p => p.UniqueIdentifier)
                                                  .With(p => p.DeliveryMethod)
                                                  .With(p => p.Language)
            );

            var import = fixture.Create<Import>();

            var serialized = import.ToXml();

            XmlSchemas.XmlImport.ValidateDocument(serialized);

            Console.Write(serialized);
        }

        [Test]
        public void Event_with_text_lastApplicationDate_can_be_serialized()
        {
            fixture.Customize<LastApplicationDateNode>(o => o.FromFactory((string text) => new LastApplicationDateNode(text)).OmitAutoProperties());

            fixture.Customize<DistanceEvent>(o => o
                                                  .OmitAutoProperties()
                                                  .With(p => p.UniqueIdentifier)
                                                  .With(p => p.DeliveryMethod)
                                                  .With(p => p.LastApplicationDate)
            );

            var import = fixture.Create<Import>();

            var serialized = import.ToXml();

            XmlSchemas.XmlImport.ValidateDocument(serialized);

            Console.Write(serialized);
        }

        [Test]
        public void Event_with_date_lastApplicationDate_can_be_serialized()
        {
            fixture.Customize<LastApplicationDateNode>(o => o.FromFactory((DateTime dateTime) => new LastApplicationDateNode(dateTime)).OmitAutoProperties());

            fixture.Customize<DistanceEvent>(o => o
                                                  .OmitAutoProperties()
                                                  .With(p => p.UniqueIdentifier)
                                                  .With(p => p.DeliveryMethod)
                                                  .With(p => p.LastApplicationDate)
            );

            var import = fixture.Create<Import>();

            var serialized = import.ToXml();

            XmlSchemas.XmlImport.ValidateDocument(serialized);

            Console.Write(serialized);
        }
    }
}