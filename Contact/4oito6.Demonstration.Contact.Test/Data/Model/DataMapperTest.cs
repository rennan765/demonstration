using _4oito6.Demonstration.Application.Model;
using _4oito6.Demonstration.Contact.Data.Model;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using System.Collections.Generic;
using Xunit;

namespace _4oito6.Demonstration.Contact.Test.Data.Model
{
    [Trait("DataMapper", "Data Mapper tests")]
    public class DataMapperTest
    {
        [Fact(DisplayName = "Attempting to converto to a Bulk Dictionary.")]
        public void ToBulkDictionary_Success()
        {
            //arrange:
            var model = new Notification("Email", "Invalid");

            var expectedResult = new Dictionary<string, object>
            {
                { nameof(model.Name), model.Name },
                { nameof(model.Message), model.Message }
            };

            //act:
            var result = model.ToBulkDictionary();

            //assert:
            new CompareLogic().Compare(expectedResult, result).AreEqual.Should().BeTrue();
        }
    }
}