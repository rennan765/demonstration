using _4oito6.Demonstration.Domain.Model.Entities;
using _4oito6.Demonstration.Domain.Model.Enum;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace _4oito6.Demonstration.Test.Domain.Model.Entities
{
    [Trait("Person", "Domain Model tests")]
    public class PersonTest
    {
        [Fact(DisplayName = "Attempting do create a new Person with invalid entries.")]
        public void Validate_Failure()
        {
            var person = new Person
            (
                name: string.Empty,
                email: "not valid e-mail",

                document: "not valid document",
                gender: Gender.NotInformed,

                birthDate: DateTime.UtcNow,
                phones: Array.Empty<Phone>()
            );

            person.IsValid.Should().BeFalse();
        }

        [Fact(DisplayName = "Person created successfully.")]
        public void ValidateToCreate_Success()
        {
            var phone = new Phone(PhoneType.Home, "21", "27172770");

            var person = new Person
            (
                name: "Fulano de Tal",
                email: "my@gmail.com",

                document: "49843479025",
                gender: Gender.NotInformed,

                birthDate: DateTime.UtcNow.AddYears(-20),
                phones: new[] { phone },

                mainPhone: phone,
                address: new Address
                (
                    street: "Avenida Rio Branco",

                    number: "156",
                    complement: null,

                    district: "Centro",
                    city: "Rio de Janeiro",

                    state: "RJ",
                    postalCode: "20040901"
                )
            );

            person.ValidateToCreate().Should().BeTrue();
        }

        [Fact(DisplayName = "Person updated successfully.")]
        public void ValidateToUpdate_Success()
        {
            var phone = new Phone(PhoneType.Home, "21", "27172770");

            var person = new Person
            (
                id: 50,
                name: "Fulano de Tal",
                email: "my@gmail.com",

                document: "49843479025",
                gender: Gender.NotInformed,

                birthDate: DateTime.UtcNow.AddYears(-20),
                phones: new[] { phone },

                mainPhone: phone,
                address: new Address
                (
                    street: "Avenida Rio Branco",

                    number: "156",
                    complement: null,

                    district: "Centro",
                    city: "Rio de Janeiro",

                    state: "RJ",
                    postalCode: "20040901"
                )
            );

            person.ValidateToUpdate().Should().BeTrue();
        }

        [Fact(DisplayName = "Clearing Person's phones.")]
        public void ClearPhones_Success()
        {
            //arrange:
            var phone = new Phone(PhoneType.Home, "21", "27172770");
            var person = new Person
            (
                id: 50,
                name: "Fulano de Tal",
                email: "my@gmail.com",

                document: "49843479025",
                gender: Gender.NotInformed,

                birthDate: DateTime.UtcNow.AddYears(-20),
                phones: new[] { phone },

                mainPhone: phone,
                address: new Address
                (
                    street: "Avenida Rio Branco",

                    number: "156",
                    complement: null,

                    district: "Centro",
                    city: "Rio de Janeiro",

                    state: "RJ",
                    postalCode: "20040901"
                )
            );

            //act:
            person.ClearPhones();

            //assert:
            person.Phones.Any().Should().BeFalse();
            person.MainPhone.Should().BeNull();
        }

        [Fact(DisplayName = "Clearing Person's phones.")]
        public void ClearAddress_Success()
        {
            //arrange:
            var phone = new Phone(PhoneType.Home, "21", "27172770");
            var person = new Person
            (
                id: 50,
                name: "Fulano de Tal",
                email: "my@gmail.com",

                document: "49843479025",
                gender: Gender.NotInformed,

                birthDate: DateTime.UtcNow.AddYears(-20),
                phones: new[] { phone },

                mainPhone: phone,
                address: new Address
                (
                    street: "Avenida Rio Branco",

                    number: "156",
                    complement: null,

                    district: "Centro",
                    city: "Rio de Janeiro",

                    state: "RJ",
                    postalCode: "20040901"
                )
            );

            //act:
            person.ClearAddress();

            //assert:
            person.Address.Should().BeNull();
        }

        [Fact(DisplayName = "Handling null Address while attempting to attach.")]
        public void AttachAddress_ArgumentNullException()
        {
            //arrange:
            var phone = new Phone(PhoneType.Home, "21", "27172770");
            var person = new Person
            (
                id: 50,
                name: "Fulano de Tal",
                email: "my@gmail.com",

                document: "49843479025",
                gender: Gender.NotInformed,

                birthDate: DateTime.UtcNow.AddYears(-20),
                phones: new[] { phone },
                mainPhone: phone
            );

            //act and assert:
            person.Invoking(p => p.Attach((Address)null))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact(DisplayName = "Attaching new Address successfully.")]
        public void AttachAddress_Success()
        {
            //arrange:
            var phone = new Phone(PhoneType.Home, "21", "27172770");
            var person = new Person
            (
                id: 50,
                name: "Fulano de Tal",
                email: "my@gmail.com",

                document: "49843479025",
                gender: Gender.NotInformed,

                birthDate: DateTime.UtcNow.AddYears(-20),
                phones: new[] { phone },
                mainPhone: phone
            );

            var address = new Address
            (
                street: "Avenida Rio Branco",

                number: "156",
                complement: null,

                district: "Centro",
                city: "Rio de Janeiro",

                state: "RJ",
                postalCode: "20040901"
            );

            //act:
            person.Attach(address);

            //assert:
            person.Address.Match(address).Should().BeTrue();
        }

        [Fact(DisplayName = "Handling null Phone while attempting to attach.")]
        public void AttachPhone_ArgumentNullException()
        {
            //arrange:
            var phone = new Phone(PhoneType.Home, "21", "27172770");
            var person = new Person
            (
                id: 50,
                name: "Fulano de Tal",
                email: "my@gmail.com",

                document: "49843479025",
                gender: Gender.NotInformed,

                birthDate: DateTime.UtcNow.AddYears(-20),
                phones: new[] { phone },
                mainPhone: phone
            );

            //act and assert:
            person.Invoking(p => p.Attach((Phone)null))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact(DisplayName = "Handling null Phone collection while attempting to attach.")]
        public void AttachPhones_ArgumentNullException()
        {
            //arrange:
            var phone = new Phone(PhoneType.Home, "21", "27172770");
            var person = new Person
            (
                id: 50,
                name: "Fulano de Tal",
                email: "my@gmail.com",

                document: "49843479025",
                gender: Gender.NotInformed,

                birthDate: DateTime.UtcNow.AddYears(-20),
                phones: new[] { phone },
                mainPhone: phone
            );

            //act and assert:
            person.Invoking(p => p.Attach((IEnumerable<Phone>)null))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact(DisplayName = "Attaching no new Phone due to empty collection.")]
        public void AttachPhone_NoNewPhone()
        {
            //arrange:
            var phone = new Phone(PhoneType.Home, "21", "27172770");
            var person = new Person
            (
                id: 50,
                name: "Fulano de Tal",
                email: "my@gmail.com",

                document: "49843479025",
                gender: Gender.NotInformed,

                birthDate: DateTime.UtcNow.AddYears(-20),
                phones: new[] { phone },
                mainPhone: phone
            );

            var previousCount = person.Phones.Count();

            //act:
            person.Attach(Array.Empty<Phone>());

            //assert:
            person.Phones.Count().Should().Be(previousCount);
        }

        [Fact(DisplayName = "Attaching new Phone successfully.")]
        public void AttachPhone_Success()
        {
            //arrange:
            var phone = new Phone(PhoneType.Home, "21", "27172770");
            var person = new Person
            (
                id: 50,
                name: "Fulano de Tal",
                email: "my@gmail.com",

                document: "49843479025",
                gender: Gender.NotInformed,

                birthDate: DateTime.UtcNow.AddYears(-20),
                phones: new[] { phone },
                mainPhone: phone
            );

            var newPhone = new Phone(PhoneType.Business, "21", "26287325");

            //act:
            person.Attach(newPhone, isMainPhone: true);

            //assert:
            person.Phones.Any(p => p.ToString().Equals(newPhone.ToString())).Should().BeTrue();
            person.MainPhone.ToString().Should().Be(newPhone.ToString());
        }
    }
}